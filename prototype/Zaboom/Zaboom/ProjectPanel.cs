using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Zaboom
{
    public partial class ProjectPanel : UserControl
    {
        private Project project;
        private int pixelsPerSecond;

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            pixelsPerSecond = WaveformPanel.DEFAULT_PIXELS_PER_SECOND;
        }

        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null && value != null)
                {
                    project = value;
                    project.TreeNodeAdded += new TreeNodeAddedHandler(project_TreeNodeAdded);
                    project.Resync();
                }
            }
        }

        public int PixelsPerSecond
        {
            get { return pixelsPerSecond; }
            set
            {
                pixelsPerSecond = value;
                foreach (Control c in flowLayout.Controls)
                {
                    WaveformPanel panel = c as WaveformPanel;
                    if (panel != null) panel.PixelsPerSecond = pixelsPerSecond;
                }
            }
        }

        private void project_TreeNodeAdded(object sender, TreeNodeEventArgs e)
        {
            WaveformPanel panel = new WaveformPanel(e.Project, e.Node, pixelsPerSecond);
            flowLayout.Controls.Add(panel);
        }
    }
}
