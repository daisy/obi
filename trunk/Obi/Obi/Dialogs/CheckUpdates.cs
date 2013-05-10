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
        private string m_CriticalText;
        private string m_GoToWebPageLabel;
        private Settings m_Settings;
        private bool m_IsAutomaticUpdate;
        private BackgroundWorker m_BackgroundWorker = new BackgroundWorker();

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
            // the functions checks the file named latest-release.txt on the server. The format of the file is as follows
            //line 1: keyword to inform if new version is test version. text used is either "test" or "Obi"
            //Line 2: a user friendly line describing which update is available
            //line 3: numeric version number
            // line 4: url of web page of release 
            // line 5: optional key word indicating critical release. It is useful for a critical bug fix etc. Char 'c' is used to indicate critical.
            // line 6: Optional Go to webpage text: char 'd' will change the text to go to download page
            // typical example is as follows
            //Test
//A new test release, Obi 2.6 beta is available.
//2.5.6
//http://www.daisy.org/obi/obi-2.6-test-releases
            //c
            //d
            if (m_BackgroundWorker.IsBusy)
            {
                MessageBox.Show(Localizer.Message("CheckUpdate_UpdateProcessActive"));
                return;
            }
            m_BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
            {
                try
                {
                    string releaseInfoContents = GetReleaseInfoFileContents();
                    //string releaseInfoContents = GetReleaseInfoFileContentsFromLocalFileForTesting();
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
                    if (infoArray.Length > 4) m_CriticalText = infoArray[4];
                    if (infoArray.Length > 5) m_GoToWebPageLabel= infoArray[5];
                    if (m_GoToWebPageLabel != null) m_GoToWebPageLabel = m_GoToWebPageLabel.Trim(' ');
                    Console.WriteLine(releaseInfoContents);
                    if (IsVersionNumberNew())
                    {
                        Console.WriteLine("New version is available");
                    }
                    else if (!m_IsAutomaticUpdate)
                    {
                        MessageBox.Show(string.Format( Localizer.Message("CheckUpdates_NewVersionNotAvailable"),m_Settings.Project_LatestVersionCheckedByUpdate ));
                    }
                }
                catch (System.Exception ex)
                {
                    ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                    Console.WriteLine(ex.ToString());
                }
            });
            m_BackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            {
                if (IsNewVersionAvailable)
                    {   
                        ShowDialogThroughCallBack();
                    }
            });
            m_BackgroundWorker.RunWorkerAsync();
        }

        private  delegate void ShowDialogDelegate () ;
        private void ShowDialogThroughCallBack()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new ShowDialogDelegate(ShowDialogThroughCallBack));
            }
            else
            {
                this.Text = string.Format(Localizer.Message("CheckUpdate_Title"), !string.IsNullOrEmpty(m_TitleText) ? m_TitleText : "");
                if (!string.IsNullOrEmpty(m_LabelText) && m_LabelText.EndsWith(".")) m_LabelText = m_LabelText.Remove(m_LabelText.LastIndexOf('.'));
                mInfoTxtBox.Text = string.Format(Localizer.Message("CheckUpdate_LabelText"), !String.IsNullOrEmpty(m_LabelText) ? m_LabelText : "");
                mInfoTxtBox.AccessibleName = "use arrow keys to read";
                if (!string.IsNullOrEmpty(m_GoToWebPageLabel))
                {
                    m_RdOpenWebPage.Text = m_GoToWebPageLabel.ToLower() == "d" ? Localizer.Message("CheckUpdates_GoToDownloadPage") : 
                        m_GoToWebPageLabel;
                }
                ShowDialog();
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
            System.IO.Stream receiveStream = null;
            string s = null;
            try
            {
                receiveStream = System.IO.File.OpenRead("c:\\latest-release.txt");
                System.IO.StreamReader strReader =
                  new System.IO.StreamReader(receiveStream, Encoding.UTF8);
                s = strReader.ReadToEnd();
            }
            finally
            {
                if(receiveStream != null)  receiveStream.Close();
            }
            return s;
        }

        private bool IsVersionNumberNew()
        {
            string strLocalVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //Console.WriteLine("-" + strLocalVersion.Trim(' ') + "-" + m_AvailableVersion.Trim(' ') + "-");
            if (!string.IsNullOrEmpty(m_CriticalText) && m_CriticalText.ToLower() == "c"
                && m_AvailableVersion.Trim(' ') != strLocalVersion.Trim(' '))
            {
                m_IsNewVersionAvailable = true ;
                return m_IsNewVersionAvailable ;
            }
            int localVersion = GetNumericVersion(strLocalVersion);
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
                ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                Console.WriteLine(ex.ToString());
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}