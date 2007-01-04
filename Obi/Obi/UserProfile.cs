using System;
using System.Globalization;

namespace Obi
{
    /// <summary>
    /// The user profile stores basic information about the user.
    /// This information is used to provide default values for book metadata.
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
            Culture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Short string version of the user profile,
        /// e.g. "David Brent @Wernham Hogg [en-UK]"
        /// </summary>
        public override string ToString()
        {
            return String.Format(Localizer.Message("user_profile_template"),
                Name, Organization == null ? "" : " @" + Organization, Culture);
        }
    }
}