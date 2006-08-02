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

using VirtualAudioBackend;
using VirtualAudioBackend.events.AudioRecorderEvents;



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
        private AssetManager mAssManager;       // the asset manager (for creating new assets)
        private VuMeter ob_VuMeter = new VuMeter();

        private List<AudioMediaAsset> mAssets;  // the list of assets created while recording

        /// <summary>
        /// The list of assets created.
        /// </summary>
        public List<AudioMediaAsset> Assets
        {
            get
            {
                return mAssets;
            }
        }

        public Record(int channels, int sampleRate, int bitDepth, AssetManager assManager)
        {
            InitializeComponent();
            mChannels = channels;
            mSampleRate = sampleRate;
            mBitDepth = bitDepth;
            mAssManager = assManager;
            mAssets = new List<AudioMediaAsset>();
            AudioRecorder.Instance.StateChanged += new StateChangedHandler(AudioRecorder_StateChanged);
            AudioRecorder.Instance.UpdateVuMeterFromRecorder += new UpdateVuMeterFromRecorderHandler(AudioRecorder_UpdateVuMeter);
        }
        private void AudioRecorder_StateChanged(object sender, StateChanged state)
        {
        }
        private void AudioRecorder_UpdateVuMeter(Object sender, UpdateVuMeterFromRecorder update)
        {
        }
        private void Record_Load(object sender, EventArgs e)
        {
            ArrayList arDevices = new ArrayList();
            arDevices = AudioRecorder.Instance.GetInputDevices();
            //AudioRecorder.Instance.InitDirectSound(2);
//AudioRecorder.Instance.InitDirectSound(mIndex);

                ob_VuMeter.ScaleFactor = 2;
                ob_VuMeter.SampleTimeLength = 2000;
                ob_VuMeter.UpperThreshold = 150;
                ob_VuMeter.LowerThreshold = 100;
                AudioRecorder.Instance.VuMeterObject = ob_VuMeter;
                ob_VuMeter.ShowForm();
                AudioMediaAsset mAudioAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
                mAssManager.AddAsset(mAudioAsset);    
            AudioRecorder.Instance.StartListening(mAudioAsset);
                timer1.Enabled = true;
            }


        private void btnRecordAndPause_Click(object sender, EventArgs e)
        {
            // AudioRecorder.Instance.InitDirectSound(mIndex);
            AudioMediaAsset mRecordAsset = new AudioMediaAsset(mChannels, mBitDepth, mSampleRate);
            mAssManager.AddAsset(mRecordAsset);
            if (AudioRecorder.Instance.State == AudioRecorderState.Idle && btnRecordAndPause.Text == "Record")
            {
                timer1.Enabled = true;
                btnRecordAndPause.Text ="&Pause";
                AudioRecorder.Instance.StartRecording(mRecordAsset);
}
            if(AudioRecorder.Instance.State == AudioRecorderState.Recording && btnRecordAndPause.Text == "Pause")
            {
                AudioRecorder.Instance.StopRecording();
                btnRecordAndPause.Text ="&Record";
                timer1.Enabled = false;
            }
        }

private void btnStop_Click(object sender, EventArgs e)
        {
    if(AudioRecorder.Instance.State == AudioRecorderState.Recording || AudioRecorder.Instance.State == AudioRecorderState.Listening)
                AudioRecorder.Instance.StopRecording();
                AudioRecorder.Instance.VuMeterObject.CloseVuMeterForm();
                timer1.Enabled = false;
                this.Close();
            }

        

        

        private void timer1_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = AudioRecorder.Instance.CurrentTime;
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