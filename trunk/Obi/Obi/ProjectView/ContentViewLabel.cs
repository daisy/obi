using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
    {
    public partial class ContentViewLabel : UserControl
    {
        private Graphics g;
        private ContentView mCont = null;
        private SolidBrush brushRect = new SolidBrush(Color.LightGray);
        private SolidBrush blackRect = new SolidBrush(SystemColors.ControlText);
        private SolidBrush brushRect1 = new SolidBrush(SystemColors.ControlLightLight);
        private SolidBrush brushRect2 = new SolidBrush(SystemColors.ControlLight);

        private float m_ZoomFactor;
        private bool mFlagInvert = false;
        private bool mFlagSectionSelected = false;
        private bool mSectionFocus;
        private Rectangle contentRect;
        private float height;
        private int m_contentViewLabelHeight;
               
        public ContentViewLabel()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            m_contentViewLabelHeight = Convert.ToInt32(23 * zoomFactor);
            Name_SectionDisplayed = Localizer.Message ( "ContentViewLabel_NoSection" );
        }
        public ContentView contentView
        {
            get { return mCont; }
            set { mCont = value; }
        } 
        
        public bool invertColor
        {
            get { return mFlagInvert; }
            set { mFlagInvert = value; }
        }

        public bool sectionSelected
        {
            get { return mFlagSectionSelected; }
            set { mFlagSectionSelected = value;
            InvertColor();
                }            
        }

        public float zoomFactor
        {
            get { return m_ZoomFactor; }
            set
            {
                m_ZoomFactor = value;
                ZoomLabel();
            }
        }

        public string Name_SectionDisplayed
        {
            get { return m_lblSectionName.Text; }
            set { m_lblSectionName.Text = value; }
        }

        private void ContentViewLabel_Paint(object sender, PaintEventArgs e)
        {
            g = this.CreateGraphics();
            Pen rectPen = new Pen(Color.White, 2F);
            int m_contentRectHeight = Convert.ToInt32(19 * zoomFactor);
            contentRect = new Rectangle(10, ((m_contentViewLabelHeight / 2) - (m_contentRectHeight / 2)), mCont.Size.Width, m_contentRectHeight);
            g.FillRectangle(brushRect, contentRect);

             if (mCont != null)
             {
                 if (mFlagInvert && mFlagSectionSelected)
                 {
                     g.DrawRectangle(rectPen, contentRect);
                     g.FillRectangle(blackRect, contentRect);
                 }
                 else if (mFlagInvert && !mFlagSectionSelected)
                     g.FillRectangle(blackRect, contentRect);
                 else if (!mFlagInvert && mFlagSectionSelected)
                     g.FillRectangle(brushRect, contentRect);
                 else if (!mFlagInvert && !mFlagSectionSelected)
                     g.FillRectangle(brushRect2, contentRect);
             }
        }
        public void ZoomLabel()
        {
            int fontSize = Convert.ToInt32( 9.75 * zoomFactor); 
            if (mCont != null)
            {
                m_contentViewLabelHeight = Convert.ToInt32(25 * zoomFactor);
                m_lblSectionName.Font = new Font ( Font.FontFamily, fontSize );
                m_lblStaticLabel.Font = new Font ( Font.FontFamily, fontSize );
                m_lblStaticLabel.Location = new Point ( m_lblStaticLabel.Location.X, m_contentViewLabelHeight / 2 - m_lblStaticLabel.Height / 2 );
                m_lblSectionName.Location = new Point ( m_lblStaticLabel.Location.X + m_lblStaticLabel.Width, m_contentViewLabelHeight / 2 - m_lblSectionName.Height / 2 );
                this.Size = new Size(this.Size.Width, m_contentViewLabelHeight);
                this.Location = new Point ( this.Location.X, mCont.Height - m_contentViewLabelHeight );
                Invalidate();
              }
            //    InvertColor(false);
         }

       public void InvertColor()
        {

            if (mFlagInvert && mFlagSectionSelected)
            {
                m_lblSectionName.ForeColor = SystemColors.Window;
                m_lblStaticLabel.ForeColor = SystemColors.Window;
                m_lblSectionName.BackColor = SystemColors.ControlText;
                m_lblStaticLabel.BackColor = SystemColors.ControlText;
                this.BackColor = SystemColors.ControlText;
            }
            if (mFlagInvert && !mFlagSectionSelected)
            {
                m_lblSectionName.ForeColor = SystemColors.Window;
                m_lblStaticLabel.ForeColor = SystemColors.Window;
                m_lblSectionName.BackColor = SystemColors.ControlText;
                m_lblStaticLabel.BackColor = SystemColors.ControlText;
                this.BackColor = SystemColors.ControlText;
            }
            if (!mFlagInvert && mFlagSectionSelected)
            {
                m_lblSectionName.ForeColor = SystemColors.ControlText;
                m_lblStaticLabel.ForeColor = SystemColors.ControlText;
                m_lblSectionName.BackColor = SystemColors.ControlLight;
                m_lblStaticLabel.BackColor = SystemColors.ControlLight;
                this.BackColor = SystemColors.ControlLight;
            }
            if (!mFlagInvert && !mFlagSectionSelected)
            {
                m_lblSectionName.ForeColor = SystemColors.Window;
                m_lblStaticLabel.ForeColor = SystemColors.Window;
                m_lblSectionName.BackColor = SystemColors.Control;
                m_lblStaticLabel.BackColor = SystemColors.Control;
                this.BackColor = SystemColors.Control;
            }           
        }         
    }
}