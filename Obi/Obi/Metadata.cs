using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Contain the metadata names.
    /// </summary>
    public class Metadata
    {
        public static readonly string DC_IDENTIFIER = "dc:Identifier";
        public static readonly string DC_LANGUAGE = "dc:Language";
        public static readonly string DC_PUBLISHER = "dc:Publisher";
        public static readonly string DC_TITLE = "dc:Title";
        public static readonly string DTB_GENERATOR = "dtb:generator";
        public static readonly string DTB_NARRATOR = "dtb:narrator";
        public static readonly string DTB_PRODUCED_DATE = "dtb:producedDate";
        public static readonly string DTB_REVISION = "dtb:revision";
        public static readonly string DTB_REVISION_DATE = "dtb:revisionDate";
        public static readonly string OBI_XUK_VERSION = "obi:xukversion";

        // this goes out
        public static readonly string OBI_ASSETS_DIR = "obi:assetsdir";
        public static readonly string OBI_AUDIO_CHANNELS = "obi:audioChannels";
        public static readonly string OBI_BIT_DEPTH = "obi:bitDepth";
        public static readonly string OBI_SAMPLE_RATE = "obi:sampleRate";
    }

    /// <summary>
    /// A metadata item may be required, recommeded or optional.
    /// </summary>
    public enum MetadataOccurrence { Required, Recommended, Optional };

    /// <summary>
    /// Describe an entry: name, occurrence, information.
    /// </summary>
    //TODO: add type checking functions both for entries and additional attributes.
    public class MetadataEntryDescription
    {
        private List<List<string>> mAddlAttrs;   // additional attributes (name/description pairs)
        private string mDescription;             // information about the entry (shown to the user in the metadata panel)
        private string mName;                    // name of the entry
        private MetadataOccurrence mOccurrence;  // entry occurrence
        private bool mRepeatable;                // repeatable property

        private static readonly List<MetadataEntryDescription> DAISY_ENTRIES = new List<MetadataEntryDescription>(0);

        /// <summary>
        /// Create a new entry description with no additional attributes.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        /// <param name="occurrence">Its occurrence.</param>
        /// <param name="description">Description that will be shown to the user.</param>
        /// <param name="repeatable">Repeatable or not.</param>
        public MetadataEntryDescription(string name, MetadataOccurrence occurrence, string description, bool repeatable)
        {
            mName = name;
            mOccurrence = occurrence;
            mDescription = description;
            mRepeatable = repeatable;
            mAddlAttrs = new List<List<string>>(0);
        }


        /// <summary>
        /// Add a new additional attribute.
        /// </summary>
        /// <param name="name">The name of the additional attribute.</param>
        /// <param name="description">Its description.</param>
        public void AddAddlAttr(string name, string description)
        {
            List<string> attr = new List<string>(2);
            attr.Add(name);
            attr.Add(description);
            mAddlAttrs.Add(attr);
        }

        /// <summary>
        /// Add a new additional attribute with a list of possible values.
        /// </summary>
        /// <param name="name">The name of the additional attribute.</param>
        /// <param name="description">Its description.</param>
        /// <param name="values">The possible values of the attribute.</param>
        public void AddAddlAttr(string name, string description, List<string> values)
        {
            List<string> attr = new List<string>(values.Count + 2);
            attr.Add(name);
            attr.Add(description);
            attr.AddRange(values);
            mAddlAttrs.Add(attr);
        }

        /// <summary>
        /// Description of the entry.
        /// </summary>
        public string Description { get { return mDescription; } }

        /// <summary>
        /// Get hte list of all defined DAISY entries.
        /// </summary>
        /// <returns></returns>
        public static List<MetadataEntryDescription> GetDAISYEntries()
        {
            if (DAISY_ENTRIES.Count == 0) CreateDAISYEntries();
            return DAISY_ENTRIES;
        }
        
        /// <summary>
        /// Name of the entry.
        /// </summary>
        public string Name { get { return mName; } }

        /// <summary>
        /// Occurrence of the entry.
        /// </summary>
        public MetadataOccurrence Occurrence { get { return mOccurrence; } }

        /// <summary>
        /// Repeatable property of the entry.
        /// </summary>
        public bool Repeatable { get { return mRepeatable; } }

        public override string ToString() { return mName; }

        /// <summary>
        /// Create the default DAISY entries.
        /// </summary>
        private static void CreateDAISYEntries()
        {
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Title",
                MetadataOccurrence.Required,
                Localizer.Message("dc_title_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Publisher",
                MetadataOccurrence.Required,
                Localizer.Message("dc_publisher_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Date",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Format",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Identifier",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Language",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Language",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:narrator",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:producer",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Creator",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Subject",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Description",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Contributor",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Source",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Relation",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Coverage",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dc:Rights",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:sourceDate",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:sourceEdition",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:sourcePublisher",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:sourceRights",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:sourceTitle",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                false));
            DAISY_ENTRIES.Add(new MetadataEntryDescription(
                "dtb:revisionDescription",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                false));
        }
    }
}