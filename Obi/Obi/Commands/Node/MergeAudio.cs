using urakawa.media.timing;

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
        private Time mSplitTime;           // the split time of the new merged node


        private MergeAudio(ProjectView.ProjectView view, PhraseNode node, PhraseNode next)
            : base(view)
        {
            mNode = node;
            mNextNode = next;
            mSplitTime = new urakawa.media.timing.Time(mNode.Audio.getDuration().getTimeDeltaAsMillisecondFloat());
        }


        public static urakawa.undo.CompositeCommand GetMergeCommand(ProjectView.ProjectView view)
        {
            return GetMergeCommand(view, view.SelectedNodeAs<EmptyNode>());
        }

        public static urakawa.undo.CompositeCommand GetMergeCommand(ProjectView.ProjectView view, EmptyNode node)
        {
            if (node != null)
            {
                urakawa.undo.CompositeCommand command =
                    view.Presentation.CreateCompositeCommand(Localizer.Message("merge_phrase_with_next"));
                EmptyNode next = node.ParentAs<ObiNode>().PhraseChild(node.Index + 1) as EmptyNode;
                if (node.NodeKind == EmptyNode.Kind.Plain && next.NodeKind != EmptyNode.Kind.Plain)
                {
                    if (next.NodeKind == EmptyNode.Kind.Page)
                    {
                        command.append(new Commands.Node.SetPageNumber(view, node, next.PageNumber.Clone()));
                    }
                    else
                    {
                        command.append(new Commands.Node.ChangeCustomType(view, node, next.NodeKind, next.CustomClass));
                    }
                }
                if (!node.TODO && next.TODO) command.append(new Commands.Node.ToggleNodeTo_Do(view, node));
                if (!node.Used && next.Used) command.append(new Commands.Node.ToggleNodeUsed(view, node));
                if (node is PhraseNode)
                {
                    if (next is PhraseNode)
                    {
                        command.append(new Commands.Node.MergeAudio(view, (PhraseNode)node, (PhraseNode)next));
                    }
                    else
                    {
                        command.append(new Commands.Node.Delete(view, next));
                    }
                }
                else
                {
                    command.append(new Commands.Node.Delete(view, node));
                }
                return command;
            }
            return null;
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
            next.Detach();
            node.MergeAudioWith(next.Audio);
            if (updateSelection) view.SelectedBlockNode = node;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override void execute()
        {
            Merge(View, mNode, mNextNode, UpdateSelection);
        }

        public override void unExecute()
        {
            SplitAudio.Split(View, mNode, mNextNode, mSplitTime, UpdateSelection);
            base.unExecute();
        }
    }
}
