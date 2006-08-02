using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using urakawa.core;
using VirtualAudioBackend;
using VirtualAudioBackend.events.AudioPlayerEvents;

namespace Obi.Dialogs
{
    public partial class Split : Form
    {
        private CoreNode mNode;  // the node to split

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
            AudioPlayer.Instance.StateChanged += new StateChangedHandler(AudioPlayer_StateChanged);
                        AudioPlayer.Instance.EndOfAudioAsset += new EndOfAudioAssetHandler(AudioPlayer_EndOfAudioAsset);
            AudioPlayer.Instance.EndOfAudioBuffer += new EndOfAudioBufferHandler(AudioPlayer_EndOfAudioBuffer);
            AudioPlayer.Instance.UpdateVuMeter += new UpdateVuMeterHandler(AudioPlayer_UpdateVuMeter);
            tmUpdateTimePosition.Enabled = true;
        }
        
        AudioMediaAsset ob_AudioAsset;
        double m_dSplitTime;
        int m_Step=10000;
        int m_FineStep = 2000;

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (ob_AudioAsset.AudioLengthInBytes > m_dSplitTime)
            {
                VuMeter ob_VuMeter = new VuMeter();
                ob_VuMeter.LowerThreshold = 50;
                ob_VuMeter.UpperThreshold = 300;
                ob_VuMeter.SampleTimeLength = 1000;
                AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
                AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, m_dSplitTime + 4000));
            }
        }

        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {
            txtDisplayTime.Text = AudioPlayer.Instance.CurrentTimePosition.ToString();
        }

        private void btnFastRewind_Click(object sender, EventArgs e)
        {
                    m_dSplitTime = m_dSplitTime - m_Step;
                    txtDisplayTime.Text = m_dSplitTime.ToString();

        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime + m_Step;
            txtDisplayTime.Text = m_dSplitTime.ToString();

        }

        private void btnFineRewind_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime - m_FineStep;
            txtDisplayTime.Text = m_dSplitTime.ToString();

        }

        private void btnFineForward_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime + m_FineStep;
            txtDisplayTime.Text = m_dSplitTime.ToString();
            //double dTempPosition = AudioPlayer.Instance.CurrentTimePosition;
            //AudioPlayer.Instance.CurrentTimePosition= dTempPosition +m_FineStep;
        }

        private void btnSplit_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            AudioPlayer.Instance.Stop();
            AudioPlayer.Instance.VuMeterObject.CloseVuMeterForm();
            this.Close();
        }

        private void Split_Load(object sender, EventArgs e)
        {
            
            VuMeter ob_VuMeter = new VuMeter();
            ob_VuMeter.LowerThreshold = 50;
            ob_VuMeter.UpperThreshold = 300;
            ob_VuMeter.SampleTimeLength = 1000;
            AudioPlayer.Instance.VuMeterObject = ob_VuMeter;
            AudioPlayer.Instance.Play(ob_AudioAsset);
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            m_dSplitTime = AudioPlayer.Instance.CurrentTimePosition;
            AudioPlayer.Instance.Stop();
            tmUpdateTimePosition.Enabled = false;
            txtDisplayTime.Text = m_dSplitTime.ToString();
        }

        
            

            



        private void AudioPlayer_StateChanged(object sender, StateChanged e)
        {
        }

        private void AudioPlayer_EndOfAudioAsset(object sender, EndOfAudioAsset e)
        {
            

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

        private void AudioPlayer_EndOfAudioBuffer(object sender, EndOfAudioBuffer e)
        {
        }

        private void AudioPlayer_UpdateVuMeter(object sender, UpdateVuMeter e)
        {
        }

        private void Split_FormClosing(object sender, FormClosingEventArgs e)
        {
        }

    }// end of class
}