using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Commands.Strips
{
    /// <summary>
    /// An annotation was edited.
    /// </summary>
    /// <remarks>Needs to be renamed.</remarks>
    class RenamePhrase: Command
    {
        private PhraseNode mNode;       // the phrase node for which the annotation is changing
        private string mOldAnnotation;  // the old annotation ("" if none)
        private string mNewAnnotation;  // the new annotation ("" if node)

        public override string Label
        {
            get
            {
                return Localizer.Message(mNewAnnotation == "" ?
                    "remove_annotation_command_label" :
                    "edit_annotation_command_label");
            }
        }

        /// <summary>
        /// The command is created before the media was updated, but after the asset was renamed.
        /// This does not look good...
        /// </summary>
        public RenamePhrase(PhraseNode node, string annotation)
        {
            mNode = node;
            mOldAnnotation = node.Annotation;
            mNewAnnotation = annotation;
        }

        /// <summary>
        /// Do: set the new annotation (or remove it if null)
        /// </summary>
        public override void Do()
        {
            mNode.Annotation = mNewAnnotation;
        }

        /// <summary>
        /// Undo: set the old annotation (or remove if null)
        /// </summary>
        public override void Undo()
        {
            mNode.Annotation = mOldAnnotation;
        }
    }
}
