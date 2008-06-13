using System;
using System.Collections.Generic;
using System.Text;

namespace Bobi.Commands
{
    public class ImportAudio: Command
    {
        private urakawa.core.TreeNode trackNode;    // node of the track where the file is imported
        private urakawa.core.TreeNode audioNode;    // new audio node
        private string filename;                    // original filename


        public ImportAudio(View.ProjectView view, urakawa.core.TreeNode node, string filename): base(view)
        {
            this.trackNode = node;
            this.filename = filename;
            this.audioNode = view.Project.CreateAudioNode(filename);
            this.selectionAfter = new NodeSelection(view, this.audioNode);
        }

        public override string getLongDescription() { return string.Format("Imported audio from file \"{0}\".", this.filename); }
        public override string getShortDescription() { return string.Format("import audio"); }

        public override void execute()
        {
            this.trackNode.appendChild(this.audioNode);
            base.execute();
        }

        public override void unExecute()
        {
            this.audioNode.detach();
            base.unExecute();
        }
    }
}
