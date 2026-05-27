namespace SemanticStructureDetector.WinForms.Models;

public class WordModel
{
    public string Text { get; set; } = "";

    public double Start { get; set; }

    public double End { get; set; }

    public double Confidence { get; set; }
}