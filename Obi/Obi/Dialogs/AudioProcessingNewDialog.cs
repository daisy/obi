using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioProcessingNewDialog : Form
    {
        private bool m_IsAudioProcessingParameterInSeconds = false;
        private AudioLib.WavAudioProcessing.AudioProcessingKind m_AudioProcessingType;
        public AudioProcessingNewDialog(Settings settings)
        {
            InitializeComponent();
            m_txt_info.Text = Localizer.Message("AudioProcessing_InfoText");
            m_InfoToolTip.SetToolTip(m_txt_info, m_txt_info.Text);

            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Audio/Audio processing.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig            
            }

        }
        public AudioProcessingNewDialog(AudioLib.WavAudioProcessing.AudioProcessingKind typeOfAudioProcessing, Settings settings, double durationOfFadeInOut)
        {
            InitializeComponent();

            m_AudioProcessingType = typeOfAudioProcessing;
            m_txt_info.Text = Localizer.Message("AudioProcessing_InfoText");
            if (AudioLib.WavAudioProcessing.AudioProcessingKind.FadeIn == typeOfAudioProcessing || AudioLib.WavAudioProcessing.AudioProcessingKind.FadeOut == typeOfAudioProcessing)
            {
                if (AudioLib.WavAudioProcessing.AudioProcessingKind.FadeIn == typeOfAudioProcessing)
                {
                    m_lbl_Process.Text = Localizer.Message("FadeInProcess");
                    m_lbl_Parameters.Text = Localizer.Message("FadeInDuration");
                    this.Text = Localizer.Message("FadeIn");
                    this.AccessibleName = Localizer.Message("FadeIn");
                    m_numericUpDown1.AccessibleName = Localizer.Message("FadeInDurationAccessibleName");
                    //m_lbl_Parameters.Location = new Point(0, m_lbl_Parameters.Location.Y);
                }
                else
                {
                    m_lbl_Process.Text = Localizer.Message("FadeOutProcess");
                    m_lbl_Parameters.Text = Localizer.Message("FadeOutDuration");
                    this.Text = Localizer.Message("FadeOut");
                    this.AccessibleName = Localizer.Message("FadeOut");
                    m_numericUpDown1.AccessibleName = Localizer.Message("FadeOutDurationAccessibleName");
                    //m_lbl_Parameters.Location = new Point(0, m_lbl_Parameters.Location.Y);
                }

                if (m_lbl_StartTime.Width > m_lbl_Parameters.Width)
                {
                    m_StartTimeNumericUpDown.Location = new Point(m_lbl_StartTime.Location.X + m_lbl_StartTime.Width, m_StartTimeNumericUpDown.Location.Y);
                    m_lbl_StartTimeSeconds.Location = new Point(m_StartTimeNumericUpDown.Location.X + m_StartTimeNumericUpDown.Width, m_StartTimeNumericUpDown.Location.Y);

                    m_numericUpDown1.Location = new Point(m_StartTimeNumericUpDown.Location.X, m_numericUpDown1.Location.Y);
                    m_lbl_Seconds.Location = new Point(m_numericUpDown1.Location.X + m_numericUpDown1.Width, m_numericUpDown1.Location.Y);
                }
                else
                {
                    m_numericUpDown1.Location = new Point(m_lbl_Parameters.Location.X + m_lbl_Parameters.Width, m_numericUpDown1.Location.Y);
                    m_lbl_Seconds.Location = new Point(m_numericUpDown1.Location.X + m_numericUpDown1.Width, m_numericUpDown1.Location.Y);

                    m_StartTimeNumericUpDown.Location = new Point(m_numericUpDown1.Location.X, m_StartTimeNumericUpDown.Location.Y);
                    m_lbl_StartTimeSeconds.Location = new Point(m_StartTimeNumericUpDown.Location.X + m_StartTimeNumericUpDown.Width, m_StartTimeNumericUpDown.Location.Y);
                }
                double durationiInSeconds = durationOfFadeInOut * 0.001;
                m_IsAudioProcessingParameterInSeconds = true;
                m_numericUpDown1.Maximum = m_StartTimeNumericUpDown.Maximum = (decimal)durationiInSeconds;
                m_numericUpDown1.Value = (decimal)durationiInSeconds;
                m_txt_info.Text = string.Format(Localizer.Message("TextInfoForFadeInOutOperation"),Math.Round(m_numericUpDown1.Value,2));
                m_AmplifyParameter.Visible = false;
                m_lbl_Low.Visible = false;
                m_lbl_High.Visible = false;
                m_lbl_Seconds.Visible = true;
                m_NAudioForAudioProcessing.Visible = false;
                m_lbl_StartTime.Visible = true;
                m_StartTimeNumericUpDown.Visible = true;
                m_lbl_StartTimeSeconds.Visible = true;
            }
            else if (AudioLib.WavAudioProcessing.AudioProcessingKind.Normalize == typeOfAudioProcessing)
            {
                m_lbl_Process.Text = Localizer.Message("NormalizeProcess");
                this.Text = Localizer.Message("Normalize");
                m_txt_info.Text = Localizer.Message("TextInfoForNormalization");
                this.AccessibleName = Localizer.Message("Normalize");
                m_numericUpDown1.AccessibleName = Localizer.Message("NormalizeAceesibleName");
                m_AmplifyParameter.AccessibleName = Localizer.Message("NormalizeAceesibleNameForSlider");
                m_NAudioForAudioProcessing.Visible = true;
            }
            else if (AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch == typeOfAudioProcessing)
            {
                m_lbl_Process.Text = Localizer.Message("SpeechRateOfSeclection");
                this.Text = Localizer.Message("SpeechRate");
                m_txt_info.Text = Localizer.Message("TextInfoForSpeechRate");
                this.AccessibleName = Localizer.Message("SpeechRate");
                m_numericUpDown1.AccessibleName = Localizer.Message("SpeechRateAceesibleName");
                m_AmplifyParameter.AccessibleName = Localizer.Message("SpeechRateAceesibleNameForSlider");
                m_NAudioForAudioProcessing.Visible = false;
                m_numericUpDown1.Minimum = 0.6M;
            }
            m_InfoToolTip.SetToolTip(m_txt_info, m_txt_info.Text);
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Audio/Audio processing.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig            
            }
        }

        public bool IsUseNAudioForAudioProcessing
        {
            get
            {
                return m_NAudioForAudioProcessing.Checked;
            }
        }

       
     


        public float AudioProcessingParameter
        {
            get
            {
                //if (m_IsAudioProcessingParameterInSeconds)
                //{
                //    return (float)m_numericUpDown1.Value * 1000;
                //}
                return (float) m_numericUpDown1.Value;
            }
        }

        public float FadeEffectStartTime
        {
            get
            {

                return (float)m_StartTimeNumericUpDown.Value;

            }
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
            Close();
        }

      

        private void m_numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl == m_numericUpDown1 && m_AudioProcessingType != AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch)
            {
                if (!m_IsAudioProcessingParameterInSeconds)
                {
                    if (m_numericUpDown1.Value > 0 && m_numericUpDown1.Value <= (decimal)0.25)
                        m_AmplifyParameter.Value = -3;
                    else if (m_numericUpDown1.Value > (decimal)0.25 && m_numericUpDown1.Value <= (decimal)0.50)
                        m_AmplifyParameter.Value = -2;
                    else if (m_numericUpDown1.Value > (decimal)0.50 && m_numericUpDown1.Value <= (decimal)0.75)
                        m_AmplifyParameter.Value = -1;
                    else if (m_numericUpDown1.Value > (decimal)0.75 && m_numericUpDown1.Value <= (decimal)1)
                        m_AmplifyParameter.Value = 0;
                    else if (m_numericUpDown1.Value > 1 && m_numericUpDown1.Value <= 2)
                        m_AmplifyParameter.Value = 1;
                    else if (m_numericUpDown1.Value > 2 && m_numericUpDown1.Value <= 3)
                        m_AmplifyParameter.Value = 2;
                    else if (m_numericUpDown1.Value > 3 && m_numericUpDown1.Value <= 4)
                        m_AmplifyParameter.Value = 3;
                }
            }
            else if (this.ActiveControl == m_numericUpDown1 && m_AudioProcessingType == AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch)
            {
                if (!m_IsAudioProcessingParameterInSeconds)
                {
            
                    if (m_numericUpDown1.Value > (decimal)0.60 && m_numericUpDown1.Value <= (decimal)0.70)
                        m_AmplifyParameter.Value = -3;
                    else if (m_numericUpDown1.Value > (decimal)0.70 && m_numericUpDown1.Value <= (decimal)0.80)
                        m_AmplifyParameter.Value = -2;
                    else if (m_numericUpDown1.Value > (decimal)0.80 && m_numericUpDown1.Value <= (decimal)0.90)
                        m_AmplifyParameter.Value = -1;
                    else if (m_numericUpDown1.Value > (decimal)0.90 && m_numericUpDown1.Value <= (decimal)1)
                        m_AmplifyParameter.Value = 0;
                    else if (m_numericUpDown1.Value > 1 && m_numericUpDown1.Value <= 2)
                        m_AmplifyParameter.Value = 1;
                    else if (m_numericUpDown1.Value > 2 && m_numericUpDown1.Value <= 3)
                        m_AmplifyParameter.Value = 2;
                    else if (m_numericUpDown1.Value > 3 && m_numericUpDown1.Value <= 4)
                        m_AmplifyParameter.Value = 3;
                }
            }

        }

        private void m_AmplifyParameter_ValueChanged(object sender, EventArgs e)
        {
            if (this.ActiveControl == m_AmplifyParameter && m_AudioProcessingType != AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch)
            {
                float value = 0;
                if (m_AmplifyParameter.Value == 0)
                {
                    value = 1;
                }
                else if (m_AmplifyParameter.Value == 1)
                {
                    value = 2;
                }
                else if (m_AmplifyParameter.Value == 2)
                {
                    value = 3;
                }
                else if (m_AmplifyParameter.Value == 3)
                {
                    value = 4;
                }
                else if (m_AmplifyParameter.Value == -1)
                {
                    value = (float)0.75;
                }
                else if (m_AmplifyParameter.Value == -2)
                {
                    value = (float)0.50;
                }
                else if (m_AmplifyParameter.Value == -3)
                {
                    value = (float)0.25;
                }
                m_numericUpDown1.Value = (decimal)value;
            }
            else if (this.ActiveControl == m_AmplifyParameter && m_AudioProcessingType == AudioLib.WavAudioProcessing.AudioProcessingKind.SoundTouch)
            {
                float value = 0;
                if (m_AmplifyParameter.Value == 0)
                {
                    value = 1;
                }
                else if (m_AmplifyParameter.Value == 1)
                {
                    value = 2;
                }
                else if (m_AmplifyParameter.Value == 2)
                {
                    value = 3;
                }
                else if (m_AmplifyParameter.Value == 3)
                {
                    value = 4;
                }
                else if (m_AmplifyParameter.Value == -1)
                {
                    value = (float)0.90;
                }
                else if (m_AmplifyParameter.Value == -2)
                {
                    value = (float)0.80;
                }
                else if (m_AmplifyParameter.Value == -3)
                {
                    value = (float)0.70;
                }
                m_numericUpDown1.Value = (decimal)value;
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (this.ActiveControl == m_AmplifyParameter)
            {
                switch (keyData)
                {
                    case Keys.Up:
                        m_AmplifyParameter.Value = Math.Min(m_AmplifyParameter.Value + m_AmplifyParameter.SmallChange, m_AmplifyParameter.Maximum);
                        return true;

                    case Keys.Down:
                        m_AmplifyParameter.Value = Math.Max(m_AmplifyParameter.Value - m_AmplifyParameter.SmallChange, m_AmplifyParameter.Minimum);
                        return true;

                    case Keys.PageUp:
                        m_AmplifyParameter.Value = Math.Min(m_AmplifyParameter.Value + m_AmplifyParameter.LargeChange, m_AmplifyParameter.Maximum);
                        return true;

                    case Keys.PageDown:
                        m_AmplifyParameter.Value = Math.Max(m_AmplifyParameter.Value - m_AmplifyParameter.LargeChange, m_AmplifyParameter.Minimum);
                        return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}