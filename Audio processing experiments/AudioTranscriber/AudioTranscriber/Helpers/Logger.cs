using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioTranscriber.Helpers
{
    public static class Logger
    {
        private static readonly string LogFile =
            Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "app.log");

        public static void Log(string message)
        {
            try
            {
                File.AppendAllText(
                    LogFile,
                    $"[{DateTime.Now}] {message}{Environment.NewLine}");
            }
            catch
            {
            }
        }

        public static void Log(Exception ex)
        {
            Log(ex.ToString());
        }
    }
}