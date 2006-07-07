using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using UrakawaApplicationBackend;

namespace Zaboom
{
    public partial class SplitForm : Form
    {
        private AudioMediaAsset mAsset;  // the asset to split
        private double mTime;            // the time of the split (in ms)
        private AudioPlayer mPlayer;     // the audio player for preview

        public double Time
        {
            get
            {
                return mTime;
            }
        }

        /// <summary>
        /// Create a new split form to let the user decide where to split the asset.
        /// </summary>
        /// <param name="asset">The asset to split.</param>
        /// <param name="player">The application's audio player for preview.</param>
        public SplitForm(AudioMediaAsset asset, AudioPlayer player)
        {
            InitializeComponent();
            mAsset = asset;
            mTime = player.CurrentTimePosition;
            mPlayer = player;
        }

        private void mPreviewButton_Click(object sender, EventArgs e)
        {

        }

        private void mBeforeButton_Click(object sender, EventArgs e)
        {

        }

        private void mAfterButton_Click(object sender, EventArgs e)
        {

        }
    }
}