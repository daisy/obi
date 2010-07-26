using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class VerticleScrollPane : UserControl
    {

        private ContentView m_ContentView = null;
        

        public VerticleScrollPane()
        {
            InitializeComponent();
        }

        public ContentView contentView
        {
            get { return m_ContentView; }
            set { m_ContentView = value; }
        }

        public int TrackBarValueInPercentage
            {
            get { return trackBar1.Value; }
            set 
                {
                if (value >= 0 && value <= 100)
                    {
                    trackBar1.Value = value;
                    }
                    }
            }

        private void m_BtnGoToBegining_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
              scrollButtons(false);
              m_ContentView.ScrollStripsPanel_Top();
              scrollButtons(true);
            }
        }
       
        private void m_BtnLargeIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                scrollButtons(false);
                m_ContentView.ScrollUp_LargeIncrement();
                scrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                scrollButtons(false);
                m_ContentView.ScrollDown_LargeIncrement();
                scrollButtons(true);
            }
        }       

        private void m_BtnGoToEnd_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                scrollButtons(false);
                m_ContentView.ScrollStripsPanel_Bottom();
                scrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                scrollButtons(false);
                m_ContentView.ScrollUp_SmallIncrement();
                scrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                scrollButtons(false);
                m_ContentView.ScrollDown_SmallIncrement();
                scrollButtons(true);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            trackBar1.Capture = false;            
        }

        public void scrollButtons(bool value)
        {
            m_BtnGoToBegining.Enabled = value;
            m_BtnGoToEnd.Enabled = value;
            m_BtnLargeIncrementDown.Enabled = value;
            m_BtnLargeIncrementUp.Enabled = value ;
            m_BtnSmallIncrementDown.Enabled = value;
            m_BtnSmallIncrementUp.Enabled = value;
        }      

    }
}
