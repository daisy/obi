using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class TransportRecord : Form
    {
        private RecordingSession mRecordingSession;

        public TransportRecord()
        {
            InitializeComponent();
        }

        public TransportRecord(RecordingSession recordingSession)
        {
            InitializeComponent();
            mRecordingSession = recordingSession;
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            mRecordingSession.Stop();
            Close();
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            mRecordingSession.Stop();
            mRecordingSession.Listen();
            mPauseButton.Visible = false ;
            mRecordButton.Visible = true;
            mRecordButton.Focus();
        }

        private void mRecordButton_Click(object sender, EventArgs e)
        {
            mRecordingSession.Stop();
            mRecordingSession.Record();
            mPauseButton.Visible = true;
            mRecordButton.Visible = false;
            mPauseButton.Focus();
        }

        private void btnPageMark_Click(object sender, EventArgs e)
        {
            mRecordingSession.MarkPage();
        }

        private void btnBeginSection_Click(object sender, EventArgs e)
        {
            mRecordingSession.NextSection();
        }

        private void btnPhraseMark_Click(object sender, EventArgs e)
        {
            mRecordingSession.NextPhrase();
        }

        private void TransportRecord_Load(object sender, EventArgs e)
        {
            mPauseButton.Visible = false;
            mRecordingSession.Listen();
            mTextVuMeter.Enable = true;
            mTextVuMeter.RecordingSessionObj = mRecordingSession;
        }

        private void TransportRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            mRecordingSession.Stop();
        }

    }
}