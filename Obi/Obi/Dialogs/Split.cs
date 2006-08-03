using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using urakawa.core;

namespace Obi.Dialogs
{
    public partial class Split : Form
    {
        private CoreNode mNode;  // the node to split
        private Assets.AudioMediaAsset mResultAsset;  // the new asset created by the split

        public Assets.AudioMediaAsset ResultAsset
        {
            get
            {
                return mResultAsset;
            }
        }

        public double SplitTime
        {
            get
            {
                return m_dSplitTime;
            }
        }

        public Split(CoreNode node, double splitTime)
        {
            InitializeComponent();
            mNode = node;
            m_dSplitTime = splitTime;
            ob_AudioAsset = Project.GetAudioMediaAsset(node);
            Audio.AudioPlayer.Instance.StateChanged += new Events.Audio.Player.StateChangedHandler(AudioPlayer_StateChanged);
            Audio.AudioPlayer.Instance.EndOfAudioAsset += new Events.Audio.Player.EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            Audio.AudioPlayer.Instance.EndOfAudioBuffer += new Events.Audio.Player.EndOfAudioBufferHandler(AudioPlayer_EndOfAudioBuffer);
            Audio.AudioPlayer.Instance.UpdateVuMeter += new Events.Audio.Player.UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
            tmUpdateTimePosition.Enabled = true;
        }
        
        //member variables
        Assets.AudioMediaAsset ob_AudioAsset;
        double m_dSplitTime;
        int m_Step=10000;
        int m_FineStep = 2000;
        

        

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (ob_AudioAsset.AudioLengthInBytes > m_dSplitTime &&
                Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped)
            {
                Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
                ob_VuMeter.LowerThreshold = 50;
                ob_VuMeter.UpperThreshold = 300;
                ob_VuMeter.SampleTimeLength = 1000;
                Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
                Audio.AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, m_dSplitTime + 4000));
            }
        }

        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {
            txtDisplayTime.Text = ChangeTimeToDisplay(Audio.AudioPlayer.Instance.CurrentTimePosition);
        }

        private void btnFastRewind_Click(object sender, EventArgs e)
        {
            
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition - m_Step > 0)
                Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition - m_Step;
            }
            else
            {
                m_dSplitTime = m_dSplitTime - m_Step;
                if (m_dSplitTime < 0)
                    m_dSplitTime = 0;

                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition + m_Step < ob_AudioAsset.LengthInMilliseconds)
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_Step;
            }
            else
            {
                m_dSplitTime = m_dSplitTime + m_Step;
                if (m_dSplitTime > ob_AudioAsset.LengthInMilliseconds)
                    m_dSplitTime = ob_AudioAsset.LengthInMilliseconds;

                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
        }

        private void btnFineRewind_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition - m_FineStep > 0)
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition - m_FineStep;
            }
            else
            {
                m_dSplitTime = m_dSplitTime - m_FineStep;
                if (m_dSplitTime < 0)
                    m_dSplitTime = 0;
                txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
        }

        private void btnFineForward_Click(object sender, EventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                double dCurrentPlayPosition = Audio.AudioPlayer.Instance.CurrentTimePosition;
                if (dCurrentPlayPosition + m_FineStep < ob_AudioAsset.LengthInMilliseconds)
                    Audio.AudioPlayer.Instance.CurrentTimePosition = dCurrentPlayPosition + m_FineStep;
            }
            else
            {
                m_dSplitTime = m_dSplitTime + m_FineStep;
                if (m_dSplitTime > ob_AudioAsset.LengthInMilliseconds)
                    m_dSplitTime = ob_AudioAsset.LengthInMilliseconds;

                txtDisplayTime.Text = m_dSplitTime.ToString(); txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
            }
            //double dTempPosition = AudioPlayer.Instance.CurrentTimePosition;
            //AudioPlayer.Instance.CurrentTimePosition= dTempPosition +m_FineStep;
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {
            // result of the split must be in mResultAsset
            if (m_dSplitTime > 0 && m_dSplitTime < ob_AudioAsset.LengthInMilliseconds)
            {
                mResultAsset = ob_AudioAsset.Split(m_dSplitTime) as Assets.AudioMediaAsset;
                ob_AudioAsset.Manager.AddAsset(mResultAsset);
            }
            
                
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Audio.AudioPlayer.Instance.Stop();
            Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

        private void Split_Load(object sender, EventArgs e)
        {
            txtDisplayTime.Text = "00:00:00";
            Audio.VuMeter ob_VuMeter = new Audio.VuMeter();
            ob_VuMeter.LowerThreshold = 50;
            ob_VuMeter.UpperThreshold = 300;
            ob_VuMeter.SampleTimeLength = 1000;
            Audio.AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
            Audio.AudioPlayer.Instance.Play(ob_AudioAsset);
            btnPreview.Enabled = false;
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if(Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
            {
                m_dSplitTime = Audio.AudioPlayer.Instance.CurrentTimePosition;
                Audio.AudioPlayer.Instance.Stop();
            tmUpdateTimePosition.Enabled = false;
                btnPause.Text = "&Play";
                btnPreview.Enabled= true;
            }
            else if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Stopped)
            {
                //MessageBox.Show(m_dSplitTime.ToString());
                //AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, ob_AudioAsset.LengthInMilliseconds));
                Audio.AudioPlayer.Instance.Play( ob_AudioAsset , m_dSplitTime);
                //AudioPlayer.Instance.Resume();
                //AudioPlayer.Instance.CurrentTimePosition = m_dSplitTime;
                tmUpdateTimePosition.Enabled = true;
                btnPause.Text = "&Pause";
                btnPreview.Enabled = false;
            }

            //txtDisplayTime.Text = m_dSplitTime.ToString();
            txtDisplayTime.Text = ChangeTimeToDisplay(m_dSplitTime);
        }

        
            

            



        private void AudioPlayer_StateChanged(object sender, Events.Audio.Player.StateChangedEventArgs e)
        {
        }

        private void AudioPlayer_EndOfAudioAsset(object sender, Events.Audio.Player.EndOfAudioAssetEventArgs e)
        {
            tmUpdateTimePosition.Enabled = false;
            btnPause.Text = "&Play";
        }

        // Convoluted way to close necessary for debugging (JQ)
        private delegate void CloseCallback();
        public new void Close()
        {
            if (InvokeRequired)
            {
                Invoke(new CloseCallback(Close));
            }
            else
            {
                base.Close();
            }
        }

        private void AudioPlayer_EndOfAudioBuffer(object sender, Events.Audio.Player.EndOfAudioBufferEventArgs e)
        {
        }

        private void AudioPlayer_UpdateVuMeter(object sender, Events.Audio.Player.UpdateVuMeterEventArgs e)
        {
        }

        private void Split_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Audio.AudioPlayer.Instance.State == Audio.AudioPlayerState.Playing)
                Audio.AudioPlayer.Instance.Stop();
            Audio.AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
        }

        string ChangeTimeToDisplay(double dTime)
        {
            double dMiliSeconds = dTime;
            int Seconds = Convert.ToInt32 (dMiliSeconds / 1000);
            string sSeconds = Seconds.ToString("00");
            int Minutes = Convert.ToInt32(Seconds / 60);
            string sMinutes = Minutes.ToString("00");
            int Hours = Minutes / 60;
            string sHours = Hours.ToString("00");
            return sHours + ":" + sMinutes + ":" + sSeconds;
        
        }

    }// end of class
}