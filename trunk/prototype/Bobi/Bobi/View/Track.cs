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
        private Size baseSize;               // size at zoom factor 1
        private float baseFontSize;          // font size at zoom factor 1
        private urakawa.core.TreeNode node;  // node for this track
        private bool selected;               // selected flag
        private double zoom;                 // zoom factor


        /// <summary>
        /// Create a new, empty track.
        /// </summary>
        public Track()
        {
            InitializeComponent();
            DoubleBuffered = true;
            this.baseSize = Size;
            this.baseFontSize = this.label.Font.SizeInPoints;
            Selected = false;
            Zoom = 1.0;
            this.node = null;
        }

        /// <summary>
        /// Create a new track for a node.
        /// </summary>
        /// <param name="node"></param>
        public Track(urakawa.core.TreeNode node): this()
        {
            this.node = node;
        }


        /// <summary>
        /// Add a new audio block to the track.
        /// </summary>
        public void AddAudioBlock(AudioBlock block)
        {
            block.Colors = ((ProjectView)Parent).ColorSettings;
            layoutPanel.Controls.Add(block);
        }

        /// <summary>
        /// Update colors for this track and its children.
        /// </summary>
        public ColorSettings Colors
        {
            get { return ((ProjectView)Parent).ColorSettings; }
            set
            {
                BackColor = this.selected ? value.TrackSelectedBackColor : value.TrackBackColor;
                ForeColor = this.selected ? value.TrackSelectedForeColor : value.TrackForeColor;
                this.layoutPanel.BackColor = value.TrackLayoutBackColor;
                foreach (Control c in this.layoutPanel.Controls) if (c is AudioBlock) ((AudioBlock)c).Colors = value;
            }
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
            get { return this.selected; }
            set
            {
                this.selected = value;
                if (Parent is ProjectView)
                {
                    Colors = ((ProjectView)Parent).ColorSettings;
                    if (this.selected) ((ProjectView)Parent).ScrollControlIntoView(this);
                }
            }
        }

        /// <summary>
        /// Zoom factor.
        /// </summary>
        public double Zoom
        {
            get { return this.zoom; }
            set
            {
                this.zoom = value;
                int ydiff = this.label.Height;
                this.label.Font = new Font(this.label.Font.FontFamily, 10.0f * (float)this.zoom);
                ydiff = this.label.Height - ydiff;
                this.Size = new Size((int)Math.Round(baseSize.Width * value), (int)Math.Round(baseSize.Height * value));
                this.layoutPanel.Location = new Point(this.layoutPanel.Location.X, this.layoutPanel.Location.Y + ydiff);
                this.layoutPanel.Height -= ydiff;
                foreach (Control c in this.layoutPanel.Controls) if (c is AudioBlock) ((AudioBlock)c).Zoom(this.layoutPanel.Height);
            }
        }

        // Propagate selection upward
        private void SelectUp()
        {
            if (!this.selected)
            {
                ProjectView view = Parent as ProjectView;
                if (view != null)
                {
                    Selected = true;
                    view.SelectFromBelow(this.node);
                }
            }
        }

        // Select the track by clicking it.
        private void Track_Click(object sender, EventArgs e) { SelectUp(); }
    }
}
