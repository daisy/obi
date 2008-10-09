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
                MessageBox.Show(Localizer.Message("no_preceding_silent_phrase"));
                mThresholdBox.Text = "280";
            }
            mGapBox.Text = Audio.PhraseDetection.DEFAULT_GAP.ToString();
            mLeadingSilenceBox.Text = Audio.PhraseDetection.DEFAULT_LEADING_SILENCE.ToString();
        }

        private void mLeadingSilenceBox_TextChanged ( object sender, EventArgs e )
            {
            CheckLeadingSilenceInput ();            
            }

        private void CheckLeadingSilenceInput ()
            {
            if (mGapBox.Text != "" && mLeadingSilenceBox.Text != ""
 && mGapBox.Text != "-" && mLeadingSilenceBox.Text != "-"
                && Convert.ToInt32 ( mLeadingSilenceBox.Text ) > Convert.ToInt32 ( mGapBox.Text ))
                mLeadingSilenceBox.Text = mGapBox.Text;
            }

        private void mGapBox_TextChanged ( object sender, EventArgs e )
            {
            CheckLeadingSilenceInput ();
            }

        private void mOKButton_Click ( object sender, EventArgs e )
            {
            if (mThresholdBox.Text.Trim () == ""
                || mGapBox.Text.Trim () == ""
                || mLeadingSilenceBox.Text.Trim () == "")
                {
                MessageBox.Show ( Localizer.Message ( "Textboxs_Empty" ), Localizer.Message ( "Caption_Error" ) );
                return;
                                }
                            this.DialogResult = DialogResult.OK;
            }

    }
}