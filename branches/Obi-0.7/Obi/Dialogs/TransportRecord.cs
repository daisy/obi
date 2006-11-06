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
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
//            mRecordingSession.Listen();
        }

        private void mRecordButton_Click(object sender, EventArgs e)
        {
            if (combRecordingSelect.SelectedIndex == 1)
            {
                
                mRecordingSession.Listen();
            }
            else if (combRecordingSelect.SelectedIndex == 0)
            {
                mRecordingSession.CommitIntervalSeconds = Convert.ToInt32(txtCommitInterval.Text);
                
                mRecordingSession.Record();


                
            }
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
            combRecordingSelect.Items.Add("Record");
            combRecordingSelect.Items.Add("Listen");
            combRecordingSelect.SelectedIndex = 0 ;
        }

        private void TmCommit_Tick(object sender, EventArgs e)
        {
//            mRecordingSession.Stop();
            //mRecordingSession.Record();
        }
    }
}