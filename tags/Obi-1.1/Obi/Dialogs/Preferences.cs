using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Obi.Audio;

namespace Obi.Dialogs
    {
    public partial class Preferences : Form
        {
        private ObiForm mForm;                           // parent form
        private Settings mSettings;                      // the settings to modify
        private bool mCanChangeAudioSettings;            // if the settings come from the project they cannot change
        private Presentation mPresentation;              // current presentation (may be null)
        private ProjectView.TransportBar mTransportBar;  // application transport bar


        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences ( ObiForm form, Settings settings, Presentation presentation, ProjectView.TransportBar transportbar )
            {
            InitializeComponent ();
            mForm = form;
            mSettings = settings;
            mPresentation = presentation;
            mTransportBar = transportbar;
            InitializeProjectTab ();
            InitializeAudioTab ();
            InitializeProfileTab ();
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
                sampleRate = (int)mPresentation.DataManager.getDefaultPCMFormat ().getSampleRate ();
                audioChannels = mPresentation.DataManager.getDefaultPCMFormat ().getNumberOfChannels ();
                mCanChangeAudioSettings = mPresentation.getMediaDataManager ().getListOfMediaData ().Count == 0;
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

            mPreviewDurationUpDown.Value = (decimal)(mSettings.PreviewDuration / 1000.0);
            mNoiseLevelComboBox.SelectedIndex =
                mSettings.NoiseLevel == Audio.VuMeter.NoiseLevelSelection.Low ? 0 :
                mSettings.NoiseLevel == Audio.VuMeter.NoiseLevelSelection.Medium ? 1 : 2;
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
                if (ObiForm.CheckProjectDirectory_Safe ( mDirectoryTextbox.Text, false ))
                    mSettings.DefaultPath = mDirectoryTextbox.Text;

                mSettings.PipelineScriptsPath = mPipelineTextbox.Text;
                }
            else
                {
                MessageBox.Show ( Localizer.Message ( "InvalidPaths" ), Localizer.Message ( "Caption_Error" ) );
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
                mTransportBar.AudioPlayer.SetDevice ( mForm, (OutputDevice)mOutputDeviceCombo.SelectedItem );
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

            mSettings.NoiseLevel = mNoiseLevelComboBox.SelectedIndex == 0 ? Audio.VuMeter.NoiseLevelSelection.Low :
                mNoiseLevelComboBox.SelectedIndex == 1 ? Audio.VuMeter.NoiseLevelSelection.Medium : Audio.VuMeter.NoiseLevelSelection.High;
            mSettings.AudioClues = mAudioCluesCheckBox.Checked;
            try
                {
                mSettings.PreviewDuration = (int)Math.Round ( 1000 * mPreviewDurationUpDown.Value );
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
            else if (mSettings.UserProfile.Culture.Name != ((CultureInfo)mCultureBox.SelectedItem).Name )
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
        }
    }