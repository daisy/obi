using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Obi
{
    /// <summary>
    /// Simple metadata for the project. Most users will want to deal only with this, and this data will serve as the basis to
    /// fill all DC/x-metadata.
    /// </summary>
    public class SimpleMetadata
    {
        private string mIdentifier;     // identifier of the project
        private string mAuthor;         // author of the project
        private string mPublisher;      // publisher of the project
        private string mTitle;          // title of the project
        private CultureInfo mLanguage;  // main language of the project

        public string Identifier { get { return mIdentifier; } set { mIdentifier = value; } }
        public string Author { get { return mAuthor; } set { mAuthor = value; } }
        public string Publisher { get { return mPublisher; } set { mPublisher = value; } }
        public string Title { get { return mTitle; } set { mTitle = value; } }
        public CultureInfo Language { get { return mLanguage; } set { mLanguage = value; } }

        /// <summary>
        /// Create a new metadata object from user provided information.
        /// </summary>
        /// <param name="title">Title of the project.</param>
        /// <param name="id">Template for the identifier.</param>
        /// <param name="profile">User profile (for name, publisher and language.)</param>
        public SimpleMetadata(string title, string id, UserProfile profile)
        {
            mIdentifier = GenerateIdFromTemplate(id);
            mAuthor = profile.Name;
            mPublisher = profile.Organization;
            mTitle = title;
            mLanguage = profile.Culture;
        }

        /// <summary>
        /// Generates a random id from a template by replacing # with digits.
        /// E.g. obi_###### can give obi_045524.
        /// </summary>
        /// <param name="template">The id template.</param>
        /// <returns>The randomly generated id.</returns>
        private static string GenerateIdFromTemplate(string template)
        {
            Random rand = new Random();
            Regex regex = new Regex("#");
            while (template.Contains("#"))
            {
                template = regex.Replace(template, String.Format("{0}", rand.Next(0, 10)), 1);
            }
            return template;
        }
    }
}
