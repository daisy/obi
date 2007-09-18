using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Node
{
    public class MarkSectionHeading : Command__OLD__
    {
        PhraseNode mNewHeading;  // new section heading
        PhraseNode mOldHeading;  // previous section heading

        public MarkSectionHeading(PhraseNode newHeading, PhraseNode oldHeading)
        {
            mNewHeading = newHeading;
            mOldHeading = oldHeading;
        }

        public override string Label
        {
            get
            {
                return Localizer.Message(mNewHeading == null ?
                    "unmark_section_heading_command_label" :
                    "mark_section_heading_command_label");
            }
        }

        public override void Do()
        {
            if (mNewHeading == null)
            {
                mOldHeading.Project.UnmakePhraseHeading(mOldHeading.ParentSection);
            }
            else
            {
                mNewHeading.Project.MakePhraseHeading(mNewHeading);
            }
        }

        public override void Undo()
        {
            if (mOldHeading == null)
            {
                mNewHeading.Project.UnmakePhraseHeading(mNewHeading.ParentSection);
            }
            else
            {
                mOldHeading.Project.MakePhraseHeading(mOldHeading);
            }
        }
    }
}
