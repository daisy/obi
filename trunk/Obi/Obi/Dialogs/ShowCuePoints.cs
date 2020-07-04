using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class ShowCuePoints : Form
    {
        private string[] m_CueLabels;
        private List<decimal> m_CuePoints;
        private List<string> m_FilePaths;

        public ShowCuePoints(List<string> filePaths)
        {
            InitializeComponent();
            m_FilePaths = filePaths;
            int count = 1;
            foreach (string path in m_FilePaths)
            {

                DisplayCuePoints(path,count);
                count++;
            }
        }


        public void DisplayCuePoints(string path,int audiofileCount)
        {
            AudioLib.ReadCueMarkers readCues = new AudioLib.ReadCueMarkers(path);
            m_CueLabels = readCues.ListOfCueLabels;
            m_CuePoints = readCues.ListOfCuePoints;
            if (m_CuePoints == null || m_CuePoints.Count == 0)
            {
                MessageBox.Show("There are no cue points in the audio files being imported");
                return;
            }
            int count = 0;
            m_CuePointsListView.View = View.Details;
            m_CuePointsListView.GridLines = true;
            m_CuePointsListView.FullRowSelect = true;

            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { "Audio "+audiofileCount.ToString() +" :"  , string.Empty }));

            foreach (decimal cupoint in m_CuePoints)
            {
                
                m_CuePointsListView.Items.Add(new ListViewItem(new string[] { m_CuePoints[count].ToString(), m_CueLabels[count] }));

                count++;
            }

        }

    }
}
