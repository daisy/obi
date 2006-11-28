using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.Dialogs
{
    public partial class TransportPlay : Form
    {
        private Playlist mPlaylist;
        

        public TransportPlay()
        {
            InitializeComponent();
        }

        public TransportPlay(Playlist playlist)
        {
            InitializeComponent();
            mPlaylist = playlist;
            mPlaylist.StateChanged += new Obi.Events.Audio.Player.StateChangedHandler
            (
                delegate(object sender, Obi.Events.Audio.Player.StateChangedEventArgs e) { PlayerStateChanged(); }
            );
            mPlaylist.EndOfPlaylist += new Playlist.EndOfPlaylistHandler
            (
                delegate(object sender, EventArgs e) { PlayerStopped(); }
            );
            mPlaylist.MovedToPhrase += new Playlist.MovedToPhraseHandler(MovedToPhrase);
            mPlaylist.Play();

           mPlaylist.Audioplayer.VuMeter.PeakOverload += new Events.Audio.VuMeter.PeakOverloadHandler(CatchPeakOverloadEvent);
        }

        delegate void PlayerStateChangedCallback();

        private void PlayerStateChanged()
        {
            if (InvokeRequired)
            {
                Invoke(new PlayerStateChangedCallback(PlayerStateChanged));
            }
            else
            {
                if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Stopped)
                {
                    PlayerStopped();
                }
                else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Paused)
                {
                    mPauseButton.Visible = false;
                    mPlayButton.Visible = true;
                    mStopButton.Visible = true;
                    mCloseButton.Visible = false;
                }
                else if (mPlaylist.Audioplayer.State == Obi.Audio.AudioPlayerState.Playing)
                {
                    mPauseButton.Visible = true;
                    mPlayButton.Visible = false;
                    mStopButton.Visible = true;
                    mCloseButton.Visible = false;
                }
            }
        }

        private void PlayerStopped()
        {
            mPauseButton.Visible = false;
            mPlayButton.Visible = true;
            mStopButton.Visible = false;
            mCloseButton.Visible = true;
        }

        private void MovedToPhrase(object sender, Events.Node.NodeEventArgs e)
        {
            System.Diagnostics.Debug.Print(">>> Moved to phrase {0}", Project.GetAudioMediaAsset(e.Node).Name);
        }

        /// <summary>
        /// Stops playback and close the dialog.
        /// </summary>
        private void mStopButton_Click(object sender, EventArgs e)
        {
            tmUpdateAmplitudeText.Enabled = false;
            mPlaylist.Stop();
        }

        private void mPauseButton_Click(object sender, EventArgs e)
        {
            tmUpdateAmplitudeText.Enabled = false;
            mPlaylist.Pause();
        }

        private void mPlayButton_Click(object sender, EventArgs e)
        {
            mPlaylist.Play();
            tmUpdateAmplitudeText.Enabled = true ;
        }

        private void TransportPlay_FormClosing(object sender, FormClosingEventArgs e)
        {
            mPlaylist.Stop();
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
            mPlaylist.NavigateNextPhrase();
        }

        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            mPlaylist.NavigatePreviousPhrase();
        }

        private void btnRewind_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(mPlaylist.CurrentTime.ToString());
            mPlaylist.CurrentTime = mPlaylist.CurrentTime - 10000;
        }

        private void btnForward_Click(object sender, EventArgs e)
        {
//            MessageBox.Show(mPlaylist.CurrentTime.ToString());
            mPlaylist.CurrentTime = mPlaylist.CurrentTime  + 10000;
        }

        // catch the peak overload event triggered by VuMeter
        public void CatchPeakOverloadEvent(object sender, Events.Audio.VuMeter.PeakOverloadEventArgs ob_PeakOverload)
        {
            Audio.VuMeter ob_VuMeter = sender as Audio.VuMeter;
            if (ob_PeakOverload.Channel == 1)
            {
                				SetTextBoxText(txtOverloadLeft, ob_VuMeter.m_MeanValueLeft.ToString());  
            }

            if (ob_PeakOverload.Channel == 2)
            {
                SetTextBoxText(txtOverloadRight, ob_VuMeter.m_MeanValueRight.ToString());
            }
        }

        private delegate void SetTextBoxTextCallback(TextBox box, string text);

        private void SetTextBoxText(TextBox box, string text)
        {
            if (InvokeRequired)
            {
                Invoke(new SetTextBoxTextCallback(SetTextBoxText), new object[] { box, text });
            }
            else
            {
                box.Text = text;
            }
        }

        private void tmUpdateAmplitudeText_Tick(object sender, EventArgs e)
        {
            txtAmplitudeLeft.Text = mPlaylist.Audioplayer.VuMeter.m_MeanValueLeft.ToString () ;
            txtAmplitudeRight.Text = mPlaylist.Audioplayer.VuMeter.m_MeanValueRight.ToString();
        }


    }
}