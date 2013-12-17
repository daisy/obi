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
        private bool Dragging = false;
        private Point DragStart = Point.Empty;
        private KeyboardShortcuts_Settings keyboardShortcuts;
        private bool IsSelection = false;
        private Size m_InitialtoolStripSize;
      



         
        public Toolbar_EditAudio()
        {
            InitializeComponent();
            m_InitialtoolStripSize = this.toolStrip1.Size;
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

          //  mInitialSizeOfPanel = this.Size;
            keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
           // keyboardShortcuts.MenuNameDictionary
          //  this.mbtnCopytoolStrip.ToolTipText= keyboardShortcuts.KeyboardShortcutsDescription["Copy"].Value.ToString();

            //this.mbtnCopytoolStrip.ToolTipText = "Copy (Cntrl+C)";
            //this.mbtnCuttoolStrip.ToolTipText = "Copy (Cntrl+X)";
            //this.mbtnDeletetoolStrip.ToolTipText="Delete (Del)";
            //this.
            this.mbtnCuttoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Cut") +"("+ FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCutToolStripMenuItem"].Value.ToString())+")";
            this.mbtnCopytoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Copy") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mCopyToolStripMenuItem"].Value.ToString()) + ")";
            this.mbtnDeletetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Delete") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mDeleteToolStripMenuItem"].Value.ToString()) + ")";
            this.mbtnPastetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Paste") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPasteToolStripMenuItem"].Value.ToString()) + ")";
            this.mbtnMergetoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Merge") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mMergePhraseWithNextToolStripMenuItem"].Value.ToString()) + ")";
            this.mbtnPraseDetectiontoolStrip.ToolTipText = Localizer.Message("EditAudioTT_PhraseDetect") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mPhrases_ApplyPhraseDetectionMenuItem"].Value.ToString()) + ")";
            this.mbtnSplittoolStrip.ToolTipText = Localizer.Message("EditAudioTT_Split") + "(" + FormatKeyboardShorcut(keyboardShortcuts.MenuNameDictionary["mSplitPhraseToolStripMenuItem"].Value.ToString()) + ")";
        }
        private string FormatKeyboardShorcut(string str)
        {
            string[] tempStore = str.Split(',');
            //return new string( charArray );
            if (tempStore.Length > 1)
            {
                str = tempStore[1] + "+" + tempStore[0];
            }
            return str;
        }

        private void ProjectViewSelectionChanged(object sender, EventArgs e)
        {
            EnableDisableCut();
        }


        public void EnableDisableCut()
        {
            mbtnCuttoolStrip.Enabled = (m_ContentView.CanRemoveAudio ) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnCopytoolStrip.Enabled = (m_ContentView.CanCopyAudio || m_ContentView.CanCopyBlock || m_ContentView.CanCopyStrip) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnPastetoolStrip.Enabled = m_ProjectView.CanPaste;
            mbtnSplittoolStrip.Enabled =  m_ProjectView.CanSplitPhrase;
            mbtnDeletetoolStrip.Enabled = (m_ContentView.CanRemoveAudio ) && !m_ProjectView.TransportBar.IsRecorderActive;
            mbtnMergetoolStrip.Enabled = m_ContentView.CanMergeBlockWithNext;
            
        }



        public void SetEditPanelFontSize(Size thisSize)
        {
            if (m_ContentView.ZoomFactor > 1.1 && m_ContentView.ZoomFactor < 4)
            {
                float tempZoomfactor;
                if (m_ContentView.ZoomFactor > 1.5)
                {
                    tempZoomfactor = 1.46f;
                }
                else
                {
                    tempZoomfactor = m_ContentView.ZoomFactor;
                }
              //  this.toolStrip1.Size = new Size((int)(this.toolStrip1.Size.Width + (this.toolStrip1.Size.Width * (tempZoomfactor - 1))), (int)(this.toolStrip1.Size.Height + (this.toolStrip1.Size.Height * (tempZoomfactor - 1))));
              //  this.toolStrip1.Size = thisSize;
                this.toolStrip1.MinimumSize = thisSize;
                this.toolStrip1.Font = new Font(this.toolStrip1.Font.Name, (this.toolStrip1.Font.Size + (float)3.0), FontStyle.Bold);
            }
            else
            {
                //this.toolStrip1.MinimumSize = new Size(574, 25);
                //this.toolStrip1.Size = new Size(574, 25);
                this.toolStrip1.MinimumSize = m_InitialtoolStripSize;
                this.toolStrip1.Size = m_InitialtoolStripSize;
                this.toolStrip1.Font = new Font(this.toolStrip1.Font.Name, (this.toolStrip1.Font.Size - (float)3.0), FontStyle.Regular);
            }
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

        private void toolStrip1_MouseHover(object sender, EventArgs e)
        {
            
        }
    }
}
