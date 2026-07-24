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
using Obi.Services;
using System.Windows.Forms;
using NAudio.Wave;

namespace Obi.Services
{
    public class WhisperXService
    {
        public async Task<List<TranscriptSegment>> TranscribeAsync(string audioFile, WhisperModel model, CancellationToken cancellationToken, IProgress<string>? progress = null)
        {
            string backendFolder =
                ObiPaths.PythonBackend;

            string scriptPath =
                ObiPaths.WhisperScript;

            string jsonOutput =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + ".json");


            ProcessStartInfo psi =
             await CreateProcessStartInfoAsync(
          $"\"{scriptPath}\" " +
          $"\"{audioFile}\" " +
          $"\"{jsonOutput}\" " +
          $"\"{GetModelName(model)}\" " +
          $"\"{ObiPaths.ModelsFolder}\" " +
          $"\"{ObiPaths.HuggingFaceFolder}\" " +
          $"\"{ObiPaths.NltkDataFolder}\"");




            // following code will use existing ffmpeg of Obi
            //string ffmpegPath =
            //            Path.Combine(
            //                AppDomain.CurrentDomain.BaseDirectory,
            //                "ffmpeg.exe");

            //if (!File.Exists(ffmpegPath))
            //{
            //    throw new Exception(
            //        "FFmpeg is missing.\n\nExpected:\n" +
            //        ffmpegPath);
            //}



            await ExecuteWhisperProcessAsync(psi, cancellationToken, progress);
            var segments = await LoadTranscriptAsync(jsonOutput, audioFile, cancellationToken, progress);

            SafeDelete(jsonOutput);

            return segments;
        }

        public async Task<Dictionary<string, List<TranscriptSegment>>> TranscribeBatchAsync(List<string> audioFiles, WhisperModel model, CancellationToken cancellationToken, IProgress<string>? progress = null)
        {
            string backendFolder =
                ObiPaths.PythonBackend;

            string scriptPath =
                ObiPaths.WhisperScript;



            string jobsFile =
                Path.Combine(
                    Path.GetTempPath(),
                    Guid.NewGuid() + "_jobs.json");

            var jobs = new
            {
                files = audioFiles.Select(
                    file => new
                    {
                        input = file,

                        output = Path.ChangeExtension(
                            file,
                            ".json")
                    })
            };

            await File.WriteAllTextAsync(
                jobsFile,
                JsonSerializer.Serialize(
                    jobs,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }),
                cancellationToken);

            ProcessStartInfo psi =
                await CreateProcessStartInfoAsync(
                    $"\"{scriptPath}\" " +
                    $"--batch " +
                    $"\"{jobsFile}\" " +
                    $"\"{GetModelName(model)}\" " +
                    $"\"{ObiPaths.ModelsFolder}\" " +
                    $"\"{ObiPaths.HuggingFaceFolder}\" " +
                    $"\"{ObiPaths.NltkDataFolder}\"");



            await ExecuteWhisperProcessAsync(psi, cancellationToken, progress);


            Dictionary<string, List<TranscriptSegment>> results = new();

            foreach (string audioFile in audioFiles)
            {
                string jsonFile = Path.ChangeExtension(audioFile, ".json");

                List<TranscriptSegment> segments = await LoadTranscriptAsync(jsonFile, audioFile, cancellationToken, progress);

                results.Add(audioFile, segments);

                SafeDelete(jsonFile);
            }

            SafeDelete(jobsFile);

            return results;
        }


        private static string GetModelName(WhisperModel model)
        {
            return model switch
            {
                WhisperModel.Small => "small",
                WhisperModel.Medium => "medium",
                _ => "large-v3"
            };
        }


        private static void SafeDelete(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch
            {
            }
        }


        private async Task<ProcessStartInfo> CreateProcessStartInfoAsync(string arguments)
        {
            string backendFolder =
                ObiPaths.PythonBackend;

            string pythonExe =
                ObiPaths.PythonExe;

            if (!await WhisperXInstallerService
                .IsPythonEnvironmentInstalledAsync())
            {
                throw new Exception(
                    "WhisperX is not installed.");
            }

            ProcessStartInfo psi =
                new()
                {
                    FileName = pythonExe,

                    Arguments = arguments,

                    RedirectStandardOutput = true,
                    RedirectStandardError = true,

                    UseShellExecute = false,

                    CreateNoWindow = true,

                    WorkingDirectory =
                        backendFolder
                };


            string ffmpegFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory);

            string ffmpegExe =
                ObiPaths.FFmpegExe;

            if (!File.Exists(ffmpegExe))
            {
                throw new Exception(
                    "FFmpeg is missing.\n\nExpected:\n" +
                    ffmpegExe);
            }

            string existingPath =
                Environment.GetEnvironmentVariable(
                    "PATH") ?? "";

            psi.Environment["PATH"] =
                ffmpegFolder +
                ";" +
                existingPath;

            return psi;
        }


        private async Task ExecuteWhisperProcessAsync(ProcessStartInfo psi, CancellationToken cancellationToken, IProgress<string>? progress, Action<string>? outputHandler = null)
        {
            using Process process = new();

            process.StartInfo = psi;

            StringBuilder errorBuilder =
                new();

            Stopwatch stopwatch =
                Stopwatch.StartNew();

            using CancellationTokenSource heartbeatCts =  new();


            process.OutputDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        progress?.Report(e.Data);

                        outputHandler?.Invoke(e.Data);

                    }
                };

            process.ErrorDataReceived +=
                (s, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                    {
                        errorBuilder.AppendLine(e.Data);
                    }
                };

            process.Start();

            process.BeginOutputReadLine();

            process.BeginErrorReadLine();

            using CancellationTokenRegistration cancellationRegistration =
                cancellationToken.Register(() =>
                {
                    try
                    {
                        progress?.Report("Cancelling transcription...");

                        if (!process.HasExited)
                        {
                            process.Kill(entireProcessTree: true);
                        }
                    }
                    catch
                    {
                        // Ignore cleanup errors.
                    }
                });

            Task heartbeatTask =
                Task.Run(async () =>
                {
                    try
                    {
                        while (!process.HasExited)
                        {
                            await Task.Delay(
                                TimeSpan.FromSeconds(30),
                                heartbeatCts.Token);

                            if (!process.HasExited)
                            {
                                string message = $"Still transcribing... Elapsed: {stopwatch.Elapsed:hh\\:mm\\:ss}";

                                progress?.Report(message);
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                    }
                });

            try
            {

                await process.WaitForExitAsync(
                    cancellationToken);

                if (process.ExitCode != 0)
                {
                    string error =
                        errorBuilder.ToString();

                    throw CreateWhisperException(error);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            finally
            {
                stopwatch.Stop();

                heartbeatCts.Cancel();

                try
                {
                    await heartbeatTask;
                }
                catch
                {
                    // Ignore heartbeat cancellation during cleanup.
                }

                try
                {
                    if (!process.HasExited)
                    {
                        process.Kill(
                            entireProcessTree: true);

                        process.WaitForExit(5000);
                    }
                }
                catch (InvalidOperationException)
                {
                    // Process has already exited.
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    // Process could not be terminated.
                }
            }
        }



        private async Task<List<TranscriptSegment>> LoadTranscriptAsync(string jsonFile, string audioFile, CancellationToken cancellationToken, IProgress<string>? progress)
        {
            if (!File.Exists(jsonFile))
            {
                throw new Exception(
                    "Output JSON not generated.\n\n" +
                    jsonFile);
            }

            string json =
                await File.ReadAllTextAsync(
                    jsonFile,
                    cancellationToken);

            return await ParseWhisperJsonAsync(
                json,
                audioFile,
                progress);
        }

        private async Task<List<TranscriptSegment>> ParseWhisperJsonAsync(string json, string audioFile, IProgress<string>? progress)
        {

            WhisperXResult? result =
                JsonSerializer.Deserialize
                <WhisperXResult>(
                    json);

            if (result == null)
            {
                throw new Exception(
                    "Failed to parse WhisperX JSON.");
            }

            TimeSpan audioDuration;

            using (AudioFileReader reader =
                new AudioFileReader(audioFile))
            {
                audioDuration =
                    reader.TotalTime;
            }

            List<TranscriptSegment>
                segments = new();

            foreach (var phrase
                in result.Phrases)
            {

                double phraseStart =
                        phrase.Words.Count > 0
                            ? phrase.Words.Min(
                                w => w.Start)
                            : phrase.Start;

                phraseStart =
                        Math.Max(
                            0,
                            phraseStart - 0.15);

                double phraseEnd =
                        phrase.Words.Count > 0
                            ? phrase.Words.Max(
                                w => w.End)
                            : phrase.End;
                TranscriptSegment segment =
                    new()
                    {
                        PhraseId =
                            phrase.PhraseId,

                        Text =
                            phrase.Text,


                        Start =
                                TimeSpan.FromSeconds(
                                        phraseStart),

                        End =
                                TimeSpan.FromSeconds(
                                        phraseEnd),

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

            // Remove overlaps between adjacent phrases
            for (int i = 0; i < segments.Count; i++)
            {
                TranscriptSegment current =
                    segments[i];

                if (i < segments.Count - 1)
                {
                    TranscriptSegment next =
                        segments[i + 1];

                    TimeSpan gap =
                        next.Start - current.End;

                    if (gap <= TimeSpan.Zero)
                    {
                        continue;
                    }

                    TimeSpan actualPadding =
                        gap - TimeSpan.FromMilliseconds(20);

                    if (actualPadding < TimeSpan.Zero)
                    {
                        actualPadding =
                            TimeSpan.Zero;
                    }

                    // Allow up to 2 seconds padding
                    if (actualPadding >
                        TimeSpan.FromMilliseconds(500))
                    {
                        actualPadding =
                            TimeSpan.FromMilliseconds(500);
                    }

                    current.End += actualPadding;
                }
                else
                {
                    // Last phrase should extend to end of audio
                    current.End =
                        audioDuration;
                    progress?.Report(
                            $"Last phrase extended to audio end: " +
                            $"{audioDuration.TotalSeconds:F3}");
                }
            }


            List<SilenceRegion> silences =
                await AudioSilenceDetector
                    .DetectAsync(audioFile);

            progress?.Report(
                            $"Detected {silences.Count} silence regions");

            foreach (var s in silences)
            {
                progress?.Report(
                    $"Silence: " +
                    $"{s.Start.TotalSeconds:F3} - " +
                    $"{s.End.TotalSeconds:F3}");
            }

            for (int i = 0; i < segments.Count - 1; i++)
            {
                TranscriptSegment current =
                    segments[i];

                TranscriptSegment next =
                    segments[i + 1];

                SilenceRegion? silence =
                    silences
                        .Where(
                            s =>
                                s.Start >
                                    current.End -
                                    TimeSpan.FromSeconds(1)

                                &&

                                s.End <
                                    next.Start +
                                    TimeSpan.FromMilliseconds(500))
                        .OrderByDescending(
                            s =>
                                (s.End - s.Start).TotalSeconds)
                        .FirstOrDefault();

                if (silence == null)
                {
                    progress?.Report(
                            $"No silence found between:\n" +
                            $"{current.Text}\n" +
                            $"AND\n" +
                            $"{next.Text}");
                    continue;
                }

                TimeSpan midpoint =
                    silence.Start +
                    ((silence.End - silence.Start) / 2);

                progress?.Report(
                            $"Boundary correction: " +
                            $"{current.Text}\n" +
                            $"Old End={current.End.TotalSeconds:F3}\n" +
                            $"Old Next Start={next.Start.TotalSeconds:F3}\n" +
                            $"Silence={silence.Start.TotalSeconds:F3}-{silence.End.TotalSeconds:F3}\n" +
                            $"Midpoint={midpoint.TotalSeconds:F3}");

                current.End = midpoint;

            }

            foreach (var s in segments)
            {
                if (s.End <=
                    s.Start + TimeSpan.FromMilliseconds(100))
                {
                    progress?.Report(
                        $"Removing invalid phrase:\n" +
                        $"{s.Text}\n" +
                        $"Start={s.Start.TotalSeconds:F3}\n" +
                        $"End={s.End.TotalSeconds:F3}");
                }
            }
            // Remove invalid / near-zero phrases
            segments = segments
                .Where(s =>
                    s.End >
                    s.Start +
                    TimeSpan.FromMilliseconds(100))
                .ToList();

            progress?.Report(
                $"Remaining phrases after cleanup: {segments.Count}");

            return segments;
        }

        private static bool IsOutOfMemoryError(string error)
        {
            if (string.IsNullOrWhiteSpace(error))
            {
                return false;
            }

            error = error.ToLowerInvariant();

            return
                error.Contains("mkl_malloc")
                || error.Contains("failed to allocate memory")
                || error.Contains("cannot allocate memory")
                || error.Contains("out of memory")
                || error.Contains("memoryerror")
                || error.Contains("bad_alloc");
        }

        private static Exception CreateWhisperException(
    string error)
        {
            if (IsOutOfMemoryError(error))
            {
                return new Exception(
                @"Unable to load the selected Whisper model.

                The selected Whisper model requires more memory than is currently available on this computer.

                Suggestions:

                • Close other applications.
                • Restart Obi.
                • Select the Medium or Small Whisper model.

                Technical details:

                " + error);
            }

            return new Exception(
                "WhisperX Error:\n\n" +
                error);
        }
    }
}
