using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace Obi.Dialogs
{
    public partial class UserRegistration : Form
    {
        public static readonly string NoInfo = "NoInfo";
        public static readonly string Registered = "Registered";
        public static readonly int MaxUploadAttemptsAllowed = 10;

        private static Settings m_Settings;
        private static BackgroundWorker m_BackgroundWorker = new BackgroundWorker();

        private UserRegistration()
        {
            InitializeComponent();
            m_txtBoxObiInformation.Text = Localizer.Message("Obi_UserInformationText");
            m_txtBoxObiInformation.Select(0, 0);
        }

        public UserRegistration(Settings settings)
            : this()
        {
            m_Settings = settings;
        }

        public static string GenerateFileName ()
        {
            string fileName = System.DateTime.Now.ToShortDateString().Replace("/", "").Replace(":", "") + "-"
                        + System.DateTime.Now.TimeOfDay.Ticks.ToString() + "-"
                        + System.DateTime.Now.ToUniversalTime().ToShortTimeString().Replace("/", "").Replace(":", "")
                        +".txt";

            return fileName;
        }

        public static void UploadUserInformation(Settings settings)
        {

            if (settings == null || m_BackgroundWorker.IsBusy) return;
            m_Settings = settings;
            string fileName = GenerateFileName();
            string uploadPath = Code.UploadCode.UploadDirectoryPath + fileName;
            string userName = Code.UploadCode.userName;
            string pass = Code.UploadCode.Code;
            string dataToUpload = settings.UsersInfoToUpload;
            bool isUploadSuccessful = false;

            m_BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
            {

                System.IO.MemoryStream memoryStream = null;
                System.IO.Stream stream = null;

                try
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(dataToUpload);
                    memoryStream = new System.IO.MemoryStream(byteArray);

                    // Create FtpWebRequest object
                    FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(new Uri(uploadPath));
                    // Provide username and password
                    ftpRequest.Credentials = new System.Net.NetworkCredential(userName, pass);

                    // after a command is executed, do not keep connection alive 
                    ftpRequest.KeepAlive = false;
                    ftpRequest.Timeout = 30000;
                    // Specify the command to be executed 
                    ftpRequest.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                    ftpRequest.UseBinary = true;

                    ftpRequest.ContentLength = memoryStream.Length;
                    // buffer size is kept 2kb. 
                    int bufferLength = 2048;
                    byte[] buff = new byte[bufferLength];

                    stream = ftpRequest.GetRequestStream();
                    // Read the memory stream 2kb at a time and copy to ftpStream
                    int dataLength = memoryStream.Read(buff, 0, bufferLength);

                    while (dataLength != 0)
                    {
                        // Write Content from the memory stream to the FTP Upload Stream. 
                        stream.Write(buff, 0, dataLength);
                        dataLength = memoryStream.Read(buff, 0, bufferLength);
                    }
                    isUploadSuccessful = true;
                }
                catch (System.Exception ex)
                {
                    m_Settings.UploadAttemptsCount++;
                    m_Settings.SaveSettings();
                    Console.WriteLine(ex.ToString());
                    ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                }
                finally
                {
                    if (stream != null)
                    {
                        stream.Close();
                        stream.Dispose();
                    }
                    if (memoryStream != null)
                    {
                        memoryStream.Close();
                        memoryStream.Dispose();
                    }
                }
            });

            m_BackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(delegate(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
            {
                if (!isUploadSuccessful) return;
                try
                {
                    FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(new Uri(uploadPath));
                    ftpRequest.Credentials = new System.Net.NetworkCredential(userName, pass);
                    ftpRequest.KeepAlive = false;
                    ftpRequest.Timeout = 20000;
                    ftpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                    ftpRequest.UseBinary = true;

                    // if successful then update settings for the same

                    if (ftpRequest.GetResponse().ContentLength > 0)
                    {
                        Console.WriteLine("registered");
                        m_Settings.UsersInfoToUpload = Registered;
                        m_Settings.SaveSettings();
                        //MessageBox.Show("done");
                    }
                    else
                    {
                        m_Settings.UploadAttemptsCount++;
                        m_Settings.SaveSettings();
                    }
                }
                catch (System.Exception ex)
                {
                    m_Settings.UploadAttemptsCount++;
                    m_Settings.SaveSettings();
                    Console.WriteLine(ex.ToString());
                    ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
                }
            });
            m_BackgroundWorker.RunWorkerAsync();
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            //string fileName = System.DateTime.Now.ToShortDateString() + System.DateTime.Now.ToShortTimeString() + System.DateTime.Now.ToUniversalTime().ToShortTimeString();
            //fileName = fileName.Replace(":", "") + ".txt";
            //fileName = fileName.Replace("/", "");

            // the upload data should be in following format
            // "un:User Name,em:abc@abc.com,og:Organization Name,ct:Delhi,cn:India",vr:Obi version,os:windows version;
              //if (m_txtName.Text == "" || m_txtEmail.Text == "" || m_txtCountry.Text == "" || m_txtOrganizationName.Text == "" ||
              //  m_txtCity.Text == "" || m_txtCountry.Text == "" || ((m_txtName.Text.Contains(" ")) && m_txtName.Text.Trim().Length==0 )
              //  || ((m_txtEmail.Text.Contains(" ")) && m_txtEmail.Text.Trim().Length == 0) 
              //  || ((m_txtOrganizationName.Text.Contains(" ")) && m_txtOrganizationName.Text.Trim().Length == 0)
              //   || ((m_txtCity.Text.Contains(" ")) && m_txtCity.Text.Trim().Length == 0))
            bool flag = false;            
                string str="Please Enter the Following correctly :";
                if (m_txtName.Text == "" || ((m_txtName.Text.Contains(" ")) && m_txtName.Text.Trim().Length==0 ) || (m_txtName.Text.StartsWith(".") && m_txtName.Text.EndsWith(".")))
                {
                    str += "\nName";
                    flag = true;
                }
                if (m_txtEmail.Text == "" || ((m_txtEmail.Text.Contains(" ")) && m_txtEmail.Text.Trim().Length == 0) || (m_txtEmail.Text.StartsWith(".") && m_txtEmail.Text.EndsWith(".")))
                {
                    str += "\nEmail";
                    flag = true;
                }
                if (m_txtOrganizationName.Text == "" || ((m_txtOrganizationName.Text.Contains(" ")) && m_txtOrganizationName.Text.Trim().Length == 0) || (m_txtOrganizationName.Text.StartsWith(".") && m_txtOrganizationName.Text.EndsWith(".")))
                {
                    str += "\nOrganization Name";
                    flag = true;
                }
                if (m_txtCity.Text == "" || ((m_txtCity.Text.Contains(" ")) && m_txtCity.Text.Trim().Length == 0) || (m_txtCity.Text.StartsWith(".") && m_txtCity.Text.EndsWith(".")))
                {
                    str += "\nCity";
                    flag = true;
                }
                if (m_txtCountry.Text == "" || ((m_txtCountry.Text.Contains(" ")) && m_txtCountry.Text.Trim().Length == 0) || (m_txtCountry.Text.StartsWith(".") && m_txtCountry.Text.EndsWith(".")))
                {
                    str += "\nCountry";
                    flag = true;
                }
                if (flag)
                {
                    MessageBox.Show(str);
                }


                else if (!m_txtEmail.Text.Contains("@") || !m_txtEmail.Text.Contains(".") || m_txtEmail.Text.Contains(" "))
               {
                MessageBox.Show("Incorrect Email Address");
               }
               else if ((m_rdbDaisyProduction.Checked == false && m_rdbTryingObi.Checked == false))
               {
                  MessageBox.Show("Please indicate us about your usage of Obi by checking any one of the radio button ");
               }
              else
              {
                  string userInfo = "un:" + m_txtName.Text
                                    + ",em:" + m_txtEmail.Text
                                    + ",og:" + m_txtOrganizationName.Text
                                    + ",ct:" + m_txtCity.Text
                                    + ",cn:" + m_txtCountry.Text
                                    + ",ss:" + (m_rdbDaisyProduction.Checked ? "used" : "not-used")
                                    + ",vr:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString()
                                    + ",os:" + System.Environment.OSVersion.ToString();
                  
                  Console.WriteLine(userInfo);
                  m_Settings.UsersInfoToUpload = userInfo;
                  m_Settings.SaveSettings();
                  //MessageBox.Show(userInfo);
                  Close();
                  //return;
              }
        }

        private void m_btnRemindMeLater_Click(object sender, EventArgs e)
        {
            m_Settings.UploadAttemptsCount++;
            m_Settings.SaveSettings();
            Console.WriteLine("attempts count " + m_Settings.UploadAttemptsCount);
            Close();
        }

      
        public static void OpenEmailToSend(Settings settings)
        {
            m_Settings = settings;
            try
            {
                String tempStr = "mailto:Obi.feedback@gmail.com? Subject=My Details &body=";
                tempStr += m_Settings.UsersInfoToUpload;
                Process.Start(tempStr);
            }
            catch (System.Exception ex)
            {
                ProjectView.ProjectView.WriteToLogFile_Static(ex.ToString());
            }
        }


    }
}