using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.CognitiveServices.Speech;

namespace Obi.Dialogs
{
    public partial class AddAzureVoice : Form
    {
        private SpeechSynthesizer? m_AzureSpeechSynthesizer;
        private readonly SpeechConfig m_AzureSpeechConfig;
        private GenerateSpeech m_GenerateSpeech;

        public AddAzureVoice(SpeechSynthesizer speechSynthesizer, SpeechConfig speechConfig, GenerateSpeech generateSpeech)
        {
            InitializeComponent();
            m_AzureSpeechSynthesizer = speechSynthesizer;
            m_AzureSpeechConfig = speechConfig;
            m_GenerateSpeech = generateSpeech;  

            LoadAzureVoices();
        }

        private void m_AddVoiceBtn_Click(object sender, EventArgs e)
        {
            string voice = m_AddAzureVoiceListBox.SelectedItem.ToString();
            m_GenerateSpeech.AddAzureVoices(voice);
            this.Close();

        }

        private async void LoadAzureVoices()
        {
            if (m_AzureSpeechSynthesizer == null)
            {
                m_AzureSpeechSynthesizer = new(m_AzureSpeechConfig);
            }
            using var resultasync = await m_AzureSpeechSynthesizer.GetVoicesAsync();
            if (resultasync.Voices.Count != null && resultasync.Voices.Count > 0)
            {
                foreach (var tempVoice in resultasync.Voices)
                {
                    m_AddAzureVoiceListBox.Items.Add(tempVoice.ShortName);
                }
            }

        }
    }
}
