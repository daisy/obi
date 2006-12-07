using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Commands.Strips
{
    class RenamePhrase: Command
    {
        private PhraseNode mNode;
        private string mOldName;
        private string mNewName;

        public override string Label
        {
            get { return Localizer.Message("rename_phrase_command_label"); }
        }

        /// <summary>
        /// The command is created before the media was updated, but after the asset was renamed.
        /// This does not look good...
        /// </summary>
        public RenamePhrase(PhraseNode node)
        {
            mNode = node;
            mOldName = node.Annotation;
            mNewName = node.Asset.Name;
        }

        public override void Do()
        {
            mNode.Project.EditAnnotationPhraseNode(mNode, mNewName);
        }

        public override void Undo()
        {
            mNode.Project.EditAnnotationPhraseNode(mNode, mOldName);
        }
    }
}
