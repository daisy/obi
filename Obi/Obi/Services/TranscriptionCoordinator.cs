using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Obi.Models;

namespace Obi.Services
{
    public class TranscriptionCoordinator
    {
        private readonly ITranscriptionService _whisperService;

        private readonly ITranscriptionService _parakeetService;


        public TranscriptionCoordinator(
            ITranscriptionService whisperService,
            ITranscriptionService parakeetService)
        {
            _whisperService =
                whisperService;

            _parakeetService =
                parakeetService;
        }


        // ==========================================================
        // SINGLE FILE
        // ==========================================================

        public async Task<List<TranscriptSegment>> TranscribeAsync(
            string audioFile,
            TranscriptionEngine engine,
            TranscriptionOptions options,
            CancellationToken cancellationToken,
            IProgress<string>? progress = null)
        {
            TranscriptionEngine selectedEngine =
                ResolveEngine(
                    engine,
                    options,
                    progress);


            switch (selectedEngine)
            {
                case TranscriptionEngine.Parakeet:

                    progress?.Report(
                        "Transcription engine: Parakeet");

                    return await _parakeetService
                        .TranscribeAsync(
                            audioFile,
                            options,
                            cancellationToken,
                            progress);


                case TranscriptionEngine.Whisper:

                    progress?.Report(
                        "Transcription engine: Whisper");

                    return await _whisperService
                        .TranscribeAsync(
                            audioFile,
                            options,
                            cancellationToken,
                            progress);


                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(selectedEngine),
                        selectedEngine,
                        "Invalid transcription engine selection.");
            }
        }


        // ==========================================================
        // BATCH
        // ==========================================================

        public async Task<
            Dictionary<string, List<TranscriptSegment>>>
            TranscribeBatchAsync(
                List<string> audioFiles,
                TranscriptionEngine engine,
                TranscriptionOptions options,
                CancellationToken cancellationToken,
                IProgress<string>? progress = null)
        {
            TranscriptionEngine selectedEngine =
                ResolveEngine(
                    engine,
                    options,
                    progress);


            switch (selectedEngine)
            {
                case TranscriptionEngine.Parakeet:

                    progress?.Report(
                        "Transcription engine: Parakeet");

                    return await _parakeetService
                        .TranscribeBatchAsync(
                            audioFiles,
                            options,
                            cancellationToken,
                            progress);


                case TranscriptionEngine.Whisper:

                    progress?.Report(
                        "Transcription engine: Whisper");

                    return await _whisperService
                        .TranscribeBatchAsync(
                            audioFiles,
                            options,
                            cancellationToken,
                            progress);


                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(selectedEngine),
                        selectedEngine,
                        "Invalid transcription engine selection.");
            }
        }


        // ==========================================================
        // ENGINE RESOLUTION
        // ==========================================================

        private static TranscriptionEngine ResolveEngine(
            TranscriptionEngine requestedEngine,
            TranscriptionOptions options,
            IProgress<string>? progress)
        {
            // ------------------------------------------------------
            // Explicit engine selection always wins.
            // ------------------------------------------------------

            if (requestedEngine != TranscriptionEngine.Auto)
            {
                return requestedEngine;
            }


            progress?.Report(
                "Transcription engine: Auto");


            string language =
                string.IsNullOrWhiteSpace(options.Language)
                    ? "auto"
                    : options.Language.Trim().ToLowerInvariant();


            // ------------------------------------------------------
            // Auto language
            //
            // Parakeet v3 performs its own language detection and
            // supports 25 languages.
            //
            // Therefore Auto + Auto uses Parakeet.
            // ------------------------------------------------------

            if (language == "auto")
            {
                progress?.Report(
                    "Book language: Auto Detect");

                progress?.Report(
                    "Auto selected Parakeet " +
                    "(built-in multilingual language detection).");

                return TranscriptionEngine.Parakeet;
            }


            // ------------------------------------------------------
            // Explicit language
            //
            // Prefer Parakeet when the selected language is one of
            // the languages supported by Parakeet v3.
            // ------------------------------------------------------

            if (ParakeetLanguages.SupportedCodes.Contains(
                language))
            {
                progress?.Report(
                    $"Book language: {language}");

                progress?.Report(
                    "Auto selected Parakeet " +
                    $"for language '{language}'.");

                return TranscriptionEngine.Parakeet;
            }


            // ------------------------------------------------------
            // Language is not supported by Parakeet.
            //
            // WhisperX remains the fallback engine.
            // ------------------------------------------------------

            progress?.Report(
                $"Book language '{language}' " +
                "is not supported by Parakeet.");

            progress?.Report(
                "Auto selected WhisperX.");

            return TranscriptionEngine.Whisper;
        }
    }
}