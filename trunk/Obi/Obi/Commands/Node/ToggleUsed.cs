using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class ToggleUsed : Command
    {
        private ObiNode mNode;
        private bool mNowUsed;
        private bool mDeep;

        public override string Label
        {
            get
            {
                return String.Format(Localizer.Message("toggle_used_command_label"),
                    Localizer.Message(mNode is SectionNode ? mDeep ? "section" : "strip" : "audio_block"),
                    Localizer.Message(mNowUsed ? "used" : "unused"));
            }
        }

        public ToggleUsed(ObiNode node, bool deep)
        {
            mNode = node;
            mNowUsed = node.Used;
            mDeep = deep;
        }

        public override void Do()
        {
            mNode.Project.ToggleNodeUsed(mNode, mDeep);
        }

        public override void Undo()
        {
            mNode.Project.ToggleNodeUsed(mNode, mDeep);
        }
    }
}
