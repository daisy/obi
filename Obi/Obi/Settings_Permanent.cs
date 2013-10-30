using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;

namespace Obi
{
    /// <summary>
    /// Persistent application settings.
    /// </summary>
    /// <remarks>It also seems that making a change in the class resets the existing settings.</remarks>
    [Serializable()]
    public class Settings_Permanent
    {

        public string UsersInfoToUpload; //users info is temporarily stored till it is uploaded or timed out
        public int UploadAttemptsCount; // number of times user info upload attempted
        public bool RegistrationComplete;        

        private static readonly string SETTINGS_FILE_NAME = "obi_permanent_settings.xml";

        private static void InitializeDefaultSettings(Settings_Permanent settings)
        {

            settings.UsersInfoToUpload = "NoInfo" ;
            settings.UploadAttemptsCount = 0 ;
            settings.RegistrationComplete = false;
        }

        /// <summary>
        /// Creates a settings object having default values
        /// </summary>
        /// <returns></returns>
        public static Settings_Permanent GetDefaultSettings()
        {
            Settings_Permanent settings = new Settings_Permanent();
            InitializeDefaultSettings(settings);
            return settings;
        }

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        /// <remarks>Errors are silently ignored and default settings are returned.</remarks>
        public static Settings_Permanent GetSettings()
        {
            Settings_Permanent settings = new Settings_Permanent();
            InitializeDefaultSettings(settings);
            
            System.Windows.Forms.MessageBox.Show(GetSettingFilePath());
            //IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                FileStream stream =
                    new FileStream (GetSettingFilePath(), FileMode.Open, FileAccess.Read);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings_Permanent)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception) { }
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            
            FileStream stream =
                new FileStream (GetSettingFilePath(), FileMode.Create, FileAccess.Write);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }


        public void ResetSettingsFile()
        {
            
            FileStream stream =
                new FileStream(GetSettingFilePath(), FileMode.Create, FileAccess.Write);
            InitializeDefaultSettings(this);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

        private static string GetSettingFilePath()
        {
            string appDataDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
            string obiSettingsDir = System.IO.Path.Combine(appDataDir, "Obi");
            if (!System.IO.Directory.Exists(obiSettingsDir)) System.IO.Directory.CreateDirectory(obiSettingsDir);
            string permanentSettingsPath = System.IO.Path.Combine(obiSettingsDir, SETTINGS_FILE_NAME);
            return permanentSettingsPath;
        }

    }
}
