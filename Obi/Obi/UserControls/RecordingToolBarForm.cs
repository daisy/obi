using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace Obi.UserControls
{
    public partial class RecordingToolBarForm : Form
    {
        ProjectView.TransportBar m_TransportBar;
      
        public RecordingToolBarForm()
        {
            InitializeComponent();
        }

        public RecordingToolBarForm(ProjectView.TransportBar transportBar):this  ()
        {
            m_TransportBar = transportBar;
        }

        private void UpdateButtons()
        {   
            m_recordingToolBarPlayBtn.Enabled = !m_TransportBar.IsRecorderActive || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped );
            m_recordingToolBarRecordingBtn.Enabled = !m_TransportBar.IsPlayerActive || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
        }

        private void m_recordingToolBarPlayBtn_Click(object sender, EventArgs e)
        {
          Assembly myAssembly = Assembly.GetExecutingAssembly();
          Stream pauseStr = null;
          Stream playStr = null;
          pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause.png");
          playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start.png");
          Bitmap pauseImg = new Bitmap(pauseStr);
          Bitmap playImg = new Bitmap (playStr);  

          if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_recordingToolBarPlayBtn.Image =  playImg;
                m_TransportBar.Pause();
            }
          else
            {
                m_recordingToolBarPlayBtn.Image = pauseImg;          
                m_TransportBar.PlayOrResume();
            }
          //  m_StatusLabel.Text = "Playing";
        //    UpdateButtons();        
         }

        private void m_recordingToolBarStopBtn_Click(object sender, EventArgs e)
        {
            
            m_TransportBar.Stop();
         //   m_StatusLabel.Text = "Stopped";
         //   UpdateButtons();
        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.Record();
         //   UpdateButtons();
        }

        private void m_recordingToolBarPrePhraseBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.PrevPhrase();
        }

        private void m_recordingGoToNextPhraseBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.NextPhrase();
        }

        private void m_recordingToolBarNextPageBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.NextPage();
        }

        private void m_recordingToolBarNextSectionBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.NextSection();
        }
    }
}