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
            mRecordingSession.Listen();
        }

        private void mRecordButton_Click(object sender, EventArgs e)
        {
            mRecordingSession.Record();
        }
    }
}