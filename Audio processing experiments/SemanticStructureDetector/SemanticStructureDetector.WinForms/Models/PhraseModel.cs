using System.Collections.Generic;

namespace SemanticStructureDetector.WinForms.Models;

public class PhraseModel
{
    public string PhraseId { get; set; } = "";

    public double Start { get; set; }

    public double End { get; set; }

    public string Text { get; set; } = "";

    //--------------------------------------------------
    // PHRASE CONFIDENCE
    //--------------------------------------------------

    public double Confidence { get; set; }

    //--------------------------------------------------
    // WORDS
    //--------------------------------------------------

    public List<WordModel> Words
    { get; set; }
        = new();
}