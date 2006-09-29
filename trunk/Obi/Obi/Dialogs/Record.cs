using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace Obi.Dialogs
{
    /// <summary>
    /// The record dialog.
    /// Start listening as soon as it is open.
    /// </summary>
    public partial class Record : Form
    {
        private int mChannels;                    // required number of channels
        private int mSampleRate;                  // required sample rate
        private int mBitDepth;                    // required bit depth
        private Assets.AssetManager mAssManager;  // the asset manager (for creating new assets)
        // private Audio.VuMeter mVuMeter;           // the VU meter is disabled for iteration 1

        public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;

        private List<Assets.AudioMediaAsset> mAssets;  // the list of assets created while recording

        /// <summary>
        /// The list of assets created.
        /// </summary>
        public List<Assets.AudioMediaAsset> Assets
        {
            get
            {
                return mAssets;
            }
        }

        public Record(int channels, int sampleRate, int bitDepth, Assets.AssetManager assManager)
        {
            InitializeComponent();
            mChannels = channels;
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            mAssManager = assManager;
            // mVuMeter = new Audio.VuMeter();
            mAssets = new List<Assets.AudioMediaAsset>();
            Audio.AudioRecorder.Instance.StateChanged += new Events.Audio.Recorder.StateChangedHandler(AudioRecorder_StateChanged);
            Audio.AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(AudioRecorder_UpdateVuMeter);
        }

        private void AudioRecorder_StateChanged(object sender, Events.Audio.Recorder.StateChangedEventArgs state)
        {
        }

        private void AudioRecorder_UpdateVuMeter(Object sender, Events.Audio.Recorder.UpdateVuMeterEventArgs update)
        {
        }

        private void Record_Load(object sender, EventArgs e)
        {
            // to be replaced by the actual device in use.
            //ArrayList arDevices = new ArrayList();
            //arDevices = Audio.AudioRecorder.Instance.GetInputDevices();
            //Audio.AudioRecorder.Instance.InitDirectSound(1);
            
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
            {
                // mVuMeter.ScaleFactor = 2;
                // mVuMeter.SampleTimeLength = 2000;
                // mVuMeter.UpperThreshold = 50;
                // mVuMeter.LowerThreshold = 10;
                // Audio.AudioRecorder.Instance.VuMeterObject = mVuMeter;
                // mVuMeter.ShowForm();
                StartListening();
            }
            // timer1.Enabled = true;
        }

        private void StartListening()
        {
            Assets.AudioMediaAsset mAudioAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Audio.AudioRecorder.Instance.StartListening(mAudioAsset);
            mRecordButton.Text = Localizer.Message("record");
            mTimeTextBox.Text = Localizer.Message("listening");
        }

        private void btnRecordAndPause_Click(object sender, EventArgs e)
        {
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Listening))
            {
                StopRecording();
                Assets.AudioMediaAsset asset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mAssets.Add(asset);
                Audio.AudioRecorder.Instance.StartRecording(asset);
                StartingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(asset, mAssets.Count - 1, 0));
                mRecordButton.Text = Localizer.Message("pause");
                mTimer.Enabled = true;
                // mPhraseMarkerButton.Enabled = false;
            }
            else if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Recording))
            {
                StopRecording();
                FinishingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAssets[mAssets.Count - 1], mAssets.Count - 1,
                    Audio.AudioRecorder.Instance.TimeOfAsset));
                mRecordButton.Text = Localizer.Message("record");
                StartListening();
                mTimer.Enabled = false;
                // mPhraseMarkerButton.Enabled = true;
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Recording))
            {
                StopRecording();
                FinishingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAssets[mAssets.Count - 1], mAssets.Count - 1,
                    Audio.AudioRecorder.Instance.TimeOfAsset));
            }
            else
            {
                StopRecording();
            }
            this.Close();
        }

        /// <summary>
        /// Stop recording. If this generates an exception, catch it and return Cancel.
        /// </summary>
        private void StopRecording()
        {
            try
            {
                Audio.AudioRecorder.Instance.StopRecording();
            }
            catch (Exception)
            {
                MessageBox.Show(Localizer.Message("recorder_error_text"),@Localizer.Message("recorder_error_caption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                Close();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = Audio.AudioRecorder.Instance.TimeOfAsset;
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
            mTimeTextBox.Text = sHours + ":" + sMinutes + ":" + sSeconds;
            /*
            int Seconds = Convert.ToInt32(dMiliSeconds / 1000);
            string sSeconds = Seconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes = Minutes.ToString("00");
            int Hours = Minutes / 60;
            string sHours = Hours.ToString("00");
            mTimeTextBox.Text = sHours + ":" + sMinutes + ":" + sSeconds;
            */
            ContinuingPhrase(this, new Events.Audio.Recorder.PhraseEventArgs(mAssets[mAssets.Count - 1], mAssets.Count - 1,
                Audio.AudioRecorder.Instance.TimeOfAsset));
        }

        private void mPhraseMarkerButton_Click_1(object sender, EventArgs e)
        {/*
            Assets.AudioMediaAsset mPhraseMarkerAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Obi.Assets.AudioMediaAsset newPhrase = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Listening))
            {
                Audio.AudioRecorder.Instance.StopRecording();
                mTimer.Enabled = false;
            }
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Recording))
            {
                Audio.AudioRecorder.Instance.StopRecording();
                EndTime = CurrentTime;
                Obi.Events.Audio.Recorder.PhraseEventArgs mEnd = new Obi.Events.Audio.Recorder.PhraseEventArgs(mPhraseMarkerAsset);
                FinishingPhrase(this, mEnd);
                mTimer.Enabled = false;
            }
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
            {
                Obi.Events.Audio.Recorder.PhraseEventArgs mStart = new Obi.Events.Audio.Recorder.PhraseEventArgs(newPhrase);
                StartingPhrase(this, mStart);
                Audio.AudioRecorder.Instance.StartRecording(mPhraseMarkerAsset);
                CurrentTime = Audio.AudioRecorder.Instance.TimeOfAsset;
                Obi.Events.Audio.Recorder.PhraseEventArgs mContinue = new Obi.Events.Audio.Recorder.PhraseEventArgs(newPhrase);
                ContinuingPhrase(this, mContinue);
                mTimer.Enabled = true;
                mRecordButton.Enabled = false;
           } */
        }

        private void Record_FormClosing(object sender, FormClosingEventArgs e)
        {
            mTimer.Enabled = false;
            Audio.AudioRecorder.Instance.StopRecording();
            // Audio.AudioRecorder.Instance.VuMeterObject.CloseVuMeterForm();
        }
    }
}
