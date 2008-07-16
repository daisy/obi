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

        public ImportFileSplitSize(uint duration)
        {
            InitializeComponent();
            mMaxPhraseDurationMinutes = duration;
            mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString();
            mCanClose = true;
        }

        public uint MaxPhraseDurationMinutes
        {
            get { return mMaxPhraseDurationMinutes; }
        }

        private void mOKButton_Click(object sender, EventArgs e)
        {
            try
            {
                uint duration = Convert.ToUInt32(mPhraseSizeTextBox.Text);
                mMaxPhraseDurationMinutes = duration;
            }
            catch (System.Exception)
            {
                MessageBox.Show(Localizer.Message("max_phrase_duration_invalid_input"));
                mPhraseSizeTextBox.Text = mMaxPhraseDurationMinutes.ToString();
                mCanClose = false;
            }
        }

        private void ImportFileSplitSize_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!mCanClose)
            {
                mCanClose = true;
                e.Cancel = true;
            }
        }
    }
}