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
    /// 
    /// </summary>
    /// <remarks>JQ</remarks>
    public partial class Record : Form
    {
        private int mChannels;                  // required number of channels
        private int mSampleRate;                // required sample rate
        private int mBitDepth;                  // required bit depth
        private Assets.AssetManager mAssManager;       // the asset manager (for creating new assets)
        private Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
        private event Events.Audio.Recorder.StartingPhraseHandler StartPhrase;
        private event Events.Audio.Recorder.ContinuingPhraseHandler ContinuePhrase;
        private event Events.Audio.Recorder.FinishingPhraseHandler FinishPhrase;
        double BeginTime = 0;
        double CurrentTime ;//time while the phrase is recorded
        double EndTime;//the end time when finish phrase event is triggered


        private List<Assets.AudioMediaAsset> mAssets;  // the list of assets created while recording

        // public event Events.Audio.Recorder.StartingPhraseHandler StartingPhrase;
        // public event Events.Audio.Recorder.ContinuingPhraseHandler ContinuingPhrase;
        // public event Events.Audio.Recorder.FinishingPhraseHandler FinishingPhrase;

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
            mAssets = new List<Assets.AudioMediaAsset>();
            Audio.AudioRecorder.Instance.StateChanged += new Events.Audio.Recorder.StateChangedHandler(AudioRecorder_StateChanged);
            Audio.AudioRecorder.Instance.UpdateVuMeterFromRecorder +=
                new Events.Audio.Recorder.UpdateVuMeterHandler(AudioRecorder_UpdateVuMeter);
            StartPhrase += new Obi.Events.Audio.Recorder.StartingPhraseHandler(Start_Phrase);
            ContinuePhrase += new Obi.Events.Audio.Recorder.ContinuingPhraseHandler(Continue_Phrase);
            FinishPhrase += new Events.Audio.Recorder.FinishingPhraseHandler(Finish_Phrase);
        }

        private void AudioRecorder_StateChanged(object sender, Events.Audio.Recorder.StateChangedEventArgs state)
        {
        }

        private void AudioRecorder_UpdateVuMeter(Object sender, Events.Audio.Recorder.UpdateVuMeterEventArgs update)
        {
        }
        private void Finish_Phrase(object sender, Events.Audio.Recorder.PhraseEventArgs Finish)
        {
}
        private void Continue_Phrase(object sender, Events.Audio.Recorder.PhraseEventArgs contine)
        {
}
        
        private void Start_Phrase(object sender, Events.Audio.Recorder.PhraseEventArgs start)
        {
        }

        private void Record_Load(object sender, EventArgs e)
        {
            //suman
//on closing the record form the recording does not stop
            ArrayList arDevices = new ArrayList();
            arDevices = Audio.AudioRecorder.Instance.GetInputDevices();
            Audio.AudioRecorder.Instance.InitDirectSound(1);
            
            if(Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
            {
            ob_VuMeter.ScaleFactor = 2;
            ob_VuMeter.SampleTimeLength = 2000;
            ob_VuMeter.UpperThreshold = 50;
            ob_VuMeter.LowerThreshold = 10;
            Audio.AudioRecorder.Instance.VuMeterObject = ob_VuMeter;
            ob_VuMeter.ShowForm();
            Assets.AudioMediaAsset mAudioAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Audio.AudioRecorder.Instance.StartListening(mAudioAsset);
            mRecordButton.Text = Localizer.Message("record");}
            timer1.Enabled = true;
        }



        private void btnRecordAndPause_Click(object sender, EventArgs e)
        {
            
Assets.AudioMediaAsset mRecordAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            if(Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Listening))
            {
            Audio.AudioRecorder.Instance.StopRecording();
            timer1.Enabled = false;
            //mRecordButton.Text = Localizer.Message("record");
            }                   
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
            {
                Audio.AudioRecorder.Instance.StartRecording(mRecordAsset);
                mRecordButton.Text = Localizer.Message("pause");
                timer1.Enabled = true;
                mPhraseMarkerButton.Enabled = false;
            }
        
        else if(Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Recording) && mRecordButton.Text.Contains("Pause"))
            {
                Audio.AudioRecorder.Instance.StopRecording();
                mRecordButton.Text = Localizer.Message("record");
                timer1.Enabled = false;
                mPhraseMarkerButton.Enabled = true;
                
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Recording ||
                Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Listening)
            {
                Audio.AudioRecorder.Instance.StopRecording();
            }
            Audio.AudioRecorder.Instance.VuMeterObject.CloseVuMeterForm();
            timer1.Enabled = false;
            this.Close();
        }

private void timer1_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = Audio.AudioRecorder.Instance.TimeOfAsset;
            int Seconds = Convert.ToInt32(dMiliSeconds / 1000);
            string sSeconds = Seconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes = Minutes.ToString("00");
            int Hours = Minutes / 60;
            string sHours = Hours.ToString("00");
            mTimeTextBox.Text = sHours + ":" + sMinutes + ":" + sSeconds;
            //mTimeTextBox.Text = Audio.AudioRecorder.Instance.GetTime.ToString();
        }

        

        private void mPhraseMarkerButton_Click_1(object sender, EventArgs e)
        {
            Assets.AudioMediaAsset mPhraseMarkerAsset = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            Obi.Assets.AudioMediaAsset newPhrase = mAssManager.NewAudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Listening))
            {
                Audio.AudioRecorder.Instance.StopRecording();
                timer1.Enabled = false;
            }
                if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Recording))
                {
                    Audio.AudioRecorder.Instance.StopRecording();
                    EndTime = CurrentTime;
                    Obi.Events.Audio.Recorder.PhraseEventArgs mEnd = new Obi.Events.Audio.Recorder.PhraseEventArgs(1);
                    FinishPhrase(this, mEnd);
                    timer1.Enabled = false;
                }
                if (Audio.AudioRecorder.Instance.State.Equals(Audio.AudioRecorderState.Idle))
                {
                    Obi.Events.Audio.Recorder.PhraseEventArgs mStart = new Obi.Events.Audio.Recorder.PhraseEventArgs(BeginTime);
                    StartPhrase(this, mStart);
                    Audio.AudioRecorder.Instance.StartRecording(mPhraseMarkerAsset);
                    CurrentTime = Audio.AudioRecorder.Instance.TimeOfAsset;
                    Obi.Events.Audio.Recorder.PhraseEventArgs mContinue = new Obi.Events.Audio.Recorder.PhraseEventArgs(CurrentTime);
                    ContinuePhrase(this, mContinue);
                    timer1.Enabled = true;
                    mRecordButton.Enabled = false;
                }
            }
        }
    }
