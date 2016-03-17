using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioSettings : Form
    {
        private AudioSettings()
        {
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Creating a DTB\\Creating and Working with Projects\\Audio settings for the project.htm");          
        }

        public AudioSettings(int audioChannels, int audioSampleRate, Settings settings)
            : this()
        {
            for (int i = 0; i < mcbSampleRate.Items.Count; i++ )
            {
                
                if ((Convert.ToInt32 (mcbSampleRate.Items[i])) == audioSampleRate) mcbSampleRate.SelectedIndex = i;
            }
            mcbAudioChannel.SelectedIndex = AudioChannels== 1? 0:
                AudioChannels == 2? 1: 0;
            if (settings.ObiFont != this.Font.Name) //@fontconfig
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }

        }

        
        public int AudioChannels 
        { 
            get 
            {
                return mcbAudioChannel.SelectedIndex + 1;
                }
        }

        public int AudioSampleRate
        {
            get
            {
                return mcbSampleRate.SelectedIndex == 0 ? 11025 :
                    mcbSampleRate.SelectedIndex == 1 ? 22050 :
                    44100;
            }
        }

        private void mbtnOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mbtnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
