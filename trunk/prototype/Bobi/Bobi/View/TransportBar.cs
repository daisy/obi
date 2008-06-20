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
        }

        public Audio.Player Player { get { return this.player; } }


        private void playButton_Click(object sender, EventArgs e)
        {
            if (Parent is BobiForm) ((BobiForm)Parent).Play();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            if (Parent is BobiForm) ((BobiForm)Parent).Stop();
        }

        private void TransportBar_ParentChanged(object sender, EventArgs e)
        {
            if (Parent != null) this.player = new Audio.Player(Parent);
        }
    }
}
