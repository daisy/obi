using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;
using urakawa.media.data.audio;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Command adding an existing node.
    /// </summary>
    public class UpdateAudioMedia: Command
    {
        private PhraseNode m_Node;
        private ManagedAudioMedia m_OriginalManagedAudioMedia ;
        private ManagedAudioMedia m_ManagedAudioMedia ;
        private NodeSelection mSelection;

        /// <summary>
        /// Add an existing node to a parent node at the given index.
        /// </summary>
        public UpdateAudioMedia(ProjectView.ProjectView view, PhraseNode node, ManagedAudioMedia media, bool updateSelection): base(view, "")
        {
            m_Node = node;
            m_OriginalManagedAudioMedia = node.Audio ;
            m_ManagedAudioMedia = media ;
            UpdateSelection = updateSelection;
            mSelection = view.Selection != null && view.Selection.Control is ProjectView.ContentView ?
                new NodeSelection(m_Node, view.Selection.Control) : view.Selection;
        }
        


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {

                if (m_Node != null && m_Node is PhraseNode && ((PhraseNode)m_Node).Audio != null)
                {
                    List<MediaData> mediaList = new List<MediaData>();
                    mediaList.Add(((PhraseNode)m_Node).Audio.MediaData);
                    return mediaList;
                }
                else
                {
                    return new List<MediaData>();
                }
            }
        }
        
        public override void Execute()
        {
            m_Node.Audio = m_ManagedAudioMedia;
            if (UpdateSelection) View.Selection = mSelection;
            TriggerProgressChanged ();
        }

        public override bool CanExecute { get { return true; } }

        public override void UnExecute()
        {
            m_Node.Audio = m_OriginalManagedAudioMedia;
            base.UnExecute();
        }
    
   }
}