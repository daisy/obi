using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using urakawa.project;

namespace Obi
{
    /// <summary>
    /// Simple metadata for the project. Most users will want to deal only with this, and this data will serve as the basis to
    /// fill all DC/x-metadata.
    /// </summary>
    public class SimpleMetadata
    {
        public string Identifier;     // identifier of the project
        public string Narrator;       // author of the project
        public string Publisher;      // publisher of the project
        public string Title;          // title of the project
        public CultureInfo Language;  // main language of the project

        public static readonly string MetaTitle = "dc:Title";
        public static readonly string MetaPublisher = "dc:Publisher";
        public static readonly string MetaIdentifier = "dc:Identifier";
        public static readonly string MetaLanguage = "dc:Language";
        public static readonly string MetaNarrator = "dtb:narrator";

        public static readonly string MetaProducedDate = "dtb:producedDate";  // date of first export
        public static readonly string MetaRevision = "dtb:revision";
        public static readonly string MetaRevisionDate = "dtb:revisionDate";  // date of the latest revision (last export)
        
        public static readonly string MetaGenerator = "dtb:generator";
        public static readonly string MetaAssetsDir = "obi:assetsdir";
        public static readonly string MetaXUKVersion = "obi:xukversion";

        public static readonly string MetaAudioChannels = "obi:audioChannels";
        public static readonly string MetaSampleRate = "obi:sampleRate";
        public static readonly string MetaBitDepth = "obi:bitDepth";

        /// <summary>
        /// Create an empty metadata object that will be filled field by field.
        /// </summary>
        public SimpleMetadata()
        {
        }

        /// <summary>
        /// Create a new metadata object from user provided information.
        /// </summary>
        /// <param name="title">Title of the project.</param>
        /// <param name="id">Template for the identifier.</param>
        /// <param name="profile">User profile (for name, publisher and language.)</param>
        public SimpleMetadata(string title, string id, UserProfile profile)
        {
            Identifier = GenerateIdFromTemplate(id);
            Narrator = profile.Name;
            Publisher = profile.Organization;
            Title = title;
            Language = profile.Culture;
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
