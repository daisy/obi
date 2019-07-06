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
        private int m_OriginalPanelSize;
        private int mOldMulFactor = 0;
        private int mHeightOfButton = 44;
        private int mHeightOfToolstrip = 195;
                       
        public VerticalScrollToolStripContainer()
        {
            InitializeComponent();
            TrackBarValueInPercentage = 0;
            if(this.Height > 0)
            m_OriginalPanelSize = this.Height;
            else
                m_OriginalPanelSize = 500;
          //  this.Height = toolStripContainer1.Height;   
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

                if (!m_CanScrollDown  && CanScrollUp) TrackBarValueInPercentage = 100;
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

        public void toolStripResizing()
        {
            if (m_OriginalPanelSize <= 0) return;
            int interval = (this.Height * 100) / m_OriginalPanelSize;
            int mulFactor = (interval / 10) * 10;
            
            if (this.Height > (m_OriginalPanelSize / 2))
            { this.Height = toolStripContainer1.Height; }
            else
            { }                    

            if (mOldMulFactor == mulFactor)
            {  }
                
            else

            {
                if (this.Height > (m_OriginalPanelSize / 2))
                {
                    m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                    m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                    m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                    m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                    m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                    m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32((mHeightOfButton * mulFactor) / 100));
                   
                    toolStripTop.Size = new Size(toolStripBottom.Size.Width, Convert.ToInt32((mHeightOfToolstrip * (mulFactor - 12)) / 100));
                    toolStripTop.Location = new Point(toolStripTop.Location.X, toolStripTop.Location.Y);
                    toolStripBottom.Size = new Size(toolStripTop.Size.Width, Convert.ToInt32((mHeightOfToolstrip * (mulFactor - 12)) / 100));
                    trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height + 50);
                    toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height) + 30));
                    toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height) + 300);
                                   
                }
                else
                {
                    m_BtnGoToBegining.Size = new Size(m_BtnGoToBegining.Width, Convert.ToInt32((mHeightOfButton / 2)));
                    m_BtnGoToEnd.Size = new Size(m_BtnGoToEnd.Width, Convert.ToInt32((mHeightOfButton / 2)));
                    m_BtnLargeIncrementDown.Size = new Size(m_BtnLargeIncrementDown.Width, Convert.ToInt32((mHeightOfButton / 2)));
                    m_BtnLargeIncrementUp.Size = new Size(m_BtnLargeIncrementUp.Width, Convert.ToInt32((mHeightOfButton / 2)));
                    m_BtnSmallIncrementDown.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32((mHeightOfButton / 2)));
                    m_BtnSmallIncrementUp.Size = new Size(m_BtnSmallIncrementDown.Width, Convert.ToInt32((mHeightOfButton / 2)));

                    toolStripTop.Size = new Size(toolStripBottom.Size.Width, Convert.ToInt32((mHeightOfToolstrip / 2)));
                    toolStripTop.Location = new Point(toolStripTop.Location.X, toolStripTop.Location.Y);
                    toolStripBottom.Size = new Size(toolStripTop.Size.Width, Convert.ToInt32((mHeightOfToolstrip / 2)));
                    trackBar1.Location = new Point(trackBar1.Location.X, toolStripTop.Location.Y + toolStripTop.Height);
                    toolStripBottom.Location = new Point(toolStripBottom.Location.X, (Convert.ToInt32(trackBar1.Location.Y + trackBar1.Height)));
                    toolStripContainer1.Size = new Size(toolStripContainer1.Size.Width, (toolStripTop.Height + toolStripBottom.Height + trackBar1.Height));
                 }                      
            }            
            mOldMulFactor = mulFactor;
        }

        private void VerticalScrollToolStripContainer_Resize(object sender, EventArgs e)
        {
            toolStripResizing();        
        }       
    }
}