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
        }

        /// <summary>
        /// Instantiate the dialog.
        /// </summary>
        /// <param name="silence">The silence phrase.</param>
        public SentenceDetection(PhraseNode silence)
        {
            InitializeComponent();

            if (silence != null)
                {
                double threshold = Audio.PhraseDetection.GetSilenceAmplitude ( silence.Audio );
                if (threshold > Convert.ToDouble( mThresholdNumericBox.Maximum)) 
                    threshold = Convert.ToDouble( mThresholdNumericBox.Maximum) ;

                                mThresholdNumericBox.Value = Convert.ToDecimal ( threshold) ;
                }
            else
                {
                MessageBox.Show ( Localizer.Message ( "no_preceding_silent_phrase" ) );
                                mThresholdNumericBox.Value = 280;
                }
            mGapNumericBox.Value = Convert.ToDecimal ( Audio.PhraseDetection.DEFAULT_GAP ) ;
                        mLeadingNumericBox.Value =Convert.ToDecimal ( Audio.PhraseDetection.DEFAULT_LEADING_SILENCE ) ;
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


        private void mLeadingNumericBox_ValueChanged ( object sender, EventArgs e )
            {
            CheckLeadingSilenceInput ();
            }

        private void mOKButton_Click ( object sender, EventArgs e )
            {
            /*
            if (mThresholdBox.Text.Trim () == ""
                || mGapBox.Text.Trim () == ""
                || mLeadingSilenceBox.Text.Trim () == "")
                {
                MessageBox.Show ( Localizer.Message ( "Textboxs_Empty" ), Localizer.Message ( "Caption_Error" ) );
                return;
                                }
             */ 
                            this.DialogResult = DialogResult.OK;
            }


    }
}