using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        }

        public UserRegistration(Settings settings)
            : this()
        {
            m_Settings = settings;
        }

        public static void UploadUserInformation(Settings settings)
        {
            
            
            if (settings == null || m_BackgroundWorker.IsBusy) return;
            m_Settings = settings;

            m_BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(delegate(object sender, System.ComponentModel.DoWorkEventArgs e)
            {
                
                string dataToUpload = settings.UsersInfoToUpload;
                System.IO.MemoryStream memoryStream = null;
                System.IO.Stream stream = null;
                try
                {
                    string fileName = System.DateTime.Now.ToShortDateString() + System.DateTime.Now.ToShortTimeString() + System.DateTime.Now.ToUniversalTime().ToShortTimeString();
                    fileName = fileName.Replace(":", "") + ".txt";
                    fileName = fileName.Replace("/", "");

                    byte[] byteArray = Encoding.UTF8.GetBytes(dataToUpload);
                    memoryStream = new System.IO.MemoryStream(byteArray);

                    // Create FtpWebRequest object
                    string uploadPath = "ftp://ftp.drivehq.com/users-info/user1.txt";
                    FtpWebRequest ftpRequest = (System.Net.FtpWebRequest)System.Net.FtpWebRequest.Create(new Uri(uploadPath));
                    // Provide username and password
                    string userName = "obi-ftp";
                    string pass = "obi";
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
                    // if successful then update settings for the same
                    m_Settings.UsersInfoToUpload = Registered;
                    m_Settings.SaveSettings();
                }
                catch (System.Exception ex)
                {
                    m_Settings.UploadAttemptsCount = m_Settings.UploadAttemptsCount++;
                    MessageBox.Show(ex.ToString());
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
                MessageBox.Show("done");
            });
            m_BackgroundWorker.RunWorkerAsync();
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            //string fileName = System.DateTime.Now.ToShortDateString() + System.DateTime.Now.ToShortTimeString() + System.DateTime.Now.ToUniversalTime().ToShortTimeString();
            //fileName = fileName.Replace(":", "") + ".txt";
            //fileName = fileName.Replace("/", "");

            // the upload data should be in following format
            // "un:User Name,em:abc@abc.com,og:Organization Name,ct:Delhi,cn:India";
            string userInfo = "un:" + m_txtName.Text
                + ",em:" + m_txtEmail.Text 
                + ",og:" + m_txtOrganizationName.Text 
                + ",ct:" + m_txtCity.Text 
                + ",cn:" + m_txtCountry.Text 
                + ",ss:"+ (m_rdbDaisyProduction.Checked?"used":"not-used") ;
            Console.WriteLine(userInfo);
            m_Settings.UsersInfoToUpload = userInfo;
            m_Settings.SaveSettings();
            //MessageBox.Show(userInfo);
            Close();
        }

        private void m_btnRemindMeLater_Click(object sender, EventArgs e)
        {
            m_Settings.UploadAttemptsCount++;
            Close();
        }


    }
}