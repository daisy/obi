using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VirtualAudioBackend;
using VirtualAudioBackend.events.AudioPlayerEvents;

using urakawa.core;
using urakawa.media;

namespace Obi.Dialogs
{
    public partial class Play : Form
    {
        private CoreNode mNode;  // the node whose asset we want to play

        public Play(CoreNode node)
        {
            InitializeComponent();
            mNode = node;
            AudioPlayer.Instance.StateChanged += new StateChangedHandler(AudioPlayer_StateChanged);
            AudioPlayer.Instance.EndOfAudioAsset += new EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            AudioPlayer.Instance.EndOfAudioBuffer += new EndOfAudioBufferHandler(AudioPlayer_EndOfAudioBuffer);
            AudioPlayer.Instance.UpdateVuMeter += new UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
        }

        private void AudioPlayer_StateChanged(object sender, StateChanged e)
        {
        }

        private void AudioPlayer_EndOfAudioAsset(object sender, EndOfAudioAsset e)
        {
            AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            Close();
        }

        // Convoluted way to close necessary for debugging (JQ)
        private delegate void CloseCallback();
        public new void Close()
        {
            if (InvokeRequired)
            {
                Invoke(new CloseCallback(Close));
            }
            else
            {
                base.Close();
            }
        }

        private void AudioPlayer_EndOfAudioBuffer(object sender, EndOfAudioBuffer e)
        {
        }

        private void AudioPlayer_UpdateVuMeter(object sender, UpdateVuMeter e)
        {
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            if (AudioPlayer.Instance.State.Equals(AudioPlayerState.Playing))
            {
                AudioPlayer.Instance.Pause();
                mPlayButton.Text = Localizer.Message("play");
                tmUpdateCurrentTime.Enabled = false;
            }
            else if (AudioPlayer.Instance.State.Equals(AudioPlayerState.Paused))
            {
                AudioPlayer.Instance.Resume();
                mPlayButton.Text = Localizer.Message("pause");
                tmUpdateCurrentTime.Enabled = true;
            }

        }

        private void tmUpdateCurrentTime_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = AudioPlayer.Instance.CurrentTimePosition;
            int Seconds = Convert.ToInt32 (dMiliSeconds / 1000);
            string sSeconds = Seconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes = Minutes.ToString("00");
            int Hours = Minutes / 60;
            string sHours = Hours.ToString("00");
            mTimeDisplay.Text = sHours + ":" + sMinutes + ":" + sSeconds;
        }

        private void Play_Load(object sender, EventArgs e)
        {
            mNameDisplay.Text = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannel)).getText();
            mTimeDisplay.Text = "00:00:00";
            if (AudioPlayer.Instance.State.Equals(AudioPlayerState.Stopped))
            {
                VuMeter ob_VuMeter = new VuMeter();
                ob_VuMeter.ScaleFactor = 2;
                ob_VuMeter.LowerThreshold = 70;
                ob_VuMeter.UpperThreshold = 150;
                ob_VuMeter.SampleTimeLength = 1000;
                ob_VuMeter.ShowForm();
                AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
                AudioPlayer.Instance.Play(Project.GetAudioMediaAsset(mNode));
                mPlayButton.Text = Localizer.Message("pause");
                tmUpdateCurrentTime.Enabled = true;
            }
        }

        private void Play_FormClosing(object sender, FormClosingEventArgs e)
        {
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
        }
    }
}