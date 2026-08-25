using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class ParakeetInstallerService
    {
        // --------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------

        private const string RequiredPythonVersion =
            "3.11.9";


        // --------------------------------------------------
        // PYTHON INSTALLATION
        // --------------------------------------------------

        private const string PythonInstallerUrl =
            "https://www.python.org/ftp/python/3.11.9/" +
            "python-3.11.9-amd64.exe";


        // --------------------------------------------------
        // PATHS
        // --------------------------------------------------

        public static string GetVenvPath()
        {
            return ObiPaths.ParakeetEnvironment;
        }


        public static string GetPythonExe()
        {
            return ObiPaths.ParakeetPythonExe;
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


        private static bool IsPython311Installed()
        {
            return File.Exists(
                GetInstalledPythonExe());
        }


        private static string GetPythonInstallerPath()
        {
            return Path.Combine(
                ObiPaths.PythonInstallerFolder,
                $"python-{RequiredPythonVersion}-amd64.exe");
        }


        // --------------------------------------------------
        // ENVIRONMENT CHECK
        // --------------------------------------------------

        public static Task<bool>
            IsPythonEnvironmentInstalledAsync()
        {
            bool installed =
                File.Exists(
                    GetPythonExe());

            return Task.FromResult(
                installed);
        }


        // --------------------------------------------------
        // INSTALL
        // --------------------------------------------------

        public static async Task InstallAsync(
            IProgress<string>? progress = null)
        {
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
            // PYTHON
            // --------------------------------------------------

            string pythonExe =
                await EnsurePythonInstalledAsync(
                    progress);


            // --------------------------------------------------
            // CREATE VIRTUAL ENVIRONMENT
            // --------------------------------------------------

            string venvPath =
                GetVenvPath();


            if (!File.Exists(
                    GetPythonExe()))
            {
                progress?.Report(
                    "Creating Parakeet environment...");


                await RunProcess(
                    pythonExe,
                    $"-m venv \"{venvPath}\"",
                    progress);
            }


            if (!File.Exists(
                    GetPythonExe()))
            {
                throw new Exception(
                    "Parakeet virtual environment " +
                    "was not created successfully.");
            }


            pythonExe =
                GetPythonExe();


            // --------------------------------------------------
            // UPGRADE PIP
            // --------------------------------------------------

            progress?.Report(
                "Upgrading Parakeet pip...");


            await RunProcess(
                pythonExe,
                "-m pip install --upgrade pip",
                progress);


            // --------------------------------------------------
            // INSTALL PACKAGES
            // --------------------------------------------------

            progress?.Report(
                @"Installing Parakeet packages...

This may take several minutes.

The Parakeet AI environment is being
prepared for offline transcription.");


            await RunProcess(
                pythonExe,
                $"-m pip install -r " +
                $"\"{ObiPaths.ParakeetRequirements}\"",
                progress);


            // --------------------------------------------------
            // VERIFY
            // --------------------------------------------------

            progress?.Report(
                "Verifying Parakeet installation...");


            await RunProcess(
                pythonExe,
                "-c \"import torch; " +
                "import transformers; " +
                "import librosa; " +
                "print('Parakeet dependencies OK'); " +
                "print('Torch:', torch.__version__); " +
                "print('Transformers:', transformers.__version__); " +
                "print('Librosa:', librosa.__version__)\"",
                progress);


            progress?.Report(
                "Parakeet environment installed successfully.");
        }


        // --------------------------------------------------
        // ENSURE PYTHON
        // --------------------------------------------------

        private static async Task<string>
            EnsurePythonInstalledAsync(
                IProgress<string>? progress = null)
        {
            progress?.Report(
                "Checking Python installation...");


            progress?.Report(
                $"Checking: {GetInstalledPythonExe()}");


            if (IsPython311Installed())
            {
                progress?.Report(
                    "Python 3.11 found.");

                return GetInstalledPythonExe();
            }


            progress?.Report(
                "Python 3.11 not found. " +
                "Preparing automatic installation...");


            await DownloadPythonInstallerAsync(
                progress);


            await InstallPythonAsync(
                progress);


            progress?.Report(
                "Verifying Python installation...");


            for (int i = 0; i < 10; i++)
            {
                if (IsPython311Installed())
                {
                    break;
                }


                await Task.Delay(
                    1000);
            }


            if (!IsPython311Installed())
            {
                throw new Exception(
                    "Python installation failed.");
            }


            progress?.Report(
                "Python 3.11 installed successfully.");


            return GetInstalledPythonExe();
        }


        // --------------------------------------------------
        // DOWNLOAD PYTHON
        // --------------------------------------------------

        private static async Task
            DownloadPythonInstallerAsync(
                IProgress<string>? progress = null)
        {
            string installerPath =
                GetPythonInstallerPath();


            if (File.Exists(
                    installerPath))
            {
                progress?.Report(
                    "Python installer already downloaded.");

                return;
            }


            Directory.CreateDirectory(
                Path.GetDirectoryName(
                    installerPath)!);


            progress?.Report(
                "Downloading Python installer...");


            using HttpClient client =
                new();


            using HttpResponseMessage response =
                await client.GetAsync(
                    PythonInstallerUrl,
                    HttpCompletionOption.ResponseHeadersRead);


            response.EnsureSuccessStatusCode();


            using Stream input =
                await response.Content
                    .ReadAsStreamAsync();


            using FileStream output =
                File.Create(
                    installerPath);


            await input.CopyToAsync(
                output);


            progress?.Report(
                "Python installer downloaded.");
        }


        // --------------------------------------------------
        // INSTALL PYTHON
        // --------------------------------------------------

        private static async Task
            InstallPythonAsync(
                IProgress<string>? progress = null)
        {
            progress?.Report(
                "Installing Python 3.11...");


            string installer =
                GetPythonInstallerPath();


            await RunInstaller(
                installer,
                "/quiet InstallAllUsers=0 " +
                "PrependPath=1 Include_launcher=1",
                progress);


            progress?.Report(
                "Python installation completed.");
        }


        // --------------------------------------------------
        // RUN PROCESS
        // --------------------------------------------------

        private static async Task RunProcess(
            string fileName,
            string arguments,
            IProgress<string>? progress = null)
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
                new();


            process.StartInfo =
                psi;


            process.OutputDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(
                            e.Data))
                    {
                        progress?.Report(
                            e.Data);
                    }
                };


            process.ErrorDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(
                            e.Data))
                    {
                        progress?.Report(
                            e.Data);
                    }
                };


            progress?.Report(
                $"{Path.GetFileName(fileName)} {arguments}");


            if (!process.Start())
            {
                throw new Exception(
                    $"Failed to start process: {fileName}");
            }


            process.BeginOutputReadLine();

            process.BeginErrorReadLine();


            await process.WaitForExitAsync();


            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"Command failed:\n" +
                    $"{fileName} {arguments}\n\n" +
                    $"ExitCode={process.ExitCode}");
            }
        }


        // --------------------------------------------------
        // PYTHON INSTALLER PROCESS
        // --------------------------------------------------

        private static async Task RunInstaller(
            string installer,
            string arguments,
            IProgress<string>? progress = null)
        {
            progress?.Report(
                $"{Path.GetFileName(installer)} {arguments}");


            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        installer,

                    Arguments =
                        arguments,

                    UseShellExecute =
                        true
                };


            using Process process =
                Process.Start(psi)!;


            await process.WaitForExitAsync();


            if (process.ExitCode != 0)
            {
                throw new Exception(
                    "Python installer failed. " +
                    $"ExitCode={process.ExitCode}");
            }
        }
    }
}