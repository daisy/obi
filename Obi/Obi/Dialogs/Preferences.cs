using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

using Obi.Audio;

namespace Obi.Dialogs
{
    public partial class Preferences : Form
    {
        private string mIdTemplate;             // identifier template
        private string mDefaultXUKDirectory;    // default project directory
        private string mDefaultDAISYDirectory;  // default export directory
        private bool mOpenLastProject;          // automatically open last project
        private bool mEnableTooltips;                 // enable/disable tooltips
        private InputDevice mInputDevice;       // preferred input device
        private OutputDevice mOutputDevice;     // preferred output device
        private int mAudioChannels;             // preferred number of audio channels
        private int mSampleRate;                // preferred sample rate
        private int mBitDepth;                  // preferred bit depth
        private bool mCanChangeAudioSettings;   // if the settings come from the project they cannot change

        private Audio.AudioPlayer mPlayer;      // audio player
        private Audio.AudioRecorder m_Recorder; // Recorder instance

        /// <summary>
        /// Identifier template for new projects
        /// </summary>
        public string IdTemplate
        {
            get { return mIdTemplate; }
        }

        /// <summary>
        /// Default directory for new projects
        /// </summary>
        public string DefaultXUKDirectory
        {
            get { return mDefaultXUKDirectory; }
        }

        /// <summary>
        /// Default directory for exported DAISY books
        /// </summary>
        public string DefaultDAISYDirectory
        {
            get { return mDefaultDAISYDirectory; }
        }

        /// <summary>
        /// Automatically open last open project on startup.
        /// </summary>
        public bool OpenLastProject
        {
            get { return mOpenLastProject; }
        }

        /// <summary>
        /// Enable or disable tooltips.
        /// </summary>
        public bool EnableTooltips
        {
            get { return mEnableTooltips; }
        }

        public OutputDevice OutputDevice
        {
            get { return mOutputDevice; }
        }

        public InputDevice InputDevice
        {
            get { return mInputDevice; }
        }

        public int AudioChannels
        {
            get { return mAudioChannels; }
        }

        public int SampleRate
        {
            get { return mSampleRate; }
        }

        public int BitDepth
        {
            get { return mBitDepth; }
        }

        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences(Settings settings, Project project, UserControls.TransportBar transportbar)
        {
            InitializeComponent();
            mIdTemplate = settings.IdTemplate;
            mTemplateBox.Text = mIdTemplate;
            mDefaultXUKDirectory = settings.DefaultPath;
            mDefaultDAISYDirectory = settings.DefaultExportPath;
            mDirectoryBox.Text = mDefaultXUKDirectory;
            mExportBox.Text = mDefaultDAISYDirectory;
            mLastOpenCheckBox.Checked = settings.OpenLastProject;
            mTooltipsCheckBox.Checked = settings.EnableTooltips;
            mPlayer = transportbar.AudioPlayer;
            m_Recorder = transportbar.Recorder;
            mInputDevice = m_Recorder.InputDevice;
            mOutputDevice = mPlayer.OutputDevice;
            if (project != null && project.HasAudioSettings)
            {
                mSampleRate = project.SampleRate;
                mAudioChannels = project.AudioChannels;
                mBitDepth = project.AudioChannels;
                mCanChangeAudioSettings = false;
            }
            else
            {
                mSampleRate = settings.SampleRate;
                mAudioChannels = settings.AudioChannels;
                mBitDepth = settings.BitDepth;
                mCanChangeAudioSettings = true;
            }
        }

        /// <summary>
        /// Browse for a project directory.
        /// </summary>
        private void mBrowseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mDefaultXUKDirectory;
            dialog.Description = Localizer.Message("default_directory_browser");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mDefaultXUKDirectory = dialog.SelectedPath;
                mDirectoryBox.Text = mDefaultXUKDirectory;
            }
        }

        /// <summary>
        /// Browse for the export directory.
        /// </summary>
        private void mBrowseExportButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mDefaultXUKDirectory;
            dialog.Description = Localizer.Message("export_directory_browser");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mDefaultDAISYDirectory = dialog.SelectedPath;
                mExportBox.Text = mDefaultDAISYDirectory;
            }
        }

        /// <summary>
        /// Validate the changes.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            mIdTemplate = mTemplateBox.Text;
            //mg: rewrite of the above to try to make sure dir exists
            //only if the value changed since showdialog...
            if (mDefaultXUKDirectory != mDirectoryBox.Text)                
            {
                //...do we go through the test to avoid annyoing repeats
                if (ObiForm.CanUseDirectory(mDirectoryBox.Text, false))
                    mDefaultXUKDirectory = mDirectoryBox.Text;                
            }
            if (mDefaultDAISYDirectory != mExportBox.Text &&
                ObiForm.CanUseDirectory(mExportBox.Text, false))
            {
                mDefaultDAISYDirectory = mExportBox.Text;
            }
            mOpenLastProject = mLastOpenCheckBox.Checked;
            mEnableTooltips = mTooltipsCheckBox.Checked;
            mInputDevice = (InputDevice)comboInputDevice.SelectedItem;
            mOutputDevice = (OutputDevice)comboOutputDevice.SelectedItem;
            if (comboChannels.SelectedItem.ToString() == "Mono")
                mAudioChannels = 1;
            else
                mAudioChannels = 2;
            mSampleRate = Convert.ToInt32(comboSampleRate.SelectedItem);
            mBitDepth = 16;
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            comboInputDevice.DataSource = m_Recorder.InputDevices;
            comboInputDevice.SelectedIndex =m_Recorder.InputDevices.IndexOf(mInputDevice);
            comboOutputDevice.DataSource = mPlayer.OutputDevices;
            comboOutputDevice.SelectedIndex = mPlayer.OutputDevices.IndexOf(mOutputDevice);
            ArrayList mSample = new ArrayList();
            // TODO: replace this with a list obtained from the player or the device
            mSample.Add("11025");
            mSample.Add("22050");
            mSample.Add("44100");
            mSample.Add("48000");
            comboSampleRate.DataSource = mSample;
            comboSampleRate.SelectedIndex = mSample.IndexOf(mSampleRate.ToString());
            comboSampleRate.Enabled = mCanChangeAudioSettings;
            ArrayList mArrayChannels = new ArrayList();
            mArrayChannels.Add(Localizer.Message("mono"));
            mArrayChannels.Add(Localizer.Message("stereo"));
            comboChannels.DataSource = mArrayChannels;
            comboChannels.SelectedIndex = mArrayChannels.IndexOf(Localizer.Message(mAudioChannels == 1 ? "mono" : "stereo"));
            comboChannels.Enabled = mCanChangeAudioSettings;
        }
 
        public void SelectProjectTab()
        {
            mTab.SelectedTab = mProjectTab;
        }

        public void SelectAudioTab()
        {
            mTab.SelectedTab = mAudioTab;
        }
    }
}