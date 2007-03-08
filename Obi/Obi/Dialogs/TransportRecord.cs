using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog for recording new audio, including marking phrases and pages automatically.
    /// </summary>
    public partial class TransportRecord : Form
    {
        private RecordingSession mRecordingSession;

        /// <summary>
        /// Constructor with no parameter (used by the designer.)
        /// </summary>
        public TransportRecord()
        {
            InitializeComponent();
            mRecordingSession = null;
        }

        /// <summary>
        /// Instantiate a new dialog for an existing session.
        /// </summary>
        /// <param name="recordingSession">The initial recording session.</param>
        public TransportRecord(RecordingSession recordingSession)
        {
            InitializeComponent();
            mRecordingSession = recordingSession;
        }

        /// <summary>
        /// Close the form.
        /// </summary>
        /// <remarks>Recording stops when the form is closing.</remarks>
        private void mStopButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Pause.
        /// </summary>
        private void mPauseButton_Click(object sender, EventArgs e)
        {
            Pause();
        }

        /// <summary>
        /// Pause the recording and switch to listening mode.
        /// If we were recording, the recorded audio is committed.
        /// <summary>
        private void Pause()
        {
            if (mPauseButton.Enabled)
            {
                mTimeDisplay.Enabled = false;
                mTimeDisplayBox.Text = Localizer.Message("listening");
                mRecordingSession.Stop();
                mRecordingSession.Listen();
                mRecordButton.Visible = mRecordButton.Enabled = true;
                if (mPauseButton.Focused) mRecordButton.Focus();
                mPauseButton.Visible = mPauseButton.Enabled = false;
                Text = Localizer.Message("listening");
            }
        }

        /// <summary>
        /// Start recording.
        /// </summary>
        private void mRecordButton_Click(object sender, EventArgs e)
        {
            StartRecording();
        }

        /// <summary>
        ///  Start recording if we were listening.
        /// </summary>
        private void StartRecording ()
        {
            if (mRecordButton.Enabled)
            {
                mRecordingSession.Stop();
                mRecordingSession.Record();
                mPauseButton.Visible = mPauseButton.Enabled = true;
                if (mRecordButton.Focused) mPauseButton.Focus();
                mRecordButton.Visible = mRecordButton.Enabled = false;
                Text = Localizer.Message("recording");
                mTimeDisplay.Start();
            }
        }

        private void mPageMarkButton_Click(object sender, EventArgs e)
        {
            PageMark();
        }

        /// <summary>
        /// Mark a page.
        /// </summary>
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

            mTimeDisplay.Enabled = false;
            mTimeDisplayBox.Text = "Listening";
        }

        private void TransportRecord_FormClosing(object sender, FormClosingEventArgs e)
        {
            mTimeDisplay.Enabled = false;
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
                    mTimeDisplayBox.Focus();
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
           mTimeDisplayBox.Text = sHours + ":" + sMinutes + ":" + sSeconds;
        }


    }
}