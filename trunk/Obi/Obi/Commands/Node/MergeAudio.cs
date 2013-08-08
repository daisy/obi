using System;
using System.Collections.Generic;
using urakawa.media.timing;
using urakawa.command;
using urakawa.media.data;

namespace Obi.Commands.Node
{
    /// <summary>
    /// Basic merge commands to merge the two nodes given as argument.
    /// Does not preserve node attributes (used, TODO, etc.) so use as
    /// part of a composite command.
    /// </summary>
    class MergeAudio: Command
    {
        private PhraseNode mNode;          // the selected phrase
        private PhraseNode mNextNode;      // the following phrase to merge with
        private Time mSplitTime ;           // the split time of the new merged node
        private bool m_IsNextNodeRooted;

        public MergeAudio(ProjectView.ProjectView view, PhraseNode node, PhraseNode next)
            : base(view)
        {
            mNode = node;
            mNextNode = next;

            // allow setting split time while executing command if the current node does not have audio yet. it is useful for composite commands
            mSplitTime = mNode.Audio != null? mNode.Audio.Duration : null; // new Time instance (no shared)
            
            m_IsNextNodeRooted = mNextNode.IsRooted;
        }


        public static CompositeCommand GetMergeCommand(ProjectView.ProjectView view)
        {
            EmptyNode node = view.SelectedNodeAs<EmptyNode>();
            return GetMergeCommand(view, node, node.NextSibling as EmptyNode);
        }

        public static CompositeCommand GetMergeCommand(ProjectView.ProjectView view, EmptyNode node, EmptyNode next)
        {
            if (node != null && next != null)
            {
                CompositeCommand command =
                    view.Presentation.CreateCompositeCommand(Localizer.Message("merge_phrase_with_next"));
                if (node is PhraseNode)
                {
                    AppendCopyNodeAttributes(command, view, next, node);
                    if (next is PhraseNode)
                    {
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.MergeAudio(view, (PhraseNode)node, (PhraseNode)next));
                    }
                    else
                    {
                        command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.Delete(view, next));
                    }
                }
                else
                {
                    AppendCopyNodeAttributes(command, view, node, next);
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(view, node));
                }
                return command;
            }
            return null;
        }
       
        /// <summary>
        /// Append commands to transfer the attributes of a node to another (used, TODO, role, page number)
        /// </summary>
        public static void AppendCopyNodeAttributes(CompositeCommand command, ProjectView.ProjectView view,
            EmptyNode from, EmptyNode to)
        {
                                if (from.TODO && !to.TODO) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeTODO(view, to));
                                if ((!from.Used && to.Used) && (to.Role_ != EmptyNode.Role.Page && to.Role_ != EmptyNode.Role.Heading )) 
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed(view, to));

            // role of next phrase is copied to selected phrase only when next phrase is page, heading or silence.
            // The priority is highest for page, followed by heading followed by silence. If next phrase is of higher priority only then its role is copied.
            if (from.Role_ == EmptyNode.Role.Page && to.Role_ != EmptyNode.Role.Page)
            {
                command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.SetPageNumber(view, to, from.PageNumber.Clone()));
                            }
            else if ( ( to.Role_ != EmptyNode.Role.Heading && to.Role_ != EmptyNode.Role.Page )
                &&  (from.Role_ != EmptyNode.Role.Plain &&
                (from.Role_ != EmptyNode.Role.Silence || to is PhraseNode)) )
            {
                        if (from.Role_ == EmptyNode.Role.Heading && to.Role_ != EmptyNode.Role.Page)
                {
                                command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.AssignRole ( view, from, EmptyNode.Role.Plain, null) );
                command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.AssignRole ( view, to, EmptyNode.Role.Heading, null) );
                }
            else
                command.ChildCommands.Insert (command.ChildCommands.Count, new Commands.Node.AssignRole(view, to, from.Role_, from.CustomRole));
            }
        }

        /// <summary>
        /// Merge the selected phrase with the following phrase.
        /// </summary>
        //public MergeAudio(ProjectView.ProjectView view):
        //    this(view, (PhraseNode)view.Selection.Node.ParentAs<ObiNode>().PhraseChild(view.Selection.Node.Index + 1)) {}

        /// <summary>
        /// Merge two nodes; the "next" one is removed after merging.
        /// </summary>
        public static void Merge(ProjectView.ProjectView view, PhraseNode node, PhraseNode next, bool updateSelection)
        {
            if(next.IsRooted)  next.Detach();
            node.MergeAudioWith(next.Audio);
            if (updateSelection) view.SelectedBlockNode = node;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override IEnumerable<MediaData> UsedMediaData
            {
                get
                {
                    List<MediaData> mediaList = new List<MediaData>();

                    if (mNode != null && mNode is PhraseNode && mNode.Audio != null)
                        mediaList.Add(mNode.Audio.MediaData);

                    return mediaList;
                }
            }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            if (mSplitTime == null) mSplitTime=  mNode.Audio.Duration; // assign split time for unexecute if it was not assigned in merge constructor
            Merge(View, mNode, mNextNode, UpdateSelection);
            TriggerProgressChanged ();
        }

        public override void UnExecute()
        {
            SplitAudio.Split(View, mNode, mNextNode, mSplitTime, UpdateSelection, false);
            if (!m_IsNextNodeRooted) mNextNode.Detach();
            base.UnExecute();
        }
    }
}
