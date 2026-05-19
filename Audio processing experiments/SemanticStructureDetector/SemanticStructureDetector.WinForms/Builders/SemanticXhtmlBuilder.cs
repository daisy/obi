using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Builders;

public class SemanticXhtmlBuilder
{
    public string Build(
        List<PhraseModel> phrases,
        List<StructureItem> structure)
    {
        var structureMap =
            structure.ToDictionary(
                x => x.PhraseId);

        var sb = new StringBuilder();

        //--------------------------------------------------
        // XHTML HEADER
        //--------------------------------------------------

        sb.AppendLine(
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>");

        sb.AppendLine(
@"<html xmlns=""http://www.w3.org/1999/xhtml"">");

        sb.AppendLine("<body>");

        //--------------------------------------------------
        // STATE
        //--------------------------------------------------

        string? currentParagraphId = null;

        bool paragraphOpen = false;

        //--------------------------------------------------
        // PROCESS PHRASES
        //--------------------------------------------------

        foreach (var phrase in phrases)
        {
            if (!structureMap.ContainsKey(
                phrase.PhraseId))
            {
                continue;
            }

            var item =
                structureMap[phrase.PhraseId];

            //--------------------------------------------------
            // CLOSE PARAGRAPH BEFORE HEADING
            //--------------------------------------------------

            if (item.Role == "heading")
            {
                if (paragraphOpen)
                {
                    sb.AppendLine("</p>");
                    paragraphOpen = false;
                }

                int level =
                    item.Level ?? 1;

                level =
                    Math.Max(
                        1,
                        Math.Min(6, level));

                sb.AppendLine(
$"""
<h{level}
    data-phraseid="{Escape(phrase.PhraseId)}"
    data-start="{phrase.Start:0.000}"
    data-end="{phrase.End:0.000}">
    {Escape(phrase.Text)}
</h{level}>
""");

                continue;
            }

            //--------------------------------------------------
            // OPEN PARAGRAPH
            //--------------------------------------------------

            if (!paragraphOpen
                ||
                currentParagraphId
                    != item.ParagraphId)
            {
                if (paragraphOpen)
                {
                    sb.AppendLine("</p>");
                }

                sb.AppendLine("<p>");

                paragraphOpen = true;

                currentParagraphId =
                    item.ParagraphId;
            }

            //--------------------------------------------------
            // PHRASE SPAN
            //--------------------------------------------------

            sb.AppendLine(
$"""
<span
    data-phraseid="{Escape(phrase.PhraseId)}"
    data-start="{phrase.Start:0.000}"
    data-end="{phrase.End:0.000}">
    {Escape(phrase.Text)}
</span>
""");
        }

        //--------------------------------------------------
        // CLOSE LAST PARAGRAPH
        //--------------------------------------------------

        if (paragraphOpen)
        {
            sb.AppendLine("</p>");
        }

        //--------------------------------------------------
        // CLOSE XHTML
        //--------------------------------------------------

        sb.AppendLine("</body>");
        sb.AppendLine("</html>");

        return sb.ToString();
    }

    //--------------------------------------------------
    // XML ESCAPE
    //--------------------------------------------------

    private string Escape(string text)
    {
        return System.Security.SecurityElement
            .Escape(text)
            ?? "";
    }
}