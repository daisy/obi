using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
    {
    public partial class WelcomeDialog : Form
        {
        public WelcomeDialog ()
            {
            InitializeComponent ();
            }

        public enum Option { newProject , OpenProject , OpenBlank } ;
        private Option m_Result;

        public Option Result
            {
            get { return m_Result; }
            }


        private void m_btnNewProject_Click ( object sender, EventArgs e )
            {
            m_Result = Option.newProject;
            Close ();
            }

        private void m_btnOpenProject_Click ( object sender, EventArgs e )
            {
            m_Result = Option.OpenProject;
            Close ();
            }

        private void m_btnOpenBlank_Click ( object sender, EventArgs e )
            {
            m_Result = Option.OpenBlank;
            Close ();
            }

        private void m_btnCancel_Click ( object sender, EventArgs e )
            {
            m_Result = Option.OpenBlank;
            Close ();
            }
        }
    }