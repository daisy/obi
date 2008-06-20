using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class TransportBar : UserControl
    {
        private Audio.Player player;

        public TransportBar()
        {
            InitializeComponent();
            this.player = null;
            this.timeDisplayUpdateTimer.Stop();
        }

        public Audio.Player Player { get { return this.player; } }


        private void pauseButton_Click(object sender, EventArgs e)
        {
            if (Parent is BobiForm) ((BobiForm)Parent).Pause();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            if (Parent is BobiForm) ((BobiForm)Parent).PlayOrResume();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (Parent is BobiForm) ((BobiForm)Parent).Stop();
        }

        private void TransportBar_ParentChanged(object sender, EventArgs e)
        {
            if (this.player == null)
            {
                if (Parent != null) this.player = new Audio.Player(Parent);
                this.player.StateChanged += new Bobi.Audio.StateChangedEventHandler(player_StateChanged);
            }
        }

        private void player_StateChanged(object sender, Bobi.Audio.StateChangedEventArgs e)
        {
            if (e.Player.State == Bobi.Audio.PlayerState.Playing)
            {
                this.timeDisplayUpdateTimer.Start();
            }
            else
            {
                this.timeDisplayUpdateTimer.Stop();
            }
        }

        private void timeDisplayUpdateTimer_Tick(object sender, EventArgs e)
        {
            timeDisplay.Text = string.Format("{0:0.000}s", this.player.CurrentTimePosition / 1000.0);
            ((BobiForm)Parent).ReportPlaybackPosition(this.player.CurrentTimePosition);
        }
    }
}
