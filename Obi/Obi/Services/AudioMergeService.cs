using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Services
{
    public static class AudioMergeService
    {
        public static string Merge(
            IList<string> audioFiles)
        {


            string tempOutput =
                Path.Combine(
                    Path.GetDirectoryName(
                        audioFiles[0])!,
                    "CombinedAudio.wav");

            if (File.Exists(tempOutput))
            {
                File.Delete(tempOutput);
            }
            string listFile =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".txt");

            File.WriteAllLines(
                listFile,
                audioFiles.Select(
                    f => $"file '{f.Replace("'", "'\\''")}'"));

            string ffmpegExe =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ffmpeg",
                    "ffmpeg.exe");

            using Process process =
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = ffmpegExe,
                        Arguments =
                                $"-y " +
                                $"-f concat -safe 0 " +
                                $"-i \"{listFile}\" " +
                                $"-c:a pcm_s16le " +
                                $"\"{tempOutput}\"",

                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    })!;

            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error =
                    process.StandardError.ReadToEnd();

                throw new Exception(
                    "Failed to merge audio files.\n\n" +
                    error);
            }

            File.Delete(listFile);

            return tempOutput;
        }
    }
}