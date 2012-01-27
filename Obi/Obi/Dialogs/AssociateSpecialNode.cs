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
            if (selectedNode != null && selectedNode is EmptyNode)
            {                
                m_txtBox_SectionName.Visible = true;
                m_lb_listOfAllAnchorNodes.Visible = false;
                if (selectedNode.AssociatedNode != null)
                    m_txtBox_SectionName.Text = selectedNode.ToString() + " = ";
                else
                    m_txtBox_SectionName.Text = selectedNode.ToString();
                groupBox1.Text = "Selected node";
                
                AddToListBox();
            }
            else
            {
                AddToListBox();
                m_btn_ShowAll.Enabled = false;
                m_txtBox_SectionName.Visible = false;
                groupBox1.Text = "List of anchor nodes";
                m_lb_listOfAllAnchorNodes.Visible = true;
            }                       
        }

        public Dictionary<EmptyNode,EmptyNode> DictionaryToMapValues
        { get { return nodes_phraseMap; } }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            if (m_lb_listOfAllAnchorNodes.Items.Count > 0)
                m_lb_listOfAllAnchorNodes.Items.Clear();
            if (m_btn_ShowAll.Text == "Show all")
            {
                listOfAnchorNodes.Clear();
                m_IsShowAll = true;
                m_lb_listOfAllAnchorNodes.Visible = true;      
                AddToListBox();
                if (m_SelectedNode != null)
                {
                    int index = m_lb_listOfAllAnchorNodes.FindString("=> Section " + m_SelectedNode.ParentAs<SectionNode>().Label + " " + m_txtBox_SectionName.Text);
                    if (index != -1)
                        m_lb_listOfAllAnchorNodes.SetSelected(index, true);
                }
                groupBox1.Text = "List of anchor nodes";
                m_txtBox_SectionName.Visible = false;
                m_btn_ShowAll.Text = "Show selected";               
            }
            else if (m_btn_ShowAll.Text == "Show selected")
            {
                listOfAnchorNodes.Clear();
                m_txtBox_SectionName.Visible = true;
                m_lb_listOfAllAnchorNodes.Visible = false;
                groupBox1.Text = "Selected node";
                m_btn_ShowAll.Text = "Show all";
                AddToListBox();
            }
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
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom && (node.PhraseChild(i).CustomRole == EmptyNode.Footnote || node.PhraseChild(i).CustomRole == EmptyNode.EndNote || node.PhraseChild(i).CustomRole == EmptyNode.Annotation || node.PhraseChild(i).CustomRole == EmptyNode.ProducerNote))
                    {
                        tempString = node.PhraseChild(i).CustomRole;
                        if ( i < node.PhraseChildCount - 1 && tempString != node.PhraseChild(i + 1).CustomRole )
                        {
                            if (!m_IsShowAll)
                            {
                                if (firstIndex == -1)
                                {
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (i + 1));
                                    listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));
                                }
                                else
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));                               
                            }
                           
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
                                firstIndex = -1;
                            }
                        }
                    }
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Anchor || (node == m_SelectedNode.ParentAs<SectionNode>() && node.PhraseChild(i) == m_SelectedNode))
                    {
        //                m_lb_listOfAllAnchorNodes.Items.Add(node.PhraseChild(i));
                        if (m_IsShowAll || m_SelectedNode == null)
                        {
                            if (m_SelectedNode == node.PhraseChild(i))
                                m_lb_listOfAllAnchorNodes.Items.Add("=> Section " + node.Label + " " + node.PhraseChild(i) + " = " + node.PhraseChild(i).AssociatedNode);
                            else if (node.PhraseChild(i).AssociatedNode != null)
                                m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i) + " = " + node.PhraseChild(i).AssociatedNode);
                            else
                                m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i));
                            listOfAnchorNodes.Add(node.PhraseChild(i));
                        }
                    }
                    if (m_txtBox_SectionName.Visible == false)
                    {
                        listOfAnchorNodes.Clear();
                        //m_lb_listOfAllAnchorNodes.Items.Add("Section " + m_SelectedNode.ParentAs<SectionNode>() + " " + m_SelectedNode);
                        listOfAnchorNodes.Add(m_SelectedNode);        
                    }
                }
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
            EmptyNode anchorNode = null;
            if (listOfAnchorNodes.Count > 0 && (m_SelectedNode == null || m_IsShowAll))
            {
                if (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0)
                    anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];
                else
                    anchorNode = listOfAnchorNodes[0];
            }
            else 
            {
                anchorNode = m_SelectedNode;
            }
            if (nodes_phraseMap.ContainsKey(anchorNode))
            {
                nodes_phraseMap[anchorNode] = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
            }
            else
            {
                nodes_phraseMap.Add(anchorNode, listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]);
            }
          //  if(listOfAnchorNodes.Count == 1)
            m_btn_Deassociate.Enabled = true;
        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
            EmptyNode anchorNode = null;
            if (listOfAnchorNodes.Count > 0 && (m_SelectedNode == null || m_IsShowAll))
            {
                anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];                
            }
            else
            {
                anchorNode = m_SelectedNode;
                 
            }
            if (nodes_phraseMap.ContainsKey(anchorNode))
            {
                nodes_phraseMap[anchorNode] = null;
            }
            else
            {
                nodes_phraseMap.Add(anchorNode, null);
            }

            if (m_lb_listOfAllAnchorNodes.SelectedIndex >=0)
            {
            m_lb_listOfAllAnchorNodes.Items.Insert(m_lb_listOfAllAnchorNodes.SelectedIndex,"Section " + listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].ParentAs<SectionNode>().Label + " "+ listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]);
            m_lb_listOfAllAnchorNodes.Items.Remove(m_lb_listOfAllAnchorNodes.SelectedItem);
            }
    //        if(listOfAnchorNodes.Count == 1)
            m_btn_Deassociate.Enabled = false;
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            //m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.Items.Count > 0 && m_lb_ListOfSpecialNodes.Items.Count >0;
            if (m_lb_listOfAllAnchorNodes.Visible)
                m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
            else
                m_btn_Associate.Enabled = m_lb_ListOfSpecialNodes.SelectedItem != null;
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0 &&
                ((listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode != null && !nodes_phraseMap.ContainsKey(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex])) ||
                (nodes_phraseMap.ContainsKey(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]) && nodes_phraseMap[listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]] != null)))
                m_btn_Deassociate.Enabled = true;
            else
                m_btn_Deassociate.Enabled = false;
            if (m_lb_listOfAllAnchorNodes.Visible)
                m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
            else
                m_btn_Associate.Enabled = m_lb_ListOfSpecialNodes.SelectedItem != null;
            /*
            //m_btn_Associate.Enabled = (m_lb_listOfAllAnchorNodes.Items.Count > 0 && m_lb_ListOfSpecialNodes.Items.Count > 0);
            m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
          //  if (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0 && listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode != null || nodes_phraseMap[listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex] != null )
            //    MessageBox.Show(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode.ToString());
            if (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0)
            {
                if (listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode != null || (nodes_phraseMap.ContainsKey(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]) && nodes_phraseMap.ContainsValue(nodes_phraseMap[listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]]) && nodes_phraseMap[listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]] != null))
                    m_btn_Deassociate.Enabled = true;
                else
                    m_btn_Deassociate.Enabled = false;
            }*/
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {   
        }

        private void AssociateSpecialNode_Load(object sender, EventArgs e)
        {
            if (listOfAnchorNodes.Count == 1 &&
               ((listOfAnchorNodes[0].AssociatedNode != null && !nodes_phraseMap.ContainsKey(listOfAnchorNodes[0])) ||
               (nodes_phraseMap.ContainsKey(listOfAnchorNodes[0]) && nodes_phraseMap[listOfAnchorNodes[0]] != null)))
                m_btn_Deassociate.Enabled = true;
            else
                m_btn_Deassociate.Enabled = false;      
        }    
    }
}

