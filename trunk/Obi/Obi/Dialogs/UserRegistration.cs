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
        public UserRegistration()
        {
            InitializeComponent();
        }

        public static void UploadUserInformation()
        {
            //return;
            string dataToUpload = "un:User Name,Email:abc@abc.com,org:Organization Name,ct:Delhi,con:India";
            
            System.IO.MemoryStream memoryStream = null;
            System.IO.Stream  stream = null;
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
                
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString() );
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
            MessageBox.Show("done");
        }

        private void m_btnOk_Click(object sender, EventArgs e)
        {
            string fileName = System.DateTime.Now.ToShortDateString() + System.DateTime.Now.ToShortTimeString() + System.DateTime.Now.ToUniversalTime().ToShortTimeString();
            fileName = fileName.Replace(":", "") + ".txt";
            fileName = fileName.Replace("/", "");
            ///MessageBox.Show(fileName);
        }


    }
}