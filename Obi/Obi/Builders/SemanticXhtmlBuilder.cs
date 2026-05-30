using Obi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Builders
{
    public class SemanticXhtmlBuilder
    {
        public string Build(
            List<PhraseModel> phrases,
            List<StructureItem> structure)
        {
            //--------------------------------------------------
            // MAP STRUCTURE
            //--------------------------------------------------

            var structureMap =
                structure.ToDictionary(
                    x => x.PhraseId);

            //--------------------------------------------------
            // XHTML BUILDER
            //--------------------------------------------------

            var sb = new StringBuilder();

            //--------------------------------------------------
            // XML HEADER
            //--------------------------------------------------

            sb.AppendLine(
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>");

            sb.AppendLine(
    @"<html xmlns=""http://www.w3.org/1999/xhtml"">");

            //--------------------------------------------------
            // HEAD
            //--------------------------------------------------

            sb.AppendLine("<head>");

            sb.AppendLine(
    @"<link
    rel=""stylesheet""
    type=""text/css""
    href=""structure.css"" />");

            sb.AppendLine("</head>");

            //--------------------------------------------------
            // BODY
            //--------------------------------------------------

            sb.AppendLine("<body>");

            //--------------------------------------------------
            // COUNTERS
            //--------------------------------------------------

            int headingCounter = 1;

            int phraseCounter = 1;

            //--------------------------------------------------
            // FIRST TITLE FLAG
            //--------------------------------------------------

            bool firstPhrase = true;

            //--------------------------------------------------
            // PROCESS PHRASES
            //--------------------------------------------------

            foreach (var phrase in phrases)
            {
                //--------------------------------------------------
                // GET STRUCTURE
                //--------------------------------------------------

                StructureItem? item = null;

                if (structureMap.ContainsKey(
                    phrase.PhraseId))
                {
                    item =
                        structureMap[
                            phrase.PhraseId];
                }

                //--------------------------------------------------
                // HEADING?
                //--------------------------------------------------

                bool isHeading =
                    firstPhrase
                    ||
                    (
                        item != null
                        && item.Role == "heading"
                    );

                //--------------------------------------------------
                // WRITE HEADING
                //--------------------------------------------------

                if (isHeading)
                {
                    //--------------------------------------------------
                    // LEVEL
                    //--------------------------------------------------

                    int level = 1;

                    if (!firstPhrase
                        &&
                        item != null
                        &&
                        item.Level != null)
                    {
                        level =
                            item.Level.Value;
                    }

                    level =
                        Math.Max(
                            1,
                            Math.Min(6, level));

                    //--------------------------------------------------
                    // CSS CLASS
                    //--------------------------------------------------

                    string cssClass =
                        firstPhrase
                        ? "title"
                        : "section";

                    //--------------------------------------------------
                    // ID
                    //--------------------------------------------------

                    string headingId =
                        $"h{headingCounter++}";

                    //--------------------------------------------------
                    // CLEAN HEADING TEXT
                    //--------------------------------------------------

                    string headingText =
                        phrase.Text;

                    if (item != null
                        &&
                        !string.IsNullOrWhiteSpace(
                            item.HeadingText))
                    {
                        headingText =
                            item.HeadingText;
                    }

                    //--------------------------------------------------
                    // WRITE HEADING
                    //--------------------------------------------------

                    sb.AppendLine(
    $"""
<h{level}
    class="{cssClass}"
    id="{headingId}"
    data-start="{phrase.Start:0.000}"
    data-end="{phrase.End:0.000}">

    {Escape(headingText)}

</h{level}>
""");
                }

                //--------------------------------------------------
                // ALWAYS WRITE PHRASE
                //--------------------------------------------------

                string phraseId =
                    $"p{phraseCounter++}";

                sb.AppendLine(
    $"""
<p
    class="phrase"
    id="{phraseId}"
    data-start="{phrase.Start:0.000}"
    data-end="{phrase.End:0.000}">

    {Escape(phrase.Text)}

</p>
""");

                //--------------------------------------------------
                // TITLE COMPLETE
                //--------------------------------------------------

                firstPhrase = false;
            }

            //--------------------------------------------------
            // CLOSE BODY
            //--------------------------------------------------

            sb.AppendLine("</body>");

            //--------------------------------------------------
            // CLOSE HTML
            //--------------------------------------------------

            sb.AppendLine("</html>");

            return sb.ToString();
        }

        //--------------------------------------------------
        // XML ESCAPE
        //--------------------------------------------------

        private string Escape(
            string text)
        {
            return System.Security
                .SecurityElement
                .Escape(text)
                ?? "";
        }
    }
}
