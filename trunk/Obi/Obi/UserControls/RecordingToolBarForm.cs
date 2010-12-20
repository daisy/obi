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
        private bool m_IsPlaying = false;
        private int m_TimeCounter;
        private int m_Count;
        private bool m_IsPage = false;
        private bool m_IsPhrase = false;
        private bool m_IsSection = false;
        private bool m_IsTODO = false;
              
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
                     m_TransportBar.StateChanged+= new Obi.Events.Audio.Player.StateChangedHandler (State_Changed);
                     m_TransportBar.Recorder.StateChanged += new Obi.Events.Audio.Recorder.StateChangedHandler(State_Changed);

        }

        public void State_Changed(object sender, EventArgs e)
        {
            m_TimeCounter = 0;
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring) 
                timer1.Start();
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)
                timer1.Stop();
            UpdateStatus();
            UpdateButtons();           
        }

        private void UpdateButtons()
        {
            m_recordingToolBarPlayBtn.Enabled = !m_TransportBar.IsRecorderActive;
            m_recordingToolBarRecordingBtn.Enabled = m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
            m_recordingToolBarStopBtn.Enabled = m_TransportBar.CanStop || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring) ;
            m_recordingToolBarNextPageBtn.Enabled = m_TransportBar.CanNavigateNextPage;
            m_recordingToolBarPrePhraseBtn.Enabled = m_TransportBar.CanNavigatePrevPhrase;
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                this.Text = "Obi recorder bar : [" + m_TransportBar.RecordingSection.Label.ToString() + "]";
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                this.Text = "Obi recorder bar : [" + m_TransportBar.PlaybackPhrase.ParentAs<SectionNode>().Label.ToString() + "]";
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_recordingToolBarPlayBtn.Image = m_PauseImg;
                this.m_recordingToolBarPlayBtn.AccessibleName = "Pause";              
            }
            else
            {
                this.m_recordingToolBarPlayBtn.AccessibleName = "Play";
                m_recordingToolBarRecordingBtn.AccessibleName = "Monitoring";
                m_recordingToolBarPlayBtn.Image = m_PlayImg;
            }
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
            {
                m_recordingToolBarRecordingBtn.AccessibleName = "Recording";
            }
        }

        private void m_recordingToolBarPlayBtn_Click(object sender, EventArgs e)
        {
            m_IsPhrase = false;
            if (!(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording))
                m_IsPlaying = true;
            
          if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {            
                m_TransportBar.Pause();
                m_StatusLabel.Text = "Paused";
                m_IsPlaying = false;               
            }
          else
            {             
              m_TransportBar.PlayOrResume();              
              timer1.Start();              
            }
            UpdateButtons();            
         }

        private void m_recordingToolBarStopBtn_Click(object sender, EventArgs e)
        {
            m_IsPlaying = false;
            m_TimeCounter = 0;
            m_TransportBar.Stop();
            m_StatusLabel.Text = "Stopped";
            timer1.Stop();
            UpdateButtons();
        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {
            m_IsPage = false;
            m_IsPlaying = false;
            m_IsTODO = false;
            m_IsSection = false;
            
             m_TransportBar.Record();
                   
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                m_StatusLabel.Text = "Monitoring";          
                       
            if ((m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring))
            {
                m_TimeCounter = 0;
                timer1.Start();
            }
            UpdateButtons();
        }

        private void m_recordingToolBarPrePhraseBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.PrevPhrase();
            UpdateButtons();
        }

        private void m_recordingGoToNextPhraseBtn_Click(object sender, EventArgs e)
        {
            m_IsPhrase = true;
            m_IsPage = false;
            m_IsSection = false;
            m_IsTODO = false;
            m_Count = 0;              
            m_TransportBar.NextPhrase();           
        }

        private void m_recordingToolBarNextPageBtn_Click(object sender, EventArgs e)
        {
            m_IsPage = true;
            m_IsTODO = false;
            m_IsPhrase = false;
            m_IsSection = false;
            m_TransportBar.NextPage();                    
        }

        private void m_recordingToolBarNextSectionBtn_Click(object sender, EventArgs e)
        {
            m_IsSection = true;
            m_IsPage = false;
            m_IsPhrase = false;
            m_IsTODO = false;
            m_Count = 0;
            m_TransportBar.NextSection();
        }

        private string format(double durationMs)
        {
            double seconds = durationMs / 1000.0;
            int minutes = (int)Math.Floor(seconds / 60.0);
            int seconds_ = (int)Math.Floor(seconds - minutes * 60.0);
            return string.Format(Localizer.Message("duration_hh_mm_ss"), minutes / 60, minutes % 60, seconds_);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateStatus();
       }

        private void UpdateStatus()
        {
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording && !m_IsPage)
            {
                string recordingPhrase = null;
                if (m_TransportBar.RecordingPhrase != null)
                {
                    recordingPhrase = m_TransportBar.RecordingPhrase.ToString();
                    m_StatusLabel.Text = String.Format("Recording {0} {1}", recordingPhrase, format(m_TimeCounter * 500));
                }
                else
                    m_StatusLabel.Text = "";
                m_TimeCounter++;
            }
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                m_StatusLabel.Text = "Monitoring";
            if (m_TransportBar.CurrentPlaylist.CurrentTimeInAsset == 0 && !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording) && !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) &&!(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)) 
                {
                    m_StatusLabel.Text = "Stopped";
                    timer1.Stop();
                }
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
                {
                    m_StatusLabel.Text = String.Format("Playing {0} {1} ", m_TransportBar.CurrentPlaylist.CurrentPhrase.ToString(), format(++m_TransportBar.CurrentPlaylist.CurrentTimeInAsset));
                }
            if (m_IsPhrase && (m_Count <= 4) && (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording))
            {
                m_StatusLabel.Text = "New phrase " + format(m_TimeCounter * 500);
                m_Count++;
            }
            if (m_IsPage && (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording))
            {
                m_StatusLabel.Text = "Recording page phrase " + format(m_TimeCounter * 500);
                m_TimeCounter++;
            }
            if (m_IsSection && (m_Count <= 4) && !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) && (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording))
            {
                m_StatusLabel.Text = "New Section " + format(m_Count * 500);
                m_Count++;
                m_TimeCounter = m_Count;
            }
            if (m_IsTODO && m_Count <= 4 && (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording))
            {
                m_StatusLabel.Text = "Marked TODO " + format(m_TimeCounter * 500);
                m_Count++;
            }
        }

        private void m_TODOBtn_Click(object sender, EventArgs e)
        {
            m_IsTODO = true;
            m_IsPage = false;
            m_IsPhrase = false;
            m_IsSection = false;
            m_Count = 0;
            m_TransportBar.MarkTodo();           
        }        
    }
}