using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    class NewTrack : Command
    {
        private urakawa.core.TreeNode node;         // node for the new track

        public NewTrack(View.ProjectView view): base(view)
        {
            this.node = getPresentation().getTreeNodeFactory().createNode(typeof(TrackNode).Name, DataModelFactory.NS);
            this.selectionAfter = new NodeSelection(view, this.node);
        }

        
        public override string getLongDescription()
        {
            return string.Format("Added new track (number of tracks in project: {0}.)",
                this.view.Project.NumberOfTracks);
        }
        
        public override string getShortDescription() { return "add new track"; }

        public override void execute()
        {
            getPresentation().getRootNode().appendChild(this.node);
            base.execute();
        }

        public override void unExecute()
        {
            this.node.detach();
            base.unExecute();
        }
    }
}
