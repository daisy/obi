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
        private ContentView m_ContentView = null;    
        private string m_SectionLabelString = "";
        private float m_ZoomFactor;
        private bool m_IsColorHighContrast = false;
        private bool m_IsSectionSelected = false;
        private bool m_SectionFocus;       
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
            get { return m_ContentView; }
            set { m_ContentView = value; }
        } 
        
        public bool invertColor
        {
            get { return m_IsColorHighContrast; }
            set 
                { 
                m_IsColorHighContrast = value;
                UpdateColors ();
                }
        }

        public bool sectionSelected
        {
            get { return m_IsSectionSelected; }
            set { m_IsSectionSelected = value;
            UpdateColors();
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
            set 
                {
                m_SectionLabelString = value;
                UpdateSectionLabel ();
                }
        }

        private void ContentViewLabel_Paint(object sender, PaintEventArgs e)
        {
            SolidBrush solidBrushControlLight = new SolidBrush(SystemColors.ControlLight);
            SolidBrush solidBrushBlack = new SolidBrush(Color.Black);
            Rectangle contentRect;
            if (m_ContentView == null) return;

            Graphics g = this.CreateGraphics();
            Pen rectPen = new Pen(Color.White, 2F);
            int m_contentRectHeight = Convert.ToInt32(19 * zoomFactor);
            contentRect = new Rectangle(10, ((m_contentViewLabelHeight / 2) - (m_contentRectHeight / 2)), m_ContentView.Size.Width, m_contentRectHeight);
            g.FillRectangle(solidBrushControlLight, contentRect);

             if (m_ContentView != null)
             {
                 if (m_IsColorHighContrast && m_IsSectionSelected)
                 {
                     g.DrawRectangle(rectPen, contentRect);
                     g.FillRectangle(solidBrushBlack, contentRect);
                 }
                 else if (m_IsColorHighContrast && !m_IsSectionSelected)
                     g.FillRectangle(solidBrushBlack, contentRect);
                 else if (!m_IsColorHighContrast && m_IsSectionSelected)
                     g.FillRectangle(solidBrushControlLight, contentRect);                
             }
        }
        public void ZoomLabel()
        {
            int fontSize = Convert.ToInt32( 9.75 * zoomFactor); 
            if (m_ContentView != null)
            {
                m_contentViewLabelHeight = Convert.ToInt32(25 * zoomFactor);
                m_lblSectionName.Font = new Font ( Font.FontFamily, fontSize );
                m_lblStaticLabel.Font = new Font ( Font.FontFamily, fontSize );
                m_lblStaticLabel.Location = new Point ( m_lblStaticLabel.Location.X, m_contentViewLabelHeight / 2 - m_lblStaticLabel.Height / 2 );
                m_lblSectionName.Location = new Point ( m_lblStaticLabel.Location.X + m_lblStaticLabel.Width, m_contentViewLabelHeight / 2 - m_lblSectionName.Height / 2 );
                this.Size = new Size(this.Size.Width, m_contentViewLabelHeight);
                this.Location = new Point ( this.Location.X, m_ContentView.Height - m_contentViewLabelHeight );
                Invalidate();
              }
            //    InvertColor(false);
         }


        private delegate void UpdateInvokation();

        private void UpdateSectionLabel ()
            {
            if (InvokeRequired)
                {
                Invoke ( new UpdateInvokation ( UpdateSectionLabel ) );
                }
            else
                {
                m_lblSectionName.Text = m_SectionLabelString;
                }
            }

        public void UpdateColors ()
            {
            if (InvokeRequired)
                {
                Invoke ( new UpdateInvokation ( UpdateSectionLabel ) );
                }
            else
                {
                if (m_IsColorHighContrast && m_IsSectionSelected)
                    {
                    m_lblSectionName.ForeColor = Color.White;
                    m_lblStaticLabel.ForeColor = Color.White;
                    m_lblSectionName.BackColor = Color.Black;
                    m_lblStaticLabel.BackColor = Color.Black;
                    this.BackColor = Color.Black;
                    }
                if (m_IsColorHighContrast && !m_IsSectionSelected)
                    {
                    m_lblSectionName.ForeColor = Color.White;
                    m_lblStaticLabel.ForeColor = Color.White;
                    m_lblSectionName.BackColor = Color.Black;
                    m_lblStaticLabel.BackColor = Color.Black;
                    this.BackColor = Color.Black;
                    }
                if (!m_IsColorHighContrast && m_IsSectionSelected)
                    {
                    m_lblSectionName.ForeColor = SystemColors.ControlText;
                    m_lblStaticLabel.ForeColor = SystemColors.ControlText;
                    m_lblSectionName.BackColor = SystemColors.ControlLight;
                    m_lblStaticLabel.BackColor = SystemColors.ControlLight;
                    this.BackColor = SystemColors.ControlLight;
                    }
                if (!m_IsColorHighContrast && !m_IsSectionSelected)
                    {
                    m_lblSectionName.ForeColor = SystemColors.ControlDark;
                    m_lblStaticLabel.ForeColor = SystemColors.ControlDark;
                    m_lblSectionName.BackColor = SystemColors.Control;
                    m_lblStaticLabel.BackColor = SystemColors.Control;
                    this.BackColor = SystemColors.Control;
                    }
                }
         /*       if (mFlagInvert)
                {
                    m_lblSectionName.ForeColor = Color.White;
                    m_lblStaticLabel.ForeColor = Color.White;
                    m_lblSectionName.BackColor = Color.Black;
                    m_lblStaticLabel.BackColor = Color.Black;
                    this.BackColor = Color.Black;
                }*/
                Invalidate();
        }
    }
}