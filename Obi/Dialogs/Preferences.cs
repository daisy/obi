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
        
        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences ( ObiForm form, Settings settings, ObiPresentation presentation, ProjectView.TransportBar transportbar )
            {
            InitializeComponent ();
            mForm = form;
            mSettings = settings;
            mPresentation = presentation;
            mTransportBar = transportbar;
            m_KeyboardShortcuts = mForm.KeyboardShortcuts;
            InitializeProjectTab ();
            InitializeAudioTab ();
            InitializeProfileTab ();
            InitializeKeyboardShortcutsTab();
            m_IsKeyboardShortcutChanged = false;
            }

        // Initialize the project tab
        private void InitializeProjectTab ()
            {
            mDirectoryTextbox.Text = mSettings.DefaultPath;
            mLastOpenCheckBox.Checked = mSettings.OpenLastProject;
            m_ChkAutoSaveInterval.CheckStateChanged -= new System.EventHandler ( this.m_ChkAutoSaveInterval_CheckStateChanged );
            m_ChkAutoSaveInterval.Checked = mSettings.AutoSaveTimeIntervalEnabled;
            m_ChkAutoSaveInterval.CheckStateChanged += new System.EventHandler ( this.m_ChkAutoSaveInterval_CheckStateChanged );
            int intervalMinutes = Convert.ToInt32 ( mSettings.AutoSaveTimeInterval / 60000 );
            MnumAutoSaveInterval.Value = intervalMinutes;
            MnumAutoSaveInterval.Enabled = m_ChkAutoSaveInterval.Checked;
            mChkAutoSaveOnRecordingEnd.Checked = mSettings.AutoSave_RecordingEnd;
            mPipelineTextbox.Text = mSettings.PipelineScriptsPath;
            }

        // Initialize audio tab
        private void InitializeAudioTab ()
            {
            AudioRecorder recorder = mTransportBar.Recorder;
            mInputDeviceCombo.DataSource = recorder.InputDevices;
            mInputDeviceCombo.SelectedIndex = recorder.InputDevices.IndexOf ( recorder.InputDevice );

            AudioPlayer player = mTransportBar.AudioPlayer;
            mOutputDeviceCombo.DataSource = player.OutputDevices;
            mOutputDeviceCombo.SelectedIndex = player.OutputDevices.IndexOf ( player.OutputDevice );

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
                sampleRate = mSettings.SampleRate;
                audioChannels = mSettings.AudioChannels;
                mCanChangeAudioSettings = true;
                }
            ArrayList sampleRates = new ArrayList ();
            // TODO: replace this with a list obtained from the player or the device
            sampleRates.Add ( "11025" );
            sampleRates.Add ( "22050" );
            sampleRates.Add ( "44100" );
            sampleRates.Add ( "48000" );
            mSampleRateCombo.DataSource = sampleRates;
            mSampleRateCombo.SelectedIndex = sampleRates.IndexOf ( sampleRate.ToString () );
            mSampleRateCombo.Visible = mCanChangeAudioSettings;
            mSampleRateTextbox.Text = sampleRate.ToString ();
            mSampleRateTextbox.Visible = !mCanChangeAudioSettings;
            ArrayList channels = new ArrayList ();
            channels.Add ( Localizer.Message ( "mono" ) );
            channels.Add ( Localizer.Message ( "stereo" ) );
            mChannelsCombo.DataSource = channels;
            mChannelsCombo.SelectedIndex = channels.IndexOf ( Localizer.Message ( audioChannels == 1 ? "mono" : "stereo" ) );
            mChannelsCombo.Visible = mCanChangeAudioSettings;
            mChannelsTextbox.Text = Localizer.Message ( audioChannels == 1 ? "mono" : "stereo" );
            mChannelsTextbox.Visible = !mCanChangeAudioSettings;
            
            if(m_cbOperation.SelectedIndex == 0)
                m_OperationDurationUpDown.Value = (decimal)(mSettings.NudgeTimeMs);
            if(m_cbOperation.SelectedIndex == 1)
                m_OperationDurationUpDown.Value = (decimal)(mSettings.PreviewDuration);
            if (m_cbOperation.SelectedIndex == 2)
                m_OperationDurationUpDown.Value = (decimal)(mSettings.ElapseBackTimeInMilliseconds);
            mNoiseLevelComboBox.SelectedIndex =
                mSettings.NoiseLevel == AudioLib.VuMeter.NoiseLevelSelection.Low ? 0 :
                mSettings.NoiseLevel == AudioLib.VuMeter.NoiseLevelSelection.Medium ? 1 : 2;
            mAudioCluesCheckBox.Checked = mSettings.AudioClues;
            }

        // Initialize user profile preferences
        private void InitializeProfileTab ()
            {
            mFullNameTextbox.Text = mSettings.UserProfile.Name;
            mOrganizationTextbox.Text = mSettings.UserProfile.Organization;
            mCultureBox.Items.AddRange ( CultureInfo.GetCultures ( CultureTypes.AllCultures ) );
            mCultureBox.SelectedItem = mSettings.UserProfile.Culture;
            }

        private void InitializeKeyboardShortcutsTab()
        {
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
        }

        /// <summary>
        /// Browse for a project directory.
        /// </summary>
        private void mBrowseButton_Click ( object sender, EventArgs e )
            {
            SelectFolder ( mSettings.DefaultPath, "default_directory_browser", mDirectoryTextbox );
            }

        private void mPipelineBrowseButton_Click ( object sender, EventArgs e )
            {
            SelectFolder ( mSettings.PipelineScriptsPath, "pipeline_path_browser", mPipelineTextbox );
            }

        private void SelectFolder ( string path, string description, TextBox textBox )
            {
            FolderBrowserDialog dialog = new FolderBrowserDialog ();
            dialog.SelectedPath = path;
            dialog.Description = Localizer.Message ( description );
            if (dialog.ShowDialog () == DialogResult.OK) textBox.Text = dialog.SelectedPath;
            }

        // Update settings
        private void mOKButton_Click ( object sender, EventArgs e )
            {
            if (UpdateProjectSettings ()
            && UpdateAudioSettings ()
            && UpdateUserProfile ())
                {
                    if (m_IsKeyboardShortcutChanged)
                    {
                        mForm.KeyboardShortcuts.SaveSettings();
                        mForm.InitializeKeyboardShortcuts(false);
                    }
                DialogResult = DialogResult.OK;
                Close ();
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
            if (System.IO.Directory.Exists ( mDirectoryTextbox.Text )
                && System.IO.Directory.Exists ( mPipelineTextbox.Text ))
                {
                    string[] getFiles = System.IO.Directory.GetFiles(mPipelineTextbox.Text, "*.taskScript", System.IO.SearchOption.AllDirectories); 
                if (ObiForm.CheckProjectDirectory_Safe ( mDirectoryTextbox.Text, false ))
                    mSettings.DefaultPath = mDirectoryTextbox.Text;
                if (getFiles.Length > 0)
                    mSettings.PipelineScriptsPath = mPipelineTextbox.Text;
                else
                {
                    MessageBox.Show(Localizer.Message("Preferences_PipelineScriptNotFound"));
                    returnVal = false;
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

            mSettings.OpenLastProject = mLastOpenCheckBox.Checked;
            mSettings.AutoSave_RecordingEnd = mChkAutoSaveOnRecordingEnd.Checked;
            mSettings.AutoSaveTimeIntervalEnabled = m_ChkAutoSaveInterval.Checked;
            try
                {
                mSettings.AutoSaveTimeInterval = Convert.ToInt32 ( MnumAutoSaveInterval.Value * 60000 );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                returnVal = false;
                }
            mForm.SetAutoSaverInterval = mSettings.AutoSaveTimeInterval;
            if (mSettings.AutoSaveTimeIntervalEnabled) mForm.StartAutoSaveTimeInterval ();
            return returnVal;
            }

        // Update audio settings
        private bool UpdateAudioSettings ()
            {
            try
                {
                mTransportBar.AudioPlayer.SetOutputDevice( mForm, ((OutputDevice)mOutputDeviceCombo.SelectedItem).Name );
                mTransportBar.Recorder.InputDevice = (InputDevice)mInputDeviceCombo.SelectedItem;
                mSettings.LastInputDevice = ((InputDevice)mInputDeviceCombo.SelectedItem).Name;
                mSettings.LastOutputDevice = ((OutputDevice)mOutputDeviceCombo.SelectedItem).Name;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                return false;
                }
            if (mCanChangeAudioSettings)
                {
                mSettings.AudioChannels = mChannelsCombo.SelectedItem.ToString () == Localizer.Message ( "mono" ) ? 1 : 2;
                mSettings.SampleRate = Convert.ToInt32 ( mSampleRateCombo.SelectedItem );
                if (mPresentation != null)
                    {
                    mPresentation.UpdatePresentationAudioProperties ( mSettings.AudioChannels, mSettings.BitDepth, mSettings.SampleRate );
                    }
                }

            mSettings.NoiseLevel = mNoiseLevelComboBox.SelectedIndex == 0 ? AudioLib.VuMeter.NoiseLevelSelection.Low :
                mNoiseLevelComboBox.SelectedIndex == 1 ? AudioLib.VuMeter.NoiseLevelSelection.Medium : AudioLib.VuMeter.NoiseLevelSelection.High;
            mSettings.AudioClues = mAudioCluesCheckBox.Checked;
            try
                {
                  mSettings.NudgeTimeMs = (int)m_Nudge;
                  mSettings.PreviewDuration = (int)m_Preview;
                  mSettings.ElapseBackTimeInMilliseconds = (int)m_Elapse;
                 }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                return false;
                }
            return true;
            }

        // Update user profile
        private bool UpdateUserProfile ()
            {
            mSettings.UserProfile.Name = mFullNameTextbox.Text;
            mSettings.UserProfile.Organization = mOrganizationTextbox.Text;

            if (mCultureBox.SelectedItem.ToString () == "en-US"
                || mCultureBox.SelectedItem.ToString () == "hi-IN")
                {
                if (mSettings.UserProfile.Culture.ToString () != mCultureBox.SelectedItem.ToString ())
                    MessageBox.Show ( Localizer.Message ( "Preferences_RestartForCultureChange" ) );
                }
            else if (mSettings.UserProfile.Culture.Name != ((CultureInfo)mCultureBox.SelectedItem).Name)
                {
                // show this message only if selected culture is different from application's existing culture.
                MessageBox.Show ( string.Format ( Localizer.Message ( "Peferences_GUIDonotSupportCulture" ),
                    mCultureBox.SelectedItem.ToString () ) );
                }
            mSettings.UserProfile.Culture = (CultureInfo)mCultureBox.SelectedItem;
            return true;
            }

        private void m_ChkAutoSaveInterval_CheckStateChanged ( object sender, EventArgs e )
            {
            MnumAutoSaveInterval.Enabled = m_ChkAutoSaveInterval.Checked;
            }

        private void m_lvShortcutKeysList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lvShortcutKeysList.SelectedIndices.Count > 0 && m_lvShortcutKeysList.SelectedIndices[0] >= 0)
            {
                string desc = m_lvShortcutKeysList.Items[m_lvShortcutKeysList.SelectedIndices[0]].Text;
                m_txtShortcutKeys.Text = m_KeyboardShortcuts.KeyboardShortcutsDescription[desc].Value.ToString();
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
            }
        }

        private void m_txtShortcutKeys_Enter(object sender, EventArgs e)
        {
            m_txtShortcutKeys.SelectAll();
        }
        
        private void m_btnAssign_Click(object sender, EventArgs e)
        {
            if (m_lvShortcutKeysList.SelectedIndices.Count > 0 && m_lvShortcutKeysList.SelectedIndices[0] >= 0 && m_CapturedKey != Keys.None)
            {
                if( m_KeyboardShortcuts.IsDuplicate(m_CapturedKey))
                {
                    MessageBox.Show(Localizer.Message("KeyboardShortcut_DuplicateMessage"), Localizer.Message ("Caption_Error"));
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
                m_OperationDurationUpDown.Value = (int)mSettings.NudgeTimeMs;
                m_OperationDurationUpDown.Increment = 50;
            }
            if(m_cbOperation.SelectedIndex == 1)
            {
                m_OperationDurationUpDown.Value = mSettings.PreviewDuration;
                m_OperationDurationUpDown.Increment = 100;
            }
            if(m_cbOperation.SelectedIndex == 2)
            {
                m_OperationDurationUpDown.Value = mSettings.ElapseBackTimeInMilliseconds;
                m_OperationDurationUpDown.Increment = 150;
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
        }
        }
    }