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
        private Dictionary<string, List<double>> m_CuePointsDictionary;

        public ShowCuePoints(Dictionary<string, List<double>> cuePointsDictionary)
        {
            InitializeComponent();

            m_CuePointsDictionary = cuePointsDictionary;
            DisplayCuePoints();
        }

        public void DisplayCuePoints()
        {
            foreach (KeyValuePair<string, List<double>> entry in m_CuePointsDictionary)
            {
                string path = entry.Key;
                AudioLib.ReadWriteCueMarkers readCues = new AudioLib.ReadWriteCueMarkers();
                readCues.ReadCues(path);
                List<double> CuePoints = entry.Value;
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

                    foreach (double tempCuePoint in CuePoints)
                    {
                         double CuePointInSeconds = tempCuePoint / 1000;
                        
                        if (CueLabels != null)
                            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { CuePointInSeconds.ToString(), CueLabels[count] }));
                        else
                        {
                            m_CuePointsListView.Items.Add(new ListViewItem(new string[] { CuePointInSeconds.ToString(), string.Empty }));
                        }

                        count++;
                    }

                }
            }
        }

    }
}
