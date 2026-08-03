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
        public static string Merge(IList<string> audioFiles,IProgress<string>? progress = null)
        {

            progress?.Report("===== Audio Merge =====");

            foreach (string file in audioFiles)
            {
                FileInfo info = new FileInfo(file);

                progress?.Report($"Input : {file}");
                progress?.Report($"Exists: {info.Exists}");

                if (info.Exists)
                {
                    progress?.Report($"Size  : {info.Length} bytes");
                }
            }

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

            progress?.Report("===== FFmpeg List File =====");
            progress?.Report(File.ReadAllText(listFile));

            string ffmpegExe = ObiPaths.FFmpegExe;

            progress?.Report($"FFmpeg: {ffmpegExe}");

            if (File.Exists(ffmpegExe))
            {
                progress?.Report($"FFmpeg Size: {new FileInfo(ffmpegExe).Length} bytes");
            }
            else
            {
                progress?.Report("FFmpeg executable NOT FOUND.");
            }

            using Process process = new Process();

            process.StartInfo = new ProcessStartInfo
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
            };

            StringBuilder stdout = new();
            StringBuilder stderr = new();

            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data != null)
                    stdout.AppendLine(e.Data);
            };

            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data != null)
                    stderr.AppendLine(e.Data);
            };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            progress?.Report($"FFmpeg Exit Code: {process.ExitCode}");

            string output = stdout.ToString();
            string error = stderr.ToString();

            progress?.Report("===== FFmpeg STDOUT =====");

            if (!string.IsNullOrWhiteSpace(output))
            {
                progress?.Report(output);
            }

            progress?.Report("===== FFmpeg STDERR =====");

            if (!string.IsNullOrWhiteSpace(error))
            {
                progress?.Report(error);
            }

            if (process.ExitCode != 0)
            {
                throw new Exception( "Failed to merge audio files.\n\n" +
                    error);
            }

            FileInfo merged = new FileInfo(tempOutput);

            progress?.Report("===== Combined Audio =====");
            progress?.Report($"Exists : {merged.Exists}");

            if (merged.Exists)
            {
                progress?.Report($"Size : {merged.Length} bytes");

                if (merged.Length < 1024)
                {
                    throw new Exception(
@"Unable to import audio.

One or more selected audio files are invalid, corrupted, or contain no usable audio data.

Please verify the audio files and try again.");
                }
            }
            else
            {
                throw new Exception(
                    "CombinedAudio.wav was not created.");
            }

            File.Delete(listFile);

            return tempOutput;
        }
    }
}