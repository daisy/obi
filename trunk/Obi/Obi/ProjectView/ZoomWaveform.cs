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
    public partial class ZoomWaveform : UserControl, ISelectableInContentViewWithColors
    {
        private ContentView m_ContentView = null;
        private Strip m_Strip;
        private  EmptyNode m_Node;
        private AudioBlock m_AudioBlock;
        private int initialWaveformWidth = 0;
        private float m_ZoomFactor = 0;
        private ProjectView m_ProjectView;
        private Toolbar_EditAudio m_Edit;
        private bool m_IsPanelSizeMax = false;
        private int m_count = 0;
        private Size m_btnCloseSize;
        private Size m_btnNextPhraseSize;
        private Size m_btnPreviousPhraseSize;
        private Size m_btnZoomInPhraseSize;
        private Size m_btnZoomOutPhraseSize;
        private Size m_btnResetPhraseSize;
        private Size m_btnZoomSelectionSize;
        private Size m_btnResetSelectionSize;
        private bool m_buttonSizeinit = false;
        private bool flag = false;
        private AudioSelection audioSel;
       
        private ZoomWaveform()
        {
            
            InitializeComponent();
            this.Controls.Add(panelZooomWaveform);       
            this.Controls.Add(txtZoomSelected);
            
        }
        public void SetSelectionFromContentView(NodeSelection selection) 
        {
            if (m_AudioBlock == null 
                || (selection != null && selection.Node is EmptyNode &&  m_AudioBlock.Node != selection.Node))
            {
                PhraseLoad((EmptyNode) selection.Node);
            }
            if (m_AudioBlock != null) m_AudioBlock.SetSelectionFromContentView(selection);
            txtZoomSelected.Focus();
        }
        private bool m_Highlighted;
        public bool Highlighted { get { return m_Highlighted; } set { m_Highlighted =  value; } }
        public ObiNode ObiNode { get { return m_Node; } }
        ColorSettings m_ColorSettings ;
        public ColorSettings ColorSettings { get { return m_ColorSettings; } set { m_ColorSettings = value; } }
        public ContentView ContentView { get { return m_ContentView; } }
        public Strip Strip { get { return m_Strip; } }
        public string ToMatch() { return null; }

        public void UpdateCursorTime (double time ) 
        {
            if( m_AudioBlock != null ) m_AudioBlock.UpdateCursorTime (time) ;
            
        }
        public ZoomWaveform ZoomWaveformObj
        {
            get { return this; }
        }
  

        public EmptyNode ZoomPanelNode
        {
            get
            {
                return m_Node;
            }
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
            //    Type type = m_ProjectView.Selection.GetType();
                //if (!(m_ProjectView.Selection is Obi.NodeSelection))
                //{
                //    audioSel = (AudioSelection)m_ProjectView.Selection;
                //    if (audioSel.AudioRange.SelectionBeginTime != audioSel.AudioRange.SelectionEndTime)
                //    {
                //        mbtnZoomSelection.Enabled = true;
                //    }
                //    else
                //    {
                //        mbtnZoomSelection.Enabled = false;
                //    }

                //    txtZoomSelected.Text += m_ProjectView.Selection.Phrase.ToString();
                //}

            }
        }
         private void ZoomPanelLostFocus(object sender,EventArgs e)
         {
            // this.ActiveControl = panelZooomWaveform;
             if(this.ActiveControl==txtZoomSelected)
             {
                 this.ActiveControl = txtZoomSelected;
             }
         }
        private void ZoomPanelResize(object sender,EventArgs e)
        {
            this.BringToFront();
            if (m_ContentView.ZoomFactor > 1)
            {
                this.Height = m_ContentView.Height;
            }
            else
            {
                this.Height = m_ContentView.Height;
            }
            this.AutoScroll = true;
            this.Width = m_ContentView.Width;
            txtZoomSelected.Width = this.Width - 40;           
        }
         public void ZoomAudioFocus()
         {
             if(this.ActiveControl==btnClose || this.ActiveControl==txtZoomSelected)
             {
                m_AudioBlock.TabStop = false;
             }
             if(m_AudioBlock.FlagMouseDown)
             {
                 m_AudioBlock.FlagMouseDown = false;
                 this.ActiveControl = txtZoomSelected;
             }
            // this.ActiveControl = btnClose;
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
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip,true);
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
               // txtZoomSelected.Location = new Point(0, this.Height - 50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
                //   m_AudioBlock.Width = m_ContentView.Width;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                if (m_AudioBlock != null)
                {
                //    m_AudioBlock.MouseDown += new MouseEventHandler(m_AudioBlock_MouseDown);
                    m_AudioBlock.GotFocus += new EventHandler(m_AudioBlock_MouseDown);
                }
                m_AudioBlock.TabStop = false;
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
            this.LostFocus += new EventHandler(ZoomPanelLostFocus);

          
            //panelZooomWaveform.LostFocus+=new EventHandler(ZoomPanelLostFocus);
            m_ContentView.Resize += new EventHandler(ZoomPanelResize);
            m_Strip = strip;
            m_Node = node;
            if (m_ProjectView.Selection.Phrase != null)
            {
                if (m_ContentView != null)
                {                    
                    this.Height = m_ContentView.Height;
                    //this.Height = m_ContentView.Height-22;
                    this.Width = m_ContentView.Width;
                    this.MouseWheel += new MouseEventHandler(ZoomWaveform_MouseWheel);
                  //  btnClose.Anchor = AnchorStyles.None;
                    btnClose.Location = new Point(btnClose.Location.X, this.Height - 25);
                   // btnNextPhrase.Anchor = AnchorStyles.None;
                    btnNextPhrase.Location = new Point(btnNextPhrase.Location.X, this.Height - 25);
                    btnPreviousPhrase.Location = new Point(btnPreviousPhrase.Location.X, this.Height - 25);
                    btnReset.Location = new Point(btnReset.Location.X, this.Height - 25);
                    btnZoomIn.Location = new Point(btnZoomIn.Location.X, this.Height - 25);
                    btnZoomOut.Location = new Point(btnZoomOut.Location.X, this.Height - 25);
                    mbtnZoomSelection.Location = new Point(mbtnZoomSelection.Location.X, this.Height - 25);
                  //  mbtnResetSelection.Location = new Point(mbtnResetSelection.Location.X, this.Height - 25);
                    panelZooomWaveform.Width = this.Width - 30;
                    panelZooomWaveform.Height = this.Height - 90;
                    txtZoomSelected.Width = this.Width - 40;
                    txtZoomSelected.Location = new Point(0, this.Height - 50);
                    
                }
            }
           //this.Width=m_ContentView.Width;
            if (m_Node is PhraseNode)
            {
               
                m_AudioBlock = new AudioBlock((PhraseNode)m_Node, m_Strip, true);
                
                panelZooomWaveform.Controls.Add(m_AudioBlock);
                m_AudioBlock.Location = new Point(0, 0);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                txtZoomSelected.Location = new Point(0,this.Height-50);
                txtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection)
                {
                    audioSel = (AudioSelection)m_ProjectView.Selection;
                    if (audioSel.AudioRange.SelectionBeginTime != audioSel.AudioRange.SelectionEndTime)
                    {
                        m_AudioBlock.SetTimeBoundsForWaveformDisplay(audioSel.AudioRange.SelectionBeginTime, audioSel.AudioRange.SelectionEndTime);
                    }
                }
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                if (m_buttonSizeinit == false)
                {
                    m_btnCloseSize = btnClose.Size;
                    m_btnNextPhraseSize = btnNextPhrase.Size;
                    m_btnPreviousPhraseSize = btnPreviousPhrase.Size;
                    m_btnResetPhraseSize = btnReset.Size;
                    m_btnZoomInPhraseSize = btnZoomIn.Size;
                    m_btnZoomOutPhraseSize = btnZoomOut.Size;
                    m_btnZoomSelectionSize = mbtnZoomSelection.Size;
              //      m_btnResetSelectionSize = mbtnResetSelection.Size;
                    m_buttonSizeinit = true;
                }
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
                    this.BringToFront();
                
                    btnClose.Size = new Size((int)(btnClose.Size.Width + (btnClose.Size.Width * (tempZoomfactor - 1))), (int)(btnClose.Size.Height + (btnClose.Size.Height * (tempZoomfactor - 1))));                    
                    btnNextPhrase.Size = new Size((int)(btnNextPhrase.Size.Width + (btnNextPhrase.Size.Width * (tempZoomfactor - 1))), (int)(btnNextPhrase.Size.Height + (btnNextPhrase.Size.Height * (tempZoomfactor - 1))));
                    btnPreviousPhrase.Size = new Size((int)(btnPreviousPhrase.Size.Width + (btnPreviousPhrase.Size.Width * (tempZoomfactor - 1))), (int)(btnPreviousPhrase.Size.Height + (btnPreviousPhrase.Size.Height * (tempZoomfactor - 1))));
                    btnReset.Size = new Size((int)(btnReset.Size.Width + (btnReset.Size.Width * (tempZoomfactor - 1))), (int)(btnReset.Size.Height + (btnReset.Size.Height * (tempZoomfactor - 1))));
                    btnZoomIn.Size = new Size((int)(btnZoomIn.Size.Width + (btnZoomIn.Size.Width * (tempZoomfactor - 1))), (int)(btnZoomIn.Size.Height + (btnZoomIn.Size.Height * (tempZoomfactor - 1))));
                    btnZoomOut.Size = new Size((int)(btnZoomOut.Size.Width + (btnZoomOut.Size.Width * (tempZoomfactor - 1))), (int)(btnZoomOut.Size.Height + (btnZoomOut.Size.Height * (tempZoomfactor - 1))));
                //    mbtnResetSelection.Size = new Size((int)(mbtnResetSelection.Size.Width + (mbtnResetSelection.Size.Width * (tempZoomfactor - 1))), (int)(mbtnResetSelection.Size.Height + (mbtnResetSelection.Size.Height * (tempZoomfactor - 1))));
                    mbtnZoomSelection.Size = new Size((int)(mbtnZoomSelection.Size.Width + (mbtnZoomSelection.Size.Width * (tempZoomfactor - 1))), (int)(mbtnZoomSelection.Size.Height + (mbtnZoomSelection.Size.Height * (tempZoomfactor - 1))));


                    txtZoomSelected.Font = new Font(txtZoomSelected.Font.Name, (txtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                    btnClose.Font = new Font(btnClose.Font.Name, (btnClose.Font.Size + (float)3.0), FontStyle.Bold);
                    btnNextPhrase.Font = new Font(btnNextPhrase.Font.Name, (btnNextPhrase.Font.Size + (float)3.0), FontStyle.Bold);
                    btnPreviousPhrase.Font = new Font(btnPreviousPhrase.Font.Name, (btnPreviousPhrase.Font.Size + (float)3.0), FontStyle.Bold);
                    btnReset.Font = new Font(btnReset.Font.Name, (btnReset.Font.Size + (float)3.0), FontStyle.Bold);
                    btnZoomIn.Font = new Font(btnZoomIn.Font.Name, (btnZoomIn.Font.Size + (float)3.0), FontStyle.Bold);
                    btnZoomOut.Font = new Font(btnZoomOut.Font.Name, (btnZoomOut.Font.Size + (float)3.0), FontStyle.Bold);
                  //  mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size + (float)3.0), FontStyle.Bold);
                    mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size + (float)3.0), FontStyle.Bold);
                    flag = true;

                    this.AutoScrollMinSize = new Size(this.Width, this.Height + 35);
                }
                else
                {
                    this.AutoScrollMinSize = new Size(this.Width, this.Height + 15);
                }
                m_AudioBlock.InitCursor(0);
                m_AudioBlock.Focus();
                this.ActiveControl = btnClose;
            }
            m_count = 0;
            m_Edit = new Toolbar_EditAudio(m_ContentView, m_Strip, m_Node, m_ProjectView);
            this.Controls.Add(m_Edit);
            m_Edit.Show();
            m_Edit.Location = new Point(39,this.Height-83);
            m_Edit.BringToFront();
        }

       void ZoomWaveform_MouseWheel(object sender, MouseEventArgs e)
       {
           Console.WriteLine("Delta Value of MouseWheel is{0}",e.Delta);
           if(e.Delta<0)
           {
               int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
               if (tempWidth > (initialWaveformWidth / 10))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
               }
               
           }
           if(e.Delta>0)
           {
               int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
               if (tempWidth < (initialWaveformWidth * 60))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
               }
           }
           //  throw new NotImplementedException();
       }

       private void m_AudioBlock_MouseDown(object sender, EventArgs e)
       {

          // throw new NotImplementedException();
       }



        
      

        private void btnClose_Click(object sender, EventArgs e)
        {
         m_buttonSizeinit = false;
         m_ContentView.RemovePanel();
         m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
        }

        private void btnNextPhrase_Click(object sender, EventArgs e)
        {
                
                ObiNode nextNode = m_Node.FollowingNode;
            if (nextNode!=null && nextNode.Parent != null && m_AudioBlock.Node.Parent != null)
            {
                if (m_AudioBlock.Node.Parent == nextNode.Parent)
                {
                    if (m_Node.FollowingNode is PhraseNode)
                    {
                        m_Node = nextNode as PhraseNode;
                        m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                        //if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                        //{
                            //panelZooomWaveform.Controls.Remove(m_AudioBlock);
                        //}
                        //m_AudioBlock = new AudioBlock((PhraseNode) nextNode, m_Strip);
                        //panelRender();
                    }
                    txtZoomSelected.Text = " ";
                }
            }

        }


        private void btnPreviousPhrase_Click(object sender, EventArgs e)
        {
            ObiNode previousNode = m_Node.PrecedingNode;
            if (previousNode != null && previousNode.Parent != null && m_AudioBlock.Node.Parent != null)
            {
                if (m_AudioBlock.Node.Parent == previousNode.Parent)
                {
                    if (m_Node.PrecedingNode is PhraseNode)
                    {
                        m_Node = previousNode as PhraseNode;
                        m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                        //if (panelZooomWaveform.Controls.Contains(m_AudioBlock))
                        //{
                            //panelZooomWaveform.Controls.Remove(m_AudioBlock);
                        //}
                        //m_AudioBlock = new AudioBlock((PhraseNode) previousNode, m_Strip);
                        //panelRender();
                    }
                    txtZoomSelected.Text = " ";
                }

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
            int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
            if (tempWidth < (initialWaveformWidth * 60))
            {
                m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
                m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            }

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
            if (tempWidth > (initialWaveformWidth / 10))
            {
                m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
                m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            }
            
 
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            m_AudioBlock.Waveform.Width = initialWaveformWidth;
            m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height);
            m_AudioBlock.ResetTimeBoundsForWaveformDisplay();

        }

 protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
     if ( m_ProjectView.ObiForm.KeyboardShortcuts == null ) return false ;
     KeyboardShortcuts_Settings keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
            //this.Focus();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
           if (keyData == Keys.Tab
                && this.ActiveControl != null)
            {
                Console.WriteLine("It will come in Tab");
                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, true, true);
                
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();
                //Console.WriteLine("Active Control After {0}", this.ActiveControl);
                return true;
            }
            else if (keyData == (Keys)(Keys.Shift | Keys.Tab)
                && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, true, true);
                if (this.ActiveControl != null && c.TabIndex < this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();

                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_SelectUp.Value)
            {
                if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null)
                {
                    IControlWithSelection tempControl;
                    tempControl = m_ProjectView.Selection.Control;
                    m_ProjectView.Selection = new NodeSelection((ObiNode)m_Node, tempControl);
                }
            }
            else if (keyData == Keys.Right || keyData == Keys.Left)
            {
                return false;
            }
            if (keyData == keyboardShortcuts.ContentView_ScrollDown_LargeIncrementWithSelection.Value || keyData == keyboardShortcuts.ContentView_ScrollUp_LargeIncrementWithSelection.Value
                || keyData == keyboardShortcuts.ContentView_SelectFollowingStripCursor.Value || keyData == keyboardShortcuts.ContentView_SelectPrecedingStripCursor.Value
                || keyData == keyboardShortcuts.ContentView_SelectFollowingStrip.Value || keyData == keyboardShortcuts.ContentView_SelectPrecedingStrip.Value
                || keyData == keyboardShortcuts.ContentView_SelectFirstStrip.Value || keyData == keyboardShortcuts.ContentView_SelectLastStrip.Value)
            // to do: handle global shortcuts after merging with trunck
            // || keyData == (Keys.Control | Keys.H)
            //|| keyData == (Keys.Control | Keys.Alt | Keys.H) || keyData == (Keys.Control | Keys.Shift | Keys.Q) || keyData == (Keys.Control | Keys.B)
            //|| keyData == (Keys.Control | Keys.Shift | Keys.B) || keyData == (Keys.Control | Keys.Shift | Keys.Space))
            {
                return false;
            }
           
            return base.ProcessCmdKey(ref msg, keyData);
            // return true;

        }
        /*
        protected override bool ProcessTabKey(bool forward)
        {
            Console.WriteLine("Active Control is {0}",this.ActiveControl);
            this.Focus();
            if (this.ActiveControl == txtZoomSelected)
            {
                this.ActiveControl = btnClose;
            }
            else
            {
                return base.ProcessTabKey(forward);
                
            }
            return forward;
        }
         */ 
        public float ZoomFactor
        {
           set
            {
                
                Console.WriteLine("Value of the zooming {0}",value);
                if (value > 1.1 && value < 1.5)
                {
                    if (flag == false)
                    {
                        btnClose.Size = new Size((int)(btnClose.Size.Width + (btnClose.Size.Width * (value - 1))), (int)(btnClose.Size.Height + (btnClose.Size.Height * (value - 1))));
                        btnNextPhrase.Size = new Size((int)(btnNextPhrase.Size.Width + (btnNextPhrase.Size.Width * (value - 1))), (int)(btnNextPhrase.Size.Height + (btnNextPhrase.Size.Height * (value - 1))));
                        btnPreviousPhrase.Size = new Size((int)(btnPreviousPhrase.Size.Width + (btnPreviousPhrase.Size.Width * (value - 1))), (int)(btnPreviousPhrase.Size.Height + (btnPreviousPhrase.Size.Height * (value - 1))));
                        btnReset.Size = new Size((int)(btnReset.Size.Width + (btnReset.Size.Width * (value - 1))), (int)(btnReset.Size.Height + (btnReset.Size.Height * (value - 1))));
                        btnZoomIn.Size = new Size((int)(btnZoomIn.Size.Width + (btnZoomIn.Size.Width * (value - 1))), (int)(btnZoomIn.Size.Height + (btnZoomIn.Size.Height * (value - 1))));
                        btnZoomOut.Size = new Size((int)(btnZoomOut.Size.Width + (btnZoomOut.Size.Width * (value - 1))), (int)(btnZoomOut.Size.Height + (btnZoomOut.Size.Height * (value - 1))));
                        mbtnZoomSelection.Size = new Size((int)(mbtnZoomSelection.Size.Width + (mbtnZoomSelection.Size.Width * (value - 1))), (int)(mbtnZoomSelection.Size.Height + (mbtnZoomSelection.Size.Height * (value - 1))));
                    //    mbtnResetSelection.Size = new Size((int)(mbtnResetSelection.Size.Width + (mbtnResetSelection.Size.Width * (value - 1))), (int)(mbtnResetSelection.Size.Height + (mbtnResetSelection.Size.Height * (value - 1))));

                        txtZoomSelected.Font = new Font(txtZoomSelected.Font.Name, (txtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                        btnClose.Font = new Font(btnClose.Font.Name, (btnClose.Font.Size + (float)3.0), FontStyle.Bold);
                        btnNextPhrase.Font = new Font(btnNextPhrase.Font.Name, (btnNextPhrase.Font.Size + (float)3.0), FontStyle.Bold);
                        btnPreviousPhrase.Font = new Font(btnPreviousPhrase.Font.Name, (btnPreviousPhrase.Font.Size + (float)3.0), FontStyle.Bold);
                        btnReset.Font = new Font(btnReset.Font.Name, (btnReset.Font.Size + (float)3.0), FontStyle.Bold);
                        btnZoomIn.Font = new Font(btnZoomIn.Font.Name, (btnZoomIn.Font.Size + (float)3.0), FontStyle.Bold);
                        btnZoomOut.Font = new Font(btnZoomOut.Font.Name, (btnZoomOut.Font.Size + (float)3.0), FontStyle.Bold);
                        mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size + (float)3.0), FontStyle.Bold);
                     //   mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size + (float)3.0), FontStyle.Bold);

                        flag = true;
                    }
                }
                if (value <= 1.1)
                {                
                    
                    btnClose.Size = new Size(m_btnCloseSize.Width, m_btnCloseSize.Height);                   
                    btnNextPhrase.Size = new Size(m_btnNextPhraseSize.Width, m_btnNextPhraseSize.Height);
                    btnPreviousPhrase.Size = new Size(m_btnPreviousPhraseSize.Width, m_btnPreviousPhraseSize.Height);
                    btnReset.Size = new Size(m_btnResetPhraseSize.Width, m_btnResetPhraseSize.Height);
                    btnZoomIn.Size = new Size(m_btnZoomInPhraseSize.Width, m_btnZoomInPhraseSize.Height);
                    btnZoomOut.Size = new Size(m_btnZoomOutPhraseSize.Width, m_btnZoomOutPhraseSize.Height);
                    mbtnZoomSelection.Size = new Size(m_btnZoomSelectionSize.Width, m_btnZoomSelectionSize.Height);
                 //   mbtnResetSelection.Size = new Size(m_btnResetSelectionSize.Width, m_btnResetSelectionSize.Height);
                    if (flag)
                    {
                        txtZoomSelected.Font = new Font(txtZoomSelected.Font.Name, (txtZoomSelected.Font.Size - (float)3.0), FontStyle.Regular);
                        btnClose.Font = new Font(btnClose.Font.Name, (btnClose.Font.Size - (float)3.0), FontStyle.Regular);
                        btnNextPhrase.Font = new Font(btnNextPhrase.Font.Name, (btnNextPhrase.Font.Size - (float)3.0), FontStyle.Regular);
                        btnPreviousPhrase.Font = new Font(btnPreviousPhrase.Font.Name, (btnPreviousPhrase.Font.Size - (float)3.0), FontStyle.Regular);
                        btnReset.Font = new Font(btnReset.Font.Name, (btnReset.Font.Size - (float)3.0), FontStyle.Regular);
                        btnZoomIn.Font = new Font(btnZoomIn.Font.Name, (btnZoomIn.Font.Size - (float)3.0), FontStyle.Regular);
                        btnZoomOut.Font = new Font(btnZoomOut.Font.Name, (btnZoomOut.Font.Size - (float)3.0), FontStyle.Regular);
                        mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size - (float)3.0), FontStyle.Regular);
                    //    mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size - (float)3.0), FontStyle.Regular);

                    }
                    flag = false;
                    
                }
            }
        }

        private void mbtnZoomSelection_Click(object sender, EventArgs e)
        {
            if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null)
            {
                audioSel = (AudioSelection)m_ProjectView.Selection;
                if (audioSel.AudioRange.SelectionBeginTime != audioSel.AudioRange.SelectionEndTime)
                {
                    m_AudioBlock.SetTimeBoundsForWaveformDisplay(audioSel.AudioRange.SelectionBeginTime, audioSel.AudioRange.SelectionEndTime);
                //    mbtnResetSelection.Enabled = true;
                }
            }
           
        }

        private void mbtnResetSelection_Click(object sender, EventArgs e)
        {
            m_AudioBlock.ResetTimeBoundsForWaveformDisplay();
          //  mbtnResetSelection.Enabled = false;
        }
   

       // public Panel Panel_WaveForm { get { return panel_ZoomWaveform; } }
    }
}
