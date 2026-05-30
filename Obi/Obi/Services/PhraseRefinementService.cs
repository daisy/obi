using Obi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Obi.Services
{
    public class PhraseRefinementService
    {
        public List<PhraseModel> Refine(
            List<PhraseModel> phrases)
        {
            var result =
                new List<PhraseModel>();

            int counter = 1;

            foreach (var phrase in phrases)
            {
                //--------------------------------------------------
                // CLEAN TEXT
                //--------------------------------------------------

                string text =
                    phrase.Text.Trim();

                //--------------------------------------------------
                // SPLIT PAGE MARKERS
                //--------------------------------------------------

                var parts =
                    Regex.Split(
                        text,
                        @"(?=Page\s+\d+\.?)",
                        RegexOptions.IgnoreCase);

                //--------------------------------------------------
                // NO SPLIT
                //--------------------------------------------------

                if (parts.Length <= 1)
                {
                    phrase.PhraseId =
                        $"p{counter++}";

                    result.Add(phrase);

                    continue;
                }

                //--------------------------------------------------
                // SPLIT INTO SUBPHRASES
                //--------------------------------------------------

                double totalDuration =
                    phrase.End - phrase.Start;

                double segmentDuration =
                    totalDuration / parts.Length;

                for (int i = 0;
                     i < parts.Length;
                     i++)
                {
                    string part =
                        parts[i].Trim();

                    if (string.IsNullOrWhiteSpace(part))
                    {
                        continue;
                    }

                    //--------------------------------------------------
                    // CREATE NEW PHRASE
                    //--------------------------------------------------

                    result.Add(
                        new PhraseModel
                        {
                            PhraseId =
                                $"p{counter++}",

                            Start =
                                phrase.Start
                                + (i * segmentDuration),

                            End =
                                phrase.Start
                                + ((i + 1)
                                   * segmentDuration),

                            Text = part
                        });
                }
            }

            return result;
        }
    }
}
