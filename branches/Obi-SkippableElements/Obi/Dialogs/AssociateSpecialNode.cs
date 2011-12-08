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
        private ObiRootNode m_ObiNode;
        private bool m_IsShowAll = false;
        List<EmptyNode> listOfFirstNodeOfSpecialNodes = new List<EmptyNode>();
        List<EmptyNode> listOfAnchorNodes = new List<EmptyNode>();
        private EmptyNode m_SelectedNode = null;
        Dictionary<EmptyNode, EmptyNode> nodes_phraseMap = new Dictionary<EmptyNode, EmptyNode>(); // used for importing sections
      
        public AssociateSpecialNode(ObiRootNode obiNode, EmptyNode selectedNode)
        {
            m_ObiNode = obiNode;
            m_SelectedNode = selectedNode;
            InitializeComponent();
            m_txtBox_SectionName.Text = selectedNode.ToString();
            AddToListBox();           
        }

        public List<EmptyNode> AnchorNode
        { get { return listOfAnchorNodes; } }

        public List<EmptyNode> SpecialNode
        { get { return listOfFirstNodeOfSpecialNodes; } }

        public Dictionary<EmptyNode,EmptyNode> DictionaryToMapValues
        { get { return nodes_phraseMap; } }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            m_IsShowAll = true;
            m_lb_listOfAllAnchorNodes.Visible = true;
            AddToListBox();
            m_btn_ShowAll.Enabled = false;
        }

        public void AddToListBox()
        {
            List<SectionNode> listOfAllSections = new List<SectionNode>();
            listOfAllSections = m_ObiNode.GetListOfAllSections();
            string tempString = "";
            int firstIndex = -1;
            foreach (SectionNode node in listOfAllSections)
            {
                for (int i = 0; i < node.PhraseChildCount; i++)
                {
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom)
                    {
                        tempString = node.PhraseChild(i).CustomRole;
                        if ( i < node.PhraseChildCount - 1 && tempString != node.PhraseChild(i + 1).CustomRole )
                        {
                            if (!m_IsShowAll)
                            {
                               if (firstIndex == -1 && !m_IsShowAll)
                                  m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (i + 1));
                               else
                                  m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));                               
                            }
                           // if (m_IsShowAll)
                             //  m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                            firstIndex = -1;
                        }
                        else
                        {
                            if (firstIndex == -1)
                            {
                                firstIndex = i;
                                listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                            }
                            if (i == node.PhraseChildCount - 1)
                            {                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
                                if(!m_IsShowAll)
                                m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));
                               // if(m_IsShowAll)
                               // m_lb_listOfAllAnchorNodes.Items.Add(node.Label);
                                firstIndex = -1;
                            }
                        }
                    }
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Anchor && m_IsShowAll)
                    {
                        m_lb_listOfAllAnchorNodes.Items.Add(node.PhraseChild(i));
                        listOfAnchorNodes.Add(node.PhraseChild(i));
                    }
                       // m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " +node.PhraseChild(i));
                }
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
            if (m_IsShowAll)
                nodes_phraseMap.Add(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex], listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]);
            else
                nodes_phraseMap.Add(m_SelectedNode, listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]); 

            //  listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode = c;
           //  if(m_IsShowAll)
            //     listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
            // else
              //  ((EmptyNode)m_View.Selection.Node).AssociatedNode = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
             //   m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.Items[m_AnchorNodeIndex]);
             //   m_lb_ListOfSpecialNodes.Items.Insert(m_AnchorNodeIndex, m_lb_ListOfSpecialNodes.Items[m_SpecialNodeIndex - 1]);                           
        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
           //  ((EmptyNode)m_View.Selection.Node).AssociatedNode = null;
            // m_lb_ListOfSpecialNodes.Items.Insert(m_lb_ListOfSpecialNodes.SelectedIndex, ""); 
            //  m_lb_ListOfSpecialNodes.Items.Remove(m_lb_ListOfSpecialNodes.SelectedItem);
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_btn_Associate.Enabled = true;
            m_btn_Deassociate.Enabled = true;        
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_btn_Associate.Enabled = true;
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listOfAnchorNodes.Count; i++ )
                Console.WriteLine("JUMP  " + listOfAnchorNodes[i] + "   " + listOfAnchorNodes[i].AssociatedNode);           
        }
    }
}


