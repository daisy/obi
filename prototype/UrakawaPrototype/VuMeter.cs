using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace UrakawaPrototype
{
    public partial class VuMeter : UserControl
    {
        public VuMeter()
        {
            InitializeComponent();
        }

        private void load(object sender, EventArgs e)
        {
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            panel1.Refresh();
        }

        private void onPaint(object sender, PaintEventArgs e)
        {
            Rectangle rect = new Rectangle(
                this.panel1.Top,
                this.panel1.Left,
                this.panel1.Width,
                this.panel1.Height);

            Rectangle rect2 = rect;

            //paint the background green (full rect)
            e.Graphics.FillRectangle(Brushes.Green, rect2);

            int min = rect.Top;
            int max = rect.Height;

            Random random = new Random();
            int val = random.Next(min, max);

            int label = 0 - val;
            this.label1.Text = label.ToString();
            
            //this is sort of backwards
            //the black changes, not the green
            rect.Height = val;
            e.Graphics.FillRectangle(Brushes.Black, rect);
        }
    }
}
