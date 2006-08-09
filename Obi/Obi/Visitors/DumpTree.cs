using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    public class DumpTree: ICoreNodeVisitor
    {
        private string indent = "+ ";

        public void postVisit(ICoreNode node)
        {
            indent = indent.Substring(2);
        }

        public bool preVisit(ICoreNode node)
        {
            CoreNode n = (CoreNode)node;
            NodeInformationProperty p = (NodeInformationProperty)n.getProperty(typeof(NodeInformationProperty));
            string info = String.Format("{0}{1}[{2}]", indent, p.NodeType, p.Id);
            switch (p.NodeType)
            {
                case NodeType.Section:
                    info += " " + Project.GetTextMedia(n).getText();
                    break;
                case NodeType.Phrase:
                    info += " " + ((TextMedia)Project.GetMediaForChannel(n, Project.AnnotationChannel)).getText();
                    break;
                default:
                    break;
            }
            System.Diagnostics.Debug.Print(info);
            indent = "  " + indent;
            return true;
        }
    }
}
