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
        private Image m_PauseImg;
        private Image m_PlayImg;
        
        public RecordingToolBarForm()
        {
            InitializeComponent();
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream pauseStr = null;
            Stream playStr = null;
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause.png");
            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start.png");
            m_PauseImg = Image.FromStream(pauseStr);
            m_PlayImg = Image.FromStream(playStr);
        }

        public RecordingToolBarForm(ProjectView.TransportBar transportBar):this  ()
        {
            m_TransportBar = transportBar;
        }

        private void UpdateButtons()
        {
            if ((m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording) || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing))
                m_recordingToolBarRecordingBtn.Enabled = false;
            
            else if (!m_TransportBar.IsPlayerActive || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring) || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused))
                m_recordingToolBarRecordingBtn.Enabled = true;
                            
            m_recordingToolBarPrePhraseBtn.Enabled = !m_TransportBar.IsRecorderActive;
            m_recordingToolBarStopBtn.Enabled = !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);          
        }

        private void m_recordingToolBarPlayBtn_Click(object sender, EventArgs e)
        {           
          if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_recordingToolBarPlayBtn.Image =  m_PlayImg;               
                m_TransportBar.Pause();
            }
          else if(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording)
            {
                m_recordingToolBarPlayBtn.Image = m_PauseImg;
                m_TransportBar.Pause();
                m_recordingToolBarPlayBtn.Image = m_PlayImg;
                //m_StatusLabel.Text = " Recording Paused" +m_TransportBar.;                
             }
          else
              {
              m_recordingToolBarPlayBtn.Image = m_PauseImg;
              m_TransportBar.PlayOrResume();
              }
            m_StatusLabel.Text = "Playing";
            UpdateButtons(); 
                   
         }

        private void m_recordingToolBarStopBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.Stop();
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)
                m_recordingToolBarPlayBtn.Image = m_PlayImg;
            m_StatusLabel.Text = "Stopped";
            UpdateButtons();            
        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.Record();
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording)
               m_recordingToolBarPlayBtn.Image = m_PauseImg;  
            else
              m_recordingToolBarPlayBtn.Image = m_PlayImg; 
            UpdateButtons();
           
        }

        private void m_recordingToolBarPrePhraseBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.PrevPhrase();
            UpdateButtons();
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