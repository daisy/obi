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
    public partial class Split : Form
    {
        public Split()
        {
            InitializeComponent();
            AudioClip ob_Clip = new AudioClip("c:\\atest\\a\\a1.wav");
        ArrayList al = new ArrayList();
         al.Add(ob_Clip);
            ob_AudioAsset = new AudioMediaAsset (al) ;
        }
        
        AudioMediaAsset ob_AudioAsset;
        double m_dSplitTime;
        int m_Step=10;
        int m_FineStep = 2;

        private void btnPreview_Click(object sender, EventArgs e)
        {
            if(ob_AudioAsset.AudioLengthInBytes  > m_dSplitTime)
            AudioPlayer.Instance.Play(ob_AudioAsset.GetChunk(m_dSplitTime, m_dSplitTime+5000));
        }

        private void tmUpdateTimePosition_Tick(object sender, EventArgs e)
        {

        }

        private void btnFastRewind_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime - m_Step;
            

        }

        private void btnFastForward_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime + m_Step;
            

        }

        private void btnFineRewind_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime - m_FineStep;
            

        }

        private void btnFineForward_Click(object sender, EventArgs e)
        {
            m_dSplitTime = m_dSplitTime + m_FineStep;
            //double dTempPosition = AudioPlayer.Instance.CurrentTimePosition;
            //AudioPlayer.Instance.CurrentTimePosition= dTempPosition +m_FineStep;
        }
    }
}