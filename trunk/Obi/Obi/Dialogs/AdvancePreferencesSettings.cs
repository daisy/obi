using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AdvancePreferencesSettings : Form
    {
        private Settings m_Settings;
        private bool m_IsProjectTabSelected;

        public AdvancePreferencesSettings(Settings settings, bool IsProjectTabSelected)
        {
            InitializeComponent();
            m_Settings = settings;
            m_IsProjectTabSelected = IsProjectTabSelected;

            UpdateAdvanceSettings();

        }


   

        public void UpdateAdvanceSettings()
        {
            //    m_IsComplete = false;
            m_CheckBoxListView.Columns.Clear();
            m_CheckBoxListView.HeaderStyle = ColumnHeaderStyle.None;
            m_CheckBoxListView.ShowItemToolTips = true;
            m_CheckBoxListView.Scrollable = true;

            m_CheckBoxListView.Columns.Add("", 550, HorizontalAlignment.Left);

          
            m_CheckBoxListView.Items.Clear();

            if (m_IsProjectTabSelected)
            {
                //helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
                //helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                //helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/Project Preferences.htm");
              
                //m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_FixContentViewWidth"));  but remove

                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_OptimizeMemory")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_DisableRollBackForCleanUp")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_CheckForUpdates")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_AlwaysIgnoreIndentationForExportFiles")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_DisableTOCViewCollapse")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_VAXhtmlExport")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_SaveTOCViewWidth"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_MaximumPhrasesSelectLimit"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_ReadOnlyMode"));


                
                //m_CheckBoxListView.Items[2].Checked = m_Settings.Project_LeftAlignPhrasesInContentView;  but remove
                //m_CheckBoxListView.Items[2].ToolTipText = Localizer.Message("ProjectTab_FixContentViewWidth"); but remove

                m_CheckBoxListView.Items[0].Checked = m_Settings.Project_OptimizeMemory; 
                m_CheckBoxListView.Items[0].ToolTipText = Localizer.Message("ProjectTab_OptimizeMemory"); 
                m_CheckBoxListView.Items[1].Checked = m_Settings.Project_DisableRollBackForCleanUp; 
                m_CheckBoxListView.Items[1].ToolTipText = Localizer.Message("Project_DisableRollBackForCleanUp");
                m_CheckBoxListView.Items[2].Checked = m_Settings.Project_CheckForUpdates;  
                m_CheckBoxListView.Items[2].ToolTipText = Localizer.Message("Project_CheckForUpdates"); 
                m_CheckBoxListView.Items[3].Checked = m_Settings.Project_Export_AlwaysIgnoreIndentation;
                m_CheckBoxListView.Items[3].ToolTipText = Localizer.Message("Project_AlwaysIgnoreIndentationForExportFiles"); 
                m_CheckBoxListView.Items[4].Checked = m_Settings.Project_DisableTOCViewCollapse;  
                m_CheckBoxListView.Items[4].ToolTipText = Localizer.Message("Project_DisableTOCViewCollapse"); 
                m_CheckBoxListView.Items[5].Checked = m_Settings.Project_VAXhtmlExport; 
                m_CheckBoxListView.Items[5].ToolTipText = Localizer.Message("Project_VAXhtmlExport"); 
                m_CheckBoxListView.Items[6].Checked = m_Settings.Project_SaveTOCViewWidth; 
                m_CheckBoxListView.Items[6].ToolTipText = Localizer.Message("Project_SaveTOCViewWidth"); 
                m_CheckBoxListView.Items[7].Checked = m_Settings.Project_MaximumPhrasesSelectLimit;
                m_CheckBoxListView.Items[7].ToolTipText = Localizer.Message("Project_MaximumPhrasesSelectLimit");
                m_CheckBoxListView.Items[8].Checked = m_Settings.Project_ReadOnlyMode;
                m_CheckBoxListView.Items[8].ToolTipText = Localizer.Message("Project_ReadOnlyMode"); 
            }
            else
            {   //helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
                //helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
                //helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/Audio Preferences.htm");

                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_RetainInitialSilence")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_PreviewBeforeRecording")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_Limit max phrase duration to 50 minutes")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_MergeFirstTwoPhrasesInPhraseDetection")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_FastPlayWithoutPitchChange")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_PreservePage")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_EnsureCursorVisibilityInUndoOfSplitRecording")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_PreventSplittingPages")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_SelectLastPhrasePlayed"));  
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_RecordInFirstEmptyPhraseWithRecordSectionCommand"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_MergeFirstTwoPhrasesAfterPhraseDetectionWhileRecording")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_AutoPlayAfterRecordingStops")); 
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_RevertOverwriteBehaviourForRecordOnSelection"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_RemoveAccentsFromDaisy2ExportFileNames"));

               
                m_CheckBoxListView.Items[0].Checked = m_Settings.Audio_RetainInitialSilenceInPhraseDetection; 
                m_CheckBoxListView.Items[0].ToolTipText = Localizer.Message("AudioTab_RetainInitialSilence"); 
                m_CheckBoxListView.Items[1].Checked = m_Settings.Audio_Recording_PreviewBeforeStarting; 
                m_CheckBoxListView.Items[1].ToolTipText = Localizer.Message("AudioTab_PreviewBeforeRecording"); 
                m_CheckBoxListView.Items[2].Checked = m_Settings.MaxAllowedPhraseDurationInMinutes == 50; 
                m_CheckBoxListView.Items[2].ToolTipText = Localizer.Message("AudioTab_Limit max phrase duration to 50 minutes"); 
                m_CheckBoxListView.Items[3].Checked = m_Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection; 
                m_CheckBoxListView.Items[3].ToolTipText = Localizer.Message("Audio_MergeFirstTwoPhrasesInPhraseDetection"); 
                m_CheckBoxListView.Items[4].Checked = m_Settings.Audio_FastPlayWithoutPitchChange;
                m_CheckBoxListView.Items[4].ToolTipText = Localizer.Message("Audio_FastPlayWithoutPitchChange");
                m_CheckBoxListView.Items[5].Checked = m_Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio; 
                m_CheckBoxListView.Items[5].ToolTipText = Localizer.Message("Audio_PreservePage");
                m_CheckBoxListView.Items[6].Checked = m_Settings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording;
                m_CheckBoxListView.Items[6].ToolTipText = Localizer.Message("Audio_EnsureCursorVisibilityInUndoOfSplitRecording");
                m_CheckBoxListView.Items[7].Checked = m_Settings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording; 
                m_CheckBoxListView.Items[7].ToolTipText = Localizer.Message("Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording");
                m_CheckBoxListView.Items[8].Checked = m_Settings.Audio_PreventSplittingPages; 
                m_CheckBoxListView.Items[8].ToolTipText = Localizer.Message("Audio_PreventSplittingPages"); 
                m_CheckBoxListView.Items[9].Checked = m_Settings.Audio_SelectLastPhrasePlayed; 
                m_CheckBoxListView.Items[9].ToolTipText = Localizer.Message("Audio_SelectLastPhrasePlayed"); 
                m_CheckBoxListView.Items[10].Checked = m_Settings.Audio_RecordInFirstEmptyPhraseWithRecordSectionCommand; 
                m_CheckBoxListView.Items[10].ToolTipText = Localizer.Message("Audio_RecordInFirstEmptyPhraseWithRecordSectionCommand"); 
                m_CheckBoxListView.Items[11].Checked = m_Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetectionWhileRecording; 
                m_CheckBoxListView.Items[11].ToolTipText = Localizer.Message("Audio_MergeFirstTwoPhrasesAfterPhraseDetectionWhileRecording"); 
                m_CheckBoxListView.Items[12].Checked = m_Settings.Audio_AutoPlayAfterRecordingStops; 
                m_CheckBoxListView.Items[12].ToolTipText = Localizer.Message("Audio_AutoPlayAfterRecordingStops"); 
                m_CheckBoxListView.Items[13].Checked = m_Settings.Audio_RevertOverwriteBehaviourForRecordOnSelection; 
                m_CheckBoxListView.Items[13].ToolTipText = Localizer.Message("Audio_RevertOverwriteBehaviourForRecordOnSelection");
                m_CheckBoxListView.Items[14].Checked = m_Settings.Audio_RemoveAccentsFromDaisy2ExportFileNames;
                m_CheckBoxListView.Items[14].ToolTipText = Localizer.Message("Audio_RemoveAccentsFromDaisy2ExportFileNames"); 

            }

            m_CheckBoxListView.View = View.Details;
        }



        public void UpdateBoolSettings()
        {
            if (m_IsProjectTabSelected)
            {
               
                //m_Settings.Project_LeftAlignPhrasesInContentView = m_CheckBoxListView.Items[8].Checked ? m_CheckBoxListView.Items[2].Checked : false; // false if waveform is disabled

                m_Settings.Project_OptimizeMemory = m_CheckBoxListView.Items[0].Checked; // do not remove
                m_Settings.Project_DisableRollBackForCleanUp = m_CheckBoxListView.Items[1].Checked;
                m_Settings.Project_CheckForUpdates = m_CheckBoxListView.Items[2].Checked; 
                m_Settings.Project_Export_AlwaysIgnoreIndentation = m_CheckBoxListView.Items[3].Checked; 
                m_Settings.Project_DisableTOCViewCollapse = m_CheckBoxListView.Items[4].Checked;  
                m_Settings.Project_VAXhtmlExport = m_CheckBoxListView.Items[5].Checked; 
                m_Settings.Project_SaveTOCViewWidth = m_CheckBoxListView.Items[6].Checked; 
                m_Settings.Project_MaximumPhrasesSelectLimit = m_CheckBoxListView.Items[7].Checked;
                m_Settings.Project_ReadOnlyMode = m_CheckBoxListView.Items[8].Checked; 
            }
            else
            {
                m_Settings.Audio_RetainInitialSilenceInPhraseDetection = m_CheckBoxListView.Items[0].Checked; 
                m_Settings.Audio_Recording_PreviewBeforeStarting = m_CheckBoxListView.Items[1].Checked; 
                m_Settings.MaxAllowedPhraseDurationInMinutes = (uint)(m_CheckBoxListView.Items[2].Checked ? 50 : 180);  
                m_Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = m_CheckBoxListView.Items[3].Checked;
                m_Settings.Audio_FastPlayWithoutPitchChange = m_CheckBoxListView.Items[4].Checked; 
                m_Settings.Audio_PreservePagesWhileRecordOverSubsequentAudio = m_CheckBoxListView.Items[5].Checked; 
                m_Settings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording = m_CheckBoxListView.Items[6].Checked; 
                m_Settings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording = m_CheckBoxListView.Items[7].Checked; 
                m_Settings.Audio_PreventSplittingPages = m_CheckBoxListView.Items[8].Checked; 
                m_Settings.Audio_SelectLastPhrasePlayed = m_CheckBoxListView.Items[9].Checked; 
                m_Settings.Audio_RecordInFirstEmptyPhraseWithRecordSectionCommand = m_CheckBoxListView.Items[10].Checked; 
                m_Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetectionWhileRecording = m_CheckBoxListView.Items[11].Checked; 
                m_Settings.Audio_AutoPlayAfterRecordingStops = m_CheckBoxListView.Items[12].Checked; 
                m_Settings.Audio_RevertOverwriteBehaviourForRecordOnSelection = m_CheckBoxListView.Items[13].Checked;
                m_Settings.Audio_RemoveAccentsFromDaisy2ExportFileNames = m_CheckBoxListView.Items[14].Checked;

            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            UpdateBoolSettings();
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
