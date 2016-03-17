using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ExtendedMessageToAssociate : Form
    {
        private ObiRootNode m_ObiNode;
        private EmptyNode m_SelectedNode = null;
        private EmptyNode m_EndNode = null;
        private bool m_AssignRole = false;
        private bool m_IsYesToAll = false;
        private bool m_Abort = false;

        public EmptyNode EndNode
        { get { return m_EndNode; } }

        public ExtendedMessageToAssociate(Settings settings)
        {
           // m_ObiNode = obiNode;
           // m_SelectedNode = selectedNode;
            InitializeComponent();
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files\\Introducing Obi\\Introducing Obi.htm");
            if (settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig  
            }      
        }

        public bool Is_AssignRole
        { get { return m_AssignRole; } }

        public bool Is_YesToAll
        { get { return m_IsYesToAll; } }

        public bool Is_Abort
        { get { return m_Abort; } }

        private void m_btn_YesToAll_Click(object sender, EventArgs e)
        {
            m_IsYesToAll = true;
            Close();
        }

        private void m_btn_Yes_Click(object sender, EventArgs e)
        {
            m_AssignRole = true;
        }

        private void m_btn_No_Click(object sender, EventArgs e)
        {
            m_Abort = true;
            Close();
        }

        private void m_btn_NoToAll_Click(object sender, EventArgs e)
        {

        }
    }
}