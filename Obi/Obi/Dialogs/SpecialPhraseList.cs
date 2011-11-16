using System;
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
        
        public SpecialPhraseList(Obi.ProjectView.ProjectView projectView)
        {
            mView = projectView;
            InitializeComponent();
            m_btnOK.Enabled = m_lbSpecialPhrasesList.Items.Count != 0;
        }

        public EmptyNode SpecialPhraseSelected
        {
            get { return selectedItem; }
        }
       private void m_btnFind_Click(object sender, EventArgs e)
        {
            this.m_lbSpecialPhrasesList.Items.Clear();
            mView.Presentation.RootNode.AcceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                        {
                            switch (m_cb_SpecialPhrases.SelectedIndex)
                            {
                                case 0:
                                    if (n is EmptyNode && ((EmptyNode) n).TODO)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 1:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Heading)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 2:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Silence)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 3:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 4:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Front)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 5:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Normal)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                                case 6:
                                    if (n is EmptyNode && ((EmptyNode) n).Role_ == EmptyNode.Role.Page &&
                                        ((EmptyNode) n).PageNumber.Kind == PageKind.Special)
                                    {
                                        m_lbSpecialPhrasesList.Items.Add(((EmptyNode) n));
                                    }
                                    break;
                            }
                            return true;
                        },
                    delegate(urakawa.core.TreeNode n) { });
            m_btnOK.Enabled = m_lbSpecialPhrasesList.Items.Count != 0;
        }
       
        private void m_lbSpecialPhrasesList_SelectedIndexChanged(object sender, EventArgs e)
        {
             selectedItem = (EmptyNode)(m_lbSpecialPhrasesList.SelectedItem);
        }
        }
    }
