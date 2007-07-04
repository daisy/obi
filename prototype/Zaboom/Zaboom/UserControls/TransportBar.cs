using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.property.channel;

namespace Zaboom.UserControls
{
    public partial class TransportBar : UserControl
    {
        private Audio.Player player;            // audio player for this transport bar
        private List<AudioMediaData> playlist;  // list of audio phrases to play
        private int nowPlaying;                 // index in the playlist of the phrase currently playing

        /// <summary>
        /// Create a new transport bar.
        /// </summary>
        public TransportBar()
        {
            InitializeComponent();
            player = new Audio.Player();
            player.StateChanged += new Audio.StateChangedHandler(player_StateChanged);
            ShowPlayerStatus();
            playlist = new List<AudioMediaData>();
        }

        /// <summary>
        /// Get the parent as a project panel.
        /// </summary>
        private ProjectPanel ProjectPanel { get { return Parent as ProjectPanel; } }

        /// <summary>
        /// Update the playlist with the selection.
        /// </summary>
        private void ProjectPanel_SelectionChanged(ProjectPanel sender, EventArgs e)
        {
            playlist.Clear();
            foreach (Selectable s in ProjectPanel.Selected)
            {
                if (s.Node != null)
                {
                    ChannelsProperty chprop = (ChannelsProperty)s.Node.getProperty(typeof(ChannelsProperty));
                    if (chprop != null)
                    {
                        ManagedAudioMedia media = chprop.getMedia(ProjectPanel.Project.AudioChannel) as ManagedAudioMedia;
                        if (media != null) playlist.Add(media.getMediaData());
                    }
                }
            }
        }

        /// <summary>
        /// Play the currently selected phrase(s).
        /// </summary>
        private void playButton_Click(object sender, EventArgs e)
        {
                if (playlist.Count > 0)
                {
                    player.EndOfAudioAsset += new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
                    nowPlaying = 0;
                    Play();
                }
        }

        private bool Play()
        {
            if (nowPlaying < playlist.Count)
            {
                player.Stop();
                player.Play(playlist[nowPlaying]);
                return true;
            }
            else
            {
                return false;
            }
        }         

        /// <summary>
        /// Move to the next asset in the playlist or stop if at the end.
        /// </summary>
        private void player_EndOfAudioAsset(Audio.Player player, EventArgs e)
        {
            ++nowPlaying;
            if (!Play())
            {
                player.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
                player.Stop();
            }
        }

        private void player_StateChanged(Audio.Player player, Audio.StateChangedEventArgs e)
        {
            ShowPlayerStatus();

        }

        private delegate void VoidCallback();

        private void ShowPlayerStatus()
        {
            if (statusLabel.InvokeRequired)
            {
                Invoke(new VoidCallback(ShowPlayerStatus));
            }
            else
            {
                int w = statusLabel.Width;
                statusLabel.Text = player.State.ToString();
                statusLabel.Location = new Point(statusLabel.Location.X - statusLabel.Width + w, statusLabel.Location.Y);
            }
        }

        /// <summary>
        /// Stop playback.
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            player.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
            player.Stop();
        }

        /// <summary>
        /// Set the audio device and listen to selection events from the parent project panel.
        /// </summary>
        private void TransportBar_Load(object sender, EventArgs e)
        {
            ProjectPanel.SelectionChanged += new SelectionChangedHandler(ProjectPanel_SelectionChanged);
            player.SetDevice(ParentForm, "");
        }
    }
}