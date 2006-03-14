using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UrakawaPrototype
{
    public partial class SettingsDialog : Form
    {
        public SettingsDialog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
          
            dlg.Color = button1.BackColor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                button1.BackColor = dlg.Color;
                button1.Text = dlg.Color.ToString();
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = button2.BackColor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                button2.BackColor = dlg.Color;
                button2.Text = dlg.Color.ToString();
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();

            dlg.Color = button3.BackColor;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                button3.BackColor = dlg.Color;
                button3.Text = dlg.Color.ToString();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}