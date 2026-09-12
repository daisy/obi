using Obi.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Obi.Services
{
    public class AudioTranscriptionService
    {
        private readonly TranscriptionCoordinator _transcriptionCoordinator;

        public AudioTranscriptionService()
        {
            _transcriptionCoordinator =
                new TranscriptionCoordinator(
                    new WhisperXService(),
                    new ParakeetService());
        }


        public async Task<List<TranscriptSegment>> TranscribeAsync(
            string audioFile,
            TranscriptionSettings settings,
            CancellationToken cancellationToken,
            IProgress<string>? progress = null)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(
                    nameof(settings));
            }


            TranscriptionOptions options = new()
            {
                WhisperModel =
                    settings.WhisperModel,

                Language =
                    settings.Language
            };


            return await _transcriptionCoordinator
                .TranscribeAsync(
                    audioFile,
                    settings.Engine,
                    options,
                    cancellationToken,
                    progress);
        }
    }
}