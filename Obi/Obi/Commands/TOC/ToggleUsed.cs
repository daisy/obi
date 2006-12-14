using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.TOC
{
    public class ToggleUsed : Command
    {
        private SectionNode mNode;
        private bool mNowUsed;

        public override string Label
        {
            get
            {
                return String.Format(Localizer.Message("toggle_used_command_label"), Localizer.Message("section"),
                    Localizer.Message(mNowUsed ? "used" : "unused"));
            }
        }

        public ToggleUsed(SectionNode node)
        {
            mNode = node;
            mNowUsed = node.Used;
        }

        public override void Do()
        {
            mNode.Project.ToggleSectionUsedState(new Events.Node.SectionNodeEventArgs(this, mNode));
        }

        public override void Undo()
        {
            mNode.Project.ToggleSectionUsedState(new Events.Node.SectionNodeEventArgs(this, mNode));
        }
    }
}
