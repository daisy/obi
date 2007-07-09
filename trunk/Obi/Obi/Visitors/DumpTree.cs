using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core.visitor;
using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    public class DumpTree: ITreeNodeVisitor
    {
        private string indent = "+ ";

        public void postVisit(TreeNode node)
        {
            indent = indent.Substring(2);
        }

        public bool preVisit(TreeNode node)
        {
            TreeNode n = (TreeNode)node;
           
            string info = String.Format("{0}{1}{2}", indent, n.GetType(), ((TreeNode)n).GetHashCode().ToString());
           
            if (node.GetType() == Type.GetType("Obi.SectionNode"))
            {
                info += " " + Project.GetTextMedia(n).getText();
            }
            else if (node.GetType() == Type.GetType("Obi.PhraseNode"))
            {
                info += " " + ((TextMedia)Project.GetMediaForChannel(n, Project.ANNOTATION_CHANNEL_NAME)).getText();
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
