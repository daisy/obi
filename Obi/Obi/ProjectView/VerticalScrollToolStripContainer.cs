using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class VerticalScrollToolStripContainer : UserControl
    {
        private ContentView m_ContentView = null;
        private bool m_CanScrollUp = true;
        private bool m_CanScrollDown = true;
               
        public VerticalScrollToolStripContainer()
        {
            InitializeComponent();
            TrackBarValueInPercentage = 0;
            m_BtnGoToBegining.Height = 44;
            m_BtnGoToEnd.Height = 44;
            m_BtnLargeIncrementDown.Height = 44;
            m_BtnLargeIncrementUp.Height = 44;
            toolStripContainer1.Height = 533;
        }
        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
        }

        public int TrackBarValueInPercentage
        {
            get { return 100 - trackBar1.Value; }
            set
            {
                if (value >= 0 && value <= 100)
                {
                    trackBar1.Value = 100 - value;
                }
            }
        }

        public bool CanScrollUp
        {
            get { return m_CanScrollUp; }
            set
            {
                m_CanScrollUp = value;
                m_BtnGoToBegining.Enabled =
                    m_BtnLargeIncrementUp.Enabled =
                    m_BtnSmallIncrementUp.Enabled = m_CanScrollUp;

                if (!m_CanScrollUp) TrackBarValueInPercentage = 0;
            }
        }

        public bool CanScrollDown
        {
            get { return m_CanScrollDown; }
            set
            {
                m_CanScrollDown = value;
                m_BtnGoToEnd.Enabled =
                m_BtnLargeIncrementDown.Enabled =
                 m_BtnSmallIncrementDown.Enabled = m_CanScrollDown;

                if (!m_CanScrollDown) TrackBarValueInPercentage = 100;
            }
        }

       

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Capture = false;
        }

        public void UpdateScrollButtons(bool value)
        {
            m_BtnGoToBegining.Enabled = value && m_CanScrollUp;
            m_BtnLargeIncrementUp.Enabled = value && m_CanScrollUp;
            m_BtnSmallIncrementUp.Enabled = value && m_CanScrollUp;

            m_BtnGoToEnd.Enabled = value && m_CanScrollDown;
            m_BtnLargeIncrementDown.Enabled = value && m_CanScrollDown;
            m_BtnSmallIncrementDown.Enabled = value && m_CanScrollDown;

            //Console.WriteLine ( "down button status : " + m_BtnGoToEnd.Enabled + " Scroll up button status : " + m_BtnGoToBegining.Enabled );
        }

        private void m_BtnGoToBegining_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollStripsPanel_Top();
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollUp_LargeIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollUp_SmallIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_SmallIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_LargeIncrement(false);
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnGoToEnd_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollStripsPanel_Bottom();
                UpdateScrollButtons(true);
            }
        }

        public void verticalScrolling()
        {
             Console.WriteLine(" tool strip cont " + toolStripContainer1.Height + "panel height " + this.Height);
             if ((toolStripContainer1.Height - this.Height) > 20)
             {
                 //   toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, Convert.ToInt32(0.7 * toolStripContainer1.Size.Height));
                 m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, Convert.ToInt32(0.7 * m_BtnGoToBegining.Height));
                 m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, Convert.ToInt32(0.7 * m_BtnGoToEnd.Height));
                 m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, Convert.ToInt32(0.7 * m_BtnLargeIncrementDown.Height));
                 m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, Convert.ToInt32(0.7 * m_BtnLargeIncrementUp.Height));
                 m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32(0.7 * m_BtnSmallIncrementDown.Height));
                 m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32(0.7 * m_BtnSmallIncrementUp.Height));
                 Console.WriteLine("total but " + (m_BtnGoToBegining.Height + m_BtnLargeIncrementUp.Height + m_BtnSmallIncrementUp.Height));
                 toolStripTop.Size = new Size(toolStripBottom.Size.Width, Convert.ToInt32(0.7 * toolStripTop.Size.Height));
                 toolStripBottom.Size = new Size(toolStripTop.Size.Width, Convert.ToInt32(0.7 * toolStripBottom.Size.Height));
                 trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                 toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height)));
                 //   Console.WriteLine("totall sze " + (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height));
                 //toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, this.Size.Height);
                 toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, Convert.ToInt32((toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) * 1.2));
                 Console.WriteLine("location of bottom tool strip " + toolStripBottom.Location);
                 Console.WriteLine("location of tool strip top " + toolStripTop.Location);
                 //    Console.WriteLine("cint ht s " + toolStripContainer1.Height);
             }
             else if ((this.Height - toolStripContainer1.Height) > 20)
             {
                 toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, Convert.ToInt32((toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) * 1.2));
                 toolStripTop.Size = new Size(toolStripBottom.Size.Width, Convert.ToInt32(1.3 * toolStripTop.Size.Height));
                 toolStripBottom.Size = new Size(toolStripTop.Size.Width, Convert.ToInt32(1.3 * toolStripBottom.Size.Height));
                 trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                 toolStripBottom.Location = new Point(toolStripBottom.Location.X, (trackBar1.Location.Y + trackBar1.Height));
                 //  Console.WriteLine("totall sze " + (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height));
                 //   toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, Convert.ToInt32(1.2 * toolStripContainer1.Size.Height));
                 m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Size.Width, Convert.ToInt32(1.3 * m_BtnGoToBegining.Height));
                 m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, Convert.ToInt32(1.3 * m_BtnGoToEnd.Height));
                 m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, Convert.ToInt32(1.3 * m_BtnLargeIncrementDown.Height));
                 m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, Convert.ToInt32(1.3 * m_BtnLargeIncrementUp.Height));
                 m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32(1.3 * m_BtnSmallIncrementDown.Height));
                 m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32(1.3 * m_BtnSmallIncrementUp.Height));
                 Console.WriteLine("location of bottom tool strip " + toolStripBottom.Location);
                 Console.WriteLine("location of tool strip top " + toolStripTop.Location);
                 //  toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, this.Size.Height);
                 //  toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, Convert.ToInt32((toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) + 50));
                 //   Console.WriteLine("cint ht s " + toolStripContainer1.Height);
             }               
            
        }

        private void VerticalScrollToolStripContainer_Resize(object sender, EventArgs e)
        {
            verticalScrolling();
        
        }       
    }
}

