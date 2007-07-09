using System;
using System.Threading;
using System.Globalization;

namespace Obi
{
    /// <summary>
    /// The user profile stores basic information about the user.
    /// This information is used to provide default values for project metadata.
    /// </summary>
    [Serializable]
    public class UserProfile
    {
        public string Name;          // user name
        public string Organization;  // user organization
        public CultureInfo Culture;  // user language

        /// <summary>
        /// Create a new user profile from the OS settings.
        /// </summary>
        public UserProfile()
        {
            Name = Environment.UserName;
            Organization = Localizer.Message("default_organization");
            Culture = Thread.CurrentThread.CurrentCulture;
        }
    }
}