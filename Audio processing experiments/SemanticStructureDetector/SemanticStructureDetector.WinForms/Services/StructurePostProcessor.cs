using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Services;

public class StructurePostProcessor
{
    //--------------------------------------------------
    // MAIN ENTRY
    //--------------------------------------------------

    public List<StructureItem> Process(
     List<PhraseModel> phrases,
     List<StructureItem> structure)
    {
        var phraseMap =
            phrases.ToDictionary(
                x => x.PhraseId);

        foreach (var item in structure)
        {
            if (!phraseMap.ContainsKey(
                item.PhraseId))
            {
                continue;
            }

            var phrase =
                phraseMap[item.PhraseId];

            string text =
                phrase.Text.Trim();

            //--------------------------------------------------
            // METADATA-LIKE TITLE
            //--------------------------------------------------

            if (text.Contains(",")
                && text.Length > 40)
            {
                item.Role = "paragraph";
                item.Level = null;
            }

            //--------------------------------------------------
            // EXTREMELY LONG
            //--------------------------------------------------

            if (text.Length > 120)
            {
                item.Role = "paragraph";
                item.Level = null;
            }

            //--------------------------------------------------
            // LOW CONFIDENCE
            //--------------------------------------------------

            if (item.Confidence < 0.50)
            {
                item.Role = "paragraph";
                item.Level = null;
            }
        }

        //--------------------------------------------------
        // FIRST HEADING = TITLE
        //--------------------------------------------------

        var firstHeading =
            structure.FirstOrDefault(
                x => x.Role == "heading");

        if (firstHeading != null)
        {
            firstHeading.Level = 1;
        }

        //--------------------------------------------------
        // PATTERN-BASED TOC / INDEX DETECTION
        //--------------------------------------------------

        int tocLikeCount = 0;

        int indexLikeCount = 0;

        bool insideTocRegion = false;

        bool insideIndexRegion = false;

        foreach (var item in structure)
        {
            if (!phraseMap.ContainsKey(
                    item.PhraseId))
            {
                continue;
            }

            var phrase =
                phraseMap[item.PhraseId];

            string transcript =
                phrase.Text.Trim();

            //--------------------------------------------------
            // PAGE REFERENCES
            //--------------------------------------------------

            bool hasPageReference =
                transcript.Contains(
                    "page",
                    StringComparison
                        .OrdinalIgnoreCase)
                ||
                transcript.Contains(
                    "pages",
                    StringComparison
                        .OrdinalIgnoreCase);

            //--------------------------------------------------
            // SHORT ENTRY
            //--------------------------------------------------

            bool shortEntry =
                transcript.Length < 80;

            //--------------------------------------------------
            // COMMA HEAVY
            //--------------------------------------------------

            bool commaHeavy =
                transcript.Count(
                    c => c == ',')
                >= 1;

            //--------------------------------------------------
            // TOC-LIKE ENTRY
            //--------------------------------------------------

            bool tocLike =
                hasPageReference
                &&
                shortEntry
                &&
                transcript.StartsWith(
                    "Page",
                    StringComparison
                        .OrdinalIgnoreCase);

            //--------------------------------------------------
            // INDEX-LIKE ENTRY
            //--------------------------------------------------

            bool indexLike =
                hasPageReference
                &&
                commaHeavy;

            //--------------------------------------------------
            // TRACK REGIONS
            //--------------------------------------------------

            if (tocLike)
            {
                tocLikeCount++;
            }
            else
            {
                tocLikeCount = 0;
            }

            if (indexLike)
            {
                indexLikeCount++;
            }
            else
            {
                indexLikeCount = 0;
            }

            //--------------------------------------------------
            // ENTER TOC REGION
            //--------------------------------------------------

            if (tocLikeCount >= 3)
            {
                insideTocRegion = true;
            }

            //--------------------------------------------------
            // ENTER INDEX REGION
            //--------------------------------------------------

            if (indexLikeCount >= 3)
            {
                insideIndexRegion = true;
            }

            //--------------------------------------------------
            // EXIT TOC REGION
            //--------------------------------------------------

            if (insideTocRegion
                &&
                transcript.Length > 120
                &&
                !hasPageReference)
            {
                insideTocRegion = false;
            }

            //--------------------------------------------------
            // EXIT INDEX REGION
            //--------------------------------------------------

            if (insideIndexRegion
                &&
                transcript.Length > 150
                &&
                !hasPageReference)
            {
                insideIndexRegion = false;
            }

            //--------------------------------------------------
            // DOWNGRADE TOC HEADINGS
            //--------------------------------------------------

            if (insideTocRegion
                &&
                item.Role == "heading")
            {
                item.Role = "paragraph";

                item.Level = null;

                item.HeadingText = "";
            }

            //--------------------------------------------------
            // DOWNGRADE INDEX HEADINGS
            //--------------------------------------------------

            if (insideIndexRegion
                &&
                item.Role == "heading")
            {
                item.Role = "paragraph";

                item.Level = null;

                item.HeadingText = "";
            }
        }


        return structure;
    }
}