using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AudioProcessingNoiseReduction : Form
    {
        private bool m_IsSetNoiseLevelDoneFromNumericUpDownControl;
        private bool m_IsSetNoiseReductionDoneFromNumericUpDownControl;
        private ProjectView.ProjectView m_ProjectView;
        private bool m_IsSelectionNull = false;

        public AudioProcessingNoiseReduction(ProjectView.ProjectView projectView, bool isSelectionNull, bool isRnnNoiseReduction = false)
        {
            InitializeComponent();
            m_SelectPresetComboBox.SelectedIndex = 1;
            m_ProjectView = projectView;
            m_IsSelectionNull = isSelectionNull;
            if (isRnnNoiseReduction)
            {
                m_lblNoiseLevel.Visible = false;
                m_SetNoiseLevelTrackBar.Visible = false;
                m_SetNoiseLevelInPercent.Visible = false;
                m_lblPercent.Visible = false;
                m_lblNoiseReduction.Visible = false;
                m_SetNoiseReductionTrackBar.Visible = false;
                m_SetNoiseReduction.Visible = false;
                m_lbdB.Visible = false;
            }
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Audio/Audio processing.htm");
        }

    
        public decimal NoiseReductionInDb
        {
            get
            {
                return m_SetNoiseReduction.Value;
            }
        }

        public decimal NoiseFloorInDb
        {
            get
            {
                decimal SetNoiseFloorFromNoiseLevel = ((-20 - (-80)) * (m_SetNoiseLevelInPercent.Value / 100)) + (-80);
                return SetNoiseFloorFromNoiseLevel;
            }
        }

        public bool IsApplyOnWholeBook
        {
            get
            {
                return m_ApplyOnWholeBook.Checked;
            }
        }

        public bool ShowApplyWholeBookCheckbox
        {
            set
            {
                m_ApplyOnWholeBook.Visible = value;
            }
        }


        private void m_SetNoiseFloor_ValueChanged(object sender, EventArgs e)
        {
        }

        private void m_SetNoiseLevelInPercent_ValueChanged(object sender, EventArgs e)
        {

            m_IsSetNoiseLevelDoneFromNumericUpDownControl = true;
            m_SetNoiseLevelTrackBar.Value = (int)m_SetNoiseLevelInPercent.Value;
        }

        private void m_SetNoiseLevelTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if(!m_IsSetNoiseLevelDoneFromNumericUpDownControl)
            m_SetNoiseLevelInPercent.Value = m_SetNoiseLevelTrackBar.Value;
            m_IsSetNoiseLevelDoneFromNumericUpDownControl = false;
        }

        private void m_SetNoiseReductionTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (!m_IsSetNoiseReductionDoneFromNumericUpDownControl)
            {
                if (m_SetNoiseReductionTrackBar.Value != 0)
                    m_SetNoiseReduction.Value = m_SetNoiseReductionTrackBar.Value;
                else
                    m_SetNoiseReduction.Value = 0.01M;
            }
            m_IsSetNoiseReductionDoneFromNumericUpDownControl = false;
        }

        private void m_SetNoiseReduction_ValueChanged(object sender, EventArgs e)
        {
            m_IsSetNoiseReductionDoneFromNumericUpDownControl = true;
            m_SetNoiseReductionTrackBar.Value = (int)m_SetNoiseReduction.Value;
        }

        private void m_SelectPresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SelectPresetComboBox.SelectedIndex == 0)
            {
                m_SetNoiseLevelInPercent.Value = 100;
                m_SetNoiseReduction.Value = 50;
            }
            else if (m_SelectPresetComboBox.SelectedIndex == 1)
            {
                m_SetNoiseLevelInPercent.Value = 75;
                m_SetNoiseReduction.Value = 30;
            }
            else if (m_SelectPresetComboBox.SelectedIndex == 2)
            {
                m_SetNoiseLevelInPercent.Value = 50;
                m_SetNoiseReduction.Value = 10;
            }

        }

        private void m_btn_Ok_Click(object sender, EventArgs e)
        {
            if (m_IsSelectionNull && !m_ApplyOnWholeBook.Checked)
            {
                MessageBox.Show(Localizer.Message("AudioProcessingApplyOnWholeBook"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (m_ApplyOnWholeBook.Checked)
            {
                List<SectionNode> sectionsList = ((ObiRootNode)m_ProjectView.Presentation.RootNode).GetListOfAllSections();
                double totalDuration = 0;
                foreach (SectionNode section in sectionsList)
                {
                    totalDuration += section.Duration;
                }
                totalDuration = totalDuration / 2;
                int minutes = (int)Math.Floor(totalDuration / (60000));
                string duration;
                if (minutes <= 45)
                    duration = Localizer.Message("AudioProcessingEstimateFewMinutes");
                else if (minutes <= 120)
                {
                    duration = Localizer.Message("AudioProcessingEstimateSeveralMinutes");

                }
                else
                    duration = Localizer.Message("AudioProcessingEstimateFewHours");
                DialogResult result = MessageBox.Show(string.Format(Localizer.Message("ApplyingOperationOnWholeBook"), duration), Localizer.Message("Caption_Information"),  MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
