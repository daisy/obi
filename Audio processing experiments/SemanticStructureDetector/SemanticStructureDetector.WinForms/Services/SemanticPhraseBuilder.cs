using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Services;

public class SemanticPhraseBuilder
{
    //--------------------------------------------------
    // BUILD
    //--------------------------------------------------

    public List<PhraseModel> Build(
        List<PhraseModel> phrases)
    {
        var result =
            new List<PhraseModel>();

        PhraseModel? current =
            null;

        foreach (var phrase in phrases)
        {
            //--------------------------------------------------
            // START
            //--------------------------------------------------

            if (current == null)
            {
                current =
                    ClonePhrase(phrase);

                continue;
            }

            //--------------------------------------------------
            // MERGE?
            //--------------------------------------------------

            if (ShouldMerge(
                    current,
                    phrase))
            {
                MergePhrase(
                    current,
                    phrase);
            }
            else
            {
                result.Add(current);

                current =
                    ClonePhrase(phrase);
            }
        }

        //--------------------------------------------------
        // LAST
        //--------------------------------------------------

        if (current != null)
        {
            result.Add(current);
        }

        //--------------------------------------------------
        // CLEAN IDS
        //--------------------------------------------------

        for (int i = 0;
             i < result.Count;
             i++)
        {
            result[i].PhraseId =
                $"p{i + 1}";
        }

        return result;
    }

    //--------------------------------------------------
    // SHOULD MERGE
    //--------------------------------------------------

    private bool ShouldMerge(
        PhraseModel current,
        PhraseModel next)
    {
        string currentText =
            current.Text.Trim();

        string nextText =
            next.Text.Trim();

        //--------------------------------------------------
        // EMPTY
        //--------------------------------------------------

        if (string.IsNullOrWhiteSpace(
                currentText)
            ||
            string.IsNullOrWhiteSpace(
                nextText))
        {
            return false;
        }

        //--------------------------------------------------
        // HARD SENTENCE END
        //--------------------------------------------------

        if (EndsSentence(
                currentText))
        {
            return false;
        }

        //--------------------------------------------------
        // LOWERCASE CONTINUATION
        //--------------------------------------------------

        if (char.IsLower(
                nextText[0]))
        {
            return true;
        }

        //--------------------------------------------------
        // CONTINUATION WORDS
        //--------------------------------------------------

        //--------------------------------------------------
        // NEXT STARTS LOWERCASE
        //--------------------------------------------------

        if (!string.IsNullOrWhiteSpace(
                nextText)
            &&
            char.IsLower(nextText[0]))
        {
            return true;
        }

        //--------------------------------------------------
        // CURRENT ENDS WITHOUT
        // TERMINAL PUNCTUATION
        //--------------------------------------------------

        char lastChar =
            currentText[
                currentText.Length - 1];

        bool softEnding =
            !".?!:;".Contains(
                lastChar);

        if (softEnding)
        {
            //--------------------------------------------------
            // SMALL GAP
            //--------------------------------------------------

            double smallGap =
                next.Start - current.End;

            if (smallGap < 0.5)
            {
                return true;
            }
        }
        //--------------------------------------------------
        // SHORT CURRENT PHRASE
        //--------------------------------------------------

        int currentWordCount =
            currentText
                .Split(
                    ' ',
                    StringSplitOptions
                        .RemoveEmptyEntries)
                .Length;

        if (currentWordCount <= 4)
        {
            return true;
        }

        //--------------------------------------------------
        // VERY SMALL GAP
        //--------------------------------------------------

        double gap =
            next.Start - current.End;

        if (gap < 0.35)
        {
            return true;
        }

        //--------------------------------------------------
        // LOW CONFIDENCE
        //--------------------------------------------------

        if (current.Confidence < 0.65)
        {
            return true;
        }

        return false;
    }

    //--------------------------------------------------
    // ENDS SENTENCE
    //--------------------------------------------------

    private bool EndsSentence(
        string text)
    {
        return
            text.EndsWith(".")
            || text.EndsWith("?")
            || text.EndsWith("!")
            || text.EndsWith(":");
    }

    //--------------------------------------------------
    // MERGE
    //--------------------------------------------------

    private void MergePhrase(
        PhraseModel current,
        PhraseModel next)
    {
        current.Text =
            current.Text.Trim()
            + " "
            + next.Text.Trim();

        //--------------------------------------------------
        // TIMING
        //--------------------------------------------------

        current.End =
            next.End;

        //--------------------------------------------------
        // CONFIDENCE
        //--------------------------------------------------

        current.Confidence =
            (
                current.Confidence
                + next.Confidence
            ) / 2.0;

        //--------------------------------------------------
        // WORDS
        //--------------------------------------------------

        current.Words.AddRange(
            next.Words);
    }

    //--------------------------------------------------
    // CLONE
    //--------------------------------------------------

    private PhraseModel ClonePhrase(
        PhraseModel phrase)
    {
        return new PhraseModel
        {
            PhraseId =
                phrase.PhraseId,

            Start =
                phrase.Start,

            End =
                phrase.End,

            Text =
                phrase.Text,

            Confidence =
                phrase.Confidence,

            Words =
                phrase.Words
                    .ToList()
        };
    }
}