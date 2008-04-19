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
        private bool mEnableTooltips;           // enable/disable tooltips
        private InputDevice mInputDevice;       // preferred input device
        private OutputDevice mOutputDevice;     // preferred output device
        private int mAudioChannels;             // preferred number of audio channels
        private int mSampleRate;                // preferred sample rate
        private int mBitDepth;                  // preferred bit depth
        private bool mCanChangeAudioSettings;   // if the settings come from the project they cannot change
        private Presentation mPresentation; // presentation of active project

        private Audio.AudioPlayer mPlayer;      // audio player
        private Audio.AudioRecorder mRecorder;  // Recorder instance

        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences(Settings settings, Presentation presentation, ProjectView.TransportBar transportbar)
        {
            InitializeComponent();
            mDefaultXUKDirectory = settings.DefaultPath;
            mDefaultDAISYDirectory = settings.DefaultExportPath;
            mDirectoryBox.Text = mDefaultXUKDirectory;
            mExportBox.Text = mDefaultDAISYDirectory;
            mLastOpenCheckBox.Checked = settings.OpenLastProject;
            mTooltipsCheckBox.Checked = settings.EnableTooltips;
            mPlayer = transportbar.AudioPlayer;
            mRecorder = transportbar.Recorder;
            mInputDevice = mRecorder.InputDevice;
            mOutputDevice = mPlayer.OutputDevice;
            mPresentation = presentation;

            if (presentation != null &&    presentation.getMediaDataManager().getListOfMediaData().Count > 0 )
            {
                mSampleRate = (int)  presentation.DataManager.getDefaultPCMFormat ().getSampleRate () ;
                mAudioChannels = presentation.DataManager.getDefaultPCMFormat ().getNumberOfChannels () ;
                mBitDepth = presentation.DataManager.getDefaultPCMFormat().getBitDepth() ;
                mCanChangeAudioSettings = false ;
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
        /// Current number of audio channels.
        /// </summary>
        public int AudioChannels { get { return mAudioChannels; } }

        /// <summary>
        /// Current bit depth.
        /// </summary>
        public int BitDepth { get { return mBitDepth; } }

        /// <summary>
        /// Default directory for exported DAISY books
        /// </summary>
        public string DefaultDAISYDirectory { get { return mDefaultDAISYDirectory; } }

        /// <summary>
        /// Default directory for new projects
        /// </summary>
        public string DefaultXUKDirectory { get { return mDefaultXUKDirectory; } }

        /// <summary>
        /// Enable or disable tooltips.
        /// </summary>
        public bool EnableTooltips { get { return mEnableTooltips; } }

        /// <summary>
        /// Identifier template for new projects
        /// </summary>
        public string IdTemplate { get { return mIdTemplate; } }

        /// <summary>
        /// The current input device.
        /// </summary>
        public InputDevice InputDevice { get { return mInputDevice; } }

        /// <summary>
        /// Automatically open last open project on startup.
        /// </summary>
        public bool OpenLastProject { get { return mOpenLastProject; } }

        /// <summary>
        /// The current output device.
        /// </summary>
        public OutputDevice OutputDevice { get { return mOutputDevice; } }

        /// <summary>
        /// Current sample rate.
        /// </summary>
        public int SampleRate { get { return mSampleRate; } }

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

            UpdatePresentationAudioProperties(mAudioChannels, mBitDepth, mSampleRate);
        }

        private bool UpdatePresentationAudioProperties(int channels, int bitDepth, int samplingRate)
        {
            if (mPresentation != null && mCanChangeAudioSettings)
            {
                try
                {
                    mPresentation.DataManager.setDefaultNumberOfChannels((ushort)channels);
                    mPresentation.DataManager.setDefaultBitDepth((ushort)bitDepth);
                    mPresentation.DataManager.setDefaultSampleRate((uint)SampleRate);
                    mPresentation.DataManager.setEnforceSinglePCMFormat(true);

                    Settings.GetSettings().AudioChannels = channels;
                    Settings.GetSettings().BitDepth = bitDepth;
                    Settings.GetSettings().SampleRate = SampleRate;

                    return true;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                                    }
                        }
                        return false;
        }
        
        private void Preferences_Load(object sender, EventArgs e)
        {
            comboInputDevice.DataSource = mRecorder.InputDevices;
            comboInputDevice.SelectedIndex =mRecorder.InputDevices.IndexOf(mInputDevice);
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
 
        public void SelectProjectTab() { mTab.SelectedTab = mProjectTab; }
        public void SelectAudioTab() { mTab.SelectedTab = mAudioTab; }
    }
}