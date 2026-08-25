using Obi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Obi.Services
{
    public class ParakeetService : ITranscriptionService
    {
        // --------------------------------------------------
        // CONSTANTS
        // --------------------------------------------------

        private const string OutputFileName =
            "parakeet-output.json";


        // --------------------------------------------------
        // TRANSCRIBE
        // --------------------------------------------------

        public async Task<List<TranscriptSegment>> TranscribeAsync(
            string audioFile,
            TranscriptionOptions options,
            CancellationToken cancellationToken,
            IProgress<string>? progress = null)
        {
            if (string.IsNullOrWhiteSpace(audioFile))
                throw new ArgumentException(
                    "Audio file path is required.",
                    nameof(audioFile));


            if (!File.Exists(audioFile))
                throw new FileNotFoundException(
                    "Audio file was not found.",
                    audioFile);


            if (!File.Exists(ObiPaths.ParakeetPythonExe))
                throw new FileNotFoundException(
                    "Parakeet Python environment was not found.",
                    ObiPaths.ParakeetPythonExe);

            progress?.Report(
    $"Parakeet script path: {ObiPaths.ParakeetScript}");
            if (!File.Exists(ObiPaths.ParakeetScript))
                throw new FileNotFoundException(
                    "Parakeet Python script was not found.",
                    ObiPaths.ParakeetScript);


            if (!File.Exists(ObiPaths.FFmpegExe))
                throw new FileNotFoundException(
                    "FFmpeg executable was not found.",
                    ObiPaths.FFmpegExe);


            cancellationToken.ThrowIfCancellationRequested();


            progress?.Report(
                "Starting Parakeet transcription...");


            // --------------------------------------------------
            // OUTPUT FILE
            // --------------------------------------------------

            string outputFile =
                Path.Combine(
                    Path.GetTempPath(),
                    $"{Guid.NewGuid():N}_{OutputFileName}");


            try
            {
                // --------------------------------------------------
                // LANGUAGE
                // --------------------------------------------------

                string language =
                    string.IsNullOrWhiteSpace(options.Language)
                        ? "auto"
                        : options.Language;


                // --------------------------------------------------
                // PYTHON ARGUMENTS
                // --------------------------------------------------

                string arguments =
                    $"-u " +
                    $"{Quote(ObiPaths.ParakeetScript)} " +
                    $"{Quote(audioFile)} " +
                    $"{Quote(outputFile)} " +
                    $"{Quote(language)} " +
                    $"{Quote(ObiPaths.ModelsFolder)} " +
                    $"{Quote(ObiPaths.HuggingFaceFolder)} " +
                    $"{Quote(ObiPaths.FFmpegExe)}";


                progress?.Report(
                    "Running Parakeet...");


                // --------------------------------------------------
                // START PYTHON
                // --------------------------------------------------

                var startInfo =
                    new ProcessStartInfo
                    {
                        FileName =
                            ObiPaths.ParakeetPythonExe,

                        Arguments =
                            arguments,

                        WorkingDirectory =
                            ObiPaths.PythonBackend,

                        UseShellExecute =
                            false,

                        RedirectStandardOutput =
                            true,

                        RedirectStandardError =
                            true,

                        CreateNoWindow =
                            true
                    };


                using var process =
                    new Process
                    {
                        StartInfo =
                            startInfo,

                        EnableRaisingEvents =
                            true
                    };


                process.OutputDataReceived +=
                    (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            progress?.Report(
                                e.Data);
                        }
                    };


                process.ErrorDataReceived +=
                    (_, e) =>
                    {
                        if (!string.IsNullOrWhiteSpace(e.Data))
                        {
                            progress?.Report(
                                e.Data);
                        }
                    };


                if (!process.Start())
                {
                    throw new InvalidOperationException(
                        "Failed to start Parakeet Python process.");
                }


                process.BeginOutputReadLine();

                process.BeginErrorReadLine();


                // --------------------------------------------------
                // WAIT WITH CANCELLATION
                // --------------------------------------------------

                await process.WaitForExitAsync(
                    cancellationToken);


                cancellationToken.ThrowIfCancellationRequested();


                // --------------------------------------------------
                // CHECK EXIT CODE
                // --------------------------------------------------

                if (process.ExitCode != 0)
                {
                    throw new InvalidOperationException(
                        $"Parakeet transcription failed. " +
                        $"Python process exited with code " +
                        $"{process.ExitCode}.");
                }


                // --------------------------------------------------
                // CHECK OUTPUT
                // --------------------------------------------------

                if (!File.Exists(outputFile))
                {
                    throw new InvalidOperationException(
                        "Parakeet completed successfully, " +
                        "but did not create the expected output JSON.");
                }


                progress?.Report(
                    "Reading Parakeet transcription...");


                // --------------------------------------------------
                // READ JSON
                // --------------------------------------------------

                string json =
                    await File.ReadAllTextAsync(
                        outputFile,
                        cancellationToken);


                return ParseResult(
                    json);
            }
            finally
            {
                // --------------------------------------------------
                // CLEAN TEMP FILE
                // --------------------------------------------------

                try
                {
                    if (File.Exists(outputFile))
                    {
                        File.Delete(outputFile);
                    }
                }
                catch
                {
                    // Ignore cleanup errors.
                }
            }
        }


        // --------------------------------------------------
        // BATCH
        // --------------------------------------------------

        public async Task<
            Dictionary<string, List<TranscriptSegment>>>
            TranscribeBatchAsync(
                List<string> audioFiles,
                TranscriptionOptions options,
                CancellationToken cancellationToken,
                IProgress<string>? progress = null)
        {
            var result =
                new Dictionary<
                    string,
                    List<TranscriptSegment>>();


            foreach (string audioFile in audioFiles)
            {
                cancellationToken.ThrowIfCancellationRequested();


                List<TranscriptSegment> segments =
                    await TranscribeAsync(
                        audioFile,
                        options,
                        cancellationToken,
                        progress);


                result[audioFile] =
                    segments;
            }


            return result;
        }


        // --------------------------------------------------
        // PARSE JSON
        // --------------------------------------------------

        private static List<TranscriptSegment> ParseResult(
            string json)
        {
            var result =
                new List<TranscriptSegment>();


            using JsonDocument document =
                JsonDocument.Parse(json);


            JsonElement root =
                document.RootElement;


            if (!root.TryGetProperty(
                    "segments",
                    out JsonElement segmentsElement))
            {
                throw new InvalidOperationException(
                    "Parakeet output JSON does not contain " +
                    "a 'segments' property.");
            }


            int phraseNumber = 1;


            foreach (JsonElement segmentElement
                in segmentsElement.EnumerateArray())
            {
                string text =
                    segmentElement
                        .GetProperty("text")
                        .GetString()
                    ?? string.Empty;


                double start =
                    segmentElement
                        .GetProperty("start")
                        .GetDouble();


                double end =
                    segmentElement
                        .GetProperty("end")
                        .GetDouble();


                var words =
                    new List<WordTimestamp>();


                if (segmentElement.TryGetProperty(
                        "words",
                        out JsonElement wordsElement))
                {
                    foreach (JsonElement wordElement
                        in wordsElement.EnumerateArray())
                    {
                        string word =
                            wordElement
                                .GetProperty("word")
                                .GetString()
                            ?? string.Empty;


                        double wordStart =
                            wordElement
                                .GetProperty("start")
                                .GetDouble();


                        double wordEnd =
                            wordElement
                                .GetProperty("end")
                                .GetDouble();


                        words.Add(
                            new WordTimestamp
                            {
                                Word =
                                    word,

                                Start =
                                    wordStart,

                                End =
                                    wordEnd
                            });
                    }
                }


                result.Add(
                    new TranscriptSegment
                    {
                        PhraseId =
                            $"p{phraseNumber++}",

                        Text =
                            text,

                        Start =
                            TimeSpan.FromSeconds(
                                start),

                        End =
                            TimeSpan.FromSeconds(
                                end),

                        Words =
                            words,

                        Confidence =
                            1.0
                    });
            }


            return result;
        }


        // --------------------------------------------------
        // QUOTE ARGUMENT
        // --------------------------------------------------

        private static string Quote(
            string value)
        {
            if (value == null)
                return "\"\"";


            return "\"" +
                value.Replace(
                    "\"",
                    "\\\"") +
                "\"";
        }
    }
}
