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

        private int mAudioChannels;  // preferred number of audio channels
        private int mSampleRate;     // preferred sample rate
        private int mBitDepth;       // preferred bit depth

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

        Settings settings = new Settings();

        ArrayList m_InDevicesList = new ArrayList();
        ArrayList m_OutDevicesList = new ArrayList();
        AudioRecorder ob_AudioRecorder = new AudioRecorder();
        AudioPlayer ob_AudioPlayer = new AudioPlayer();

        public Preferences(string template, string dir)
        {
            InitializeComponent();
            mIdTemplate = template;
            mTemplateBox.Text = mIdTemplate;
            mDefaultDir = dir;
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
            mIdTemplate      = mTemplateBox.Text;
            mDefaultDir = mDirectoryBox.Text;
            settings.LastInputDevice = comboInputDevice.SelectedItem.ToString();
            settings.LastOutputDevice = comboOutputDevice.SelectedItem.ToString();
        }

        private void Preferences_Load(object sender, EventArgs e)
        {
            m_InDevicesList = ob_AudioRecorder.GetInputDevices();
            m_OutDevicesList = ob_AudioPlayer.GetOutputDevices();
            comboInputDevice.DataSource = m_InDevicesList;
            comboOutputDevice.DataSource = m_OutDevicesList;
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