using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Obi.Services
{
    public static class ObiPaths
    {
        public static string ApplicationFolder =>
            AppDomain.CurrentDomain.BaseDirectory;

        public static string LocalDataFolder =>
          Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"Obi");

        public static string ModelsFolder =>
          Path.Combine(LocalDataFolder,"Models");

        public static string HuggingFaceFolder =>
            Path.Combine(
                LocalDataFolder,
                "HuggingFace");

        public static string NltkDataFolder =>
            Path.Combine(
                LocalDataFolder,
                "nltk_data");

        public static string LogsFolder =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                "Whisper Log");
        public static string PythonBackend =>
            Path.Combine(
                ApplicationFolder,
                "PythonBackend");

        public static string WhisperEnvironment =>
            Path.Combine(
                LocalDataFolder,
                "whisperx_env");

        public static string PythonExe =>
            Path.Combine(
                WhisperEnvironment,
                "Scripts",
                "python.exe");

        public static string Requirements =>
            Path.Combine(
                PythonBackend,
                "requirements.txt");

        public static string WhisperScript =>
            Path.Combine(
                PythonBackend,
                "run_whisperx.py");

        public static string PythonInstallerFolder =>
            Path.Combine(
                LocalDataFolder,
                "PythonInstaller");

        public static string FFmpegExe =>
            Path.Combine(
                ApplicationFolder,
                "ffmpeg.exe");
    }
}
