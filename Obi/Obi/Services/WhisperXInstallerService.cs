using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class WhisperXInstallerService
    {
        public static string GetVenvPath()
        {
            return Path.GetFullPath(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "whisperx_env"));
        }

        public static string GetPythonExe()
        {
            return Path.Combine(
                GetVenvPath(),
                "Scripts",
                "python.exe");
        }

        public static Task<bool>
            IsPythonEnvironmentInstalledAsync()
        {
            return Task.FromResult(
                File.Exists(
                    GetPythonExe()));
        }

        public static async Task InstallAsync(
            IProgress<string>? progress = null)
        {
            progress?.Report(
                "Checking Python installation...");

            await VerifyPythonInstalled();

            string venvPath =
                GetVenvPath();

            if (!Directory.Exists(
                venvPath))
            {
                progress?.Report(
                    "Creating virtual environment...");

                await RunProcess(
                    "python",
                    $"-m venv \"{venvPath}\"",
                    progress);
            }

            string pythonExe =
                GetPythonExe();

            progress?.Report(
                "Upgrading pip...");

            await RunProcess(
                pythonExe,
                "-m pip install --upgrade pip",
                progress);

            string requirements =
                Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "..",
                        "..",
                        "..",
                        "..",
                        "PythonBackend",
                        "requirements.txt"));

            progress?.Report(
                "Installing WhisperX packages...");

            await RunProcess(
                pythonExe,
                $"-m pip install -r \"{requirements}\"",
                progress);

            progress?.Report(
                "WhisperX installation completed.");
        }

        public static async Task EnsureModelsAsync(
    IProgress<string>? progress = null)
        {
            if (AreModelsPresent())
            {
                return;
            }

            progress?.Report(
                "Downloading AI models...");

            string pythonExe =
                GetPythonExe();

            string backendFolder =
                Path.GetFullPath(
                    Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "..",
                        "..",
                        "..",
                        "..",
                        "PythonBackend"));

            string scriptPath =
                Path.Combine(
                    backendFolder,
                    "run_whisperx.py");

            string tempWav =
                Path.Combine(
                    backendFolder,
                    "dummy.wav");

            string tempJson =
                Path.Combine(
                    backendFolder,
                    "dummy.json");

            await CreateSilentWav(
                tempWav);

            try
            {
                await RunProcess(
                    pythonExe,
                    $"\"{scriptPath}\" " +
                    $"\"{tempWav}\" " +
                    $"\"{tempJson}\"",
                    progress);
            }
            finally
            {
                try
                {
                    File.Delete(
                        tempWav);
                }
                catch
                {
                }

                try
                {
                    File.Delete(
                        tempJson);
                }
                catch
                {
                }
            }

            progress?.Report(
                "AI models downloaded.");
        }

        private static async Task CreateSilentWav(string path)
        {
            byte[] wavHeader =
            {
                82,73,70,70,
                36,0,0,0,
                87,65,86,69,
                102,109,116,32,
                16,0,0,0,
                1,0,
                1,0,
                128,62,0,0,
                0,125,0,0,
                2,0,
                16,0,
                100,97,116,97,
                0,0,0,0
             };

            await File.WriteAllBytesAsync(
                path,
                wavHeader);
        }

        private static async Task
            VerifyPythonInstalled()
        {
            try
            {
                await RunProcess(
                    "python",
                    "--version");
            }
            catch
            {
                throw new Exception(
                    "Python 3.11 or newer is not installed.");
            }
        }

        private static async Task RunProcess(
            string fileName,
            string arguments,
            IProgress<string>? progress = null)
        {
            ProcessStartInfo psi =
                new()
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

        public static string GetModelsFolder()
        {
            return Path.GetFullPath(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "Models"));
        }

        public static bool AreModelsPresent()
        {
            string modelsFolder =
                GetModelsFolder();

            if (!Directory.Exists(
                modelsFolder))
            {
                return false;
            }

            return Directory.EnumerateFiles(
                    modelsFolder,
                    "*",
                    SearchOption.AllDirectories)
                .Any();
        }
    }
}