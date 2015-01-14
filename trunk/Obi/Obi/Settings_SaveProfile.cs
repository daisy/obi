using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Saves and loads the user defined profiles to the settings.
    /// </summary>
    [Serializable()]
   public class Settings_SaveProfile : Settings
    {

        public void GetObject(string profileFilePath)
        {
            Settings_SaveProfile settingsObject = new Settings_SaveProfile();
            //Settings.InitializeDefaultSettings(settingsObject);
            
            FileStream fs = new FileStream(profileFilePath, FileMode.OpenOrCreate);
            SoapFormatter soap = new SoapFormatter();
            settingsObject  = (Settings_SaveProfile)soap.Deserialize(fs);
            fs.Close();
        }

        public void Save(string profileFilePath)
        {
            try
            {
                
                FileStream fs = new FileStream(profileFilePath, FileMode.OpenOrCreate);
                SoapFormatter soap = new SoapFormatter();
                soap.Serialize(fs, this);
                
                fs.Close();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

            }
}
