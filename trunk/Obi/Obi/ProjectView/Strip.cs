using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace Obi.ProjectView
{
    public partial class Strip : UserControl, ISearchable, ISelectableInContentViewWithColors
    {
        private int mBlockLayoutBaseHeight;  // base height of the block layout (for zooming)
        private Mutex mLabelUpdateThread;    // thread to update labels
        private SectionNode mNode;           // the section node for this strip
        private ContentView mParentView;     // parent strip view
        private bool mHighlighted;           // highlighted flag (when the section node is selected)
        private bool mWrap;                  // wrap contents


        /// <summary>
        /// This constructor is used by the designer.
        /// </summary>
        public Strip()
        {
            InitializeComponent();
            mBlockLayoutBaseHeight = mBlockLayout.Height;
            mLabel.Editable = false;
            mNode = null;
            Highlighted = false;
            mWrap = false;
            mLabelUpdateThread = new Mutex();
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
            mParentView = parent;
            ZoomFactor = mParentView.ZoomFactor;
            UpdateColors();
            SetAccessibleName();
        }


        /// <summary>
        /// Add a new block for a phrase node.
        /// </summary>
        public Block AddBlockForNode(EmptyNode node)
        {
            Block block = node is PhraseNode ? new AudioBlock((PhraseNode)node, this) : new Block(node, this);
            mBlockLayout.Controls.Add(block);
            mBlockLayout.Controls.SetChildIndex(block, node.Index);
            block.SetZoomFactorAndHeight(mParentView.ZoomFactor, mBlockLayout.Height);
            UpdateSize();
            return block;
        }

        /// <summary>
        /// Return the block after the selected block or strip. In the case of a strip it is the first block.
        /// Return null if this is the last block, there are no blocks, or nothing was selected in the first place.
        /// This is used for arrow navigation.
        /// </summary>
        public Block BlockAfter(ISelectableInContentView item)
        {
            int count = mBlockLayout.Controls.Count;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index : 0 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) + 1 : count;
            return index < count ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Index in the strip after the current item.
        /// </summary>
        public int StripIndexAfter(ISelectableInContentView item)
        {
            int count = mBlockLayout.Controls.Count;
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index + 1 : 0 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) + 1 : count;
            return index <= count ? index : count;
        }

        /// <summary>
        /// Return the block before the selected block or strip. In the case of a strip this is the last block.
        /// Return null if this the first block, there are no blocks, or nothing was selected in the first place.
        /// </summary>
        public Block BlockBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    mBlockLayout.Controls.Count - 1 :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) - 1 : 0;
            return index >= 0 ? (Block)mBlockLayout.Controls[index] : null;
        }

        /// <summary>
        /// Index in the strip before the current item.
        /// </summary>
        public int StripIndexBefore(ISelectableInContentView item)
        {
            int index = item is Strip ?
                ((Strip)item).Selection is StripIndexSelection ? ((StripIndexSelection)((Strip)item).Selection).Index - 1 :
                    mBlockLayout.Controls.Count :
                item is Block ? mBlockLayout.Controls.IndexOf((Control)item) : 0;
            return index >= 0 ? index : 0;
        }

        /// <summary>
        /// Get the content view for the strip.
        /// </summary>
        public ContentView ContentView { get { return mParentView; } }

        /// <summary>
        /// Find the block for the corresponding node inside the strip.
        /// </summary>
        public Block FindBlock(EmptyNode node)
        {
            foreach (Control c in mBlockLayout.Controls)
            {
                // this needs to be updated for container blocks
                if (c is Block && ((Block)c).Node == node) return (Block)c;
            }
            return null;
        }

        /// <summary>
        /// Return the first block in the strip, or null if empty.
        /// </summary>
        public Block FirstBlock
        {
            get { return mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[0] as Block : null; }
        }

        /// <summary>
        /// Focus on the label.
        /// </summary>
        public void FocusStripLabel() { mLabel.Focus(); }

        /// <summary>
        /// Set the highlighted flag for the strip when the node is (de)selected.
        /// </summary>
        public bool Highlighted
        {
            get { return mHighlighted; }
            set
            {
                mHighlighted = value && !(mParentView.Selection is TextSelection);
                if (mHighlighted && mLabel.Editable) mLabel.Editable = false;
                UpdateColors();
                mBlockLayout.Invalidate();
            }
        }

        /// <summary>
        /// The label of the strip where the title of the section can be edited.
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
                int w = mLabel.MinimumSize.Width + mLabel.Margin.Left + mLabel.Margin.Right;
                if (w > MinimumSize.Width) MinimumSize = new Size(w, MinimumSize.Height);
            }
        }

        /// <summary>
        /// Return the last block in the strip, or null if empty.
        /// </summary>
        public Block LastBlock
        {
            get
            {
                return mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 1] as Block :
                    null;
            }
        }

        /// <summary>
        /// Get the tab index of the last control in the strip
        /// </summary>
        public int LastTabIndex
        {
            get
            {
                int last = mBlockLayout.Controls.Count - 1;
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
        /// Get the strips view to which this strip belongs.
        /// </summary>
        public ContentView ParentView { get { return mParentView; } }

        /// <summary>
        /// Remove the block for the given node.
        /// </summary>
        public void RemoveBlock(EmptyNode node)
        {
            Block block = FindBlock(node);
            if (block != null)
            {
                mBlockLayout.Controls.Remove(block);
                UpdateSize();
            }
        }

        /// <summary>
        /// Select a block in the strip.
        /// </summary>
        public Block SelectedBlock { set { mParentView.SelectedNode = value.Node; } }

        /// <summary>
        /// Get the current selection, if this node is concerned.
        /// </summary>
        public NodeSelection Selection
        {
            get
            {
                NodeSelection selection = mParentView == null ? null : mParentView.Selection;
                return selection == null || selection.Node != mNode ? null : selection;
            }
        }

        /// <summary>
        /// Set the selection from the parent view
        /// </summary>
        public NodeSelection SelectionFromView
        {
            set
            {
                SetAccessibleName();
                Highlighted = !(value is StripIndexSelection) && value != null;
                mBlockLayout.Invalidate();
            }
        }

        /// <summary>
        /// Show the cursor at the current time in the waveform of the current playing block.
        /// </summary>
        public void SelectTimeInBlock(Block block, AudioRange audioRange)
        {
            mParentView.SelectionFromStrip = new AudioSelection((PhraseNode)block.Node, mParentView, audioRange);
        }

        /// <summary>
        /// Start renaming the strip.
        /// </summary>
        public void StartRenaming()
        {
            mLabel.Editable = true;
            mParentView.SelectionFromStrip = new TextSelection(mNode, mParentView, Label);
            SetAccessibleName();
        }

        /// <summary>
        /// Clear the selection in the strip from its contents.
        /// </summary>
        public void UnselectInStrip()
        {
            mParentView.Selection = null;
        }

        public ColorSettings ColorSettings
        {
            get { return mParentView == null ? null : mParentView.ColorSettings; }
            set { UpdateColors(value); }
        }

        /// <summary>
        /// Update the colors of the block when the state of its node has changed.
        /// </summary>
        public void UpdateColors(ColorSettings settings)
        {
            if (settings != null && mNode != null)
            {
                BackColor =
                mLabel.BackColor =
                mBlockLayout.BackColor =
                    mHighlighted ? settings.StripSelectedBackColor :
                    mNode.Used ? settings.StripBackColor : settings.StripUnusedBackColor;
                mLabel.ForeColor =
                    mHighlighted ? settings.StripSelectedForeColor :
                    mNode.Used ? settings.StripForeColor : settings.StripUnusedForeColor;
                mLabel.UpdateColors(settings);
                mBlockLayout.UpdateColors(settings);
            }
        }

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
        /// Set the wrap parameter.
        /// </summary>
        public bool Wrap
        {
            set
            {
                mWrap = value;
                UpdateSize();
                if (mWrap)
                {
                    mParentView.SizeChanged += new EventHandler(mParentView_SizeChanged);
                }
                else
                {
                    mParentView.SizeChanged -= new EventHandler(mParentView_SizeChanged);
                }
            }
        }

        public float AudioScale
        {
            get { return mParentView == null ? 0.01f : mParentView.AudioScale; }
            set
            {
                foreach (Control c in mBlockLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).AudioScale = value;
                ResizeToBlocksLength(mBlockLayout.Height);
            }
        }

        public float ZoomFactor
        {
            set
            {
                if (value > 0.0f)
                {
                    mLabel.ZoomFactor = value;
                    int h = (int)Math.Round(value * mBlockLayoutBaseHeight);
                    foreach (Control c in mBlockLayout.Controls) if (c is Block) ((Block)c).SetZoomFactorAndHeight(value, h);
                    ResizeToBlocksLength(h);
                }
            }
        }

        private void ResizeToBlocksLength(int h)
        {
            Control k = mBlockLayout.Controls.Count > 0 ? mBlockLayout.Controls[mBlockLayout.Controls.Count - 1] : null;
            int w = k == null ? Width - mBlockLayout.Margin.Horizontal :
                k.Location.X + k.Width + k.Margin.Right;
            int h_ = mBlockLayout.Height - h;
            mBlockLayout.Size = new Size(w, h);
            Size = new Size(mBlockLayout.Location.X + w + mBlockLayout.Margin.Right, Height - h_);
        }


        private void mParentView_SizeChanged(object sender, EventArgs e)
        {
            UpdateSize();
        }


        #region ISearchable Members

        public string ToMatch()
        {
            return Label.ToLowerInvariant();
        }

        #endregion

        // Add content view label to the accessible name of the strip when entering.
        private void AddContentsViewLabel()
        {
            SetAccessibleName();
            if (mParentView.IsEnteringView)
            {
                mLabel.AccessibleName = string.Format("{0} {1}", Localizer.Message("content_view"), mLabel.AccessibleName);
                Thread TrimAccessibleName = new Thread(new ThreadStart(TrimContentsViewAccessibleLabel));
                TrimAccessibleName.Start();
            }
        }

        private void Label_EditableChanged(object sender, EventArgs e)
        {
            if (mParentView != null)
            {
                mParentView.SelectionFromStrip = mLabel.Editable ?
                    new TextSelection(mNode, mParentView, mLabel.Label) :
                    new NodeSelection(mNode, mParentView);
            }
        }

        // Update the label of the node after the user edited it.
        private void Label_LabelEditedByUser(object sender, EventArgs e)
        {
            if (mLabel.Label != "")
            {
                // update the label for the node
                mParentView.RenameStrip(this);
                mParentView.SelectionFromStrip = new NodeSelection(mNode, mParentView);
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

        // Set verbose accessible name for the strip 
        private void SetAccessibleName()
        {
            if (Selection is StripIndexSelection)
            {
                mLabel.AccessibleName = Selection.ToString();
            }
            else
            {
                mLabel.AccessibleName = string.Concat(mNode.Used ? "" : Localizer.Message("unused"),
                    mNode.Label,
                    mNode.Duration == 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("duration_s_ms"), mNode.Duration / 1000.0),
                    string.Format(Localizer.Message("section_level_to_string"), mNode.IsRooted ? mNode.Level : 0),
                    mNode.PhraseChildCount == 0 ? "" :
                        mNode.PhraseChildCount == 1 ? Localizer.Message("section_one_phrase_to_string") :
                            string.Format(Localizer.Message("section_phrases_to_string"), mNode.PhraseChildCount));
            }
        }

        // Select when tabbed into
        private void Strip_Enter(object sender, EventArgs e)
        {
            AddContentsViewLabel();
             mLabel.Focus();

            if ((mParentView.SelectedSection != mNode || mParentView.Selection is StripIndexSelection) &&
                !mParentView.Focusing)
            {
                mParentView.SelectedNode = mNode;
                            }
        }

        // Reset the accessible name after a short while.
        private void TrimContentsViewAccessibleLabel()
        {
            Thread.Sleep(750);
            SetAccessibleName();
        }

        // Update the size of the strip to use the available width of the view
        private void UpdateSize()
        {
            if (mWrap)
            {
                MinimumSize = new Size(ParentView.Width, MinimumSize.Height);
                Width = ParentView.Width;
                mBlockLayout.AutoSize = true;
                mBlockLayout.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                mBlockLayout.WrapContents = true;
                Height = mBlockLayout.Location.Y + mBlockLayout.Height + mBlockLayout.Margin.Bottom;
            }
            else
            {
                mBlockLayout.AutoSize = false;
                mBlockLayout.WrapContents = false;
                // Compute the minimum width of the block panel (largest of label or block layout width.)
                int wl = mLabel.Width;
                int wb = 0;
                foreach (Control c in mBlockLayout.Controls) wb += c.Width + c.Margin.Horizontal;
                int w = wl > wb ? wl : wb;
                mBlockLayout.Size = new Size(wb, mBlockLayout.Height);
                Size = new Size(w + mBlockLayout.Margin.Horizontal, Height);
            }
        }

        public void UpdateBlockLabelsInStrip(object sender, DoWorkEventArgs e)
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
                }// end loop
            }
            catch (System.Exception)
            {
                mLabelUpdateThread.ReleaseMutex();
                return;
            }
                        mLabelUpdateThread.ReleaseMutex();
        }

        private void mBlockLayout_MouseClick(object sender, MouseEventArgs e)
        {
            if (mParentView != null && e.Button == MouseButtons.Left)
            {
                mParentView.SelectionFromStrip = new StripIndexSelection(mNode, mParentView, mBlockLayout.IndexForX(e.X));
            }
        }

        public void UpdateWaveforms()
        {
            foreach (Control c in mBlockLayout.Controls)
            {
                if (c is AudioBlock) mParentView.RenderWaveform(new WaveformWithPriority(((AudioBlock)c).Waveform));
            }
        }
    }
}
