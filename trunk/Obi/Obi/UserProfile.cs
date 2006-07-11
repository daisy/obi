using System;
using System.Collections.Generic;
using System.Text;
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
        private string mName;          // user name
        private string mOrganization;  // user organization
        private CultureInfo mCulture;  // user language

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        public string Organization
        {
            get
            {
                return mOrganization;
            }
            set
            {
                mOrganization = value;
            }
        }

        public CultureInfo Culture
        {
            get
            {
                return mCulture;
            }
            set
            {
                mCulture = value;
            }
        }

        /// <summary>
        /// Create a new user profile from the OS settings.
        /// </summary>
        public UserProfile()
        {
            mName = Environment.UserName;
            mOrganization = null;
            mCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
        }

        /// <summary>
        /// Short string version of the user profile.
        /// e.g. "David Brent @Wernham Hogg [en-UK]"
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}{1} [{2}]", mName, mOrganization == null ? "" : " @" + mOrganization, mCulture);
        }
    }
}