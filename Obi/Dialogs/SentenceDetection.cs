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
            get { return Convert.ToInt64(mThresholdNumericBox.Value); }
        }

        public double Gap
        {
            get { return Convert.ToDouble(mGapNumericBox.Value); }
        }

        public double LeadingSilence
        {
            get { return Convert.ToDouble(mLeadingNumericBox.Value); }
        }

        public SentenceDetection()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Phrase detection settings dialog.htm");
        }

        
        /// <summary>
        /// Instantiate the dialog.
        /// </summary>
        /// <param name="silence">The silence phrase.</param>
        public SentenceDetection(PhraseNode silence,Settings settings):this    ()
        {
            
            if (silence != null)
            {
                double threshold = Audio.PhraseDetection.GetSilenceAmplitude(silence.Audio);
                if (threshold > Convert.ToDouble(mThresholdNumericBox.Maximum))
                {
                    threshold = Convert.ToDouble(mThresholdNumericBox.Maximum);
                }
                mThresholdNumericBox.Value = Convert.ToDecimal(threshold);
            }
            else
            {
                MessageBox.Show(Localizer.Message("no_preceding_silence_phrase"),
                    Localizer.Message("no_preceding_silence_phrase_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                mThresholdNumericBox.Value = Convert.ToDecimal(Audio.PhraseDetection.DEFAULT_THRESHOLD);
            }
            mGapNumericBox.Value = Convert.ToDecimal(Audio.PhraseDetection.DEFAULT_GAP);
            mLeadingNumericBox.Value = Convert.ToDecimal(Audio.PhraseDetection.DEFAULT_LEADING_SILENCE);
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new System.Drawing.Font(settings.ObiFont, this.Font.Size, System.Drawing.FontStyle.Regular);//@fontconfig
            }
        }

        public SentenceDetection(long threshold, double gap, double leadingSilence ):this    ()
        {
            double thres = Convert.ToDouble (threshold);
            mThresholdNumericBox.Value = Convert.ToDecimal( thres);
            
            mGapNumericBox.Value = Convert.ToDecimal( gap);
            mLeadingNumericBox.Value = Convert.ToDecimal( leadingSilence);
        }
        private void mGapNumericBox_ValueChanged ( object sender, EventArgs e )
            {
            CheckLeadingSilenceInput ();
            }


        private void CheckLeadingSilenceInput ()
            {
            if ( mLeadingNumericBox.Value >  mGapNumericBox.Value )
                mLeadingNumericBox.Value = mGapNumericBox.Value ;
                        }


        private void mLeadingNumericBox_ValueChanged(object sender, EventArgs e)
        {
            CheckLeadingSilenceInput();
        }
    }
}