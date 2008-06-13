using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Bobi.View
{
    public partial class AudioBlock : UserControl
    {
        private AudioNode node;

        public AudioBlock()
        {
            InitializeComponent();
        }

        public AudioBlock(AudioNode node): base()
        {
            this.node = node;
        }

        public AudioNode Node { get { return this.node; } }
    }
}
