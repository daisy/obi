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
       
        public AssociateSpecialNode(ObiRootNode obiNode, EmptyNode selectedNode,Settings settings)
        {
            m_ObiNode = obiNode;
            m_SelectedNode = selectedNode;

            InitializeComponent();
            if (selectedNode != null && selectedNode is EmptyNode)
            {
                m_txtBox_SectionName.Visible = true;
                m_lb_listOfAllAnchorNodes.Visible = false;
                if (GetReferedNode(selectedNode) != null)
                    m_txtBox_SectionName.Text = selectedNode.ToString() + " = " + GetReferedNode(selectedNode);
                else
                    m_txtBox_SectionName.Text = selectedNode.ToString();
                groupBox1.Text = Localizer.Message("AssociateNode_SelectedNodeGroupBox");

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
            helpProvider1.HelpNamespace = Localizer.Message("CHMhelp_file_name");
            helpProvider1.SetHelpNavigator(this, HelpNavigator.Topic);
            helpProvider1.SetHelpKeyword(this, "HTML Files/Creating a DTB/Working with Phrases/Associating Skippable Note with Anchor.htm");
            if (this.Font.Name != settings.ObiFont)
            {
                this.Font = new Font(settings.ObiFont, this.Font.Size, FontStyle.Regular);//@fontconfig
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
                listOfFirstNodeOfSpecialNodes.Clear();
                m_IsShowAll = true;
                m_lb_listOfAllAnchorNodes.Visible = true;      
                AddToListBox();
                if (m_SelectedNode != null)
                {
                    int index = m_lb_listOfAllAnchorNodes.FindString(Localizer.Message("AssociateNode_SelectedSection") + m_SelectedNode.ParentAs<SectionNode>().Label + " " + m_txtBox_SectionName.Text);
                    if (index != -1)
                        m_lb_listOfAllAnchorNodes.SetSelected(index, true);
                }
                groupBox1.Text = Localizer.Message("List_of_anchor_nodes");
                m_txtBox_SectionName.Visible = false;
                m_btn_ShowAll.Text = Localizer.Message("Associate_show_selected");
                m_IsShowAll = true;
            }
            else if (m_btn_ShowAll.Text == Localizer.Message("Associate_show_selected"))
            {
                listOfAnchorNodes.Clear();
                listOfFirstNodeOfSpecialNodes.Clear();
                m_txtBox_SectionName.Visible = true;
                if (GetReferedNode(m_SelectedNode) != null)
                    m_txtBox_SectionName.Text = GetEmptyNodeString(m_SelectedNode) + " = " + GetEmptyNodeString(GetReferedNode(m_SelectedNode));
                else
                    m_txtBox_SectionName.Text = GetEmptyNodeString(m_SelectedNode);
                m_lb_listOfAllAnchorNodes.Visible = false;
                groupBox1.Text = Localizer.Message("Selected_node");
                m_btn_ShowAll.Text = Localizer.Message("Associate_show_all");
                m_IsShowAll = false;
                AddToListBox();
            }
            UpdateButtons();
          //  if (m_lb_listOfAllAnchorNodes.SelectedIndex < 0)
            //    m_btn_Deassociate.Enabled = false;
        }

        public void AddToListBox()
        {
            List<SectionNode> listOfAllSections = new List<SectionNode>();
            listOfAllSections = m_ObiNode.GetListOfAllSections();

            foreach (SectionNode node in listOfAllSections)
            {                
                for (int i = 0; i < node.PhraseChildCount; i++)
                {
                    AddToSpecialNodeListbox(node, i);
                    AddToAnchorNodeListbox(node, i);                   
                }
            }

            foreach (EmptyNode eNode in listOfFirstNodeOfSpecialNodes)
            {
                m_lb_ListOfSpecialNodes.Items.Add(GetEmptyNodeString(eNode));
            }
        }

        private void AddToSpecialNodeListbox(SectionNode node, int i)
        {
            string tempString = "";
            if (node.PhraseChild(i).Role_ == EmptyNode.Role.Custom && (node.PhraseChild(i).CustomRole == EmptyNode.Footnote || node.PhraseChild(i).CustomRole == EmptyNode.EndNote || node.PhraseChild(i).CustomRole == EmptyNode.Annotation || node.PhraseChild(i).CustomRole == EmptyNode.ProducerNote || node.PhraseChild(i).CustomRole == EmptyNode.Note))
            {
                tempString = node.PhraseChild(i).CustomRole;
                if(i <= node.PhraseChildCount -1)
                {
                    if(((node.PhraseChild(i).PrecedingNode is SectionNode || (node.PhraseChild(i).PrecedingNode is EmptyNode &&  tempString != ((EmptyNode)node.PhraseChild(i).PrecedingNode).CustomRole )) && node.PhraseChild(i).FollowingNode is EmptyNode && tempString  == ((EmptyNode)node.PhraseChild(i).FollowingNode).CustomRole )
                        || (node.PhraseChild(i).PrecedingNode is EmptyNode && node.PhraseChild(i).FollowingNode is EmptyNode && tempString != ((EmptyNode)node.PhraseChild(i).PrecedingNode).CustomRole && tempString != ((EmptyNode)node.PhraseChild(i).FollowingNode).CustomRole)
                        || (i == node.PhraseChildCount - 1 && ((node.PhraseChild(i).PrecedingNode is SectionNode || (node.PhraseChild(i).PrecedingNode is EmptyNode && ((EmptyNode)node.PhraseChild(i).PrecedingNode).CustomRole != tempString))))
                        )
                        listOfFirstNodeOfSpecialNodes.Add(node.PhraseChild(i));            
                }                     
            }
        }

        private void AddToAnchorNodeListbox(SectionNode node, int i)
        {
            string selectedSymbol = m_SelectedNode != null && m_SelectedNode == node.PhraseChild(i) ? Localizer.Message("AssociateNode_SelectedSection") : Localizer.Message("AssociateNode_Section");
            if (IsAnchor(node.PhraseChild(i)) || (m_SelectedNode != null && node == m_SelectedNode.ParentAs<SectionNode>() && node.PhraseChild(i) == m_SelectedNode))
            {
                if (m_IsShowAll || m_SelectedNode == null)
                {
                    
                    if (m_SelectedNode == node.PhraseChild(i))
                    {
                        if (GetReferedNode(m_SelectedNode) != null)
                            m_lb_listOfAllAnchorNodes.Items.Add(selectedSymbol + node.Label + " " + GetEmptyNodeString(node.PhraseChild(i)) + " = " + GetEmptyNodeString(GetReferedNode(node.PhraseChild(i))));
                        else
                            m_lb_listOfAllAnchorNodes.Items.Add(selectedSymbol + node.Label + " " + GetEmptyNodeString(node.PhraseChild(i)));
                     }
                    else if (GetReferedNode(node.PhraseChild(i)) != null)
                    {
                        m_lb_listOfAllAnchorNodes.Items.Add(selectedSymbol + node.Label + " " + GetEmptyNodeString(node.PhraseChild(i)) + " = " + GetEmptyNodeString(GetReferedNode(node.PhraseChild(i))));
                    }
                    else
                        m_lb_listOfAllAnchorNodes.Items.Add(selectedSymbol + node.Label + " " + GetEmptyNodeString(node.PhraseChild(i)));

                    listOfAnchorNodes.Add(node.PhraseChild(i));
                }
                listOfAnchorNodesCopy.Add(node.PhraseChild(i));
            }
            if (m_IsShowAll == false)
            {
                listOfAnchorNodes.Clear();
                listOfAnchorNodes.Add(m_SelectedNode);                
            }
        }

        private void m_btn_Associate_Click(object sender, EventArgs e)
        {
             if (m_IsShowAll && (m_lb_listOfAllAnchorNodes.SelectedIndex < 0 || m_lb_ListOfSpecialNodes.SelectedIndex < 0)) return;

            EmptyNode anchorNode = null; 
                       
            if (m_IsShowAll)
            {
                anchorNode = listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex];
            }
            else
            {
               anchorNode = m_SelectedNode;
            }
            if (anchorNode != null)
            {
                if ( GetReferedNode (anchorNode ) != null 
                    && GetReferedNode(anchorNode) != listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex] &&  MessageBox.Show(Localizer.Message("Node_already_associated"),Localizer.Message("Associate"), MessageBoxButtons.YesNo,
                                           MessageBoxIcon.Question) == DialogResult.No)
                    {
                         return ;
                    }
                    
                AssociateNodes (anchorNode, listOfFirstNodeOfSpecialNodes[m_lb_ListOfSpecialNodes.SelectedIndex]);
            }
            UpdateButtons();
            m_btn_Associate.Enabled = false;
        }

        private void AssociateNodes(EmptyNode anchorNode, EmptyNode referedNode)
        {
            if (m_Nodes_phraseMap.ContainsKey(anchorNode))
            {
                m_Nodes_phraseMap[anchorNode] = referedNode;
            }
            else
            {
                m_Nodes_phraseMap.Add(anchorNode, referedNode);
            }
           // anchorNode.AssociatedNode = referedNode;
            UpdateAnchorUserInterfaceForAssociation(anchorNode);
        }

        private void UpdateAnchorUserInterfaceForAssociation(EmptyNode anchorNode)
        {
            if (m_IsShowAll)
            {
                int listBoxIndex = listOfAnchorNodes.IndexOf(anchorNode);
                string selectedSymbol = m_SelectedNode != null && m_SelectedNode == anchorNode ? "=>>" : "";
                m_lb_listOfAllAnchorNodes.Items[listBoxIndex] = selectedSymbol + Localizer.Message("AssociateNode_Section") + listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].ParentAs<SectionNode>().Label + " " + GetEmptyNodeString(anchorNode) + " = " + GetEmptyNodeString(GetReferedNode(anchorNode));
            }
            else
            {
                m_txtBox_SectionName.Text = GetEmptyNodeString(anchorNode) + " = " + GetEmptyNodeString(GetReferedNode(anchorNode));
            }
        }

        private void UpdateAnchorUIForDeassociation(EmptyNode anchorNode)
        {
            if (m_IsShowAll)
            {
                if (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0)
                {
                    if (anchorNode == m_SelectedNode)
                        m_lb_listOfAllAnchorNodes.Items.Insert(m_lb_listOfAllAnchorNodes.SelectedIndex, Localizer.Message("AssociateNode_SelectedSection") + listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].ParentAs<SectionNode>().Label + " " + GetEmptyNodeString(anchorNode));
                    else
                        m_lb_listOfAllAnchorNodes.Items.Insert(m_lb_listOfAllAnchorNodes.SelectedIndex, Localizer.Message("AssociateNode_Section") + listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex].ParentAs<SectionNode>().Label + " " + GetEmptyNodeString(anchorNode));
                    m_lb_listOfAllAnchorNodes.Items.Remove(m_lb_listOfAllAnchorNodes.SelectedItem);
                }
            }
            else
                m_txtBox_SectionName.Text = GetEmptyNodeString(anchorNode);
        }

        private void DeassociateNodes(EmptyNode anchorNode)
        {
            if (m_Nodes_phraseMap.ContainsKey(anchorNode))
            {
                m_Nodes_phraseMap[anchorNode] = null;
            }
            else
            {
                m_Nodes_phraseMap.Add(anchorNode, null);
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
            DeassociateNodes(anchorNode);
            UpdateAnchorUIForDeassociation(anchorNode);
            UpdateButtons();
    //        if(listOfAnchorNodes.Count == 1)
           // m_btn_Deassociate.Enabled = false;
        }

        private void m_lb_ListOfSpecialNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void m_lb_listOfAllAnchorNodes_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtons();         
        }

        private void m_btn_OK_Click(object sender, EventArgs e)
        {   
        }

        private void AssociateSpecialNode_Load(object sender, EventArgs e)
        {
            UpdateButtons();
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
            if (node.Role_ == EmptyNode.Role.Custom && EmptyNode.SkippableNamesList.Contains(node.CustomRole))
            {
                SectionNode parentSection = node.ParentAs<SectionNode>();
                EmptyNode lastNode = parentSection.PhraseChild(parentSection.PhraseChildCount - 1);
                for (int i = node.Index; i < parentSection.PhraseChildCount; i++)
                {
                    if (parentSection.PhraseChild(i).Role_ != node.Role_
                        || parentSection.PhraseChild(i).CustomRole != node.CustomRole)
                    {
                        break;
                    }
                    lastNode = parentSection.PhraseChild(i);
                }
                string range = node != lastNode? (node.Index + 1).ToString() + " to " + (lastNode.Index + 1).ToString():
                    (node.Index + 1).ToString();
                info = Localizer.Message("AssociateNode_Section") + parentSection.Label + " " + node.CustomRole + " " + range;
                return info;
            }
            else
            {

                double durationMs = node.Duration;
                double seconds = Math.Round((durationMs / 1000), 1, MidpointRounding.ToEven);
                string dur = "(" + Convert.ToString(seconds) + "s)";
                info = String.Format(Localizer.Message("phrase_to_string"),
                    "",
                    "",
                    node.IsRooted ? node.Index + 1 : 0,
                    node.IsRooted ? node.ParentAs<ObiNode>().PhraseChildCount : 0,
                    "",
                    node.Role_ == EmptyNode.Role.Custom ? String.Format(Localizer.Message("phrase_extra_custom"), node.CustomRole) :

                        Localizer.Message("phrase_extra_" + node.Role_.ToString()));
                info = info.Replace("(", "");
                info = info.Replace(")", "");
                return info;
            }
        }

        private void UpdateButtons()
        {
            if (m_lb_listOfAllAnchorNodes.Visible)
                m_btn_Associate.Enabled = m_lb_listOfAllAnchorNodes.SelectedItem != null && m_lb_ListOfSpecialNodes.SelectedItem != null;
            else
                m_btn_Associate.Enabled = m_lb_ListOfSpecialNodes.SelectedItem != null;

            m_btn_Deassociate.Enabled = (m_lb_listOfAllAnchorNodes.SelectedIndex >= 0 && GetReferedNode(listOfAnchorNodes[m_lb_listOfAllAnchorNodes.SelectedIndex]) != null) || (!m_IsShowAll && m_SelectedNode != null && GetReferedNode(m_SelectedNode) != null);
        }

    }
}

