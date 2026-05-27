using System;
using System.Collections.Generic;
using HtmlAgilityPack;
using SemanticStructureDetector.WinForms.Models;

namespace SemanticStructureDetector.WinForms.Parsers;

public class XhtmlPhraseParser
{
    public List<PhraseModel> Parse(
        string xhtmlPath)
    {
        var document =
            new HtmlAgilityPack.HtmlDocument();

        document.OptionFixNestedTags =
            true;

        document.Load(xhtmlPath);

        var phrases =
            new List<PhraseModel>();

        //--------------------------------------------------
        // PHRASE NODES
        //--------------------------------------------------

        var nodes =
            document.DocumentNode
                .SelectNodes(
                    "//p[@class='phrase']");

        if (nodes == null)
        {
            return phrases;
        }

        //--------------------------------------------------
        // PARSE PHRASES
        //--------------------------------------------------

        foreach (var node in nodes)
        {
            var phrase =
                new PhraseModel
                {
                    //--------------------------------------------------
                    // ID
                    //--------------------------------------------------

                    PhraseId =
                        node.GetAttributeValue(
                            "id",
                            ""),

                    //--------------------------------------------------
                    // TIMING
                    //--------------------------------------------------

                    Start =
                        node.GetAttributeValue(
                            "data-start",
                            0.0),

                    End =
                        node.GetAttributeValue(
                            "data-end",
                            0.0),

                    //--------------------------------------------------
                    // CONFIDENCE
                    //--------------------------------------------------

                    Confidence =
                        node.GetAttributeValue(
                            "data-confidence",
                            0.0),

                    //--------------------------------------------------
                    // TEXT
                    //--------------------------------------------------

                    Text =
                        HtmlEntity.DeEntitize(
                            node.InnerText.Trim())
                };

            //--------------------------------------------------
            // WORDS
            //--------------------------------------------------

            var wordNodes =
                node.SelectNodes(
                    ".//span[@class='word']");

            if (wordNodes != null)
            {
                foreach (var wordNode
                    in wordNodes)
                {
                    phrase.Words.Add(
                        new WordModel
                        {
                            //--------------------------------------------------
                            // WORD TEXT
                            //--------------------------------------------------

                            Text =
                                HtmlEntity.DeEntitize(
                                    wordNode.InnerText
                                        .Trim()),

                            //--------------------------------------------------
                            // TIMING
                            //--------------------------------------------------

                            Start =
                                wordNode
                                    .GetAttributeValue(
                                        "data-start",
                                        0.0),

                            End =
                                wordNode
                                    .GetAttributeValue(
                                        "data-end",
                                        0.0),

                            //--------------------------------------------------
                            // CONFIDENCE
                            //--------------------------------------------------

                            Confidence =
                                wordNode
                                    .GetAttributeValue(
                                        "data-confidence",
                                        0.0)
                        });
                }
            }

            //--------------------------------------------------
            // ADD
            //--------------------------------------------------

            phrases.Add(phrase);
        }

        return phrases;
    }
}