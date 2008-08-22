using urakawa.media.data.audio;
using urakawa.media.timing;

namespace Obi.Commands.Node
{
    class SplitAudio: Command
    {
        private PhraseNode mNode;       // node to split
        private PhraseNode mNodeAfter;  // node after split point
        private Time mSplitTime;        // split point (begin/cursor)


        /// <summary>
        /// Create a split command to split the selected node at the given position.
        /// </summary>
        public SplitAudio(ProjectView.ProjectView view, PhraseNode node, double time): base(view)
        {
            mNode = node;
            mNodeAfter = view.Presentation.CreatePhraseNode();
            mSplitTime = new Time(time);
            Label = Localizer.Message("split_phrase");
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
                    if (end > 0.0) command.append(new SplitAudio(view, phrase, end));
                    if (begin > 0.0) command.append(new SplitAudio(view, phrase, begin));
                    if (command.getCount() > 0) return command;
                }
            }
            return null;
        }

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
