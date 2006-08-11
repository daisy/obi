using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Commands.Strips
{
    class RenamePhrase: Command
    {
        private Project mProject;
        private CoreNode mNode;
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
        public RenamePhrase(Project project, CoreNode node)
        {
            mProject = project;
            mNode = node;
            mOldName = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannel)).getText();
            mNewName = Project.GetAudioMediaAsset(mNode).Name;
        }

        public override void Do()
        {
            mProject.RenamePhraseNode(mNode, mNewName);
        }

        public override void Undo()
        {
            mProject.RenamePhraseNode(mNode, mOldName);
        }
    }
}
