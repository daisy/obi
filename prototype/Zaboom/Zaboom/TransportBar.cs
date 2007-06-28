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
using urakawa.properties.channel;

namespace Zaboom
{
    public partial class TransportBar : UserControl
    {
        private Audio.Player player;
        private List<AudioMediaData> playlist;
        private int nowPlaying;

        public TransportBar()
        {
            InitializeComponent();
            player = new Audio.Player();
            player.SetOutputDevice(this);
        }

        private ProjectPanel ProjectPanel { get { return Parent as ProjectPanel; } }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (ProjectPanel != null && ProjectPanel.Project != null)
            {
                playlist = new List<AudioMediaData>();
                Channel audioch = ProjectPanel.Project.AudioChannel;
                ProjectPanel.Project.getPresentation().getRootNode().acceptDepthFirst(
                    delegate(urakawa.core.TreeNode node)
                    {
                        ChannelsProperty chprop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
                        if (chprop != null)
                        {
                            ManagedAudioMedia media = chprop.getMedia(audioch) as ManagedAudioMedia;
                            if (media != null) playlist.Add(media.getMediaData());
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode node) { }
                );
                System.Diagnostics.Debug.Print("# of media to play: {0}", playlist.Count);
                if (playlist.Count > 0)
                {
                    player.EndOfAudioAsset += new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
                    nowPlaying = -1;
                    PlayNext();
                }
            }
        }

        private bool PlayNext()
        {
            if (++nowPlaying < playlist.Count)
            {
                player.Stop();
                player.CurrentMedia = playlist[nowPlaying];
                player.Play();
                return true;
            }
            else
            {
                return false;
            }
        }         

        private void player_EndOfAudioAsset(object sender, EventArgs e)
        {
            if (!PlayNext())
            {
                player.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
                player.Stop();
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            player.EndOfAudioAsset -= new Audio.EndOfAudioAssetHandler(player_EndOfAudioAsset);
            player.Stop();
        }
    }
}
