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
            ArrayList arDevices = new ArrayList();
            arDevices = Audio.AudioRecorder.Instance.GetInputDevices();
            //AudioRecorder.Instance.InitDirectSound(2);
//AudioRecorder.Instance.InitDirectSound(mIndex);

                ob_VuMeter.ScaleFactor = 2;
                ob_VuMeter.SampleTimeLength = 2000;
                ob_VuMeter.UpperThreshold = 150;
                ob_VuMeter.LowerThreshold = 100;
                Audio.AudioRecorder.Instance.VuMeterObject = ob_VuMeter;
                ob_VuMeter.ShowForm();
                Assets.AudioMediaAsset mAudioAsset = new Assets.AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mAssManager.AddAsset(mAudioAsset);    
            Audio.AudioRecorder.Instance.StartListening(mAudioAsset);
                timer1.Enabled = true;
            }


        private void btnRecordAndPause_Click(object sender, EventArgs e)
        {
            // AudioRecorder.Instance.InitDirectSound(mIndex);
            Assets.AudioMediaAsset mRecordAsset = new Assets.AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            mAssManager.AddAsset(mRecordAsset);
            if (Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Idle && btnRecordAndPause.Text == "Record")
            {
                timer1.Enabled = true;
                btnRecordAndPause.Text ="&Pause";
                Audio.AudioRecorder.Instance.StartRecording(mRecordAsset);
}
            if(Audio.AudioRecorder.Instance.State == Audio.AudioRecorderState.Recording && btnRecordAndPause.Text == "Pause")
            {
                Audio.AudioRecorder.Instance.StopRecording();
                btnRecordAndPause.Text ="&Record";
                timer1.Enabled = false;
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
            double dMiliSeconds = Audio.AudioRecorder.Instance.CurrentTime;
            int Seconds = Convert.ToInt32(dMiliSeconds / 1000);
            string sSeconds;
            if (Seconds > 9)
                sSeconds = Seconds.ToString();
            else
                sSeconds = "0" + Seconds.ToString();

            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes;
            if (Minutes > 9)
                sMinutes = Minutes.ToString();
            else
                sMinutes = "0" + Minutes.ToString();


            int Hours = Minutes / 60;
            string sHours;
            if (Hours > 9)
                sHours = Hours.ToString();
            else
                sHours = "0" + Hours.ToString();



            string sDisplayTime = sHours + ":" + sMinutes + ":" + sSeconds;
            txtDisplayTime.Text = sDisplayTime;            
                
        }
    }
}