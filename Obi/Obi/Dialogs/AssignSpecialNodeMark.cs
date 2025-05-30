using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssignSpecialNodeMark : Form
    {
        private string m_SelectedSpecialNode = "";
        public AssignSpecialNodeMark(Settings settings) //@fontconfig
        {
            InitializeComponent();
            m_cmbBoxSpecialNode.SelectedIndex = 0;
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Creating a skippable note.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
            if (settings.Project_ReadOnlyMode)
            {
                m_rdb_Copy.Enabled = m_rdb_Cut.Enabled = m_rdb_Delete.Enabled = !settings.Project_ReadOnlyMode;
            }
        }

        public string SelectedSpecialNode
        {
            get { return m_SelectedSpecialNode; }
        }

        public bool IsRenumberChecked
        { get { return m_rdb_btn_RenumberPages.Checked; } }

        public bool IsSpeechRateChecked
        { get { return m_rdb_btn_SpeechRate.Checked; } }

        public bool IsChangeVolumeChecked
        { get { return m_rdb_btn_ChangeVolume.Checked; } }

        public bool IsNormalizeChecked
        { get { return m_rdb_btn_Normalize.Checked; } }

        public bool IsNoiseReductionChecked
        { get { return m_rdb_btn_NoiseReduction.Checked; } }

        public bool IsNoiseReductionRnnChecked
        { get { return m_rdb_btn_NoiseReductionRnn.Checked; } }

        public bool IsCopyChecked
        {
            get { return m_rdb_Copy.Checked; }
        }

        public bool IsDeleteChecked
        {
            get { return m_rdb_Delete.Checked; }
        }

        public bool IsCutChecked
        {
            get { return m_rdb_Cut.Checked; }
        }

        public bool IsTimeElapsedChecked
        {
            get
            {
                return m_rtb_btn_TimeElapsed.Checked;
            }
        }

        public bool IsMergeChecked
        {
            get { return m_rdb_Merge.Checked; }
        }

     

        public bool EnableSkippableNotesAndMerge
        {
            set
            {
                m_rdb_btn_SpecialPhrase.Enabled = value;
                m_rdb_Merge.Enabled = value;
                m_rdb_Copy.Enabled = value;
                m_rdb_Cut.Enabled = value;
                m_rdb_Delete.Enabled = value;
                if (!value)
                {
                    m_rdb_btn_RenumberPages.Checked = true;
                }
            }
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            if (m_cmbBoxSpecialNode.SelectedItem != null)
            {
                m_SelectedSpecialNode = m_cmbBoxSpecialNode.SelectedItem.ToString();
            }
        }

        private void m_rdb_btn_SpecialPhrase_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = true;
        }

        private void m_rdb_btn_RenumberPages_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rtb_btn_AudioProcessing_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rtb_btn_TimeElapsed_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
            
        }

        private void m_rdb_Copy_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_Cut_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_Merge_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_Delete_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_btn_ChangeVolume_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_btn_Normalize_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_btn_SpeechRate_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }

        private void m_rdb_btn_NoiseReduction_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;

        }

        private void m_rdb_btn_NoiseReductionRnn_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = false;
        }
    }
}