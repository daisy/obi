using urakawa.media.data.audio;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;       // node to split
        private PhraseNode mNodeAfter;  // node after split point
        private Time mSplitTime;        // split point (begin/cursor)


        // Create a split command to split the selected node at the given position.
        // Use only through GetSplitCommand.
        private SplitAudio(ProjectView.ProjectView view, PhraseNode node, double time): base(view)
        {
            mNode = node;
            mNodeAfter = view.Presentation.CreatePhraseNode();
            mSplitTime = new Time(time);
            Label = Localizer.Message("split_phrase");
        }

        public PhraseNode Node { get { return mNode; } }
        public PhraseNode NodeAfter { get { return mNodeAfter; } }

        /// <summary>
        /// Create a command to crop the current selection.
        /// </summary>
        public static urakawa.undo.ICommand GetCropCommand(Obi.ProjectView.ProjectView view)
        {
            PhraseNode phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (end >= phrase.Duration) end = 0.0;
                if (begin > 0.0 || end > 0.0)
                {
                    urakawa.undo.CompositeCommand command =
                        view.Presentation.CreateCompositeCommand(Localizer.Message("crop_audio"));
                    if (end > 0.0)
                    {
                        AppendSplitCommandWithProperties(view, command, phrase, end, false);
                        command.append(new Commands.Node.DeleteWithOffset(view, phrase, 1));
                    }
                    if (begin > 0.0)
                    {
                        AppendSplitCommandWithProperties(view, command, phrase, begin,
                            view.Selection is AudioSelection && !((AudioSelection)view.Selection).AudioRange.HasCursor);
                        command.append(new Commands.Node.Delete(view, phrase));
                    }
                    if (command.getCount() > 0) return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Create a split command for the current selection.
        /// </summary>
        public static urakawa.undo.CompositeCommand GetSplitCommand(ProjectView.ProjectView view)
        {
            PhraseNode phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (end >= phrase.Duration) end = 0.0;
                if (begin > 0.0 || end > 0.0)
                {
                    urakawa.undo.CompositeCommand command =
                        view.Presentation.CreateCompositeCommand(Localizer.Message("split_phrase"));
                    if (end > 0.0) AppendSplitCommandWithProperties(view, command, phrase, end, false);
                    if (begin > 0.0) AppendSplitCommandWithProperties(view, command, phrase, begin,
                        view.Selection is AudioSelection && !((AudioSelection)view.Selection).AudioRange.HasCursor);
                    if (command.getCount() > 0) return command;
                }
            }
            return null;
        }

        // Create a split command preserving used/TODO status, and optionally transferring the role to the next node
        private static void AppendSplitCommandWithProperties(ProjectView.ProjectView view, urakawa.undo.CompositeCommand command,
            PhraseNode phrase, double time, bool transferRole)
        {
            SplitAudio split = new SplitAudio(view, phrase, time);
            if (split.Node.TODO) command.append(new Commands.Node.ToggleNodeTo_Do(view, split.NodeAfter));
            if (!split.Node.Used) command.append(new Commands.Node.ToggleNodeUsed(view, split.NodeAfter));
            command.append(split);
            if (transferRole && phrase.NodeKind != EmptyNode.Kind.Plain)
            {
                command.append(new Commands.Node.ChangeCustomType(view, phrase, EmptyNode.Kind.Plain));
                if (phrase.NodeKind == EmptyNode.Kind.Page)
                {
                    command.append(new Commands.Node.SetPageNumber(view, split.NodeAfter, phrase.PageNumber.Clone()));
                }
                else
                {
                    command.append(new Commands.Node.ChangeCustomType(view, split.NodeAfter, phrase.NodeKind, phrase.CustomClass));
                }
            }
        }

        // Perform a split given a time and a node with no audio, optionally updating the selection in the view afterward.
        public static void Split(ProjectView.ProjectView view, PhraseNode node, PhraseNode nodeAfter, Time splitTime,
            bool updateSelection)
        {
            nodeAfter.Audio = node.SplitAudio(splitTime);
            node.InsertAfterSelf(nodeAfter);
            if (updateSelection) view.SelectedBlockNode = nodeAfter;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
        }

        public override void execute()
        {
            Split(View, mNode, mNodeAfter, mSplitTime, UpdateSelection);
        }

        public override void unExecute()
        {
            MergeAudio.Merge(View, mNode, mNodeAfter, UpdateSelection);
            base.unExecute();
        }
    }
}
