using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Obi
{
    /// <summary>
    /// Project metadata.
    /// </summary>
    public class Metadata
    {
        private string mId;     // unique identifier
        private List<string> mTitles;  // titles of the presentation/project

        public string Id
        {
            get
            {
                return mId;
            }
            set
            {
                mId = value;
            }
        }

        /// <summary>
        /// Return the project title, which is the first presentation title.
        /// </summary>
        public string Title
        {
            get
            {
                return mTitles.Count == 0 ? Localizer.Message("untitled") : mTitles[0];
            }
        }

        public List<string> Titles
        {
            get
            {
                return mTitles;
            }
        }

        /// <summary>
        /// Create a new metadata object.
        /// </summary>
        public Metadata(string id, string title, UserProfile userProfile)
        {
            mId = GenerateIdFromTemplate(id);
            mTitles = new List<string>();
            mTitles.Add(title);
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
