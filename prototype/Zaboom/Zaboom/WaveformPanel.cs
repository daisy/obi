using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.properties.channel;

namespace Zaboom
{
    public partial class WaveformPanel : UserControl
    {
        private Project project;
        private urakawa.core.TreeNode node;

        public WaveformPanel()
        {
            InitializeComponent();
        }

        public WaveformPanel(Project project, urakawa.core.TreeNode node)
        {
            InitializeComponent();
            this.project = project;
            this.node = node;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.DrawLine(Pens.Red, new Point(0, Height / 2), new Point(Width, Height / 2));
        }

        private ProjectPanel Panel { get { return (ProjectPanel)Parent.Parent; } }

        private void WaveformPanel_Click(object sender, EventArgs e)
        {
            ChannelsProperty channelsProp = (ChannelsProperty)
                node.getProperty(typeof(urakawa.properties.channel.ChannelsProperty));
            Channel audioChannel = project.GetSingleChannelByName(Project.AUDIO_CHANNEL_NAME);
            ManagedAudioMedia media = (ManagedAudioMedia)channelsProp.getMedia(audioChannel);
            Panel.Player.CurrentMedia = media.getMediaData();
            Panel.Player.Play();
        }
    }
}
