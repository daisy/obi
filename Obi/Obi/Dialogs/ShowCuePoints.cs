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
        private Dictionary<string, double[]> m_CuePointsDictionary;

        public ShowCuePoints(Dictionary<string, double[]> cuePointsDictionary)
        {
            InitializeComponent();

            m_CuePointsDictionary = cuePointsDictionary;
            DisplayCuePoints();
        }

        public void DisplayCuePoints()
        {
            foreach (KeyValuePair<string, double[]> entry in m_CuePointsDictionary)
            {
                string path = entry.Key;
                AudioLib.ReadCueMarkers readCues = new AudioLib.ReadCueMarkers(path);
                double[] CuePoints = entry.Value;
                string[] CueLabels = readCues.ListOfCueLabels;

                int count = 0;
                m_CuePointsListView.View = View.Details;
                m_CuePointsListView.GridLines = true;
                m_CuePointsListView.FullRowSelect = true;
                string fileName = Path.GetFileName(path);


                m_CuePointsListView.Items.Add(new ListViewItem(new string[] { string.Empty, string.Empty }));
                m_CuePointsListView.Items.Add(new ListViewItem(new string[] { fileName + " :", string.Empty }));

                if (CuePoints == null || CuePoints.Count() == 0)
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

                }
                else
                {

                    foreach (decimal cupoint in CuePoints)
                    {
                        if (CueLabels != null)
                            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { CuePoints[count].ToString(), CueLabels[count] }));
                        else
                        {
                            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { CuePoints[count].ToString(), string.Empty }));
                        }

                        count++;
                    }

                }
            }
        }

    }
}
