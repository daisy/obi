using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class CheckUpdates : Form
    {
        private string m_TitleText;
        private string m_LabelText;
        private string m_AvailableVersion;
        private string m_ReleaseUrl;
        private Settings m_Settings;
        private bool m_IsAutomaticUpdate;

        private CheckUpdates()
        {
            InitializeComponent();
        }

        public CheckUpdates(Settings settings, bool isAutomaticUpdate):this ()
        {
            m_Settings = settings;
            m_IsAutomaticUpdate = isAutomaticUpdate;
            m_IsNewVersionAvailable = false ;
        }

        private bool m_IsNewVersionAvailable;
        public bool IsNewVersionAvailable { get { return m_IsNewVersionAvailable; } }

        public void CheckForAvailableUpdate()
        {
            try
            {
                string releaseInfoContents = GetReleaseInfoFileContents();
                if (string.IsNullOrEmpty(releaseInfoContents))
                {
                    m_IsNewVersionAvailable = false;
                    return;
                }
                string[] infoArray = releaseInfoContents.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (infoArray.Length > 0) m_TitleText = infoArray[0];
                if (infoArray.Length > 1) m_LabelText = infoArray[1];
                if (infoArray.Length > 2) m_AvailableVersion = infoArray[2];
                if (infoArray.Length > 3) m_ReleaseUrl = infoArray[3];

                //MessageBox.Show(s + " : " + infoArray.Length.ToString ());
                if (IsVersionNumberNew())
                {
                    this.Text = string.Format(Localizer.Message("CheckUpdate_Title"), !string.IsNullOrEmpty(m_TitleText) ? m_TitleText : "");
                    m_lblInfo.Text = string.Format(Localizer.Message("CheckUpdate_LabelText"), !String.IsNullOrEmpty(m_LabelText) ? m_LabelText : "");
                    this.Show();
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private string GetReleaseInfoFileContents()
        {
            Uri releaseInfoFileUri = new Uri("http://data.daisy.org/projects/obi/releases/latest-release.txt");
            System.Net.HttpWebRequest request = (System.Net.HttpWebRequest)
                System.Net.WebRequest.Create(releaseInfoFileUri);
            System.Net.HttpWebResponse response =
              (System.Net.HttpWebResponse)request.GetResponse();
            string s = null;
            try
            {
                System.IO.Stream receiveStream = response.GetResponseStream();
                System.IO.StreamReader strReader =
                  new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                s = strReader.ReadToEnd();
            }
            finally
            {
                response.Close();
            }
            return s;
        }

        private string GetReleaseInfoFileContentsFromLocalFileForTesting()
        {
            System.IO.Stream receiveStream = System.IO.File.OpenRead("c:\\release-info.txt");
            System.IO.StreamReader strReader =
              new System.IO.StreamReader(receiveStream, Encoding.UTF8);
            string s = strReader.ReadToEnd();
            receiveStream.Close();
            return s;
        }

        private bool IsVersionNumberNew()
        {
            int localVersion = GetNumericVersion(System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            if (m_IsAutomaticUpdate)
            {
                int settingsVersion = GetNumericVersion(m_Settings.Project_LatestVersionCheckedByUpdate);
                if (settingsVersion > localVersion) localVersion = settingsVersion;
            }
            int newVersion = GetNumericVersion(m_AvailableVersion);

            m_IsNewVersionAvailable =  (newVersion > localVersion);
            return m_IsNewVersionAvailable;
        }

        private int GetNumericVersion(string strVersion)
        {
            if (strVersion.Split('.').Length > 3)
            {
                int lastPeriod = strVersion.LastIndexOf('.');
                strVersion = strVersion.Substring(0, lastPeriod);
            }
            Console.WriteLine(strVersion);
            strVersion = strVersion.Replace(".", "");
            strVersion = strVersion.Length == 1 ? strVersion + "00" : strVersion.Length == 2 ? strVersion + "0" : strVersion;
            Console.WriteLine(strVersion);
            int numericVersion = 0 ;
            int.TryParse(strVersion, out numericVersion);
            Console.WriteLine(numericVersion);
            return numericVersion;
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                if (m_RdOpenWebPage.Checked)
                {
                    if (!string.IsNullOrEmpty(m_ReleaseUrl))
                    {
                        System.Diagnostics.Process.Start(m_ReleaseUrl);
                        m_Settings.Project_LatestVersionCheckedByUpdate = m_AvailableVersion;
                        m_Settings.SaveSettings();
                    }
                }
                    else if (m_RdRemindLater.Checked)
                    {
                        // allow it close the window
                    }
                    else if (m_RdRemindForNextVersion.Checked)
                    {
                        m_Settings.Project_LatestVersionCheckedByUpdate = m_AvailableVersion;
                        m_Settings.SaveSettings();
                    }
                    else if (m_RdDisableCheckUpdates.Checked)
                    {
                        m_Settings.Project_CheckForUpdates = false;
                        m_Settings.SaveSettings();
                    }
                    Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}