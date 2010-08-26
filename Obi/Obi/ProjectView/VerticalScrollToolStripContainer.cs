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
                m_ContentView.ScrollUp_LargeIncrement();
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementUp_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollUp_SmallIncrement();
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnSmallIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_SmallIncrement();
                UpdateScrollButtons(true);
            }
        }

        private void m_BtnLargeIncrementDown_Click(object sender, EventArgs e)
        {
            if (m_ContentView != null)
            {
                UpdateScrollButtons(false);
                m_ContentView.ScrollDown_LargeIncrement();
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
    }
}

