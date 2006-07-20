using System;
using System.Collections.Generic;
using System.Text;

using Commands;
using urakawa.core;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// The command for adding a child node.
    /// </summary>
    class AddChild: Command
    {
        private CoreNode mNode;    // the child node to add
        private CoreNode mParent;  // the parent node

        public override string Label
        {
            get
            {
                return Localizer.Message("add_child_command_label");
            }
        }

        public AddChild(CoreNode node, CoreNode parent)
        {
            mNode = node;
            mParent = parent;
        }

        public override void Do()
        {
            // mParent.appendChild(mNode);
        }

        public override void Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
