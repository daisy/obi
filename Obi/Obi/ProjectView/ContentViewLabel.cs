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
        private SolidBrush brushRect = new SolidBrush(Color.Turquoise);
     //   private SolidBrush brushRect1 = new SolidBrush(Color.Black);
        private float m_ZoomFactor;
    //    private bool mFlagInvert = false;
        private Rectangle contentRect;
        private float height;

        public ContentViewLabel()
        {
            InitializeComponent();
            m_ZoomFactor = 1.0f;
            Name_SectionDisplayed = Localizer.Message ( "ContentViewLabel_NoSection" );
        }
        public ContentView contentView
        {
            get { return mCont; }
            set {
                mCont = value;
            if (mCont == null)
            {
                MessageBox.Show("null");
                Console.WriteLine("cony val" + mCont);
            }
             }
        } 
        public float zoomFactor
        {
            get { return m_ZoomFactor; }
            set
            {
                m_ZoomFactor = value;                
                ZoomLabel();
                Console.WriteLine("displayed" + zoomFactor);
            }            
        }      

        public string Name_SectionDisplayed
        {
            get { return m_lblSectionName.Text; }
            set { m_lblSectionName.Text = value; }
        }

     /*   private void ContentViewLabel_Paint(object sender, PaintEventArgs e)
        {
            if (mCont != null)
            {
                 contentRect = new Rectangle(10, 0, mCont.Size.Width, Convert.ToInt32(height));
                 Console.WriteLine("ht of rect 1 " + height);
                 g = this.CreateGraphics();
                        if (mCont != null)
                              
                   if (mFlagInvert)
                   {                   
                       g.FillRectangle(brushRect1, contentRect);
                       m_lblStaticLabel.Location = new Point(m_lblStaticLabel.Location.X, contentRect.Height / 2 - m_lblStaticLabel.Height / 2);
                       m_lblSectionName.Location = new Point(m_lblStaticLabel.Location.X + m_lblStaticLabel.Width, contentRect.Height / 2 - m_lblSectionName.Height / 2);                
                   }

                   else
                   {
                       g.FillRectangle(brushRect, contentRect);
                       m_lblStaticLabel.Location = new Point(m_lblStaticLabel.Location.X, contentRect.Height / 2 - m_lblStaticLabel.Height / 2);
                       m_lblSectionName.Location = new Point(m_lblStaticLabel.Location.X + m_lblStaticLabel.Width, contentRect.Height / 2 - m_lblSectionName.Height / 2);                
                   } 
            }                      
        }
    */

        public void ZoomLabel()
        {
            if (mCont != null)
            {
                int contentViewLabelHeight = Convert.ToInt32(22 * zoomFactor) ;

                m_lblSectionName.Font = new Font ( Font.FontFamily, zoomFactor * this.Font.Size );
                m_lblStaticLabel.Font = new Font ( Font.FontFamily, zoomFactor * this.Font.Size );
                m_lblStaticLabel.Location = new Point ( m_lblStaticLabel.Location.X, contentViewLabelHeight / 2 - m_lblStaticLabel.Height / 2 );
                m_lblSectionName.Location = new Point ( m_lblStaticLabel.Location.X + m_lblStaticLabel.Width, contentViewLabelHeight / 2 - m_lblSectionName.Height / 2 );
                Console.WriteLine ( "ht of font " + m_lblSectionName.Height );

            this.Size = new Size(this.Size.Width, contentViewLabelHeight);
            this.Location = new Point ( this.Location.X, mCont.Height - contentViewLabelHeight );
            
            }
        //    InvertColor(false);    
         }

    /*    public void InvertColor(bool highContrast)
        {
            mFlagInvert = highContrast;
            if (mFlagInvert)
            {
                m_lblSectionName.ForeColor = Color.White;
                m_lblStaticLabel.ForeColor = Color.White;
                //   this.BackColor = Color.Black;
                this.Invalidate();
            }
        } */        
    }
}