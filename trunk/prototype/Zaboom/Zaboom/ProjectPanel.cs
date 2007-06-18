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

        public ProjectPanel()
        {
            InitializeComponent();
            project = null;
        }

        public Project Project
        {
            get { return project; }
            set
            {
                if (project == null)
                {
                    project = value;
                }
            }
        }

        internal void AddWavPanel()
        {
            flowLayout.Controls.Add(new WaveformPanel());
        }
    }
}
