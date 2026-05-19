using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AudioTranscriber.Models;
namespace AudioTranscriber.Services
{
    public static class ExportService
    {
        public static async Task SaveTxtAsync(
            List<TranscriptSegment> segments,
            string outputPath)
        {
            StringBuilder sb =
                new();

            foreach (var segment
                in segments)
            {
                sb.AppendLine(
                    $"[{segment.Start:hh\\:mm\\:ss\\.fff} - " +
                    $"{segment.End:hh\\:mm\\:ss\\.fff}]");

                sb.AppendLine(
                    segment.Text);

                sb.AppendLine();
            }

            await File.WriteAllTextAsync(
                outputPath,
                sb.ToString());
        }
    }
}