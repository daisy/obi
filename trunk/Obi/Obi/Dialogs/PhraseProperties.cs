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
        private Obi.EmptyNode mNode;
        private ProjectView.ProjectView mView;

        public PhraseProperties(ProjectView.ProjectView view)
        {
            InitializeComponent();
            mView = view;
            mNode = view.SelectedNodeAs<EmptyNode>();
        }

        private void PhraseProperties_Load(object sender, EventArgs e)
        {
            m_txtParentSection.Text = mNode.ParentAs<SectionNode>().Label;
            m_txtLocationInsideSection.Text = string.Format(Localizer.Message("node_position"),
                mNode.Index + 1, mNode.ParentAs<ObiNode>().PhraseChildCount);
            m_txtTimeLength.Text = Program.FormatDuration_Long(mNode.Duration);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Heading);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Page);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Plain);
            m_comboPhraseRole.Items.Add(PhraseNode.Kind.Silence);
            m_comboPhraseRole.Items.Add(EmptyNode.Kind.Custom);
            m_comboPhraseRole.SelectedItem = mNode.NodeKind;
            m_chkUsed.Checked = mNode.Used;
            m_chkToDo.Checked = mNode.IsTo_Do;


            SectionNode IterationNode = mNode.ParentAs<SectionNode>() ;
            if (IterationNode.Level == 1) m_lbParentsList.Items.Insert(0, "It has no parent sections");
            for (int i = 0; i < mNode.ParentAs<SectionNode>().Level - 1; i++)
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
            if (m_chkToDo.Checked != mNode.IsTo_Do)
                mView.ToggleEmptyNodeTo_DoMark();
        }

        private void ChangeUsedStatus()
        {
            if (m_chkUsed.Checked != mNode.Used)
                mView.SetSelectedNodeUsedStatus(m_chkUsed.Checked);
        }
        private void ChangeNodeKind()
        {
            if (((EmptyNode.Kind)m_comboPhraseRole.SelectedItem) != mNode.NodeKind)
            {
                switch ((EmptyNode.Kind)m_comboPhraseRole.SelectedItem)
                {
                    case EmptyNode.Kind.Heading:
                        mView.MakeSelectedBlockIntoHeadingPhrase();
                        break;
                    case EmptyNode.Kind.Silence:
                        mView.MakeSelectedBlockIntoSilencePhrase();
                        break;
                    case EmptyNode.Kind.Plain:
                        mView.SetCustomTypeForSelectedBlock(EmptyNode.Kind.Plain, null);
                        break;
                    case EmptyNode.Kind.Page:
                        Dialogs.SetPageNumber dialog = new SetPageNumber(mView.NextPageNumber, false, false);
                        if (dialog.ShowDialog() == DialogResult.OK) mView.SetPageNumberOnSelectedBock(dialog.Number, dialog.Renumber);
                        break;
                    case EmptyNode.Kind.Custom:
                        if (mView.CanMarkPhrase)
                        mView.Presentation.getUndoRedoManager().execute(new Commands.Node.ChangeCustomType(mView, mNode, EmptyNode.Kind.Custom, m_txtCustomClassName.Text));
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