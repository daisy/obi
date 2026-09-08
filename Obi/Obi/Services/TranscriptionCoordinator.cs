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
            switch (engine)
            {
                // --------------------------------------------------
                // PARAKEET
                // --------------------------------------------------

                case TranscriptionEngine.Parakeet:

                    progress?.Report(
                        "Transcription engine: Parakeet");

                    return await _parakeetService
                        .TranscribeAsync(
                            audioFile,
                            options,
                            cancellationToken,
                            progress);


                // --------------------------------------------------
                // WHISPER
                // --------------------------------------------------

                case TranscriptionEngine.Whisper:

                    progress?.Report(
                        "Transcription engine: Whisper");

                    return await _whisperService
                        .TranscribeAsync(
                            audioFile,
                            options,
                            cancellationToken,
                            progress);


                // --------------------------------------------------
                // AUTO
                //
                // Auto is no longer exposed by the UI.
                // It is retained in the enum only for compatibility
                // with other parts of the project.
                // --------------------------------------------------

                case TranscriptionEngine.Auto:

                    throw new InvalidOperationException(
                        "Automatic transcription engine selection " +
                        "is no longer supported.");


                // --------------------------------------------------
                // UNKNOWN
                // --------------------------------------------------

                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(engine),
                        engine,
                        "Unknown transcription engine.");
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
            switch (engine)
            {
                // --------------------------------------------------
                // PARAKEET
                // --------------------------------------------------

                case TranscriptionEngine.Parakeet:

                    progress?.Report(
                        "Transcription engine: Parakeet");

                    return await _parakeetService
                        .TranscribeBatchAsync(
                            audioFiles,
                            options,
                            cancellationToken,
                            progress);


                // --------------------------------------------------
                // WHISPER
                // --------------------------------------------------

                case TranscriptionEngine.Whisper:

                    progress?.Report(
                        "Transcription engine: Whisper");

                    return await _whisperService
                        .TranscribeBatchAsync(
                            audioFiles,
                            options,
                            cancellationToken,
                            progress);


                // --------------------------------------------------
                // AUTO
                //
                // Auto is no longer exposed by the UI.
                // It is retained in the enum only for compatibility
                // with other parts of the project.
                // --------------------------------------------------

                case TranscriptionEngine.Auto:

                    throw new InvalidOperationException(
                        "Automatic transcription engine selection " +
                        "is no longer supported.");


                // --------------------------------------------------
                // UNKNOWN
                // --------------------------------------------------

                default:

                    throw new ArgumentOutOfRangeException(
                        nameof(engine),
                        engine,
                        "Unknown transcription engine.");
            }
        }
    }
}