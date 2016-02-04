using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using AudioLib;
using Obi.Audio;

namespace Obi.Dialogs
    {
    public partial class Preferences : Form
        {
        private ObiForm mForm;                           // parent form
        private Settings mSettings;                      // the settings to modify
        private bool mCanChangeAudioSettings;            // if the settings come from the project they cannot change
        private ObiPresentation mPresentation;              // current presentation (may be null)
        private ProjectView.TransportBar mTransportBar;  // application transport bar
        private KeyboardShortcuts_Settings m_KeyboardShortcuts;
        private bool m_IsKeyboardShortcutChanged = false;
        private int m_Nudge = 200;
        private int m_Preview = 1500;
        private int m_Elapse = 2500;
        private bool m_AutoSave;
        private bool m_SaveBookmarkNode;
        private bool m_OpenLastProject;
        private bool m_IsComplete = false;
        private string m_lblShortcutKeys_text ; //workaround for screen reader response, will be removed in future
        private Settings m_DefaultSettings;
        private Dictionary <string,string> m_KeyboardShortcutReadableNamesMap = new Dictionary<string,string> () ;
        private bool m_IsColorChanged = false;
        private double m_DefaultGap = 300.0;
        private double m_DefaultLeadingSilence = 50.0;
        private double m_DefaultThreshold = 280.0;
      //  private bool m_FlagComboBoxIndexChange = false;
       // private int m_IndexOfLevelCombox = 0;
        private bool m_UpdateBackgroundColorRequired = false;
        private int m_Audio_CleanupMaxFileSizeInMB = 100;
      //  private bool m_IsComboBoxExpanded = false;
      //  private bool m_LeaveOnEscape = false;

        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences ( ObiForm form, Settings settings, ObiPresentation presentation, ProjectView.TransportBar transportbar, Settings defaultSettings)
            {
            InitializeComponent ();
            mForm = form;
            mSettings = settings;
            mPresentation = presentation;
            mTransportBar = transportbar;
            m_KeyboardShortcuts = mForm.KeyboardShortcuts;
            InitializeProjectTab ();
            InitializeAudioTab ();
            InitializeUserProfileTab ();
            InitializeKeyboardShortcutsTab();
            InitializeColorPreferenceTab();
            InitializePreferencesProfileTab ();
            m_IsKeyboardShortcutChanged = false;
            this.m_CheckBoxListView.BringToFront();
            m_DefaultSettings = defaultSettings;
            //m_IndexOfLevelCombox = mSettings.Audio_LevelComboBoxIndex;
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Basic"));
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Intermediate"));
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Advance"));
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Profile_1"));
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Profile_2"));
            //m_ComboSelectAudioProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Custom"));
            m_Preference_ToolTip.SetToolTip(m_btnProfileDiscription, Localizer.Message("Preferences_AudioProfileDesc"));            
           }

        public bool IsColorChanged
        { get { return m_IsColorChanged; } }

        // Initialize the project tab
        private void InitializeProjectTab ()
            {
            UpdateTabControl();
            mDirectoryTextbox.Text = mSettings.Project_DefaultPath;

            
         //   mLastOpenCheckBox.Checked = mSettings.OpenLastProject;
            m_ChkAutoSaveInterval.CheckStateChanged -= new System.EventHandler ( this.m_ChkAutoSaveInterval_CheckStateChanged );
            m_ChkAutoSaveInterval.Checked = mSettings.Project_AutoSaveTimeIntervalEnabled;
            m_ChkAutoSaveInterval.CheckStateChanged += new System.EventHandler ( this.m_ChkAutoSaveInterval_CheckStateChanged );
            int intervalMinutes = Convert.ToInt32 ( mSettings.Project_AutoSaveTimeInterval / 60000 );
            MnumAutoSaveInterval.Value = intervalMinutes;
            MnumAutoSaveInterval.Enabled = m_ChkAutoSaveInterval.Checked;
          //  mChkAutoSaveOnRecordingEnd.Checked = mSettings.AutoSave_RecordingEnd;
            mPipelineTextbox.Text = mSettings.Project_PipelineScriptsPath;
            if (!string.IsNullOrEmpty(mSettings.Audio_LocalRecordingDirectory)
                && System.IO.Directory.Exists(mSettings.Audio_LocalRecordingDirectory))
            {
                m_chkRecordInLocalDrive.Checked = true;
                m_txtRecordInLocalDrive.Enabled = true;
                m_txtRecordInLocalDrive.Text = mSettings.Audio_LocalRecordingDirectory;
            }
            else
            {
                m_chkRecordInLocalDrive.Checked = false;
                m_txtRecordInLocalDrive.Enabled = false;
                m_txtRecordInLocalDrive.Text = "";
            }

            m_NumImportTolerance.Value = (decimal) mSettings.Project_ImportToleranceForAudioInMs;

            }

        // Initialize audio tab
        private void InitializeAudioTab ()
            {
                try
                {
                    AudioRecorder recorder = mTransportBar.Recorder;
                    string defaultInputName = "";
                    string defaultOutputName = "";
                    mInputDeviceCombo.Items.Clear();
                    mOutputDeviceCombo.Items.Clear();
                    // mInputDeviceCombo.DataSource = recorder.InputDevices; 
                    // mInputDeviceCombo.SelectedIndex = recorder.InputDevices.IndexOf ( recorder.InputDevice ); 
                    foreach (InputDevice input in recorder.InputDevices)
                    {
                        mInputDeviceCombo.Items.Add(input.Name);
                        if (recorder.InputDevice.Name == input.Name)
                            defaultInputName = input.Name;
                    }
                    if (mSettings.Audio_LastInputDevice == "")
                        mInputDeviceCombo.SelectedIndex = 0;
                    //  mInputDeviceCombo.SelectedItem = defaultInputName;
                    else
                        mInputDeviceCombo.SelectedIndex = mInputDeviceCombo.Items.IndexOf(mSettings.Audio_LastInputDevice);


                    AudioPlayer player = mTransportBar.AudioPlayer;
                    // mOutputDeviceCombo.DataSource = player.OutputDevices; avn
                    // mOutputDeviceCombo.SelectedIndex = player.OutputDevices.IndexOf ( player.OutputDevice ); avn

                    foreach (OutputDevice output in player.OutputDevices)
                    {
                        mOutputDeviceCombo.Items.Add(output.Name);
                        if (player.OutputDevice.Name == output.Name)
                            defaultOutputName = output.Name;
                    }
                    if (mSettings.Audio_LastOutputDevice == "")
                        mOutputDeviceCombo.SelectedIndex = 0;
                    //mOutputDeviceCombo.SelectedItem = defaultOutputName;
                    else
                        mOutputDeviceCombo.SelectedIndex = mOutputDeviceCombo.Items.IndexOf(mSettings.Audio_LastOutputDevice);
                    
                    int sampleRate;
                    int audioChannels;
                    if (mPresentation != null)
                    {
                        sampleRate = (int)mPresentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate;
                        audioChannels = mPresentation.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels;
                        mCanChangeAudioSettings = mPresentation.MediaDataManager.ManagedObjects.Count == 0;
                    }
                    else
                    {
                        sampleRate = mSettings.Audio_SampleRate;
                        audioChannels = mSettings.Audio_Channels;
                        mCanChangeAudioSettings = true;
                    }
                    ArrayList sampleRates = new ArrayList();
                    // TODO: replace this with a list obtained from the player or the device
                    sampleRates.Add("11025");
                    sampleRates.Add("22050");
                    sampleRates.Add("44100");
                    sampleRates.Add("48000");
                    mSampleRateCombo.DataSource = sampleRates;
                    mSampleRateCombo.SelectedIndex = sampleRates.IndexOf(sampleRate.ToString());
                    mSampleRateCombo.Visible = mCanChangeAudioSettings;
                    mSampleRateTextbox.Text = sampleRate.ToString();
                    mSampleRateTextbox.Visible = !mCanChangeAudioSettings;
                    ArrayList channels = new ArrayList();
                    channels.Add(Localizer.Message("mono"));
                    channels.Add(Localizer.Message("stereo"));
                    mChannelsCombo.DataSource = channels;
                    mChannelsCombo.SelectedIndex = channels.IndexOf(Localizer.Message(audioChannels == 1 ? "mono" : "stereo"));
                    mChannelsCombo.Visible = mCanChangeAudioSettings;
                    mChannelsTextbox.Text = Localizer.Message(audioChannels == 1 ? "mono" : "stereo");
                    mChannelsTextbox.Visible = !mCanChangeAudioSettings;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }

            try
            {
                List<string> installedTTSVoices = AudioFormatConverter.InstalledTTSVoices;
                
                if (installedTTSVoices == null || installedTTSVoices.Count == 0)
                {
                    AudioFormatConverter.InitializeTTS(mSettings, mPresentation != null ? mPresentation.MediaDataManager.DefaultPCMFormat.Data : new AudioLibPCMFormat((ushort)mSettings.Audio_Channels, (uint)mSettings.Audio_SampleRate, (ushort)mSettings.Audio_BitDepth));
                }
                installedTTSVoices = AudioFormatConverter.InstalledTTSVoices;
                if (installedTTSVoices != null && installedTTSVoices.Count > 0)
                {
                    mTTSvoiceCombo.Items.AddRange(installedTTSVoices.ToArray());
                    if (string.IsNullOrEmpty(mSettings.Audio_TTSVoice))
                    {
                        if (mTTSvoiceCombo.Items.Count > 0) mTTSvoiceCombo.SelectedIndex = 0;
                    }
                    else
                    {
                        int ttsIndex = installedTTSVoices.IndexOf(mSettings.Audio_TTSVoice);
                        if (ttsIndex >= 0) mTTSvoiceCombo.SelectedIndex = ttsIndex;
                    }
                }
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            m_Nudge = (int)(mSettings.Audio_NudgeTimeMs);
            m_Preview = (int)(mSettings.Audio_PreviewDuration);
            m_Elapse = (int)(mSettings.Audio_ElapseBackTimeInMilliseconds);
            m_DefaultLeadingSilence = (double)(mSettings.Audio_DefaultLeadingSilence);
            m_DefaultThreshold = (double)(mSettings.Audio_DefaultThreshold);
            m_DefaultGap = (double)(mSettings.Audio_DefaultGap);

            mNoiseLevelComboBox.SelectedIndex =
                mSettings.Audio_NoiseLevel == AudioLib.VuMeter.NoiseLevelSelection.Low ? 0 :
                mSettings.Audio_NoiseLevel == AudioLib.VuMeter.NoiseLevelSelection.Medium ? 1 : 2;
        //    mAudioCluesCheckBox.Checked = mSettings.AudioClues;
            m_cbOperation.SelectedIndex = 0;
            m_CleanUpFileSizeNumericUpDown.Value = (decimal)mSettings.Audio_CleanupMaxFileSizeInMB;
            if (this.mTab.SelectedTab == mAudioTab)
            {
                m_IsComplete = false;
                m_CheckBoxListView.Items[0].Checked = mSettings.Audio_AudioClues;
                m_CheckBoxListView.Items[1].Checked = mSettings.Audio_RetainInitialSilenceInPhraseDetection;
                m_CheckBoxListView.Items[2].Checked = mSettings.Audio_Recording_PreviewBeforeStarting;
                m_CheckBoxListView.Items[3].Checked = mSettings.Audio_UseRecordingPauseShortcutForStopping;
                m_CheckBoxListView.Items[4].Checked = mSettings.Audio_AllowOverwrite;
                m_CheckBoxListView.Items[5].Checked = mSettings.Audio_RecordDirectlyWithRecordButton;
                m_CheckBoxListView.Items[6].Checked = mSettings.MaxAllowedPhraseDurationInMinutes == 50 ;
                m_CheckBoxListView.Items[7].Checked = mSettings.Audio_ShowLiveWaveformWhileRecording;
                m_CheckBoxListView.Items[8].Checked = mSettings.Audio_EnableLivePhraseDetection;
                m_CheckBoxListView.Items[9].Checked = mSettings.Audio_EnablePostRecordingPageRenumbering;
                m_CheckBoxListView.Items[10].Checked = mSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
                m_CheckBoxListView.Items[11].Checked = mSettings.Audio_FastPlayWithoutPitchChange;
                m_CheckBoxListView.Items[12].Checked = mSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio;
                m_CheckBoxListView.Items[13].Checked = mSettings.Audio_EnforceSingleCursor;
                m_CheckBoxListView.Items[14].Checked = mSettings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording;
                m_CheckBoxListView.Items[15].Checked = mSettings.Audio_DisableDeselectionOnStop;
                m_CheckBoxListView.Items[16].Checked = mSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio;
                m_CheckBoxListView.Items[17].Checked = mSettings.Audio_AlwaysMonitorRecordingToolBar;
                m_CheckBoxListView.Items[18].Checked = mSettings.Audio_ColorFlickerPreviewBeforeRecording;
                m_CheckBoxListView.Items[19].Checked = mSettings.Audio_PlaySectionUsingPlayBtn;
                m_CheckBoxListView.Items[20].Checked = mSettings.Audio_EnableFileDataProviderPreservation;
                m_CheckBoxListView.Items[21].Checked = mSettings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording;
                m_CheckBoxListView.Items[22].Checked = mSettings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording;
                m_CheckBoxListView.Items[23].Checked = mSettings.Audio_PreventSplittingPages;
                m_CheckBoxListView.Items[24].Checked = mSettings.Audio_SaveAudioZoom;
                m_IsComplete = true;

            }
            }

        // Initialize user profile preferences
        private void InitializeUserProfileTab ()
            {
            mFullNameTextbox.Text = mSettings.UserProfile.Name;
            mOrganizationTextbox.Text = mSettings.UserProfile.Organization;
            mCultureBox.Items.AddRange ( CultureInfo.GetCultures ( CultureTypes.AllCultures ) );
            mCultureBox.SelectedItem = mSettings.UserProfile.Culture;
            }

        private void InitializeKeyboardShortcutsTab()
        {
            m_KeyboardShortcutReadableNamesMap.Clear () ;
            m_KeyboardShortcutReadableNamesMap.Add ("Selected item", "Apply phrase detection on selected item") ;
            m_KeyboardShortcutReadableNamesMap.Add ("Multiple sections", "Apply phrase detection on multiple sections") ;
            m_KeyboardShortcutReadableNamesMap.Add("Page / Phrase/ Time", "Go to Page/Phrase/Time");
            m_cbShortcutKeys.SelectedIndex = 0;
        //    m_lvShortcutKeysList.Clear();
            string[] tempArray = new string[2];
            foreach (string desc in m_KeyboardShortcuts.KeyboardShortcutsDescription.Keys)
            {
                tempArray[0] = desc;
                if (!m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].IsMenuShortcut)
                {
                    tempArray[1] = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                    ListViewItem item = new ListViewItem(tempArray);
                    m_lvShortcutKeysList.Items.Add(item);
                }
            }
            if (!string.IsNullOrEmpty(m_lblShortcutKeys.Text)) m_lblShortcutKeys_text = m_lblShortcutKeys.Text;
            if (mTab.SelectedTab != mKeyboardShortcutTab) m_lblShortcutKeys.Text = "";
        }

        private void InitializeColorPreferenceTab()
        {
            //mNormalColorCombo.Items.AddRange(new object[] (GetType (System.Drawing.Color ) ;
            
            System.Reflection.PropertyInfo[] colorProperties = typeof(System.Drawing.Color).GetProperties(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public);
    
             System.Reflection.PropertyInfo[] systemColorProperties = typeof(System.Drawing.SystemColors).GetProperties();

             foreach (System.Reflection.PropertyInfo p in colorProperties)
             {
                 System.Drawing.Color col = System.Drawing.Color.FromName(p.Name);
                 if (col != System.Drawing.Color.Transparent) mNormalColorCombo.Items.Add(col);
             }

             foreach (System.Reflection.PropertyInfo p in systemColorProperties)
             {   
                 System.Drawing.ColorConverter c = new ColorConverter ();
                 object col = c.ConvertFromString (p.Name) ;
                                                   mNormalColorCombo.Items.Add(col);
             }
             mNormalColorCombo.SelectedIndex = 0;

             System.Drawing.Text.InstalledFontCollection fonts = new System.Drawing.Text.InstalledFontCollection();
             foreach (FontFamily family in fonts.Families)
             {
                 mChooseFontCombo.Items.Add(family.Name);
             }
             mChooseFontCombo.SelectedIndex = 0;
            //mNormalColorCombo.Items.AddRange(new object[] { Color.Orange, Color.LightSkyBlue, Color.LightGreen, Color.LightSalmon,
               //SystemColors.Window, Color.Purple, SystemColors.Highlight, Color.Red,Color.BlueViolet, SystemColors.ControlDark,
               //SystemColors.HighlightText, SystemColors.ControlText, SystemColors.ControlText, SystemColors.ControlText,
               //SystemColors.ControlText, SystemColors.HighlightText, SystemColors.HighlightText, Color.Yellow,
               //SystemColors.HighlightText, SystemColors.HighlightText, SystemColors.Highlight, SystemColors.AppWorkspace,
               //SystemColors.Window, SystemColors.Control, SystemColors.Control, SystemColors.Highlight, SystemColors.ControlText,
               //SystemColors.Highlight, SystemColors.HighlightText, SystemColors.ControlDark, SystemColors.ControlText,
               //SystemColors.GradientActiveCaption, SystemColors.Window, SystemColors.ControlText, SystemColors.InactiveCaptionText,
               //SystemColors.ControlText, SystemColors.Control, Color.Azure, SystemColors.ControlText, SystemColors.Window, SystemColors.ControlText,
               //SystemColors.Highlight, SystemColors.HighlightText, 
               //SystemColors.Highlight, Color.Red
            //});
            mHighContrastCombo.Items.AddRange(new object[] { SystemColors.Window, SystemColors.AppWorkspace, SystemColors.InactiveCaptionText, SystemColors.GradientActiveCaption, SystemColors.Highlight, SystemColors.HighlightText, SystemColors.ControlDark, SystemColors.Control, SystemColors.ControlText, Color.DarkSlateGray, Color.Green, Color.Yellow});

            mHighContrastCombo.SelectedIndex = 0;
            mSettings.ColorSettings.PopulateColorSettingsDictionary();
            LoadListViewWithColors();
        }        

        /// <summary>
        /// Browse for a project directory.
        /// </summary>
        private void mBrowseButton_Click ( object sender, EventArgs e )
            {
            SelectFolder ( mSettings.Project_DefaultPath, "default_directory_browser", mDirectoryTextbox );
            }

        private void mPipelineBrowseButton_Click ( object sender, EventArgs e )
            {
            SelectFolder ( mSettings.Project_PipelineScriptsPath, "pipeline_path_browser", mPipelineTextbox );
            }


        private void m_RecordInLocalDriveButton_Click(object sender, EventArgs e)
        {
            SelectFolder(mSettings.Audio_LocalRecordingDirectory, "LocalDirectoryToSaveRecording", m_txtRecordInLocalDrive);
        }

        private void SelectFolder ( string path, string description, TextBox textBox )
            {
            FolderBrowserDialog dialog = new FolderBrowserDialog ();
            dialog.SelectedPath = path;
            dialog.Description = Localizer.Message ( description );
            if (dialog.ShowDialog () == DialogResult.OK) textBox.Text = dialog.SelectedPath;
            }

        // Update settings
        private void mOKButton_Click(object sender, EventArgs e)
        {
         
            UpdateBoolSettings();
            if (this.mTab.SelectedTab == mKeyboardShortcutTab)
            {
                AssignKeyboardShortcut();
            }
            if (this.mTab.SelectedTab == mColorPreferencesTab)
            {
                ApplyColorPreference();
            }
            if (UpdateProjectSettings()
            && UpdateAudioSettings()
            && UpdateUserProfile())
            {
                if (m_IsKeyboardShortcutChanged)
                {
                    mForm.KeyboardShortcuts.SaveSettings();
                    mForm.InitializeKeyboardShortcuts(false);
                }
                //   UpdateColorSettings();
                DialogResult = DialogResult.OK;
                Close();
                
            }
            else
            {
                return;
            }
            
        }

        // Update project settings
        private bool UpdateProjectSettings ()
            {
            
            bool returnVal = true;
            string [] getFiles = null;
            string[] logicalDrives = System.IO.Directory.GetLogicalDrives();

            if (System.IO.Directory.Exists ( mDirectoryTextbox.Text )
                && System.IO.Directory.Exists ( mPipelineTextbox.Text ))
                {
                foreach (string drive in logicalDrives)
                        {
                            if (mPipelineTextbox.Text == drive || mPipelineTextbox.Text == Environment.GetEnvironmentVariable("windir").ToString() || mPipelineTextbox.Text == Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) )
                            {
                                MessageBox.Show(Localizer.Message("Preferences_PipelineScriptNotFound"));
                                return false;
                            }
                            else
                                getFiles = System.IO.Directory.GetFiles(mPipelineTextbox.Text, "*.taskScript", System.IO.SearchOption.AllDirectories);
                        }
                   
                if (ObiForm.CheckProjectDirectory_Safe ( mDirectoryTextbox.Text, false ))
                    mSettings.Project_DefaultPath = mDirectoryTextbox.Text;
                if (getFiles.Length > 0)
                    mSettings.Project_PipelineScriptsPath = mPipelineTextbox.Text;
                else
                {
                    MessageBox.Show(Localizer.Message("Preferences_PipelineScriptNotFound"));
                    returnVal = false;
                }
                if (m_chkRecordInLocalDrive.Checked)
                {
                    if (m_txtRecordInLocalDrive.Text != string.Empty)
                    {
                        mSettings.Audio_LocalRecordingDirectory = m_txtRecordInLocalDrive.Text;
                    }
                    else
                    {
                        mSettings.Audio_LocalRecordingDirectory = "";
                    }
                }
                else
                {
                    mSettings.Audio_LocalRecordingDirectory = string.Empty;
                }
             }

            else
                {
                if(!System.IO.Directory.Exists(mDirectoryTextbox.Text))
                MessageBox.Show ( Localizer.Message ( "InvalidPaths" ) + " : " + mDirectoryTextbox.Text, Localizer.Message ( "Caption_Error" ));
                else if (!System.IO.Directory.Exists(mPipelineTextbox.Text))
                MessageBox.Show(Localizer.Message("InvalidPaths") + " : " + mPipelineTextbox.Text, Localizer.Message("Caption_Error"));
                returnVal = false;
                }
                
          //  mSettings.OpenLastProject = mLastOpenCheckBox.Checked;
          //  mSettings.AutoSave_RecordingEnd = mChkAutoSaveOnRecordingEnd.Checked;
          //  mSettings.AutoSaveTimeIntervalEnabled = m_ChkAutoSaveInterval.Checked;
            try
                {
                mSettings.Project_AutoSaveTimeInterval = Convert.ToInt32 ( MnumAutoSaveInterval.Value * 60000 );
                mSettings.Project_ImportToleranceForAudioInMs = (int) m_NumImportTolerance.Value;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                returnVal = false;
                }
            mForm.SetAutoSaverInterval = mSettings.Project_AutoSaveTimeInterval;
            if (mSettings.Project_AutoSaveTimeIntervalEnabled) mForm.StartAutoSaveTimeInterval ();
            VerifyChangeInLoadedSettings();
            return returnVal;
            }

        // Update audio settings
        private bool UpdateAudioSettings()
        {
            AudioRecorder recorder = mTransportBar.Recorder;
            AudioPlayer player = mTransportBar.AudioPlayer;

            //  try
            {
                //  mTransportBar.AudioPlayer.SetOutputDevice( mForm, ((OutputDevice)mOutputDeviceCombo.SelectedItem).Name );  avn
                //  mTransportBar.Recorder.InputDevice = (InputDevice)mInputDeviceCombo.SelectedItem;  

                foreach (InputDevice inputDev in recorder.InputDevices)
                {
                    if (mInputDeviceCombo.SelectedItem != null && mInputDeviceCombo.SelectedItem.ToString() == inputDev.Name)
                    {
                        mTransportBar.Recorder.SetInputDevice(inputDev.Name);
                        mSettings.Audio_LastInputDevice = inputDev.Name;
                    }
                }
                foreach (OutputDevice outputDev in player.OutputDevices)
                {
                    if (mOutputDeviceCombo.SelectedItem != null && mOutputDeviceCombo.SelectedItem.ToString() == outputDev.Name)
                    {
                        mTransportBar.AudioPlayer.SetOutputDevice(mForm, (outputDev.Name));
                        mSettings.Audio_LastOutputDevice = outputDev.Name;
                    }
                }
                //  mSettings.LastInputDevice = ((InputDevice)mInputDeviceCombo.SelectedItem).Name; 
                //  mSettings.LastOutputDevice = ((OutputDevice)mOutputDeviceCombo.SelectedItem).Name;

            }

            /*   catch (System.Exception ex)
                   {
                   MessageBox.Show ( ex.ToString () );
                   return false;
                   }*/
            if (mCanChangeAudioSettings)
            {
                mSettings.Audio_Channels = mChannelsCombo.SelectedItem.ToString() == Localizer.Message("mono") ? 1 : 2;
                mSettings.Audio_SampleRate = Convert.ToInt32(mSampleRateCombo.SelectedItem);
                if (mPresentation != null)
                {
                    if (!mPresentation.UpdatePresentationAudioProperties(mSettings.Audio_Channels, mSettings.Audio_BitDepth, mSettings.Audio_SampleRate))
                    {
                        MessageBox.Show(Localizer.Message("Preferences_UnableToUpdateProjectAudioFormat"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }

            string selectedTTSVoice = GetTTSVoiceNameFromTTSCombo();
            if (!string.IsNullOrEmpty(selectedTTSVoice)) mSettings.Audio_TTSVoice = selectedTTSVoice;

            mSettings.Audio_NoiseLevel = mNoiseLevelComboBox.SelectedIndex == 0 ? AudioLib.VuMeter.NoiseLevelSelection.Low :
                mNoiseLevelComboBox.SelectedIndex == 1 ? AudioLib.VuMeter.NoiseLevelSelection.Medium : AudioLib.VuMeter.NoiseLevelSelection.High;

            try
            {
                mSettings.Audio_NudgeTimeMs = (int)m_Nudge;
                mSettings.Audio_PreviewDuration = (int)m_Preview;
                mSettings.Audio_ElapseBackTimeInMilliseconds = (int)m_Elapse;
                mSettings.Audio_DefaultLeadingSilence = (decimal)m_DefaultLeadingSilence;
                mSettings.Audio_DefaultThreshold = (decimal)m_DefaultThreshold;
                mSettings.Audio_DefaultGap = (decimal)m_DefaultGap;
                mSettings.Audio_CleanupMaxFileSizeInMB = m_Audio_CleanupMaxFileSizeInMB;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return false;
            }

            VerifyChangeInLoadedSettings();
            return true;
        }

        private string GetTTSVoiceNameFromTTSCombo()
        {
            int ttsIndex = mTTSvoiceCombo.SelectedIndex;
            if (ttsIndex >= 0 && AudioFormatConverter.InstalledTTSVoices.Count > 0)
            {
                return AudioFormatConverter.InstalledTTSVoices[ttsIndex];
            }
            return "";
        }

        // Update user profile
        private bool UpdateUserProfile ()
            {
            mSettings.UserProfile.Name = mFullNameTextbox.Text;
            mSettings.UserProfile.Organization = mOrganizationTextbox.Text;

            if (mCultureBox.SelectedItem.ToString () == "en-US"
                || IsResourceForLanguageExist(mCultureBox.SelectedItem.ToString () ) )
                //|| mCultureBox.SelectedItem.ToString () == "hi-IN"
                //|| mCultureBox.SelectedItem.ToString () == "fr-FR")
                {
                    if (mSettings.UserProfile.Culture.ToString() != mCultureBox.SelectedItem.ToString()
                        || mCultureBox.SelectedItem.ToString() != System.Globalization.CultureInfo.CurrentCulture.ToString())
                    {
                        MessageBox.Show(Localizer.Message("Preferences_RestartForCultureChange"));
                    }
                }
            else if (mSettings.UserProfile.Culture.Name != ((CultureInfo)mCultureBox.SelectedItem).Name)
                {
                // show this message only if selected culture is different from application's existing culture.
                MessageBox.Show ( string.Format ( Localizer.Message ( "Peferences_GUIDonotSupportCulture" ),
                    mCultureBox.SelectedItem.ToString () ) );
                }
                if (mCultureBox.SelectedItem.ToString() != "zh-CHT" && mCultureBox.SelectedItem.ToString() != "zh-TW"
                    && mCultureBox.SelectedItem.ToString() != "zh-CHS")
                {
                    mSettings.UserProfile.Culture = (CultureInfo)mCultureBox.SelectedItem;
                }
                else
                {
                    if (mCultureBox.SelectedItem.ToString() == "zh-CHT" || mCultureBox.SelectedItem.ToString() == "zh-TW")
                    {
                        CultureInfo temp = new CultureInfo("zh-HK");
                        mSettings.UserProfile.Culture = temp;
                    }
                    else if (mCultureBox.SelectedItem.ToString() == "zh-CHS")
                    {
                        CultureInfo temp = new CultureInfo("zh-CN");
                        mSettings.UserProfile.Culture = temp;
                    }
                }
                VerifyChangeInLoadedSettings();
            return true;
            }

        private bool IsResourceForLanguageExist(string cultureName)
        {
            string cultureDirName = "";
            if (cultureName != "zh-HK"  && cultureName != "zh-TW" && cultureName != "zh-CHT" && cultureName != "zh-CN" && cultureName!="zh-CHS")
            {
                cultureDirName = cultureName.Split('-')[0];
            }
            else if (cultureName != "zh-CN" && cultureName != "zh-CHS")
            {
                cultureDirName = "zh-HK";
            }
            else 
            {
                cultureDirName = "zh-CN";
            }
                  
            string[] dirList = System.IO.Directory.GetDirectories(System.AppDomain.CurrentDomain.BaseDirectory, cultureDirName, System.IO.SearchOption.TopDirectoryOnly);
            return dirList != null && dirList.Length > 0;
            
        }

        private void m_ChkAutoSaveInterval_CheckStateChanged ( object sender, EventArgs e )
            {
            MnumAutoSaveInterval.Enabled = m_ChkAutoSaveInterval.Checked;
            }

        private void m_lvShortcutKeysList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowSelectedShortcutKeyInKeyboardShortcutTextbox();
        }

        private void ShowSelectedShortcutKeyInKeyboardShortcutTextbox ()
    {
            if (m_lvShortcutKeysList.SelectedIndices.Count > 0 && m_lvShortcutKeysList.SelectedIndices[0] >= 0)
            {
                string desc = m_lvShortcutKeysList.Items[m_lvShortcutKeysList.SelectedIndices[0]].Text;
                m_txtShortcutKeys.Text = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                m_txtShortcutKeys.AccessibleName = string.Format(Localizer.Message("KeyboardShortcuts_TextboxAccessibleName"),m_KeyboardShortcutReadableNamesMap.ContainsKey (desc)? m_KeyboardShortcutReadableNamesMap[desc]: desc );
            }
        }

        private Keys m_CapturedKey ;
        
        private void m_txtShortcutKeys_KeyDown(object sender, KeyEventArgs e)
        {
            if (m_txtShortcutKeys.ContainsFocus && e.KeyData != Keys.Tab && e.KeyData != (Keys.Shift | Keys.Tab)
    && e.KeyData != Keys.Shift && e.KeyData != Keys.ShiftKey && e.KeyData != (Keys.ShiftKey | Keys.Shift)
    && e.KeyData != Keys.Control && e.KeyData != (Keys.ControlKey | Keys.Control)
    && e.KeyData != (Keys.Control | Keys.A) && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.ControlKey
                && e.KeyData != Keys.Return)
            {
                m_CapturedKey = e.KeyData;
            }
        }

        private void m_txtShortcutKeys_KeyUp(object sender, KeyEventArgs e)
        {
            if (m_CapturedKey != Keys.None)
            {
                m_txtShortcutKeys.Text = m_CapturedKey.ToString();
                m_txtShortcutKeys.SelectAll();
                if (e.KeyData == Keys.Enter)
                {
                    AssignKeyboardShortcut();
                    e.Handled = true;
                }
            }
        }

        private void m_txtShortcutKeys_Enter(object sender, EventArgs e)
        {
            m_txtShortcutKeys.SelectAll();
            this.AcceptButton = null;
        }
        
        private void m_btnAssign_Click(object sender, EventArgs e)
        {
            AssignKeyboardShortcut();
        }

        private void AssignKeyboardShortcut ()
    {
            if (m_lvShortcutKeysList.SelectedIndices.Count > 0 && m_lvShortcutKeysList.SelectedIndices[0] >= 0 && m_CapturedKey != Keys.None)
            {
                if( m_KeyboardShortcuts.IsDuplicate(m_CapturedKey))
                {
                    MessageBox.Show(Localizer.Message("KeyboardShortcut_DuplicateMessage"), Localizer.Message ("Caption_Error"));
                    m_CapturedKey = Keys.None;
                    ShowSelectedShortcutKeyInKeyboardShortcutTextbox();
                    m_txtShortcutKeys.Focus();
                    return;
                }
                if ( !IsValidMenuShortcut ( m_CapturedKey)) return ;
                ListViewItem selectedItem = m_lvShortcutKeysList.Items[m_lvShortcutKeysList.SelectedIndices[0]] ;
                                string desc = selectedItem.Text;
                m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value = m_CapturedKey;
                selectedItem.SubItems[1].Text = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                m_IsKeyboardShortcutChanged = true;
                m_txtShortcutKeys.Clear ();
                m_CapturedKey = Keys.None;
            }
        }

        private bool IsValidMenuShortcut( Keys capturedKey)
        {
            if (m_cbShortcutKeys.SelectedIndex == 1)
            {
                try
                {
                    ToolStripMenuItem m = new ToolStripMenuItem();
                    m.ShortcutKeys = capturedKey;
                    m.Dispose();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(string.Format ( Localizer.Message ( "Preference_InvalidKeyExceptionMsg" ), "\n\n",ex.ToString()),"Invalid Key Pressed",MessageBoxButtons.OK,MessageBoxIcon.Error );
                    m_CapturedKey = Keys.None;
                    ShowSelectedShortcutKeyInKeyboardShortcutTextbox();
                    m_txtShortcutKeys.Focus();
                    return false;
                }
            }
            return true;
        }

        private void m_btnRemove_Click(object sender, EventArgs e)
        {
            if (m_lvShortcutKeysList.SelectedIndices.Count > 0 && m_lvShortcutKeysList.SelectedIndices[0] >= 0 )
            {
                ListViewItem selectedItem = m_lvShortcutKeysList.Items[m_lvShortcutKeysList.SelectedIndices[0]];
                string desc = selectedItem.Text;
                m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value = Keys.None;
                selectedItem.SubItems[1].Text = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                m_txtShortcutKeys.Text = Keys.None.ToString();
                m_CapturedKey = Keys.None;
                m_IsKeyboardShortcutChanged = true;
            }
        }

        private void m_cbShortcutKeys_SelectionChangeCommitted(object sender, EventArgs e)
        {
            LoadListviewAccordingToComboboxSelection();
        }

        private void LoadListviewAccordingToComboboxSelection ()
        {
            string[] tempArray = new string[2];
            m_txtShortcutKeys.AccessibleName = string.Format(Localizer.Message("KeyboardShortcuts_TextboxAccessibleName"), "" );

            if ( m_cbShortcutKeys.SelectedIndex == 0 )
            {
                m_lvShortcutKeysList.Items.Clear();
            foreach (string desc in m_KeyboardShortcuts.KeyboardShortcutsDescription.Keys)
            {
                tempArray[0] = desc;
                if (!m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].IsMenuShortcut)
                {
                    tempArray[1] = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                    ListViewItem item = new ListViewItem(tempArray);
                    m_lvShortcutKeysList.Items.Add(item);
                }
            }
            m_txtShortcutKeys.Clear();
            m_CapturedKey = Keys.None;
        }
        else if (m_cbShortcutKeys.SelectedIndex == 1)
        {
            m_lvShortcutKeysList.Items.Clear();
            foreach (string desc in m_KeyboardShortcuts.KeyboardShortcutsDescription.Keys)
            {
                tempArray[0] = desc;
                if (m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].IsMenuShortcut)
                {
                    tempArray[1] = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
                    ListViewItem item = new ListViewItem(tempArray);
                    m_lvShortcutKeysList.Items.Add(item);
                }
            }
            m_txtShortcutKeys.Clear();
            m_CapturedKey = Keys.None;
        }
        }

        private void LoadListViewWithColors()
        {
            string[] tempArray = new string[3];
            m_lv_ColorPref.Items.Clear();
            mSettings.ColorSettings.PopulateColorSettingsDictionary();
            mSettings.ColorSettingsHC.PopulateColorSettingsDictionary();
            foreach (string desc in mSettings.ColorSettings.ColorSetting.Keys)
            {
                tempArray[0] = desc;
                tempArray[1] = mSettings.ColorSettings.ColorSetting[desc].Name;
                tempArray[2] = mSettings.ColorSettingsHC.ColorSetting[desc].Name;
                ListViewItem item = new ListViewItem(tempArray);
                m_lv_ColorPref.Items.Add(item);
            }
        }

        private void m_RestoreDefaults_Click(object sender, EventArgs e)
        {   
            mForm.LoadDefaultKeyboardShortcuts();
            m_KeyboardShortcuts = mForm.KeyboardShortcuts;
            
            m_lvShortcutKeysList.Items.Clear();
            LoadListviewAccordingToComboboxSelection();
        }

        private void m_cbOperation_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(m_cbOperation.SelectedIndex == 0)
            {
                if (m_Nudge >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = m_Nudge;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 20;
                label3.Visible = true;
            }
            if(m_cbOperation.SelectedIndex == 1)
            {
                if (m_Preview >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = m_Preview;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 100;
                label3.Visible = true;
            }
            if(m_cbOperation.SelectedIndex == 2)
            {
                if (m_Elapse >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = m_Elapse;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 150;
                label3.Visible = true;
            }
            if (m_cbOperation.SelectedIndex == 3)
            {
                if ((decimal)m_DefaultLeadingSilence >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = (decimal)m_DefaultLeadingSilence;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 10;
                label3.Visible = true;
            }
            if (m_cbOperation.SelectedIndex == 4)
            {
                if ((decimal)m_DefaultThreshold >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = (decimal)m_DefaultThreshold;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 25;
                label3.Visible = false;
            }
            if (m_cbOperation.SelectedIndex == 5)
            {
                if ((decimal)m_DefaultGap >= m_OperationDurationUpDown.Minimum)
                {
                    m_OperationDurationUpDown.Value = (decimal)m_DefaultGap;
                }
                else
                {
                    m_OperationDurationUpDown.Value = m_OperationDurationUpDown.Minimum;
                }
                m_OperationDurationUpDown.Increment = 25;
                label3.Visible = true;
            }
        }

        private void m_OperationDurationUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (m_cbOperation.SelectedIndex == 0)
                m_Nudge = (int)(m_OperationDurationUpDown.Value);
            if (m_cbOperation.SelectedIndex == 1)
                m_Preview = (int)(m_OperationDurationUpDown.Value);
            if (m_cbOperation.SelectedIndex == 2)
                m_Elapse = (int) (m_OperationDurationUpDown.Value);
            if (m_cbOperation.SelectedIndex == 3)
                m_DefaultLeadingSilence = (double) (m_OperationDurationUpDown.Value);
            if (m_cbOperation.SelectedIndex == 4)
                m_DefaultThreshold = (double) (m_OperationDurationUpDown.Value);
            if (m_cbOperation.SelectedIndex == 5)
                m_DefaultGap = (double) (m_OperationDurationUpDown.Value);
        }

        private void m_CheckBoxListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (!m_IsComplete)
                return;
            //else
                //UpdateBoolSettings();
            if (mTab.SelectedTab == mAudioTab)
            {
                if (e.Item.Text == Localizer.Message("Audio_DetectPhrasesWhileRecording") && e.Item.Checked)
                {
                    MessageBox.Show(string.Format(Localizer.Message("AudioPref_LivePhraseDetectionEnable"), mSettings.Audio_DefaultThreshold, mSettings.Audio_DefaultGap,mSettings.Audio_DefaultLeadingSilence )) ;
                }

                if (e.Item.Text == Localizer.Message("Audio_SaveAudioZoom") && !e.Item.Checked)
                {
                    mSettings.Audio_AudioScale = 0.01f;
                }

                //if (m_FlagComboBoxIndexChange == false)
                //{
                //    m_ComboSelectAudioProfile.SelectedIndex = 5;
                //    m_IndexOfLevelCombox = 5;
                //}

               
            }
        }
        public void UpdateBoolSettings()
        {

            if (mTab.SelectedTab == mProjectTab)
            {
                mSettings.Project_OpenLastProject = m_CheckBoxListView.Items[0].Checked;
               // mSettings.AutoSave_RecordingEnd = m_CheckBoxListView.Items[1].Checked;
                mSettings.Project_OpenBookmarkNodeOnReopeningProject = m_CheckBoxListView.Items[1].Checked;
                mSettings.Project_LeftAlignPhrasesInContentView = m_CheckBoxListView.Items[8].Checked ? m_CheckBoxListView.Items[2].Checked : false; // false if waveform is disabled

                //MessageBox.Show(mSettings.Project_ShowWaveformInContentView.ToString () + " : " +  mSettings.LeftAlignPhrasesInContentView.ToString()); 
                mSettings.Project_OptimizeMemory = m_CheckBoxListView.Items[3].Checked;
                mSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup = m_CheckBoxListView.Items[4].Checked;
                mSettings.Project_EnableFreeDiskSpaceCheck= m_CheckBoxListView.Items[5].Checked;
                mSettings.Project_SaveProjectWhenRecordingEnds= m_CheckBoxListView.Items[6].Checked;
                mSettings.Project_CheckForUpdates = m_CheckBoxListView.Items[7].Checked;
                bool tempShowWaveformStatus = mSettings.Project_ShowWaveformInContentView;
                mSettings.Project_ShowWaveformInContentView = m_CheckBoxListView.Items[8].Checked;
                if (!tempShowWaveformStatus && mSettings.Project_ShowWaveformInContentView)
                {
                    DialogResult dr = MessageBox.Show(Localizer.Message("Project_MessageBoxToCheckFixContentView"), "?", MessageBoxButtons.YesNo);
                    if (dr == DialogResult.Yes)
                    {
                        m_CheckBoxListView.Items[2].Checked = true;
                        mSettings.Project_LeftAlignPhrasesInContentView = m_CheckBoxListView.Items[2].Checked;
                    }

                }
                mSettings.Project_Export_AlwaysIgnoreIndentation= m_CheckBoxListView.Items[9].Checked;
                mSettings.Project_BackgroundColorForEmptySection = m_CheckBoxListView.Items[10].Checked;
                mSettings.Project_SaveObiLocationAndSize = m_CheckBoxListView.Items[11].Checked;
                mSettings.Project_PeakMeterChangeLocation = m_CheckBoxListView.Items[12].Checked;
                mSettings.Project_EPUBCheckTimeOutEnabled = m_CheckBoxListView.Items[13].Checked;
                mSettings.Project_MinimizeObi = m_CheckBoxListView.Items[14].Checked;
                mSettings.Project_EnableMouseScrolling = m_CheckBoxListView.Items[15].Checked;
                mSettings.Project_DisableTOCViewCollapse = m_CheckBoxListView.Items[16].Checked;
                mSettings.Project_MaximizeObi = m_CheckBoxListView.Items[17].Checked;
                mSettings.Project_VAXhtmlExport = m_CheckBoxListView.Items[18].Checked;
            }
            if (mTab.SelectedTab == mAudioTab)
            {
                mSettings.Audio_AudioClues = m_CheckBoxListView.Items[0].Checked;
                mSettings.Audio_RetainInitialSilenceInPhraseDetection = m_CheckBoxListView.Items[1].Checked;
                mSettings.Audio_Recording_PreviewBeforeStarting = m_CheckBoxListView.Items[2].Checked;
                mSettings.Audio_UseRecordingPauseShortcutForStopping = m_CheckBoxListView.Items[3].Checked;
                mSettings.Audio_AllowOverwrite = m_CheckBoxListView.Items[4].Checked;                
                mSettings.Audio_RecordDirectlyWithRecordButton = m_CheckBoxListView.Items[5].Checked;
                
                mSettings.MaxAllowedPhraseDurationInMinutes = (uint)(m_CheckBoxListView.Items[6].Checked ? 50 : 180);
                mSettings.Audio_ShowLiveWaveformWhileRecording = m_CheckBoxListView.Items[7].Checked;
                mSettings.Audio_EnableLivePhraseDetection= m_CheckBoxListView.Items[8].Checked;
                mSettings.Audio_EnablePostRecordingPageRenumbering= m_CheckBoxListView.Items[9].Checked;
                mSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection= m_CheckBoxListView.Items[10].Checked;
                mSettings.Audio_FastPlayWithoutPitchChange= m_CheckBoxListView.Items[11].Checked;
                mSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio = m_CheckBoxListView.Items[12].Checked;
                mSettings.Audio_EnforceSingleCursor = m_CheckBoxListView.Items[13].Checked;
                mSettings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording = m_CheckBoxListView.Items[14].Checked;
                mSettings.Audio_DisableDeselectionOnStop = m_CheckBoxListView.Items[15].Checked;
                mSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio = m_CheckBoxListView.Items[16].Checked;
                mSettings.Audio_AlwaysMonitorRecordingToolBar = m_CheckBoxListView.Items[17].Checked;
                mSettings.Audio_ColorFlickerPreviewBeforeRecording = m_CheckBoxListView.Items[18].Checked;
                mSettings.Audio_PlaySectionUsingPlayBtn = m_CheckBoxListView.Items[19].Checked;
                mSettings.Audio_EnableFileDataProviderPreservation = m_CheckBoxListView.Items[20].Checked;
                mSettings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording = m_CheckBoxListView.Items[21].Checked;
                mSettings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording = m_CheckBoxListView.Items[22].Checked;
                mSettings.Audio_PreventSplittingPages = m_CheckBoxListView.Items[23].Checked;
                mSettings.Audio_SaveAudioZoom = m_CheckBoxListView.Items[24].Checked;
                
            }
            //if (mTab.SelectedTab == mAdvanceTab)
            //{
            //    mSettings.Audio_LevelComboBoxIndex = m_cb_SelectProfile.SelectedIndex;
            //}
        }

        public void UpdateTabControl()
        {
            m_IsComplete = false;
            m_CheckBoxListView.Columns.Clear();
            m_CheckBoxListView.HeaderStyle = ColumnHeaderStyle.None;
            m_CheckBoxListView.ShowItemToolTips = true;
            m_CheckBoxListView.Columns.Add("", 317, HorizontalAlignment.Left);
            if (this.mTab.SelectedTab == this.mAudioTab)
            {
             helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
             helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
             helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/Audio Preferences.htm");

                m_CheckBoxListView.Visible = true;
               
                m_grpBoxChkBoxListView.Visible = true;
                m_CheckBoxListView.Items.Clear();
                //m_CheckBoxListView.Location = new Point(93, 284);
                m_CheckBoxListView.Location = new Point(120, 255);
                m_CheckBoxListView.Size = new Size(365, 135);
                
               //m_grpBoxChkBoxListView.Location = new Point(85, 264);
                m_grpBoxChkBoxListView.Location = new Point(108, 235);
                m_grpBoxChkBoxListView.Size = new Size(385, 165);

                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_AudioClues"));
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_RetainInitialSilence"));
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_PreviewBeforeRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_UseRecordingPauseShortcutForStopping"));
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_AllowOverwrite"));
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_RecordDirectlyFromTransportBar"));
                m_CheckBoxListView.Items.Add(Localizer.Message("AudioTab_Limit max phrase duration to 50 minutes"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_ShowLiveWaveformWhileRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_DetectPhrasesWhileRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_EnablePostRecordingPageRenumbering"));
                
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_MergeFirstTwoPhrasesInPhraseDetection"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_FastPlayWithoutPitchChange"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_RecordSubsequentPhrases"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_EnforceSingleCursor"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_DeleteFollowingPhrasesOfSectionAfterRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_DisableDeselectionOnStop"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_PreservePage"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_AlwaysMonitoringRecordingToolBar"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_ColorFlickerPreviewBeforeRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_PlaySectionUsingPlayBtn"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_EnableFileDataProviderPreservation"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_EnsureCursorVisibilityInUndoOfSplitRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_PreventSplittingPages"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Audio_SaveAudioZoom"));
               
                m_CheckBoxListView.Items[0].Checked = mSettings.Audio_AudioClues;
                m_CheckBoxListView.Items[0].ToolTipText = Localizer.Message("AudioTab_AudioClues");                
                m_CheckBoxListView.Items[1].Checked = mSettings.Audio_RetainInitialSilenceInPhraseDetection;
                m_CheckBoxListView.Items[1].ToolTipText = Localizer.Message("AudioTab_RetainInitialSilence");
                m_CheckBoxListView.Items[2].Checked = mSettings.Audio_Recording_PreviewBeforeStarting;
                m_CheckBoxListView.Items[2].ToolTipText = Localizer.Message("AudioTab_PreviewBeforeRecording");
                m_CheckBoxListView.Items[3].Checked = mSettings.Audio_UseRecordingPauseShortcutForStopping;
                m_CheckBoxListView.Items[3].ToolTipText = Localizer.Message("Audio_UseRecordingPauseShortcutForStopping");
                m_CheckBoxListView.Items[4].Checked = mSettings.Audio_AllowOverwrite;
                m_CheckBoxListView.Items[4].ToolTipText = Localizer.Message("AudioTab_AllowOverwrite");
                m_CheckBoxListView.Items[5].Checked = mSettings.Audio_RecordDirectlyWithRecordButton;
                m_CheckBoxListView.Items[5].ToolTipText = Localizer.Message("AudioTab_RecordDirectlyFromTransportBar");
                m_CheckBoxListView.Items[6].Checked = mSettings.MaxAllowedPhraseDurationInMinutes == 50 ;
                m_CheckBoxListView.Items[6].ToolTipText = Localizer.Message("AudioTab_Limit max phrase duration to 50 minutes");
                m_CheckBoxListView.Items[7].Checked = mSettings.Audio_ShowLiveWaveformWhileRecording;
                m_CheckBoxListView.Items[7].ToolTipText = Localizer.Message("Audio_ShowLiveWaveformWhileRecording");
                m_CheckBoxListView.Items[8].Checked = mSettings.Audio_EnableLivePhraseDetection;
                m_CheckBoxListView.Items[8].ToolTipText = Localizer.Message("Audio_DetectPhrasesWhileRecording");
                m_CheckBoxListView.Items[9].Checked = mSettings.Audio_EnablePostRecordingPageRenumbering;
                m_CheckBoxListView.Items[9].ToolTipText = Localizer.Message("Audio_EnablePostRecordingPageRenumbering");
                m_CheckBoxListView.Items[10].Checked = mSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
                m_CheckBoxListView.Items[10].ToolTipText = Localizer.Message("Audio_MergeFirstTwoPhrasesInPhraseDetection");
                m_CheckBoxListView.Items[11].Checked = mSettings.Audio_FastPlayWithoutPitchChange;
                m_CheckBoxListView.Items[11].ToolTipText = Localizer.Message("Audio_FastPlayWithoutPitchChange");
                m_CheckBoxListView.Items[12].Checked = mSettings.Audio_UseRecordBtnToRecordOverSubsequentAudio;
                m_CheckBoxListView.Items[12].ToolTipText = Localizer.Message("Audio_RecordSubsequentPhrases");
                m_CheckBoxListView.Items[13].Checked = mSettings.Audio_EnforceSingleCursor;
                m_CheckBoxListView.Items[13].ToolTipText = Localizer.Message("Audio_EnforceSingleCursor");
                m_CheckBoxListView.Items[14].Checked = mSettings.Audio_DeleteFollowingPhrasesOfSectionAfterRecording;
                m_CheckBoxListView.Items[14].ToolTipText = Localizer.Message("Audio_DeleteFollowingPhrasesOfSectionAfterRecording");
                m_CheckBoxListView.Items[15].Checked = mSettings.Audio_DisableDeselectionOnStop;
                m_CheckBoxListView.Items[15].ToolTipText = Localizer.Message("Audio_DisableDeselectionOnStop");
                m_CheckBoxListView.Items[16].Checked = mSettings.Audio_PreservePagesWhileRecordOverSubsequentAudio;
                m_CheckBoxListView.Items[16].ToolTipText = Localizer.Message("Audio_PreservePage");
                m_CheckBoxListView.Items[17].Checked = mSettings.Audio_AlwaysMonitorRecordingToolBar;
                m_CheckBoxListView.Items[17].ToolTipText = Localizer.Message("Audio_AlwaysMonitoringRecordingToolBar");
                m_CheckBoxListView.Items[18].Checked = mSettings.Audio_ColorFlickerPreviewBeforeRecording;
                m_CheckBoxListView.Items[18].ToolTipText = Localizer.Message("Audio_ColorFlickerPreviewBeforeRecording");
                m_CheckBoxListView.Items[19].Checked = mSettings.Audio_PlaySectionUsingPlayBtn;
                m_CheckBoxListView.Items[19].ToolTipText = Localizer.Message("Audio_PlaySectionUsingPlayBtn");
                m_CheckBoxListView.Items[20].Checked = mSettings.Audio_EnableFileDataProviderPreservation;
                m_CheckBoxListView.Items[20].ToolTipText = Localizer.Message("Audio_EnableFileDataProviderPreservation");
                m_CheckBoxListView.Items[21].Checked = mSettings.Audio_EnsureCursorVisibilityInUndoOfSplitRecording;
                m_CheckBoxListView.Items[21].ToolTipText = Localizer.Message("Audio_EnsureCursorVisibilityInUndoOfSplitRecording");
                m_CheckBoxListView.Items[22].Checked = mSettings.Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording;
                m_CheckBoxListView.Items[22].ToolTipText = Localizer.Message("Audio_DisableCreationOfNewHeadingsAndPagesWhileRecording");
                m_CheckBoxListView.Items[23].Checked = mSettings.Audio_PreventSplittingPages;
                m_CheckBoxListView.Items[23].ToolTipText = Localizer.Message("Audio_PreventSplittingPages");
                m_CheckBoxListView.Items[24].Checked = mSettings.Audio_SaveAudioZoom;
                m_CheckBoxListView.Items[24].ToolTipText = Localizer.Message("Audio_SaveAudioZoom");

              
            }
            if (this.mTab.SelectedTab == this.mProjectTab)
            {
             helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
             helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
             helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/Project Preferences.htm");

                m_CheckBoxListView.Visible = true;
                m_grpBoxChkBoxListView.Visible = true;
                m_CheckBoxListView.Items.Clear();
                m_CheckBoxListView.Size = new Size(365, 135);
                m_CheckBoxListView.Location = new Point(120, 255);
                m_grpBoxChkBoxListView.Size = new Size(385, 165);
                m_grpBoxChkBoxListView.Location = new Point(108, 235);
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_OpenLastProject"));
               // m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_AutoSaveWhenRecordingEnds"));
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_SelectBookmark"));
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_FixContentViewWidth"));
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_OptimizeMemory"));
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_DeleteUnusedFilesAfterCleanUp"));
                m_CheckBoxListView.Items.Add(Localizer.Message("ProjectTab_EnableFreeDiskSpaceCheck"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_SaveProjectWhenRecordingEnds"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_CheckForUpdates"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_ShowWaveformsInContentView"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_AlwaysIgnoreIndentationForExportFiles"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_BackgroundColorForEmptySection"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_SaveObiLocationAndSize"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_PeakMeterChangeLocation"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_EPUBCheckTimeOutEnabled"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_MinimizeObi"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_EnableMouseScrolling"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_DisableTOCViewCollapse"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_MaximizeObi"));
                m_CheckBoxListView.Items.Add(Localizer.Message("Project_VAXhtmlExport"));
               
                m_CheckBoxListView.Items[0].Checked = mSettings.Project_OpenLastProject;
                m_CheckBoxListView.Items[0].ToolTipText = Localizer.Message("ProjectTab_OpenLastProject");
                m_CheckBoxListView.Items[1].Checked = mSettings.Project_OpenBookmarkNodeOnReopeningProject;
                m_CheckBoxListView.Items[1].ToolTipText = Localizer.Message("ProjectTab_SelectBookmark");
                m_CheckBoxListView.Items[2].Checked = mSettings.Project_LeftAlignPhrasesInContentView;
                m_CheckBoxListView.Items[2].ToolTipText = Localizer.Message("ProjectTab_FixContentViewWidth");
                m_CheckBoxListView.Items[3].Checked = mSettings.Project_OptimizeMemory;
                m_CheckBoxListView.Items[3].ToolTipText = Localizer.Message("ProjectTab_OptimizeMemory");
                m_CheckBoxListView.Items[4].Checked = mSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup;
                m_CheckBoxListView.Items[4].ToolTipText = Localizer.Message("ProjectTab_DeleteUnusedFilesAfterCleanUp");
                m_CheckBoxListView.Items[5].Checked = mSettings.Project_EnableFreeDiskSpaceCheck;
                m_CheckBoxListView.Items[5].ToolTipText = Localizer.Message("ProjectTab_EnableFreeDiskSpaceCheck");
                m_CheckBoxListView.Items[6].Checked = mSettings.Project_SaveProjectWhenRecordingEnds;
                m_CheckBoxListView.Items[6].ToolTipText = Localizer.Message("Project_SaveProjectWhenRecordingEnds");
                m_CheckBoxListView.Items[7].Checked = mSettings.Project_CheckForUpdates;
                m_CheckBoxListView.Items[7].ToolTipText = Localizer.Message("Project_CheckForUpdates");
                m_CheckBoxListView.Items[8].Checked = mSettings.Project_ShowWaveformInContentView;
                m_CheckBoxListView.Items[8].ToolTipText = Localizer.Message("Project_ShowWaveformsInContentView");
                m_CheckBoxListView.Items[9].Checked = mSettings.Project_Export_AlwaysIgnoreIndentation;
                m_CheckBoxListView.Items[9].ToolTipText = Localizer.Message("Project_AlwaysIgnoreIndentationForExportFiles");
                m_CheckBoxListView.Items[10].Checked = mSettings.Project_BackgroundColorForEmptySection;
                m_CheckBoxListView.Items[10].ToolTipText = Localizer.Message("Project_BackgroundColorForEmptySection");
                m_CheckBoxListView.Items[11].Checked = mSettings.Project_SaveObiLocationAndSize;
                m_CheckBoxListView.Items[11].ToolTipText = Localizer.Message("Project_SaveObiLocationAndSize");
                m_CheckBoxListView.Items[12].Checked = mSettings.Project_PeakMeterChangeLocation;
                m_CheckBoxListView.Items[12].ToolTipText = Localizer.Message("Project_PeakMeterChangeLocation");
                m_CheckBoxListView.Items[13].Checked = mSettings.Project_EPUBCheckTimeOutEnabled;
                m_CheckBoxListView.Items[13].ToolTipText = Localizer.Message("Project_EPUBCheckTimeOutEnabled");
                m_CheckBoxListView.Items[14].Checked = mSettings.Project_MinimizeObi;
                m_CheckBoxListView.Items[14].ToolTipText = Localizer.Message("Project_MinimizeObi");
                m_CheckBoxListView.Items[15].Checked = mSettings.Project_EnableMouseScrolling;
                m_CheckBoxListView.Items[15].ToolTipText = Localizer.Message("Project_EnableMouseScrolling");
                m_CheckBoxListView.Items[16].Checked = mSettings.Project_DisableTOCViewCollapse;
                m_CheckBoxListView.Items[16].ToolTipText = Localizer.Message("Project_DisableTOCViewCollapse");
                m_CheckBoxListView.Items[17].Checked = mSettings.Project_MaximizeObi;
                m_CheckBoxListView.Items[17].ToolTipText = Localizer.Message("Project_MaximizeObi");
                m_CheckBoxListView.Items[18].Checked = mSettings.Project_VAXhtmlExport;
                m_CheckBoxListView.Items[18].ToolTipText = Localizer.Message("Project_VAXhtmlExport");

            }
            m_CheckBoxListView.View = View.Details;
            m_IsComplete = true;
        }

        private void mTab_SelectedIndexChanged(object sender, EventArgs e)
        {
         //   m_FlagComboBoxIndexChange = true;
            UpdateTabControl();
           // m_FlagComboBoxIndexChange = false;
            if (mTab.SelectedTab == mKeyboardShortcutTab)
            {
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/Keyboard shortcut preferences.htm");         

                m_CheckBoxListView.Visible = false;
                m_grpBoxChkBoxListView.Visible = false;
                m_lblShortcutKeys.Text = m_lblShortcutKeys_text;
            }
            else
            {
                m_lblShortcutKeys.Text = "";
            }
            if (mTab.SelectedTab == mUserProfileTab)
            {
             helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
             helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
             helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/The Preferences Dialog/User Profile Preferences.htm");         

                m_CheckBoxListView.Visible = false;
                m_grpBoxChkBoxListView.Visible = false;
            }
            if (mTab.SelectedTab == mColorPreferencesTab)
            {
             helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
             helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
             helpProvider1.SetHelpKeyword(this,"HTML Files/Exploring the GUI/The Preferences Dialog/Color preferences.htm");

                m_CheckBoxListView.Visible = false;
                m_grpBoxChkBoxListView.Visible = false;
            }
            if (mTab.SelectedTab == mAdvanceTab)
            {
                m_CheckBoxListView.Visible = false;
                m_grpBoxChkBoxListView.Visible = false;
             //  m_cb_SelectProfile.SelectedIndex = mSettings.Audio_LevelComboBoxIndex;
            }
            if (!m_txtShortcutKeys.Focused) this.AcceptButton = mOKButton;
            m_ApplyButton.Enabled = (mTab.SelectedTab != mAdvanceTab);
            
        }

        private void ResetPreferences()
        {
            if (mTab.SelectedTab == mProjectTab)// Default settings for Project tab
            {
                mSettings.Project_DefaultPath = m_DefaultSettings.Project_DefaultPath;
                mSettings.Project_PipelineScriptsPath = m_DefaultSettings.Project_PipelineScriptsPath;
                mSettings.Audio_LocalRecordingDirectory = m_DefaultSettings.Audio_LocalRecordingDirectory;
                mSettings.Project_OpenLastProject = m_DefaultSettings.Project_OpenLastProject;
                mSettings.Project_AutoSave_RecordingEnd = m_DefaultSettings.Project_AutoSave_RecordingEnd;
                mSettings.Project_OpenBookmarkNodeOnReopeningProject = m_DefaultSettings.Project_OpenBookmarkNodeOnReopeningProject;
                mSettings.Project_LeftAlignPhrasesInContentView = m_DefaultSettings.Project_LeftAlignPhrasesInContentView;
                mSettings.Project_OptimizeMemory = m_DefaultSettings.Project_OptimizeMemory;
                mSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup =
                    m_DefaultSettings.Project_AutomaticallyDeleteUnusedFilesAfterCleanup;
                mSettings.Project_EnableFreeDiskSpaceCheck=
                    m_DefaultSettings.Project_EnableFreeDiskSpaceCheck;
                mSettings.Project_SaveProjectWhenRecordingEnds=
                    m_DefaultSettings.Project_SaveProjectWhenRecordingEnds;
                mSettings.Project_CheckForUpdates = m_DefaultSettings.Project_CheckForUpdates;
                mSettings.Project_ShowWaveformInContentView = m_DefaultSettings.Project_ShowWaveformInContentView;
                mSettings.Project_Export_AlwaysIgnoreIndentation = m_DefaultSettings.Project_Export_AlwaysIgnoreIndentation;
                mSettings.Project_BackgroundColorForEmptySection = m_DefaultSettings.Project_BackgroundColorForEmptySection;
                mSettings.Project_SaveObiLocationAndSize = m_DefaultSettings.Project_SaveObiLocationAndSize;
                mSettings.Project_PeakMeterChangeLocation = m_DefaultSettings.Project_PeakMeterChangeLocation;
                mSettings.Project_EPUBCheckTimeOutEnabled= m_DefaultSettings.Project_EPUBCheckTimeOutEnabled;
                mSettings.Project_MinimizeObi = m_DefaultSettings.Project_MinimizeObi;
                mSettings.Project_EnableMouseScrolling = m_DefaultSettings.Project_EnableMouseScrolling;
                mSettings.Project_DisableTOCViewCollapse = m_DefaultSettings.Project_DisableTOCViewCollapse;
                mSettings.Project_MaximizeObi = m_DefaultSettings.Project_MaximizeObi;
                mSettings.Project_VAXhtmlExport = m_DefaultSettings.Project_VAXhtmlExport;
                InitializeProjectTab();
            }
            else if (mTab.SelectedTab == mAudioTab) // Default settings for Audio tab
            {
                mSettings.Audio_LastInputDevice = m_DefaultSettings.Audio_LastInputDevice;
                mSettings.Audio_LastOutputDevice = m_DefaultSettings.Audio_LastOutputDevice;
                mSettings.Audio_SampleRate = m_DefaultSettings.Audio_SampleRate;
                mSettings.Audio_Channels = m_DefaultSettings.Audio_Channels;
                mSettings.Audio_NoiseLevel = m_DefaultSettings.Audio_NoiseLevel;
                mSettings.Audio_AudioClues = m_DefaultSettings.Audio_AudioClues;                
                mSettings.Audio_RetainInitialSilenceInPhraseDetection = m_DefaultSettings.Audio_RetainInitialSilenceInPhraseDetection;
                mSettings.Audio_Recording_PreviewBeforeStarting = m_DefaultSettings.Audio_Recording_PreviewBeforeStarting;
                mSettings.Audio_AllowOverwrite = m_DefaultSettings.Audio_AllowOverwrite;
                mSettings.Audio_UseRecordingPauseShortcutForStopping = m_DefaultSettings.Audio_UseRecordingPauseShortcutForStopping;
                mSettings.Audio_RecordDirectlyWithRecordButton = m_DefaultSettings.Audio_RecordDirectlyWithRecordButton;
                mSettings.MaxAllowedPhraseDurationInMinutes = m_DefaultSettings.MaxAllowedPhraseDurationInMinutes;
                mSettings.Audio_ShowLiveWaveformWhileRecording = m_DefaultSettings.Audio_ShowLiveWaveformWhileRecording;
                mSettings.Audio_EnableLivePhraseDetection = m_DefaultSettings.Audio_EnableLivePhraseDetection;
                mSettings.Audio_EnablePostRecordingPageRenumbering= m_DefaultSettings.Audio_EnablePostRecordingPageRenumbering;
                mSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection = m_DefaultSettings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection;
                mSettings.Audio_FastPlayWithoutPitchChange= m_DefaultSettings.Audio_FastPlayWithoutPitchChange;

                //If operation is empty then nothing will b selected.
                mSettings.Audio_NudgeTimeMs = m_DefaultSettings.Audio_NudgeTimeMs;
                mSettings.Audio_PreviewDuration = m_DefaultSettings.Audio_PreviewDuration;
                mSettings.Audio_ElapseBackTimeInMilliseconds = m_DefaultSettings.Audio_ElapseBackTimeInMilliseconds;
                mSettings.Audio_DefaultLeadingSilence = m_DefaultSettings.Audio_DefaultLeadingSilence;
                mSettings.Audio_DefaultThreshold = m_DefaultSettings.Audio_DefaultThreshold;
                mSettings.Audio_DefaultGap = m_DefaultSettings.Audio_DefaultGap;
                mSettings.Project_ImportToleranceForAudioInMs = m_DefaultSettings.Project_ImportToleranceForAudioInMs;
              //  mSettings.Audio_LevelComboBoxIndex = m_DefaultSettings.Audio_LevelComboBoxIndex;
                mSettings.Audio_CleanupMaxFileSizeInMB = m_DefaultSettings.Audio_CleanupMaxFileSizeInMB;

                InitializeAudioTab();
                m_cbOperation.SelectedIndex = 0;
                m_OperationDurationUpDown.Value = 200;                
            }
            else if (mTab.SelectedTab == mUserProfileTab)  // Default settings for User Profile tab
            {
                mSettings.UserProfile = m_DefaultSettings.UserProfile;
                InitializeUserProfileTab();
            }
            else if (mTab.SelectedTab == mKeyboardShortcutTab)   // Default settings for keyboard Shortcuts tab
            {
                ResetKeyboardShortcuts();
                 LoadListviewAccordingToComboboxSelection();
                 //InitializeKeyboardShortcutsTab();
                // Not required it already has reset button.
            }
            else if (mTab.SelectedTab == mColorPreferencesTab)
            {
                ResetColors();
                m_IsColorChanged = true;
            }
        }

        private void ResetKeyboardShortcuts()
        {
            mForm.LoadDefaultKeyboardShortcuts();
            m_KeyboardShortcuts = mForm.KeyboardShortcuts;

            m_lvShortcutKeysList.Items.Clear();
            LoadListviewAccordingToComboboxSelection();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetPreferences();
            m_cb_SelectProfile.SelectedIndex = 0;
        }

        private void m_txtShortcutKeys_Leave(object sender, EventArgs e)
        {
            this.AcceptButton = mOKButton;
        }

        //private void m_btn_AdvancedRecording_Click(object sender, EventArgs e)
        //{
        //    if (m_btn_AdvancedRecording.Text == Localizer.Message("EnableAdvancedRecording"))
        //    {
        //        if (MessageBox.Show(Localizer.Message("Preferences_Allow_overwrite"),Localizer.Message("Preferences_advanced_recording_mode"), MessageBoxButtons.YesNo,
        //                    MessageBoxIcon.Question) == DialogResult.Yes)
        //        {
        //            m_CheckBoxListView.Items[4].Checked = true;
        //            m_CheckBoxListView.Items[5].Checked = true;
        //            m_CheckBoxListView.Items[8].Checked = true;
        //            m_btn_AdvancedRecording.Text = Localizer.Message("DisableAdvancedRecording");
        //        }
        //        else
        //            return;
        //    }
        //    else if (m_btn_AdvancedRecording.Text ==  Localizer.Message("DisableAdvancedRecording"))
        //    {
        //        m_CheckBoxListView.Items[4].Checked = false;
        //        m_CheckBoxListView.Items[5].Checked = false;
        //        m_CheckBoxListView.Items[8].Checked = false;
        //        m_btn_AdvancedRecording.Text = Localizer.Message("EnableAdvancedRecording");
        //    }
        //}

        private void m_lvShortcutKeysList_ItemMouseHover(object sender, ListViewItemMouseHoverEventArgs e)
        {
            string desc = e.Item.Text;
            if (!string.IsNullOrEmpty(desc))
            {
                e.Item.ToolTipText = m_KeyboardShortcutReadableNamesMap.ContainsKey(desc) ?
                    m_KeyboardShortcutReadableNamesMap[desc] :
                    desc;
            }
        }

        public void UpdateColorSettings()
        {
            if (m_lv_ColorPref.SelectedIndices.Count > 0 && mNormalColorCombo.SelectedItem != null)
            {
                Console.WriteLine("show selected index ---" + m_lv_ColorPref.SelectedIndices[0]);
                switch (m_lv_ColorPref.SelectedIndices[0])
                {                      
                    case 0: mSettings.ColorSettings.BlockBackColor_Custom = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 1: mSettings.ColorSettings.BlockBackColor_Empty = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 2: mSettings.ColorSettings.BlockBackColor_Heading = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 3: mSettings.ColorSettings.BlockBackColor_Page = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 4: mSettings.ColorSettings.BlockBackColor_Plain = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 5: mSettings.ColorSettings.BlockBackColor_Selected = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 6: mSettings.ColorSettings.BlockBackColor_Silence = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 7: mSettings.ColorSettings.BlockBackColor_TODO = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 8: mSettings.ColorSettings.BlockBackColor_Unused = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 9: mSettings.ColorSettings.BlockBackColor_Anchor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 10: mSettings.ColorSettings.BlockForeColor_Custom = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 11: mSettings.ColorSettings.BlockForeColor_Empty = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 12: mSettings.ColorSettings.BlockForeColor_Heading = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 13: mSettings.ColorSettings.BlockForeColor_Page = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 14: mSettings.ColorSettings.BlockForeColor_Plain = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 15: mSettings.ColorSettings.BlockForeColor_Selected = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 16: mSettings.ColorSettings.BlockForeColor_Silence = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 17: mSettings.ColorSettings.BlockForeColor_TODO = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 18: mSettings.ColorSettings.BlockForeColor_Anchor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 19: mSettings.ColorSettings.BlockForeColor_Unused = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 20:mSettings.ColorSettings.BlockLayoutSelectedColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 21:  mSettings.ColorSettings.ContentViewBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 22: mSettings.ColorSettings.EditableLabelTextBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 23: mSettings.ColorSettings.ProjectViewBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 24: mSettings.ColorSettings.StripBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 25: mSettings.ColorSettings.StripCursorSelectedBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 26: mSettings.ColorSettings.StripForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 27: mSettings.ColorSettings.StripSelectedBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 28: mSettings.ColorSettings.StripSelectedForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 29: mSettings.ColorSettings.StripUnusedBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 30: mSettings.ColorSettings.StripUnusedForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 31: mSettings.ColorSettings.StripWithoutPhrasesBackcolor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 32: mSettings.ColorSettings.TOCViewBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 33: mSettings.ColorSettings.TOCViewForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 34: mSettings.ColorSettings.TOCViewUnusedColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 35: mSettings.ColorSettings.ToolTipForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 36: mSettings.ColorSettings.TransportBarBackColor= (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 37: mSettings.ColorSettings.TransportBarLabelBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 38: mSettings.ColorSettings.TransportBarLabelForeColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 39: mSettings.ColorSettings.WaveformBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 40:mSettings.ColorSettings.WaveformBaseLineColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 41: mSettings.ColorSettings.WaveformHighlightedBackColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 42: mSettings.ColorSettings.FineNavigationColor = (Color) mNormalColorCombo.SelectedItem;
                        break;
                    case 43: mSettings.ColorSettings.RecordingHighlightPhraseColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 44: mSettings.ColorSettings.EmptySectionBackgroundColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    case 45: mSettings.ColorSettings.HighlightedSectionNodeWithoutSelectionColor = (Color)mNormalColorCombo.SelectedItem;
                        break;
                    default: break;
                }

                ListViewItem selectedItem = m_lv_ColorPref.Items[m_lv_ColorPref.SelectedIndices[0]];
                string desc = selectedItem.Text;
                selectedItem.SubItems[1].Text = ((Color)mNormalColorCombo.SelectedItem).Name;
            }

            if (m_lv_ColorPref.SelectedIndices.Count > 0 && mHighContrastCombo.SelectedItem != null)
            {
                Console.WriteLine("show selected index" + m_lv_ColorPref.SelectedIndices[0]);
                switch (m_lv_ColorPref.SelectedIndices[0])
                {
                    case 0: mSettings.ColorSettingsHC.BlockBackColor_Custom = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 1: mSettings.ColorSettingsHC.BlockBackColor_Empty = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 2: mSettings.ColorSettingsHC.BlockBackColor_Heading = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 3: mSettings.ColorSettingsHC.BlockBackColor_Page = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 4: mSettings.ColorSettingsHC.BlockBackColor_Plain = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 5: mSettings.ColorSettingsHC.BlockBackColor_Selected = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 6: mSettings.ColorSettingsHC.BlockBackColor_Silence = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 7: mSettings.ColorSettingsHC.BlockBackColor_TODO = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 8: mSettings.ColorSettingsHC.BlockBackColor_Unused = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 9: mSettings.ColorSettingsHC.BlockBackColor_Anchor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 10: mSettings.ColorSettingsHC.BlockForeColor_Custom = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 11: mSettings.ColorSettingsHC.BlockForeColor_Empty = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 12: mSettings.ColorSettingsHC.BlockForeColor_Heading = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 13: mSettings.ColorSettingsHC.BlockForeColor_Page = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 14: mSettings.ColorSettingsHC.BlockForeColor_Plain = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 15: mSettings.ColorSettingsHC.BlockForeColor_Selected = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 16: mSettings.ColorSettingsHC.BlockForeColor_Silence = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 17: mSettings.ColorSettingsHC.BlockForeColor_TODO = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 18: mSettings.ColorSettingsHC.BlockForeColor_Anchor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 19: mSettings.ColorSettingsHC.BlockForeColor_Unused = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 20: mSettings.ColorSettingsHC.BlockLayoutSelectedColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 21: mSettings.ColorSettingsHC.ContentViewBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 22: mSettings.ColorSettingsHC.EditableLabelTextBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 23: mSettings.ColorSettingsHC.ProjectViewBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 24: mSettings.ColorSettingsHC.StripBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 25: mSettings.ColorSettingsHC.StripCursorSelectedBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 26: mSettings.ColorSettingsHC.StripForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 27: mSettings.ColorSettingsHC.StripSelectedBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 28: mSettings.ColorSettingsHC.StripSelectedForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 29: mSettings.ColorSettingsHC.StripUnusedBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 30: mSettings.ColorSettingsHC.StripUnusedForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 31: mSettings.ColorSettingsHC.StripWithoutPhrasesBackcolor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 32: mSettings.ColorSettingsHC.TOCViewBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 33: mSettings.ColorSettingsHC.TOCViewForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 34: mSettings.ColorSettingsHC.TOCViewUnusedColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 35: mSettings.ColorSettingsHC.ToolTipForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 36: mSettings.ColorSettingsHC.TransportBarBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 37: mSettings.ColorSettingsHC.TransportBarLabelBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 38: mSettings.ColorSettingsHC.TransportBarLabelForeColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 39: mSettings.ColorSettingsHC.WaveformBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 40: mSettings.ColorSettingsHC.WaveformBaseLineColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 41: mSettings.ColorSettingsHC.WaveformHighlightedBackColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 42: mSettings.ColorSettingsHC.FineNavigationColor = (Color) mHighContrastCombo.SelectedItem;
                        break;
                    case 43: mSettings.ColorSettingsHC.RecordingHighlightPhraseColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 44: mSettings.ColorSettingsHC.EmptySectionBackgroundColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    case 45: mSettings.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor = (Color)mHighContrastCombo.SelectedItem;
                        break;
                    default: break;
                }
                ListViewItem selectedItem = m_lv_ColorPref.Items[m_lv_ColorPref.SelectedIndices[0]];
                string desc = selectedItem.Text;
                selectedItem.SubItems[2].Text = ((Color)mHighContrastCombo.SelectedItem).Name;
            }
        }

        private void mNormalColorCombo_SelectedIndexChanged(object sender, EventArgs e)
        {  try
            {
                if (mNormalColorCombo.SelectedItem != null) 
                {
                    System.Drawing.ColorConverter col = new ColorConverter ();                    
                   if (mNormalColorCombo.SelectedItem is Color)
                       m_txtBox_Color.BackColor = (Color)mNormalColorCombo.SelectedItem;
                   else if (mNormalColorCombo.SelectedItem is SystemColors)
                       m_txtBox_Color.BackColor = (Color)col.ConvertFromString(mNormalColorCombo.SelectedText);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public bool UpdateBackgroundColorRequired
        {
            set
            {
                m_UpdateBackgroundColorRequired = value;
            }
            get
            {
                return m_UpdateBackgroundColorRequired;
            }
        }
        private void m_btn_Apply_Click(object sender, EventArgs e)
        {
            ApplyColorPreference();
        }
        private void ApplyColorPreference()
        {
            Color tempEmptyBackgroundColor = mSettings.ColorSettings.EmptySectionBackgroundColor;
            Color tempEmptyBackgroundColorHC = mSettings.ColorSettingsHC.EmptySectionBackgroundColor;
            UpdateColorSettings();
            if (((tempEmptyBackgroundColor != mSettings.ColorSettings.EmptySectionBackgroundColor) && !SystemInformation.HighContrast) ||
                 ((tempEmptyBackgroundColorHC != mSettings.ColorSettingsHC.EmptySectionBackgroundColor) && SystemInformation.HighContrast))
            {
                UpdateBackgroundColorRequired = true;
            }
            else
            {
                UpdateBackgroundColorRequired = false;
            }
            m_IsColorChanged = true;
            VerifyChangeInLoadedSettings();
        }
        private void ResetColors()
        {
            ColorSettings settings = ColorSettings.DefaultColorSettings();
            ColorSettings settingsHC = ColorSettings.DefaultColorSettingsHC();

            mSettings.ColorSettings.BlockBackColor_Custom = settings.BlockBackColor_Custom;
            mSettings.ColorSettings.BlockBackColor_Anchor = settings.BlockBackColor_Anchor;
            mSettings.ColorSettings.BlockBackColor_Empty = settings.BlockBackColor_Empty;
            mSettings.ColorSettings.BlockBackColor_Heading = settings.BlockBackColor_Heading;
            mSettings.ColorSettings.BlockBackColor_Page = settings.BlockBackColor_Page;
            mSettings.ColorSettings.BlockBackColor_Plain = settings.BlockBackColor_Plain;
            mSettings.ColorSettings.BlockBackColor_Selected = settings.BlockBackColor_Selected;
            mSettings.ColorSettings.BlockBackColor_Silence = settings.BlockBackColor_Silence;
            mSettings.ColorSettings.BlockBackColor_TODO = settings.BlockBackColor_TODO;
            mSettings.ColorSettings.BlockBackColor_Unused = settings.BlockBackColor_Unused;
            mSettings.ColorSettings.BlockForeColor_Anchor = settings.BlockForeColor_Anchor;
            mSettings.ColorSettings.BlockForeColor_Custom = settings.BlockForeColor_Custom;
            mSettings.ColorSettings.BlockForeColor_Empty = settings.BlockForeColor_Empty;
            mSettings.ColorSettings.BlockForeColor_Heading = settings.BlockForeColor_Heading;
            mSettings.ColorSettings.BlockForeColor_Page = settings.BlockForeColor_Page;
            mSettings.ColorSettings.BlockForeColor_Plain = settings.BlockForeColor_Plain;
            mSettings.ColorSettings.BlockForeColor_Selected = settings.BlockForeColor_Selected;
            mSettings.ColorSettings.BlockForeColor_Silence = settings.BlockForeColor_Silence;
            mSettings.ColorSettings.BlockForeColor_TODO = settings.BlockForeColor_TODO;
            mSettings.ColorSettings.BlockForeColor_Unused = settings.BlockForeColor_Unused;
            mSettings.ColorSettings.BlockLayoutSelectedColor = settings.BlockLayoutSelectedColor;
            mSettings.ColorSettings.ContentViewBackColor = settings.ContentViewBackColor;
            mSettings.ColorSettings.EditableLabelTextBackColor = settings.EditableLabelTextBackColor;
            mSettings.ColorSettings.ProjectViewBackColor = settings.ProjectViewBackColor;
            mSettings.ColorSettings.StripBackColor = settings.StripBackColor;
            mSettings.ColorSettings.StripCursorSelectedBackColor = settings.StripCursorSelectedBackColor;
            mSettings.ColorSettings.StripForeColor = settings.StripForeColor;
            mSettings.ColorSettings.StripSelectedBackColor = settings.StripSelectedBackColor;
            mSettings.ColorSettings.StripSelectedForeColor = settings.StripSelectedForeColor;
            mSettings.ColorSettings.StripUnusedBackColor = settings.StripUnusedBackColor;
            mSettings.ColorSettings.StripUnusedForeColor = settings.StripUnusedForeColor;
            mSettings.ColorSettings.StripWithoutPhrasesBackcolor = settings.StripWithoutPhrasesBackcolor;
            mSettings.ColorSettings.TOCViewBackColor = settings.TOCViewBackColor;
            mSettings.ColorSettings.TOCViewForeColor = settings.TOCViewForeColor;
            mSettings.ColorSettings.TOCViewUnusedColor = settings.TOCViewUnusedColor;
            mSettings.ColorSettings.ToolTipForeColor = settings.ToolTipForeColor;
            mSettings.ColorSettings.TransportBarBackColor = settings.TransportBarBackColor;
            mSettings.ColorSettings.TransportBarLabelBackColor = settings.TransportBarLabelBackColor;
            mSettings.ColorSettings.TransportBarLabelForeColor = settings.TransportBarLabelForeColor;
            mSettings.ColorSettings.WaveformBackColor = settings.WaveformBackColor;
            mSettings.ColorSettings.WaveformBaseLineColor = settings.WaveformBaseLineColor;
            mSettings.ColorSettings.WaveformHighlightedBackColor = settings.WaveformHighlightedBackColor;
            mSettings.ColorSettings.FineNavigationColor = settings.FineNavigationColor;
            mSettings.ColorSettings.HighlightedSectionNodeWithoutSelectionColor = settings.HighlightedSectionNodeWithoutSelectionColor;
            mSettings.ColorSettings.RecordingHighlightPhraseColor = settings.RecordingHighlightPhraseColor;
            

            mSettings.ColorSettingsHC.BlockBackColor_Custom = settingsHC.BlockBackColor_Custom;
            mSettings.ColorSettingsHC.BlockBackColor_Anchor = settingsHC.BlockBackColor_Anchor;
            mSettings.ColorSettingsHC.BlockBackColor_Empty = settingsHC.BlockBackColor_Empty;
            mSettings.ColorSettingsHC.BlockBackColor_Heading = settingsHC.BlockBackColor_Heading;
            mSettings.ColorSettingsHC.BlockBackColor_Page = settingsHC.BlockBackColor_Page;
            mSettings.ColorSettingsHC.BlockBackColor_Plain = settingsHC.BlockBackColor_Plain;
            mSettings.ColorSettingsHC.BlockBackColor_Selected = settingsHC.BlockBackColor_Selected;
            mSettings.ColorSettingsHC.BlockBackColor_Silence = settingsHC.BlockBackColor_Silence;
            mSettings.ColorSettingsHC.BlockBackColor_TODO = settingsHC.BlockBackColor_TODO;
            mSettings.ColorSettingsHC.BlockBackColor_Unused = settingsHC.BlockBackColor_Unused;
            mSettings.ColorSettingsHC.BlockForeColor_Anchor = settingsHC.BlockForeColor_Anchor;
            mSettings.ColorSettingsHC.BlockForeColor_Custom = settingsHC.BlockForeColor_Custom;
            mSettings.ColorSettingsHC.BlockForeColor_Empty = settingsHC.BlockForeColor_Empty;
            mSettings.ColorSettingsHC.BlockForeColor_Heading = settingsHC.BlockForeColor_Heading;
            mSettings.ColorSettingsHC.BlockForeColor_Page = settingsHC.BlockForeColor_Page;
            mSettings.ColorSettingsHC.BlockForeColor_Plain = settingsHC.BlockForeColor_Plain;
            mSettings.ColorSettingsHC.BlockForeColor_Selected = settingsHC.BlockForeColor_Selected;
            mSettings.ColorSettingsHC.BlockForeColor_Silence = settingsHC.BlockForeColor_Silence;
            mSettings.ColorSettingsHC.BlockForeColor_TODO = settingsHC.BlockForeColor_TODO;
            mSettings.ColorSettingsHC.BlockForeColor_Unused = settingsHC.BlockForeColor_Unused;
            mSettings.ColorSettingsHC.BlockLayoutSelectedColor = settingsHC.BlockLayoutSelectedColor;
            mSettings.ColorSettingsHC.ContentViewBackColor = settingsHC.ContentViewBackColor;
            mSettings.ColorSettingsHC.EditableLabelTextBackColor = settingsHC.EditableLabelTextBackColor;
            mSettings.ColorSettingsHC.ProjectViewBackColor = settingsHC.ProjectViewBackColor;
            mSettings.ColorSettingsHC.StripBackColor = settingsHC.StripBackColor;
            mSettings.ColorSettingsHC.StripCursorSelectedBackColor = settingsHC.StripCursorSelectedBackColor;
            mSettings.ColorSettingsHC.StripForeColor = settingsHC.StripForeColor;
            mSettings.ColorSettingsHC.StripSelectedBackColor = settingsHC.StripSelectedBackColor;
            mSettings.ColorSettingsHC.StripSelectedForeColor = settingsHC.StripSelectedForeColor;
            mSettings.ColorSettingsHC.StripUnusedBackColor = settingsHC.StripUnusedBackColor;
            mSettings.ColorSettingsHC.StripUnusedForeColor = settingsHC.StripUnusedForeColor;
            mSettings.ColorSettingsHC.StripWithoutPhrasesBackcolor = settingsHC.StripWithoutPhrasesBackcolor;
            mSettings.ColorSettingsHC.TOCViewBackColor = settingsHC.TOCViewBackColor;
            mSettings.ColorSettingsHC.TOCViewForeColor = settingsHC.TOCViewForeColor;
            mSettings.ColorSettingsHC.TOCViewUnusedColor = settingsHC.TOCViewUnusedColor;
            mSettings.ColorSettingsHC.ToolTipForeColor = settingsHC.ToolTipForeColor;
            mSettings.ColorSettingsHC.TransportBarBackColor = settingsHC.TransportBarBackColor;
            mSettings.ColorSettingsHC.TransportBarLabelBackColor = settingsHC.TransportBarLabelBackColor;
            mSettings.ColorSettingsHC.TransportBarLabelForeColor = settingsHC.TransportBarLabelForeColor;
            mSettings.ColorSettingsHC.WaveformBackColor = settingsHC.WaveformBackColor;
            mSettings.ColorSettingsHC.WaveformBaseLineColor = settingsHC.WaveformBaseLineColor;
            mSettings.ColorSettingsHC.WaveformHighlightedBackColor = settingsHC.WaveformHighlightedBackColor;
            mSettings.ColorSettingsHC.FineNavigationColor = settingsHC.FineNavigationColor;
            mSettings.ColorSettingsHC.HighlightedSectionNodeWithoutSelectionColor = settingsHC.HighlightedSectionNodeWithoutSelectionColor;
            mSettings.ColorSettingsHC.RecordingHighlightPhraseColor = settingsHC.RecordingHighlightPhraseColor;            
            LoadListViewWithColors();
        }

        private void m_lv_ColorPref_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lv_ColorPref.SelectedIndices.Count > 0 && (mNormalColorCombo.SelectedItem != null || mHighContrastCombo.SelectedItem != null))
            {
                ListViewItem selectedItem = m_lv_ColorPref.Items[m_lv_ColorPref.SelectedIndices[0]];
                string desc = selectedItem.Text;
                int index = mNormalColorCombo.FindStringExact(selectedItem.SubItems[1].Text);
                mNormalColorCombo.SelectedIndex = index;
                int indexHC = mHighContrastCombo.FindStringExact(selectedItem.SubItems[2].Text);
                mHighContrastCombo.SelectedIndex = indexHC;
            }
        }

        private void mHighContrastCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (mHighContrastCombo.SelectedItem != null)
                {
                    System.Drawing.ColorConverter col = new ColorConverter();
                    if (mHighContrastCombo.SelectedItem is Color)
                        m_txtBox_HighContrast.BackColor = (Color)mHighContrastCombo.SelectedItem;
                    else if (mHighContrastCombo.SelectedItem is SystemColors)
                        m_txtBox_HighContrast.BackColor = (Color)col.ConvertFromString(mHighContrastCombo.SelectedText);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void m_btn_speak_Click(object sender, EventArgs e)
        {
            try
            {
                string selectedTTSVoice = GetTTSVoiceNameFromTTSCombo();

                if (!string.IsNullOrEmpty(selectedTTSVoice))
                {
                    AudioFormatConverter.TestVoice(selectedTTSVoice, selectedTTSVoice, mSettings);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        // For Developer uncomment to genearte XML

        private void GenerateProfilesForUpdatedSettingsFile()
        {
            string preDefinedProfilesDirectory = GetPredefinedProfilesDirectory();
            string[] filePaths = System.IO.Directory.GetFiles(preDefinedProfilesDirectory, "*.xml");
            for (int i = 0; i < filePaths.Length; i++)
            {
               // m_cb_SelectProfile.SelectedIndex = index;
                
                //string profilePath = GetFilePathOfSelectedPreferencesComboBox();
                string profilePath = filePaths[i];
               // string Profile = m_cb_SelectProfile.Items[m_cb_SelectProfile.SelectedIndex].ToString();
                string Profile =  System.IO.Path.GetFileName(profilePath);

                mTab.SelectedTab = mAudioTab;
                if (Profile == "Basic.xml" || Profile == "Basic.XML")
                {
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = false;
                    m_CheckBoxListView.Items[4].Checked = false;
                    m_CheckBoxListView.Items[5].Checked = false;
                    m_CheckBoxListView.Items[7].Checked = false;
                    m_CheckBoxListView.Items[8].Checked = false;
                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = false;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    m_CheckBoxListView.Items[19].Checked = false;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = false;
                    m_CheckBoxListView.Items[23].Checked = true;
                    m_CheckBoxListView.Items[24].Checked = false;

                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;

                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[11].Checked = false;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    UpdateBoolSettings();
                }
                else if (Profile == "Intermediate.xml" || Profile == "Intermediate.XML")
                {

                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = false;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = false;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = false;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    m_CheckBoxListView.Items[19].Checked = false;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = false;
                    m_CheckBoxListView.Items[23].Checked = true;
                    m_CheckBoxListView.Items[24].Checked = false;

                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;

                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[11].Checked = false;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    UpdateBoolSettings();
                }
                else if (Profile == "Advance.xml" || Profile == "Advance.XML")
                {
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = false;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    m_CheckBoxListView.Items[19].Checked = false;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = false;
                    m_CheckBoxListView.Items[23].Checked = true;
                    m_CheckBoxListView.Items[24].Checked = false;
      
                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;

                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[11].Checked = false;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    UpdateBoolSettings();
                }
                else if (Profile == "Profile-2-SBS.xml" || Profile == "Profile-2-SBS.XML")
                {
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = false;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = false;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = true;
                    m_CheckBoxListView.Items[18].Checked = true;
                    m_CheckBoxListView.Items[19].Checked = false;
                    m_CheckBoxListView.Items[20].Checked = true;
                    m_CheckBoxListView.Items[21].Checked = true;
                    m_CheckBoxListView.Items[22].Checked = false;
                    m_CheckBoxListView.Items[23].Checked = true;
                    m_CheckBoxListView.Items[24].Checked = true;

                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;

                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = true;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    UpdateBoolSettings();


                }
                else if (Profile == "Profile-1-VA.xml" || Profile == "Profile-1-VA.XML")
                {

                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = false;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = true;
                    m_CheckBoxListView.Items[13].Checked = false;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = true;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = true;
                    m_CheckBoxListView.Items[19].Checked = true;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = true;
                    m_CheckBoxListView.Items[23].Checked = false;
                    m_CheckBoxListView.Items[24].Checked = false;

                    mSettings.Audio_PreviewDuration = 7000;

                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = false;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;

                    m_CheckBoxListView.Items[10].Checked = false;
                    m_CheckBoxListView.Items[11].Checked = false;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = true;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = true;
                    m_CheckBoxListView.Items[17].Checked = true;
                    m_CheckBoxListView.Items[18].Checked = true;
                    UpdateBoolSettings();

                }
                else if (Profile == "VA-Insert.xml" || Profile == "VA-Insert.XML")
                {

                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = false;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = false;
                    m_CheckBoxListView.Items[15].Checked = false;
                    m_CheckBoxListView.Items[16].Checked = false;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    m_CheckBoxListView.Items[19].Checked = true;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = true;
                    m_CheckBoxListView.Items[23].Checked = false;
                    m_CheckBoxListView.Items[24].Checked = false;


                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = true;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = true;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = true;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = true;
                    m_CheckBoxListView.Items[17].Checked = true;
                    m_CheckBoxListView.Items[18].Checked = true;
                    UpdateBoolSettings();

                }
                else if (Profile == "VA-Overwrite.xml" || Profile == "VA-Overwrite.XML")
                {

                    m_CheckBoxListView.Items[0].Checked = false;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = false;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = true;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = true;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = true;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = true;
                    m_CheckBoxListView.Items[17].Checked = false;
                    m_CheckBoxListView.Items[18].Checked = false;
                    m_CheckBoxListView.Items[19].Checked = true;
                    m_CheckBoxListView.Items[20].Checked = false;
                    m_CheckBoxListView.Items[21].Checked = false;
                    m_CheckBoxListView.Items[22].Checked = true;
                    m_CheckBoxListView.Items[23].Checked = false;
                    m_CheckBoxListView.Items[24].Checked = false;

                    UpdateBoolSettings();
                    mTab.SelectedTab = mProjectTab;
                    m_CheckBoxListView.Items[0].Checked = true;
                    m_CheckBoxListView.Items[1].Checked = true;
                    m_CheckBoxListView.Items[2].Checked = true;
                    m_CheckBoxListView.Items[3].Checked = true;
                    m_CheckBoxListView.Items[4].Checked = true;
                    m_CheckBoxListView.Items[5].Checked = true;
                    m_CheckBoxListView.Items[6].Checked = true;
                    m_CheckBoxListView.Items[7].Checked = true;
                    m_CheckBoxListView.Items[8].Checked = true;
                    m_CheckBoxListView.Items[9].Checked = false;
                    m_CheckBoxListView.Items[10].Checked = true;
                    m_CheckBoxListView.Items[11].Checked = true;
                    m_CheckBoxListView.Items[12].Checked = true;
                    m_CheckBoxListView.Items[13].Checked = true;
                    m_CheckBoxListView.Items[14].Checked = true;
                    m_CheckBoxListView.Items[15].Checked = true;
                    m_CheckBoxListView.Items[16].Checked = true;
                    m_CheckBoxListView.Items[17].Checked = true;
                    m_CheckBoxListView.Items[18].Checked = true;
                    UpdateBoolSettings();

                }

                System.IO.DirectoryInfo profilePathInProject = System.IO.Directory.GetParent(profilePath);
                while (profilePathInProject.Name != "Obi")
                {
                    profilePathInProject = System.IO.Directory.GetParent(profilePathInProject.ToString());
                }
                string defaultProfilesDirectory = System.IO.Path.Combine(profilePathInProject.ToString(), "profiles");
                defaultProfilesDirectory = defaultProfilesDirectory + "\\" + Profile;
                // remove personal information from settings
                Settings tempSettings = Settings.GetDefaultSettings();
                tempSettings.Project_DefaultPath = mSettings.Project_DefaultPath;
                mSettings.Project_DefaultPath = "";

                tempSettings.Project_PipelineScriptsPath = mSettings.Project_PipelineScriptsPath;
                mSettings.Project_PipelineScriptsPath = "";

                mSettings.Audio_LocalRecordingDirectory = "";

                tempSettings.UserProfile.Name = mSettings.UserProfile.Name;
                mSettings.UserProfile.Name = "";

                tempSettings.UserProfile.Organization = mSettings.UserProfile.Organization;
                mSettings.UserProfile.Organization = "";

                try
                {
                    mSettings.Save(defaultProfilesDirectory);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                // assign personal info back to msettings
                mSettings.Project_DefaultPath = tempSettings.Project_DefaultPath;
                mSettings.Project_PipelineScriptsPath = tempSettings.Project_PipelineScriptsPath;
                mSettings.Audio_LocalRecordingDirectory = tempSettings.Audio_LocalRecordingDirectory;
                mSettings.UserProfile.Name = tempSettings.UserProfile.Name;
                mSettings.UserProfile.Organization = tempSettings.UserProfile.Organization;
            }
        }


        private void m_SelectLevelComboBox_DropDownClosed(object sender, EventArgs e)
        {
           // m_IsComboBoxExpanded = false;
        }

        private void m_SelectLevelComboBox_DropDown(object sender, EventArgs e)
        {
          //  m_IsComboBoxExpanded = true;
        }


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //if (keyData == Keys.Escape)
            //{
            //    m_LeaveOnEscape = true;
            //}
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void m_btnProfileDiscription_Click(object sender, EventArgs e)
        {
            if (m_cb_SelectProfile.SelectedIndex != -1)
            {
                ProfileDescription profileDesc = new ProfileDescription();
                profileDesc.ProfileSelected = m_cb_SelectProfile.SelectedIndex;
                profileDesc.ShowDialog();
                profileDesc.Focus();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            m_Audio_CleanupMaxFileSizeInMB = (int)m_CleanUpFileSizeNumericUpDown.Value;
        }

        private void m_btnLoadProfile_Click(object sender, EventArgs e)
        {
            if (m_rdb_Preferences.Checked)
            {
                //For Developer uncomment Use below lines to genarate XML
                //GenerateProfilesForUpdatedSettingsFile();
                //return;


             
                string profilePath = GetFilePathOfSelectedPreferencesComboBox();
                try
                {
                    if (profilePath != null) LoadPreferenceProfile(profilePath);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                mTransportBar.ShowSwitchProfileContextMenu();
            }//if (m_rdb_Preferences.Checked)
            else if (m_rdb_KeyboardShortcuts.Checked)
            {
                if (m_cb_SelectShorcutsProfile.SelectedIndex == 0)
                {
                    mForm.LoadDefaultKeyboardShortcuts();
                    m_KeyboardShortcuts = mForm.KeyboardShortcuts;

                    m_lvShortcutKeysList.Items.Clear();
                    LoadListviewAccordingToComboboxSelection();
                    MessageBox.Show(Localizer.Message("Preferences_ProfilesShortcutsLoaded"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (m_cb_SelectShorcutsProfile.SelectedIndex >= 0 && m_cb_SelectShorcutsProfile.SelectedIndex < m_cb_SelectShorcutsProfile.Items.Count)
                {   
                    string shortcutsPath = GetPathOfSelectedShortcutsComboBox();

                    try
                    {
                        if (shortcutsPath != null) LoadShortcutsFromFile(shortcutsPath);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }

                }
            }//else if (m_rdb_KeyboardShortcuts.Checked)
         }

        private string GetFilePathOfSelectedPreferencesComboBox()
        {
            string profilePath = null;
            if (m_cb_SelectProfile.SelectedIndex >= 0 && m_cb_SelectProfile.SelectedIndex < m_PredefinedProfilesCount)
            {
                string profileFileName = m_cb_SelectProfile.Items[m_cb_SelectProfile.SelectedIndex].ToString() + ".xml";
                profilePath = System.IO.Path.Combine(GetPredefinedProfilesDirectory(), profileFileName);
            }
            else if (m_cb_SelectProfile.SelectedIndex >= m_PredefinedProfilesCount && m_cb_SelectProfile.SelectedIndex < m_cb_SelectProfile.Items.Count)
            {
                string profileFileName = m_cb_SelectProfile.Items[m_cb_SelectProfile.SelectedIndex].ToString() + ".xml";
                profilePath = System.IO.Path.Combine(GetCustomProfilesDirectory(true), profileFileName);
            }
            return profilePath;
        }

        private string GetPathOfSelectedShortcutsComboBox()
        {
            string shortcutsFileName = m_cb_SelectShorcutsProfile.Items[m_cb_SelectShorcutsProfile.SelectedIndex].ToString() + ".xml";
            string shortcutsPath = System.IO.Path.Combine(GetCustomProfilesDirectory(false), shortcutsFileName);
            return shortcutsPath;
        }

        private Settings m_ProfileLoaded = null;
        private void LoadPreferenceProfile (string profilePath )
        {
            
            if (profilePath != null && System.IO.File.Exists(profilePath))
            {
                Settings saveProfile = Settings.GetSettingsFromSavedProfile(profilePath);
                
                if (m_chkAll.Checked)
                {
                    saveProfile.CopyPropertiesToExistingSettings(mForm.Settings, PreferenceProfiles.All);
                }
                else
                {
                    if (m_chkProject.Checked)
                        saveProfile.CopyPropertiesToExistingSettings(mForm.Settings, PreferenceProfiles.Project);                    

                    if (m_chkAudio.Checked)
                        saveProfile.CopyPropertiesToExistingSettings(mForm.Settings, PreferenceProfiles.Audio);                    

                    if (m_chkLanguage.Checked ) 
                        saveProfile.CopyPropertiesToExistingSettings(mForm.Settings, PreferenceProfiles.UserProfile);

                    if (m_chkColor.Checked )
                        saveProfile.CopyPropertiesToExistingSettings(mForm.Settings, PreferenceProfiles.Colors);
                }
                    
                    mSettings = mForm.Settings;

                    if (m_chkProject.Checked || m_chkAll.Checked) InitializeProjectTab();
                    if (m_chkAudio.Checked || m_chkAll.Checked) InitializeAudioTab();
                    if (m_chkLanguage.Checked || m_chkAll.Checked) InitializeUserProfileTab();
                    if (m_chkColor.Checked || m_chkAll.Checked)
                    {
                        InitializeColorPreferenceTab();
                        m_IsColorChanged = true;
                    }
                    m_ProfileLoaded = saveProfile;
                m_ProfileLoaded.SettingsName = System.IO.Path.GetFileNameWithoutExtension (profilePath);
                
                VerifyChangeInLoadedSettings();
                    MessageBox.Show(Localizer.Message("Preferences_ProfileLoaded"),Localizer.Message("Preference_ProfileCaption"),
                        MessageBoxButtons.OK,MessageBoxIcon.Information);
                
            }// check for path
        }

        private void m_btnSaveProfile_Click(object sender, EventArgs e)
        {
            if (m_rdb_Preferences.Checked)
            {
                try
                {
                    SaveFileDialog fileDialog = new SaveFileDialog();
                    fileDialog.Filter = "*.xml|*.XML";
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {   
                        mSettings.Save(fileDialog.FileName);
                        string tempString = Localizer.Message("Profile_Saved");
                        MessageBox.Show(tempString, tempString, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }//if (m_rdb_Preferences)
            else if (m_rdb_KeyboardShortcuts.Checked)
            {
                SaveFileDialog fileDialog = new SaveFileDialog();
                fileDialog.Filter = "*.xml|*.XML";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        mForm.KeyboardShortcuts.SaveSettingsAs(fileDialog.FileName);
                        string tempString = Localizer.Message("Profile_Saved");
                        MessageBox.Show(tempString, tempString, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }//else if (m_rdb_KeyboardShortcuts)
        }

        private void InitializePreferencesProfileTab ()
        {
            try
            {
                LoadProfilesToComboboxes();
            }
            catch (System.Exception ex)
            {
                ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
            if (mSettings != null && !string.IsNullOrEmpty(mSettings.SettingsName)) m_txtSelectedProfile.Text = mSettings.SettingsName;
            if (mForm.KeyboardShortcuts != null && !string.IsNullOrEmpty(mForm.KeyboardShortcuts.SettingsName)) m_txtSelectedShortcutsProfile.Text = mForm.KeyboardShortcuts.SettingsName;
        }

        private string GetPredefinedProfilesDirectory()
        {
            string appDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
            string defaultProfilesDirectory = System.IO.Path.Combine(appDirectory, "profiles");
            return defaultProfilesDirectory;
        }

        private string GetCustomProfilesDirectory(bool preferencesProfile)
        {
            string permanentSettingsDirectory = System.IO.Directory.GetParent(Settings_Permanent.GetSettingFilePath()).ToString();
            string filesDirectory = preferencesProfile ? "profiles" : "keyboard-shortcuts";
            string customProfilesDirectory = System.IO.Path.Combine(permanentSettingsDirectory, filesDirectory);
            return customProfilesDirectory;
        }

        int m_PredefinedProfilesCount;
        private void LoadProfilesToComboboxes()
        {
            // first load the default profiles
            m_PredefinedProfilesCount = 0;
            string preDefinedProfilesDirectory = GetPredefinedProfilesDirectory();
            if (!System.IO.Directory.Exists(preDefinedProfilesDirectory))
            {
                MessageBox.Show(Localizer.Message("PreferencesPredefined_ProfilesNotExist"), Localizer.Message("PreferencesPredefined_ProfilesNotExistCaption"),
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string[] filePaths = System.IO.Directory.GetFiles(preDefinedProfilesDirectory, "*.xml");
            List<string> filePathsList = new List<string>();
            if (filePaths != null && filePaths.Length > 0)
            {
                //string[] profileFileNames = new string[filePaths.Length];
                for (int i = 0; i < filePaths.Length; i++)
                {
                    filePathsList.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                    //   m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                }
                if (filePathsList.Contains("Basic"))
                {
                    int index = filePathsList.IndexOf("Basic");
                    m_cb_SelectProfile.Items.Add(filePathsList[index]);
                    m_cb_Profile1.Items.Add(filePathsList[index]);
                    m_cb_Profile2.Items.Add(filePathsList[index]);
                    filePathsList.RemoveAt(index);
                }
                if (filePathsList.Contains("Intermediate"))
                {
                    int index = filePathsList.IndexOf("Intermediate");
                    m_cb_SelectProfile.Items.Add(filePathsList[index]);
                    m_cb_Profile1.Items.Add(filePathsList[index]);
                    m_cb_Profile2.Items.Add(filePathsList[index]);
                    filePathsList.RemoveAt(index);
                }
                foreach (string file in filePathsList)
                {
                    m_cb_SelectProfile.Items.Add(file);
                    m_cb_Profile1.Items.Add(file);
                    m_cb_Profile2.Items.Add(file);
                }
                //if (filePathsList.Contains("Advance"))
                //{
                //    m_cb_SelectProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Advance"));
                //}
                //if (filePathsList.Contains("Profile-1-VA"))
                //{
                //    m_cb_SelectProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Profile_1"));
                //}
                //if (filePathsList.Contains("Profile-2-SBS"))
                //{
                //    m_cb_SelectProfile.Items.Add(Localizer.Message("Preferences_Level_ComboBox_Profile_2"));
                //}

            }
            m_PredefinedProfilesCount = m_cb_SelectProfile.Items.Count;
            // now load user defined profiles from the roming folder, the permanent settings are at same location
            string customProfilesDirectory = GetCustomProfilesDirectory(true);
            if (System.IO.Directory.Exists(customProfilesDirectory))
            {
                filePaths = System.IO.Directory.GetFiles(customProfilesDirectory, "*.xml");
                if (filePaths != null && filePaths.Length > 0)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                        m_cb_Profile1.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                        m_cb_Profile2.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));
                    }
                }
            }// directory exists check
            m_cb_SelectProfile.SelectedIndex = -1;
            if (mSettings.Audio_RecordingToolbarProfile1 != null && mSettings.Audio_RecordingToolbarProfile2 != null)
            {
                int tempIndex = m_cb_Profile1.Items.IndexOf(mSettings.Audio_RecordingToolbarProfile1);
                m_cb_Profile1.SelectedIndex = tempIndex;
                tempIndex = m_cb_Profile2.Items.IndexOf(mSettings.Audio_RecordingToolbarProfile2);
                m_cb_Profile2.SelectedIndex = tempIndex;
            }

            // now add keyboard shortcuts profile to the respective combobox
            m_cb_SelectShorcutsProfile.Items.Add("Default Shortcuts");
            string shortcutsProfilesDirectory = GetCustomProfilesDirectory(false);
            if (System.IO.Directory.Exists(shortcutsProfilesDirectory))
            {
                filePaths = System.IO.Directory.GetFiles(shortcutsProfilesDirectory, "*.xml");
                if (filePaths != null && filePaths.Length > 0)
                {
                    for (int i = 0; i < filePaths.Length; i++)
                    {
                        m_cb_SelectShorcutsProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(filePaths[i]));

                    }
                }
            }// directory exists check
            m_cb_SelectShorcutsProfile.SelectedIndex = -1;
        }

        private void m_btnAddProfile_Click(object sender, EventArgs e)
        {
            if (m_rdb_Preferences.Checked)
            {
                BrowseForExistingProfile();
            }
            else if (m_rdb_KeyboardShortcuts.Checked)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.Filter = "*.xml|*.XML";
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        LoadShortcutsFromFile(fileDialog.FileName);
                        // copy the profile file to custom profile directory
                        string shortcutsProfilesDirectory = GetCustomProfilesDirectory(false);
                        if (!System.IO.Directory.Exists(shortcutsProfilesDirectory)) System.IO.Directory.CreateDirectory(shortcutsProfilesDirectory);
                        string newCustomFilePath = System.IO.Path.Combine(shortcutsProfilesDirectory,
                            System.IO.Path.GetFileName(fileDialog.FileName));
                        System.IO.File.Copy(fileDialog.FileName, newCustomFilePath, true);
                        m_cb_SelectShorcutsProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath));
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }//else if (m_rdb_KeyboardShortcuts)
        }

        private void BrowseForExistingProfile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "*.xml|*.XML";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string FileName = System.IO.Path.GetFileNameWithoutExtension(fileDialog.FileName);
                    string preDefinedProfilesDirectory = GetPredefinedProfilesDirectory();
                    string[] defaultProfiles = System.IO.Directory.GetFiles(preDefinedProfilesDirectory);
                    foreach (string profile in defaultProfiles)
                    {
                        if (FileName == System.IO.Path.GetFileNameWithoutExtension(profile))
                        {
                            MessageBox.Show(Localizer.Message("Preferences_DefaultProfileExists"));
                            return;
                        }
                    }
                    if (m_cb_SelectProfile.Items.Contains(FileName))
                    {
                        DialogResult result = MessageBox.Show(Localizer.Message("Preferences_ProfileExists"), Localizer.Message("Caption_Warning"), MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        if (result == DialogResult.Cancel)
                        {
                            return;
                        }
                    }
                   
                    LoadPreferenceProfile(fileDialog.FileName);
                    // copy the profile file to custom profile directory
                    string customProfilesDirectory = GetCustomProfilesDirectory(true);
                    if (!System.IO.Directory.Exists(customProfilesDirectory)) System.IO.Directory.CreateDirectory(customProfilesDirectory);
                    string newCustomFilePath = System.IO.Path.Combine(customProfilesDirectory,
                        System.IO.Path.GetFileName(fileDialog.FileName));
                    System.IO.File.Copy(fileDialog.FileName, newCustomFilePath, true);
                    if (!m_cb_SelectProfile.Items.Contains(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath)))
                    {
                        m_cb_SelectProfile.Items.Add(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath));
                        m_cb_Profile1.Items.Add(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath));
                        m_cb_Profile2.Items.Add(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath));
                       // mTransportBar.InitializeSwitchProfiles();
                        mTransportBar.AddProfileToSwitchProfile(System.IO.Path.GetFileNameWithoutExtension(newCustomFilePath));
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                //if (MessageBox.Show("Please restart Obi for loading the settings",
                    //Localizer.Message("Caption_Information"),
                    //MessageBoxButtons.OKCancel) == DialogResult.OK)
                //{
                    //this.Close();
                //}
            }
        }

        private void m_chkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkAll.Checked)
            {
                m_chkProject.Checked = true;
                m_chkAudio.Checked = true;
                m_chkLanguage.Checked = true;
                m_chkColor.Checked = true;
            }
            else
            {
                m_chkProject.Checked = false;
                m_chkAudio.Checked = false;
                m_chkLanguage.Checked = false;
                m_chkColor.Checked = false;
            }
        }

        private void m_ApplyButton_Click(object sender, EventArgs e)
        {
             UpdateBoolSettings();
             if (this.mTab.SelectedTab == mProjectTab)
             {
                 UpdateProjectSettings();
             }
             if (this.mTab.SelectedTab == mAudioTab)
             {
                 UpdateAudioSettings();
             }
             if (this.mTab.SelectedTab == mUserProfileTab)
             {
                 UpdateUserProfile();
             }
             if (this.mTab.SelectedTab == mKeyboardShortcutTab)
             {
                 AssignKeyboardShortcut();
             }
             if (this.mTab.SelectedTab == mColorPreferencesTab)
             {
                 ApplyColorPreference();
             }

        }

        private void VerifyChangeInLoadedSettings()
        {
            if (m_ProfileLoaded != null && mSettings != null
                && !string.IsNullOrEmpty(m_ProfileLoaded.SettingsName))
            //&& !m_ProfileLoaded.Compare(mSettings,PreferenceProfiles.All))
            {
                string strLoadedProfiles = "";

                if (m_ProfileLoaded.Compare(mSettings, PreferenceProfiles.All))
                {
                    strLoadedProfiles = "all";
                }
                else
                {
                    if (m_ProfileLoaded.Compare(mSettings, PreferenceProfiles.Project))
                    {
                        strLoadedProfiles = "project, ";
                    }
                    if (m_ProfileLoaded.Compare(mSettings, PreferenceProfiles.Audio))
                    {
                        strLoadedProfiles += "audio, ";
                    }
                    if (m_ProfileLoaded.Compare(mSettings, PreferenceProfiles.UserProfile))
                    {
                        strLoadedProfiles += "users profile, ";
                    }
                    if (m_ProfileLoaded.Compare(mSettings, PreferenceProfiles.Colors))
                    {
                        strLoadedProfiles += "colors";
                    }

                }

                if (string.IsNullOrEmpty(strLoadedProfiles))
                {
                    mSettings.SettingsName = "customized";
                }
                else
                {
                    if (strLoadedProfiles.EndsWith(",")) strLoadedProfiles = strLoadedProfiles.Remove(strLoadedProfiles.Length - 2);
                }
                m_txtSelectedProfile.Text = string.Format(Localizer.Message("Preferences_ProfilesStatus"),
                    m_ProfileLoaded.SettingsName, strLoadedProfiles);
                mSettings.SettingsName = m_txtSelectedProfile.Text;
                
            }
            if (mForm.KeyboardShortcuts != null && !string.IsNullOrEmpty(mForm.KeyboardShortcuts.SettingsName)) m_txtSelectedShortcutsProfile.Text = mForm.KeyboardShortcuts.SettingsName;
        }

        private void LoadShortcutsFromFile(string filePath)
        {

            if (filePath != null && System.IO.File.Exists(filePath))
            {
                KeyboardShortcuts_Settings shortCuts = KeyboardShortcuts_Settings.GetKeyboardShortcuts_SettingsFromFile(filePath);
                List<string> descriptions = new List<string> () ;
                descriptions.AddRange(mForm.KeyboardShortcuts.KeyboardShortcutsDescription.Keys);

                foreach (string desc in descriptions)
                {
                    if (shortCuts.KeyboardShortcutsDescription.ContainsKey(desc))
                    {
                        mForm.KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value = shortCuts.KeyboardShortcutsDescription[desc].Value;
                    }
                }
                
                mForm.KeyboardShortcuts.SaveSettings();
                mForm.InitializeKeyboardShortcuts(false);
                
                m_lvShortcutKeysList.Items.Clear();
                LoadListviewAccordingToComboboxSelection();
                mForm.KeyboardShortcuts.SettingsName = System.IO.Path.GetFileNameWithoutExtension(filePath);
                if (mForm.KeyboardShortcuts != null && !string.IsNullOrEmpty(mForm.KeyboardShortcuts.SettingsName)) m_txtSelectedShortcutsProfile.Text = mForm.KeyboardShortcuts.SettingsName;
                MessageBox.Show(Localizer.Message("Preferences_ProfilesShortcutsLoaded"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                //text, caption, button,icons
            }
        }

        private void m_btnRemoveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_rdb_Preferences.Checked)
                {
                    if (m_cb_SelectProfile.SelectedIndex >= m_PredefinedProfilesCount && m_cb_SelectProfile.SelectedIndex < m_cb_SelectProfile.Items.Count)
                    {
                        int indexOfCombobox = m_cb_SelectProfile.SelectedIndex ;
                        string profilePath = GetFilePathOfSelectedPreferencesComboBox();
                        if (System.IO.File.Exists(profilePath)
                            && MessageBox.Show(Localizer.Message("Preferences_ConfirmForDeletingProfile"),Localizer.Message("Caption_Warning"), MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            System.IO.File.Delete(profilePath);
                            m_cb_SelectProfile.Items.RemoveAt(indexOfCombobox);
                            if (m_cb_Profile1.SelectedIndex == indexOfCombobox)
                            {
                                m_cb_Profile1.SelectedIndex = 0;
                                mSettings.Audio_RecordingToolbarProfile1 = m_cb_Profile1.SelectedItem.ToString();                               
                            }
                            if (m_cb_Profile2.SelectedIndex == indexOfCombobox)
                            {
                                m_cb_Profile2.SelectedIndex = 0;
                                mSettings.Audio_RecordingToolbarProfile2 = m_cb_Profile2.SelectedItem.ToString();
                            }
                            m_cb_Profile1.Items.RemoveAt(indexOfCombobox);
                            m_cb_Profile2.Items.RemoveAt(indexOfCombobox);
                           // mTransportBar.InitializeSwitchProfiles();
                            mTransportBar.RemoveProfileFromSwitchProfile(System.IO.Path.GetFileNameWithoutExtension(profilePath));
                        }
                    }
                }
                else if (m_rdb_KeyboardShortcuts.Checked)
                {
                    if (m_cb_SelectShorcutsProfile.SelectedIndex > 0 && m_cb_SelectShorcutsProfile.SelectedIndex < m_cb_SelectShorcutsProfile.Items.Count)
                    {
                        int indexOfCombobox = m_cb_SelectShorcutsProfile.SelectedIndex;
                        string shortcutsFilePath = GetPathOfSelectedShortcutsComboBox();
                        if (System.IO.File.Exists (shortcutsFilePath )
                            && MessageBox.Show(Localizer.Message("Preferences_ConfirmForDeletingProfile"), Localizer.Message("Caption_Warning"), MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            System.IO.File.Delete(shortcutsFilePath);
                            m_cb_SelectShorcutsProfile.Items.RemoveAt(indexOfCombobox);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void m_rdb_Preferences_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rdb_Preferences.Checked)
            {
                m_cb_SelectProfile.Enabled = true;
                m_btnProfileDiscription.Enabled = true;
                m_cb_SelectShorcutsProfile.Enabled = false;
                m_chkProject.Enabled = true;
                m_chkLanguage.Enabled = true;
                m_chkAll.Enabled = true;
                m_chkAudio.Enabled = true;
                m_chkColor.Enabled = true;
            }
        }

        private void m_rdb_KeyboardShortcuts_CheckedChanged(object sender, EventArgs e)
        {
            if (m_rdb_KeyboardShortcuts.Checked)
            {
                m_cb_SelectProfile.Enabled = false;
                m_btnProfileDiscription.Enabled = false;
                m_cb_SelectShorcutsProfile.Enabled = true;
                m_chkProject.Enabled = false;
                m_chkLanguage.Enabled = false;
                m_chkAll.Enabled = false;
                m_chkAudio.Enabled = false;
                m_chkColor.Enabled = false;
            }
        }

        private void m_btnAssignProfile_Click(object sender, EventArgs e)
        {
            mSettings.Audio_RecordingToolbarProfile1 = m_cb_Profile1.SelectedItem.ToString();
            mSettings.Audio_RecordingToolbarProfile2 = m_cb_Profile2.SelectedItem.ToString();
        }

        private void m_ChkRecordInLocalDrive_CheckedChanged(object sender, EventArgs e)
        {
            if (m_chkRecordInLocalDrive.Checked)
            {
                m_txtRecordInLocalDrive.Enabled = true;
                m_btnRecordInLocalDrive.Enabled = true;
                m_txtRecordInLocalDrive.Text = mSettings.Audio_LocalRecordingDirectory;
            }
            else
            {
                m_txtRecordInLocalDrive.Enabled = false;
                m_btnRecordInLocalDrive.Enabled = false;
                m_txtRecordInLocalDrive.Text = "";
            }
        }


    }
    }   