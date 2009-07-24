using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MergeUtilityUI
{
    public partial class DaisyMergerOptionForm : Form
    {        
        public enum Option { d3 , d202 };
        private Option mOpt;

        public DaisyMergerOptionForm()
        {
            InitializeComponent();
            mOpt = Option.d3;
        }

        public Option chooseOption { get { return mOpt; } }       
        
        private void m_btnOK_Click(object sender, EventArgs e)
        {
        this.DialogResult = DialogResult.OK;
            if (m_rdbDaisy3.Checked) 
            {
                mOpt = Option.d3;
                Close();
            }
            else if (m_rdbDaisy202.Checked)
            {
                mOpt = Option.d202;
                Close();
            }
            else
            {
                MessageBox.Show(" Please select one of the Export on which merging can take place");
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();            
        }


        private void ShowHelpFile ()
            {
            try
                {
                System.Diagnostics.Process.Start ( (new Uri ( Path.Combine ( Path.GetDirectoryName ( GetType ().Assembly.Location ),
                     "DTB Merger-Help.htm" ) )).ToString () );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                return;
                }
            }

        private void DaisyMergerOptionForm_HelpRequested ( object sender, HelpEventArgs hlpevent )
            {
            ShowHelpFile ();
            }

        
    }
}