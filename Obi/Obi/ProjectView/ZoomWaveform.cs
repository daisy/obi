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
        private Size m_PanelSize;
        private bool m_ResizeIsDone = false;
        private bool m_buttonSizeinit = false;
        private bool flag = false;
        private AudioSelection audioSel;
        private int m_PreviousHeight = 0;
        private bool m_ZoomfactorFlag = false;
        private int XVal = 0;
    

        private KeyboardShortcuts_Settings keyboardShortcuts;

       
       
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
            //btntxtZoomSelected.Focus();
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
             XVal = m_AudioBlock.UpdateCursorTime(time);             

            // if ((XVal >= m_ContentView.Width + Math.Abs(m_AudioBlock.Location.X)) || (XVal < m_ContentView.Width && m_AudioBlock.Location.X < 0) || (XVal < Math.Abs(m_AudioBlock.Location.X)))
             if ((XVal >= m_ContentView.Width + Math.Abs(m_AudioBlock.Location.X)) || (XVal < Math.Abs(m_AudioBlock.Location.X)))
             {
                 panelZooomWaveform.AutoScrollPosition = new Point(XVal, panelZooomWaveform.AutoScrollPosition.Y);
                 Console.WriteLine("Xval Value {0}", XVal);
                 Console.WriteLine("Content view width {0}", m_ContentView.Width);
                 Console.WriteLine("Location.X value {0}", m_AudioBlock.Location.X);
             }
            
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
            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {
                btntxtZoomSelected.Text = " ";
                btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                btntxtZoomSelected.Text += " " + (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : "");
               
                
                if (m_ProjectView.Selection.Phrase != null)
                {

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
            else if(m_ContentView.ZoomFactor<=1)
            {
                this.Height = m_ContentView.Height;
            }
            if (m_ZoomfactorFlag == false || m_ContentView.ZoomFactor > 1.1)
            {
                this.AutoScroll = true;
          
               
            }
            else if (m_ContentView.ZoomFactor <= 1.1 && m_ZoomfactorFlag == true && (m_ProjectView.Height >= m_PreviousHeight))
            {
                this.AutoScroll = false;
                m_ZoomfactorFlag = false;
                
            }
       
            
            m_ResizeIsDone = true;
            this.Width = m_ContentView.Width;
            btntxtZoomSelected.Width = this.Width - 40;            
         
        }
         public void ZoomAudioFocus()
         {
          
             if (this.ActiveControl == btntxtZoomSelected)
             {
                m_AudioBlock.TabStop = false;
             }
             if(m_AudioBlock.FlagMouseDown)
             {
                 m_AudioBlock.FlagMouseDown = false;
                 this.ActiveControl = btntxtZoomSelected;
             }
          
         }
        public void PhraseLoad(EmptyNode phrase)
        {
          
            m_Node = phrase;
            if (m_Node is PhraseNode)
            {
                if (panelZooomWaveform.Controls.Contains(m_Block))
                {
                    panelZooomWaveform.Controls.Remove(m_Block);
                    m_Block.Dispose();
                    m_Block = null;
                }
               m_Block = new AudioBlock((PhraseNode)m_Node, m_Strip,true, true);
               m_AudioBlock = (AudioBlock)m_Block;              

                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock.Location = new Point(5, 5);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                m_AudioBlock.IsFineNavigationMode = true;
                m_AudioBlock.UpdateColors();
                btntxtZoomSelected.BringToFront();
                m_ZoomFactor = zoomFactor;
              
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                if (m_AudioBlock != null)
                {
                    
                    m_AudioBlock.GotFocus += new EventHandler(m_AudioBlock_MouseDown);
                }
                m_AudioBlock.TabStop = false;

                m_AudioBlock.InitCursor(0);
                if (m_ProjectView.TransportBar.IsPlayerActive) UpdateCursorTime(m_ProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
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
        private string FormatKeyboardShorcut(string str)
        {
            string[] tempStore = str.Split(',');
            //return new string( charArray );
            str = tempStore[1] + "+" + tempStore[0];
            return str;
        }
       public ZoomWaveform(ContentView contentView, Strip strip,EmptyNode node,ProjectView mProjectView ):this    ()
        {
            
            m_ContentView = contentView;
            m_ProjectView = mProjectView;
            m_ProjectView.SelectionChanged += new EventHandler(ProjectViewSelectionChanged);
            this.LostFocus += new EventHandler(ZoomPanelLostFocus);          

            keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
  
            m_ContentView.Resize += new EventHandler(ZoomPanelResize);
            m_Strip = strip;
            m_Node = node;

            this.btnClosetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Close") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString()) + ")";
            this.btnNextPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowNextPhrase")+"(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString()) + ")";
            this.btnPreviousPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString()) + ")";
            this.btnResettoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Reset") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString()) + ")";
            this.btnZoomIntoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomIn") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString()) + ")";
            this.btnZoomOuttoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomOut") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString()) + ")";
            this.btnZoomSelectiontoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomSelection") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString()) + ")";

            this.toolStripZoomPanel.AccessibleName = Localizer.Message("ZoomAudioTT_Close") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString()) + ")";
            this.btnNextPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowNextPhrase") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString()) + ")";
            this.btnPreviousPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString()) + ")";
            this.btnResettoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_Reset") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString()) + ")";
            this.btnZoomIntoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomIn") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString()) + ")";
            this.btnZoomOuttoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomOut") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString()) + ")";
            this.btnZoomSelectiontoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomSelection") + "(" + FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString()) + ")";


            if (m_ProjectView.Selection.Phrase != null || m_Node is EmptyNode)
            {
                if (m_ContentView != null)
                {                    
                    this.Height = m_ContentView.Height;
                    this.Width = m_ContentView.Width;

                    m_Edit = new Toolbar_EditAudio(m_ContentView, m_Strip, m_Node, m_ProjectView);
                    m_Edit.TabIndex = 0;
                    m_Edit.TabStop = true;
                
                    this.Controls.Add(m_Edit);
                    m_Edit.Show();
                    m_Edit.EnableDisableCut();
                    toolStripZoomPanel.TabIndex = 1;
                    toolStripZoomPanel.TabStop = true;
                    btntxtZoomSelected.TabIndex = 2;
                    btntxtZoomSelected.TabStop = true;
                    

                    this.MouseWheel += new MouseEventHandler(ZoomWaveform_MouseWheel);
                    if (m_buttonSizeinit == false)
                    {

                        m_toolStripSize = toolStripZoomPanel.Size;
                        m_EditSize = m_Edit.Size;
                        m_btntxtZoomSelectedSize = btntxtZoomSelected.Size;
                        m_PanelSize = this.Size;

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


                        panelZooomWaveform.Height = this.Height - (toolStripZoomPanel.Height + btntxtZoomSelected.Height + m_Edit.Height + 15);
                        btntxtZoomSelected.Size = new Size((int)(btntxtZoomSelected.Size.Width + (btntxtZoomSelected.Size.Width * (tempZoomfactor - 1))), (int)(btntxtZoomSelected.Size.Height + (btntxtZoomSelected.Size.Height * (tempZoomfactor - 1))));
                        toolStripZoomPanel.Size = new Size((int)(toolStripZoomPanel.Size.Width + (toolStripZoomPanel.Size.Width * (tempZoomfactor - 1))), (int)(toolStripZoomPanel.Size.Height + (toolStripZoomPanel.Size.Height * (tempZoomfactor - 1))));
                    
                        m_Edit.Size = new Size((int)(m_Edit.Size.Width + (m_Edit.Size.Width * (tempZoomfactor - 1.4))), (int)(m_Edit.Size.Height));
                

                        
                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size + (float)3.0), FontStyle.Bold);
                        m_Edit.SetEditPanelFontSize(m_Edit.Size);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size + (float)3.0), FontStyle.Bold);                        

                        flag = true;
                        btntxtZoomSelected.Location = new Point(0, this.Height);
                        toolStripZoomPanel.Location = new Point(0, this.Height - btntxtZoomSelected.Height - 4);
                        m_Edit.Location = new Point(0, this.Height - m_Edit.Height - btntxtZoomSelected.Height);

                        this.AutoScrollMinSize = new Size(this.Width, this.Height + 35);
                    }
                    else
                    {

                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height - 2);
                        toolStripZoomPanel.Location = new Point(0, this.Height - btntxtZoomSelected.Height - toolStripZoomPanel.Height - 4);
                        m_Edit.Location = new Point(0, this.Height - m_Edit.Height - toolStripZoomPanel.Height - btntxtZoomSelected.Height - 8);
                        panelZooomWaveform.Height = this.Height - (toolStripZoomPanel.Height + btntxtZoomSelected.Height + m_Edit.Height + 15);
                        this.AutoScrollMinSize = new Size(this.Width, this.Height + 15);
                    }
                   
                             
                    panelZooomWaveform.Width = this.Width - 30;
                    panelZooomWaveform.Location = new Point(0, 0);
                   
                m_Edit.BringToFront();


                    btntxtZoomSelected.Width = this.Width - 40;
             
                }
            }

            if (m_Node is PhraseNode)
            {
               
                m_Block = new AudioBlock((PhraseNode)m_Node, m_Strip, true,true);
                m_AudioBlock = (AudioBlock)m_Block;
                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock.Location = new Point(5,5);
                float zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                m_AudioBlock.IsFineNavigationMode = true;
                m_AudioBlock.UpdateColors();
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


                m_AudioBlock.InitCursor(0);
                if (m_ProjectView.TransportBar.IsPlayerActive) UpdateCursorTime(m_ProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
                btntxtZoomSelected.Focus();
                //m_AudioBlock.Focus();
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



            btntxtZoomSelected.Text = " ";
            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {
                btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                btntxtZoomSelected.Text += " " + (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : "");
            }
            m_PreviousHeight = this.Height;
            if (toolStripZoomPanel.Width < m_ContentView.Width && (m_Edit.Width + 5) < m_ContentView.Width && m_ProjectView.Height > m_PreviousHeight )
            {
                this.AutoScroll = false;
            }
            else
            {
                this.AutoScroll = true;
            }
            this.Load += new EventHandler(ZoomWaveform_Load);
            //btntxtZoomSelected.Focus ();
            //Console.WriteLine("constructor " + (m_ProjectView.Selection is AudioSelection? "audio selection": "") +  m_ProjectView.Selection);
        }

        void ZoomWaveform_Load(object sender, EventArgs e)
        {
            btntxtZoomSelected.Focus();
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
     
     string g=keyboardShortcuts.ZoomPanel_Close.Value.ToString();
     string p=g;
            //this.Focus();
             if (keyData == keyboardShortcuts.ContentView_TransportBarRecordSingleKey.Value)
             {
                 return true;
             }
             if (keyData == keyboardShortcuts.ContentView_ZoomWaveformPanel.Value)
             {
                 return true;
             }
             if (keyData == keyboardShortcuts.ZoomPanel_Close.Value) 
             {
                 Close();
                 return true;
             }
             else if(keyData ==keyboardShortcuts.ZoomPanel_NextPhrase.Value )
             {
                 NextPhrase();
                 return true;
             }
             else if (keyData == keyboardShortcuts.ZoomPanel_PreviousPhrase.Value)
             {
                 PreviousPhrase();
                 return true;
             }
             else if (keyData == keyboardShortcuts.ZoomPanel_Reset.Value)
             {
                 Reset();
                 return true;
             }
             else if (keyData == keyboardShortcuts.ZoomPanel_ZoomIn.Value)
             {
                 ZoomIn();
                 return true;
             }
             else if (keyData == keyboardShortcuts.ZoomPanel_ZoomOut.Value)
             {
                 ZoomOut();
                 return true;
             }
             else if (keyData == keyboardShortcuts.ZoomPanel_ZoomSelection.Value)
             {
                 ZoomSelection();
                 return true;
             }
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
            else if (keyData == keyboardShortcuts.ContentView_ScrollDown_SmallIncrementWithSelection.Value )
            {
                m_ContentView.NudgeIntervalIncrement(false);
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ScrollUp_SmallIncrementWithSelection.Value )
            {
                m_ContentView.NudgeIntervalIncrement(true);
                return true;
            }
            else if (keyData == Keys.Tab
        && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, true, true, false, true);
                //Console.WriteLine(c.ToString());
                //Console.WriteLine(m_ProjectView.Selection);
                if (this.ActiveControl != null && c.TabIndex > this.ActiveControl.TabIndex)
                    System.Media.SystemSounds.Beep.Play();
            
                return true;
            }
            else if (keyData == (Keys)(Keys.Shift | Keys.Tab)
                 && this.ActiveControl != null)
            {
                Control c = this.ActiveControl;
                this.SelectNextControl(c, false, true, false, true);
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
        protected override void OnPaint(PaintEventArgs e)
        {
            this.BringToFront();
            base.OnPaint(e);
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
                      
                        m_Edit.Size = new Size((int)(m_Edit.Size.Width + (m_Edit.Size.Width * (value - 1))), (int)(m_Edit.Size.Height));
                        btntxtZoomSelected.Size = new Size((int)(btntxtZoomSelected.Size.Width + (btntxtZoomSelected.Size.Width * (value - 1))), (int)(btntxtZoomSelected.Size.Height + (btntxtZoomSelected.Size.Height * (value - 1))));
          
                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size + (float)3.0), FontStyle.Bold);
                        m_Edit.SetEditPanelFontSize(m_Edit.Size);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size + (float)3.0), FontStyle.Bold);
                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size + (float)3.0), FontStyle.Bold);
                  



                        btntxtZoomSelected.Location = new Point(0, this.Height-this.btntxtZoomSelected.Height);
                        
                        toolStripZoomPanel.Location = new Point(0, this.Height - btntxtZoomSelected.Height-toolStripZoomPanel.Height - 15);
                        m_Edit.Location = new Point(0, this.Height - m_Edit.Height - btntxtZoomSelected.Height-toolStripZoomPanel.Height-15);


                        m_ZoomfactorFlag = true;
                        flag = true;
                    }
                }
                if (value <= 1.1)
                {



                    toolStripZoomPanel.Size = m_toolStripSize;
                    m_Edit.Size = m_EditSize;
                    btntxtZoomSelected.Size = m_btntxtZoomSelectedSize;
                    

                   

                    if (flag)
                    {
                        toolStripZoomPanel.Font = new Font(toolStripZoomPanel.Font.Name, (toolStripZoomPanel.Font.Size - (float)3.0), FontStyle.Regular);
                        m_Edit.MinimumSize = m_EditSize;
                        m_Edit.Size = m_EditSize;
                        m_Edit.SetEditPanelFontSize(m_Edit.Size);
                        m_Edit.Font = new Font(m_Edit.Font.Name, (m_Edit.Font.Size - (float)3.0), FontStyle.Regular);
                        btntxtZoomSelected.Font = new Font(btntxtZoomSelected.Font.Name, (btntxtZoomSelected.Font.Size - (float)3.0), FontStyle.Regular);


                        btntxtZoomSelected.Location = new Point(0, this.Height - btntxtZoomSelected.Height);
                        toolStripZoomPanel.Location = new Point(0, this.Height - btntxtZoomSelected.Height - toolStripZoomPanel.Height - 15);
                        m_Edit.Location = new Point(0, this.Height - m_Edit.Height - toolStripZoomPanel.Height - btntxtZoomSelected.Height - 15);
                        

                        flag = false;

                    }
                    
                   
                    
                }
            }
        }

       
          public void IsNewProjectOpened()
        {
            m_buttonSizeinit = false;
            m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
            m_ContentView.RemovePanel();

            
           
        }

          private void btnClosetoolStrip_Click(object sender, EventArgs e)
          {
              Close();
          }
          private void Close()
          {
                m_buttonSizeinit = false;
                m_ContentView.RemovePanel();

                m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);               
                
                
                
           }
       

          private void btnNextPhrasetoolStrip_Click(object sender, EventArgs e)
          {
              NextPhrase();
          }
        private void NextPhrase()
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
            if (nextNode != null && nextNode.Parent != null && m_Block != null && m_Block.Node != null && m_Block.Node.Parent != null)
            {
                if (m_Block.Node.Parent == nextNode.Parent)
                {
                    if (m_Node.FollowingNode is PhraseNode)
                    {
                        m_Node = nextNode as PhraseNode;
                        m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);

                    }

                    else if (m_Node.FollowingNode is EmptyNode)
                    {
                        m_Node = nextNode as EmptyNode;
                        m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                    }
                }
            }
            else if (m_Node.FollowingNode is EmptyNode)
            {
              if (m_AudioBlock != null)
                {
                    m_Node = nextNode as EmptyNode;
                    m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                }
            }
        }

          private void btnPreviousPhrasetoolStrip_Click(object sender, EventArgs e)
          {
              PreviousPhrase();
          }
        private void PreviousPhrase()
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
            if (previousNode != null && previousNode.Parent != null && m_Block != null && m_Block.Node != null && m_Block.Node.Parent != null)
            {
                if (m_Block.Node.Parent == previousNode.Parent)
                {
                    if (m_Node.PrecedingNode is PhraseNode)
                    {
                        m_Node = previousNode as PhraseNode;
                        m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
             
                    }
    
                    else if (m_Node.PrecedingNode is EmptyNode)
                    {

                        {
                            m_Node = previousNode as EmptyNode;
                            m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                        }
                    }

                }

            }
            else if (m_Node.PrecedingNode is EmptyNode)
            {
                if (m_AudioBlock != null)
                {
                    m_Node = previousNode as EmptyNode;
                    m_ProjectView.Selection = new NodeSelection(m_Node, m_ContentView);
                }
            }
        }

          private void btnZoomIntoolStrip_Click(object sender, EventArgs e)
          {
              ZoomIn();
          }
        private void ZoomIn()
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
              ZoomOut();        
          }
        private void ZoomOut()
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
              Reset();
          }

        private void Reset()
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
              ZoomSelection();
          }
        private void ZoomSelection()
        {
            if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null && m_ProjectView.Selection is Obi.AudioSelection)
            {
                audioSel = (AudioSelection)m_ProjectView.Selection;
                if (audioSel.AudioRange.SelectionBeginTime != audioSel.AudioRange.SelectionEndTime)
                {
                    m_AudioBlock.SetTimeBoundsForWaveformDisplay(audioSel.AudioRange.SelectionBeginTime, audioSel.AudioRange.SelectionEndTime);
                  
                }
            }
        }

      
    }
}
