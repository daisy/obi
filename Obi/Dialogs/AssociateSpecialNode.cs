using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class AssociateSpecialNode : Form
    {
        private ProjectView.ProjectView m_View;
        private int m_AnchorNodeIndex = 0;
        private int m_SpecialNodeIndex = 0;
        private bool m_IsShowAll = false;
        List<EmptyNode> listOfFirstNodeOfSpecialNodes = new List<EmptyNode>();

        public AssociateSpecialNode(ProjectView.ProjectView projView)
        {
            m_View = projView;
            InitializeComponent();
            m_txtBox_SectionName.Text = m_View.Selection.ToString();
            AddToListBox();           
        }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            m_IsShowAll = true;
            m_lb_listOfAllAnchorNodes.Visible = true;
            AddToListBox();            
        }

        public void AddToListBox()
        {
            List<SectionNode> listOfAllSections = new List<SectionNode>();
            listOfAllSections = ((ObiRootNode)m_View.Presentation.RootNode).GetListOfAllSections();
            string tempString = "";
            int firstIndex = -1;
            //temp string mein store kare custin ckass. n put this string in list box/
            foreach (SectionNode node in listOfAllSections)
            {
                for (int i = 0; i < node.PhraseChildCount; i++)
                {
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom)
                    {
                        tempString = node.PhraseChild(i).CustomRole;
                        if (tempString != node.PhraseChild(i + 1).CustomRole)
                        {
                            if (!m_IsShowAll)
                            {
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + i);
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                            }
                            
                            if (m_IsShowAll)
                            {
                                m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                
                            }
                        }
                        else
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                               // firstIndex = -1;
                            }
                        }
                    }

                }
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
           // if (m_lb_ListOfSpecialNodes.Items[m_AnchorNodeIndex] == "")
            {
                ((EmptyNode)m_View.Selection.Node).AssociatedNode = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
                m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.Items[m_AnchorNodeIndex]);
                m_lb_ListOfSpecialNodes.Items.Insert(m_AnchorNodeIndex, m_lb_ListOfSpecialNodes.Items[m_SpecialNodeIndex - 1]);               
            }
        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
             ((EmptyNode)m_View.Selection.Node).AssociatedNode = null;
             m_lb_ListOfSpecialNodes.Items.Insert(m_lb_ListOfSpecialNodes.SelectedIndex, ""); 
             m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.SelectedItem);
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_btn_Associate.Enabled = true;
            m_btn_Deassociate.Enabled = true;
            m_SpecialNodeIndex = m_lb_ListOfSpecialNodes.SelectedIndex;            
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_btn_Associate.Enabled = true;
            m_AnchorNodeIndex = m_lb_listOfAllAnchorNodes.SelectedIndex;
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            m_View.SelectedBlockNode = ((EmptyNode)m_View.Selection.Node).AssociatedNode ;
        }
    }
}


