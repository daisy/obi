using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticStructureDetector.WinForms.Models;

public class StructureItem
{
    public string PhraseId { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public int? Level { get; set; }

    public string? ParagraphId { get; set; }

    public double Confidence { get; set; }

    public string Reason { get; set; } = "";

    public string HeadingText { get; set; } = "";
}