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
        public async Task<List<TranscriptSegment>>
            TranscribeAsync(
                string audioFile,
                CancellationToken cancellationToken,
                IProgress<string>? progress = null)
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
                    Guid.NewGuid().ToString() + ".json");


            string pythonExe =
                Path.GetFullPath(
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "..",
                    "..",
                    "..",
                    "..",
                    "whisperx_env",
                    "Scripts",
                    "python.exe"));


            if (!await WhisperXInstallerService.IsPythonEnvironmentInstalledAsync())
            {
                throw new Exception(
                    "WhisperX is not installed.");
            }

            //if (!File.Exists(pythonExe))
            //{
            //    throw new Exception(
            //        "AI Transcription support is not installed.\n\n" +
            //        "Expected:\n" +
            //        pythonExe +
            //        "\n\n" +
            //        "Please install WhisperX support first.");
            //}

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


            string ffmpegFolder =  Path.Combine( AppDomain.CurrentDomain.BaseDirectory,"ffmpeg");

            string ffmpegExe =
            Path.Combine(
                ffmpegFolder,
                "ffmpeg.exe");

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



            using Process process =
                new();
            //process.StartInfo.FileName = ffmpegPath;
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
                        progress?.Report(
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

            try
            {
                File.Delete(jsonOutput);
            }
            catch
            {
            }

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

                current.End =  midpoint;

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
    }
}
