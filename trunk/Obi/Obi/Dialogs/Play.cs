using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VirtualAudioBackend;

namespace Obi.Dialogs
{
    public partial class Play : Form
    {
        public Play()
        {
            InitializeComponent();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            ob_play.Stop();
            this.Close();
        }
        
        AudioClip Clip;
        AudioMediaAsset ob_AudioMedia;
        AudioPlayer ob_play =new AudioPlayer();

        private void btnPlay_Click(object sender, EventArgs e)
        {
 
            if (ob_play.State.Equals(AudioPlayerState.NotReady))	
//            if (i == 1)
            {
                
            Clip = new AudioClip("c:\\atest\\a\\a1.wav");
            ArrayList al = new ArrayList();
            al.Add(Clip);
            ob_AudioMedia = new AudioMediaAsset(al);

            
                VuMeter ob_VuMeter = new VuMeter();
                ob_VuMeter.ScaleFactor = 2;
                ob_VuMeter.LowerThreshold = 70;
                ob_VuMeter.UpperThreshold = 150;
                ob_VuMeter.SampleTimeLength = 1000;
                ob_VuMeter.ShowForm();
                ob_play.VuMeterObject = ob_VuMeter;
                ob_play.GetOutputDevices();
                
                ob_play.SetDevice(this , 0);
            ob_play.Play(ob_AudioMedia);
        btnPlay.Text = "&Pause";
        tmUpdateCurrentTime.Enabled = true;
            }
            else if (ob_play.State.Equals (AudioPlayerState.Playing))
            {
                ob_play.Pause();
                btnPlay.Text = "&Play";
                tmUpdateCurrentTime.Enabled = false;
            }
            else if (ob_play.State.Equals(AudioPlayerState.Paused))
            {
                ob_play.Resume();
                btnPlay.Text = "&Pause";
                tmUpdateCurrentTime.Enabled = true;
            }

        }

        private void tmUpdateCurrentTime_Tick(object sender, EventArgs e)
        {
            txtDisplayTime.Text = ob_play.CurrentTimePosition.ToString(); 
        }

        private void Play_Load(object sender, EventArgs e)
        {
            MessageBox.Show("Openning Play Diaglog"); 
            //txtDisplayAsset.Text = 
        }
            


    
    }
}