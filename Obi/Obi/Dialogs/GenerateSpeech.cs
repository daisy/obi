using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Speech.Synthesis;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Win32;
using System.IO;
using urakawa;
using System.Speech.AudioFormat;
using AudioLib;

namespace Obi.Dialogs
{
    public partial class GenerateSpeech : Form
    {
        private System.Speech.Synthesis.SpeechSynthesizer? synthsizer;
        private List<InstalledVoice>? voiceList;
        private ProjectView.ProjectView m_ProjectView;
        private ObiPresentation m_Presentation;
        private List<string> m_voices;
        private Settings m_Settings;

        private Microsoft.CognitiveServices.Speech.SpeechSynthesizer? m_AzureSpeechSynthesizer;
        private readonly SpeechConfig m_AzureSpeechConfig;

      
        public GenerateSpeech(ProjectView.ProjectView view, ObiPresentation presentation, Settings settings)
        {
            InitializeComponent();
            m_ProjectView = view;
            m_Presentation = presentation;
            m_Settings = settings;
            this.synthsizer = null;
            this.voiceList = null;

            string directory = AppDomain.CurrentDomain.BaseDirectory;
            string keyFile = directory + "\\key.csv ";
            if (File.Exists(keyFile))
            {
                string[] lines = File.ReadAllLines(keyFile);
                string key = lines[0];
                string region = lines[1];
                m_AzureSpeechConfig = SpeechConfig.FromSubscription(key, region);
            }
            else
            {
                m_AddAzureVoiceBtn.Enabled = false;
                m_DeleteAzureVoiceBtn.Enabled = false;
                m_AzureRbtn.Enabled = false;
            }
            bool rv = LoadInstalledVoices();
            InitializeEnabledState(rv);
        }


        private bool LoadInstalledVoices()
        {

            using (System.Speech.Synthesis.SpeechSynthesizer synth = new System.Speech.Synthesis.SpeechSynthesizer())
            {
                voiceList = new List<InstalledVoice>();
                foreach (InstalledVoice voice in synth.GetInstalledVoices())
                {
                    voiceList.Add(voice);
                    m_VoiceSelectionCb.Items.Add($"{voice.VoiceInfo.Name}-" +
                        $"{voice.VoiceInfo.Gender}-{voice.VoiceInfo.Culture}");
                }
                if (voiceList.Count == 0)
                    return (false);
            }

            m_VoiceSelectionCb.SelectedIndex = 0;
            return (true);

        }

        public void AddAzureVoices(string voice)
        {
            if (!m_Settings.AzureVoices.Contains(voice))
            {
                m_VoiceSelectionCb.Items.Add(voice);
                m_Settings.AzureVoices.Add(voice);
            }
            else
            {
                MessageBox.Show("Voice is already there in the list");
            }
        }

        private void LoadAzureVoicesAsync()
        {
            m_SpeedTb.Minimum = -50;
            m_SpeedTb.Maximum = 50;

            if (m_Settings.AzureVoices != null)
            {
                if (m_Settings.AzureVoices.Count == 0)
                {
                    m_Settings.AzureVoices.Add("en-GB-SoniaNeural");
                    m_Settings.AzureVoices.Add("en-US-JennyNeural");
                }
                if (m_Settings.AzureVoices.Count > 0)
                {
                    foreach (var voice in m_Settings.AzureVoices)
                    {
                        m_VoiceSelectionCb.Items.Add(voice);
                    }
                }
            }



            if (m_VoiceSelectionCb.Items.Count > 0)
            {
                m_VoiceSelectionCb.SelectedIndex = 0;
            }

        }


        private void InitializeEnabledState(bool state)
        {
            if (state == false)
            {
                MessageBox.Show("Missing:  Installed Voices");
            }

            this.m_PreviewBtn.Enabled = state;
            this.m_StopBtn.Enabled = state;
            this.m_ClearBtn.Enabled = state;
        }

        private string? GetVoiceName()
        {
            if (this.m_VoiceSelectionCb.Items.Count == 0 || this.m_VoiceSelectionCb.SelectedIndex < 0)
            {
                MessageBox.Show("You must first install some voices");
                return null;
            }
            else
            {

                string selectedVoice = (string)this.m_VoiceSelectionCb.SelectedItem;

                var buckets = selectedVoice.ToString().Split("-");

                if (voiceList != null)
                {
                    for (int j = 0; j < voiceList.Count; j++)
                    {
                        var VoiceFromList = voiceList[j].VoiceInfo.Name.ToString().Trim().ToLower();
                        var VoiceSelected = buckets[0].ToString().Trim().ToLower();
                        if (VoiceFromList == VoiceSelected)
                            return (voiceList[j].VoiceInfo.Name.ToString());
                    }
                }
                return null;
            }
        }

        private int GetVoiceRate()
        {
            int voiceRate = 0;
            voiceRate = m_SpeedTb.Value;
            return (voiceRate);
        }

        private void InitializeSynthsizer()
        {
            if (m_BuildInRbtn.Checked)
            {
                this.synthsizer = new System.Speech.Synthesis.SpeechSynthesizer();
                this.synthsizer.SpeakStarted += Synthsizer_SpeakStarted;
                this.synthsizer.SpeakCompleted += Synthsizer_SpeakCompleted;
                this.synthsizer.SelectVoice(GetVoiceName());
                this.synthsizer.Rate = GetVoiceRate();
            }
        }

        private void Synthsizer_SpeakStarted(object sender, SpeakStartedEventArgs e)
        {
            AzureSpeechSynthesizer_SynthesisStarted();
        }

        private void AzureSpeechSynthesizer_SynthesisStarted()
        {
            m_PreviewBtn.Enabled = false;
            m_ClearBtn.Enabled = false;
            m_SpeedTb.Enabled = false;
            m_VoiceSelectionCb.Enabled = false;
            m_AddAzureVoiceBtn.Enabled = false;
            m_DeleteAzureVoiceBtn.Enabled = false;
            m_AzureRbtn.Enabled = false;
            m_BuildInRbtn.Enabled = false;
            m_GenerateBtn.Enabled = false;
            m_AddAzureKeyBtn.Enabled = false;
            m_CloseBtn.Enabled = false;
        }

        private void AzureSpeechSynthesizer_SynthesisCompleted()
        {
            m_PreviewBtn.Enabled = true;
            m_ClearBtn.Enabled = true;
            m_SpeedTb.Enabled = true;
            m_VoiceSelectionCb.Enabled = true;
            m_AddAzureVoiceBtn.Enabled = true;
            m_DeleteAzureVoiceBtn.Enabled = true;
            m_AzureRbtn.Enabled = true;
            m_BuildInRbtn.Enabled = true;
            m_GenerateBtn.Enabled = true;
            m_AddAzureKeyBtn.Enabled = true;
            m_CloseBtn.Enabled = true;
        }


        private void Synthsizer_SpeakCompleted(object? sender, SpeakCompletedEventArgs e)
        {
            if (this.synthsizer != null)
            {
                this.synthsizer.Dispose();
                this.synthsizer = null;
            }
                  
            AzureSpeechSynthesizer_SynthesisCompleted();

        }



        private async void m_PreviewBtn_ClickAsync(object sender, EventArgs e)
        {

            if (m_TextToSpeechTb.Text.ToString().Trim().Length == 0)
            {
                MessageBox.Show("Please provide some text to continue");
                return;
            }
            if (m_BuildInRbtn.Checked)
            {
                if (synthsizer != null)
                {
                    synthsizer.Dispose();
                }


                InitializeSynthsizer();

                if (synthsizer != null)
                {
                    synthsizer.SpeakAsync(this.m_TextToSpeechTb.Text);
                }

                this.m_VoiceSelectionCb.Enabled = false;
                this.m_SpeedTb.Enabled = false;

            }
            else if (m_AzureRbtn.Checked)
            {
                if (m_AzureSpeechSynthesizer != null)
                {
                    m_AzureSpeechSynthesizer.Dispose();
                    m_AzureSpeechSynthesizer = null;
                }
                if (m_AzureSpeechConfig != null)
                {
                    m_AzureSpeechSynthesizer = new(m_AzureSpeechConfig);
                    //SetPlayingState();
                    string voiceName = m_VoiceSelectionCb.SelectedItem.ToString();
                    InitializeSynthsizer();
                    //string ssml = GenerateSSML(m_TextToSpeechTb.Text, m_VoiceSelectionCb.SelectedItem.ToString(), (int)m_SpeedTb.Value, 0); 
                    string ssml = GenerateSSML(m_TextToSpeechTb.Text, voiceName, (int)m_SpeedTb.Value, 0);
                    AzureSpeechSynthesizer_SynthesisStarted();
                    m_GenerateBtn.Enabled = false;
                    await m_AzureSpeechSynthesizer.SpeakSsmlAsync(ssml);
                    AzureSpeechSynthesizer_SynthesisCompleted();
                    m_GenerateBtn.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Kindly add Azure Subcription details using Add Azure Key");
                }
            }
        }

        private void m_ClearBtn_Click(object sender, EventArgs e)
        {
            this.m_TextToSpeechTb.Text = String.Empty;
            this.m_PreviewBtn.Enabled = true;
            this.m_TextToSpeechTb.SelectedText = String.Empty;
            this.m_VoiceSelectionCb.Enabled = true;
            this.m_SpeedTb.Enabled = true;

            if (this.synthsizer != null)
            {
                this.synthsizer.Dispose();
                this.synthsizer = null;
            }
            if(m_AzureSpeechSynthesizer!= null)
            {
                m_AzureSpeechSynthesizer.Dispose();
                m_AzureSpeechSynthesizer = null;
            }

        }

      

        private async void m_StopBtn_Click(object sender, EventArgs e)
        {
            if (m_BuildInRbtn.Checked)
            {
             
                if (this.synthsizer != null)
                {
                    this.synthsizer.Dispose();
                    this.synthsizer = null;
                }
            }
            else if(m_AzureRbtn.Checked)
            {
                m_AzureSpeechSynthesizer?.StopSpeakingAsync();
              
            }
            AzureSpeechSynthesizer_SynthesisCompleted();

        }
     

        private async void m_GenerateBtn_ClickAsync(object sender, EventArgs e)
        {
            if (m_TextToSpeechTb.Text.ToString().Trim().Length == 0)
            {
                MessageBox.Show("Please provide some text to continue");
                return;
            }

            string tempDirectoryName = "Generate Speech";
            string directoryFullPath = Path.Combine(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                tempDirectoryName);
            if (Directory.Exists(directoryFullPath)) Directory.Delete(directoryFullPath, true);
            Directory.CreateDirectory(directoryFullPath);

          
            if (!Directory.Exists(directoryFullPath))
            {
                Directory.CreateDirectory(directoryFullPath);
            }
            string fileName = directoryFullPath + "\\ temp.wav";


            if (m_BuildInRbtn.Checked == true)
            {

                if (synthsizer != null)
                {
                    synthsizer.Dispose();
                }
                if (synthsizer == null)
                {
                    InitializeSynthsizer();
                }

                if (synthsizer != null)
                {
                    // synthsizer.SelectVoice(voice);
                    //  synthsizer.Rate = (int)mSpeedTrackBar.Value;
                    AudioLibPCMFormat pcmformat = m_Presentation.MediaDataManager.DefaultPCMFormat.Data;
                    SpeechAudioFormatInfo formatInfo = new SpeechAudioFormatInfo((int)pcmformat.SampleRate, AudioBitsPerSample.Sixteen, pcmformat.NumberOfChannels == 2 ? AudioChannel.Stereo : AudioChannel.Mono);
                    m_StopBtn.Enabled = false;
                    synthsizer.SetOutputToWaveFile(fileName, formatInfo);

                    synthsizer.Speak(m_TextToSpeechTb.Text);
                    synthsizer.SetOutputToDefaultAudioDevice();
                }
                if(File.Exists(fileName))
                {
                    m_ProjectView.ImportPhrasesTTS(fileName);
                }
                if (Directory.Exists(directoryFullPath))
                {
                    File.Delete(fileName);
                    Directory.Delete(directoryFullPath, true);
                }
                this.Close();

            }
            else
            {
                if (m_AzureSpeechConfig != null)
                {
                    if (m_AzureSpeechSynthesizer != null)
                    {
                        m_AzureSpeechSynthesizer?.Dispose();
                        m_AzureSpeechSynthesizer = null;
                    }
                    m_AzureSpeechSynthesizer = new(m_AzureSpeechConfig, AudioConfig.FromWavFileOutput(fileName));

                    string voiceName = m_VoiceSelectionCb.SelectedItem.ToString();
                    InitializeSynthsizer();
                    string ssml = GenerateSSML(m_TextToSpeechTb.Text, voiceName, (int)m_SpeedTb.Value, 0);
                    AzureSpeechSynthesizer_SynthesisStarted();
                    m_StopBtn.Enabled = false;
                    await m_AzureSpeechSynthesizer.SpeakSsmlAsync(ssml);;
                    m_AzureSpeechSynthesizer.Dispose();
                    if (File.Exists(fileName)) 
                    {
                        m_ProjectView.ImportPhrasesTTS(fileName);
                    }
                    if (Directory.Exists(directoryFullPath))
                    {
                        File.Delete(fileName);
                        Directory.Delete(directoryFullPath, true);
                    }
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kindly add Azure Subcription details using Add Azure Key");
                }
            }
        }

        private static string GenerateSSML(string text, string voice, int speed, int pitch)
        {
            return @$"<speak xmlns=""http://www.w3.org/2001/10/synthesis"" version=""1.0"" xml:lang=""en-US"">
                        <voice name=""{voice}"">
                          <prosody rate=""{speed}%"" pitch=""{pitch}%"">
                            {text.Replace("&", "&amp;").Replace("<", " &lt;").Replace(">", "&gt;")}
                          </prosody>
                        </voice>
                      </speak>";
        }

        private void m_AddAzureKeyBtn_Click(object sender, EventArgs e)
        {
            AddAzureKey dialog = new AddAzureKey();
            dialog.ShowDialog();
        }

        private void m_BuildInRbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (m_BuildInRbtn.Checked)
            {
                m_VoiceSelectionCb.Items.Clear();
                LoadInstalledVoices();
                if(m_VoiceSelectionCb.Items.Count> 0)
                {
                    m_VoiceSelectionCb.SelectedIndex = 0;
                }
            }
        }

        private void m_AzurRbtn_CheckedChanged(object sender, EventArgs e)
        {
            if (m_AzureRbtn.Checked)
            {
                m_VoiceSelectionCb.Items.Clear();
                LoadAzureVoicesAsync();
            }
        }

        private void m_FontBiggerBtn_Click(object sender, EventArgs e)
        {
            if (m_TextToSpeechTb.Font.Size < 60)
            {
                m_TextToSpeechTb.Font = new Font(m_TextToSpeechTb.Font.FontFamily, m_TextToSpeechTb.Font.Size + 2);
            }
        }

        private void m_FontSmallerBtn_Click(object sender, EventArgs e)
        {
            if (m_TextToSpeechTb.Font.Size > 10)
            {
                m_TextToSpeechTb.Font = new Font(m_TextToSpeechTb.Font.FontFamily, m_TextToSpeechTb.Font.Size - 2);
            }
        }

        private void m_AddAzureVoiceBtn_Click(object sender, EventArgs e)
        {
            AddAzureVoice addAzureVoice = new AddAzureVoice(m_AzureSpeechSynthesizer, m_AzureSpeechConfig, this);
            addAzureVoice.ShowDialog();
        }

        private void m_DeleteAzureVoiceBtn_Click(object sender, EventArgs e)
        {
            if (m_VoiceSelectionCb.Items.Count > 0 && m_Settings.AzureVoices.Contains(m_VoiceSelectionCb.SelectedItem.ToString()))
            {

                m_Settings.AzureVoices.Remove(m_VoiceSelectionCb.SelectedItem.ToString());
                m_VoiceSelectionCb.Items.Remove(m_VoiceSelectionCb.SelectedItem);
            }
        }

        private void m_CloseBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
