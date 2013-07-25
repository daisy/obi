using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    public partial class Toolbar_EditAudio : UserControl
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private EmptyNode m_Node;
        private ProjectView m_ProjectView;
        private const int mCutEnable = 1;
        private const int mCopyEnable = 2;
        private const int mPasteEnable = 3;
        private const int mSplitEnable = 4;
        private const int mDeleteEnable = 5;
        private const int mMergeEnable = 6;
        private const int mPhraseDetectEnable = 7;
        private bool Dragging = false;
        private Point DragStart = Point.Empty;



         
        public Toolbar_EditAudio()
        {
            InitializeComponent();
        }
        public Toolbar_EditAudio(ContentView contentView, Strip strip, EmptyNode node, ProjectView mProjectView)
            : this()
        {
            m_ContentView = contentView;
            m_Strip = strip;
            m_Node = node;
            m_ProjectView = mProjectView;          
            m_ProjectView.SelectionChanged+= new EventHandler(ProjectViewSelectionChanged);
         
            this.toolStrip1.MouseDown+= new MouseEventHandler(Toolbar_EditAudio_MouseDown);
            this.toolStrip1.MouseUp+= new MouseEventHandler(Toolbar_EditAudio_MouseUp);
            this.toolStrip1.MouseMove += new MouseEventHandler(Toolbar_EditAudio_MouseMove);

            this.toolStrip1.MinimumSize = this.Size;
            this.toolStrip1.MaximumSize = this.Size;
            this.toolStrip1.Size = this.Size;
        }

        private void ProjectViewSelectionChanged(object sender, EventArgs e)
        {
            EnableDisableCut();
        }


        public void EnableDisableCut()
        {
            mbtnCuttoolStrip.Enabled = (m_ContentView.CanRemoveAudio || m_ContentView.CanRemoveBlock || m_ContentView.CanRemoveStrip) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnCopytoolStrip.Enabled = (m_ContentView.CanCopyAudio || m_ContentView.CanCopyBlock || m_ContentView.CanCopyStrip) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnPastetoolStrip.Enabled = m_ProjectView.CanPaste;
            mbtnSplittoolStrip.Enabled = m_ContentView.CanSplitStrip && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnDeletetoolStrip.Enabled = (m_ContentView.CanRemoveAudio || m_ContentView.CanRemoveBlock || m_ContentView.CanRemoveStrip) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnMergetoolStrip.Enabled = m_ContentView.CanMergeBlockWithNext;
            
        }

       




        private void mbtnCuttoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Cut();
        }

        private void mbtnCopytoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Copy();
        }

        private void mbtnPastetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Paste();

        }

        private void mbtnSplittoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.SplitPhrase();
        }

        private void mbtnDeletetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.Delete();

        }

        private void mbtnMergetoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.MergeBlockWithNext();
        }

        private void mbtnPraseDetectiontoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.ApplyPhraseDetection();
        }

        private void toolStrip1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void toolStrip1_MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void toolStrip1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Toolbar_EditAudio_MouseDown(object sender, MouseEventArgs e)
        {
            Dragging = true;
            DragStart = new Point(e.X, e.Y);
            toolStrip1.Capture = true;
        }

        private void Toolbar_EditAudio_MouseMove(object sender, MouseEventArgs e)
        {
            if (Dragging)
            {
               
                    this.Left = Math.Max(0, e.X + this.Left - DragStart.X);
               
                    this.Top = Math.Max(0, e.Y + this.Top - DragStart.Y);
            }
         
        }

        private void Toolbar_EditAudio_MouseUp(object sender, MouseEventArgs e)
        {
            Dragging = false;
            toolStrip1.Capture = false;
        }
    }
}
