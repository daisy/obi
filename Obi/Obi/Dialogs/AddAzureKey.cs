using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AddAzureKey : Form
    {
        private bool m_KeyAdded = false;
        public AddAzureKey()
        {
            InitializeComponent();
        }

        public bool KeyAdded
        {
            get
            { return m_KeyAdded; }  
        }
        private async void m_AddBtn_Click(object sender, EventArgs e)
        {
            if(m_KeyTB.Text.Length == 0 || m_ResgionTB.Text.Length == 0) 
            {
                MessageBox.Show(Localizer.Message("Azure_EnterSubcriptionKey"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            SpeechConfig AzureSpeechConfig = SpeechConfig.FromSubscription(m_KeyTB.Text, m_ResgionTB.Text);
            SpeechSynthesizer? AzureSpeechSynthesizer = new SpeechSynthesizer(AzureSpeechConfig);
            SynthesisVoicesResult voicesResult = await AzureSpeechSynthesizer.GetVoicesAsync(); 
            if (voicesResult.Voices.Count ==  0)
            {
                MessageBox.Show(Localizer.Message("Azure_EnterCorrectSubcriptionKey"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            StringBuilder csvContent = new StringBuilder();
            csvContent.AppendLine(m_KeyTB.Text);
            csvContent.AppendLine(m_ResgionTB.Text);
            //string directory = AppDomain.CurrentDomain.BaseDirectory;

            string permanentSettingsDirectory = System.IO.Directory.GetParent(Settings_Permanent.GetSettingFilePath()).ToString();
           
            string csvPath = permanentSettingsDirectory + "\\Azurekey.csv ";
            if(File.Exists(csvPath))
            {
                File.Delete(csvPath);
            }
            File.AppendAllText(csvPath, csvContent.ToString());
            m_KeyAdded = true;
            MessageBox.Show(Localizer.Message("Azure_SubcriptionAdded"), Localizer.Message("Caption_Information"), MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }
    }
}
