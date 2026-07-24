using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class WhisperXInstallerService
    {

        private const string RequiredPythonVersion = "3.11.9";

        private const string PythonInstallerUrl = "https://www.python.org/ftp/python/3.11.9/python-3.11.9-amd64.exe";
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

        private static bool IsPython311Installed()
        {
            return File.Exists(GetInstalledPythonExe());
        }
        private static string GetPythonInstallerPath()
        {
            return Path.Combine(
                ObiPaths.PythonInstallerFolder,
                $"python-{RequiredPythonVersion}-amd64.exe");
        }
        private static async Task DownloadPythonInstallerAsync(IProgress<string>? progress = null)
        {
            string installerPath = GetPythonInstallerPath();

            if (File.Exists(installerPath))
            {
                progress?.Report("Python installer already downloaded.");

                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(installerPath)!);

            progress?.Report("Downloading Python installer...");

            using HttpClient client = new();

            using HttpResponseMessage response =
                await client.GetAsync(PythonInstallerUrl,HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            using Stream input = await response.Content.ReadAsStreamAsync();

            using FileStream output = File.Create(installerPath);

            await input.CopyToAsync(output);

            progress?.Report("Python installer downloaded.");
        }

        private static async Task InstallPythonAsync(IProgress<string>? progress = null)
        {
            progress?.Report("Installing Python 3.11...");

            string installer = GetPythonInstallerPath();

            await RunInstaller(installer,"/quiet InstallAllUsers=0 PrependPath=1 Include_launcher=1",progress);

            progress?.Report("Python installation completed.");
        }


        public static Task<bool> IsPythonEnvironmentInstalledAsync()
        {
            return Task.FromResult(File.Exists(GetPythonExe()));
        }

        public static async Task InstallAsync(IProgress<string>? progress = null)
        {
            Directory.CreateDirectory(ObiPaths.LocalDataFolder);
            Directory.CreateDirectory(ObiPaths.ModelsFolder);

            Directory.CreateDirectory(ObiPaths.HuggingFaceFolder);

            Directory.CreateDirectory(ObiPaths.NltkDataFolder);

            bool firstTimeSetup = !IsPython311Installed() ||!File.Exists(GetPythonExe());

            if (firstTimeSetup)
            {
                progress?.Report(
                @"==================================================
FIRST-TIME WHISPERX SETUP

Obi will now:

• Install Python 3.11
• Create the WhisperX environment
• Install WhisperX packages
• Download AI transcription models

This is a one-time setup and may take
10-30 minutes depending on your internet
connection and computer speed.

Please do not close Obi during installation.

==================================================");
            }

            string pythonExe = await EnsurePythonInstalledAsync(progress);

            string venvPath = GetVenvPath();

            if (!File.Exists(GetPythonExe()))
            {
                progress?.Report("Creating WhisperX environment...");

                await RunProcess(
                    pythonExe,
                    $"-m venv \"{venvPath}\"",
                    progress);
            }

            if (!File.Exists(GetPythonExe()))
            {
                throw new Exception("Virtual environment was not created successfully.");
            }

            pythonExe = GetPythonExe();

            progress?.Report("Upgrading pip...");

            await RunProcess(pythonExe,"-m pip install --upgrade pip",progress);

            string requirements = ObiPaths.Requirements;

            progress?.Report(
            @"Installing WhisperX packages...

If required, AI transcription models will also
be downloaded during this step.

This process may take several minutes.

Please do not close Obi.");

            await RunProcess(
                pythonExe,
                $"-m pip install -r \"{requirements}\"",
                progress);

            progress?.Report(
            @"WhisperX installation completed successfully.

Future transcriptions will start much faster
because this setup only happens once.");
        }



        private static async Task<string> EnsurePythonInstalledAsync(IProgress<string>? progress = null)
        {
            progress?.Report("Checking Python installation...");

            progress?.Report($"Checking: {GetInstalledPythonExe()}");

            if (IsPython311Installed())
            {
                progress?.Report("Python 3.11 found.");

                return GetInstalledPythonExe();
            }

            progress?.Report("Python 3.11 not found. Preparing automatic installation...");

            await DownloadPythonInstallerAsync(progress);

            await InstallPythonAsync(progress);

            progress?.Report("Verifying Python installation...");

            for (int i = 0; i < 10; i++)
            {
                if (IsPython311Installed())
                {
                    break;
                }

                await Task.Delay(1000);
            }


            if (!IsPython311Installed())
            {
                throw new Exception("Python installation failed.");
            }

            progress?.Report("Python 3.11 installed successfully.");

            return GetInstalledPythonExe();
        }

        private static async Task RunProcess(string fileName,string arguments,IProgress<string>? progress = null)
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

            using Process process = new();

            process.StartInfo = psi;

            process.OutputDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        progress?.Report(e.Data);
                    }
                };

            process.ErrorDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        progress?.Report(e.Data);
                    }
                };

            progress?.Report($"{Path.GetFileName(fileName)} {arguments}");

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"Command failed:\n{fileName} {arguments}\n\nExitCode={process.ExitCode}");
            }
        }

        private static async Task RunInstaller(string installer,string arguments,IProgress<string>? progress = null)
        {
            progress?.Report($"{Path.GetFileName(installer)} {arguments}");

            ProcessStartInfo psi = new()
                {
                    FileName = installer,
                    Arguments = arguments,
                    UseShellExecute = true
                };

            using Process process =
                Process.Start(psi)!;

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                throw new Exception($"Python installer failed. ExitCode={process.ExitCode}");
            }
        }

    }
}