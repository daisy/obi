using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NCXEdit
{
    public partial class MainForm : Form
    {
        private NCX.NCX mNCX = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TitleAuthorDialog dialog = new TitleAuthorDialog();
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {

            }
        }
        
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}