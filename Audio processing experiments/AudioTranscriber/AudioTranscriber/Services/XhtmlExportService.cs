using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AudioTranscriber.Models;
using System.Security;

namespace AudioTranscriber.Services
{
    public static class XhtmlExportService
    {
        public static async Task SaveAsync(
            List<TranscriptSegment> segments,
            string outputPath)
        {
            StringBuilder sb =
                new();

            sb.AppendLine(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            sb.AppendLine(
                "<!DOCTYPE html>");

            sb.AppendLine(
                "<html xmlns=\"http://www.w3.org/1999/xhtml\">");

            sb.AppendLine("<head>");

            sb.AppendLine(
                "<meta charset=\"utf-8\" />");

            sb.AppendLine(
                "<title>Transcript</title>");

            sb.AppendLine("<style>");

            sb.AppendLine(@"
body
{
    font-family: Arial, sans-serif;
    line-height: 1.6;
    padding: 20px;
}

.phrase
{
    margin-bottom: 12px;
}
");

            sb.AppendLine("</style>");

            sb.AppendLine("</head>");

            sb.AppendLine("<body>");

            int counter = 1;

            foreach (var segment
                in segments)
            {
                string safeText =
                    SecurityElement.Escape(
                        segment.Text);

                sb.AppendLine(
                    $@"<p class=""phrase""
    data-phraseid=""p{counter}""
    data-start=""{segment.Start.TotalSeconds:F3}""
    data-end=""{segment.End.TotalSeconds:F3}"">
    {safeText}
</p>");

                counter++;
            }

            sb.AppendLine("</body>");

            sb.AppendLine("</html>");

            await File.WriteAllTextAsync(
                outputPath,
                sb.ToString(),
                Encoding.UTF8);
        }
    }
}
