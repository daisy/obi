using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Obi
{
    public class UserProfile
    {
        private string mName;
        private string mOrganization;
        private CultureInfo mLanguage;
        private string mIdTemplate;

        public UserProfile()
        {
            mName = null;
            mOrganization = null;
            mLanguage = null;
            mIdTemplate = null;
        }
    }
}