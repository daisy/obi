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
            // Normalize language once.
            // ------------------------------------------------------

            string language =
                string.IsNullOrWhiteSpace(options.Language)
                    ? "auto"
                    : options.Language
                        .Trim()
                        .ToLowerInvariant();


            // ------------------------------------------------------
            // Explicit Parakeet selection
            //
            // Parakeet must never be allowed to run with a language
            // that it does not support.
            //
            // "auto" is allowed because Parakeet can perform its own
            // language detection when explicitly selected.
            // ------------------------------------------------------

            if (requestedEngine ==
                TranscriptionEngine.Parakeet)
            {
                if (language != "auto" &&
                    !ParakeetLanguages.SupportedCodes.Contains(
                        language))
                {
                    throw new InvalidOperationException(
                        $"Parakeet does not support the selected " +
                        $"language '{language}'.");
                }


                return TranscriptionEngine.Parakeet;
            }


            // ------------------------------------------------------
            // Explicit Whisper selection
            // ------------------------------------------------------

            if (requestedEngine ==
                TranscriptionEngine.Whisper)
            {
                return TranscriptionEngine.Whisper;
            }


            // ------------------------------------------------------
            // Auto engine
            //
            // Normally ImportAudioUsingWhisper resolves Auto + Auto
            // before calling the coordinator.
            //
            // If Auto reaches this method with a concrete language,
            // use Parakeet when supported and Whisper otherwise.
            // ------------------------------------------------------

            progress?.Report(
                "Transcription engine: Auto");


            if (language == "auto")
            {
                // --------------------------------------------------
                // Defensive fallback.
                //
                // The normal Auto + Auto path is resolved by
                // ImportAudioUsingWhisper.ResolveAutomaticEngineAsync()
                // using WhisperX language detection.
                //
                // If the coordinator receives Auto + Auto directly,
                // Parakeet remains the default Auto engine.
                // --------------------------------------------------

                progress?.Report(
                    "Book language: Auto Detect");

                progress?.Report(
                    "Auto selected Parakeet.");

                return TranscriptionEngine.Parakeet;
            }


            // ------------------------------------------------------
            // Concrete language supported by Parakeet
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
            // Concrete language NOT supported by Parakeet
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