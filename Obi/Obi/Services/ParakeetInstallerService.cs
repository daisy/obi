using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class ParakeetInstallerService
    {
        // --------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------

        private const string RequiredPythonVersion = "3.11.9";

        private const string PythonInstallerUrl =
            "https://www.python.org/ftp/python/3.11.9/" +
            "python-3.11.9-amd64.exe";

        // Microsoft permanent endpoint for the latest supported
        // Visual C++ v14 x64 Redistributable.
        private const string VCRedistInstallerUrl =
            "https://aka.ms/vc14/vc_redist.x64.exe";

        private const string VCRedistRegistryPath =
            @"SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\x64";

        private static readonly SemaphoreSlim InstallLock = new(1, 1);

        // --------------------------------------------------
        // PATHS
        // --------------------------------------------------

        public static string GetVenvPath() =>
            ObiPaths.ParakeetEnvironment;

        public static string GetPythonExe() =>
            ObiPaths.ParakeetPythonExe;

        private static string GetInstalledPythonExe()
        {
            string localAppData =
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData);

            return Path.Combine(
                localAppData,
                "Programs",
                "Python",
                "Python311",
                "python.exe");
        }

        private static string GetPythonInstallerPath() =>
            Path.Combine(
                ObiPaths.PythonInstallerFolder,
                $"python-{RequiredPythonVersion}-amd64.exe");

        private static string GetVCRedistInstallerPath() =>
            Path.Combine(
                ObiPaths.LocalDataFolder,
                "VCRedistInstaller",
                "vc_redist.x64.exe");

        private static string GetRequirementsStampPath() =>
            Path.Combine(
                ObiPaths.ParakeetEnvironment,
                ".obi_parakeet_requirements.sha256");

        // --------------------------------------------------
        // PUBLIC READY / HEALTH CHECK
        // --------------------------------------------------

        /// <summary>
        /// Makes sure everything required by the Parakeet transcription
        /// backend is present and healthy. If the environment is damaged
        /// or out of date, it repairs it automatically. If a repair still
        /// fails, the virtual environment is recreated from scratch.
        /// </summary>
        public static async Task EnsureParakeetReadyAsync(
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default)
        {
            await InstallLock.WaitAsync(cancellationToken);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Directory.CreateDirectory(
                    ObiPaths.LocalDataFolder);

                Directory.CreateDirectory(
                    ObiPaths.ModelsFolder);

                Directory.CreateDirectory(
                    ObiPaths.HuggingFaceFolder);


                // --------------------------------------------------
                // CHECK REQUIREMENTS FILE
                // --------------------------------------------------

                if (!File.Exists(
                        ObiPaths.ParakeetRequirements))
                {
                    throw new FileNotFoundException(
                        "Parakeet requirements file was not found.",
                        ObiPaths.ParakeetRequirements);
                }


                // --------------------------------------------------
                // VISUAL C++ RUNTIME
                // --------------------------------------------------

                await EnsureVCRuntimeAsync(
                    progress,
                    cancellationToken);


                // --------------------------------------------------
                // SYSTEM PYTHON
                //
                // IMPORTANT:
                // This Python is used ONLY to create the virtual
                // environment.
                //
                // We must NOT use this Python for pip installation.
                // --------------------------------------------------

                string systemPythonExe =
                    await EnsurePythonInstalledAsync(
                        progress,
                        cancellationToken);


                // --------------------------------------------------
                // MAKE SURE PARakeet VIRTUAL ENVIRONMENT EXISTS
                // --------------------------------------------------

                string venvPythonExe =
                    GetPythonExe();


                if (!File.Exists(venvPythonExe))
                {
                    progress?.Report(
                        "Parakeet virtual environment is missing.");


                    // An incomplete/corrupt venv directory may already
                    // exist. Remove it before creating a clean one.
                    if (Directory.Exists(
                            ObiPaths.ParakeetEnvironment))
                    {
                        progress?.Report(
                            "Removing incomplete Parakeet environment...");

                        DeleteDirectorySafely(
                            ObiPaths.ParakeetEnvironment,
                            progress);
                    }


                    progress?.Report(
                        "Creating Parakeet virtual environment...");


                    cancellationToken.ThrowIfCancellationRequested();


                    await RunProcess(
                        systemPythonExe,
                        $"-m venv \"{ObiPaths.ParakeetEnvironment}\"",
                        progress,
                        cancellationToken);


                    venvPythonExe =
                        GetPythonExe();


                    if (!File.Exists(venvPythonExe))
                    {
                        throw new Exception(
                            "Parakeet virtual environment was not created successfully.");
                    }
                }


                // --------------------------------------------------
                // CHECK CURRENT ENVIRONMENT
                // --------------------------------------------------

                if (await IsEnvironmentHealthyAsync(
                        progress,
                        cancellationToken))
                {
                    progress?.Report(
                        "Parakeet environment is ready.");

                    return;
                }


                progress?.Report(
                    "Parakeet environment needs repair.");


                // --------------------------------------------------
                // FIRST REPAIR ATTEMPT
                //
                // IMPORTANT:
                // From this point onward we ALWAYS use the venv
                // Python. Never use systemPythonExe for pip.
                // --------------------------------------------------

                try
                {
                    await RepairEnvironmentAsync(
                        venvPythonExe,
                        progress,
                        cancellationToken);


                    if (await IsEnvironmentHealthyAsync(
                            progress,
                            cancellationToken))
                    {
                        progress?.Report(
                            "Parakeet environment repaired successfully.");

                        return;
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    progress?.Report(
                        "Parakeet repair attempt failed.");

                    progress?.Report(
                        GetShortExceptionMessage(ex));
                }


                // --------------------------------------------------
                // LAST RESORT
                //
                // Recreate the entire venv.
                //
                // The base/system Python installation remains.
                // Only the Parakeet venv is deleted.
                // --------------------------------------------------

                progress?.Report(
                    "Recreating the Parakeet environment from scratch...");


                cancellationToken.ThrowIfCancellationRequested();


                DeleteDirectorySafely(
                    ObiPaths.ParakeetEnvironment,
                    progress);


                Directory.CreateDirectory(
                    ObiPaths.ParakeetEnvironment);


                cancellationToken.ThrowIfCancellationRequested();


                // IMPORTANT:
                // System Python is used ONLY for "python -m venv".
                await RunProcess(
                    systemPythonExe,
                    $"-m venv \"{ObiPaths.ParakeetEnvironment}\"",
                    progress,
                    cancellationToken);


                venvPythonExe =
                    GetPythonExe();


                if (!File.Exists(venvPythonExe))
                {
                    throw new Exception(
                        "Parakeet virtual environment was not created successfully.");
                }


                // --------------------------------------------------
                // INSTALL PACKAGES INTO THE VENV ONLY
                // --------------------------------------------------

                await RepairEnvironmentAsync(
                    venvPythonExe,
                    progress,
                    cancellationToken);


                // --------------------------------------------------
                // FINAL VERIFICATION
                // --------------------------------------------------

                if (!await IsEnvironmentHealthyAsync(
                        progress,
                        cancellationToken))
                {
                    throw new Exception(
                        "Parakeet environment could not be repaired automatically. " +
                        "See the installation log above for the failed dependency or command.");
                }


                progress?.Report(
                    "Parakeet environment installed and verified successfully.");
            }
            finally
            {
                InstallLock.Release();
            }
        }

        /// <summary>
        /// Compatibility method for existing callers. It now performs a
        /// real health check instead of merely checking for python.exe.
        /// </summary>
        public static async Task<bool>
            IsPythonEnvironmentInstalledAsync()
        {
            try
            {
                if (!File.Exists(GetPythonExe()))
                    return false;

                if (!File.Exists(ObiPaths.ParakeetRequirements))
                    return false;

                return await IsEnvironmentHealthyAsync(
                    progress: null,
                    cancellationToken: default);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Compatibility method for older code paths. New code should use
        /// EnsureParakeetReadyAsync because it repairs damaged installations.
        /// </summary>
        public static async Task InstallAsync(
            IProgress<string>? progress = null)
        {
            await EnsureParakeetReadyAsync(progress);
        }

        // --------------------------------------------------
        // ENVIRONMENT HEALTH
        // --------------------------------------------------

        private static async Task<bool> IsEnvironmentHealthyAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(GetPythonExe()))
            {
                progress?.Report(
                    "Parakeet virtual environment Python executable is missing.");
                return false;
            }

            if (!File.Exists(ObiPaths.ParakeetRequirements))
                return false;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress?.Report(
                    "Checking Parakeet Python environment...");

                // Make sure the venv really runs the expected Python.
                string versionOutput =
                    await RunProcessForOutput(
                        GetPythonExe(),
                        "--version",
                        cancellationToken);

                if (!versionOutput.Contains(
                        $"Python {RequiredPythonVersion}",
                        StringComparison.OrdinalIgnoreCase))
                {
                    progress?.Report(
                        $"Unexpected Parakeet Python version: {versionOutput.Trim()}");
                    return false;
                }

                // pip check catches dependency conflicts that a simple
                // import test can miss.
                await RunProcess(
                    GetPythonExe(),
                    "-m pip check",
                    progress,
                    cancellationToken);

                string expectedVersions =
                    BuildExpectedVersionsPythonDictionary();

                string healthCode =
                    "import importlib.metadata as md; " +
                    $"expected={expectedVersions}; " +
                    "wrong=[f'{name}=={md.version(name)} (expected {ver})' for name,ver in expected.items() if md.version(name) != ver]; " +
                    "assert not wrong, '; '.join(wrong); " +
                    "import torch, torchaudio, librosa, transformers; " +
                    "from transformers import AutoProcessor, AutoModel; " +
                    "print('Parakeet dependencies OK'); " +
                    "print('Torch:', torch.__version__); " +
                    "print('TorchAudio:', torchaudio.__version__); " +
                    "print('Transformers:', transformers.__version__); " +
                    "print('Librosa:', librosa.__version__)";

                await RunProcess(
                    GetPythonExe(),
                    $"-c \"{EscapeForProcessArgument(healthCode)}\"",
                    progress,
                    cancellationToken);

                string currentStamp =
                    await CalculateFileSha256Async(
                        ObiPaths.ParakeetRequirements,
                        cancellationToken);

                string storedStamp =
                    File.Exists(GetRequirementsStampPath())
                        ? (await File.ReadAllTextAsync(
                            GetRequirementsStampPath(),
                            cancellationToken)).Trim()
                        : string.Empty;

                if (!string.Equals(
                        currentStamp,
                        storedStamp,
                        StringComparison.OrdinalIgnoreCase))
                {
                    progress?.Report(
                        "Parakeet requirements have changed; package repair is required.");
                    return false;
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                progress?.Report(
                    "Parakeet environment health check failed.");
                progress?.Report(
                    GetShortExceptionMessage(ex));
                return false;
            }
        }

        private static string BuildExpectedVersionsPythonDictionary()
        {
            var packages = ReadPinnedRequirements();

            var entries = packages.Select(
                p =>
                    $"'{p.Name.Replace("\\", "\\\\").Replace("'", "\\'")}':" +
                    $"'{p.Version.Replace("\\", "\\\\").Replace("'", "\\'")}'");

            return "{" + string.Join(",", entries) + "}";
        }

        private sealed record PinnedRequirement(
            string Name,
            string Version);

        private static List<PinnedRequirement> ReadPinnedRequirements()
        {
            var result = new List<PinnedRequirement>();

            foreach (string rawLine in
                     File.ReadAllLines(ObiPaths.ParakeetRequirements))
            {
                string line = rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("#", StringComparison.Ordinal))
                {
                    continue;
                }

                int separator = line.IndexOf("==", StringComparison.Ordinal);

                if (separator <= 0 || separator >= line.Length - 2)
                {
                    // The current production requirements file uses exact
                    // == pins. Do not silently claim a non-pinned dependency
                    // is verified.
                    continue;
                }

                string name = line[..separator].Trim();
                string version = line[(separator + 2)..].Trim();

                int comment = version.IndexOf('#');
                if (comment >= 0)
                    version = version[..comment].Trim();

                if (!string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(version))
                {
                    result.Add(
                        new PinnedRequirement(name, version));
                }
            }

            return result;
        }

        // --------------------------------------------------
        // REPAIR / INSTALL PACKAGES
        // --------------------------------------------------

        private static async Task RepairEnvironmentAsync(
            string pythonExe,
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            progress?.Report(
                "Upgrading Parakeet pip...");

            await RunProcess(
                pythonExe,
                "-m pip install --upgrade pip",
                progress,
                cancellationToken);

            progress?.Report(
                @"Installing / repairing Parakeet packages...

This may take several minutes.

The Parakeet AI environment is being
prepared for offline transcription.");

            await RunProcess(
                pythonExe,
                $"-m pip install -r \"{ObiPaths.ParakeetRequirements}\"",
                progress,
                cancellationToken);

            // Write the requirements stamp only after pip installation has
            // completed. A failed install therefore remains repairable on
            // the next launch.
            string stamp =
                await CalculateFileSha256Async(
                    ObiPaths.ParakeetRequirements,
                    cancellationToken);

            Directory.CreateDirectory(
                ObiPaths.ParakeetEnvironment);

            await File.WriteAllTextAsync(
                GetRequirementsStampPath(),
                stamp,
                cancellationToken);
        }

        // --------------------------------------------------
        // PYTHON
        // --------------------------------------------------

        private static async Task<string> EnsurePythonInstalledAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            progress?.Report(
                "Checking Python installation...");

            string pythonExe = GetInstalledPythonExe();

            progress?.Report(
                $"Checking: {pythonExe}");

            if (File.Exists(pythonExe))
            {
                try
                {
                    string version =
                        await RunProcessForOutput(
                            pythonExe,
                            "--version",
                            cancellationToken);

                    if (version.Contains(
                            $"Python {RequiredPythonVersion}",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        progress?.Report(
                            $"Python {RequiredPythonVersion} found.");
                        return pythonExe;
                    }

                    progress?.Report(
                        $"Installed Python version is not {RequiredPythonVersion}: {version.Trim()}");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    progress?.Report(
                        "Existing Python installation could not be started; it will be repaired.");
                }
            }
            else
            {
                progress?.Report(
                    $"Python {RequiredPythonVersion} not found. " +
                    "Preparing automatic installation...");
            }

            await DownloadPythonInstallerAsync(
                progress,
                cancellationToken,
                forceRedownload: true);

            await InstallPythonAsync(
                progress,
                cancellationToken);

            progress?.Report(
                "Verifying Python installation...");

            for (int i = 0; i < 15; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (File.Exists(pythonExe))
                {
                    try
                    {
                        string version =
                            await RunProcessForOutput(
                                pythonExe,
                                "--version",
                                cancellationToken);

                        if (version.Contains(
                                $"Python {RequiredPythonVersion}",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            progress?.Report(
                                $"Python {RequiredPythonVersion} installed successfully.");
                            return pythonExe;
                        }
                    }
                    catch
                    {
                        // Installer may still be finishing. Retry below.
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    cancellationToken);
            }

            throw new Exception(
                $"Python {RequiredPythonVersion} installation failed or the expected " +
                $"Python executable was not found at:\n{pythonExe}");
        }

        // --------------------------------------------------
        // PYTHON DOWNLOAD / INSTALL
        // --------------------------------------------------

        private static async Task DownloadPythonInstallerAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken,
            bool forceRedownload)
        {
            string installerPath = GetPythonInstallerPath();

            if (forceRedownload && File.Exists(installerPath))
            {
                try
                {
                    File.Delete(installerPath);
                }
                catch
                {
                    // If deletion fails, downloading to a temporary file and
                    // replacing it below will still give us a clean attempt.
                }
            }

            if (File.Exists(installerPath) &&
                new FileInfo(installerPath).Length > 1024 * 1024)
            {
                progress?.Report(
                    "Python installer already downloaded.");

                return;
            }

            string? directory =
                Path.GetDirectoryName(installerPath);

            if (directory == null)
                throw new Exception(
                    "Could not determine Python installer directory.");

            Directory.CreateDirectory(directory);

            string tempPath = installerPath + ".download";

            try
            {
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                progress?.Report(
                    "Downloading Python installer...");

                using HttpClient client = new()
                {
                    Timeout = TimeSpan.FromMinutes(15)
                };

                using (HttpResponseMessage response =
                       await client.GetAsync(
                           PythonInstallerUrl,
                           HttpCompletionOption.ResponseHeadersRead,
                           cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    await using Stream input =
                        await response.Content.ReadAsStreamAsync(
                            cancellationToken);

                    await using (FileStream output =
                        new(
                            tempPath,
                            FileMode.Create,
                            FileAccess.Write,
                            FileShare.None))
                    {
                        await input.CopyToAsync(
                            output,
                            cancellationToken);

                        await output.FlushAsync(
                            cancellationToken);
                    }
                }

                // The HTTP response and file stream are now completely
                // disposed, so the temporary file is no longer locked.

                if (new FileInfo(tempPath).Length < 1024 * 1024)
                {
                    throw new Exception(
                        "Downloaded Python installer appears to be incomplete.");
                }

                File.Move(
                    tempPath,
                    installerPath,
                    overwrite: true);

                progress?.Report(
                    "Python installer downloaded.");
            }
            finally
            {
                if (File.Exists(tempPath))
                {
                    try
                    {
                        File.Delete(tempPath);
                    }
                    catch
                    {
                        // Best effort cleanup.
                    }
                }
            }
        }

        private static async Task InstallPythonAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            progress?.Report(
                "Installing Python 3.11...");

            string installer = GetPythonInstallerPath();

            await RunInstaller(
                installer,
                "/quiet InstallAllUsers=0 " +
                "PrependPath=1 Include_launcher=1",
                progress,
                cancellationToken);

            progress?.Report(
                "Python installation completed.");
        }

        // --------------------------------------------------
        // VC++ RUNTIME
        // --------------------------------------------------

        private static async Task EnsureVCRuntimeAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            progress?.Report(
                "Checking Microsoft Visual C++ runtime...");

            if (IsVCRuntimeInstalled())
            {
                progress?.Report(
                    "Microsoft Visual C++ runtime is already installed.");

                return;
            }

            progress?.Report(
                "Microsoft Visual C++ runtime is missing. " +
                "Preparing automatic installation...");

            string installerPath =
                GetVCRedistInstallerPath();

            string? directory =
                Path.GetDirectoryName(installerPath);

            if (directory == null)
                throw new Exception(
                    "Could not determine VC++ installer directory.");

            Directory.CreateDirectory(directory);

            bool needDownload =
                !File.Exists(installerPath) ||
                new FileInfo(installerPath).Length < 1024 * 1024;

            if (needDownload)
            {
                string tempPath =
                    installerPath + ".download";

                try
                {
                    if (File.Exists(tempPath))
                        File.Delete(tempPath);

                    progress?.Report(
                        "Downloading Microsoft Visual C++ runtime...");

                    using HttpClient client = new()
                    {
                        Timeout = TimeSpan.FromMinutes(15)
                    };

                    using (HttpResponseMessage response =
                           await client.GetAsync(
                               VCRedistInstallerUrl,
                               HttpCompletionOption.ResponseHeadersRead,
                               cancellationToken))
                    {
                        response.EnsureSuccessStatusCode();

                        await using Stream input =
                            await response.Content.ReadAsStreamAsync(
                                cancellationToken);

                        await using (FileStream output =
                            new(
                                tempPath,
                                FileMode.Create,
                                FileAccess.Write,
                                FileShare.None))
                        {
                            await input.CopyToAsync(
                                output,
                                cancellationToken);

                            await output.FlushAsync(
                                cancellationToken);
                        }
                    }

                    // The HTTP response and file stream are now completely
                    // disposed, so the temporary file is no longer locked.

                    if (new FileInfo(tempPath).Length < 1024 * 1024)
                    {
                        throw new Exception(
                            "Downloaded Microsoft Visual C++ installer appears to be incomplete.");
                    }

                    File.Move(
                        tempPath,
                        installerPath,
                        overwrite: true);

                    progress?.Report(
                        "Microsoft Visual C++ runtime downloaded.");
                }
                finally
                {
                    if (File.Exists(tempPath))
                    {
                        try
                        {
                            File.Delete(tempPath);
                        }
                        catch
                        {
                            // Best effort cleanup.
                        }
                    }
                }
            }
            else
            {
                progress?.Report(
                    "Microsoft Visual C++ installer already downloaded.");
            }

            cancellationToken.ThrowIfCancellationRequested();

            progress?.Report(
                "Installing Microsoft Visual C++ runtime...");

            await RunInstaller(
                installerPath,
                "/install /quiet /norestart",
                progress,
                cancellationToken,
                allowedExitCodes: new[] { 0, 3010, 1638 });

            if (!IsVCRuntimeInstalled())
            {
                throw new Exception(
                    "Microsoft Visual C++ runtime installation completed, " +
                    "but the x64 runtime could not be verified in the registry. " +
                    "The installation may require administrator permission.");
            }

            progress?.Report(
                "Microsoft Visual C++ runtime installed successfully.");
        }

        private static bool IsVCRuntimeInstalled()
        {
            // Check the 64-bit registry view first, then the 32-bit view for
            // compatibility with how different Microsoft installers register
            // the v14 runtime.
            foreach (RegistryView view in new[]
                     { RegistryView.Registry64, RegistryView.Registry32 })
            {
                try
                {
                    using RegistryKey baseKey =
                        RegistryKey.OpenBaseKey(
                            RegistryHive.LocalMachine,
                            view);

                    using RegistryKey? key =
                        baseKey.OpenSubKey(VCRedistRegistryPath);

                    if (key == null)
                        continue;

                    object? installed = key.GetValue("Installed");
                    object? versionValue = key.GetValue("Version");

                    if (installed is int installedInt && installedInt != 1)
                        continue;

                    if (versionValue is string versionText)
                    {
                        string normalized =
                            versionText.Trim().TrimStart('v', 'V');

                        if (Version.TryParse(normalized, out Version? version) &&
                            version.Major >= 14)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // Registry access can fail on restricted machines.
                    // The installer will be attempted below rather than
                    // incorrectly claiming the runtime is present.
                }
            }

            return false;
        }

        // --------------------------------------------------
        // PROCESS HELPERS
        // --------------------------------------------------

        private static async Task RunProcess(
            string fileName,
            string arguments,
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            ProcessStartInfo psi = new()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new()
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            var output = new StringBuilder();
            var error = new StringBuilder();
            object outputLock = new();

            process.OutputDataReceived += (_, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data))
                    return;

                lock (outputLock)
                    output.AppendLine(e.Data);

                progress?.Report(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (string.IsNullOrWhiteSpace(e.Data))
                    return;

                lock (outputLock)
                    error.AppendLine(e.Data);

                progress?.Report(e.Data);
            };

            progress?.Report(
                $"> {Path.GetFileName(fileName)} {arguments}");

            if (!process.Start())
            {
                throw new Exception(
                    $"Failed to start process: {fileName}");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            try
            {
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(process);
                throw;
            }

            // Ensure the asynchronous output events have been drained.
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string diagnostic =
                    BuildProcessFailureMessage(
                        fileName,
                        arguments,
                        process.ExitCode,
                        output.ToString(),
                        error.ToString());

                throw new Exception(diagnostic);
            }
        }

        private static async Task<string> RunProcessForOutput(
            string fileName,
            string arguments,
            CancellationToken cancellationToken)
        {
            ProcessStartInfo psi = new()
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = new()
            {
                StartInfo = psi
            };

            if (!process.Start())
            {
                throw new Exception(
                    $"Failed to start process: {fileName}");
            }

            Task<string> stdoutTask =
                process.StandardOutput.ReadToEndAsync(cancellationToken);

            Task<string> stderrTask =
                process.StandardError.ReadToEndAsync(cancellationToken);

            try
            {
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(process);
                throw;
            }

            string stdout = await stdoutTask;
            string stderr = await stderrTask;

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    BuildProcessFailureMessage(
                        fileName,
                        arguments,
                        process.ExitCode,
                        stdout,
                        stderr));
            }

            return string.IsNullOrWhiteSpace(stdout)
                ? stderr
                : stdout;
        }

        private static async Task RunInstaller(
            string installer,
            string arguments,
            IProgress<string>? progress,
            CancellationToken cancellationToken,
            int[]? allowedExitCodes = null)
        {
            progress?.Report(
                $"> {Path.GetFileName(installer)} {arguments}");

            ProcessStartInfo psi = new()
            {
                FileName = installer,
                Arguments = arguments,
                UseShellExecute = true,
                WorkingDirectory =
                    Path.GetDirectoryName(installer) ??
                    ObiPaths.LocalDataFolder
            };

            using Process process =
                Process.Start(psi) ??
                throw new Exception(
                    $"Failed to start installer: {installer}");

            try
            {
                await process.WaitForExitAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(process);
                throw;
            }

            int[] successCodes =
                allowedExitCodes ?? new[] { 0 };

            if (!successCodes.Contains(process.ExitCode))
            {
                throw new Exception(
                    $"Installer failed:\n{installer} {arguments}\n\n" +
                    $"ExitCode={process.ExitCode}");
            }
        }

        private static void TryKillProcessTree(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
                // Best effort only. The original cancellation exception is
                // more useful to the caller than a secondary kill exception.
            }
        }

        private static string BuildProcessFailureMessage(
            string fileName,
            string arguments,
            int exitCode,
            string stdout,
            string stderr)
        {
            const int maxDiagnosticLength = 12000;

            string combined =
                string.Join(
                    Environment.NewLine,
                    new[]
                    {
                        stdout.Trim(),
                        stderr.Trim()
                    }.Where(s => !string.IsNullOrWhiteSpace(s)));

            if (combined.Length > maxDiagnosticLength)
            {
                combined =
                    combined[^maxDiagnosticLength..];
            }

            return
                $"Command failed:\n{fileName} {arguments}\n\n" +
                $"ExitCode={exitCode}\n\n" +
                (string.IsNullOrWhiteSpace(combined)
                    ? "No process output was captured."
                    : combined);
        }

        // --------------------------------------------------
        // UTILITY
        // --------------------------------------------------

        private static string EscapeForProcessArgument(string value)
        {
            // The health-check Python code intentionally contains no double
            // quotes, so escaping them here is sufficient for the current
            // -c "..." invocation.
            return value.Replace("\\", "\\\\").Replace("\"", "\\\"");
        }

        private static async Task<string> CalculateFileSha256Async(
            string filePath,
            CancellationToken cancellationToken)
        {
            await using FileStream stream =
                new(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

            using var sha256 =
                System.Security.Cryptography.SHA256.Create();

            byte[] hash =
                await sha256.ComputeHashAsync(
                    stream,
                    cancellationToken);

            return Convert.ToHexString(hash);
        }

        private static void DeleteDirectorySafely(
            string directory,
            IProgress<string>? progress)
        {
            if (!Directory.Exists(directory))
                return;

            try
            {
                Directory.Delete(
                    directory,
                    recursive: true);
            }
            catch (Exception ex)
            {
                progress?.Report(
                    "Could not remove the existing Parakeet environment. " +
                    "A restart may be required before automatic repair can continue.");

                throw new Exception(
                    "Failed to remove the existing Parakeet virtual environment.",
                    ex);
            }
        }

        private static string GetShortExceptionMessage(Exception ex)
        {
            string message = ex.Message.Trim();

            const int maxLength = 4000;

            if (message.Length > maxLength)
                message = message[^maxLength..];

            return message;
        }
    }
}
