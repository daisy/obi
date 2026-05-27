using AudioTranscriber.Models;
using System.Text;
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
            "<html xmlns=\"http://www.w3.org/1999/xhtml\">");

        sb.AppendLine("<head>");

        sb.AppendLine(
            "<meta charset=\"utf-8\" />");

        sb.AppendLine(
            "<title>Transcript</title>");

        sb.AppendLine("</head>");

        sb.AppendLine("<body>");

        foreach (var segment in segments)
        {
            sb.AppendLine(
                $"<p id=\"{segment.PhraseId}\" " +
                $"class=\"phrase\" " +
                $"data-start=\"{segment.Start.TotalSeconds:F3}\" " +
                $"data-end=\"{segment.End.TotalSeconds:F3}\" " +
                $"data-confidence=\"{segment.Confidence:F3}\">"
            );

            foreach (var word in segment.Words)
            {
                sb.Append(
                    $"<span class=\"word\" " +
                    $"data-start=\"{word.Start:F3}\" " +
                    $"data-end=\"{word.End:F3}\" " +
                    $"data-confidence=\"{word.Confidence:F3}\">"
                );

                sb.Append(
                    System.Net.WebUtility.HtmlEncode(
                        word.Word));

                sb.Append("</span> ");
            }

            sb.AppendLine("</p>");
        }

        sb.AppendLine("</body>");

        sb.AppendLine("</html>");

        await File.WriteAllTextAsync(
            outputPath,
            sb.ToString(),
            Encoding.UTF8);
    }
}
