using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using TreeNode = urakawa.core.TreeNode;

namespace Obi.Dialogs
{
    public partial class SpecialPhraseList : Form
    {
        private readonly ProjectView.ProjectView mView;
        private EmptyNode selectedItem = null;
        private readonly List<EmptyNode> backendList = new List<EmptyNode>();
        private ProjectView.TransportBar mBar;
        private Image m_PauseImg;
        private Image m_PlayImg;
        private Image m_StopImg;

        public SpecialPhraseList(Obi.ProjectView.ProjectView projectView)
        {
            mView = projectView;
            mBar =  projectView.TransportBar;
            InitializeComponent();
            m_btnOK.Enabled = false;
            AddCustomRoles();
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            Stream pauseStr = null;
            Stream playStr = null;
            Stream stopStr = null;
            pauseStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-pause.png");
            playStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-start.png");
            stopStr = myAssembly.GetManifestResourceStream("Obi.UserControls.media-playback-stop.png");
            m_PauseImg = Image.FromStream(pauseStr);
            m_PlayImg = Image.FromStream(playStr);
            m_StopImg = Image.FromStream(stopStr);
            m_BtnPause.Image = m_PauseImg;
            m_BtnPlay.Image = m_PlayImg;
            m_BtnStop.Image = m_StopImg;
            mBar.StateChanged += new AudioLib.AudioPlayer.StateChangedHandler(State_Changed_Player);
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Exploring the GUI/Obi Views and Transport Bar/Collect special phrases and navigate.htm");
            if (mView.ObiForm.Settings.ObiFont != this.Font.Name)
            {
                this.Font = new Font(mView.ObiForm.Settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
            }
        }

        public void State_Changed_Player(object sender, AudioLib.AudioPlayer.StateChangedEventArgs e)
        {
           UpdateButtons();
        }

        private void AddCustomRoles()
        {
            foreach (string str in mView.Presentation.CustomClasses)
            {
                m_cb_SpecialPhrases.Items.Add(str);  
            }
            
        }

        public EmptyNode SpecialPhraseSelected
        {
            get { return selectedItem; }
        }
       
        private void m_btnFind_Click(object sender, EventArgs e)
        {
            CollectPhrases();
        }

        private void CollectPhrases ()
        {
           m_btnOK.Enabled = false;
           this.m_lbSpecialPhrasesList.Items.Clear();
           string sectionName = null;
           EmptyNode lastNodeOfChunk = null;
            mView.Presentation.RootNode.AcceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                    {
                        switch (m_cb_SpecialPhrases.SelectedIndex)
                        {
                            case 0:
                                if (n is EmptyNode && ((EmptyNode) n).TODO)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 1:
                                if ((n is EmptyNode && !(n is PhraseNode)) || ( n is PhraseNode && ((PhraseNode)n).Duration == 0 ))
                                {
                                    sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode)n));   
                                }
                                break;
                            case 2:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Heading)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 3:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Silence)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 4:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 5:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode) n).PageNumber.Kind == PageKind.Front)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 6:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode) n).PageNumber.Kind == PageKind.Normal)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 7:
                                if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                    ((EmptyNode) n).PageNumber.Kind == PageKind.Special)
                                {
                                    sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode) n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode) n));
                                }
                                break;
                            case 8:
                                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Anchor)
                                {
                                    sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode)n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode)n));
                                }
                                break;
                            case 9:
                                if (n is EmptyNode && !((EmptyNode)n).Used)
                                {
                                    sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " +
                                                  ((EmptyNode)n);
                                    m_lbSpecialPhrasesList.Items.Add(sectionName);
                                    backendList.Add(((EmptyNode)n));
                                }
                                break;
                            default:
                                {
                                    string itemString = (string) m_cb_SpecialPhrases.SelectedItem;
                                    if (n is EmptyNode && !string.IsNullOrEmpty(itemString)
                                        && ((EmptyNode) n).Role_ == EmptyNode.Role.Custom &&
                                        ((EmptyNode) n).CustomRole == itemString)
                                    {
                                        if (lastNodeOfChunk != null && n.Parent == lastNodeOfChunk.Parent &&
                                            ((EmptyNode) n).Index <= lastNodeOfChunk.Index)
                                            return true;
                                        else
                                            lastNodeOfChunk = null;

                                        SectionNode section = ((EmptyNode) n).ParentAs<SectionNode>();
                                        int phraseIndex = ((EmptyNode) n).Index;
                                        if (phraseIndex < section.PhraseChildCount - 1 &&
                                            ((EmptyNode) n).CustomRole ==
                                            section.PhraseChild(phraseIndex + 1).CustomRole)
                                        {
                                            int customRoleEndIndex = phraseIndex;
                                            for (int i = phraseIndex; i < section.PhraseChildCount - 1; i++)
                                            {
                                                if (section.PhraseChild(i).CustomRole !=
                                                    section.PhraseChild(i + 1).CustomRole)
                                                {
                                                    customRoleEndIndex = i;
                                                    break;
                                                }
                                            }
                                            sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                          itemString + ": " + phraseIndex.ToString() + " - " +
                                                          customRoleEndIndex.ToString();
                                            m_lbSpecialPhrasesList.Items.Add(sectionName);
                                            backendList.Add(((EmptyNode) n));
                                            //n = section.PhraseChild(customRoleEndIndex + 1);
                                            lastNodeOfChunk = section.PhraseChild(customRoleEndIndex + 1);
                                        }
                                        else
                                        {
                                            sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " +
                                                          ((EmptyNode) n);
                                            m_lbSpecialPhrasesList.Items.Add(sectionName);
                                            backendList.Add(((EmptyNode) n));
                                        }
                                    }
                                }
                                break;
                        }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
        }

      private void m_lbSpecialPhrasesList_SelectedIndexChanged(object sender, EventArgs e)
        {
           int selectedeNode = m_lbSpecialPhrasesList.SelectedIndex;
           selectedItem = backendList[selectedeNode];
          if (mBar.IsPlayerActive)
          {
             mBar.Stop();
          }

           if (m_lbSpecialPhrasesList.SelectedIndex >= 0)
           {
             m_btnOK.Enabled = true;
             m_BtnPlay.Enabled = true;
            }
           else
           {
               m_BtnPlay.Enabled = false; 
           }
        }

        private void m_cb_SpecialPhrases_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                CollectPhrases();
            }
        }

        private void m_BtnPause_Click(object sender, EventArgs e)
        {
            mBar.Pause();
            //UpdateButtons();
        }

        private void m_BtnStop_Click(object sender, EventArgs e)
        {
         if (mBar.CanStop) mBar.Stop();
            //UpdateButtons();
        }

        private void m_BtnPlay_Click(object sender, EventArgs e)
        {
            
            if (mBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
            {
                mBar.PlayOrResume();
            }
            else
            {
                //mView.SelectFromTransportBar(backendList[m_lbSpecialPhrasesList.SelectedIndex], null);
                mBar.PlayOrResume(backendList[m_lbSpecialPhrasesList.SelectedIndex]);
            }
            //UpdateButtons();    
        }

        private void UpdateButtons()
        {

            if (mBar.CurrentState == Obi.ProjectView.TransportBar.State.Playing)
            {
                m_BtnPause.Enabled = true;
                m_BtnPause.Visible = true;
                if (m_BtnPlay.ContainsFocus) m_BtnPause.Focus();
                m_BtnStop.Enabled = true;
                m_BtnPlay.Visible = false;
            }
            else if (mBar.CurrentState == Obi.ProjectView.TransportBar.State.Paused)
            {
                m_BtnPlay.Enabled = true;
                m_BtnPlay.Visible = true;
                if (m_BtnPause.ContainsFocus) m_BtnPlay.Focus();
                m_BtnPause.Visible = false;
            }
            else if(mBar.CurrentState == Obi.ProjectView.TransportBar.State.Stopped)
            {
                m_BtnPlay.Visible = true;
                m_BtnPlay.Enabled = true;
                m_BtnPause.Visible = false;
            }
           /* m_BtnPause.Visible = mBar.CanPause;
            m_BtnPlay.Visible = !m_BtnPause.Visible;
            m_BtnPlay.Enabled = mBar.CanPlay || mBar.CanResumePlayback;
            m_BtnStop.Enabled = mBar.CanStop;
            */
        }

      }
    }
