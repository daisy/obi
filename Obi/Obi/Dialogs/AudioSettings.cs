﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioSettings : Form
    {
        private AudioSettings()
        {
            InitializeComponent();
            
        }

        public AudioSettings(int audioChannels, int audioSampleRate)
            : this()
        {
            for (int i = 0; i < mcbSampleRate.Items.Count; i++ )
            {
                
                if ((Convert.ToInt32 (mcbSampleRate.Items[i])) == audioSampleRate) mcbSampleRate.SelectedIndex = i;
            }
            mcbAudioChannel.SelectedIndex = AudioChannels== 1? 0:
                AudioChannels == 2? 1: 0;

        }

        
        public int AudioChannels 
        { 
            get 
            {
                return mcbAudioChannel.SelectedIndex + 1;
                }
        }

        public int AudioSampleRate
        {
            get
            {
                return mcbSampleRate.SelectedIndex == 0 ? 11025 :
                    mcbSampleRate.SelectedIndex == 1 ? 22050 :
                    mcbSampleRate.SelectedIndex == 2 ? 44100 :
                    48000;
            }
        }

        private void mbtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mbtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
