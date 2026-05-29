using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


using Obi.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace Obi.Services
{
    public class WhisperXService
    {
        public async Task<List<TranscriptSegment>>
            TranscribeAsync(
                string audioFile,
                CancellationToken cancellationToken)
        {
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

            string jsonOutput =
                Path.Combine(
                    backendFolder,
                    "output.json");

            string pythonExe =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "whisperx_env",
                    "Scripts",
                    "python.exe");

            ProcessStartInfo psi =
                new()
                {
                    FileName = pythonExe,

                    Arguments =
                        $"\"{scriptPath}\" " +
                        $"\"{audioFile}\" " +
                        $"\"{jsonOutput}\"",

                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                    UseShellExecute = false,

                    CreateNoWindow = true,

                    WorkingDirectory =
                        backendFolder
                };

            string ffmpegFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "ffmpeg");

            string existingPath =
                Environment.GetEnvironmentVariable(
                    "PATH") ?? "";

            psi.Environment["PATH"] =
                ffmpegFolder +
                ";" +
                existingPath;

            using Process process =
                new();

            process.StartInfo = psi;

            StringBuilder outputBuilder =
                new();

            StringBuilder errorBuilder =
                new();

            process.OutputDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(
                        e.Data))
                    {
                        Console.WriteLine(
                            e.Data);

                        outputBuilder.AppendLine(
                            e.Data);
                    }
                };

            process.ErrorDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(
                        e.Data))
                    {
                        Console.WriteLine(
                            e.Data);

                        errorBuilder.AppendLine(
                            e.Data);
                    }
                };

            process.Start();

            process.BeginOutputReadLine();

            process.BeginErrorReadLine();

            await process.WaitForExitAsync(
                cancellationToken);

            if (process.ExitCode != 0)
            {
                throw new Exception(
                    "WhisperX Error:\n" +
                    errorBuilder.ToString());
            }

            if (!File.Exists(
                jsonOutput))
            {
                throw new Exception(
                    "Output JSON not generated.");
            }

            string json =
                await File.ReadAllTextAsync(
                    jsonOutput,
                    cancellationToken);

            WhisperXResult? result =
                JsonSerializer.Deserialize
                <WhisperXResult>(
                    json);

            if (result == null)
            {
                throw new Exception(
                    "Failed to parse WhisperX JSON.");
            }

            List<TranscriptSegment>
                segments = new();

            foreach (var phrase
                in result.Phrases)
            {
                TranscriptSegment segment =
                    new()
                    {
                        PhraseId =
                            phrase.PhraseId,

                        Text =
                            phrase.Text,

                        Start =
                            TimeSpan.FromSeconds(
                                phrase.Start),

                        End =
                            TimeSpan.FromSeconds(
                                phrase.End),

                        Confidence =
                             phrase.Confidence,
                    };

                foreach (var word
                    in phrase.Words)
                {
                    segment.Words.Add(
                        new WordTimestamp
                        {
                            Word =
                                word.Word,

                            Start =
                                word.Start,

                            End =
                                word.End,

                            Confidence =
                                word.Confidence
                        });
                }

                segments.Add(segment);
            }

            return segments;
        }
    }
}
