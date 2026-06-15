using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Obi.Services
{
    public class SilenceRegion
    {
        public TimeSpan Start
        {
            get;
            set;
        }

        public TimeSpan End
        {
            get;
            set;
        }
    }

    public static class AudioSilenceDetector
    {
        public static async Task<List<SilenceRegion>>
            DetectAsync(
                string audioFile)
        {
            List<SilenceRegion> result =
                new();

            string ffmpegFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ffmpeg");

            string ffmpegExe =
                Path.Combine(
                    ffmpegFolder,
                    "ffmpeg.exe");

            if (!File.Exists(ffmpegExe))
            {
                throw new FileNotFoundException(
                    "ffmpeg.exe not found.",
                    ffmpegExe);
            }

            ProcessStartInfo psi =
                new()
                {
                    FileName =
                        ffmpegExe,

                    Arguments =
                        $"-i \"{audioFile}\" " +
                        "-af silencedetect=noise=-35dB:d=0.25 " +
                        "-f null NUL",

                    RedirectStandardError = true,
                    RedirectStandardOutput = true,

                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            using Process process =
                new();

            process.StartInfo =
                psi;

            process.Start();

            string stderr =
                await process.StandardError
                    .ReadToEndAsync();

            Debug.WriteLine(
    "===== FFMPEG SILENCE OUTPUT =====");

            Debug.WriteLine(
                stderr);

            await process.WaitForExitAsync();

            File.WriteAllText(
                        Path.Combine(
                            Path.GetTempPath(),
                            "ffmpeg_silence_log.txt"),
                        stderr);

            Regex startRegex =
                new(
                    @"silence_start:\s*([0-9\.]+)");

            Regex endRegex =
                new(
                    @"silence_end:\s*([0-9\.]+)");

            MatchCollection starts =
                startRegex.Matches(stderr);

            MatchCollection ends =
                endRegex.Matches(stderr);

            int count =
                Math.Min(
                    starts.Count,
                    ends.Count);

            for (int i = 0; i < count; i++)
            {
                if (!double.TryParse(
                        starts[i].Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double start))
                {
                    continue;
                }

                if (!double.TryParse(
                        ends[i].Groups[1].Value,
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out double end))
                {
                    continue;
                }

                result.Add(
                    new SilenceRegion
                    {
                        Start =
                            TimeSpan.FromSeconds(
                                start),

                        End =
                            TimeSpan.FromSeconds(
                                end)
                    });
            }

            return result;
        }
    }
}