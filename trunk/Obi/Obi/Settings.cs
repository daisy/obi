using System;
using System.Collections;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Various persistent application settings.
    /// </summary>
    [Serializable()]
    public class Settings
    {
        public ArrayList RecentProjects;  // paths to projects recently opened
        public UserProfile UserProfile;   // the user profile
        public string IdTemplate;         // identifier template
        public string DefaultPath;        // default location


    }
}
