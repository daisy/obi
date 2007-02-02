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
            tmDisplayTime.Enabled = false;
            mRecordingSession.Stop();
            Close();
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

/// <summary>
///  Pauses and commits recording if it is going on
/// <summary>
/// </summary>
/// </summary>
        private void Pause()
        {
            if (mPauseButton.Visible == true)
            {
                tmDisplayTime.Enabled = false;
                txtDisplayTime.Text = "Listening";

                mRecordingSession.Stop();
                mRecordingSession.Listen();
                bool PauseFocussed = mPauseButton.Focused;
                mRecordButton.Visible = true;
                mPauseButton.Visible = false;
                this.Text = "Recording Paused";

                if ( PauseFocussed == true )
                mRecordButton.Focus();
            }
        }


        private void mRecordButton_Click(object sender, EventArgs e)
        {
            StartRecording();
        }


        /// <summary>
        ///  Starts Recording if paused or initialised
        /// <summary></summary>
        /// </summary>
        private void StartRecording ()
        {
            if (mRecordButton.Visible == true)
            {
                mRecordingSession.Stop();
                mRecordingSession.Record();
                bool RecordFocussed = mRecordButton.Focused;
                mPauseButton.Visible = true;
                mRecordButton.Visible = false;
                this.Text = "Recording";

                if ( RecordFocussed == true )
                mPauseButton.Focus();

            tmDisplayTime.Start();
            }
        }


        private void btnPageMark_Click(object sender, EventArgs e)
        {
            PageMark();
        }

        private void PageMark()
        {
            mRecordingSession.MarkPage();
        }

        private void btnBeginSection_Click(object sender, EventArgs e)
        {
            NextSection();
        }

        private void NextSection()
        {
            mRecordingSession.NextSection();
        }

        private void btnPhraseMark_Click(object sender, EventArgs e)
        {
            PhraseMark();
        }

        private void PhraseMark ()
        {
            mRecordingSession.NextPhrase();
        }

        private void TransportRecord_Load(object sender, EventArgs e)
        {
            mPauseButton.Visible = false;
            mRecordingSession.Listen();
            mTextVuMeter.Enable = true;
            mTextVuMeter.RecordingSessionObj = mRecordingSession;

            tmDisplayTime.Enabled = false;
            txtDisplayTime.Text = "Listening";
        }

        private void TransportRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            tmDisplayTime.Enabled = false;
            mRecordingSession.Stop();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {

            switch (keyData)
            {
                case Keys.Control | Keys.Space :
                    if (mRecordButton.Visible == true)
                        StartRecording();
                    else if (mPauseButton.Visible == true)
                        Pause();
                    break;

                case Keys.R :
                    StartRecording();
                    break;

                case Keys.A :
                    Pause();
                    break;

                case Keys.H :
                    NextSection();
                    break;

                case Keys.J :
                    PhraseMark();
                    break;

                case Keys.K :
                    PageMark();
                    break;


                case Keys.Control | Keys.Alt | Keys.T :
                    txtDisplayTime.Focus();
                    break;

            }

            return base.ProcessDialogKey(keyData);

        }

        private void tmDisplayTime_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = mRecordingSession.AudioRecorder.TimeOfAsset;


            int Seconds = Convert.ToInt32(dMiliSeconds / 1000);
            int DisplaySeconds = Seconds;
            if (DisplaySeconds > 59)
                DisplaySeconds = DisplaySeconds - (60 * (DisplaySeconds / 60));

            string sSeconds = DisplaySeconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            int DisplayMinutes = Minutes;
            if (DisplayMinutes > 59)
                DisplayMinutes = DisplayMinutes - (60 * (DisplayMinutes / 60));

            string sMinutes = DisplayMinutes.ToString("00");
            int Hours = Minutes / 60;
            int DisplayHours = Hours;
            if (DisplayHours > 23)
                DisplayHours = DisplayHours - 60;

            string sHours = DisplayHours.ToString("00");
           txtDisplayTime.Text = sHours + ":" + sMinutes + ":" + sSeconds;
        }


    }
}