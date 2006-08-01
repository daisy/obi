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
    /// Various persistent application settings.
    /// This is mostly a glorified struct.
    /// We mark this class as serializable so that it can be easily serialized through the SOAP serializer
    /// (this is probably overkill, but it's available.)
    /// </summary>
    /// <remarks>It seems that the recent list is not saved, have to investigate...</remarks>
    /// <remarks>It also seems that making a change in the class resets the existing settings?</remarks>
    [Serializable()]
    public class Settings
    {
        public ArrayList RecentProjects;  // paths to projects recently opened
        public UserProfile UserProfile;   // the user profile
        public string IdTemplate;         // identifier template
        public string DefaultPath;        // default location
        public bool CreateTitleSection;   // defaulf for "create title section" in new project

        public string LastOutputDevice;   // the name of the last output device selected by the user
        public string LastInputDevice;    // the name of the last input device selected by the user
        public int AudioChannels;         // number of channels for recording
        public int SampleRate;            // sample rate in Hertz
        public int BitDepth;              // sample bit depth

        public static readonly string SettingsFileName = "obi_settings.xml";  // settings file name

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        public static Settings GetSettings()
        {
            Settings settings;
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SettingsFileName, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings)soap.Deserialize(stream);
                stream.Close();
            }
            catch (Exception)
            {
                settings = new Settings();
            }
            if (settings.RecentProjects == null) settings.RecentProjects = new ArrayList();
            if (settings.UserProfile == null) settings.UserProfile = new UserProfile();
            if (settings.IdTemplate == null) settings.IdTemplate = "obi_####";
            if (settings.DefaultPath == null) settings.DefaultPath = Environment.CurrentDirectory;
            if (settings.LastOutputDevice == null) settings.LastOutputDevice = "";
            if (settings.LastInputDevice == null) settings.LastInputDevice = "";
            if (settings.AudioChannels == 0) settings.AudioChannels = 1;
            if (settings.SampleRate == 0) settings.SampleRate = 44000;
            if (settings.BitDepth == 0) settings.BitDepth = 16;
            return settings;
        }

        /// <summary>
        /// Save the settings when closing.
        /// </summary>
        public void SaveSettings()
        {
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

    }
}
