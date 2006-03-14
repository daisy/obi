using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace UrakawaPrototype
{
    public partial class LabelEditor : Form
    {
        private string textboxText;
        private bool mbImageChanged;

        public LabelEditor()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void setText(string text)
        {
            this.textboxText = text;
            this.textBox1.Text = this.textboxText;
        }

        public string getText()
        {
            return this.textboxText;
        }

        public bool didImageChange()
        {
            return mbImageChanged;
        }

        public Image getNewImage()
        {
            return (Image)this.pictureBox1.Image.Clone();
        }

        private void loadForm(object sender, EventArgs e)
        {
            mbImageChanged = false;
        }

        private void imageClick(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter.EndsWith("jpg");
          
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                this.pictureBox1.ImageLocation = dlg.FileName;
                //TODO
                //autosize image
                mbImageChanged = true;
            }
        }

      

        
        private void formClosing(object sender, FormClosingEventArgs e)
        {
            textboxText = this.textBox1.Text;
        }
    }
}