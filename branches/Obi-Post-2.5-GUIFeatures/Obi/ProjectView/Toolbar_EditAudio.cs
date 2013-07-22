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
        }

        private void mbtnCut_Click(object sender, EventArgs e)
        {
         //   m_ContentView.EditPanelControls(mCutEnable);
        }
        public void EnableDisableCut(bool cut,bool copy,bool paste,bool split,bool delete,bool merge)
        {
            mbtnCuttoolStrip.Enabled = cut;
            mbtnCopytoolStrip.Enabled = copy;
            mbtnPastetoolStrip.Enabled = paste;
            mbtnSplittoolStrip.Enabled = split;
            mbtnDeletetoolStrip.Enabled = delete;
          //  mbtnMergetoolStrip.Enabled = merge;
            
        }

       




        private void mbtnCuttoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mCutEnable);
        }

        private void mbtnCopytoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mCopyEnable);
        }

        private void mbtnPastetoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mPasteEnable);

        }

        private void mbtnSplittoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mSplitEnable);
        }

        private void mbtnDeletetoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mDeleteEnable);

        }

        private void mbtnMergetoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mMergeEnable);
        }

        private void mbtnPraseDetectiontoolStrip_Click(object sender, EventArgs e)
        {
            m_ContentView.EditPanelControls(mPhraseDetectEnable);
        }
    }
}
