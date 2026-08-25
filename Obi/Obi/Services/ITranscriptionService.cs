using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Obi.Models;

namespace Obi.Services
{
    public interface ITranscriptionService
    {
        Task<List<TranscriptSegment>> TranscribeAsync(
            string audioFile,
            TranscriptionOptions options,
            CancellationToken cancellationToken,
            IProgress<string>? progress = null);

        Task<Dictionary<string, List<TranscriptSegment>>> TranscribeBatchAsync(
            List<string> audioFiles,
            TranscriptionOptions options,
            CancellationToken cancellationToken,
            IProgress<string>? progress = null);
    }
}