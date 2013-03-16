using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.media.timing;
using AudioLib;

namespace Obi.ProjectView
{
    public partial class ZoomWaveform : UserControl
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private  EmptyNode m_Node;
        private AudioBlock m_AudioBlock;
        private int initialWaveformWidth = 0;
        private float m_ZoomFactor = 0;
        private ProjectView m_ProjectView;
       
        private ZoomWaveform()
        {
            
            InitializeComponent();
            //this.Controls.Add(hScrollBar);
            this.Controls.Add(panelZooomWaveform);
       
            this.Controls.Add(txtZoomSelected);
            
        }
     
        public void UpdateCursorTime (double time ) 
        {
            if( m_AudioBlock != null ) m_AudioBlock.UpdateCursorTime (time) ;
            
        }
        private void ProjectViewSelectionChanged ( object sender, EventArgs e )
        {
            if (m_ProjectView.Selection != null && m_ProjectView.Selection.Phrase != null && m_ProjectView.GetSelectedPhraseSection != null)
            {
                txtZoomSelected.Text=" ";
                txtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                txtZoomSelected.Text +=" "+ m_ProjectView.GetSelectedPhraseSection.ToString();
                string temp = m_ProjectView.Selection.Node.ToString();
                if (m_AudioBlock.Node.ToString() != temp && m_ProjectView.Selection.EmptyNodeForSelection != null)
                {
                   PhraseLoad(m_ProjectView.Selection.EmptyNodeForSelection);
                }
              
               // txtZoomSelected.Text += m_ProjectView.Selection.Phrase.ToString();
            }
        }
        public void PhraseLoad(EmptyNode phrase)
        {
            //m_AudioBlock.Node.Parent;
            m_Node = phrase;
            if (m_Node is PhraseNode)
            {
                if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                {
                    panelZooomWaveform.Controls.Remove(m_AudioBlock);
                }
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                txtZoomSelected.Location = new Point(0, this.Height - 50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
                //   m_AudioBlock.Width = m_ContentView.Width;
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                //  m_AudioBlock.SetWaveformForZoom(m_Node as PhraseNode,zoomFactor);
                //int a=  m_AudioBlock.ComputeWaveformDefaultWidth();
                //m_AudioBlock.Waveform.Render();
                m_AudioBlock.InitCursor(0);
            }
        }
        public ZoomWaveform(ContentView contentView, Strip strip,EmptyNode node,ProjectView mProjectView ):this    ()
        {
           
            m_ContentView = contentView;
            m_ProjectView = mProjectView;
            m_ProjectView.SelectionChanged += new EventHandler(ProjectViewSelectionChanged);
            m_Strip = strip;
            m_Node = node;
            if (m_ProjectView.Selection.Phrase!=null)
            if (m_ContentView != null)
            {
                this.Width = m_ContentView.Width-22;
                this.Height = m_ContentView.Height-22;
                 btnClose.Location = new Point(btnClose.Location.X, this.Height - 25);
                btnNextPhrase.Location=new Point(btnNextPhrase.Location.X,this.Height-25);
                btnPreviousPhrase.Location=new Point(btnPreviousPhrase.Location.X,this.Height-25);
                btnReset.Location=new Point(btnReset.Location.X,this.Height-25);
                btnZoomIn.Location=new Point(btnZoomIn.Location.X,this.Height-25);
                btnZoomOut.Location=new Point(btnZoomOut.Location.X,this.Height-25);
                panelZooomWaveform.Width = this.Width - 30;
                panelZooomWaveform.Height = this.Height - 60;
                txtZoomSelected.Width = this.Width - 40;
            }
           //this.Width=m_ContentView.Width;
            if (m_Node is PhraseNode)
            {
               
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                txtZoomSelected.Location = new Point(0,this.Height-50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.InitCursor(0);
                m_AudioBlock.Focus();
            }
            
        }



        
      

        private void btnClose_Click(object sender, EventArgs e)
        {
         m_ContentView.RemovePanel();
         m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
                
                ObiNode nextNode = m_Node.FollowingNode;
                if (m_AudioBlock.Node.Parent == nextNode.Parent)
                {
                    if (m_Node.FollowingNode is PhraseNode)
                    {
                        m_Node = nextNode as PhraseNode;
                        if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                        {
                            panelZooomWaveform.Controls.Remove(m_AudioBlock);
                        }
                        m_AudioBlock = new AudioBlock((PhraseNode) nextNode, m_Strip);
                        panelRender();
                    }
                    txtZoomSelected.Text = " ";
                }

        }
       

        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            ObiNode previousNode = m_Node.PrecedingNode;
            if (m_AudioBlock.Node.Parent == previousNode.Parent)
            {
                if (m_Node.PrecedingNode is PhraseNode)
                {
                    m_Node = previousNode as PhraseNode;
                    if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                    {
                        panelZooomWaveform.Controls.Remove(m_AudioBlock);
                    }
                    m_AudioBlock = new AudioBlock((PhraseNode) previousNode, m_Strip);
                    panelRender();
                }
                txtZoomSelected.Text = " ";
            }

        }

        public void panelRender()
        {
            panelZooomWaveform.Controls.Add(m_AudioBlock);
            m_AudioBlock.Location = new Point(0, 0);
            initialWaveformWidth = m_AudioBlock.Waveform.Width;
            m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.InitCursor(0);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
;
            m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            
 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = initialWaveformWidth;
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);

        }

   

       // public Panel Panel_WaveForm { get { return panel_ZoomWaveform; } }
    }
}
