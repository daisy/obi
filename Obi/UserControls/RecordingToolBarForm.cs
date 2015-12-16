using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Resources;
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
        private Image m_PauseImg48;
        private Image m_PauseImg64;
        private Image m_PauseImg80;

        private Image m_PlayImg;
        private Image m_PlayImg48;
        private Image m_PlayImg64;
        private Image m_PlayImg80;

        private int m_TimeCounter;
        private int m_Count = 0;
        private string m_strStatus = "";
        private bool flagBtnPressed = false;
        private Size m_minimumWidth;
        private Size m_minPlayBtn;
        private Size m_minStopBtn;
        private Size m_minRecordingBtn;
        private Size m_minPrePhraseBtn;
        private Size m_minNextPhraseBtn;
        private Size m_minNextPageBtn;
        private Size m_minNextSectionBtn;
        private Size m_minTodoBtn;
        private Size m_minRecordingContainer;
        private Size m_minElapseBackSize;
        private Size m_minSectionEndSize;
        private Size m_minToggleProfileSize;
        private Point m_OriginalLocationOfEnlargeBtn;
        private Point m_OriginalLocationOfReduceBtn;

        public double NetSizeIncrementOfButtons = 0;
        public bool flagIncrementDecrement = true;
        public double NetSizeIncBtn = 0;

        private Image play48;
        private Image play64;

        private Image m_RecordingBtn48;
        private Image m_RecordingBtn64;
        private Image m_RecordingBtn80;
        private Image m_RecordingBtn;

        private Image m_MonitorBtn;
        private Image m_MonitorBtn48;
        private Image m_MonitorBtn64;
        private Image m_MonitorBtn80;

        private Image m_RecordingTodoBtn;
        private Image m_RecordingTodoBtn48;
        private Image m_RecordingTodoBtn64;
        private Image m_RecordingTodoBtn80;
       
        private Image m_RecordingGoDownBtn;
        private Image m_RecordingGoDownBtn48;
        private Image m_RecordingGoDownBtn64;
        private Image m_RecordingGoDownBtn80;
       
        private Image m_RecordingGoNextBtn;
        private Image m_RecordingGoNextBtn48;
        private Image m_RecordingGoNextBtn64;
        private Image m_RecordingGoNextBtn80;

        private Image m_RecordingGoPreviousBtn;
        private Image m_RecordingGoPreviousBtn48;
        private Image m_RecordingGoPreviousBtn64;
        private Image m_RecordingGoPreviousBtn80;


        private Image m_RecordingPlaybackStopBtn;
        private Image m_RecordingPlaybackStopBtn48;
        private Image m_RecordingPlaybackStopBtn64;
        private Image m_RecordingPlaybackStopBtn80;

        private Image m_RecordingNextPage;
        private Image m_RecordingNextPage48;
        private Image m_RecordingNextPage64;
        private Image m_RecordingNextPage80;

        private Image m_ElapseBack;
        private Image m_ElapseBack48;
        private Image m_ElapseBack64;
        private Image m_ElapseBack80;

        private Image m_SectionEnd;
        private Image m_SectionEnd48;
        private Image m_SectionEnd64;
        private Image m_SectionEnd80;

        private Image m_ToggleProfileImg;
        private Image m_ToggleProfileImg48;
        private Image m_ToggleProfileImg64;
        private Image m_ToggleProfileImg80;

        HelpProvider helpProvider1;
        //public delegate void RecorderSettingsSet();

        //public event RecorderSettingsSet SaveTheSize;
        public RecordingToolBarForm()
        {
            InitializeComponent();
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream pauseStr = null;
            Stream playStr = null;
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause.png");
            m_PauseImg = Image.FromStream(pauseStr);
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause48.png");
            m_PauseImg48 = Image.FromStream(pauseStr);
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause64.png");
            m_PauseImg64 = Image.FromStream(pauseStr);
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause80.png");
            m_PauseImg80 = Image.FromStream(pauseStr);

            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start.png");
            m_PlayImg = Image.FromStream(playStr);
            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start48.png");
            m_PlayImg48 = Image.FromStream(playStr);
            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start64.png");
            m_PlayImg64 = Image.FromStream(playStr);
            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start80.png");
            m_PlayImg80 = Image.FromStream(playStr);

            ResourceManager resourceManager = new ResourceManager("Obi.Resources.RecordingToolBarForm", GetType().Assembly);
            Stream PlayPhrase;
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-record.png");
            m_RecordingBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-record48.png");
            m_RecordingBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-record64.png");
            m_RecordingBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-record80.png");
            m_RecordingBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media_monitor.png");
            m_MonitorBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media_monitor48.png");
            m_MonitorBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media_monitor64.png");
            m_MonitorBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media_monitor80.png");
            m_MonitorBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.emblem-important.png");
            m_RecordingTodoBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.emblem-important-md48.png");
            m_RecordingTodoBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.emblem-important-md64.png");
            m_RecordingTodoBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.emblem-important-md80.png");
            m_RecordingTodoBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-down.png");
            m_RecordingGoDownBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-down48.png");
            m_RecordingGoDownBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-down64.png");
            m_RecordingGoDownBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-down80.png");
            m_RecordingGoDownBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-previous.png");
            m_RecordingGoPreviousBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-previous48.png");
            m_RecordingGoPreviousBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-previous64.png");
            m_RecordingGoPreviousBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-previous80.png");
            m_RecordingGoPreviousBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-next.png");
            m_RecordingGoNextBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-next48.png");
            m_RecordingGoNextBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-next64.png");
            m_RecordingGoNextBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.go-next80.png");
            m_RecordingGoNextBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-stop.png");
            m_RecordingPlaybackStopBtn = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-stop48.png");
            m_RecordingPlaybackStopBtn48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-stop64.png");
            m_RecordingPlaybackStopBtn64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-stop80.png");
            m_RecordingPlaybackStopBtn80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.nextpage.png");
            m_RecordingNextPage = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.nextpage48.png");
            m_RecordingNextPage48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.nextpage64.png");
            m_RecordingNextPage64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.nextpage80.png");
            m_RecordingNextPage80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ElapseBack.png");
            m_ElapseBack = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ElapseBack48.png");
            m_ElapseBack48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ElapseBack64.png");
            m_ElapseBack64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ElapseBack80.png");
            m_ElapseBack80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.Section End.png");
            m_SectionEnd = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.Section End48.png");
            m_SectionEnd48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.Section End64.png");
            m_SectionEnd64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.Section End80.png");
            m_SectionEnd80 = Image.FromStream(PlayPhrase);

            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ToggleProfile.png");
            m_ToggleProfileImg = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ToggleProfile48.png");
            m_ToggleProfileImg48 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ToggleProfile64.png");
            m_ToggleProfileImg64 = Image.FromStream(PlayPhrase);
            PlayPhrase = myAssembly.GetManifestResourceStream("Obi.UserControls.ToggleProfile80.png");
            m_ToggleProfileImg80 = Image.FromStream(PlayPhrase);            

            helpProvider1 = new HelpProvider();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Exploring the GUI\\Obi Views and Transport Bar\\Recording Toolbar.htm");  	
        }

        public RecordingToolBarForm(ProjectView.ProjectView projectView)
            : this()
        {
            m_TransportBar = projectView.TransportBar;
            m_ProjectView = projectView;
            m_TransportBar.StateChanged += new AudioLib.AudioPlayer.StateChangedHandler(State_Changed_Player);
            m_TransportBar.Recorder.StateChanged += new AudioLib.AudioRecorder.StateChangedHandler(State_Changed_Recorder);
            projectView.SelectionChanged += new EventHandler(projectview_Selection_Changed);
            m_TransportBar.EnabledChanged += new EventHandler(m_TransportBar_EnabledChanged);
            m_RecordingToolBartoolTip.SetToolTip(m_chkMonitorContinuously, Localizer.Message("Audio_MonitorAlways"));

            if (m_ProjectView != null && m_ProjectView.ObiForm != null && m_ProjectView.ObiForm.Settings != null && (m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1 == null || m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1 == ""))
            {
                m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1 = "Basic";
            }

            if (m_ProjectView != null && m_ProjectView.ObiForm != null && m_ProjectView.ObiForm.Settings != null && (m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2 == null || m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2 == ""))
            {
                m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2 = "Advance";
            }
            m_ToggleProfile.ToolTipText = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2);
            m_ToggleProfile.AccessibleName = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2);

            m_recordingToolBarRecordingBtn.ToolTipText = Localizer.Message("Transport_StartRecording");
            m_recordingToolBarRecordingBtn.AccessibleName = Localizer.Message("Transport_StartRecording");
            m_ProjectView.SelectionChanged += new EventHandler(m_ProjectView_SelectionChanged);
           
            
            // if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
            UpdateButtons();
        }

        void m_ProjectView_SelectionChanged(object sender, EventArgs e)
        {

            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {
                m_StatusLabel.Text = m_ProjectView.Selection.ToString();
            }
        }

        public Size RecordingToolBarPlayBtn
        {
            get { return m_recordingToolBarPlayBtn.Size; }
        }
        public Size RecordingToolBarStopBtn
        {
            get { return m_recordingToolBarStopBtn.Size; }
        }

        public Size RecordingToolBarRecordingBtn
        {
            get { return m_recordingToolBarRecordingBtn.Size; }
        }

        public Size RecordingToolBarPrePhraseBtn
        {
            get { return m_recordingToolBarPrePhraseBtn.Size; }
        }

        public Size RecordingGoToNextPhraseBtn
        {
            get { return m_recordingGoToNextPhraseBtn.Size; }
        }

        public Size RecordingToolBarNextSectionBtn
        {
            get { return m_recordingToolBarNextSectionBtn.Size; }
        }

        public Size TODOBtn
        {
            get { return m_TODOBtn.Size; }
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

            m_recordingToolBarElapseBackBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarPlayBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarStopBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarRecordingBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarPrePhraseBtn.Enabled = m_TransportBar.Enabled;
            m_recordingGoToNextPhraseBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarNextPageBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarNextSectionBtn.Enabled = m_TransportBar.Enabled;
            m_recordingToolBarSectionEndBtn.Enabled = m_TransportBar.Enabled;
            m_chkMonitorContinuously.Enabled = m_TransportBar.Enabled && m_ProjectView.ObiForm.Settings.Audio_AllowOverwrite && m_ProjectView.Presentation != null;
            m_Enlarge.Enabled = m_TransportBar.Enabled;
            m_Reduce.Enabled = m_TransportBar.Enabled;

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
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped) timer1.Stop();
            UpdateStatus();
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            m_recordingToolBarPlayBtn.Enabled = !m_TransportBar.IsRecorderActive && (m_TransportBar.CanPlay || m_TransportBar.CanResumePlayback);
            //  m_recordingToolBarRecordingBtn.Enabled = m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
            m_recordingToolBarRecordingBtn.Enabled = (m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)) && m_ProjectView.Selection != null;
            m_recordingToolBarStopBtn.Enabled = m_TransportBar.CanStop || !(m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped);
            m_recordingGoToNextPhraseBtn.Enabled = m_recordingToolBarStopBtn.Enabled = m_TransportBar.CanStop && (m_TransportBar.IsPlayerActive || m_TransportBar.IsRecorderActive || (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring));
            m_recordingToolBarNextPageBtn.Enabled = m_TransportBar.CanNavigateNextPage && m_ProjectView.Presentation != null;
            m_recordingToolBarPrePhraseBtn.Enabled = m_TransportBar.CanNavigatePrevPhrase;
            m_recordingGoToNextPhraseBtn.Enabled = m_TransportBar.CanNavigateNextPhrase && m_ProjectView.Presentation != null;
            m_recordingToolBarNextSectionBtn.Enabled = m_TransportBar.CanNavigateNextSection && m_ProjectView.Presentation != null;
            m_TODOBtn.Enabled = m_ProjectView.CanSetTODOStatus;
            if ((m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring) && m_TransportBar.RecordingSection != null)
                this.Text = String.Format(Localizer.Message("RecToolbar_Title"), m_TransportBar.RecordingSection.Label.ToString());
            else if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing && m_TransportBar.PlaybackPhrase != null && m_TransportBar.PlaybackPhrase.IsRooted)
                this.Text = String.Format(Localizer.Message("RecToolbar_Title"), m_TransportBar.PlaybackPhrase.ParentAs<SectionNode>().Label.ToString());
            else if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Node != null && m_ProjectView.Selection.Node.Parent != null)
            {
                if (m_ProjectView.Selection.Node is SectionNode)
                {
                    SectionNode tempNode = (SectionNode)m_ProjectView.Selection.Node;
                    this.Text = String.Format(Localizer.Message("RecToolbar_Title"), tempNode.Label);
                }
                else if (m_ProjectView.Selection.Node.Parent is SectionNode)
                {
                    SectionNode tempNode = (SectionNode)m_ProjectView.Selection.Node.Parent;
                    this.Text = String.Format(Localizer.Message("RecToolbar_Title"), tempNode.Label);
                }
            }
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_recordingToolBarPlayBtn.Image = m_PauseImg;
                m_recordingToolBarPlayBtn.Enabled = true;
                this.m_recordingToolBarPlayBtn.AccessibleName = "Pause";
                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg80;
                }
            }
            else
            {
                this.m_recordingToolBarPlayBtn.AccessibleName = "Play";
                m_recordingToolBarRecordingBtn.AccessibleName = "Monitoring";
                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg80;
                }
            }

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring || m_ProjectView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton
               || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CanResumeRecording)
            {
                m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn80;
                }

            }
            else
            {
                m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn80;
                }

            }

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring || (m_ProjectView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton && m_TransportBar.CurrentState != Obi.ProjectView.TransportBar.State.Recording))
            {
                m_recordingToolBarRecordingBtn.AccessibleName = "Recording";
            }
            m_chkMonitorContinuously.Enabled = !m_TransportBar.IsPlayerActive && m_ProjectView.ObiForm.Settings.Audio_AllowOverwrite && m_ProjectView.Presentation != null;
            if (m_chkMonitorContinuously.Enabled && m_TransportBar.MonitorContinuously != m_chkMonitorContinuously.Checked)
            {
                m_chkMonitorContinuously.CheckedChanged -= new EventHandler (m_chkMonitorContinuously_CheckedChanged) ;
                m_chkMonitorContinuously.Checked= m_TransportBar.MonitorContinuously;
                Console.WriteLine("event change " + m_chkMonitorContinuously.Checked);
                m_chkMonitorContinuously.CheckedChanged += new EventHandler(m_chkMonitorContinuously_CheckedChanged);
            }
            string tempSettingsName = m_ProjectView.ObiForm.Settings.SettingsName;
            string[] str = tempSettingsName.Split(new string[] { " profile for" }, StringSplitOptions.None);
    
            if (str[0].ToLower() == m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1.ToLower())
            {
                m_ToggleProfile.ToolTipText = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2);
                m_ToggleProfile.AccessibleName = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2);
            }
            else if (str[0].ToLower() == m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2.ToLower())
            {
                m_ToggleProfile.ToolTipText = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1);
                m_ToggleProfile.AccessibleName = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2, m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1);
            }
            else
            {
                m_ToggleProfile.ToolTipText = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), str[0], m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1);
                m_ToggleProfile.AccessibleName = String.Format(Localizer.Message("RecordingToolbar_SwitchProfile"), str[0], m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1);
            }
            
            if (m_ProjectView.TransportBar.IsRecorderActive && !m_ProjectView.TransportBar.IsRecorderActive)
            {
                this.Focus();
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
                if (m_ProjectView.ObiForm.Settings.Audio_PlaySectionUsingPlayBtn)
                {
                    m_TransportBar.PlaySection();
                }
                else
                {
                    m_TransportBar.PlayOrResume();
                }
                timer1.Start();
            }
            UpdateButtons();
            this.Focus();
        }

        private void m_recordingToolBarStopBtn_Click(object sender, EventArgs e)
        {
            m_TimeCounter = 0;
            if (m_TransportBar.CanStop)
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
            this.Focus();
        }

        private void m_recordingToolBarRecordingBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = "";
            if (m_TransportBar.CanRecord || m_TransportBar.CanResumeRecording)
                m_TransportBar.Record_Button();

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring)
                m_StatusLabel.Text = Localizer.Message("monitoring_short");

            if ((m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring))
            {
                m_TimeCounter = 0;
                timer1.Start();
            }
            UpdateButtons();
            this.Focus();
        }

        private void m_recordingToolBarPrePhraseBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.PrevPhrase();
            UpdateButtons();
            this.Focus();
        }

        private void m_recordingGoToNextPhraseBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_NewPhrase");
            m_TransportBar.NextPhrase();
            m_Count = 0;
            timer1.Start();
            UpdateButtons();
            this.Focus();
        }

        private void m_recordingToolBarNextPageBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_NewPage");
            m_TransportBar.NextPage();
            timer1.Start();
            this.Focus();
        }

        private void m_recordingToolBarNextSectionBtn_Click(object sender, EventArgs e)
        {
            m_strStatus = Localizer.Message("RecToolbar_NewSection");
            m_Count = 0;
            m_TransportBar.NextSection();
            UpdateButtons();
            this.Focus();
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
                   (double)m_TransportBar.Recorder.RecordingPCMFormat.ConvertBytesToTime(Convert.ToInt64(m_TransportBar.Recorder.CurrentDurationBytePosition)) /
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
                        m_strStatus = "";
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
            this.Focus();
        }

        private void RecordingToolBarForm_Load(object sender, EventArgs e)
        {
            m_minimumWidth = this.Size;
            m_OriginalLocationOfEnlargeBtn = m_Enlarge.Location;
            m_OriginalLocationOfReduceBtn = m_Reduce.Location;
            m_minRecordingContainer = recordingToolBarToolStrip.Size;
            m_minPlayBtn = m_recordingToolBarPlayBtn.Size;
            m_minStopBtn = m_recordingToolBarStopBtn.Size;
            m_minRecordingBtn = m_recordingToolBarRecordingBtn.Size;
            m_minPrePhraseBtn = m_recordingToolBarPrePhraseBtn.Size;
            m_minNextPhraseBtn = m_recordingGoToNextPhraseBtn.Size;
            m_minNextPageBtn = m_recordingToolBarNextPageBtn.Size;
            m_minNextSectionBtn = m_recordingToolBarNextSectionBtn.Size;
            m_minTodoBtn = m_TODOBtn.Size;
            m_minElapseBackSize = m_recordingToolBarElapseBackBtn.Size;
            m_minSectionEndSize = m_recordingToolBarSectionEndBtn.Size;
            m_minToggleProfileSize = m_ToggleProfile.Size;

            if (m_ProjectView.ObiForm.Settings.Audio_AllowOverwrite
                && m_ProjectView.ObiForm.Settings.Audio_AlwaysMonitorRecordingToolBar
                && m_ProjectView.Presentation != null)
            {
                if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing) m_TransportBar.Pause();
                if (m_TransportBar.IsPlayerActive) m_TransportBar.Stop();

                    m_chkMonitorContinuously.Checked = true;
                }

        }

        private void m_Enlarge_Click(object sender, EventArgs e)
        {

            flagBtnPressed = true;
            EnlargeButtonSize();
            flagBtnPressed = false;
        }

        private void m_Reduce_Click(object sender, EventArgs e)
        {
            int temp;

            flagBtnPressed = true;
            ReduceButtonSize();
            flagBtnPressed = false;
        }
        public void EnlargeButtonSize()
        {
            if (flagBtnPressed == true)
            {
                NetSizeIncBtn = 0.5;
            }
            else
            {
                NetSizeIncBtn = NetSizeIncrementOfButtons;
            }
            flagIncrementDecrement = true;
            if (NetSizeIncrementOfButtons > 1 && flagBtnPressed == true)
            {
                NetSizeIncBtn = 0;
            }
            double tempRatio = (this.Width / this.Height);
            this.Width = (int)(this.Width + m_minimumWidth.Width * NetSizeIncBtn);
            //this.Height = (int)(this.Height + (m_minimumWidth.Width * NetSizeIncBtn) / (tempRatio));
            //recordingToolBarToolStrip.Width =
            //    (int)(recordingToolBarToolStrip.Width + recordingToolBarToolStrip.Width * NetSizeIncBtn);
             //m_minRecordingContainer.Width
           // recordingToolBarToolStrip.Width = this.Width;
            this.Height = (int)(this.Height + m_minTodoBtn.Height * NetSizeIncBtn);
            recordingToolBarToolStrip.Height =
                (int)(recordingToolBarToolStrip.Height + m_minTodoBtn.Height * NetSizeIncBtn);
  

            m_recordingToolBarPlayBtn.Width = (int)(m_recordingToolBarPlayBtn.Width + m_minPlayBtn.Width * NetSizeIncBtn);
            m_recordingToolBarPlayBtn.Height = (int)(m_recordingToolBarPlayBtn.Height + m_minPlayBtn.Height * NetSizeIncBtn);

            m_recordingToolBarStopBtn.Width = (int)(m_recordingToolBarStopBtn.Width + m_minStopBtn.Width * NetSizeIncBtn);
            m_recordingToolBarStopBtn.Height = (int)(m_recordingToolBarStopBtn.Height + m_minStopBtn.Height * NetSizeIncBtn);

            m_recordingToolBarRecordingBtn.Width = (int)(m_recordingToolBarRecordingBtn.Width + m_minRecordingBtn.Width * NetSizeIncBtn);
            m_recordingToolBarRecordingBtn.Height = (int)(m_recordingToolBarRecordingBtn.Height + m_minRecordingBtn.Height * NetSizeIncBtn);

            m_recordingToolBarPrePhraseBtn.Width = (int)(m_recordingToolBarPrePhraseBtn.Width + m_minPrePhraseBtn.Width * NetSizeIncBtn);
            m_recordingToolBarPrePhraseBtn.Height = (int)(m_recordingToolBarPrePhraseBtn.Height + m_minPrePhraseBtn.Height * NetSizeIncBtn);

            m_recordingGoToNextPhraseBtn.Width = (int)(m_recordingGoToNextPhraseBtn.Width + m_minNextPhraseBtn.Width * NetSizeIncBtn);
            m_recordingGoToNextPhraseBtn.Height = (int)(m_recordingGoToNextPhraseBtn.Height + m_minNextPhraseBtn.Height * NetSizeIncBtn);

            m_recordingToolBarNextPageBtn.Width = (int)(m_recordingToolBarNextPageBtn.Width + m_minNextPageBtn.Width * NetSizeIncBtn);
            m_recordingToolBarNextPageBtn.Height = (int)(m_recordingToolBarNextPageBtn.Height + m_minNextPageBtn.Height * NetSizeIncBtn);

            m_recordingToolBarNextSectionBtn.Width = (int)(m_recordingToolBarNextSectionBtn.Width + m_minNextSectionBtn.Width * NetSizeIncBtn);
            m_recordingToolBarNextSectionBtn.Height = (int)(m_recordingToolBarNextSectionBtn.Height + m_minNextSectionBtn.Height * NetSizeIncBtn);

            m_TODOBtn.Width = (int)(m_TODOBtn.Width + m_minTodoBtn.Width * NetSizeIncBtn);
            m_TODOBtn.Height = (int)(m_TODOBtn.Height + m_minTodoBtn.Height * NetSizeIncBtn);

            m_recordingToolBarElapseBackBtn.Width = (int)(m_recordingToolBarElapseBackBtn.Width + m_minElapseBackSize.Width * NetSizeIncBtn);
            m_recordingToolBarElapseBackBtn.Height = (int)(m_recordingToolBarElapseBackBtn.Height + m_minElapseBackSize.Height * NetSizeIncBtn);

            m_recordingToolBarSectionEndBtn.Width = (int)(m_recordingToolBarSectionEndBtn.Width + m_minSectionEndSize.Width * NetSizeIncBtn);
            m_recordingToolBarSectionEndBtn.Height = (int)(m_recordingToolBarSectionEndBtn.Height + m_minSectionEndSize.Height * NetSizeIncBtn);

            m_ToggleProfile.Width = (int)(m_ToggleProfile.Width + m_minToggleProfileSize.Width * NetSizeIncBtn);
            m_ToggleProfile.Height = (int)(m_ToggleProfile.Height + m_minToggleProfileSize.Height * NetSizeIncBtn);

            int diff = m_minimumWidth.Width - m_minRecordingContainer.Width;
            this.Width = m_recordingToolBarPlayBtn.Width + m_recordingToolBarStopBtn.Width + m_recordingToolBarRecordingBtn.Width
                                             + m_recordingToolBarPrePhraseBtn.Width + m_recordingGoToNextPhraseBtn.Width + m_recordingToolBarNextPageBtn.Width
                                             + m_recordingToolBarNextSectionBtn.Width + m_TODOBtn.Width + m_recordingToolBarElapseBackBtn.Width
                                             + m_recordingToolBarSectionEndBtn.Width + m_ToggleProfile.Width + diff + 50;
            recordingToolBarToolStrip.Width = this.Width - diff;
            Point p = new Point(0, 0);
            //int x = m_statusStrip.Top - m_Enlarge.Bottom;
            p.Y = m_statusStrip.Top - 30;
            // p.X = m_Enlarge.Location.X;
            p.X = this.Width / 2 - 120;
            m_Enlarge.Location = p;

            // p.X = m_Reduce.Location.X;
            p.X = this.Width / 2 + 20;
            m_Reduce.Location = p;
            m_chkMonitorContinuously.Location = new Point(9, m_statusStrip.Top - 30);

            if (flagBtnPressed == true)
            {

                if (NetSizeIncrementOfButtons <= 1)
                {
                    NetSizeIncrementOfButtons += 0.5;

                }
            }
            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg80;
                }
            }
            else
            {
                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg80;
                }
            }

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring || m_ProjectView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton
   || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CanResumeRecording)
            {
                m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn80;
                }

            }
            else
            {
                m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn80;
                }

            }
            if (NetSizeIncrementOfButtons == 0)
            {
                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;
                m_TODOBtn.Image = m_RecordingTodoBtn;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd;
                m_ToggleProfile.Image = m_ToggleProfileImg;
            }
            else if (NetSizeIncrementOfButtons == 0.5)
            {
                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn48;
                m_TODOBtn.Image = m_RecordingTodoBtn48;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn48;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn48;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn48;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage48;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn48;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack48;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd48;
                m_ToggleProfile.Image = m_ToggleProfileImg48;
            }
            else if (NetSizeIncrementOfButtons == 1)
            {
                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn64;
                m_TODOBtn.Image = m_RecordingTodoBtn64;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn64;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn64;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn64;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage64;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn64;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack64;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd64;
                m_ToggleProfile.Image = m_ToggleProfileImg64;
            }
            else if (NetSizeIncrementOfButtons == 1.5)
            {
                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn80;
                m_TODOBtn.Image = m_RecordingTodoBtn80;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn80;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn80;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn80;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage80;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn80;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack80;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd80;
                m_ToggleProfile.Image = m_ToggleProfileImg80;
            }

        }
        public void ReduceButtonSize()
        {
            if (NetSizeIncrementOfButtons < 0.5)
            {
                NetSizeIncrementOfButtons = 0;
            }
            if (flagBtnPressed == true)
            {
                NetSizeIncBtn = 0.5;
            }
            else
            {
                NetSizeIncBtn = NetSizeIncrementOfButtons;
            }
            flagIncrementDecrement = false;
            if (m_minimumWidth.Width < (int)(this.Width - m_minimumWidth.Width * NetSizeIncBtn))
            {

                double tempRatio = (this.Width / this.Height);
                this.Width = (int)(this.Width - m_minimumWidth.Width * NetSizeIncBtn);
                //  this.Height = (int)(this.Height - (m_minimumWidth.Width * NetSizeIncBtn) / (tempRatio));
                 //recordingToolBarToolStrip.Width =
                //    (int)(recordingToolBarToolStrip.Width - recordingToolBarToolStrip.Width * NetSizeIncBtn);

               // recordingToolBarToolStrip.Width = this.Width;
                this.Height = (int)(this.Height - m_minTodoBtn.Height * NetSizeIncBtn);
                recordingToolBarToolStrip.Height =
                    (int)(recordingToolBarToolStrip.Height - m_minTodoBtn.Height * NetSizeIncBtn);



                //  this.Height = (int)(this.Height - this.Height * NetSizeIncBtn);
                m_recordingToolBarPlayBtn.Width = (int)(m_recordingToolBarPlayBtn.Width - m_minPlayBtn.Width * NetSizeIncBtn);
                m_recordingToolBarPlayBtn.Height = (int)(m_recordingToolBarPlayBtn.Height - m_minPlayBtn.Height * NetSizeIncBtn);

                m_recordingToolBarStopBtn.Width = (int)(m_recordingToolBarStopBtn.Width - m_minStopBtn.Width * NetSizeIncBtn);
                m_recordingToolBarStopBtn.Height = (int)(m_recordingToolBarStopBtn.Height - m_minStopBtn.Height * NetSizeIncBtn);

                m_recordingToolBarRecordingBtn.Width = (int)(m_recordingToolBarRecordingBtn.Width - m_minRecordingBtn.Width * NetSizeIncBtn);
                m_recordingToolBarRecordingBtn.Height = (int)(m_recordingToolBarRecordingBtn.Height - m_minRecordingBtn.Height * NetSizeIncBtn);

                m_recordingToolBarPrePhraseBtn.Width = (int)(m_recordingToolBarPrePhraseBtn.Width - m_minPrePhraseBtn.Width * NetSizeIncBtn);
                m_recordingToolBarPrePhraseBtn.Height = (int)(m_recordingToolBarPrePhraseBtn.Height - m_minPrePhraseBtn.Height * NetSizeIncBtn);

                m_recordingGoToNextPhraseBtn.Width = (int)(m_recordingGoToNextPhraseBtn.Width - m_minNextPhraseBtn.Width * NetSizeIncBtn);
                m_recordingGoToNextPhraseBtn.Height = (int)(m_recordingGoToNextPhraseBtn.Height - m_minNextPhraseBtn.Height * NetSizeIncBtn);

                m_recordingToolBarNextPageBtn.Width = (int)(m_recordingToolBarNextPageBtn.Width - m_minNextPageBtn.Width * NetSizeIncBtn);
                m_recordingToolBarNextPageBtn.Height = (int)(m_recordingToolBarNextPageBtn.Height - m_minNextPageBtn.Height * NetSizeIncBtn);

                m_recordingToolBarNextSectionBtn.Width = (int)(m_recordingToolBarNextSectionBtn.Width - m_minNextSectionBtn.Width * NetSizeIncBtn);
                m_recordingToolBarNextSectionBtn.Height = (int)(m_recordingToolBarNextSectionBtn.Height - m_minNextSectionBtn.Height * NetSizeIncBtn);

                m_TODOBtn.Width = (int)(m_TODOBtn.Width - m_minTodoBtn.Width * NetSizeIncBtn);
                m_TODOBtn.Height = (int)(m_TODOBtn.Height - m_minTodoBtn.Height * NetSizeIncBtn);

                m_recordingToolBarElapseBackBtn.Width = (int)(m_recordingToolBarElapseBackBtn.Width - m_minElapseBackSize.Width * NetSizeIncBtn);
                m_recordingToolBarElapseBackBtn.Height = (int)(m_recordingToolBarElapseBackBtn.Height - m_minElapseBackSize.Height * NetSizeIncBtn);

                m_recordingToolBarSectionEndBtn.Width = (int)(m_recordingToolBarSectionEndBtn.Width - m_minSectionEndSize.Width * NetSizeIncBtn);
                m_recordingToolBarSectionEndBtn.Height = (int)(m_recordingToolBarSectionEndBtn.Height - m_minSectionEndSize.Height * NetSizeIncBtn);

                m_ToggleProfile.Width = (int)(m_ToggleProfile.Width - m_minToggleProfileSize.Width * NetSizeIncBtn);
                m_ToggleProfile.Height = (int)(m_ToggleProfile.Height - m_minToggleProfileSize.Height * NetSizeIncBtn);


                int diff = m_minimumWidth.Width - m_minRecordingContainer.Width;
                this.Width = m_recordingToolBarPlayBtn.Width + m_recordingToolBarStopBtn.Width + m_recordingToolBarRecordingBtn.Width
                                      + m_recordingToolBarPrePhraseBtn.Width + m_recordingGoToNextPhraseBtn.Width + m_recordingToolBarNextPageBtn.Width
                                      + m_recordingToolBarNextSectionBtn.Width + m_TODOBtn.Width + m_recordingToolBarElapseBackBtn.Width
                                      + m_recordingToolBarSectionEndBtn.Width + m_ToggleProfile.Width + diff + 50;
                recordingToolBarToolStrip.Width = this.Width - diff;

                Point p = new Point(0, 0);
                p.Y = m_statusStrip.Top - 30;
                //  p.X = m_Enlarge.Location.X;
                p.X = this.Width / 2 - 120;
                m_Enlarge.Location = p;

                //p.X = m_Reduce.Location.X;
                p.X = this.Width / 2 + 20;
                m_Reduce.Location = p;
                m_chkMonitorContinuously.Location = new Point(9, m_statusStrip.Top - 30);
            }
            else
            {
                this.Size = m_minimumWidth;
                recordingToolBarToolStrip.Size = m_minRecordingContainer;
                m_Enlarge.Location = m_OriginalLocationOfEnlargeBtn;
                m_Reduce.Location = m_OriginalLocationOfReduceBtn;

                m_recordingToolBarPlayBtn.Size = m_minPlayBtn;


                m_recordingToolBarStopBtn.Size = m_minStopBtn;

                m_recordingToolBarRecordingBtn.Size = m_minRecordingBtn;

                m_recordingToolBarPrePhraseBtn.Size = m_minPrePhraseBtn;

                m_recordingGoToNextPhraseBtn.Size = m_minNextPhraseBtn;


                m_recordingToolBarNextPageBtn.Size = m_minNextPageBtn;

                m_recordingToolBarNextSectionBtn.Size = m_minNextSectionBtn;
                m_TODOBtn.Size = m_minTodoBtn;

                m_recordingToolBarElapseBackBtn.Size = m_minElapseBackSize;
                m_recordingToolBarSectionEndBtn.Size = m_minSectionEndSize;
                m_ToggleProfile.Size = m_minToggleProfileSize;

            }


            if (flagBtnPressed == true)
            {
                NetSizeIncrementOfButtons -= 0.5;
                if (NetSizeIncrementOfButtons < 0.5)
                {
                    NetSizeIncrementOfButtons = 0;
                }
            }

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PauseImg80;
                }
            }
            else
            {

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarPlayBtn.Image = m_PlayImg80;
                }
            }

            if (m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Monitoring || m_ProjectView.ObiForm.Settings.Audio_RecordDirectlyWithRecordButton
   || m_TransportBar.CurrentState == Obi.ProjectView.TransportBar.State.Recording || m_TransportBar.CanResumeRecording)
            {
                m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_RecordingBtn80;
                }

            }
            else
            {
                m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;

                if (NetSizeIncrementOfButtons == 0)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn;
                }
                else if (NetSizeIncrementOfButtons == 0.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn48;
                }
                else if (NetSizeIncrementOfButtons == 1)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn64;
                }
                else if (NetSizeIncrementOfButtons == 1.5)
                {
                    m_recordingToolBarRecordingBtn.Image = m_MonitorBtn80;
                }

            }
            if (NetSizeIncrementOfButtons == 0)
            {

                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn;
                m_TODOBtn.Image = m_RecordingTodoBtn;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd;
                m_ToggleProfile.Image = m_ToggleProfileImg;

            }
            else if (NetSizeIncrementOfButtons == 0.5)
            {

                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn48;
                m_TODOBtn.Image = m_RecordingTodoBtn48;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn48;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn48;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn48;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage48;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn48;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack48;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd48;
                m_ToggleProfile.Image = m_ToggleProfileImg48;
            }
            else if (NetSizeIncrementOfButtons == 1)
            {

                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn64;
                m_TODOBtn.Image = m_RecordingTodoBtn64;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn64;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn64;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn64;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage64;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn64;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack64;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd64;
                m_ToggleProfile.Image = m_ToggleProfileImg64;
            }
            else if (NetSizeIncrementOfButtons == 1.5)
            {
                //m_recordingToolBarRecordingBtn.Image = m_RecordingBtn80;
                m_TODOBtn.Image = m_RecordingTodoBtn80;
                m_recordingToolBarStopBtn.Image = m_RecordingPlaybackStopBtn80;
                m_recordingToolBarPrePhraseBtn.Image = m_RecordingGoPreviousBtn80;
                m_recordingGoToNextPhraseBtn.Image = m_RecordingGoNextBtn80;
                m_recordingToolBarNextPageBtn.Image = m_RecordingNextPage80;
                m_recordingToolBarNextSectionBtn.Image = m_RecordingGoDownBtn80;
                m_recordingToolBarElapseBackBtn.Image = m_ElapseBack80;
                m_recordingToolBarSectionEndBtn.Image = m_SectionEnd80;
                m_ToggleProfile.Image = m_ToggleProfileImg80;
            }

        }

        private void RecordingToolBarForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_chkMonitorContinuously.Checked) m_chkMonitorContinuously.Checked = false;
            this.Width = m_minimumWidth.Width;
            m_recordingToolBarPlayBtn.Size = m_minPlayBtn;


            m_recordingToolBarStopBtn.Size = m_minStopBtn;

            m_recordingToolBarRecordingBtn.Size = m_minRecordingBtn;

            m_recordingToolBarPrePhraseBtn.Size = m_minPrePhraseBtn;

            m_recordingGoToNextPhraseBtn.Size = m_minNextPhraseBtn;


            m_recordingToolBarNextPageBtn.Size = m_minNextPageBtn;

            m_recordingToolBarNextSectionBtn.Size = m_minNextSectionBtn;
            m_TODOBtn.Size = m_minTodoBtn;
            m_recordingToolBarElapseBackBtn.Size = m_minElapseBackSize;
            m_recordingToolBarSectionEndBtn.Size = m_minSectionEndSize;
            m_ToggleProfile.Size = m_minToggleProfileSize;
        }

        private void m_recordingToolBarElapseBackBtn_Click(object sender, EventArgs e)
        {
            m_TransportBar.FastPlayNormaliseWithLapseBack();
            this.Focus();
        }


        private void m_recordingToolBarSectionEndBtn_Click(object sender, EventArgs e)
        {
            if (m_TransportBar.IsPlayerActive)
            {
                m_TransportBar.Pause();
                m_TransportBar.MoveSelectionToPlaybackPhrase();
            }
            if (m_ProjectView.Selection != null && m_ProjectView.GetSelectedPhraseSection.PhraseChildCount > 0) //@singleSection
            {
                SectionNode section = m_ProjectView.GetSelectedPhraseSection;
                m_ProjectView.SelectPhraseBlockOrStrip(section.PhraseChild(section.PhraseChildCount - 1));

            }
            this.Focus();
        }

        private void m_chkMonitorContinuously_CheckedChanged(object sender, EventArgs e)
        {
            if (this.Enabled)
            {
                m_chkMonitorContinuously.CheckedChanged -= new EventHandler(m_chkMonitorContinuously_CheckedChanged);
                Console.WriteLine("Going for manual change " + m_chkMonitorContinuously.Checked);
                if (m_chkMonitorContinuously.Checked)
                {
                    m_TransportBar.MonitorContinuously = true;
                }
                else
                {
                    m_TransportBar.MonitorContinuously = false;
                }
                Console.WriteLine("manual change " + m_chkMonitorContinuously.Checked);
                m_chkMonitorContinuously.CheckedChanged += new EventHandler(m_chkMonitorContinuously_CheckedChanged);
            }
        }

        public void UpdateForChangeInObi()
        {
            UpdateButtons();
            if (m_ProjectView.ObiForm.Settings.Audio_AllowOverwrite
                && m_ProjectView.ObiForm.Settings.Audio_AlwaysMonitorRecordingToolBar)
            {
                if (m_ProjectView.Presentation != null)
                {
                    m_chkMonitorContinuously.Checked = true;
                }
                else
                {
                    m_chkMonitorContinuously.Checked = false;
                }
            }
        }

        private void m_ToggleProfile_Click(object sender, EventArgs e)
        {
            //string ProfileDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            //string ProfileDirectory = m_ProjectView.TransportBar.GetPredefinedProfilesDirectory();
            //string[] filePaths = System.IO.Directory.GetFiles(ProfileDirectory, "*.xml");
            //ProfileDirectory = m_ProjectView.TransportBar.GetCustomProfilesDirectory();
            //if (System.IO.Directory.Exists(ProfileDirectory))
            //{
            //    string[] temp = System.IO.Directory.GetFiles(ProfileDirectory, "*.xml");
            //    string[] tempFilePaths = new string[filePaths.Length + temp.Length];
            //    filePaths.CopyTo(tempFilePaths, 0);
            //    temp.CopyTo(tempFilePaths, filePaths.Length);
            //    filePaths = tempFilePaths;
            //}

            string[] filePaths = m_ProjectView.TransportBar.ProfilesPaths;
             List<string> filePathsList = new List<string>();
            if (filePaths != null && filePaths.Length > 0)
            {
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePathsList.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                }
            }
            string tempSettingsName = m_ProjectView.ObiForm.Settings.SettingsName;

            string[] str = tempSettingsName.Split(new string[] { " profile for" }, StringSplitOptions.None);
            string ProfileName = " ";
            if (m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1 == str[0])
            {
                ProfileName = m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile2;
            }
            else 
            {
                ProfileName = m_ProjectView.ObiForm.Settings.Audio_RecordingToolbarProfile1;
            }
            if (filePathsList.Contains(ProfileName))
            {
                int index = filePathsList.IndexOf(ProfileName);

                m_ProjectView.TransportBar.LoadProfile(filePaths[index], ProfileName);
            }
           // UpdateForChangeInObi();
            if (!m_ProjectView.ObiForm.Settings.Audio_AlwaysMonitorRecordingToolBar && m_chkMonitorContinuously.Checked == true)
            {
                m_chkMonitorContinuously.Checked = false;
            }
            m_StatusLabel.Text = String.Format(Localizer.Message("RecordingToolbar_SwitchProfileStatusBar"), ProfileName);
            this.Focus();
        }

    }
}