using System;
using System.Collections;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Text;
using System.Text.RegularExpressions;

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
        public string DefaultExportPath;  // default path for DAISY export
        public bool CreateTitleSection;   // defaulf for "create title section" in new project
        public string LastOpenProject;    // path to the last open project
        public bool OpenLastProject;      // open the last open project at startup
        public bool EnableTooltips;       // enable or disable tooltips

        public string LastOutputDevice;   // the name of the last output device selected by the user
        public string LastInputDevice;    // the name of the last input device selected by the user
        public int AudioChannels;         // number of channels for recording
        public int SampleRate;            // sample rate in Hertz
        public int BitDepth;              // sample bit depth

        public float FontSize;            // global font size (all font sizes must be relative to this one)

        public static readonly string SettingsFileName = "obi_settings.xml";  // settings file name

        /// <summary>
        /// Read the settings from the settings file; missing values are replaced with defaults.
        /// </summary>
        public static Settings GetSettings()
        {
            Settings settings = new Settings();
            settings.RecentProjects = new ArrayList();
            settings.UserProfile = new UserProfile();
            settings.IdTemplate = "obi_####";
            settings.DefaultPath = Environment.CurrentDirectory;
            settings.DefaultExportPath = Environment.CurrentDirectory;
            settings.LastOpenProject = "";
            settings.LastOutputDevice = "";
            settings.LastInputDevice = "";
            settings.AudioChannels = 1;
            settings.SampleRate = 44100;
            settings.BitDepth = 16;
            settings.FontSize = 10.0f;
            settings.EnableTooltips = true;
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            try
            {
                IsolatedStorageFileStream stream =
                    new IsolatedStorageFileStream(SettingsFileName, FileMode.Open, FileAccess.Read, file);
                SoapFormatter soap = new SoapFormatter();
                settings = (Settings)soap.Deserialize(stream);
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
            IsolatedStorageFile file = IsolatedStorageFile.GetUserStoreForDomain();
            IsolatedStorageFileStream stream =
                new IsolatedStorageFileStream(SettingsFileName, FileMode.Create, FileAccess.Write, file);
            SoapFormatter soap = new SoapFormatter();
            soap.Serialize(stream, this);
            stream.Close();
        }

        public string GeneratedID
        {
            get
            {
                string id = IdTemplate;
                Random rand = new Random();
                Regex regex = new Regex("#");
                while (id.Contains("#"))
                {
                    id = regex.Replace(id, String.Format("{0}", rand.Next(0, 10)), 1);
                }
                return id;
            }
        }
    }
}
