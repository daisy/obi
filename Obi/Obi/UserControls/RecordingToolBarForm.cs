using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using AudioLib;
using urakawa.media.timing;

namespace Obi.UserControls
{
    public partial class RecordingToolBarForm : Form
    {
        ProjectView.TransportBar m_TransportBar;
        ProjectView.ProjectView m_ProjectView;
        private Image m_PauseImg;
        private Image m_PlayImg;
        private int m_TimeCounter;
        private int m_Count = 0;
        private string m_strStatus = "";
        
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
            
        public RecordingToolBarForm(ProjectView.ProjectView projectView):this  ()
        {
            m_TransportBar = projectView.TransportBar;
            m_ProjectView = projectView;
            m_TransportBar.StateChanged += new AudioLib.AudioPlayer.StateChangedHandler(State_Changed_Player);
            m_TransportBar.Recorder.StateChanged += new AudioLib.AudioRecorder.StateChangedHandler(State_Changed_Recorder);
            projectView.SelectionChanged += new EventHandler(projectview_Selection_Changed);
            m_TransportBar.EnabledChanged += new EventHandler(m_TransportBar_EnabledChanged);
           // if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                UpdateButtons();
        }

        public void State_Changed_Player(object sender, AudioPlayer.StateChangedEventArgs e)
        {
            m_TimeCounter = 0;
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                timer1.Start();
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)
                timer1.Stop();
            UpdateStatus();
            UpdateButtons();
        }

        public void projectview_Selection_Changed(object sender, EventArgs e)
        { UpdateButtons(); }

        public void m_TransportBar_EnabledChanged(object sender, EventArgs e)
        {
            this.Enabled = m_TransportBar.Enabled;
            if (!this.Enabled)
                this.Text = String.Format(Localizer.Message("RecToolbar_Title"), "");
            m_StatusLabel.Text = "";
        }

        private delegate void State_Changed_Recorder_Delegate();
        public void State_Changed_Recorder(object sender, AudioRecorder.StateChangedEventArgs e)
        {
            State_Changed_Recorder();
        }
        public void State_Changed_Recorder()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new State_Changed_Recorder_Delegate(State_Changed_Recorder));
                return;
            }
        m_TimeCounter = 0;
        if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
           timer1.Start();
        else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)            timer1.Stop();
            UpdateStatus();
            UpdateButtons();           
        }

        private void UpdateButtons()
        {
            m_recordingToolBarPlayBtn.Enabled = !m_TransportBar.IsRecorderActive;
          //  m_recordingToolBarRecordingBtn.Enabled = m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
            m_recordingToolBarRecordingBtn.Enabled = (m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)) && m_ProjectView.Selection != null;
            m_recordingToolBarStopBtn.Enabled = m_TransportBar.CanStop || !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
            m_recordingGoToNextPhraseBtn.Enabled = m_recordingToolBarStopBtn.Enabled = m_TransportBar.CanStop && (m_TransportBar.IsPlayerActive || m_TransportBar.IsRecorderActive || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring));
            m_recordingToolBarNextPageBtn.Enabled = m_TransportBar.CanNavigateNextPage;
            m_recordingToolBarPrePhraseBtn.Enabled = m_TransportBar.CanNavigatePrevPhrase;
            m_recordingGoToNextPhraseBtn.Enabled = m_TransportBar.CanNavigateNextPhrase;
            m_recordingToolBarNextSectionBtn.Enabled = m_TransportBar.CanNavigateNextSection;
            if ((m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring) && m_TransportBar.RecordingSection != null)
                this.Text = String.Format(Localizer.Message("RecToolbar_Title"), m_TransportBar.RecordingSection.Label.ToString());
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing && m_TransportBar.PlaybackPhrase != null && m_TransportBar.PlaybackPhrase.IsRooted)
                this.Text = String.Format(Localizer.Message("RecToolbar_Title"), m_TransportBar.PlaybackPhrase.ParentAs<SectionNode>().Label.ToString());               
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
            
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring ||(m_ProjectView.ObiForm.Settings.RecordDirectlyWithRecordButton  && m_TransportBar.CurrentState != Obi.ProjectView.TransportBar.State.Recording))
            {
                m_recordingToolBarRecordingBtn.AccessibleName = "Recording";
            }
        }

        private void m_recordingToolBarPlayBtn_Click(object sender, EventArgs e)
        {        
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {            
                m_TransportBar.Pause();
                m_StatusLabel.Text = Localizer.Message("Paused");                  
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
            m_TimeCounter = 0;
            if(m_TransportBar.CanStop)
                m_TransportBar.Stop();
            if (m_TransportBar.CurrentPlaylist.CurrentTimeInAsset == 0 &&
                !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording) &&
                !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) &&
                !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring))
            {
                m_StatusLabel.Text = Localizer.Message("Stopped");
                timer1.Stop();
            }
            UpdateButtons();
        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = "";
            if(m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording)
             m_TransportBar.Record_Button();
                   
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                m_StatusLabel.Text = Localizer.Message("monitoring_short");          
                       
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
            m_strStatus = Localizer.Message("RecToolbar_NewPhrase");                    
            m_TransportBar.NextPhrase();
            m_Count = 0;
            timer1.Start();
            UpdateButtons();
        }

        private void m_recordingToolBarNextPageBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_NewPage");  
            m_TransportBar.NextPage();
            timer1.Start();       
        }

        private void m_recordingToolBarNextSectionBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_NewSection");  
            m_Count = 0;
            m_TransportBar.NextSection();
            UpdateButtons();
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
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording)
            {
                string recordingPhrase = "";
                if (m_TransportBar.RecordingPhrase != null)
                {
                    recordingPhrase = m_TransportBar.RecordingPhrase.ToString();

                    double timeOfAssetMilliseconds =
                   (double)m_TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64 (m_TransportBar.Recorder.CurrentDurationBytePosition)) /
                   Time.TIME_UNIT;

                    if (m_strStatus == "")
                    {
                        m_StatusLabel.Text = String.Format(Localizer.Message("RecToolbar_Recording"), recordingPhrase, format(timeOfAssetMilliseconds));                    
                    }
                   
                    if (m_Count <= 3)
                    {
                        if (m_strStatus == Localizer.Message("RecToolbar_NewPhrase") || m_strStatus == Localizer.Message("RecToolbar_MarkedTODO") || m_strStatus == Localizer.Message("RecToolbar_NewSection"))
                        {
                            m_StatusLabel.Text = m_strStatus + format(timeOfAssetMilliseconds);
                            m_Count++;
                        }                                        
                   }
                    else
                        m_strStatus= "";                   
                }
                else
                    m_StatusLabel.Text = "";
            }
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                m_StatusLabel.Text = Localizer.Message("monitoring_short");

            if (m_TransportBar.CurrentPlaylist.CurrentTimeInAsset == 0 &&
!(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording) &&
!(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) &&
!(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring))
            {
                m_StatusLabel.Text = Localizer.Message("Stopped");
                timer1.Stop();
            }
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                Console.WriteLine(m_TransportBar.CurrentPlaylist.CurrentPhrase.ToString());
                m_StatusLabel.Text = String.Format(Localizer.Message("RecordingToolBar_StatusPlaying"),
                    m_TransportBar.CurrentPlaylist.CurrentPhrase.ToString(),
                    format(m_TransportBar.CurrentPlaylist.CurrentTimeInAsset));
            }
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
                m_StatusLabel.Text = Localizer.Message("Paused");
        }

        private void m_TODOBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_MarkedTODO");
            m_Count = 0;
            m_TransportBar.MarkTodo();           
        }        
    }
}