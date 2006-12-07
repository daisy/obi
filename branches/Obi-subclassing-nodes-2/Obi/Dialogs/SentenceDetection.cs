using System;
using System.Collections ;
using System.Collections.Generic ;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for sentence detection parameter settting.
    /// </summary>
    public partial class SentenceDetection : Form
    {
        public long Threshold
        {
            get { return Convert.ToInt64(mThresholdBox.Text); }
        }

        public double Gap
        {
            get { return Convert.ToDouble(mGapBox.Text); }
        }

        public double LeadingSilence
        {
            get { return Convert.ToDouble(mLeadingSilenceBox.Text); }
        }

        public SentenceDetection()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Instantiate the dialog.
        /// </summary>
        /// <param name="silence">The silence phrase.</param>
        public SentenceDetection(PhraseNode silence)
        {
            InitializeComponent();
            Assets.AudioMediaAsset silenceAsset = silence.Asset;
            mThresholdBox.Text = silenceAsset.GetSilenceAmplitude().ToString();
            mGapBox.Text = Assets.AudioMediaAsset.DefaultGap.ToString();
            mLeadingSilenceBox.Text = Assets.AudioMediaAsset.DefaultLeadingSilence.ToString();
        }
    }
}