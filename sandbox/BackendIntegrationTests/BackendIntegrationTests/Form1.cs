using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using UrakawaApplicationBackend;

namespace BackendIntegrationTests
{
    public partial class Form1 : Form
    {
        private AssetManager mManager;

        public Form1()
        {
            InitializeComponent();
            mManager = null;
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "Select a directory for the Asset Manager.";
            if (dialog.ShowDialog() == DialogResult.OK)
           {
                Console.WriteLine("Open: {0}", dialog.SelectedPath);
                mManager = new AssetManager(dialog.SelectedPath);
            }
        }

        private void addAudioAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mManager != null)
            {
                OpenFileDialog dialog = new OpenFileDialog();
                dialog.InitialDirectory = mManager.ProjectDir;
                dialog.Filter = "Wave file (*.wav)|*.wav|Any file|*.*";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    AudioMediaAsset asset = new AudioMediaAsset(dialog.FileName);
                    string[] row = { "Play", asset.Name, asset.Path };
                    dataGridView1.Rows.Add(row);
                    DataGridViewRow gridrow = dataGridView1.Rows[dataGridView1.Rows.Count - 1];
                }
            }
        }
                
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                Console.WriteLine("Play from {0} @ {1}", sender, e.RowIndex);
            }
        }
    }
}