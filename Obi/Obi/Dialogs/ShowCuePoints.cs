using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.Dialogs
{
    public partial class ShowCuePoints : Form
    {
        private string[] m_CueLabels;
        private List<decimal> m_CuePoints;
        private string[] m_FilePaths;

        public ShowCuePoints(string[] filePaths)
        {
            InitializeComponent();
            m_FilePaths = filePaths;
            foreach (string path in m_FilePaths)
            {
                DisplayCuePoints(path);
            }
        }


        public void DisplayCuePoints(string path)
        {
            AudioLib.ReadCueMarkers readCues = new AudioLib.ReadCueMarkers(path);
            m_CueLabels = readCues.ListOfCueLabels;
            m_CuePoints = readCues.ListOfCuePoints;
           
            int count = 0;
            m_CuePointsListView.View = View.Details;
            m_CuePointsListView.GridLines = true;
            m_CuePointsListView.FullRowSelect = true;
            string fileName = Path.GetFileName(path);


            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { string.Empty, string.Empty }));
            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { fileName + " :", string.Empty }));

            if (m_CuePoints == null || m_CuePoints.Count == 0)
            {
                string message;
                if (Path.GetExtension(path).ToLower() != ".wav")
                {
                    string filename = Path.GetFileName(path);
                    message = "Show cue points work only with wave files";
                }
                else
                {
                   message = "No Cues available";
                }
                m_CuePointsListView.Items.Add(new ListViewItem(new string[] { message, string.Empty }));
                return;
            }

            foreach (decimal cupoint in m_CuePoints)
            {
                
                m_CuePointsListView.Items.Add(new ListViewItem(new string[] { m_CuePoints[count].ToString(), m_CueLabels[count] }));

                count++;
            }

        }

    }
}
