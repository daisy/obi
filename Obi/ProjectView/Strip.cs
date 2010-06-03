using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInContentViewWithColors
    {
        private float mAudioScale;           // local audio scale for the strip
        private int mBlockLayoutBaseHeight;  // base height of the block layout (for zooming)
        private int mBlockHeight;            // height of a single line of the block layout
        private ContentView mContentView;    // parent content view
        private bool mHighlighted;           // highlighted flag (when the section node is selected)
        private Mutex mLabelUpdateThread;    // thread to update labels
        private SectionNode mNode;           // the section node for this strip
        private bool mWrap;                  // wrap contents
        private bool m_IsBlocksVisibilityProcessActive; // @phraseLimit

        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mBlockLayoutBaseHeight = mBlockLayout.Height;
            mBlockHeight = 0;
            mLabel.Editable = false;
            mNode = null;
            Highlighted = false;
            mWrap = false;
            mLabelUpdateThread = new Mutex();
            m_IsBlocksVisibilityProcessActive = false; // @phraseLimit
        }

        /// <summary>
        /// Create a new strip with an associated section node.
        /// </summary>
        public Strip(SectionNode node, ContentView parent)
            : this()
        {
            if (node == null) throw new Exception("Cannot set a null section node for a strip!");
            mNode = node;
            Label = mNode.Label;
            mContentView = parent;
            mContentView.SizeChanged += new EventHandler(delegate(object sender, EventArgs e) { Resize_View(); });
            ZoomFactor = mContentView.ZoomFactor;
            AudioScale = mContentView.AudioScale;
            UpdateColors();
            SetAccessibleName();
        }


        /// <summary>
        /// Get or set the audio scale for *this* strip (may differ from the parent's.)
        /// </summary>
        public float AudioScale
        {
            get { return mAudioScale; }
            set
            {
                if (value > 0.0)
                {
                    mAudioScale = value;
                    foreach (Control c in mBlockLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).AudioScale = value;
                    Resize_Blocks();
                }
            }
        }

        /// <summary>
        /// Get or set the color settings from the content view.
        /// </summary>
        public ColorSettings ColorSettings
        {
            get { return mContentView == null ? null : mContentView.ColorSettings; }
            set { UpdateColors(value); }
        }


// @phraseLimit
        /// <summary>
        /// returns true if all the blocks in strip are visible
                /// </summary>
        public bool IsBlocksVisible
            {
            get
                {
                if (mNode.PhraseChildCount == 0)
                    return true;
                else if (mBlockLayout.Controls.Count < (mNode.PhraseChildCount * 2 ) + 1 )
                    return false;
                else
                    return true;
                }
            }
        // @phraseLimit
        public bool IsBlocksVisibilityProcessActive
            {
             get { return m_IsBlocksVisibilityProcessActive ; }
            set { m_IsBlocksVisibilityProcessActive = value ;}
            }


        /// <summary>
        /// Get the content view for the strip.
        /// </summary>
        public ContentView ContentView { get { return mContentView; } }

        /// <summary>
        /// Get the first block in the strip, or null if empty.
        /// </summary>
        public Block FirstBlock
        {
            get { return mBlockLayout.Controls.Count > 1 ? mBlockLayout.Controls[1] as Block : null; }
        }

        /// <summary>
        /// Get or set the highlighted flag for the strip corresponding to the section node being (de)selected.
        /// </summary>
        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value && !(mContentView.Selection is TextSelection);
                if (mHighlighted && mLabel.Editable) mLabel.Editable = false;
                UpdateColors();
                if (mHighlighted) UpdateWaveforms(AudioBlock.STRIP_SELECTED_PRIORITY);
            }
        }

        /// <summary>
        /// Get or set the label of the strip where the title of the section can be edited.
        /// </summary>
        public string Label
        {
            get { return mLabel.Label; }
            set
            {
                if (value != null && value != "")
                {
                    mLabel.Label = value;
                    SetAccessibleName();
                }
                Resize_Label();
            }
        }

        /// <summary>
        /// Get the last block in the strip, or null if empty.
        /// </summary>
        public Block LastBlock
        {
            get
            {
                return mBlockLayout.Controls.Count > 1 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 2] as Block :
                    null;
            }
        }

        /// <summary>
        /// Get the tab index of the last block in the strip.
        /// Strip cursors are skipped when tabbing.
        /// </summary>
        public int LastTabIndex
        {
            get
            {
                int last = mBlockLayout.Controls.Count - 2;
                return last >= 0 ? ((Block)mBlockLayout.Controls[last]).LastTabIndex : TabIndex;
            }
        }

        /// <summary>
        /// Get the section node for this strip.
        /// </summary>
        public SectionNode Node { get { return mNode; } }

        /// <summary>
        /// Get the (generic) node for this strip; used for selection.
        /// </summary>
        public ObiNode ObiNode { get { return mNode; } }

        /// <summary>
        /// Get the current selection for this node (i.e. the strip, its label or an index is selected.)
        /// </summary>
        public NodeSelection Selection
        {
            get
            {
                NodeSelection selection = mContentView == null ? null : mContentView.Selection;
                return selection == null || selection.Node != mNode ? null : selection;
            }
        }

        /// <summary>
        /// Set the wrap parameter.
        /// </summary>
        public bool WrapContents
        {
            set
            {
                mWrap = value;
                Resize_Wrap();
            }
        }

        //@singleSection
        public bool ShouldStopAddingBlocks
            {
            get
                {
                if (mBlockLayout != null && mBlockLayout.Controls.Count > 0
                                         && mBlockLayout.Controls[mBlockLayout.Controls.Count - 1].Location.Y > (mContentView.Location.Y + mContentView.Size.Height))
                    {
                    return true;
                    }
                else
                    {
                    return false;
                    }
                }
            }

        
        /// <summary>
        /// Set the zoom factor for the strip and its contents.
        /// </summary>
        public float ZoomFactor
        {
            set
            {
                if (value > 0.0f)
                {
                    mLabel.ZoomFactor = value;
                    mBlockHeight = (int)Math.Round(value * mBlockLayoutBaseHeight);
                    foreach (Control c in mBlockLayout.Controls)
                    {
                        if (c is Block)
                        {
                            ((Block)c).SetZoomFactorAndHeight(value, mBlockHeight);
                        }
                        else if (c is StripCursor)
                        {
                            ((StripCursor)c).SetHeight(mBlockHeight);
                        }
                    }
                    Resize_Zoom();
                }
            }
        }


        private delegate Block BlockInvokation(EmptyNode node);

        /// <summary>
        /// Add a new block for an empty node. Return the block once added.
        /// Cursors are added if necessary: one after the block, and one before
        /// if it is the first block of the strip.
        /// </summary>
        public Block AddBlockForNode(EmptyNode node)
        {
            if (InvokeRequired)
            {
                return (Block)Invoke(new BlockInvokation(AddBlockForNode), node);
            }
            else
            {
                if (mBlockLayout.Controls.Count == 0)
                {
                    StripCursor cursor = AddCursorAtBlockLayoutIndex(0);
                }
                Block block = node is PhraseNode ? new AudioBlock((PhraseNode)node, this) : new Block(node, this);
                mBlockLayout.Controls.Add(block);
                mBlockLayout.Controls.SetChildIndex(block, 1 + 2 * node.Index);
                AddCursorAtBlockLayoutIndex(2 + 2 * node.Index);
                block.SetZoomFactorAndHeight(mContentView.ZoomFactor, mBlockHeight);
                    block.Cursor = Cursor;
                block.SizeChanged += new EventHandler(Block_SizeChanged);

                Resize_Blocks(); 

                UpdateStripCursorsAccessibleName(2 + 2 * node.Index);

                return block;
            }
        }

        /// <summary>
        /// Return the block after the selection. In the case of a strip it is the first block.
        /// Return null if this goes past the last block, there are no blocks, or nothing was selected
        /// in the first place.
        /// </summary>
        public Block BlockAfter(ISelectableInContentView item)
        {
            int count = mBlockLayout.Controls.Count;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index : 1 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) + 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) + 1 : count;
            return index < count ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Return the block before the selection. In the case of a strip this is the last block.
        /// Return null if this is before the first block, there are no blocks, or nothing was selected
        /// in the first place.
        /// </summary>
        public Block BlockBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    mBlockLayout.Controls.Count - 2 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) - 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) - 1 : -1;
            return index >= 0 ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Find the block for the corresponding node inside the strip.
        /// </summary>
        public Block FindBlock(EmptyNode node)
        {
            // Note: we cannot rely on node.Index because we may want to find the block of a node that was deleted
            // from the tree, and it does not have an index anymore. So let's just look for it.
            foreach (Control c in mBlockLayout.Controls) if (c is Block && ((Block)c).Node == node) return (Block)c;
            return null;
        }

        /// <summary>
        /// Find the strip cursor at the given index.
        /// </summary>
        public StripCursor FindStripCursor(int index)
        {
            System.Diagnostics.Debug.Assert(index * 2 < mBlockLayout.Controls.Count, "No strip cursor at index");
            System.Diagnostics.Debug.Assert ( mBlockLayout.Controls.Count > index * 2    &&    mBlockLayout.Controls[index * 2] is StripCursor );
            return mBlockLayout.Controls.Count > index * 2? (StripCursor)mBlockLayout.Controls[index * 2] : null;
        } 

        /// <summary>
        /// Focus on the label.
        /// </summary>
        public void FocusStripLabel() { mLabel.Focus(); }

        /// <summary>
        /// Remove the block for the given node.
        /// Remove the following strip cursor as well
        /// (and the first one if it was the last block.)
        /// </summary>
        /// 
        public void RemoveBlock ( EmptyNode node )
            {
            RemoveBlock ( node, true );
            }

        private delegate void BlockRemoveInvokation ( EmptyNode node, bool updateSize ); // @phraseLimit

        public void RemoveBlock(EmptyNode node, bool updateSize)
        {
            if (InvokeRequired)
            {
                Invoke ( new BlockRemoveInvokation ( RemoveBlock ), node, updateSize );
            }
        else
            {
            Block block = FindBlock ( node );
            if (block != null)
                {
                int index = mBlockLayout.Controls.IndexOf ( block );
                if (index < mBlockLayout.Controls.Count) mBlockLayout.Controls.RemoveAt ( index + 1 );
                mBlockLayout.Controls.RemoveAt ( index );
                if (mBlockLayout.Controls.Count == 1) mBlockLayout.Controls.RemoveAt ( 0 );
                block.SizeChanged -= new EventHandler ( Block_SizeChanged );
                if (updateSize) Resize_Blocks ();
                UpdateStripCursorsAccessibleName ( index - 1 );

                // dispose block for freeing window handle only if it is not held in clipboard @phraseLimit
                if (mContentView.clipboard == null || (mContentView.clipboard != null && mContentView.clipboard.Node != block.Node))
                    {
                    block.Dispose ();
                    block = null;
                    }

                }
            }
        }


        // @phraseLimit
        /// <summary>
        /// Remove and dispose all blocks in strip without removing phrases
                /// </summary>
        /// <param name="updateSize"></param>
        /// <returns></returns>
        public int RemoveAllBlocks ( bool updateSize )
            {
            int blocksRemovedCount = 0;
            if (mBlockLayout.Controls.Count > 0)
                {
                    for (int i = mBlockLayout.Controls.Count - 1; i > 0; i--)
                    {
                    if (mBlockLayout.Controls[i] is Block)
                        {
                        RemoveBlock ( (Block)mBlockLayout.Controls[i], updateSize );
                        blocksRemovedCount++;
                        }
                    }
                }
                return blocksRemovedCount;
            }

        public int RemoveAllFollowingBlocks (EmptyNode node,  bool updateSize )
            {
            int blocksRemovedCount = 0;
            int limitIndex = node.Index;
            if (mBlockLayout.Controls.Count > 0)
                {
                for (int i = mBlockLayout.Controls.Count - 1; i > 0; i--)
                    {
                    if (mBlockLayout.Controls[i] is Block)
                        {
                        if (((Block)mBlockLayout.Controls[i]).Node.Index <=  limitIndex)
                            {
                            
                            break;
                            }

                        RemoveBlock ( (Block)mBlockLayout.Controls[i], updateSize );
                        blocksRemovedCount++;
                        }
                    }
                }
            return blocksRemovedCount;
            }



        // @phraseLimit
        private delegate void QuickBlockRemoveInvokation ( Block block, bool updateSize ); // @phraseLimit

        // @phraseLimit
        /// <summary>
        /// Remove and dispose a single block passed as parameter
                /// </summary>
        /// <param name="block"></param>
        /// <param name="updateSize"></param>
        private void RemoveBlock (Block block,  bool updateSize )
            {
            if (InvokeRequired)
                {
                Invoke ( new QuickBlockRemoveInvokation ( RemoveBlock ), block, updateSize );
                }
            else
                {
                        //Block block = (Block) mBlockLayout.Controls[i];
                        if (block != null)
                            {
                            int index = mBlockLayout.Controls.IndexOf ( block );
                            if (index < mBlockLayout.Controls.Count)
                                {
                                Control  stripCursorControl = mBlockLayout.Controls[index + 1];
                                mBlockLayout.Controls.RemoveAt ( index + 1 );
                                if (stripCursorControl != null && stripCursorControl is StripCursor) stripCursorControl.Dispose ();
                                                                                                                                            }
                            mBlockLayout.Controls.RemoveAt ( index );
                            if (mBlockLayout.Controls.Count == 1) mBlockLayout.Controls.RemoveAt ( 0 );
                            block.SizeChanged -= new EventHandler ( Block_SizeChanged );
                            if (updateSize) Resize_Blocks ();
                            UpdateStripCursorsAccessibleName ( index - 1 );

                            // dispose block for freeing window handle only if it is not held in clipboard @phraseLimit
                            if (mContentView.clipboard == null || (mContentView.clipboard != null && mContentView.clipboard.Node != block.Node))
                                {
                                block.Dispose ();
                                block = null;
                                }
                            else // in case block is held in clipboard, just destroy handle
                                block.DestroyBlockHandle ();
                            }
                        }
            }



        /// <summary>
        /// Show the cursor at the current time in the waveform of the current playing block.
        /// </summary>
        public void SetSelectedAudioInBlockFromBlock(Block block, AudioRange audioRange)
        {
            mContentView.SelectionFromStrip = new AudioSelection((PhraseNode)block.Node, mContentView, audioRange);
        }

        /// <summary>
        /// Set the current selected from the block itself. This gets passed to the content view.
        /// </summary>
        public void SetSelectedBlockFromBlock(Block block) { mContentView.SelectedNode = block.Node; }

        /// <summary>
        /// Set the selection from a strip cursor to its index. This gets passed to the content view.
        /// </summary>
        public void SetSelectedIndexFromStripCursor(StripCursor cursor)
        {
            int index = mBlockLayout.Controls.IndexOf(cursor) / 2;
            mContentView.SelectionFromStrip = new StripIndexSelection(Node, mContentView, index);
        }

        /// <summary>
        /// Set the selection from the parent control view. (From ISelectableInContentView)
        /// </summary>
        public void SetSelectionFromContentView(NodeSelection selection)
        {
            SetAccessibleName();
            Highlighted = !(selection is StripIndexSelection) && selection != null;
        }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Editable = true;
            mContentView.SelectionFromStrip = new TextSelection(mNode, mContentView, Label);
            SetAccessibleName();
        }

        /// <summary>
        /// Get the strip index after the given (selected) item.
        /// </summary>
        public int StripIndexAfter(ISelectableInContentView item)
        {
            int lastIndex = Node.PhraseChildCount;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index + 1 : 0 :
                item is Block ? (mBlockLayout.Controls.IndexOf((Control)item) + 1) / 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) / 2 + 1 : lastIndex + 1;
            return index <= lastIndex ? index : lastIndex;
        }

        /// <summary>
        /// Index in the strip before the selected item.
        /// </summary>
        public int StripIndexBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    (mBlockLayout.Controls.Count - 1) / 2 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) / 2 :
                item is StripCursor ? mBlockLayout.Controls.IndexOf((Control)item) / 2 - 1 : -1;
            return index >= 0 ? index : 0;
        }

        /// <summary>
        /// Match the label (case independent) when searching.
        /// </summary>
        public string ToMatch() { return Label.ToLowerInvariant(); }

        /// <summary>
        ///  Updates labels of all blocks in a strip, to be used with   background worker
        /// </summary>
        private void UpdateBlockLabelsInStrip(object sender, DoWorkEventArgs e)
            {
            UpdateBlockLabelsInStrip () ;
            }

        /// <summary>
        ///  Update labels of all blocks in a strip 
                /// </summary>
            public void UpdateBlockLabelsInStrip () 
        {
            mLabelUpdateThread.WaitOne();
            try
            {
                Control BlockControl = null;
                for (int i = 0; i < mBlockLayout.Controls.Count; i++)
                {
                    BlockControl = mBlockLayout.Controls[i];
                    if (BlockControl is Block)
                    {
                        ((Block)BlockControl).UpdateLabelsText();
                    }
                }
            }
            catch (System.Exception)
            {
            }
            finally
            {
                mLabelUpdateThread.ReleaseMutex();
            }
        }

        /// <summary>
        /// Update the colors of the strip and its contents's.
        /// </summary>
        public void UpdateColors() { UpdateColors(ColorSettings); }

        /// <summary>
        /// Update the tab index for the strip and all of its blocks.
        /// </summary>
        public int UpdateTabIndex(int index)
        {
            TabIndex = index;
            ++index;
            foreach (Control c in mBlockLayout.Controls)
            {
                if (c is Block) index = ((Block)c).UpdateTabIndex(index);
            }
            return index;
        }

        /// <summary>
        /// Update all waveforms in the strip with a given priority. (When rendering for the fist time, or selecting.)
        /// </summary>
        public void UpdateWaveforms(int priority)
        {
            foreach (Control c in mBlockLayout.Controls)
            {
                if (c is AudioBlock)
                {
                    mContentView.RenderWaveform(((AudioBlock)c).Waveform, priority);
                }
            }
        }


        // Add content view label to the accessible name of the strip when entering.
        private void AddContentsViewLabel()
        {
            SetAccessibleName();
            if (mContentView.IsEnteringView)
            {
                mLabel.AccessibleName = string.Format("{0} {1}", Localizer.Message("content_view"), mLabel.AccessibleName);
                Thread TrimAccessibleName = new Thread(new ThreadStart(TrimContentsViewAccessibleLabel));
                TrimAccessibleName.Start();
            }
        }

        // Add a cursor at the given index (in the context of the block layout.)
        // Return the new cursor.
        private StripCursor AddCursorAtBlockLayoutIndex(int index)
        {
            StripCursor cursor = new StripCursor();
            cursor.SetHeight(mBlockHeight);
            cursor.ColorSettings = ColorSettings;
            cursor.TabStop = false;
            cursor.SetAccessibleNameForIndex(index / 2);
            mBlockLayout.Controls.Add(cursor);
            mBlockLayout.Controls.SetChildIndex(cursor, index);
            return cursor;
        }

        // Compute the full width of the block layout that can accomodate all blocks.
        // This is exclusive of margins.
        private int BlockLayoutFullWidth
        {
            get
            {
                int w = 0;
                foreach (Control c in mBlockLayout.Controls) w += c.Width + c.Margin.Horizontal;
                return w;
            }
        }

        // Get the minimum width necessary for the block layout to contain all blocks.
        private int BlockLayoutMinimumWidth
        {
            get
            {
                int w_min = 0;
                int count = mBlockLayout.Controls.Count;
                if (count > 2)
                {
                    // there is at least one block; the first block must fit the first two cursors
                    w_min = mBlockLayout.Controls[0].Width + mBlockLayout.Controls[0].Margin.Horizontal +
                        mBlockLayout.Controls[1].Width + mBlockLayout.Controls[1].Margin.Horizontal +
                        mBlockLayout.Controls[2].Width + mBlockLayout.Controls[2].Margin.Horizontal;
                    // following blocks are counted with the following cursor
                    for (int i = 3; i < mBlockLayout.Controls.Count - 1; i += 2)
                    {
                        int w = mBlockLayout.Controls[i].Width + mBlockLayout.Controls[i].Margin.Horizontal +
                            mBlockLayout.Controls[i + 1].Width + mBlockLayout.Controls[i + 1].Margin.Horizontal;
                        if (w > w_min) w_min = w;
                    }
                }
                return w_min;
            }
        }

        // Size of the borders
        private int BorderHeight { get { return Bounds.Height - ClientSize.Height; } }
        private int BorderWidth { get { return Bounds.Width - ClientSize.Width; } }

        // Get the height of the strip for the current block layout size/position.
        // Includes the border size.
        private int HeightForContents
        {
            get
            {
            float sizeMultiplier = 1;//@singleSection
                // determine how many blocks are visible w.r.t. total no. of phrases
            if (LastBlock != null ) //@singleSection
                {
                int lastBlockIndex = mBlockLayout.Controls.IndexOf ( LastBlock ) / 2;
                if (lastBlockIndex > 0  && (( mNode.PhraseChildCount - lastBlockIndex ) < 15
                    ||    lastBlockIndex <= 40    ||    lastBlockIndex %10 == 0))
                                    {
                                        sizeMultiplier = mNode.PhraseChildCount / lastBlockIndex;
                                        //Console.WriteLine ( "Phrase index : " + lastBlockIndex + " Strip height scale: " + sizeMultiplier );
                    }
                }
                // If there are no contents, still show space for the block layout
                return mBlockLayout.Location.Y + Math.Max(mBlockHeight,Convert.ToInt32 ( mBlockLayout.Height * sizeMultiplier) ) +
                    mBlockLayout.Margin.Bottom + BorderHeight;
            }
        }

        public void Resize_All() 
        {
            Resize_Label();
            Resize_Blocks(); 
        }

        // Blocks are added, removed, or their width has changed after the audio scale changed.
        // Heights do not change, unless wrapping.
        private void Resize_Blocks()
        {
            if (mWrap)
            {
                Resize_Wrap();
            }
            else
            {
                // the block layout is resized to fit the blocks exactly; we use the last control to get the total width
                // Control k = mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 1] : null;
                // int width_blocks = k == null ? mBlockLayout.Margin.Horizontal : k.Location.X + k.Width + k.Margin.Right;
                int width_blocks = BlockLayoutFullWidth;
                mBlockLayout.Width = width_blocks;
                Width = WidthForContents;
            }
        }

        // Resize after the label has changed (edited or editable status changed)
        // This is not affected by the wrap contents setting.
        private void Resize_Label()
        {
            // move the block layout up or down if the label height has changed
            // and resize the strip accordingly
            mBlockLayout.Location = new Point(mBlockLayout.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom + mBlockLayout.Margin.Top);
            Size = new Size(WidthForContents,
                mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom + BorderHeight);
        }

        // Resize after the view has changed.
        // No effect when not wrapping.
        private void Resize_View()
        {
            if (mWrap) Resize_Wrap();
        }

        // Resize after wrapping has changed.
        private void Resize_Wrap()
        {
            if (mWrap)
            {
                mBlockLayout.AutoSize = true;
                mBlockLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                mBlockLayout.WrapContents = true;
                // The width of the block layout should fit the available space, unless it's narrower.
                // This may be overridden however by the minimum width to fit the widest waveform.
                int width_fit = mContentView.ClientRectangle.Width - Margin.Horizontal - BorderWidth -
                    mBlockLayout.Margin.Horizontal;
                mBlockLayout.MaximumSize =
                    new Size(Math.Max(BlockLayoutMinimumWidth, Math.Min(BlockLayoutFullWidth, width_fit)), 0);
                Size = new Size(WidthForContents, HeightForContents);
            }
            else
            {
                mBlockLayout.AutoSize = false;
                mBlockLayout.WrapContents = false;
                mBlockLayout.MaximumSize = new Size(0, 0);
                mBlockLayout.Height = mBlockHeight;
                Height = HeightForContents;
                Resize_Blocks();
            }
        }

        // Resize after the zoom factor has changed.
        private void Resize_Zoom()
        {
            if (mWrap)
            {
                Resize_Wrap();
            }
            else
            {
                mBlockLayout.Height = mBlockHeight;
                Height = mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom + BorderHeight;
                Resize_Blocks();
            }
        }

        // Set verbose accessible name for the strip 
        public void SetAccessibleName()
        {
            if (Selection is StripIndexSelection)
            {
                mLabel.AccessibleName = Selection.ToString();
            }
            else
            {
                mLabel.AccessibleName = string.Concat(mNode.Used ? "" : Localizer.Message("unused"),
                    mNode.Label + " " ,
                    mNode.Duration == 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("duration_s_ms"), mNode.Duration / 1000.0),
                    string.Format(Localizer.Message("section_level_to_string"), mNode.IsRooted ? mNode.Level : 0),
                    mNode.PhraseChildCount == 0 ? "" :
                        mNode.PhraseChildCount == 1 ? Localizer.Message("section_one_phrase_to_string") :
                            string.Format(Localizer.Message("section_phrases_to_string"), mNode.PhraseChildCount),
                IsBlocksVisible ? "" : Localizer.Message ("ContentsHidden_StatusMessage") ); // @phraseLimit
            }
        }

        // Reset the accessible name after a short while.
        private void TrimContentsViewAccessibleLabel()
        {
            Thread.Sleep(750);
            SetAccessibleName();
        }

        // Udpate the color of the strip and its contents.
        private void UpdateColors(ColorSettings settings)
        {
            if (settings != null && mNode != null)
            {
                BackColor =
                mLabel.BackColor =
                    mBlockLayout.BackColor =
                    mHighlighted ? settings.StripSelectedBackColor :
                    mNode.Used ?( mNode.PhraseChildCount > 0 ? settings.StripBackColor : settings.StripWithoutPhrasesBackcolor ): settings.StripUnusedBackColor;
                                mLabel.ForeColor =
                    mHighlighted ? settings.StripSelectedForeColor :
                    mNode.Used ? settings.StripForeColor : settings.StripUnusedForeColor;
                mLabel.UpdateColors(settings);
                foreach (Control c in mBlockLayout.Controls)
                {
                    if (c is Block) ((Block)c).UpdateColors();
                    else if (c is StripCursor) ((StripCursor)c).UpdateColors();
                }
            }
        }

        // Update the accessible label of the strip cursors after the given index.
        private void UpdateStripCursorsAccessibleName(int afterIndex)
        {
            for (int i = afterIndex + 2; i < mBlockLayout.Controls.Count; i += 2)
            {
                System.Diagnostics.Debug.Assert(mBlockLayout.Controls[i] is StripCursor);
                ((StripCursor)mBlockLayout.Controls[i]).SetAccessibleNameForIndex(i / 2);
            }
        }

        // Width of the strip to contain the label and the block layout, including borders.
        private int WidthForContents
        {
            get
            {
                int width_label = mLabel.Width + mLabel.Margin.Horizontal;
                int width_layout = mBlockLayout.Width + mBlockLayout.Margin.Horizontal;
                return Math.Max(width_label, width_layout) + BorderWidth;
            }
        }


        // Resize when a block size has changed.
        private void Block_SizeChanged(object sender, EventArgs e) { Resize_Blocks(); }

        // Change the selection depending on the editable state of the label.
        private void Label_EditableChanged(object sender, EventArgs e)
        {
            if (mContentView != null)
            {
                mContentView.SelectionFromStrip = mLabel.Editable ?
                    new TextSelection(mNode, mContentView, mLabel.Label) :
                    new NodeSelection(mNode, mContentView);
            }
        }

        // Update the label of the node after the user edited it.
        private void Label_LabelEditedByUser(object sender, EventArgs e)
        {
            if (mLabel.Label.Trim ()  != "")
            {
                // update the label for the node
                mContentView.RenameStrip(this);
                mContentView.SelectionFromStrip = new NodeSelection(mNode, mContentView);
            }
            else
            {
                // restore the previous label from the node
                mLabel.Label = mNode.Label;
            }
        }

        // Resize the strip according to the editable label, which size can change.
        private void Label_SizeChanged(object sender, EventArgs e)
        {
            int y = mBlockLayout.Location.Y;
            mBlockLayout.Location = new Point(mBlockLayout.Location.X,
                mLabel.Location.Y + mLabel.Height + mLabel.Margin.Bottom + mBlockLayout.Margin.Top);
            Size = new Size(Width, Height - y + mBlockLayout.Location.Y);
        }

        // Select when tabbed into
        private void Strip_Enter(object sender, EventArgs e)
        {
            AddContentsViewLabel();
            mLabel.Focus();
            if ((mContentView.SelectedSection != mNode || mContentView.Selection is StripIndexSelection) &&
                !mContentView.Focusing)
            {
                mContentView.SelectedNode = mNode;
            }
        }

        private void Strip_LocationChanged(object sender, EventArgs e)
        {
            Point offset = ((ScrollableControl)Parent).AutoScrollPosition;
            Point l = new Point(Location.X - offset.X, Location.Y - offset.Y);
            System.Diagnostics.Debug.Print("Location changed to {0}/{1}", Location, l);
        }

        public void DestroyStripHandle ()
            {
            this.DestroyHandle ();
            }
    }
}
