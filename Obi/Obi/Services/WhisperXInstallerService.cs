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

    }
}