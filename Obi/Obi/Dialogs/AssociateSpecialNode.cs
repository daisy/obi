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
        Dictionary<EmptyNode, EmptyNode> m_Nodes_phraseMap = new Dictionary<EmptyNode, EmptyNode>(); // used for importing sections
        private List<EmptyNode> listOfAnchorNodesCopy = new List<EmptyNode>();
       
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
                m_IsShowAll = true;
                AddToListBox();
                m_btn_ShowAll.Enabled = false;
                m_txtBox_SectionName.Visible = false;
                groupBox1.Text = "List of anchor nodes";
                m_lb_listOfAllAnchorNodes.Visible = true;
            }                               
        }

        public Dictionary<EmptyNode,EmptyNode> DictionaryToMapValues
        { get { return m_Nodes_phraseMap; } }

        private void m_btn_ShowAll_Click(object sender, EventArgs e)
        {
            if (m_lb_listOfAllAnchorNodes.Items.Count > 0)
                m_lb_listOfAllAnchorNodes.Items.Clear();
            if (m_lb_ListOfSpecialNodes.Items.Count > 0)
                m_lb_ListOfSpecialNodes.Items.Clear();
            listOfAnchorNodesCopy.Clear();
            if (m_btn_ShowAll.Text == Localizer.Message("Associate_show_all"))
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
                m_btn_ShowAll.Text = Localizer.Message("Associate_show_selected");
                m_IsShowAll = true;
            }
            else if (m_btn_ShowAll.Text == Localizer.Message("Associate_show_selected"))
            {
                listOfAnchorNodes.Clear();
                m_txtBox_SectionName.Visible = true;
                m_lb_listOfAllAnchorNodes.Visible = false;
                groupBox1.Text = "Selected node";
                m_btn_ShowAll.Text = Localizer.Message("Associate_show_all");
                m_IsShowAll = false;
                AddToListBox();
            }

            if (m_lb_listOfAllAnchorNodes.SelectedIndex < 0)
                m_btn_Deassociate.Enabled = false;
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
                    if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom && (node.PhraseChild(i).CustomRole == EmptyNode.Footnote || node.PhraseChild(i).CustomRole == EmptyNode.EndNote || node.PhraseChild(i).CustomRole == EmptyNode.Annotation || node.PhraseChild(i).CustomRole == EmptyNode.ProducerNote || node.PhraseChild(i).CustomRole == EmptyNode.Note))
                    {
                        tempString = node.PhraseChild(i).CustomRole;
                        if ( i < node.PhraseChildCount - 1 && tempString != node.PhraseChild(i + 1).CustomRole )
                        {
                          //  if (!m_IsShowAll)
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
                                if (firstIndex == i)
                                    m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1));
                                else
                                {
                                     // if (!m_IsShowAll)
                                     m_lb_ListOfSpecialNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i).CustomRole + " " + (firstIndex + 1) + " to " + (i + 1));
                                }
                                firstIndex = -1;
                            }
                        }
                    }
                    if (IsAnchor(node.PhraseChild(i)) || (m_SelectedNode != null && node == m_SelectedNode.ParentAs<SectionNode>() && node.PhraseChild(i) == m_SelectedNode))
                    {
        //                m_lb_listOfAllAnchorNodes.Items.Add(node.PhraseChild(i));
                        if (m_IsShowAll || m_SelectedNode == null)
                        {
                            if (m_SelectedNode == node.PhraseChild(i))
                            {
                                if (m_SelectedNode.AssociatedNode != null)
                                {
                                    //m_lb_listOfAllAnchorNodes.Items.Add("=> Section " + node.Label + " " + node.PhraseChild(i) + " = Section " + node.PhraseChild(i).AssociatedNode.ParentAs<SectionNode>().Label + ", " + node.PhraseChild(i).AssociatedNode);
                                    m_lb_listOfAllAnchorNodes.Items.Add(">> Section " + node.Label + " " + GetEmptyNodeString( node.PhraseChild(i)) + " = Section " + node.PhraseChild(i).AssociatedNode.ParentAs<SectionNode>().Label + ", " + GetEmptyNodeString( node.PhraseChild(i).AssociatedNode));
                                }
                                else
                                {
                                    //m_lb_listOfAllAnchorNodes.Items.Add("=> Section " + node.Label + " " + node.PhraseChild(i));
                                    m_lb_listOfAllAnchorNodes.Items.Add(">> Section " + node.Label + " " + GetEmptyNodeString(node.PhraseChild(i)));
                                }
                            }
                            else if (node.PhraseChild(i).AssociatedNode != null)
                            {
                                //m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i) + " = Section " + node.PhraseChild(i).AssociatedNode.ParentAs<SectionNode>().Label + ", " + node.PhraseChild(i).AssociatedNode);
                                m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + GetEmptyNodeString( node.PhraseChild(i)) + " = Section " + node.PhraseChild(i).AssociatedNode.ParentAs<SectionNode>().Label + ", " +GetEmptyNodeString (node.PhraseChild(i).AssociatedNode));
                            }
                            else
                            {
                                //m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + node.PhraseChild(i));
                                m_lb_listOfAllAnchorNodes.Items.Add("Section " + node.Label + " " + GetEmptyNodeString( node.PhraseChild(i)));
                            }
                            listOfAnchorNodes.Add(node.PhraseChild(i));                           
                        }
                        listOfAnchorNodesCopy.Add(node.PhraseChild(i));
                    }
                    if (m_IsShowAll == false)
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
                if (m_IsShowAll && (m_lb_listOfAllAnchorNodes.SelectedIndex < 0 || m_lb_ListOfSpecialNodes.SelectedIndex < 0)) return;

            EmptyNode anchorNode = null; 
            bool IsAssociated = false;
            m_btn_Deassociate.Enabled = true;
            for (int i = 0; i < listOfAnchorNodesCopy.Count; i++)
            {
                if (listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex] == listOfAnchorNodesCopy[i].AssociatedNode)
                {
                    if (MessageBox.Show(Localizer.Message("Node_already_associated"), Localizer.Message("Associate"), MessageBoxButtons.YesNo,
                           MessageBoxIcon.Question) == DialogResult.No)
                    {
                        IsAssociated = true;
                        m_btn_Deassociate.Enabled = false;
                    }
                    break;
                }
                else
                    IsAssociated = false;
            }

            foreach (KeyValuePair<EmptyNode, EmptyNode> pair in m_Nodes_phraseMap)
                {
                   
                    if (m_Nodes_phraseMap.ContainsKey(pair.Key) && listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex] == pair.Key.AssociatedNode)
                    {
                        if (MessageBox.Show(Localizer.Message("Node_already_associated"),Localizer.Message("Associate"), MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) == DialogResult.No)
                        {
                            IsAssociated = true;
                            m_btn_Deassociate.Enabled = false;
                        }
                        break;
                    }
                    else
                        IsAssociated = false;
                }
            
            if (!IsAssociated)
            { 
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
                    if (m_Nodes_phraseMap.ContainsKey(anchorNode))
                    {
                        m_Nodes_phraseMap[anchorNode] = listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex];
                    }
                    else
                    {
                        m_Nodes_phraseMap.Add(anchorNode, listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]);
                    }                  
             }

        }

        private void m_btn_Deassociate_Click(object sender, EventArgs e)
        {
            EmptyNode anchorNode = null;
            if (listOfAnchorNodes.Count > 0
                && (m_SelectedNode == null || m_IsShowAll))
            {
                if(m_lb_listOfAllAnchorNodes.SelectedIndex >= 0 && m_lb_listOfAllAnchorNodes.SelectedIndex < m_lb_listOfAllAnchorNodes.Items.Count  ) anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];                
            }
            else
            {
                anchorNode = m_SelectedNode;
                 
            }
            if (anchorNode == null) return;
            if (m_Nodes_phraseMap.ContainsKey(anchorNode))
            {
                m_Nodes_phraseMap[anchorNode] = null;
            }
            else
            {
                m_Nodes_phraseMap.Add(anchorNode, null);
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
                ((listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].AssociatedNode != null && !m_Nodes_phraseMap.ContainsKey(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex])) ||
                (m_Nodes_phraseMap.ContainsKey(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]) && m_Nodes_phraseMap[listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]] != null)))
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
            if ((listOfAnchorNodes.Count == 1  && listOfAnchorNodes[0] != null)&&
               ((listOfAnchorNodes[0].AssociatedNode != null && !m_Nodes_phraseMap.ContainsKey(listOfAnchorNodes[0])) ||
               (m_Nodes_phraseMap.ContainsKey(listOfAnchorNodes[0]) && m_Nodes_phraseMap[listOfAnchorNodes[0]] != null)))
                m_btn_Deassociate.Enabled = true;
            else
                m_btn_Deassociate.Enabled = false;
           
        }

        private bool IsAnchor(EmptyNode node)
        {
            return (m_SelectedNode != null && node == m_SelectedNode)
            || node.Role_ == EmptyNode.Role.Anchor;
        }

        private EmptyNode GetReferedNode(EmptyNode node)
        {

            if (m_Nodes_phraseMap.ContainsKey(node))
            {
                return m_Nodes_phraseMap[node];
            }
            if (node.AssociatedNode != null) return node.AssociatedNode;

            
            return null;
        }


        private string GetEmptyNodeString(EmptyNode node)
        {
            if (node == null) return "";
            string info = null;
            double durationMs = node.Duration;
            double seconds = Math.Round((durationMs / 1000), 1, MidpointRounding.ToEven);
            string dur = "("+ Convert.ToString(seconds) + "s)";
            info = String.Format(Localizer.Message("phrase_to_string"),
                "",
                "",
                node.IsRooted ? node.Index + 1 : 0,
                node.IsRooted ? node.ParentAs<ObiNode>().PhraseChildCount : 0,
                "",
                node.Role_ == EmptyNode.Role.Custom ? String.Format(Localizer.Message("phrase_extra_custom"), node.CustomRole) :

                node.Role_ == EmptyNode.Role.Anchor && node.AssociatedNode == null ? Localizer.Message("phrase_extra_" + node.Role_.ToString()) + "= ?" :
                    Localizer.Message("phrase_extra_" + node.Role_.ToString()));

            return info;
        }



    }
}

