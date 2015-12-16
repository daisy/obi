using System;
using System.Collections.Generic;

using urakawa.command;
using urakawa.media.data.audio;
using urakawa.media.data;
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
            mSplitTime = new Time((long)(time * Time.TIME_UNIT));
            SetDescriptions(Localizer.Message("split_phrase"));
        }

        public PhraseNode Node { get { return mNode; } }
        public PhraseNode NodeAfter { get { return mNodeAfter; } }

        /// <summary>
        /// Create a command to crop the current selection.
        /// </summary>
        public static urakawa.command.Command GetCropCommand(Obi.ProjectView.ProjectView view)
        {
            PhraseNode phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (end >= phrase.Duration) end = 0.0;
                CompositeCommand command = view.Presentation.CreateCompositeCommand(Localizer.Message("crop_audio"));
                if (begin > 0.0 || end > 0.0)
                {
                    SplitAudio split;         // actual split command
                    PhraseNode after = null;  // node to select after splitting
                    if (end > 0.0)
                    {
                        split = AppendSplitCommandWithProperties(view, command, phrase, end, false);
                        after = split.Node;
                        command.ChildCommands.Insert(command.ChildCommands.Count,
                            new Commands.UpdateSelection ( view, new NodeSelection ( after, view.Selection.Control ) ) );//@singleSection:moved from last of function
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.DeleteWithOffset(view, phrase, 1));
                    }
                    if (begin > 0.0)
                    {
                        split = AppendSplitCommandWithProperties(view, command, phrase, begin,
                            view.Selection is AudioSelection && ((AudioSelection)view.Selection).AudioRange != null && !((AudioSelection)view.Selection).AudioRange.HasCursor);
                        after = split.NodeAfter;
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.UpdateSelection(view, new NodeSelection(after, view.Selection.Control)));//@singleSection:moved from last of function

                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(view, phrase, false));
                    }
                    //command.append(new Commands.UpdateSelection(view, new NodeSelection(after, view.Selection.Control)));
                    return command;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first node after the split created by a split command.
        /// </summary>
        public static PhraseNode GetSplitNode(urakawa.command.Command command)
        {
            CompositeCommand c = command as CompositeCommand;
            SplitAudio split = command as SplitAudio;
            if (c != null)
            {
                System.Collections.Generic.List<urakawa.command.Command> commands = c.ChildCommands.ContentsAs_ListCopy;
                int i = commands.Count - 1;
                for (; i >= 0 && !(commands[i] is SplitAudio); --i) { }
                if (i >= 0) split = commands[i] as SplitAudio;
            }
            return split == null ? null : split.NodeAfter;
        }

        /// <summary>
        /// Get the node after the split node created by a crop command.
        /// </summary>
        public static PhraseNode GetCropNode(urakawa.command.Command command, PhraseNode splitNode)
        {
            CompositeCommand c = command as CompositeCommand;
            SplitAudio split = null;
            if (c != null)
            {
                System.Collections.Generic.List<urakawa.command.Command> commands = c.ChildCommands.ContentsAs_ListCopy;
                int i = 0;
                for (; i < commands.Count && !((commands[i] is SplitAudio) && ((SplitAudio)commands[i]).NodeAfter != splitNode);
                    ++i) { }
                if (i < commands.Count) split = commands[i] as SplitAudio;
            }
            return split == null ? null : split.NodeAfter;
        }

        /// <summary>
        /// Create a split command for the current selection.
        /// </summary>
        public static CompositeCommand GetSplitCommand(ProjectView.ProjectView view)
        {
            return GetSplitCommand(view, true);
        }

            public static CompositeCommand GetSplitCommand(ProjectView.ProjectView view, bool transferProperties)
        {
            PhraseNode phrase = view.TransportBar.PlaybackPhrase;
            if (phrase == null) phrase = view.SelectedNodeAs<PhraseNode>();
            if (phrase != null)
            {
                double begin = view.TransportBar.SplitBeginTime;
                double end = view.TransportBar.SplitEndTime;
                if (begin >= phrase.Duration) begin = 0.0;
                if (end >= phrase.Duration) end = 0.0;
                if (begin > 0.0 || end > 0.0)
                {
                    CompositeCommand command =
                        view.Presentation.CreateCompositeCommand(Localizer.Message("split_phrase"));
                    if (end > 0.0) AppendSplitCommandWithProperties(view, command, phrase, end, false);
                    if (begin > 0.0)
                    {
                        if (transferProperties)
                        {
                            AppendSplitCommandWithProperties(view, command, phrase, begin,
    view.Selection is AudioSelection && ((AudioSelection)view.Selection).AudioRange != null && !((AudioSelection)view.Selection).AudioRange.HasCursor && !(phrase.Role_ == EmptyNode.Role.Silence || phrase.Role_ == EmptyNode.Role.Custom));
                        }
                        else
                        {
                            AppendSplitCommandWithProperties(view, command, phrase, begin, false);
                        }
                    }
                    if (command.ChildCommands.Count > 0) return command;
                }
            }
            return null;
        }

        public static CompositeCommand GetSplitCommand(ProjectView.ProjectView view, PhraseNode phrase, double time)
        {
            CompositeCommand command =
                        view.Presentation.CreateCompositeCommand(Localizer.Message("split_phrase"));
            AppendSplitCommandWithProperties(view, command, phrase, time, false);
            return command;
        }

        /// <summary>
        /// Create the phrase detection command.
        /// </summary>
        public static CompositeCommand GetPhraseDetectionCommand(ProjectView.ProjectView view, ObiNode node,
            long threshold, double gap, double before, bool mergeFirstTwoPhrases)
        {
        List<PhraseNode> phraseNodesList = new List<PhraseNode> ();
        if (node is PhraseNode)
            {
            phraseNodesList.Add ( (PhraseNode)node );
            }
        else if (node is SectionNode)
            {
            SectionNode section = (SectionNode)node;
            for (int i = 0; i < section.PhraseChildCount; i++)
                {
                if (section.PhraseChild ( i ) is PhraseNode && ((PhraseNode)section.PhraseChild(i)).Role_ != EmptyNode.Role.Silence) 
                    phraseNodesList.Add ((PhraseNode)  section.PhraseChild (i) );
                }
            }
            List<List<PhraseNode>> phrasesToMerge = new List<List<PhraseNode>>();
            CompositeCommand command = view.Presentation.CreateCompositeCommand(Localizer.Message("phrase_detection"));
            // if phrase is selected but phrase detection node is section,select section
            if ( node is SectionNode && view.GetSelectedPhraseSection != null 
                && view.GetSelectedPhraseSection == node )
                {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new UpdateSelection(view, new NodeSelection(node, view.Selection.Control)));
                }

            ObiNode parent = node is SectionNode ? node : node.ParentAs<ObiNode> ();
            int index = 0;
            if (phraseNodesList.Count > 0) view.TriggerProgressChangedEvent(Localizer.Message("phrase_detection"), 0);
            for (int j = 0; j < phraseNodesList.Count; j++)
                {
                PhraseNode phrase = phraseNodesList[j];
                
                if ( j == 0 )  index = phrase.Index + 1;

                System.Collections.Generic.List<PhraseNode> phrases = view.Presentation.CreatePhraseNodesFromAudioAssetList (
                    Obi.Audio.PhraseDetection.Apply ( phrase.Audio.Copy (), threshold, gap, before ) );
                for (int i = 0; i < phrases.Count; ++i)
                    {
                    // Copy page/heading role for the first phrase only
                    if (i == 0 || (phrase.Role_ != EmptyNode.Role.Page && phrase.Role_ != EmptyNode.Role.Heading))
                        {
                        phrases[i].CopyAttributes ( phrase );
                        }
                    phrases[i].Used = phrase.Used;
                    phrases[i].TODO = phrase.TODO;
                    if (phrases[i].Role_ == EmptyNode.Role.Heading && i > 0) phrases[i].Role_ = EmptyNode.Role.Plain;

                    // in following add node constructor, update selection is made false, to improve performance (19 may, 2010)
                    command.ChildCommands.Insert(command.ChildCommands.Count , new Commands.Node.AddNode(view, phrases[i], parent, index, false));
                    index++;
                    }
                    // add first 2 phrases to the list if the merge flag is true
                    if (phrases.Count >= 2 && mergeFirstTwoPhrases
                        && phrases[0] is PhraseNode && phrases [1] is PhraseNode)
                    {
                        List<PhraseNode> mergeList = new List<PhraseNode>();
                        mergeList.Add(phrases[0]);
                        mergeList.Add(phrases[1]);
                        phrasesToMerge.Add(mergeList);
                        
                    }

                if (node is PhraseNode &&  phrases.Count > 0 && view.Selection != null)
                    {
                    //command.append ( new UpdateSelection ( view, new NodeSelection ( node, view.Selection.Control ) ) );
                        command.ChildCommands.Insert(command.ChildCommands.Count, new UpdateSelection(view, new NodeSelection(phrases[0], view.Selection.Control)));//uncommenting this because unexecute for update selection can handle null unexecute now
                                        }
                Commands.Node.Delete deleteCmd = new Commands.Node.Delete(view, phrase, false) ;
                command.ChildCommands.Insert(command.ChildCommands.Count, deleteCmd);//@singleSection: moved delete command last for improve undo selection

                if (Obi.Audio.PhraseDetection.CancelOperation) break;
                // skip to next indexes if the two consequtive phrases in phrase list are not consequitive according to phrase index in the parent section
                if (j < phraseNodesList.Count - 1
                    && phrase.Index + 1 < phraseNodesList[j + 1].Index)
                {
                    EmptyNode empty = null;
                    for (int i = phrase.Index + 1; i < phraseNodesList[j + 1].Index; ++i)
                    {
                        empty = phrase.ParentAs<SectionNode>().PhraseChild(i);
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(view, empty, false));
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(view, empty, parent, index, false));
                        index++;
                    }
                    //index = index + (phraseNodesList[j + 1].Index - (phrase.Index + 1));
                }     
                view.TriggerProgressChangedEvent(Localizer.Message("phrase_detection"), (100 * j) / phraseNodesList.Count);
                
                }
                if (phrasesToMerge.Count > 0)
                {
                    for (int i = 0; i < phrasesToMerge.Count; i++)
                    {
                        List<PhraseNode> mergeList = phrasesToMerge[i];
                        Commands.Node.MergeAudio mergeCmd = new MergeAudio(view, mergeList[0], mergeList[1]) ;
                        mergeCmd.UpdateSelection = false ;
                        command.ChildCommands.Insert(command.ChildCommands.Count, mergeCmd);
                    }
                }
            return command;
        }


        /// <summary>
        /// Create the phrase detection command for list of phrases.
        /// </summary>
        public static CompositeCommand GetPhraseDetectionCommand(ProjectView.ProjectView view, List<PhraseNode> phraseNodesList,
            long threshold, double gap, double before, bool mergeFirstTwoPhrases)
        {
            CompositeCommand command = view.Presentation.CreateCompositeCommand(Localizer.Message("phrase_detection"));
            List<List<PhraseNode>> phrasesToMerge = new List<List<PhraseNode>>();
            int index = 0;
            if (phraseNodesList.Count > 0) view.TriggerProgressChangedEvent(Localizer.Message("phrase_detection"), 0);
            
            for (int j = 0; j < phraseNodesList.Count; j++)
            {
                PhraseNode phrase = phraseNodesList[j];

                if (j == 0
                    || phraseNodesList[j].ParentAs<SectionNode>() != phraseNodesList[j - 1].ParentAs<SectionNode>())
                {
                    index = phrase.Index + 1;
                    Console.WriteLine("PH Section name " + phrase.ParentAs<SectionNode>().Label);
                }
                else
                {
                    int diff = phraseNodesList[j].Index - phraseNodesList[j - 1].Index - 1; // used -1 because the original phrase is deleted
                    index = index + diff;
                }
                System.Collections.Generic.List<PhraseNode> phrases = view.Presentation.CreatePhraseNodesFromAudioAssetList(
                    Obi.Audio.PhraseDetection.Apply(phrase.Audio.Copy(), threshold, gap, before));
                for (int i = 0; i < phrases.Count; ++i)
                {
                    // Copy page/heading role for the first phrase only
                    if (i == 0 || (phrase.Role_ != EmptyNode.Role.Page && phrase.Role_ != EmptyNode.Role.Heading))
                    {
                        phrases[i].CopyAttributes(phrase);
                    }
                    phrases[i].Used = phrase.Used;
                    phrases[i].TODO = phrase.TODO;
                    if (phrases[i].Role_ == EmptyNode.Role.Heading && i > 0) phrases[i].Role_ = EmptyNode.Role.Plain;
                    Console.WriteLine("PH index: " + index);
                    // in following add node constructor, update selection is made false, to improve performance (19 may, 2010)
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(view, phrases[i], phrase.ParentAs<SectionNode>(), index, false));
                    // add first 2 phrases to the list if the merge flag is true
                    if (phrases.Count >= 2 && mergeFirstTwoPhrases
                        && phrases[0] is PhraseNode && phrases[1] is PhraseNode)
                    {
                        List<PhraseNode> mergeList = new List<PhraseNode>();
                        mergeList.Add(phrases[0]);
                        mergeList.Add(phrases[1]);
                        phrasesToMerge.Add(mergeList);

                    }

                    index++;
                }
                
                Commands.Node.Delete deleteCmd = new Commands.Node.Delete(view, phrase, false);
                command.ChildCommands.Insert(command.ChildCommands.Count, deleteCmd);//@singleSection: moved delete command last for improve undo selection

                if (Obi.Audio.PhraseDetection.CancelOperation) break;
                /*
                // skip to next indexes if the two consequtive phrases in phrase list are not consequitive according to phrase index in the parent section
                if (j < phraseNodesList.Count - 1
                    && phrase.Index + 1 < phraseNodesList[j + 1].Index)
                {
                    EmptyNode empty = null;
                    for (int i = phrase.Index + 1; i < phraseNodesList[j + 1].Index; ++i)
                    {
                        empty = phrase.ParentAs<SectionNode>().PhraseChild(i);
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.Delete(view, empty, false));
                        command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AddNode(view, empty, parent, index, false));
                        index++;
                    }
                    //index = index + (phraseNodesList[j + 1].Index - (phrase.Index + 1));
                }
                 */ 
                view.TriggerProgressChangedEvent(Localizer.Message("phrase_detection"), (100 * j) / phraseNodesList.Count);

            }
            if (phrasesToMerge.Count > 0)
            {
                for (int i = 0; i < phrasesToMerge.Count; i++)
                {
                    List<PhraseNode> mergeList = phrasesToMerge[i];
                    Commands.Node.MergeAudio mergeCmd = new MergeAudio(view, mergeList[0], mergeList[1]);
                    mergeCmd.UpdateSelection = false;
                    command.ChildCommands.Insert(command.ChildCommands.Count, mergeCmd);
                }
            }
            return command;
        }


        // Create a split command preserving used/TODO status, and optionally transferring the role to the next node
        public static SplitAudio AppendSplitCommandWithProperties(ProjectView.ProjectView view, CompositeCommand command,
            PhraseNode phrase, double time, bool transferRole)
        {
            SplitAudio split = new SplitAudio(view, phrase, time);
            if (split.Node.TODO) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeTODO(view, split.NodeAfter));
            if (!split.Node.Used) command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.ToggleNodeUsed(view, split.NodeAfter));
            if (split.Node.Role_ == EmptyNode.Role.Silence || phrase.Role_ == EmptyNode.Role.Custom)
            {
                Commands.Node.AssignRole copyRoleCmd =
                    new Commands.Node.AssignRole(view, split.NodeAfter, phrase.Role_, phrase.CustomRole);
                copyRoleCmd.UpdateSelection = false;
                command.ChildCommands.Insert(command.ChildCommands.Count, copyRoleCmd);
            }
            command.ChildCommands.Insert(command.ChildCommands.Count, split);
            if (transferRole && phrase.Role_ != EmptyNode.Role.Plain)
            {
                command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssignRole(view, phrase, EmptyNode.Role.Plain));
                if (phrase.Role_ == EmptyNode.Role.Page)
                {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.SetPageNumber(view, split.NodeAfter, phrase.PageNumber.Clone()));
                }
                else
                {
                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Node.AssignRole(view, split.NodeAfter, phrase.Role_, phrase.CustomRole));
                }
            }
            return split;
        }

        // Perform a split given a time and a node with no audio, optionally updating the selection in the view afterward.
        public static void Split(ProjectView.ProjectView view, PhraseNode node, PhraseNode nodeAfter, Time splitTime,
            bool updateSelection, bool allowSpecialRoleMarkForSurrounding)
        {
            nodeAfter.Audio = node.SplitAudio(splitTime);
            node.InsertAfterSelf(nodeAfter);
            if (allowSpecialRoleMarkForSurrounding) AssignRole.AssignRoleToEmptyNodeSurroundedByCustomRoles(nodeAfter);
            if (updateSelection) view.SelectedBlockNode = nodeAfter;
            view.UpdateBlocksLabelInStrip(node.AncestorAs<SectionNode>());
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


        public override IEnumerable<MediaData> UsedMediaData
        {
            get
            {
                List<MediaData> mediaList = new List<MediaData>();

                if (mNodeAfter != null && mNodeAfter is PhraseNode && mNodeAfter.Audio != null)
                    mediaList.Add(mNodeAfter.Audio.MediaData);

                return mediaList;
            }
        }

        public override bool CanExecute { get { return true; } }

        public override void Execute()
        {
            Split(View, mNode, mNodeAfter, mSplitTime, UpdateSelection, AllowRoleChangeAccordingToSurroundingSpecialNodes);
            TriggerProgressChanged ();
        }

        public override void UnExecute()
        {
            MergeAudio.Merge(View, mNode, mNodeAfter, UpdateSelection);
            base.UnExecute();
        }
    }
}
