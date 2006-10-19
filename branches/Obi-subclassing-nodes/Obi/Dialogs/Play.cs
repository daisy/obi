using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

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
            Audio.AudioPlayer.Instance.StateChanged += new Events.Audio.Player.StateChangedHandler(AudioPlayer_StateChanged);
            Audio.AudioPlayer.Instance.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            Audio.AudioPlayer.Instance.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
        }

        private void AudioPlayer_StateChanged(object sender, Events.Audio.Player.StateChangedEventArgs e)
        {
        }

        private void AudioPlayer_EndOfAudioAsset(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
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

        private void AudioPlayer_UpdateVuMeter(object sender, Events.Audio.Player.UpdateVuMeterEventArgs e)
        {
        }

        private void mStopButton_Click(object sender, EventArgs e)
        {
            // closing stops playing (JQ)
            Close();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State.Equals(Audio.AudioPlayerState.Playing))
            {
                Audio.AudioPlayer.Instance.Pause();
                mPlayButton.Text = Localizer.Message("play");
                tmUpdateCurrentTime.Enabled = false;
            }
            else if (Audio.AudioPlayer.Instance.State.Equals(Audio.AudioPlayerState.Paused))
            {
                Audio.AudioPlayer.Instance.Resume();
                mPlayButton.Text = Localizer.Message("pause");
                tmUpdateCurrentTime.Enabled = true;
            }
        }

        private void tmUpdateCurrentTime_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = Audio.AudioPlayer.Instance.CurrentTimePosition;
            int Seconds = Convert.ToInt32 (dMiliSeconds / 1000);
            int DisplaySeconds = Seconds;
            if (DisplaySeconds > 59)
                DisplaySeconds = DisplaySeconds  - ( 60 * ( DisplaySeconds / 60 ));

            string sSeconds = DisplaySeconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            int DisplayMinutes = Minutes;
            if (DisplayMinutes > 59 )
                DisplayMinutes = DisplayMinutes  - ( 60 * ( DisplayMinutes / 60 ) );

            string sMinutes = DisplayMinutes.ToString("00");
            int Hours = Minutes / 60;
            int DisplayHours = Hours;
            if (DisplayHours > 23)
                DisplayHours = DisplayHours  - 24;

            string sHours = DisplayHours.ToString("00");
            mTimeDisplay.Text = sHours + ":" + sMinutes + ":" + sSeconds;
        }

        private void Play_Load(object sender, EventArgs e)
        {
            mNameDisplay.Text = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannelName)).getText();
            mNameDisplay.SelectionStart = mNameDisplay.Text.Length;
            mTimeDisplay.Text = "00:00:00";
            if (Audio.AudioPlayer.Instance.State.Equals(Audio.AudioPlayerState.Stopped))
            {
                // Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
                // ob_VuMeter.ScaleFactor = 2;
                // ob_VuMeter.LowerThreshold = 70;
                // ob_VuMeter.UpperThreshold = 150;
                // ob_VuMeter.SampleTimeLength = 1000;
                // ob_VuMeter.ShowForm();
                // Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
                Audio.AudioPlayer.Instance.VuMeterObject = null;
                Audio.AudioPlayer.Instance.Play(Project.GetAudioMediaAsset(mNode));
                mPlayButton.Text = Localizer.Message("pause");
                tmUpdateCurrentTime.Enabled = true;
            }
        }

        private void Play_FormClosing(object sender, FormClosingEventArgs e)
        {
            Audio.AudioPlayer.Instance.Stop();
            // Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
        }
    }
}