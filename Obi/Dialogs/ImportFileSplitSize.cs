using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;

namespace Obi.Dialogs
{
    public partial class ImportFileSplitSize : Form
    {
        private uint mMaxPhraseDurationMinutes;
        private bool mCanClose;

        public ImportFileSplitSize(bool split, uint duration)
        {
            InitializeComponent();
            mSplitCheckBox.Checked = split;
            mMaxPhraseDurationMinutes = duration;
            mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString();
            mPhraseSizeTextBox.ReadOnly = !split;
            mCanClose = true;
        }

        /// <summary>
        /// When set, the user wants the phrases to be split.
        /// </summary>
        public bool SplitPhrases { get { return mSplitCheckBox.Checked; } }

        public bool createSectionForEachPhrase { get { return mCreateAudioFilePerSectionCheckBox.Checked; } }
        public bool SortFileNamesAscending { get { return m_chkSortAscending.Checked; } }

        //  create a porpety for section import
        /// <summary>
        /// Maximum durations of imported phrases.
        /// </summary>
        public uint MaxPhraseDurationMinutes { get { return mMaxPhraseDurationMinutes; } }


        // Check that the duration is a number.
        private void mOKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            if (mSplitCheckBox.Checked)
                {
                try
                    {
                    uint duration = Convert.ToUInt32 ( mPhraseSizeTextBox.Text );
                    mMaxPhraseDurationMinutes = duration;
                    if (duration <= 0) throw new Exception ();
                    }
                catch (System.Exception)
                    {
                    MessageBox.Show ( Localizer.Message ( "max_phrase_duration_invalid_input" ) );
                    mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString ();
                    mCanClose = false;
                    }
                }
        }

        // Check that we have a valid value before we close, otherwise cancel.
        private void ImportFileSplitSize_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                mCanClose = true;
                e.Cancel = true;
            }
        }

        // When not splitting, don't edit the text box.
        private void mSplitCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            mPhraseSizeTextBox.ReadOnly = !mSplitCheckBox.Checked;
        }
    }
}