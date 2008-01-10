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
        private bool mReadOnly;                  // cannot be modified

        private static readonly Dictionary<string, MetadataEntryDescription> DAISY_ENTRIES = new Dictionary<string, MetadataEntryDescription>();

        /// <summary>
        /// Create a new entry description with no additional attributes.
        /// </summary>
        /// <param name="name">Name of the entry.</param>
        /// <param name="occurrence">Its occurrence.</param>
        /// <param name="description">Description that will be shown to the user.</param>
        /// <param name="repeatable">Repeatable or not.</param>
        /// <param name="readOnly">Can be modified or not.</param>
        public MetadataEntryDescription(string name, MetadataOccurrence occurrence, string description, bool repeatable, bool readOnly)
        {
            mName = name;
            mOccurrence = occurrence;
            mDescription = description;
            mRepeatable = repeatable;
            mReadOnly = readOnly;
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
        /// Add a new custom description to the list.
        /// </summary>
        public static void AddCustomEntry(MetadataEntryDescription d)
        {
            if (DAISY_ENTRIES.Count == 0) CreateDAISYEntries();
            DAISY_ENTRIES[d.Name] = d;
        }

        /// <summary>
        /// Description of the entry.
        /// </summary>
        public string Description { get { return mDescription; } }

        /// <summary>
        /// Get the hash of all defined DAISY entries.
        /// </summary>
        public static Dictionary<string, MetadataEntryDescription> GetDAISYEntries()
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
        /// Read only flag.
        /// </summary>
        public bool ReadOnly { get { return mReadOnly; } }

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
            DAISY_ENTRIES[Metadata.DC_TITLE] = new MetadataEntryDescription(
                Metadata.DC_TITLE,
                MetadataOccurrence.Required,
                Localizer.Message("dc_title_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_PUBLISHER] = new MetadataEntryDescription(
                Metadata.DC_PUBLISHER,
                MetadataOccurrence.Required,
                Localizer.Message("dc_publisher_description"),
                true, false);
            DAISY_ENTRIES["dc:Date"] = new MetadataEntryDescription(
                "dc:Date",
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_IDENTIFIER] = new MetadataEntryDescription(
                Metadata.DC_IDENTIFIER,
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_LANGUAGE] = new MetadataEntryDescription(
                Metadata.DC_LANGUAGE,
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DTB_NARRATOR] = new MetadataEntryDescription(
                Metadata.DTB_NARRATOR,
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dtb:producer"] = new MetadataEntryDescription(
                "dtb:producer",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Creator"] = new MetadataEntryDescription(
                "dc:Creator",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Subject"] = new MetadataEntryDescription(
                "dc:Subject",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Description"] = new MetadataEntryDescription(
                "dc:Description",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Contributor"] = new MetadataEntryDescription(
                "dc:Contributor",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Source"] = new MetadataEntryDescription(
                "dc:Source",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Relation"] = new MetadataEntryDescription(
                "dc:Relation",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Coverage"] = new MetadataEntryDescription(
                "dc:Coverage",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dc:Rights"] = new MetadataEntryDescription(
                "dc:Rights",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                true, false);
            DAISY_ENTRIES["dtb:sourceDate"] = new MetadataEntryDescription(
                "dtb:sourceDate",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES["dtb:sourceEdition"] = new MetadataEntryDescription(
                "dtb:sourceEdition",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES["dtb:sourcePublisher"] = new MetadataEntryDescription(
                "dtb:sourcePublisher",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES["dtb:sourceRights"] = new MetadataEntryDescription(
                "dtb:sourceRights",
                MetadataOccurrence.Recommended,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES["dtb:sourceTitle"] = new MetadataEntryDescription(
                "dtb:sourceTitle",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES["dtb:revisionDescription"] = new MetadataEntryDescription(
                "dtb:revisionDescription",
                MetadataOccurrence.Optional,
                Localizer.Message("missing_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_GENERATOR] = new MetadataEntryDescription(
                Metadata.DTB_GENERATOR,
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                false, true);
            DAISY_ENTRIES[Metadata.OBI_XUK_VERSION] = new MetadataEntryDescription(
                Metadata.OBI_XUK_VERSION,
                MetadataOccurrence.Required,
                Localizer.Message("missing_description"),
                false, true);
        }
    }
}