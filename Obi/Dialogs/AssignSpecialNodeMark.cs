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
        private bool m_IsRenumberChecked = false;
        private bool m_IsAudioProcessingChecked = false;
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
        }

        public string SelectedSpecialNode
        {
            get { return m_SelectedSpecialNode; }
        }

        public bool IsRenumberChecked
        { get { return m_IsRenumberChecked; } }

        public bool IsAudioProcessingChecked
        { get { return m_IsAudioProcessingChecked; } }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            m_SelectedSpecialNode = m_cmbBoxSpecialNode.SelectedItem.ToString();            
        }

        private void m_rdb_btn_SpecialPhrase_CheckedChanged(object sender, EventArgs e)
        {
            m_cmbBoxSpecialNode.Visible = true;
            m_IsRenumberChecked = false;
            m_IsAudioProcessingChecked = false;
        }

        private void m_rdb_btn_RenumberPages_CheckedChanged(object sender, EventArgs e)
        {
            m_IsRenumberChecked = true;
            m_cmbBoxSpecialNode.Visible = false;
            m_IsAudioProcessingChecked = false;
        }

        private void m_rtb_btn_AudioProcessing_CheckedChanged(object sender, EventArgs e)
        {
            m_IsAudioProcessingChecked = true;
            m_cmbBoxSpecialNode.Visible = false;
            m_IsRenumberChecked = false;
        }

    }
}