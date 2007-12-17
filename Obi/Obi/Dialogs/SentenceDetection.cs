using System;
using System.Collections ;
using System.Collections.Generic ;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media.data;

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

            if (silence != null)
                mThresholdBox.Text = Audio.PhraseDetection.GetSilenceAmplitude(silence.Audio).ToString();
            else
            {
                MessageBox.Show("No silence file for reference. Setting default value for  threshold");
                mThresholdBox.Text = "0";
            }
            mGapBox.Text = Audio.PhraseDetection.DEFAULT_GAP.ToString();
            mLeadingSilenceBox.Text = Audio.PhraseDetection.DEFAULT_LEADING_SILENCE.ToString();
        }
    }
}