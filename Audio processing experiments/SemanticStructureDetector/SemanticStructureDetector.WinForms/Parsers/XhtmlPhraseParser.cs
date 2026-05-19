using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HtmlAgilityPack;
using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Parsers;

public class XhtmlPhraseParser
{
    public List<PhraseModel> Parse(string xhtmlPath)
    {
        var document = new HtmlAgilityPack.HtmlDocument();

        document.OptionFixNestedTags = true;

        document.Load(xhtmlPath);

        var phrases = new List<PhraseModel>();

        var nodes = document.DocumentNode
            .SelectNodes("//*[@data-phraseid]");

        if (nodes == null)
            return phrases;

        foreach (var node in nodes)
        {
            phrases.Add(new PhraseModel
            {
                PhraseId = node.GetAttributeValue("data-phraseid", ""),
                Start = node.GetAttributeValue("data-start", 0.0),
                End = node.GetAttributeValue("data-end", 0.0),
                Text = HtmlEntity.DeEntitize(node.InnerText.Trim())
            });
        }

        return phrases;
    }
}
