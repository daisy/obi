using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class WhisperXInstallerService
    {
        // ============================================================
        // CONSTANTS
        // ============================================================

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

        // ============================================================
        // PATHS
        // ============================================================

        public static string GetVenvPath()
        {
            return ObiPaths.WhisperEnvironment;
        }

        public static string GetPythonExe()
        {
            return ObiPaths.PythonExe;
        }

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

        private static string GetPythonInstallerPath()
        {
            return Path.Combine(
                ObiPaths.PythonInstallerFolder,
                $"python-{RequiredPythonVersion}-amd64.exe");
        }

        private static string GetVCRedistInstallerPath()
        {
            return Path.Combine(
                ObiPaths.LocalDataFolder,
                "VCRedistInstaller",
                "vc_redist.x64.exe");
        }

        private static string GetRequirementsStampPath()
        {
            return Path.Combine(
                ObiPaths.WhisperEnvironment,
                ".obi_whisper_requirements.sha256");
        }

        // ============================================================
        // PUBLIC READY / HEALTH CHECK
        // ============================================================

        /// <summary>
        /// Makes sure the complete WhisperX environment is present and
        /// healthy.
        ///
        /// If the environment is damaged or the requirements have changed,
        /// packages are repaired automatically.
        ///
        /// If package repair fails, the WhisperX virtual environment is
        /// deleted and recreated from scratch.
        /// </summary>
        public static async Task EnsureWhisperReadyAsync(
            IProgress<string>? progress = null,
            CancellationToken cancellationToken = default)
        {
            progress?.Report("WhisperX readiness check started.");
            await InstallLock.WaitAsync(cancellationToken);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // --------------------------------------------------------
                // Required Obi folders
                // --------------------------------------------------------

                Directory.CreateDirectory(
                    ObiPaths.LocalDataFolder);

                Directory.CreateDirectory(
                    ObiPaths.ModelsFolder);

                Directory.CreateDirectory(
                    ObiPaths.HuggingFaceFolder);

                Directory.CreateDirectory(
                    ObiPaths.NltkDataFolder);

                // Install bundled NLTK tokenizer data before environment checks.
                EnsureBundledNltkData(progress);

                // --------------------------------------------------------
                // Requirements file
                // --------------------------------------------------------

                if (!File.Exists(ObiPaths.Requirements))
                {
                    throw new FileNotFoundException(
                        "WhisperX requirements file was not found.",
                        ObiPaths.Requirements);
                }

                // --------------------------------------------------------
                // Visual C++ Runtime
                // --------------------------------------------------------

                await EnsureVCRuntimeAsync(
                    progress,
                    cancellationToken);

                // --------------------------------------------------------
                // System Python
                // --------------------------------------------------------

                string systemPython =
                    await EnsurePythonInstalledAsync(
                        progress,
                        cancellationToken);

                // --------------------------------------------------------
                // WhisperX virtual environment
                // --------------------------------------------------------

                await EnsureVirtualEnvironmentAsync(
                    systemPython,
                    progress,
                    cancellationToken);

                // --------------------------------------------------------
                // Check existing environment
                // --------------------------------------------------------

                if (await IsEnvironmentHealthyAsync(
                        progress,
                        cancellationToken))
                {
                    progress?.Report(
                        "WhisperX environment is ready.");

                    return;
                }

                progress?.Report(
                    "WhisperX environment needs repair.");

                // --------------------------------------------------------
                // First attempt: repair existing venv
                // --------------------------------------------------------

                try
                {
                    await RepairEnvironmentAsync(
                        GetPythonExe(),
                        progress,
                        cancellationToken);

                    if (await IsEnvironmentHealthyAsync(
                            progress,
                            cancellationToken))
                    {
                        await SaveInstalledPackagesAsync(
                            GetPythonExe(),
                            cancellationToken);

                        progress?.Report(
                            "WhisperX environment repaired successfully.");

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
                        "WhisperX repair attempt failed.");

                    progress?.Report(
                        GetShortExceptionMessage(ex));
                }

                // --------------------------------------------------------
                // Last resort: recreate venv
                // --------------------------------------------------------

                progress?.Report(
                    "Recreating the WhisperX environment from scratch...");

                DeleteDirectorySafely(
                    ObiPaths.WhisperEnvironment,
                    progress);

                Directory.CreateDirectory(
                    ObiPaths.WhisperEnvironment);

                cancellationToken.ThrowIfCancellationRequested();

                await RunProcess(
                    systemPython,
                    $"-m venv \"{ObiPaths.WhisperEnvironment}\"",
                    progress,
                    cancellationToken);

                if (!File.Exists(GetPythonExe()))
                {
                    throw new Exception(
                        "WhisperX virtual environment was not created successfully.");
                }

                await RepairEnvironmentAsync(
                    GetPythonExe(),
                    progress,
                    cancellationToken);

                if (!await IsEnvironmentHealthyAsync(
                        progress,
                        cancellationToken))
                {
                    throw new Exception(
                        "WhisperX environment could not be repaired automatically. " +
                        "See the installation log above for the failed dependency or command.");
                }

                await SaveInstalledPackagesAsync(
                    GetPythonExe(),
                    cancellationToken);

                progress?.Report(
                    "WhisperX environment installed and verified successfully.");
            }
            finally
            {
                InstallLock.Release();
            }
        }

        // ============================================================
        // COMPATIBILITY METHODS
        // ============================================================

        /// <summary>
        /// Compatibility method used by existing Obi code.
        ///
        /// Unlike the old implementation, this performs a real health
        /// check instead of merely checking whether python.exe exists.
        /// </summary>
        public static async Task<bool>
            IsPythonEnvironmentInstalledAsync()
        {
            try
            {
                if (!File.Exists(GetPythonExe()))
                    return false;

                if (!File.Exists(ObiPaths.Requirements))
                    return false;

                EnsureBundledNltkData(progress: null);

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
        /// Compatibility method for existing callers.
        ///
        /// New code can call EnsureWhisperReadyAsync directly.
        /// </summary>
        public static async Task InstallAsync(
            IProgress<string>? progress = null)
        {
            await EnsureWhisperReadyAsync(
                progress,
                CancellationToken.None);
        }

        /// <summary>
        /// Optional overload for callers that already have a cancellation
        /// token.
        /// </summary>
        public static async Task InstallAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            await EnsureWhisperReadyAsync(
                progress,
                cancellationToken);
        }

        // ============================================================
        // VIRTUAL ENVIRONMENT
        // ============================================================

        private static async Task EnsureVirtualEnvironmentAsync(
            string systemPython,
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            string venvPython = GetPythonExe();

            if (File.Exists(venvPython))
            {
                progress?.Report(
                    "WhisperX virtual environment found.");

                return;
            }

            // If the directory exists but python.exe is missing, it is a
            // partially-created/corrupt environment. Remove it before
            // recreating it.
            if (Directory.Exists(GetVenvPath()))
            {
                progress?.Report(
                    "Existing WhisperX environment appears incomplete.");

                DeleteDirectorySafely(
                    GetVenvPath(),
                    progress);
            }

            progress?.Report(
                "Creating WhisperX virtual environment...");

            Directory.CreateDirectory(
                GetVenvPath());

            cancellationToken.ThrowIfCancellationRequested();

            await RunProcess(
                systemPython,
                $"-m venv \"{GetVenvPath()}\"",
                progress,
                cancellationToken);

            if (!File.Exists(venvPython))
            {
                throw new Exception(
                    "WhisperX virtual environment was not created successfully.");
            }

            progress?.Report(
                "WhisperX virtual environment created successfully.");
        }

        // ============================================================
        // ENVIRONMENT HEALTH
        // ============================================================

        private static async Task<bool> IsEnvironmentHealthyAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(GetPythonExe()))
            {
                progress?.Report(
                    "WhisperX virtual environment Python executable is missing.");

                return false;
            }

            if (!File.Exists(ObiPaths.Requirements))
            {
                progress?.Report(
                    "WhisperX requirements file is missing.");

                return false;
            }

            string? healthScriptPath = null;

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                progress?.Report(
                    "Checking WhisperX Python environment...");

                // --------------------------------------------------------
                // Verify Python version
                // --------------------------------------------------------

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
                        $"Unexpected WhisperX Python version: " +
                        $"{versionOutput.Trim()}");

                    return false;
                }

                // --------------------------------------------------------
                // pip dependency consistency
                // --------------------------------------------------------

                await RunProcess(
                    GetPythonExe(),
                    "-m pip check",
                    progress,
                    cancellationToken);

                // --------------------------------------------------------
                // Exact package versions + runtime imports
                // --------------------------------------------------------
                // IMPORTANT:
                // Do not put Python compound statements (for/try/except)
                // into a single `python -c` command. Python does not allow
                // those statements to be chained after semicolons in the
                // way the old health check generated them.
                //
                // Instead, create a temporary real Python script and execute
                // it with the WhisperX virtual-environment interpreter.
                // --------------------------------------------------------

                string healthScript =
                    BuildHealthCheckScript();

                string healthDirectory =
                    Path.Combine(
                        Path.GetTempPath(),
                        "Obi",
                        "WhisperXHealth");

                Directory.CreateDirectory(healthDirectory);

                healthScriptPath = Path.Combine(
                    healthDirectory,
                    $"health_{Guid.NewGuid():N}.py");

                await File.WriteAllTextAsync(
                    healthScriptPath,
                    healthScript,
                    new UTF8Encoding(false),
                    cancellationToken);

                await RunProcess(
                    GetPythonExe(),
                    $"\"{healthScriptPath}\"",
                    progress,
                    cancellationToken);

                // --------------------------------------------------------
                // Requirements hash
                // --------------------------------------------------------

                string currentStamp =
                    await CalculateFileSha256Async(
                        ObiPaths.Requirements,
                        cancellationToken);

                string storedStamp =
                    File.Exists(GetRequirementsStampPath())
                        ? (
                            await File.ReadAllTextAsync(
                                GetRequirementsStampPath(),
                                cancellationToken)
                          ).Trim()
                        : string.Empty;

                if (!string.Equals(
                        currentStamp,
                        storedStamp,
                        StringComparison.OrdinalIgnoreCase))
                {
                    progress?.Report(
                        "WhisperX requirements have changed; package repair is required.");

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
                    "WhisperX environment health check failed.");

                progress?.Report(
                    GetShortExceptionMessage(ex));

                return false;
            }
            finally
            {
                if (!string.IsNullOrWhiteSpace(healthScriptPath))
                {
                    try
                    {
                        if (File.Exists(healthScriptPath))
                            File.Delete(healthScriptPath);
                    }
                    catch
                    {
                        // Best effort only. The file is a temporary health-check
                        // script and does not affect the WhisperX environment.
                    }
                }
            }
        }

        private static string BuildHealthCheckScript()
        {
            string expectedVersions =
                BuildExpectedVersionsPythonDictionary();

            StringBuilder script = new();

            script.AppendLine("import importlib.metadata as md");
            script.AppendLine();
            script.AppendLine($"expected = {expectedVersions}");
            script.AppendLine("missing = []");
            script.AppendLine("wrong = []");
            script.AppendLine();
            script.AppendLine("for name, expected_version in expected.items():");
            script.AppendLine("    try:");
            script.AppendLine("        installed_version = md.version(name)");
            script.AppendLine("    except md.PackageNotFoundError:");
            script.AppendLine("        missing.append(name)");
            script.AppendLine("        continue");
            script.AppendLine();
            script.AppendLine("    if installed_version != expected_version:");
            script.AppendLine("        wrong.append(");
            script.AppendLine("            f\"{name}=={installed_version} (expected {expected_version})\"");
            script.AppendLine("        )");
            script.AppendLine();
            script.AppendLine("if missing:");
            script.AppendLine("    raise RuntimeError(");
            script.AppendLine("        \"Missing packages: \" + \", \".join(missing)");
            script.AppendLine("    )");
            script.AppendLine();
            script.AppendLine("if wrong:");
            script.AppendLine("    raise RuntimeError(\"; \".join(wrong))");
            script.AppendLine();
            script.AppendLine("# Core WhisperX runtime imports");
            script.AppendLine("import whisperx");
            script.AppendLine("import faster_whisper");
            script.AppendLine("import pyannote.audio");
            script.AppendLine("import torch");
            script.AppendLine("import torchaudio");
            script.AppendLine("import torchvision");
            script.AppendLine("import transformers");
            script.AppendLine("import onnxruntime");
            script.AppendLine("import nltk");
            script.AppendLine($"nltk.data.path.insert(0, {ToPythonStringLiteral(ObiPaths.NltkDataFolder)})");
            script.AppendLine("nltk.data.find('tokenizers/punkt_tab/english/')");
            script.AppendLine();
            script.AppendLine("print(\"WhisperX dependencies OK\")");
            script.AppendLine("print(\"WhisperX:\", getattr(whisperx, \"__version__\", \"unknown\"))");
            script.AppendLine("print(\"Faster-Whisper:\", getattr(faster_whisper, \"__version__\", \"unknown\"))");
            script.AppendLine("print(\"Torch:\", torch.__version__)");
            script.AppendLine("print(\"TorchAudio:\", torchaudio.__version__)");
            script.AppendLine("print(\"Transformers:\", transformers.__version__)");
            script.AppendLine("print(\"NLTK:\", nltk.__version__)");
            script.AppendLine("print(\"WhisperX health check completed successfully.\")");

            return script.ToString();
        }

        private static void EnsureBundledNltkData(
            IProgress<string>? progress)
        {
            string sourceRoot = Path.Combine(
                ObiPaths.PythonBackend,
                "nltk_data");

            string sourcePunktTab = Path.Combine(
                sourceRoot,
                "tokenizers",
                "punkt_tab");

            if (!Directory.Exists(sourcePunktTab))
            {
                throw new DirectoryNotFoundException(
                    "Bundled NLTK punkt_tab data was not found. Expected folder: " +
                    sourcePunktTab);
            }

            CopyDirectoryContents(sourceRoot, ObiPaths.NltkDataFolder);
            progress?.Report("Local NLTK punkt_tab data is ready.");
        }

        private static void CopyDirectoryContents(
            string sourceDirectory,
            string destinationDirectory)
        {
            Directory.CreateDirectory(destinationDirectory);

            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string destinationFile = Path.Combine(
                    destinationDirectory,
                    Path.GetFileName(file));

                File.Copy(file, destinationFile, overwrite: true);
            }

            foreach (string directory in Directory.GetDirectories(sourceDirectory))
            {
                string destinationSubdirectory = Path.Combine(
                    destinationDirectory,
                    Path.GetFileName(directory));

                CopyDirectoryContents(directory, destinationSubdirectory);
            }
        }

        private static string ToPythonStringLiteral(string value)
        {
            return "'" +
                value.Replace("\\", "\\\\").Replace("'", "\\'") +
                "'";
        }

        // ============================================================
        // REQUIREMENTS VERSION READING
        // ============================================================

        private sealed record PinnedRequirement(
            string Name,
            string Version);

        private static List<PinnedRequirement>
            ReadPinnedRequirements()
        {
            var result =
                new List<PinnedRequirement>();

            foreach (string rawLine in
                     File.ReadAllLines(ObiPaths.Requirements))
            {
                string line =
                    rawLine.Trim();

                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith(
                        "#",
                        StringComparison.Ordinal))
                {
                    continue;
                }

                int separator =
                    line.IndexOf(
                        "==",
                        StringComparison.Ordinal);

                if (separator <= 0 ||
                    separator >= line.Length - 2)
                {
                    // The current Whisper requirements file is expected
                    // to contain exact == pins.
                    continue;
                }

                string name =
                    line[..separator].Trim();

                string version =
                    line[(separator + 2)..].Trim();

                int comment =
                    version.IndexOf('#');

                if (comment >= 0)
                {
                    version =
                        version[..comment].Trim();
                }

                if (!string.IsNullOrWhiteSpace(name) &&
                    !string.IsNullOrWhiteSpace(version))
                {
                    result.Add(
                        new PinnedRequirement(
                            name,
                            version));
                }
            }

            return result;
        }

        private static string
            BuildExpectedVersionsPythonDictionary()
        {
            var packages =
                ReadPinnedRequirements();

            var entries =
                packages.Select(
                    p =>
                        $"'{p.Name.Replace("\\", "\\\\").Replace("'", "\\'")}':" +
                        $"'{p.Version.Replace("\\", "\\\\").Replace("'", "\\'")}'");

            return
                "{" +
                string.Join(",", entries) +
                "}";
        }

        // ============================================================
        // REPAIR / PACKAGE INSTALLATION
        // ============================================================

        private static async Task RepairEnvironmentAsync(
            string pythonExe,
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            if (!File.Exists(pythonExe))
            {
                throw new Exception(
                    "WhisperX virtual environment Python executable is missing.");
            }

            progress?.Report(
                "Upgrading WhisperX pip...");

            await RunProcess(
                pythonExe,
                "-m pip install --upgrade pip",
                progress,
                cancellationToken);

            progress?.Report(
                @"Installing / repairing WhisperX packages...

This may take several minutes.

The WhisperX AI environment is being
prepared for transcription.");

            await RunProcess(
                pythonExe,
                $"-m pip install -r \"{ObiPaths.Requirements}\"",
                progress,
                cancellationToken);

            // The stamp is written only after pip installation completes.
            // Therefore a failed installation remains repairable on the
            // next launch.
            string stamp =
                await CalculateFileSha256Async(
                    ObiPaths.Requirements,
                    cancellationToken);

            Directory.CreateDirectory(
                ObiPaths.WhisperEnvironment);

            await File.WriteAllTextAsync(
                GetRequirementsStampPath(),
                stamp,
                cancellationToken);
        }

        // ============================================================
        // PYTHON INSTALLATION
        // ============================================================

        private static async Task<string>
            EnsurePythonInstalledAsync(
                IProgress<string>? progress,
                CancellationToken cancellationToken)
        {
            progress?.Report(
                "Checking Python installation...");

            string pythonExe =
                GetInstalledPythonExe();

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
                        $"Installed Python version is not " +
                        $"{RequiredPythonVersion}: {version.Trim()}");
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    progress?.Report(
                        "Existing Python installation could not be started; " +
                        "it will be repaired.");
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
                    catch (OperationCanceledException)
                    {
                        throw;
                    }
                    catch
                    {
                        // Installation may still be finishing.
                    }
                }

                await Task.Delay(
                    TimeSpan.FromSeconds(1),
                    cancellationToken);
            }

            throw new Exception(
                $"Python {RequiredPythonVersion} installation failed or " +
                $"the expected Python executable was not found at:\n{pythonExe}");
        }

        // ============================================================
        // PYTHON DOWNLOAD
        // ============================================================


        private static async Task DownloadPythonInstallerAsync(
            IProgress<string>? progress,
            CancellationToken cancellationToken,
            bool forceRedownload)
        {
            string installerPath =
                GetPythonInstallerPath();

            if (forceRedownload &&
                File.Exists(installerPath))
            {
                try
                {
                    File.Delete(installerPath);
                }
                catch
                {
                    // We will download to a temporary file and replace
                    // the existing installer below.
                }
            }

            if (File.Exists(installerPath) &&
                new FileInfo(installerPath).Length >
                1024 * 1024)
            {
                progress?.Report(
                    "Python installer already downloaded.");

                return;
            }

            string? directory =
                Path.GetDirectoryName(installerPath);

            if (directory == null)
            {
                throw new Exception(
                    "Could not determine Python installer directory.");
            }

            Directory.CreateDirectory(directory);

            string tempPath =
                installerPath + ".download";

            try
            {
                if (File.Exists(tempPath))
                {
                    File.Delete(tempPath);
                }

                progress?.Report(
                    "Downloading Python installer...");

                using HttpClient client = new()
                {
                    Timeout =
                        TimeSpan.FromMinutes(15)
                };

                // Keep the HTTP response and streams inside this scope.
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

                // All download streams and the HTTP response are now disposed.

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

            string installer =
                GetPythonInstallerPath();

            await RunInstaller(
                installer,
                "/quiet InstallAllUsers=0 " +
                "PrependPath=1 Include_launcher=1",
                progress,
                cancellationToken);

            progress?.Report(
                "Python installation completed.");
        }

        // ============================================================
        // VISUAL C++ RUNTIME
        // ============================================================


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
            {
                throw new Exception(
                    "Could not determine VC++ installer directory.");
            }

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
                    {
                        File.Delete(tempPath);
                    }

                    progress?.Report(
                        "Downloading Microsoft Visual C++ runtime...");

                    using HttpClient client = new()
                    {
                        Timeout =
                            TimeSpan.FromMinutes(15)
                    };

                    // IMPORTANT:
                    // Keep the HTTP response and file streams inside
                    // this block so they are disposed before File.Move().
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

                    // At this point:
                    // - Output FileStream is disposed.
                    // - Input stream is disposed.
                    // - HTTP response is disposed.
                    // Therefore, the downloaded file is no longer
                    // held open by these objects.

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
                allowedExitCodes:
                    new[]
                    {
                0,
                3010,
                1638
                    });

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
            foreach (RegistryView view in
                     new[]
                     {
                         RegistryView.Registry64,
                         RegistryView.Registry32
                     })
            {
                try
                {
                    using RegistryKey baseKey =
                        RegistryKey.OpenBaseKey(
                            RegistryHive.LocalMachine,
                            view);

                    using RegistryKey? key =
                        baseKey.OpenSubKey(
                            VCRedistRegistryPath);

                    if (key == null)
                        continue;

                    object? installed =
                        key.GetValue("Installed");

                    object? versionValue =
                        key.GetValue("Version");

                    if (installed is int installedInt &&
                        installedInt != 1)
                    {
                        continue;
                    }

                    if (versionValue is string versionText)
                    {
                        string normalized =
                            versionText
                                .Trim()
                                .TrimStart(
                                    'v',
                                    'V');

                        if (Version.TryParse(
                                normalized,
                                out Version? version) &&
                            version.Major >= 14)
                        {
                            return true;
                        }
                    }
                }
                catch
                {
                    // If registry access is restricted, continue to the
                    // installation attempt rather than claiming the
                    // runtime is present.
                }
            }

            return false;
        }

        // ============================================================
        // PROCESS HELPERS
        // ============================================================

        private static async Task RunProcess(
            string fileName,
            string arguments,
            IProgress<string>? progress,
            CancellationToken cancellationToken)
        {
            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        fileName,

                    Arguments =
                        arguments,

                    RedirectStandardOutput =
                        true,

                    RedirectStandardError =
                        true,

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };

            using Process process =
                new()
                {
                    StartInfo =
                        psi,

                    EnableRaisingEvents =
                        true
                };

            var output =
                new StringBuilder();

            var error =
                new StringBuilder();

            object outputLock =
                new();

            process.OutputDataReceived +=
                (_, e) =>
                {
                    if (string.IsNullOrWhiteSpace(e.Data))
                        return;

                    lock (outputLock)
                    {
                        output.AppendLine(e.Data);
                    }

                    progress?.Report(
                        e.Data);
                };

            process.ErrorDataReceived +=
                (_, e) =>
                {
                    if (string.IsNullOrWhiteSpace(e.Data))
                        return;

                    lock (outputLock)
                    {
                        error.AppendLine(e.Data);
                    }

                    progress?.Report(
                        e.Data);
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
                await process.WaitForExitAsync(
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(
                    process);

                throw;
            }

            // Make sure asynchronous output events are drained.
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

                throw new Exception(
                    diagnostic);
            }
        }

        private static async Task<string>
            RunProcessForOutput(
                string fileName,
                string arguments,
                CancellationToken cancellationToken)
        {
            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        fileName,

                    Arguments =
                        arguments,

                    RedirectStandardOutput =
                        true,

                    RedirectStandardError =
                        true,

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };

            using Process process =
                new()
                {
                    StartInfo =
                        psi
                };

            if (!process.Start())
            {
                throw new Exception(
                    $"Failed to start process: {fileName}");
            }

            Task<string> stdoutTask =
                process.StandardOutput.ReadToEndAsync(
                    cancellationToken);

            Task<string> stderrTask =
                process.StandardError.ReadToEndAsync(
                    cancellationToken);

            try
            {
                await process.WaitForExitAsync(
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(
                    process);

                throw;
            }

            string stdout =
                await stdoutTask;

            string stderr =
                await stderrTask;

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

            return
                string.IsNullOrWhiteSpace(stdout)
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

            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        installer,

                    Arguments =
                        arguments,

                    UseShellExecute =
                        true,

                    WorkingDirectory =
                        Path.GetDirectoryName(installer) ??
                        ObiPaths.LocalDataFolder
                };

            using Process process =
                Process.Start(psi)
                ??
                throw new Exception(
                    $"Failed to start installer: {installer}");

            try
            {
                await process.WaitForExitAsync(
                    cancellationToken);
            }
            catch (OperationCanceledException)
            {
                TryKillProcessTree(
                    process);

                throw;
            }

            int[] successCodes =
                allowedExitCodes ??
                new[]
                {
                    0
                };

            if (!successCodes.Contains(
                    process.ExitCode))
            {
                throw new Exception(
                    $"Installer failed:\n" +
                    $"{installer} {arguments}\n\n" +
                    $"ExitCode={process.ExitCode}");
            }
        }

        private static void TryKillProcessTree(
            Process process)
        {
            try
            {
                if (!process.HasExited)
                {
                    process.Kill(
                        entireProcessTree: true);
                }
            }
            catch
            {
                // Best effort only.
            }
        }

        private static string BuildProcessFailureMessage(
            string fileName,
            string arguments,
            int exitCode,
            string stdout,
            string stderr)
        {
            const int maxDiagnosticLength =
                12000;

            string combined =
                string.Join(
                    Environment.NewLine,
                    new[]
                    {
                        stdout.Trim(),
                        stderr.Trim()
                    }
                    .Where(
                        s =>
                            !string.IsNullOrWhiteSpace(s)));

            if (combined.Length >
                maxDiagnosticLength)
            {
                combined =
                    combined[
                        ^maxDiagnosticLength..];
            }

            return
                $"Command failed:\n" +
                $"{fileName} {arguments}\n\n" +
                $"ExitCode={exitCode}\n\n" +
                (
                    string.IsNullOrWhiteSpace(
                        combined)
                        ? "No process output was captured."
                        : combined);
        }

        // ============================================================
        // INSTALLED PACKAGE LOG
        // ============================================================

        private static async Task
            SaveInstalledPackagesAsync(
                string pythonExe,
                CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(
                ObiPaths.LogsFolder);

            string outputFile =
                Path.Combine(
                    ObiPaths.LogsFolder,
                    "installed_packages.txt");

            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        pythonExe,

                    Arguments =
                        "-m pip freeze",

                    RedirectStandardOutput =
                        true,

                    RedirectStandardError =
                        true,

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };

            using Process process =
                new()
                {
                    StartInfo =
                        psi
                };

            if (!process.Start())
            {
                throw new Exception(
                    "Failed to start pip freeze.");
            }

            Task<string> packagesTask =
                process.StandardOutput.ReadToEndAsync(
                    cancellationToken);

            Task<string> errorTask =
                process.StandardError.ReadToEndAsync(
                    cancellationToken);

            await process.WaitForExitAsync(
                cancellationToken);

            string packages =
                await packagesTask;

            string error =
                await errorTask;

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"Failed to retrieve installed packages.\n\n" +
                    $"{error}");
            }

            ProcessStartInfo versionPsi =
                new()
                {
                    FileName =
                        pythonExe,

                    Arguments =
                        "--version",

                    RedirectStandardOutput =
                        true,

                    RedirectStandardError =
                        true,

                    UseShellExecute =
                        false,

                    CreateNoWindow =
                        true
                };

            using Process versionProcess =
                Process.Start(versionPsi)
                ??
                throw new Exception(
                    "Failed to retrieve Python version.");

            Task<string> versionOutputTask =
                versionProcess
                    .StandardOutput
                    .ReadToEndAsync(
                        cancellationToken);

            Task<string> versionErrorTask =
                versionProcess
                    .StandardError
                    .ReadToEndAsync(
                        cancellationToken);

            await versionProcess.WaitForExitAsync(
                cancellationToken);

            string version =
                await versionOutputTask;

            if (string.IsNullOrWhiteSpace(
                    version))
            {
                version =
                    await versionErrorTask;
            }

            if (versionProcess.ExitCode != 0)
            {
                throw new Exception(
                    "Failed to retrieve Python version.");
            }

            string output =
                $"Python Version: {version.Trim()}" +
                Environment.NewLine +
                Environment.NewLine +
                packages;

            await File.WriteAllTextAsync(
                outputFile,
                output,
                cancellationToken);
        }

        // ============================================================
        // UTILITY
        // ============================================================

        private static async Task<string>
            CalculateFileSha256Async(
                string filePath,
                CancellationToken cancellationToken)
        {
            await using FileStream stream =
                new(
                    filePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read);

            using SHA256 sha256 =
                SHA256.Create();

            byte[] hash =
                await sha256.ComputeHashAsync(
                    stream,
                    cancellationToken);

            return Convert.ToHexString(
                hash);
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
                    "Could not remove the existing WhisperX environment. " +
                    "A restart may be required before automatic repair can continue.");

                throw new Exception(
                    "Failed to remove the existing WhisperX virtual environment.",
                    ex);
            }
        }

        private static string
            GetShortExceptionMessage(
                Exception ex)
        {
            string message =
                ex.Message.Trim();

            const int maxLength =
                4000;

            if (message.Length >
                maxLength)
            {
                message =
                    message[
                        ^maxLength..];
            }

            return message;
        }
    }
}
