using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SpecialPhraseList : Form
    {
        private readonly ProjectView.ProjectView mView;
        private EmptyNode selectedItem = null;
        private readonly List<EmptyNode> backendList = new List<EmptyNode>();
       
        public SpecialPhraseList(Obi.ProjectView.ProjectView projectView)
        {
            mView = projectView;
            InitializeComponent();
            m_btnOK.Enabled = false;
        }

        public EmptyNode SpecialPhraseSelected
        {
            get { return selectedItem; }
        }
       
        private void m_btnFind_Click(object sender, EventArgs e)
        {
           m_btnOK.Enabled = false;
           this.m_lbSpecialPhrasesList.Items.Clear();
           string sectionName = null;
            mView.Presentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                        {
                            switch (m_cb_SpecialPhrases.SelectedIndex)
                            {
                                case 0:
                                    if (n is EmptyNode && ((EmptyNode) n).TODO)
                                    {
                                       sectionName = ((EmptyNode) n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                       m_lbSpecialPhrasesList.Items.Add(sectionName);
                                       backendList.Add(((EmptyNode)n));
                                    }

                                    break;
                                case 1:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Heading)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
                                       
                                    }
                                    break;
                                case 2:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Silence)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
                                    }
                                    break;
                                case 3:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
                                    }
                                    break;
                                case 4:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Front)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
                                    }
                                    break;
                                case 5:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Normal)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
                                    }
                                    break;
                                case 6:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Special)
                                    {
                                        sectionName = ((EmptyNode)n).ParentAs<SectionNode>().Label + " : " + ((EmptyNode)n);
                                        m_lbSpecialPhrasesList.Items.Add(sectionName);
                                        backendList.Add(((EmptyNode)n));
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

           m_btnOK.Enabled = m_lbSpecialPhrasesList.SelectedIndex >= 0;
        }

      }
    }
