using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class ToggleUsed : Command
    {
        private ObiNode mNode;
        private bool mNowUsed;

        public override string Label
        {
            get
            {
                return String.Format(Localizer.Message("toggle_used_command_label"),
                    Localizer.Message(mNode is SectionNode ? "section" : "phrase"),
                    Localizer.Message(mNowUsed ? "used" : "unused"));
            }
        }

        public ToggleUsed(ObiNode node)
        {
            mNode = node;
            mNowUsed = node.Used;
        }

        public override void Do()
        {
            mNode.Project.ToggleNodeUsed(mNode, this);
        }

        public override void Undo()
        {
            mNode.Project.ToggleNodeUsed(mNode, this);
        }
    }
}
