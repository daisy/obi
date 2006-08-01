using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using VirtualAudioBackend;
using urakawa.core;

namespace Obi.UserControls
{
    public partial class AudioBlock : UserControl
    {
        private StripManagerPanel mManager;  // the manager for this block
        private CoreNode mNode;              // the phrase node for this block

        public StripManagerPanel Manager
        {
            set
            {
                mManager = value;
            }
        }

        public CoreNode Node
        {
            get
            {
                return mNode;
            }
            set
            {
                mNode = value;
            }
        }

        public string Annotation
        {
            set
            {
                mAnnotationLabel.Text = value;
            }
        }

        public string Time
        {
            set
            {
                mTimeLabel.Text = value;
            }
        }

        public AudioBlock()
        {
            InitializeComponent();
        }

        internal void MarkDeselected()
        {
            BackColor = Color.MistyRose;
        }

        internal void MarkSelected()
        {
            BackColor = Color.LightPink;
        }

        private void AudioBlock_Click(object sender, EventArgs e)
        {
            mManager.SelectedPhrase = mNode; 
        }
    }
}
