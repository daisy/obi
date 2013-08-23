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
        private Block m_Block;
        private int initialWaveformWidth = 0;
        private float m_ZoomFactor = 0;
        private ProjectView m_ProjectView;
        private Toolbar_EditAudio m_Edit;
        private bool m_IsPanelSizeMax = false;
        private int m_count = 0;

        private Size m_toolStripSize;       
        private Size m_EditSize;
        private Size m_btntxtZoomSelectedSize;
        private bool m_ResizeIsDone = false;
        private bool m_buttonSizeinit = false;
        private bool flag = false;
        private AudioSelection audioSel;
        private int m_PreviousHeight = 0;
       
        private ZoomWaveform()
        {
            
            InitializeComponent();
            this.Controls.Add(panelZooomWaveform);       
            this.Controls.Add(btntxtZoomSelected);
            
        }
        public void SetSelectionFromContentView(NodeSelection selection) 
        {
            if (m_AudioBlock == null 
                || (selection != null && selection.Node is EmptyNode &&  m_AudioBlock.Node != selection.Node))
            {
                PhraseLoad((EmptyNode) selection.Node);
            }
            if (m_AudioBlock != null) m_AudioBlock.SetSelectionFromContentView(selection);
            btntxtZoomSelected.Focus();
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
            if (m_ProjectView.Selection != null && m_ProjectView.GetSelectedPhraseSection != null)
            {
                btntxtZoomSelected.Text = " ";
                btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                btntxtZoomSelected.Text += " " + m_ProjectView.GetSelectedPhraseSection.ToString();

                if ( m_ProjectView.Selection.Phrase != null )
                {
                    //btntxtZoomSelected.Text=" ";
                    //btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                    //btntxtZoomSelected.Text +=" "+ m_ProjectView.GetSelectedPhraseSection.ToString();
                    string temp = m_ProjectView.Selection.Node.ToString();
                    if (m_AudioBlock.Node.ToString() != temp && m_ProjectView.Selection.EmptyNodeForSelection != null)
                    {
                        PhraseLoad(m_ProjectView.Selection.EmptyNodeForSelection);
                    }

                }
            }
            if( m_ProjectView==null || m_ProjectView.Selection==null ||!(m_ProjectView.Selection.Node is EmptyNode))
            {
                IsNewProjectOpened();
            }
        }
         private void ZoomPanelLostFocus(object sender,EventArgs e)
         {
             if(this.ActiveControl==btntxtZoomSelected)
             {
                 this.ActiveControl = btntxtZoomSelected;
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
            m_ResizeIsDone = true;
            this.Width = m_ContentView.Width;
            btntxtZoomSelected.Width = this.Width - 40;           
        }
         public void ZoomAudioFocus()
         {
           //  if(this.ActiveControl==btnClosetoolStrip || this.ActiveControl==btntxtZoomSelected)
             if (this.ActiveControl == btntxtZoomSelected)
             {
                m_AudioBlock.TabStop = false;
             }
             if(m_AudioBlock.FlagMouseDown)
             {
                 m_AudioBlock.FlagMouseDown = false;
                 this.ActiveControl = btntxtZoomSelected;
             }
            // this.ActiveControl = btnClose;
         }
        public void PhraseLoad(EmptyNode phrase)
        {
            //m_AudioBlock.Node.Parent;
            m_Node = phrase;
            if (m_Node is PhraseNode)
            {
                if (panelZooomWaveform.Controls.Contains(m_Block))
                {
                    panelZooomWaveform.Controls.Remove(m_Block);
                }
               m_Block = new AudioBlock((PhraseNode)m_Node, m_Strip,true);
               m_AudioBlock = (AudioBlock)m_Block;              

                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock.Location = new Point(5, 5);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
               // btntxtZoomSelected.Location = new Point(0, this.Height - 50);
                btntxtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
                //   m_AudioBlock.Width = m_ContentView.Width;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
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
                m_Block = m_AudioBlock;
                
            }
            else if (m_Node is EmptyNode)
            {
                if (panelZooomWaveform.Controls.Contains(m_Block))
                {
                    panelZooomWaveform.Controls.Remove(m_Block);
                }             
                m_Block = new Block(m_Node, m_Strip) ;               
                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock = null;

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

       //    mtoolTipZoomWaveform.SetToolTip(this.btnClose,"Close Zoom Panel");

            if (m_ProjectView.Selection.Phrase != null || m_Node is EmptyNode)
            {
                if (m_ContentView != null)
                {                    
                    this.Height = m_ContentView.Height;
                    //this.Height = m_ContentView.Height-22;
                    this.Width = m_ContentView.Width;
                    m_Edit = new Toolbar_EditAudio(m_ContentView, m_Strip, m_Node, m_ProjectView);
                    this.Controls.Add(m_Edit);
                    m_Edit.Show();

                    this.MouseWheel += new MouseEventHandler(ZoomWaveform_MouseWheel);
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


                        panelZooomWaveform.Height = this.Height - (toolStripZoomPanel.Height + btntxtZoomSelected.Height + m_Edit.Height + 15);
                        btntxtZoomSelected.Size = new Size((int)(btntxtZoomSelected.Size.Width + (btntxtZoomSelected.Size.Width * (tempZoomfactor - 1))), (int)(btntxtZoomSelected.Size.Height + (btntxtZoomSelected.Size.Height * (tempZoomfactor - 1))));
                        toolStripZoomPanel.Size = new Size((int)(toolStripZoomPanel.Size.Width + (toolStripZoomPanel.Size.Width * (tempZoomfactor - 1))), (int)(toolStripZoomPanel.Size.Height + (toolStripZoomPanel.Size.Height * (tempZoomfactor - 1))));
                        m_Edit.Size = new Size((int)(m_Edit.Size.Width + (m_Edit.Size.Width * (tempZoomfactor - 1))), (int)(m_Edit.Size.Height + (m_Edit.Size.Height * (tempZoomfactor - 1))));
                       // panelZooomWaveform.Size = new Size((int)(panelZooomWaveform.Size.Width), (int)(panelZooomWaveform.Size.Height + (panelZooomWaveform.Size.Height * (tempZoomfactor - 1))));
                        //    flowLayoutPanel1.Size = new Size((int)(flowLayoutPanel1.Size.Width + (flowLayoutPanel1.Size.Width * (tempZoomfactor - 1))), (int)(flowLayoutPanel1.Size.Height + (flowLayoutPanel1.Size.Height * (tempZoomfactor - 1))));



                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size + (float)3.0), FontStyle.Bold);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size + (float)3.0), FontStyle.Bold);                        
                        //  mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size + (float)3.0), FontStyle.Bold);
                        //   mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size + (float)3.0), FontStyle.Bold);
                        flag = true;
                        toolStripZoomPanel.Location = new Point(0, this.Height);
                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height - 4);
                        m_Edit.Location = new Point(5, this.Height - m_Edit.Height - btntxtZoomSelected.Height);

                        this.AutoScrollMinSize = new Size(this.Width, this.Height + 35);
                    }
                    else
                    {
                        toolStripZoomPanel.Location = new Point(0, this.Height - toolStripZoomPanel.Height - 2);
                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height - toolStripZoomPanel.Height - 4);
                        m_Edit.Location = new Point(5, this.Height - m_Edit.Height - toolStripZoomPanel.Height - btntxtZoomSelected.Height - 8);
                        panelZooomWaveform.Height = this.Height - (toolStripZoomPanel.Height + btntxtZoomSelected.Height + m_Edit.Height + 15);
                        this.AutoScrollMinSize = new Size(this.Width, this.Height + 15);
                    }
                   
                             
                    panelZooomWaveform.Width = this.Width - 30;
                    panelZooomWaveform.Location = new Point(0, 0);
                   
                m_Edit.BringToFront();
                //    Console.WriteLine("Edit Toolbar position Inside the Condition {0}", m_Edit.Location);
                     
                  
                   // panelZooomWaveform.Height = this.Height - 100;
                   

                    btntxtZoomSelected.Width = this.Width - 40;
             
                }
            }
           //this.Width=m_ContentView.Width;
            if (m_Node is PhraseNode)
            {
               
                m_Block = new AudioBlock((PhraseNode)m_Node, m_Strip, true);
                m_AudioBlock = (AudioBlock)m_Block;
                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock.Location = new Point(5,5);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
            //    btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height-toolStripZoomPanel.Height-4);
                btntxtZoomSelected.BringToFront();
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
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                if (m_buttonSizeinit == false)
                {

                    m_toolStripSize = toolStripZoomPanel.Size;
                    m_EditSize = m_Edit.Size;
                    m_btntxtZoomSelectedSize = btntxtZoomSelected.Size;
                    

                //    m_flowLayoutPanelSize = flowLayoutPanel1.Size;
              //      m_btnResetSelectionSize = mbtnResetSelection.Size;
                    m_buttonSizeinit = true;
                }

                m_AudioBlock.InitCursor(0);
                m_AudioBlock.Focus();
                //this.ActiveControl = btnClosetoolStrip;
                m_Block = m_AudioBlock;
            }
            else if (m_Node is EmptyNode)
            {
               
                m_Block = new Block(m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock = null;

            }

            m_count = 0;
            //m_Edit = new Toolbar_EditAudio(m_ContentView, m_Strip, m_Node, m_ProjectView);
            //this.Controls.Add(m_Edit);
            //m_Edit.Show();
            //m_Edit.Location = new Point(39,this.Height-83);
            //m_Edit.BringToFront();


            btntxtZoomSelected.Text = " ";
            btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
            btntxtZoomSelected.Text += " " + m_ProjectView.GetSelectedPhraseSection.ToString();
            m_PreviousHeight = this.Height;
            if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight )
            {
                this.AutoScroll = false;
            }
            else
            {
                this.AutoScroll = true;
            }

        }

       void ZoomWaveform_MouseWheel(object sender, MouseEventArgs e)
       {
           if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
           {
               this.AutoScroll = false;
           }
           else
           {
               this.AutoScroll = true;
           }
           if(e.Delta<0 && m_Node is PhraseNode)
           {
               int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
               if (tempWidth > (initialWaveformWidth / 10))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
               }
               
           }
           if (e.Delta > 0 && m_Node is PhraseNode)
           {
               int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
               if (tempWidth < (initialWaveformWidth * 60))
               {
                   m_AudioBlock.Waveform.Width = tempWidth;
                   m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                   m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                   m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
               }
           }
           //  throw new NotImplementedException();
       }

       private void m_AudioBlock_MouseDown(object sender, EventArgs e)
       {

          // throw new NotImplementedException();
       }



        
      


        public void panelRender()
        {
            panelZooomWaveform.Controls.Add(m_AudioBlock);
            m_AudioBlock.Location = new Point(5, 5);
            initialWaveformWidth = m_AudioBlock.Waveform.Width;
            m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);
            m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
            m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
            m_AudioBlock.InitCursor(0);
        }



 protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
     if ( m_ProjectView.ObiForm.KeyboardShortcuts == null ) return false ;
     KeyboardShortcuts_Settings keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
            //this.Focus();


            if (keyData == keyboardShortcuts.ContentView_SelectPrecedingPhrase.Value && this.ActiveControl!=toolStripZoomPanel && this.ActiveControl!=m_Edit)
            {
                m_ContentView.NudgeInFineNavigation(false);
                return true; 
            }
            else if (keyData == keyboardShortcuts.ContentView_SelectFollowingPhrase.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_ContentView.NudgeInFineNavigation(true);
                return true; 
            }
            else if (keyData == keyboardShortcuts.ContentView_ScrollDown_SmallIncrementWithSelection.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_ContentView.NudgeIntervalIncrement(false);
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ScrollUp_SmallIncrementWithSelection.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_ContentView.NudgeIntervalIncrement(true);
                return true;
            }
            else if (keyData == Keys.Tab
        && this.ActiveControl != null)
            {
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
                if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null && m_ProjectView.Selection is Obi.AudioSelection)
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
 
        public float ZoomFactor
        {
           set
            {
                
   
                if (value > 1.1 && value < 1.5)
                {
                    if (flag == false)
                    {
                     

                        toolStripZoomPanel.Size = new Size((int)(toolStripZoomPanel.Size.Width + (toolStripZoomPanel.Size.Width * (value - 1))), (int)(toolStripZoomPanel.Size.Height + (toolStripZoomPanel.Size.Height * (value - 1))));
                        m_Edit.Size = new Size((int)(m_Edit.Size.Width + (m_Edit.Size.Width * (value - 1))), (int)(m_Edit.Size.Height + (m_Edit.Size.Height * (value - 1))));
                        btntxtZoomSelected.Size = new Size((int)(btntxtZoomSelected.Size.Width + (btntxtZoomSelected.Size.Width * (value - 1))), (int)(btntxtZoomSelected.Size.Height + (btntxtZoomSelected.Size.Height * (value - 1))));
                    //    flowLayoutPanel1.Size = new Size((int)(flowLayoutPanel1.Size.Width + (flowLayoutPanel1.Size.Width * (value - 1))), (int)(flowLayoutPanel1.Size.Height + (flowLayoutPanel1.Size.Height * (value - 1))));
                    //    mbtnResetSelection.Size = new Size((int)(mbtnResetSelection.Size.Width + (mbtnResetSelection.Size.Width * (value - 1))), (int)(mbtnResetSelection.Size.Height + (mbtnResetSelection.Size.Height * (value - 1))));

                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size + (float)3.0), FontStyle.Bold);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size + (float)3.0), FontStyle.Bold);
                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                       // mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size + (float)3.0), FontStyle.Bold);
                     //   mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size + (float)3.0), FontStyle.Bold);
                        toolStripZoomPanel.Location = new Point(0, this.Height);
                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height - 4);
                        m_Edit.Location = new Point(5, this.Height - m_Edit.Height - btntxtZoomSelected.Height-8);


                        flag = true;
                    }
                }
                if (value <= 1.1)
                {                
                    
                   

                    toolStripZoomPanel.Size = new Size(toolStripZoomPanel.Width, toolStripZoomPanel.Height);
                    m_Edit.Size = new Size(m_Edit.Width, m_Edit.Height);
                    btntxtZoomSelected.Size = new Size(btntxtZoomSelected.Width, btntxtZoomSelected.Height);
                //    flowLayoutPanel1.Size = new Size(m_flowLayoutPanelSize.Width, m_flowLayoutPanelSize.Height);
                 //   mbtnResetSelection.Size = new Size(m_btnResetSelectionSize.Width, m_btnResetSelectionSize.Height);
                    if (flag)
                    {
                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size - (float)3.0), FontStyle.Regular);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size - (float)3.0), FontStyle.Regular);
                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size - (float)3.0), FontStyle.Regular);
                     //   mbtnZoomSelection.Font = new Font(mbtnZoomSelection.Font.Name, (mbtnZoomSelection.Font.Size - (float)3.0), FontStyle.Regular);
                    //    mbtnResetSelection.Font = new Font(mbtnResetSelection.Font.Name, (mbtnResetSelection.Font.Size - (float)3.0), FontStyle.Regular);
                        toolStripZoomPanel.Location = new Point(0, this.Height - toolStripZoomPanel.Height - 2);
                        m_Edit.Location = new Point(5, this.Height - m_Edit.Height - toolStripZoomPanel.Height - btntxtZoomSelected.Height - 8);
                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height - toolStripZoomPanel.Height - 4);


                    }
                    flag = false;
                    
                }
            }
        }

       
          public void IsNewProjectOpened()
        {
            m_buttonSizeinit = false;
            m_ContentView.RemovePanel();

            m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
        }

          private void btnClosetoolStrip_Click(object sender, EventArgs e)
          {
              m_buttonSizeinit = false;
              m_ContentView.RemovePanel();

              m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
          }

          private void btnNextPhrasetoolStrip_Click(object sender, EventArgs e)
          {
              if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
              {
                  this.AutoScroll = false;
              }
              else
              {
                  this.AutoScroll = true;
              }

              ObiNode nextNode = m_Node.FollowingNode;
              if (nextNode != null && nextNode.Parent != null && m_AudioBlock != null && m_AudioBlock.Node != null && m_AudioBlock.Node.Parent != null)
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
                      //    btntxtZoomSelected.Text = " ";
                      else if (m_Node.FollowingNode is EmptyNode)
                      {
                          m_Node = nextNode as EmptyNode;                         
                          m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                      }
                  }
              }
              else if (m_Node.FollowingNode is EmptyNode)
              {
                  if (m_AudioBlock != null || nextNode.Parent!=null)
                  {
                      m_Node = nextNode as EmptyNode;                     
                      m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                  }
              }
          }

          private void btnPreviousPhrasetoolStrip_Click(object sender, EventArgs e)
          {
              if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
              {
                  this.AutoScroll = false;
              }
              else
              {
                  this.AutoScroll = true;
              }
              ObiNode previousNode = m_Node.PrecedingNode;
              if (previousNode != null && previousNode.Parent != null && m_AudioBlock != null && m_AudioBlock.Node != null && m_AudioBlock.Node.Parent != null)
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
                      //btntxtZoomSelected.Text = " ";
                      else if (m_Node.PrecedingNode is EmptyNode)
                      {
                         // if (m_AudioBlock != null)
                          {
                              m_Node = previousNode as EmptyNode;
                              m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                          }
                      }

                  }

              }
              else if (m_Node.PrecedingNode is EmptyNode)
              {
                  m_Node = previousNode as EmptyNode;
                  m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
              }
          }

          private void btnZoomIntoolStrip_Click(object sender, EventArgs e)
          {
              if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
              {
                  this.AutoScroll = false;
              }
              else
              {
                  this.AutoScroll = true;
              }
              if (m_Node is PhraseNode)
              {
                  int tempWidth = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
                  if (tempWidth < (initialWaveformWidth * 60))
                  {
                      m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
                      m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                      m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                      m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                  }
              }

          }

          private void btnZoomOuttoolStrip_Click(object sender, EventArgs e)
          {
              if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
              {
                  this.AutoScroll = false;
              }
              else
              {
                  this.AutoScroll = true;
              }
              if (m_Node is PhraseNode)
              {
                  int tempWidth = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
                  if (tempWidth > (initialWaveformWidth / 10))
                  {
                      m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
                      m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                      m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                      m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                  }
              }
            
          }

          private void btnResettoolStrip_Click(object sender, EventArgs e)
          {
              if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight && m_ResizeIsDone == false)
              {
                  this.AutoScroll = false;
              }
              else
              {
                  this.AutoScroll = true;
              }
              if (m_Node is PhraseNode)
              {
                  m_AudioBlock.Waveform.Width = initialWaveformWidth;
                  m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                  m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                  m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                  m_AudioBlock.ResetTimeBoundsForWaveformDisplay();
              }

          }

          private void mbtnZoomSelectiontoolStrip_Click(object sender, EventArgs e)
          {
              if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null && m_ProjectView.Selection is Obi.AudioSelection)
              {
                  audioSel = (AudioSelection)m_ProjectView.Selection;
                  if (audioSel.AudioRange.SelectionBeginTime != audioSel.AudioRange.SelectionEndTime)
                  {
                      m_AudioBlock.SetTimeBoundsForWaveformDisplay(audioSel.AudioRange.SelectionBeginTime, audioSel.AudioRange.SelectionEndTime);
                      //    mbtnResetSelection.Enabled = true;
                  }
              }
          }
   

       // public Panel Panel_WaveForm { get { return panel_ZoomWaveform; } }
    }
}
