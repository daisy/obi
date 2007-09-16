using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;

namespace Obi.ProjectView
{
    public partial class StripsView : UserControl
    {
        private Project mProject;


        public StripsView()
        {
            InitializeComponent();
        }

        public Project Project
        {
            set
            {
                mProject = value;
                mProject.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(StripsView_treeNodeAdded);
            }
        }

        void StripsView_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            SectionNode section = e.getTreeNode() as SectionNode;
            if (section != null)
            {
                Strip strip = new Strip();
                strip.Label = section.Label;
                mLayoutPanel.Controls.Add(new Strip());
                // TODO why doesn't this work?
                mLayoutPanel.ScrollControlIntoView(strip);
            }
        }

        private void mLayoutPanel_SizeChanged(object sender, EventArgs e)
        {
            foreach (Control c in mLayoutPanel.Controls)
            {
                c.MinimumSize = new Size(mLayoutPanel.Width, c.MinimumSize.Height);
            }
        }

        internal void InsertSection()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
