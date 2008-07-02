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
            private Obi.SectionNode m_node;
            private ProjectView.ProjectView m_View;
        public SectionProperties( ProjectView.ProjectView View  )
        {
            InitializeComponent();
            m_View = View;
            m_node = (SectionNode) View.Selection.Node ;
                    }

        private void SectionProperties_Load(object sender, EventArgs e)
                    {
                        m_comboLevel.Items.Add(1);
                        m_comboLevel.Items.Add(2);
                        m_comboLevel.Items.Add(3);
                        m_comboLevel.Items.Add(4);
                        m_comboLevel.Items.Add(5);
                        m_comboLevel.Items.Add(6);

            m_txtName.Text = m_node.Label;
            m_comboLevel.SelectedIndex = m_node.Level- 1;

            SectionNode IterationNode = m_node;
            for (int i = 0 ; i < m_node.Level -1; i++)
            {
                IterationNode = IterationNode.ParentAs<SectionNode>();

                string strListItem = IterationNode.Label + " Level" + IterationNode.Level.ToString();
                m_lbParentsList.Items.Insert ( 0 , strListItem);
            }

            double time = 0.0;
            for (int i = 0; i < m_node.PhraseChildCount; i++)
                time += m_node.PhraseChild(i).Duration ;

            m_txtTimeLength.Text = (time / 1000).ToString();
            m_txtPhraseCount.Text = m_node.PhraseChildCount.ToString();

            m_chkUsed.Checked = m_node.Used;
        }

            private void m_btnOk_Click(object sender, EventArgs e)
            {
                ChangeSectionName();
                ChangeSectionLevel();
                Close();
            }

            private void ChangeSectionName()
            {
                if ( m_txtName.Text != "" && m_txtName.Text != m_node.Label )
                    m_View.RenameSectionNode ( m_node , m_txtName.Text ) ;
            }

            private void ChangeUsedStatus()
            {
                if (m_chkUsed.Checked != m_node.Used)
                    m_View.SetSelectedNodeUsedStatus(!m_node.Used);
            }

            private void ChangeSectionLevel()
            {
                int newSectionLevel = m_comboLevel.SelectedIndex+ 1  ;
                if (newSectionLevel != m_node.Level)
                {
                    if (newSectionLevel < m_node.Level)
                    {
                                                for (int i = m_node.Level; i > newSectionLevel; i--)
                            m_View.DecreaseSelectedSectionLevel();
                    }
                    else
                    {
                        for (int i = m_node.Level; i < newSectionLevel; i++)
                            m_View.IncreaseSelectedSectionNodeLevel();
                    }
                }
            }

            private void m_btnCancel_Click(object sender, EventArgs e)
            {
                Close();
            }
    }
}