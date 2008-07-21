using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class SectionProperties : Form
    {
        private Obi.SectionNode mNode;
        private ProjectView.ProjectView mView;

 
        public SectionProperties(ProjectView.ProjectView View)
        {
            InitializeComponent();
            mView = View;
            mNode = View.SelectedNodeAs<SectionNode>();
        }


        private void SectionProperties_Load(object sender, EventArgs e)
        {
            for (int i = 1; i <= mNode.Level; ++i) m_comboLevel.Items.Add(i);
            if (mNode.CanIncreaseLevel) m_comboLevel.Items.Add(mNode.Level + 1);
            m_comboLevel.SelectedIndex = mNode.Level - 1;
            m_txtName.Text = mNode.Label;
            m_txtTimeLength.Text = Program.FormatDuration_Long(mNode.Duration);
            m_txtPhraseCount.Text = mNode.PhraseChildCount.ToString();
            m_chkUsed.Checked = mNode.Used;

            if (mNode.Level == 1) m_lbParentsList.Items.Insert(0, "It has no parent sections");
            SectionNode IterationNode = mNode;
            for (int i = 0; i < mNode.Level - 1; i++)
            {
                IterationNode = IterationNode.ParentAs<SectionNode>();
                string strListItem = IterationNode.Label + " Level" + IterationNode.Level.ToString();
                m_lbParentsList.Items.Insert(0, strListItem);
            }
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            ChangeSectionName();
            ChangeSectionLevel();
            Close();
        }

        private void ChangeSectionName()
        {
            if (m_txtName.Text != "" && m_txtName.Text != mNode.Label)
                mView.RenameSectionNode(mNode, m_txtName.Text);
        }

        private void ChangeUsedStatus()
        {
            if (m_chkUsed.Checked != mNode.Used)
                mView.SetSelectedNodeUsedStatus(!mNode.Used);
        }

        private void ChangeSectionLevel()
        {
            int newSectionLevel = m_comboLevel.SelectedIndex + 1;
            if (newSectionLevel != mNode.Level)
            {
                if (newSectionLevel < mNode.Level)
                {
                    for (int i = mNode.Level; i > newSectionLevel; i--)
                        mView.DecreaseSelectedSectionLevel();
                }
                else
                {
                    for (int i = mNode.Level; i < newSectionLevel; i++)
                        mView.IncreaseSelectedSectionNodeLevel();
                }
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
