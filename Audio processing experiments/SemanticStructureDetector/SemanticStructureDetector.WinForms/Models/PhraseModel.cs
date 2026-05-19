using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticStructureDetector.WinForms.Models;

public class PhraseModel
{
    public string PhraseId { get; set; } = string.Empty;

    public double Start { get; set; }

    public double End { get; set; }

    public string Text { get; set; } = string.Empty;
}