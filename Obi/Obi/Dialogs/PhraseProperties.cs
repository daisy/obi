using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class PhraseProperties : Form
    {
        private Obi.EmptyNode m_node;
        private ProjectView.ProjectView m_View;

        public PhraseProperties( ProjectView.ProjectView view)
        {
            InitializeComponent();
            m_node = (EmptyNode)view.Selection.Node;
            m_View = view;
        }

        private void PhraseProperties_Load(object sender, EventArgs e)
        {
            LoadPhraseProperties();
        }

        private void LoadPhraseProperties ()
        {
            m_txtParentSection.Text = m_node.ParentAs<SectionNode>().Label;
            m_txtLocationInsideSection.Text = (m_node.Index+1).ToString() + " of " + m_node.ParentAs<SectionNode>().PhraseChildCount ;
            m_txtTimeLength.Text = (m_node.Duration / 1000).ToString();
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Heading);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Page);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Plain);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Silence);
            m_comboPhraseRole.Items.Add(EmptyNode.Kind.Custom);
            m_comboPhraseRole.SelectedItem = m_node.NodeKind;
            m_chkUsed.Checked = m_node.Used;
            m_chkToDo.Checked = m_node.IsTo_Do;


            SectionNode IterationNode = m_node.ParentAs<SectionNode>() ;
            if (IterationNode.Level == 1) m_lbParentsList.Items.Insert(0, "It has no parent sections");
            for (int i = 0; i < m_node.ParentAs<SectionNode>().Level - 1; i++)
            {
                IterationNode = IterationNode.ParentAs<SectionNode>();

                string strListItem = IterationNode.Label + " Level" + IterationNode.Level.ToString();
                m_lbParentsList.Items.Insert(0, strListItem);

            }

            EnableCustomClassField();
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            ChangeToDoStatus();
            ChangeUsedStatus();
            ChangeNodeKind();
            Close();
        }
        private void ChangeToDoStatus()
        {
            if (m_chkToDo.Checked != m_node.IsTo_Do)
                m_View.ToggleEmptyNodeTo_DoMark();
        }

        private void ChangeUsedStatus()
        {
            if (m_chkUsed.Checked != m_node.Used)
                m_View.SetSelectedNodeUsedStatus(m_chkUsed.Checked);
        }
        private void ChangeNodeKind()
        {
            if (((EmptyNode.Kind)m_comboPhraseRole.SelectedItem) != m_node.NodeKind)
            {
                switch ((EmptyNode.Kind)m_comboPhraseRole.SelectedItem)
                {
                    case EmptyNode.Kind.Heading:
                        m_View.MakeSelectedBlockIntoHeadingPhrase();
                        break;
                    case EmptyNode.Kind.Silence:
                        m_View.MakeSelectedBlockIntoSilencePhrase();
                        break;
                    case EmptyNode.Kind.Plain:
                        m_View.SetCustomTypeForSelectedBlock(EmptyNode.Kind.Plain, null);
                        break;
                    case EmptyNode.Kind.Page:
                        Dialogs.SetPageNumber dialog = new SetPageNumber(m_View.NextPageNumber, false, false);
                        if (dialog.ShowDialog() == DialogResult.OK) m_View.SetPageNumberOnSelectedBock(dialog.Number, dialog.Renumber);
                        break;
                    case EmptyNode.Kind.Custom:
                        if (m_View.CanMarkPhrase)
                        m_View.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(m_View, m_node, EmptyNode.Kind.Custom, m_txtCustomClassName.Text));
                        break;
                }
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_comboPhraseRole_SelectionChangeCommitted(object sender, EventArgs e)
        {
            EnableCustomClassField();
        }

        private void EnableCustomClassField()
        {
            if (((EmptyNode.Kind)m_comboPhraseRole.SelectedItem) == EmptyNode.Kind.Custom)
            {
                m_txtCustomClassName.Enabled = true;
            }
            else
                m_txtCustomClassName.Enabled = false;
        }


    }
}