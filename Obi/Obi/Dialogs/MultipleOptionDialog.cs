using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class MultipleOptionDialog : Form
    {
        private bool m_IsSaveBothChecked = false;
        private bool m_IsSaveProjectChecked = false;
        private bool m_IsDiscardBothChecked = false;
       // private bool m_IsCancelBtnPressed = false;

        public MultipleOptionDialog()
        {
            InitializeComponent();
        }
        public MultipleOptionDialog(bool IsprojectUnsaved, bool IsBookmarkDifferent)
            : this()
        {
            m_rdb_DiscardBoth.Enabled = IsprojectUnsaved || IsBookmarkDifferent;
            m_rdb_SaveBookmarkAndProject.Enabled = IsBookmarkDifferent && IsprojectUnsaved;
            m_rdb_SaveProjectOnly.Enabled = IsprojectUnsaved && IsBookmarkDifferent;            
        }

        public bool IsSaveBothChecked
        {
            get { return m_IsSaveBothChecked; }
        }

        public bool IsSaveProjectChecked
        {
            get { return m_IsSaveProjectChecked; }
        }

        public bool IsDiscardBothChecked
        {
            get { return m_IsDiscardBothChecked; }
        }

      /*  public bool IsCancelBtnPressed
        {
            get { return m_IsCancelBtnPressed; }
        }*/

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
          //  m_IsCancelBtnPressed = true;
            this.Close();                
        }

        private void m_btn_Cancel_Click(object sender, EventArgs e)
        {
          //  m_IsCancelBtnPressed = true;
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void m_rdb_SaveBookmarkAndProject_CheckedChanged(object sender, EventArgs e)
        {
            m_IsDiscardBothChecked = false;
            m_IsSaveProjectChecked = false;
            m_IsSaveBothChecked = true;
        }

        private void m_rdb_DiscardBoth_CheckedChanged(object sender, EventArgs e)
        {
            m_IsSaveBothChecked = false;
            m_IsSaveProjectChecked = false;
            m_IsDiscardBothChecked = true;
        }

        private void m_rdb_SaveProjectOnly_CheckedChanged(object sender, EventArgs e)
        {
            m_IsSaveBothChecked = false;
            m_IsDiscardBothChecked = false;
            m_IsSaveProjectChecked = true;
        }
    }
}