using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using VirtualAudioBackend;
using System.Collections;
namespace Obi.Dialogs
{
    public partial class Preferences : Form
    {
        private string mIdTemplate;  // identifier template
        private string mDefaultDir;  // default project directory

        public string IdTemplate
        {
            get
            {
                return mIdTemplate;
            }
        }

        public string DefaultDir
        {
            get
            {
                return mDefaultDir;
            }
        }

        private string mOutputDevice;  // preferred output device
        private string mInputDevice;   // preferred input device
        private int mAudioChannels;    // preferred number of audio channels
        private int mSampleRate;       // preferred sample rate
        private int mBitDepth;         // preferred bit depth

        public string OutputDevice
        {
            get
            {
                return mOutputDevice;
            }
        }

        public string InputDevice
        {
            get
            {
                return mInputDevice;
            }
        }

        public int AudioChannels
        {
            get
            {
                return mAudioChannels;
            }
        }

        public int SampleRate
        {
            get
            {
                return mSampleRate;
            }
        }

        public int BitDepth
        {
            get
            {
                return mBitDepth;
            }
        }

        ArrayList m_InDevicesList = new ArrayList();
        ArrayList m_OutDevicesList = new ArrayList();
        AudioRecorder ob_AudioRecorder = AudioRecorder.Instance;
        AudioPlayer ob_AudioPlayer = AudioPlayer.Instance;

        /// <summary>
        /// Initialize the preferences with the user settings.
        /// </summary>
        public Preferences(Settings settings)
        {
            InitializeComponent();
            mIdTemplate = settings.IdTemplate;
            mTemplateBox.Text = mIdTemplate;
            mDefaultDir = settings.DefaultPath;
            mDirectoryBox.Text = mDefaultDir;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = mDefaultDir;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                mDefaultDir = dialog.SelectedPath;
                mDirectoryBox.Text = mDefaultDir;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mIdTemplate = mTemplateBox.Text;
            mDefaultDir = mDirectoryBox.Text;
            mInputDevice = comboInputDevice.SelectedItem.ToString();
            mOutputDevice = comboOutputDevice.SelectedItem.ToString();
            if (comboChannels.SelectedItem.ToString() == "Mono")
                mAudioChannels = 1;
            else
                mAudioChannels = 2;
            mSampleRate = Convert.ToInt32(comboSampleRate.SelectedItem);
            mBitDepth = 16;
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            m_InDevicesList = ob_AudioRecorder.GetInputDevices();
            m_OutDevicesList = ob_AudioPlayer.GetOutputDevices();
            comboInputDevice.DataSource = m_InDevicesList;
            comboOutputDevice.DataSource = m_OutDevicesList;
            ArrayList mSample = new ArrayList();
            mSample.Add("11025");
            mSample.Add("22050");
            mSample.Add("44100");
            mSample.Add("48000");
            comboSampleRate.DataSource= mSample;
            comboSampleRate.SelectedItem = mSample[2];
            ArrayList mArrayChannels = new ArrayList();
            mArrayChannels.Add("Mono");
            mArrayChannels.Add("Stereo");
            comboChannels.DataSource = mArrayChannels;
            comboChannels.SelectedItem = mArrayChannels[0];
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