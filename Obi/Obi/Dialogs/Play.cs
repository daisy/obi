using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VirtualAudioBackend;

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
        }

        private void btnStop_Click(object sender, EventArgs e)
        {

            AudioPlayer.Instance.Stop();
            this.Close();
        }

        //AudioPlayer ob_play = AudioPlayer.Instance;

        private void btnPlay_Click(object sender, EventArgs e)
        {


            if (AudioPlayer.Instance.State.Equals(AudioPlayerState.Playing))
            {
                AudioPlayer.Instance.Pause();
                btnPlay.Text = "&Play";
                tmUpdateCurrentTime.Enabled = false;
            }
            else if (AudioPlayer.Instance.State.Equals(AudioPlayerState.Paused))
            {
                AudioPlayer.Instance.Resume();
                btnPlay.Text = "&Pause";
                tmUpdateCurrentTime.Enabled = true;
            }

        }

        private void tmUpdateCurrentTime_Tick(object sender, EventArgs e)
        {
            double dMiliSeconds = AudioPlayer.Instance.CurrentTimePosition; 
            double dSeconds = dMiliSeconds / 1000;
            int Minutes = Convert.ToInt32(dSeconds / 60);
            int Hours = Minutes / 60;
            string sDisplayTime = Hours.ToString () +":" + Minutes.ToString () + ":" + dSeconds.ToString()    ;
            txtDisplayTime.Text = sDisplayTime;
        }

        private void Play_Load(object sender, EventArgs e)
        {
            
            txtDisplayAsset.Text = ((TextMedia)Project.GetMediaForChannel(mNode, Project.AnnotationChannel)).getText();

            if (AudioPlayer.Instance.State.Equals(AudioPlayerState.NotReady))

            {



                VuMeter ob_VuMeter = new VuMeter();
                ob_VuMeter.ScaleFactor = 2;
                ob_VuMeter.LowerThreshold = 70;
                ob_VuMeter.UpperThreshold = 150;
                ob_VuMeter.SampleTimeLength = 1000;
                ob_VuMeter.ShowForm();
                AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
//                AudioPlayer.Instance.GetOutputDevices();

                
                //AudioPlayer.Instance.SetDevice(this, 0);
                AudioPlayer.Instance.Play(Project.GetAudioMediaAsset(mNode));
                btnPlay.Text = "&Pause";
                tmUpdateCurrentTime.Enabled = true;
            }

        }
            


    
    }
}