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
        private Audio.Player player;

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            player = new Audio.Player();
            player.SetOutputDevice(this);
        }

        public Audio.Player Player { get { return player; } }

        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null && value != null)
                {
                    project = value;
                    project.TreeNodeAdded += new TreeNodeAddedHandler(project_TreeNodeAdded);
                }
            }
        }

        void project_TreeNodeAdded(object sender, TreeNodeEventArgs e)
        {
            WaveformPanel panel = new WaveformPanel(e.Project, e.Node);
            flowLayout.Controls.Add(panel);
        }

        internal void AddWavPanel()
        {
            flowLayout.Controls.Add(new WaveformPanel());
        }
    }
}
