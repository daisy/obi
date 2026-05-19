using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AudioTranscriber.Models;
using System.Diagnostics;
using System.Text.Json;

namespace AudioTranscriber.Services
{
    public class WhisperXService
    {
        public async Task<List<TranscriptSegment>> TranscribeAsync(string audioFile, CancellationToken cancellationToken)
        {
            string backendFolder =
                       Path.GetFullPath(Path.Combine(
                                              AppDomain.CurrentDomain.BaseDirectory,
                                                "..",
                                                "..",
                                                 "..",
                                                 "..",
                                                 "PythonBackend"));

            Directory.CreateDirectory(
                backendFolder);

            string jsonOutput =
                Path.Combine(
                    backendFolder,
                    "output.json");

            string scriptPath =
                Path.Combine(
                    backendFolder,
                    "run_whisperx.py");

            ProcessStartInfo psi =
               new()
               {
                   FileName =
                         Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "whisperx_env", "Scripts", "python.exe"), Arguments =
                       $"\"{scriptPath}\" \"{audioFile}\" \"{jsonOutput}\"",

                   RedirectStandardOutput = true,
                   RedirectStandardError = true,
                   UseShellExecute = false,
                   CreateNoWindow = true,
                   WorkingDirectory = backendFolder
               };

            string ffmpegFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ffmpeg");

            string existingPath = Environment.GetEnvironmentVariable("PATH") ?? "";

            psi.Environment["PATH"] =
                ffmpegFolder + ";" + existingPath;

            using Process process =
                new();

            process.StartInfo = psi;

            process.Start();

            string stdOut =
                await process.StandardOutput
                    .ReadToEndAsync();

            string stdErr =
                await process.StandardError
                    .ReadToEndAsync();

            await process.WaitForExitAsync(
                cancellationToken);

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    $"WhisperX Error:\n{stdErr}");
            }

            string json =
                await File.ReadAllTextAsync(
                    jsonOutput,
                    cancellationToken);

            WhisperXResult? result =
               JsonSerializer.Deserialize<WhisperXResult>(json);

            List<TranscriptSegment>
                segments = new();
            foreach (var phrase
              in result.Phrases)
            {
                segments.Add(
                    new TranscriptSegment
                    {
                        Text =
                            phrase.Text,

                        Start =
                            TimeSpan.FromSeconds(
                                phrase.Start),

                        End =
                            TimeSpan.FromSeconds(
                                phrase.End)
                    });
            }



            return segments;
        }




    }
}


