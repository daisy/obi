using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using UrakawaApplicationBackend;
using UrakawaApplicationBackend.events.assetManagerEvents;

namespace Zaboom
{
    public partial class Zaboom : Form
    {
        private Project mProject;

        public Zaboom()
        {
            InitializeComponent();
            AssetManager assmanager = new AssetManager(System.IO.Path.GetTempPath());
            assmanager.AssetRenamedEvent += new DAssetRenamedEvent(OnAssetRenamedEvent);
            mProject = new Project(assmanager);
        }

        #region Menu Items

        private void importAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Audio file (*.wav)|*.wav|Any file|*.*";
            dialog.Multiselect = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                foreach (string filename in dialog.FileNames)
                {
                    string name = mProject.AddFile(filename);
                    mAssBox.Items.Add(name);
                    mAssBox.SelectedIndex = mAssBox.Items.Count - 1;
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #endregion

        #region Event handlers

        public void OnAssetRenamedEvent(object sender, AssetRenamed e)
        {
        }

        #endregion
    }
}