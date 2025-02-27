﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Reflection;
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
        private AudioSelection m_audioSel;
        private int m_PreviousHeight = 0;
        private bool m_ZoomfactorFlag = false;
        private int XVal = 0;
        private bool m_NudgeAtRight=false;
        private bool m_NudgeAtLeft = false;
        private bool m_NudgeAtRightFromLeft = false;
        private bool m_NudgeAtLeftFromLeft = false;
        private int m_InitialPanelHeight = 0;
        private double m_PhraseDuration = 0;
        private double m_ZoomIncrementFactor = 0;
        private string m_SelectedPhraseSection = " "; // @ImproveZoomPanel
        private Image m_PreserveZoomCheckedImage;
        private Image m_PreserveZoomUnCheckedImage;
        private EditableLabel m_EditableLabel;
        private EmptyNode m_SelectedNodeToAddComment; // @Comment-todo
           

        private KeyboardShortcuts_Settings keyboardShortcuts;

       
       
        private ZoomWaveform()
        {            
            InitializeComponent();
            this.Controls.Add(panelZooomWaveform);       
            this.Controls.Add(btntxtZoomSelected);
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream preserveZoomChecked = null;
            preserveZoomChecked = myAssembly.GetManifestResourceStream("Obi.images.icons.Preserve_Zoom_Checked.png");
            if(preserveZoomChecked != null)
            m_PreserveZoomCheckedImage = Image.FromStream(preserveZoomChecked);
            preserveZoomChecked = myAssembly.GetManifestResourceStream("Obi.images.icons.Preserve_Zoom_Unchecked.png");
            if (preserveZoomChecked != null)
            m_PreserveZoomUnCheckedImage = Image.FromStream(preserveZoomChecked);
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/Obi Views and Transport Bar/Zoomed Waveform view.htm");           
        }

        public void SetSelectionFromContentView(NodeSelection selection) 
        {
            if (m_AudioBlock == null 
                || (selection != null && selection.Node is EmptyNode &&  m_AudioBlock.Node != selection.Node))
            {
                PhraseLoad((EmptyNode) selection.Node, true);
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
        public bool IsCommentEditLabelActive  // @Comment-todo
        {
            get
            {
                return this.Controls.Contains(m_EditableLabel);
            }
        }
        public string ToMatch() { return null; }

        public void UpdateCursorTime (double time ) 
        {
            if (m_AudioBlock != null)
            {
                if (m_audioSel!=null && m_audioSel.AudioRange.SelectionBeginTime != m_audioSel.AudioRange.SelectionEndTime)
                {
                    if (time < m_audioSel.AudioRange.SelectionBeginTime)
                    {
                        m_AudioBlock.UpdateCursorTime(m_audioSel.Node.Duration);
                        return;
                    }               
                }
                XVal = m_AudioBlock.UpdateCursorTime(time); 

                if (m_ProjectView.TransportBar.CurrentPlaylist.CurrentPhrase != m_ProjectView.Selection.Node)
                {
                    m_ProjectView.TransportBar.Stop();
                }
                if ((XVal >= m_ContentView.Width-50 + Math.Abs(m_AudioBlock.Location.X)) || (XVal < Math.Abs(m_AudioBlock.Location.X)))
                {
                    panelZooomWaveform.AutoScrollPosition = new Point(XVal, panelZooomWaveform.AutoScrollPosition.Y);

                }
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
                btntxtZoomSelected.Text = " " + m_ProjectView.Selection.ToString() + " " + m_SelectedPhraseSection;  // @ImproveZoomPanel
                //btntxtZoomSelected.Text = " ";
                //btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                //btntxtZoomSelected.Text += " " + (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : "");

                if (this.Controls.Contains(m_EditableLabel)) // @Comment-todo
                {
                    this.RemoveEditLabelControlForAddingComment();
                }
                if (m_ProjectView.Selection.Phrase != null)
                {

                    string temp = m_ProjectView.Selection.Node.ToString();

                    if (m_AudioBlock.Node.ToString() != temp && m_ProjectView.Selection.EmptyNodeForSelection != null)
                    {
                        PhraseLoad(m_ProjectView.Selection.EmptyNodeForSelection, true);
                    }
                    else
                    {
                        if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection && m_NudgeAtRight)
                        {
                            double endTime = ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionEndTime != 0 ? ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionEndTime : ((AudioSelection)m_ProjectView.Selection).AudioRange.CursorTime;
                            int Val = m_AudioBlock.UpdateSelectionTime(endTime);
                            if ((Val >= m_ContentView.Width + Math.Abs(m_AudioBlock.Location.X)) || (Val < Math.Abs(m_AudioBlock.Location.X)))
                            {
                                panelZooomWaveform.AutoScrollPosition = new Point(Val, panelZooomWaveform.AutoScrollPosition.Y);
                            }
                        }
                        if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection && m_NudgeAtLeft)
                        {
                            double endTime = ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionEndTime != 0 ? ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionEndTime : ((AudioSelection)m_ProjectView.Selection).AudioRange.CursorTime;
                            int Val = m_AudioBlock.UpdateSelectionTime(endTime);
                            int tempVar = m_ContentView.Width - Val + Math.Abs(m_AudioBlock.Location.X);
                            if ((tempVar < 0) || (Val < Math.Abs(m_AudioBlock.Location.X)))
                            {
                                panelZooomWaveform.AutoScrollPosition = new Point(Val - m_ContentView.Width, panelZooomWaveform.AutoScrollPosition.Y);
                            }
                        }
                        if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection && m_NudgeAtRightFromLeft)
                        {
                            double endTime = ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionBeginTime;
                            int Val = m_AudioBlock.UpdateSelectionTime(endTime);
                            if ((Val >= m_ContentView.Width + Math.Abs(m_AudioBlock.Location.X)) || (Val < Math.Abs(m_AudioBlock.Location.X)))
                            {
                                panelZooomWaveform.AutoScrollPosition = new Point(Val, panelZooomWaveform.AutoScrollPosition.Y);
                            }
                        }
                        if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection && m_NudgeAtLeftFromLeft)
                        {

                            double endTime = ((AudioSelection)m_ProjectView.Selection).AudioRange.SelectionBeginTime;
                            int Val = m_AudioBlock.UpdateSelectionTime(endTime);
                            if ((Val >= m_ContentView.Width + Math.Abs(m_AudioBlock.Location.X)) || (Val < Math.Abs(m_AudioBlock.Location.X)))
                            {
                                panelZooomWaveform.AutoScrollPosition = new Point(Val, panelZooomWaveform.AutoScrollPosition.Y);
                            }

                        }
                    }

                }

                if (m_Node.CommentText != null && !(m_Node is PhraseNode) && m_Node is EmptyNode) // @Comment-todo
                {
                    m_Block.AlignLabelToShowCommentIcon();
                }

            }
            if( m_ProjectView==null || m_ProjectView.Selection==null ||!(m_ProjectView.Selection.Node is EmptyNode))
            {
                IsNewProjectOpened();
            }
        }

        private void ProjectviewUpdated(object sender, EventArgs e)
        {
            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {
                btntxtZoomSelected.Text = " " + m_ProjectView.Selection.ToString() + " " + m_SelectedPhraseSection;  // @ImproveZoomPanel
                //btntxtZoomSelected.Text = " ";
                //btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                //btntxtZoomSelected.Text += " " + (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : "");
                if (m_ProjectView.Selection.Node.Duration != m_PhraseDuration)
                {
                    Point tempScrollPosition = panelZooomWaveform.AutoScrollPosition;
                    PhraseLoad((EmptyNode)m_ProjectView.Selection.Node, false);
                }
                if (m_AudioBlock != null && m_AudioBlock.Waveform != null && m_AudioBlock.Waveform.Selection != null)
                {
                    if (m_AudioBlock.Waveform.Selection.HasCursor)
                    {
                        UpdateCursorTime(m_AudioBlock.Waveform.Selection.CursorTime);
                    }
                    else
                    {
                        UpdateCursorTime(m_AudioBlock.Waveform.Selection.SelectionBeginTime);
                    }
                }

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
            if (m_AudioBlock != null && m_AudioBlock.Waveform != null)
            {
                // Below commented code can be used if we want to increase the hight of the Zoom panel in case of resizing.

                //panelZooomWaveform.Height = this.Height - (toolStripZoomPanel.Height + btntxtZoomSelected.Height + m_Edit.Height);
                //m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);
                //btntxtZoomSelected.Location = new Point(0, panelZooomWaveform.Height + btntxtZoomSelected.Height + m_Edit.Height);
                //int tempVerticalPosition = btntxtZoomSelected.Location.Y;
                //toolStripZoomPanel.Location = new Point(0, btntxtZoomSelected.Location.Y - (btntxtZoomSelected.Height)); //new Point(0, this.Height - btntxtZoomSelected.Height - 100);
                //m_Edit.Location = new Point(0, toolStripZoomPanel.Location.Y - m_Edit.Height);
               // m_AudioBlock.SetZoomFactorAndHeight(m_ZoomFactor, panelZooomWaveform.Height);

                // Below code increases width of the Audio Block.                
                panelZooomWaveform.Width = this.Width-30;                
             }
            btntxtZoomSelected.Width = this.Width - 40;
            if (m_InitialPanelHeight > this.Height)
            {
                this.AutoScroll = true;
            }
            else
            {
                this.AutoScroll = false;
            }
            EnsureCursorVisibility();
         
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
        public void AfterSelection()
        {

        }
        public void PhraseLoad(EmptyNode phrase,bool CreateNew)
        {
          
            m_Node = phrase;
            if (m_Node is PhraseNode)
            {
                float zoomFactor = 0.0f;
                if (CreateNew || m_AudioBlock.Waveform == null)
                {
                    if (panelZooomWaveform.Controls.Contains(m_Block))
                    {
                        panelZooomWaveform.Controls.Remove(m_Block);
                        m_Block.Dispose();
                        m_Block = null;
                    }
                    m_Block = new AudioBlock((PhraseNode)m_Node, m_Strip, true, true);
                    m_AudioBlock = (AudioBlock)m_Block;

                    panelZooomWaveform.Controls.Add(m_Block);

                    m_AudioBlock.Location = new Point(5, 5);
                    zoomFactor = panelZooomWaveform.Height / m_AudioBlock.Height;
                    m_AudioBlock.IsFineNavigationMode = true;
                    m_AudioBlock.UpdateColors();
                    btntxtZoomSelected.BringToFront();
                    m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                    initialWaveformWidth = m_AudioBlock.Waveform.Width;
                    zoomFactor = m_ZoomFactor;
                }
                else
                {
                    zoomFactor = m_ZoomFactor;
                   
                }
                m_AudioBlock.SetZoomFactorAndHeight(zoomFactor, Height);
                initialWaveformWidth = m_AudioBlock.Waveform.Width;
                m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * m_ZoomIncrementFactor);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);


                if (m_AudioBlock != null)
                {
                    
                    m_AudioBlock.GotFocus += new EventHandler(m_AudioBlock_MouseDown);
                }
                m_AudioBlock.TabStop = false;

                m_AudioBlock.InitCursor(0);
                if (m_ProjectView.TransportBar.IsPlayerActive) UpdateCursorTime(m_ProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
                m_Block = m_AudioBlock;
                m_PhraseDuration = m_AudioBlock.Node.Duration;
                
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
                m_PhraseDuration = 0;

            }

        }

       public ZoomWaveform(ContentView contentView, Strip strip,EmptyNode node,ProjectView mProjectView):this    ()
       {
           
           if (mProjectView.SaveZoomWaveformZoomLevel)
           {
               btnPreserveZoomtoolStrip.Image = m_PreserveZoomCheckedImage;
               btnPreserveZoomtoolStrip.AccessibleName = Localizer.Message("ZoomWaveform_PreserveZoomChecked");
           }
           else
           {
               btnPreserveZoomtoolStrip.Image = m_PreserveZoomUnCheckedImage;
               btnPreserveZoomtoolStrip.AccessibleName = Localizer.Message("ZoomWaveform_PreserveZoomUnchecked");
           }
           
            m_ContentView = contentView;
            m_ProjectView = mProjectView;
            m_ProjectView.SelectionChanged += new EventHandler(ProjectViewSelectionChanged);          
            mProjectView.Presentation.UndoRedoManager.CommandDone += new EventHandler<urakawa.events.undo.DoneEventArgs>(ProjectviewUpdated);
            mProjectView.Presentation.UndoRedoManager.CommandUnDone+= new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ProjectviewUpdated);
            mProjectView.Presentation.UndoRedoManager.CommandReDone += new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ProjectviewUpdated);
            this.LostFocus += new EventHandler(ZoomPanelLostFocus);          

            //keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
  
            m_ContentView.Resize += new EventHandler(ZoomPanelResize);
            m_Strip = strip;
            m_Node = node;
            ZoomPanelToolTipInit();
            if (m_ProjectView.SaveZoomWaveformZoomLevel)
            {
                m_ZoomIncrementFactor = m_ProjectView.ZoomWaveformIncrementFactor;
            }
            m_ZoomFactor = 0;
            if (m_ProjectView.ObiForm.Settings.ObiFont != this.Font.Name) //@fontconfig
            {
                SetFont(); //@fontconfig
            }

            //this.btnClosetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Close") + "(" +keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString()) + ")";
            //this.btnNextPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowNextPhrase")+"(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString()) + ")";
            //this.btnPreviousPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString()) + ")";
            //this.btnResettoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Reset") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString()) + ")";
            //this.btnZoomIntoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomIn") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString()) + ")";
            //this.btnZoomOuttoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomOut") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString()) + ")";
            //this.btnZoomSelectiontoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomSelection") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString()) + ")";

            //this.toolStripZoomPanel.AccessibleName = Localizer.Message("ZoomAudioTT_Close") +  keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString());
            //this.btnNextPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowNextPhrase") +  keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString());
            //this.btnPreviousPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") +  keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString());
            //this.btnResettoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_Reset") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString());
            //this.btnZoomIntoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomIn") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString());
            //this.btnZoomOuttoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomOut") +  keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString());
            //this.btnZoomSelectiontoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomSelection") +  keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString());


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

                    m_InitialPanelHeight = this.Height;         
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
                if (mProjectView.SaveZoomWaveformZoomLevel)
                {
                    m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * m_ZoomIncrementFactor);
                }
                if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection is AudioSelection)
                {
                    m_audioSel = (AudioSelection)m_ProjectView.Selection;
                    if (m_audioSel.AudioRange.SelectionBeginTime != m_audioSel.AudioRange.SelectionEndTime)
                    {
                        m_AudioBlock.SetTimeBoundsForWaveformDisplay(m_audioSel.AudioRange.SelectionBeginTime, m_audioSel.AudioRange.SelectionEndTime);
                    }
                }
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height-10);
                //int audioCursorPosition;
                //int selectionStartPosition;
                //int selectionEndPosition;
                //m_AudioBlock.GetLocationXForAudioCursorAndSelection(out audioCursorPosition, out selectionStartPosition, out selectionEndPosition);
                //Console.WriteLine("Audio Cursor Position is ------------------------------------------ {0}", audioCursorPosition);
                //Console.WriteLine("Selection Start  {0} and Selection End  {1}", selectionStartPosition, selectionEndPosition); 
                //string strTemp = m_audioSel.AudioRange.ToString();
                //int tempLength = strTemp.Length;
                //string strTEmpInt = strTemp.TrimEnd('s');
                //Console.WriteLine("Time of Selection --------------------{0}", strTEmpInt);
                //int numVal = Int32.Parse("strTEmpInt");
                if (m_audioSel != null && m_audioSel.AudioRange.HasCursor)
                {
                    m_AudioBlock.InitCursor(m_audioSel.AudioRange.CursorTime);

                   // UpdateCursorTime(m_audioSel.AudioRange.CursorTime);
                }
                else
                {
                    m_AudioBlock.InitCursor(0);
                }
                if (m_ProjectView.TransportBar.IsPlayerActive) UpdateCursorTime(m_ProjectView.TransportBar.CurrentPlaylist.CurrentTimeInAsset);
                btntxtZoomSelected.Focus();
                //m_AudioBlock.Focus();
                //this.ActiveControl = btnClosetoolStrip;
                m_Block = m_AudioBlock;
                m_PhraseDuration = m_AudioBlock.Node.Duration;
            }
            else if (m_Node is EmptyNode)
            {
               
                m_Block = new Block(m_Node, m_Strip);
                panelZooomWaveform.Controls.Add(m_Block);
                m_AudioBlock = null;
                m_PhraseDuration = 0;
                if (m_Node.CommentText != null)  // @Comment-todo
                m_Block.AlignLabelToShowCommentIcon();

            }

            m_count = 0;



            //btntxtZoomSelected.Text = " ";

            if (m_ProjectView != null && m_ProjectView.Selection != null)
            {

                m_SelectedPhraseSection = (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : ""); // @ImproveZoomPanel
                btntxtZoomSelected.Text = " " + m_ProjectView.Selection.ToString() + " " + m_SelectedPhraseSection;
                //btntxtZoomSelected.Text += " " + m_ProjectView.Selection.ToString();
                //btntxtZoomSelected.Text += " " + (m_ProjectView.GetSelectedPhraseSection != null ? m_ProjectView.GetSelectedPhraseSection.ToString() : "");
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
            EnsureCursorVisibility();
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
                   m_ZoomIncrementFactor -= 0.5;
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
                   m_ZoomIncrementFactor += 0.5;
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
            if (m_ProjectView.ObiForm.KeyboardShortcuts == null) return false;

            string g = keyboardShortcuts.ZoomPanel_Close.Value.ToString();
            string p = g;
            //this.Focus();
            if (this.Controls.Contains(m_EditableLabel))
            {
                return false;
            }
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
            else if (keyData == keyboardShortcuts.ZoomPanel_NextPhrase.Value)
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
            else if (keyData == keyboardShortcuts.ContentView_SelectPrecedingPhrase.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtLeft = true;
                m_ContentView.NudgeInFineNavigation(false);
                m_NudgeAtLeft = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_SelectFollowingPhrase.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtRight = true;
                m_ContentView.NudgeInFineNavigation(true);
                m_NudgeAtRight = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_TransportBarNudgeBackward.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtLeft = true;
                m_ContentView.NudgeInFineNavigation(false);
                m_NudgeAtLeft = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_TransportBarNudgeForward.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtRight = true;
                m_ContentView.NudgeInFineNavigation(true);
                m_NudgeAtRight = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ExpandAudioSelectionAtLeft.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtRightFromLeft = true;
                m_ProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ExpandAtLeft);
                m_NudgeAtRightFromLeft = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ContractAudioSelectionAtLeft.Value && this.ActiveControl != toolStripZoomPanel && this.ActiveControl != m_Edit)
            {
                m_NudgeAtLeftFromLeft = true;
                m_ProjectView.TransportBar.NudgeSelectedAudio(TransportBar.NudgeSelection.ContractAtLeft);
                m_NudgeAtLeftFromLeft = false;
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ScrollDown_SmallIncrementWithSelection.Value)
            {
                m_ContentView.NudgeIntervalIncrement(false);
                return true;
            }
            else if (keyData == keyboardShortcuts.ContentView_ScrollUp_SmallIncrementWithSelection.Value)
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
            Close();   
        }

          private void btnClosetoolStrip_Click(object sender, EventArgs e)
          {
              Close();
          }
          private void Close()
          {
                m_buttonSizeinit = false;
                
               if (m_ProjectView.SaveZoomWaveformZoomLevel)
                {
                    m_ProjectView.ZoomWaveformIncrementFactor = m_ZoomIncrementFactor;
                }
                else
                {
                    m_ProjectView.ZoomWaveformIncrementFactor = 0;
                }
                m_ContentView.RemovePanel();
                m_ProjectView.SelectionChanged -= new EventHandler(ProjectViewSelectionChanged);
                m_ProjectView.Presentation.UndoRedoManager.CommandDone -= new EventHandler<urakawa.events.undo.DoneEventArgs>(ProjectviewUpdated);
                m_ProjectView.Presentation.UndoRedoManager.CommandUnDone -= new EventHandler<urakawa.events.undo.UnDoneEventArgs>(ProjectviewUpdated);
                m_ProjectView.Presentation.UndoRedoManager.CommandReDone -= new EventHandler<urakawa.events.undo.ReDoneEventArgs>(ProjectviewUpdated);
                
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
            if (m_Node != null && m_Node.IsRooted==true && m_Node.Parent != null && m_Node.FollowingNode != null)
            {
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
            this.UpdateCursorTime(0);
            if (m_audioSel != null && m_audioSel.AudioRange != null)
            {
                m_audioSel.AudioRange.SelectionBeginTime = 0;
                m_audioSel.AudioRange.SelectionEndTime = 0;
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
            if (m_Node != null && m_Node.IsRooted == true && m_Node.Parent!=null && m_Node.PrecedingNode != null)
            {
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
            this.UpdateCursorTime(0);
            if (m_audioSel != null && m_audioSel.AudioRange != null)
            {
                m_audioSel.AudioRange.SelectionBeginTime = 0;
                m_audioSel.AudioRange.SelectionEndTime = 0;
            }
        }

        private void EnsureCursorVisibility()
        {
            if (m_ProjectView.Selection is AudioSelection)
            {
                AudioSelection ZoomAudioSelect = (AudioSelection)m_ProjectView.Selection;

                if (ZoomAudioSelect != null && ZoomAudioSelect.AudioRange != null && m_AudioBlock != null)
                {
                    this.AutoScroll = true;
                    if (ZoomAudioSelect.AudioRange.HasCursor)
                    {
                      //  XVal = m_AudioBlock.UpdateCursorTime(ZoomAudioSelect.AudioRange.CursorTime);
                        XVal = m_AudioBlock.UpdateSelectionTime(ZoomAudioSelect.AudioRange.CursorTime);
                    }
                    else
                    {
                        XVal = m_AudioBlock.UpdateSelectionTime(ZoomAudioSelect.AudioRange.SelectionBeginTime);
                    }
                    if ((XVal >= m_ContentView.Width - 50 + Math.Abs(m_AudioBlock.Location.X)) || (XVal < Math.Abs(m_AudioBlock.Location.X)))
                    {
                        m_ContentView.Selection = m_ProjectView.Selection;
                        panelZooomWaveform.AutoScrollPosition = new Point(XVal - 100, panelZooomWaveform.AutoScrollPosition.Y);

                    }
                }               
            }
        }
        private void btnZoomIntoolStrip_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }
        public void ZoomIn()
        {
            
            //Console.WriteLine("Zoom In");
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
                    m_ZoomIncrementFactor += 0.5;
                    m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width + (int)(initialWaveformWidth * 0.5);
                    m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                    m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                    m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                }
            }

            EnsureCursorVisibility();
        }

          private void btnZoomOuttoolStrip_Click(object sender, EventArgs e)
          {
              ZoomOut();        
          }
        public void ZoomOut()
        {
            
            //Console.WriteLine("zoom out");
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
                    m_ZoomIncrementFactor -= 0.5;
                    m_AudioBlock.Waveform.Width = m_AudioBlock.Waveform.Width - (int)(initialWaveformWidth * 0.5);
                    m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                    m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                    m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                }
            }
            EnsureCursorVisibility();
        }

          private void btnResettoolStrip_Click(object sender, EventArgs e)
          {
              Reset();
          }

        public void Reset()
        {
            
            //Console.WriteLine("zoom reset");
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
                m_ZoomIncrementFactor = 0;
                m_audioSel = null;
                m_AudioBlock.Waveform.Width = initialWaveformWidth;
                m_AudioBlock.SetZoomFactorAndHeightForZoom(m_ZoomFactor, Height);
                m_AudioBlock.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                m_AudioBlock.Waveform.Size = new Size(m_AudioBlock.Waveform.Width, panelZooomWaveform.Height - 10);
                m_AudioBlock.ResetTimeBoundsForWaveformDisplay();
            }
            EnsureCursorVisibility();
        }

        private void mbtnZoomSelectiontoolStrip_Click(object sender, EventArgs e)
        {
            ZoomSelection();
        }
        private void ZoomSelection()
        {
            if (m_ProjectView != null && m_ProjectView.Selection != null && m_ProjectView.Selection.Control != null && m_ProjectView.Selection is Obi.AudioSelection)
            {
                m_audioSel = (AudioSelection)m_ProjectView.Selection;
                if (m_audioSel.AudioRange.SelectionBeginTime != m_audioSel.AudioRange.SelectionEndTime)
                {
                    m_AudioBlock.SetTimeBoundsForWaveformDisplay(m_audioSel.AudioRange.SelectionBeginTime, m_audioSel.AudioRange.SelectionEndTime);
                  
                }
            }
        }
        public void ZoomPanelToolTipInit()
        {
            keyboardShortcuts = m_ProjectView.ObiForm.KeyboardShortcuts;
            this.btnClosetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Close") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString()) + ")";
            this.btnNextPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowNextPhrase") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString()) + ")";
            this.btnPreviousPhrasetoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString()) + ")";
            this.btnResettoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_Reset") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString()) + ")";
            this.btnZoomIntoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomIn") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString()) + ")";
            this.btnZoomOuttoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomOut") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString()) + ")";
            this.btnZoomSelectiontoolStrip.ToolTipText = Localizer.Message("ZoomAudioTT_ZoomSelection") + "(" + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString()) + ")";

            this.toolStripZoomPanel.AccessibleName = Localizer.Message("ZoomAudioTT_Close") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Close.Value.ToString());
            this.btnNextPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowNextPhrase") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_NextPhrase.Value.ToString());
            this.btnPreviousPhrasetoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ShowPreviousPhrase") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_PreviousPhrase.Value.ToString());
            this.btnResettoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_Reset") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_Reset.Value.ToString());
            this.btnZoomIntoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomIn") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomIn.Value.ToString());
            this.btnZoomOuttoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomOut") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomOut.Value.ToString());
            this.btnZoomSelectiontoolStrip.AccessibleName = Localizer.Message("ZoomAudioTT_ZoomSelection") + keyboardShortcuts.FormatKeyboardShorcut(keyboardShortcuts.ZoomPanel_ZoomSelection.Value.ToString());
            if (m_Edit != null)
            {
                m_Edit.EditAudioPanelToolTipInit();
            }
        }
        private void SetFont() //@fontconfig
        {
            mtoolTipZoomWaveform.OwnerDraw = true;
            this.Font = new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
            toolStripZoomPanel.Font = new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);
        }

        private void mtoolTipZoomWaveform_Draw(object sender, DrawToolTipEventArgs e)
        {
             Font tooltipFont = new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size);
           // Font tooltipFont = new Font((m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size));
            e.Graphics.Clear(Color.White);
            e.Graphics.DrawString(e.ToolTipText, tooltipFont, Brushes.Black, new PointF(0, 0));
        }

        private void mtoolTipZoomWaveform_Popup(object sender, PopupEventArgs e)
        {
            e.ToolTipSize = TextRenderer.MeasureText(mtoolTipZoomWaveform.GetToolTip(e.AssociatedControl), new Font(m_ProjectView.ObiForm.Settings.ObiFont, this.Font.Size));
        }

        private void btnPreserveZoomtoolStrip_Click(object sender, EventArgs e)
        {
            m_ProjectView.SaveZoomWaveformZoomLevel = !m_ProjectView.SaveZoomWaveformZoomLevel;
            if (m_ProjectView.SaveZoomWaveformZoomLevel)
            {
                m_ProjectView.ZoomWaveformIncrementFactor = m_ZoomIncrementFactor;
                btnPreserveZoomtoolStrip.Image = m_PreserveZoomCheckedImage;
                btnPreserveZoomtoolStrip.AccessibleName = Localizer.Message("ZoomWaveform_PreserveZoomChecked");
                //m_ZoomFactor = m_ZoomIncrementFactor
            }
            else
            {
                btnPreserveZoomtoolStrip.Image = m_PreserveZoomUnCheckedImage;
                btnPreserveZoomtoolStrip.AccessibleName = Localizer.Message("ZoomWaveform_PreserveZoomUnchecked");
            }
        }

        public void ShowEditLabelToAddComment() // @Comment-todo
        {
            EditableLabel editLabel = new EditableLabel(m_ContentView.Selection.Node as EmptyNode);
            this.Controls.Add(editLabel);
            m_EditableLabel = editLabel;
            m_SelectedNodeToAddComment = m_ContentView.Selection.Node as EmptyNode;
            editLabel.Show();
            editLabel.Editable = true;
            AlignCommentEditLabel();
            editLabel.BringToFront();
            editLabel.AddComment += new EventHandler(EditableLabel_AddComment);
            editLabel.CloseComment += new EventHandler(EditLabel_CloseAddComment);
        }
        private void EditLabel_CloseAddComment(object sender, EventArgs e) // @Comment-todo
        {
            RemoveEditLabelControlForAddingComment();
        }

        private void EditableLabel_AddComment(object sender, EventArgs e) // @Comment-todo
        {
            Block tempBlock = m_Block;
            if (tempBlock != null)
            {
                m_ContentView.MarkTODOAndAddComment(m_EditableLabel.CommentText);
                m_Block.UpdateLabelsText();
                m_Block.AlignLabelToShowCommentIcon();
                m_ContentView.ActiveStrip.AlignLabelToShowCommentIcon();
                RemoveEditLabelControlForAddingComment();

            }
        }

        public void RemoveEditLabelControlForAddingComment() // @Comment-todo
        {
            m_EditableLabel.AddComment -= new EventHandler(EditableLabel_AddComment);
            m_EditableLabel.CloseComment -= new EventHandler(EditLabel_CloseAddComment);
            double TimeOfCursor = 0;
            double SelectionBeginTime = 0;
            double SelectionEndTime = 0;
            if (m_ContentView.Selection is AudioSelection)
            {
                if ((m_ContentView.Selection as AudioSelection).AudioRange.HasCursor)
                {
                    TimeOfCursor = ((AudioSelection)m_ContentView.Selection).AudioRange.CursorTime;
                }
                else
                {
                    SelectionBeginTime = ((AudioSelection)m_ContentView.Selection).AudioRange.SelectionBeginTime;
                    SelectionEndTime = ((AudioSelection)m_ContentView.Selection).AudioRange.SelectionEndTime;
                }
            }

            if (TimeOfCursor != 0 && m_SelectedNodeToAddComment is PhraseNode) 
            {
                m_ContentView.Selection = new AudioSelection((PhraseNode)m_SelectedNodeToAddComment, m_ContentView,
                    new AudioRange(TimeOfCursor));
            }
            else if (SelectionBeginTime != 0 && SelectionEndTime != 0 && m_SelectedNodeToAddComment is PhraseNode)
            {
                ContentView.Selection = new AudioSelection((PhraseNode)m_SelectedNodeToAddComment, m_ContentView,
                    new AudioRange(SelectionBeginTime, SelectionEndTime));
            }
            else
            {
                m_ContentView.Selection = new NodeSelection(m_SelectedNodeToAddComment, m_ContentView);
            }

            this.Controls.Remove(m_EditableLabel);
        }
        public void AlignCommentEditLabel() // @Comment-todo
        {
                m_EditableLabel.Location = new Point(0,0);  
        }
        public void ClearComment() // @Comment-todo
        {
            if (m_ProjectView.Selection.Node is EmptyNode)
            {
                EmptyNode tempNode = (EmptyNode)m_ProjectView.Selection.Node;
                tempNode.CommentText = null;
            }
            m_Block.UpdateLabelsText();
        }
        public void ShowCommentIconInZoomWaveform() // @Comment-todo
        {
            m_Block.UpdateLabelsText();
            m_Block.AlignLabelToShowCommentIcon();
        }
      
    }
}
