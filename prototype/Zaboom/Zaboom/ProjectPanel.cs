using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using urakawa.core.events;
using urakawa.undo;

namespace Zaboom
{
    public partial class ProjectPanel : UserControl
    {
        private Project project;
        private CommandManager commandManager;
        private int pixelsPerSecond;

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
            commandManager = new CommandManager();
            pixelsPerSecond = WaveformPanel.DEFAULT_PIXELS_PER_SECOND;
            transportBar.Enabled = false;  // let's wait for a working audio player
        }

        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null && value != null)
                {
                    project = value;
                    project.getPresentation().treeNodeAdded += new TreeNodeAddedEventHandler(project_treeNodeAdded);
                    flowLayout.Controls.Add(new DummyBlock());
                }
            }
        }

        public CommandManager CommandManager { get { return commandManager; } }

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

        private void project_treeNodeAdded(ITreeNodeChangedEventManager o, TreeNodeAddedEventArgs e)
        {
            WaveformPanel panel = new WaveformPanel(project, e.getTreeNode(), pixelsPerSecond);
            flowLayout.Controls.Add(panel);
            flowLayout.Controls.SetChildIndex(panel, flowLayout.Controls.Count - 2);
        }
    }
}
