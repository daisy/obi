using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Zaboom.UserControls
{
    public interface Selectable
    {
        bool Selected { get; set; }
    }

    public partial class AudioBlock : UserControl, Selectable
    {
        private bool selected;
        protected ProjectPanel panel;

        private static readonly Color BACK_COLOR_SELECTED = Color.Aquamarine;
        private static readonly Color BACK_COLOR_UNSELECTED = Color.RoyalBlue;

        public AudioBlock()
        {
            InitializeComponent();
            panel = null;
            Selected = false;
        }

        public urakawa.core.TreeNode Node
        {
            set
            {
                waveformPanel.Node = value;
                double dur = waveformPanel.AudioData.getAudioDuration().getTimeDeltaAsMillisecondFloat() / 1000.0;
                timeLabel.Text = String.Format("{0}s", dur.ToString("0.00"));
                infoPanel.Width = timeLabel.Width + infoPanel.Padding.Right;
                Invalidate();
            }
        }

        public int PixelsPerSecond { set { waveformPanel.PixelsPerSecond = value; } }

        public ProjectPanel Panel
        {
            set
            {
                if (panel != null) throw new Exception("Panel is already set!");
                if (value == null) throw new urakawa.exception.MethodParameterIsNullException("Null panel!"); 
                panel = value;
            }
        }

        public Project Project { set { waveformPanel.Project = value; } }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            Selected = !selected;
            if (selected)
            {
                panel.ReplaceSelection(this);
            }
            else
            {
                panel.SelectionChanged(this);
            }
        }

        private void ContentsSizeChanged(object sender, EventArgs e)
        {
            waveformPanel.Location = new Point(Padding.Left + infoPanel.Width, waveformPanel.Location.Y);
            Width = Padding.Left + infoPanel.Width + waveformPanel.Width + Padding.Right;
        }

        #region Selectable Members

        /// <summary>
        /// Used by the project panel to ask or tell the control when it selected or not.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set
            {
                if (selected != value)
                {
                    selected = value;
                    BackColor = selected ? BACK_COLOR_SELECTED : BACK_COLOR_UNSELECTED;
                }
            }
        }

        #endregion
    }
}
