using System;
using System.Collections.Generic;
using System.Text;

using urakawa.media.data;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Command adding an existing node.
    /// </summary>
    public class AddNode: Command
    {
        protected ObiNode mNode;
        private ObiNode mParent;
        private int mIndex;
        private NodeSelection mSelection;

        /// <summary>
        /// Add an existing node to a parent node at the given index.
        /// </summary>
        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index): base(view, "")
        {
            mNode = node;
            mParent = parent;
            mIndex = index;
            mSelection = view.Selection != null && view.Selection.Control is ProjectView.ContentView ?
                new NodeSelection(mNode, view.Selection.Control) : view.Selection;
        }

        public AddNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index, bool update)
            : this(view, node, parent, index)
        {
            UpdateSelection = update;
        }

        /// <summary>
        /// Add an existing node to its parent at its index.
        /// </summary>
        public AddNode(ProjectView.ProjectView view, ObiNode node)
            : this(view, node, node.ParentAs<ObiNode>(), node.Index) {}


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {

                if (mNode != null && mNode is PhraseNode && ((PhraseNode)mNode).Audio != null)
                {
                    List<MediaData> mediaList = new List<MediaData>();
                    mediaList.Add(((PhraseNode)mNode).Audio.MediaData);
                    return mediaList;
                }
                else
                {
                    return new List<MediaData>();
                }
            }
        }

        private bool m_AllowRoleChangeAccordingToSurroundingSpecialNodes = true;
        /// <summary>
        /// <Allows the added phrase to change its role according to special roles surrounding it. Its true by default
        /// </summary>
        public bool AllowRoleChangeAccordingToSurroundingSpecialNodes
        {
            get { return m_AllowRoleChangeAccordingToSurroundingSpecialNodes; }
            set { m_AllowRoleChangeAccordingToSurroundingSpecialNodes = value; }
        }

        
        public override void Execute()
        {
            mParent.Insert(mNode, mIndex);
            if ( AllowRoleChangeAccordingToSurroundingSpecialNodes) AssignRole.AssignRoleToEmptyNodeSurroundedByCustomRoles(mNode);
            if (UpdateSelection) View.Selection = mSelection;
            TriggerProgressChanged ();
        }

        public override bool CanExecute { get { return true; } }

        public override void UnExecute()
        {
            if (mNode.IsRooted)  mNode.Detach();
            if (mNode is EmptyNode) View.UpdateBlocksLabelInStrip((SectionNode)mParent);
            base.UnExecute();
        }
    }

    public class AddEmptyNode : AddNode
    {
        private IControlWithSelection mControl;

        public AddEmptyNode(ProjectView.ProjectView view, ObiNode node, ObiNode parent, int index)
            : base(view, node, parent, index)
        {
            mControl = view.Selection.Control;
            SetDescriptions(Localizer.Message("add_blank_phrase"));
        }

        public override void Execute()
        {
            base.Execute();
            View.Selection = new NodeSelection(mNode, mControl);
        }
    }
}