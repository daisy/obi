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
            mInputDevice = settings.LastInputDevice;
            mOutputDevice = settings.LastOutputDevice;
            mSampleRate = settings.SampleRate;
            mSampleRate = settings.SampleRate;
            mAudioChannels = settings.AudioChannels;
            mBitDepth = settings.BitDepth;
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
            //mDefaultDir = mDirectoryBox.Text;
            //mg: rewrite of the above to try to make sure dir exists
            //only if the value changed since showdialog...
            if (!mDefaultDir.Equals(mDirectoryBox.Text))                
            {
                //...do we go through the test to avoid annyoing repeats
                if (IOUtils.ValidateAndCreateDir(mDirectoryBox.Text))
                    mDefaultDir = mDirectoryBox.Text;                
            }
            
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
            comboInputDevice.SelectedIndex = m_InDevicesList.IndexOf(mInputDevice);
            comboOutputDevice.DataSource = m_OutDevicesList;
            comboOutputDevice.SelectedIndex = m_OutDevicesList.IndexOf(mOutputDevice);
            ArrayList mSample = new ArrayList();
            mSample.Add("11025");
            mSample.Add("22050");
            mSample.Add("44100");
            mSample.Add("48000");
            comboSampleRate.DataSource= mSample;
            comboSampleRate.SelectedIndex = mSample.IndexOf(mSampleRate.ToString());
            ArrayList mArrayChannels = new ArrayList();
            mArrayChannels.Add("Mono");
            mArrayChannels.Add("Stereo");
            comboChannels.DataSource = mArrayChannels;
            if (mAudioChannels == 1)
                comboChannels.SelectedIndex = mArrayChannels.IndexOf("Mono");
            else
                comboChannels.SelectedIndex = mArrayChannels.IndexOf("Stereo");
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