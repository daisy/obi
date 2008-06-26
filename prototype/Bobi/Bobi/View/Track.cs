using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class Track : UserControl
    {
        private float baseFontSize;   // font size at zoom factor 1
        private int baseMargin;       // margins for blocks
        private TrackNode node;       // node for this track
        private Selection selection;  // current selection (whole track or track index)
        private double zoom;          // zoom factor


        /// <summary>
        /// Create a new, empty track.
        /// </summary>
        public Track()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.baseFontSize = this.label.Font.SizeInPoints;
            Selection = null;
            Zoom = 1.0;
            this.node = null;
        }

        /// <summary>
        /// Create a new track for a node.
        /// </summary>
        /// <param name="node"></param>
        public Track(TrackNode node): this()
        {
            this.node = node;
        }


        /// <summary>
        /// Add a new audio block to the track.
        /// </summary>
        public void AddAudioBlock(AudioBlock block)
        {
            block.Colors = ((ProjectView)Parent).Colors;
            this.trackLayout.Controls.Add(block);
        }

        /// <summary>
        /// Set the audio scale for the audio blocks.
        /// </summary>
        public double AudioScale
        {
            set
            {
                foreach (Control c in this.trackLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).AudioScale = value;
            }
        }

        /// <summary>
        /// Update colors for this track and its children.
        /// </summary>
        public ColorSettings Colors
        {
            get { return ((ProjectView)Parent).Colors; }
            set
            {
                BackColor = Selected ? value.TrackSelectedBackColor : value.TrackBackColor;
                ForeColor = Selected ? value.TrackSelectedForeColor : value.TrackForeColor;
                this.trackLayout.BackColor = value.TrackLayoutBackColor;
                foreach (Control c in this.trackLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).Colors = value;
            }
        }

        /// <summary>
        /// Find the audio block for the given audio node.
        /// </summary>
        public AudioBlock FindBlock(AudioNode node)
        {
            foreach (Control c in this.trackLayout.Controls) if (c is AudioBlock && ((AudioBlock)c).Node == node) return (AudioBlock)c;
            return null;
        }

        /// <summary>
        /// Node for this track.
        /// </summary>
        public urakawa.core.TreeNode Node { get { return this.node; } }

        /// <summary>
        /// Get or set the selected flag for this track.
        /// </summary>
        public bool Selected
        {
            get { return this.selection is NodeSelection && this.selection.ContainsNode(this.node); }
        }

        public Selection Selection
        {
            get { return this.selection; }
            set
            {
                this.selection = value;
                trackLayout.Invalidate();
                ProjectView view = Parent as ProjectView;
                if (view != null)
                {
                    Colors = view.Colors;
                    if (Selected) view.ScrollControlIntoView(this);
                }
            }
        }

        /// <summary>
        /// Select from below (i.e. from a block.)
        /// </summary>
        public void SelectFromBelow(Selection selection)
        {
            Selection = selection != null && selection.ContainsNode(this.node) ? selection : null;
            if (Parent is ProjectView) ((ProjectView)Parent).SelectFromBelow(selection);
        }

        /// <summary>
        /// Update size after the contents or zoom factor have changed.
        /// </summary>
        public void UpdateSize()
        {
            int w = this.label.Margin.Left + this.label.Width + this.label.Margin.Right;
            int h = this.label.Margin.Right + this.label.Height + this.label.Margin.Bottom;
            if (this.trackLayout.Controls.Count > 0)
            {
                int h_ = 0;
                int w_ = 0; // this.layoutPanel.Controls[0].Margin.Left;
                foreach (Control c in this.trackLayout.Controls)
                {
                    int h__ = c.Margin.Top + c.Height + c.Margin.Bottom;
                    if (h__ > h_) h_ = h__;
                    w_ += c.Margin.Left + c.Width + c.Margin.Right;
                }
                this.trackLayout.Size = new Size(w_, h_);
                this.trackLayout.Location = new Point(this.trackLayout.Location.X, h);
                w_ = this.trackLayout.Location.X + this.trackLayout.Width + this.trackLayout.Margin.Right;
                Size = new Size(w_ > w ? w_ : w, h + h_ + this.trackLayout.Margin.Bottom);
            }
            else
            {
                Size = new Size(w, h);
            }
        }

        /// <summary>
        /// Get the view that this track is in.
        /// </summary>
        public ProjectView View { get { return Parent as ProjectView; } }

        /// <summary>
        /// Zoom factor.
        /// </summary>
        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value;
                this.label.Font = new Font(this.label.Font.FontFamily, 10.0f * (float)this.zoom);
                foreach (Control c in this.trackLayout.Controls) if (c is AudioBlock) ((AudioBlock)c).Zoom = this.zoom;
                UpdateSize();
            }
        }

        // Propagate selection upward
        private void SelectUp()
        {
            if (!Selected)
            {
                ProjectView view = Parent as ProjectView;
                if (view != null)
                {
                    Selection = new NodeSelection(view, this.node);
                    view.SelectFromBelow(this.selection);
                }
            }
        }

        private void layoutPanel_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ProjectView view = Parent as ProjectView;
                if (view != null)
                {
                    this.Selection = new TrackIndexSelection(view, this.node, trackLayout.IndexForX(e.X));
                    this.trackLayout.Invalidate();
                    view.SelectFromBelow(this.Selection);
                }
            }
        }

        private void Track_Click(object sender, EventArgs e) { SelectUp(); }
    }
}
