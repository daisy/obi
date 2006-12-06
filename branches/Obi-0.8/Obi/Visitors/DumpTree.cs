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
           
            string info = String.Format("{0}{1}{2}", indent, n.GetType(), ((CoreNode)n).GetHashCode().ToString());
           
            if (node.GetType() == Type.GetType("Obi.SectionNode"))
            {
                info += " " + Project.GetTextMedia(n).getText();
            }
            else if (node.GetType() == Type.GetType("Obi.PhraseNode"))
            {
                info += " " + ((TextMedia)Project.GetMediaForChannel(n, Project.AnnotationChannelName)).getText();
                Assets.AudioMediaAsset asset = ((PhraseNode)n).Asset;
                foreach (Assets.AudioClip clip in asset.Clips)
                {
                    info += String.Format("\n  {0}{1} {2}-{3}", indent, clip.Path, clip.BeginTime, clip.EndTime);
                }
            }
            System.Diagnostics.Debug.Print(info);
            indent = "  " + indent;
            return true;
        }
    }
}
