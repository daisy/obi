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
        // Dublin Core metadata
        public static readonly string DC_TITLE = "dc:Title";
        public static readonly string DC_CREATOR = "dc:Creator";
        public static readonly string DC_SUBJECT = "dc:Subject";
        public static readonly string DC_DESCRIPTION = "dc:Description";
        public static readonly string DC_PUBLISHER = "dc:Publisher";
        public static readonly string DC_CONTRIBUTOR = "dc:Contributor";
        public static readonly string DC_DATE = "dc:Date";
        public static readonly string DC_TYPE = "dc:Type";
        public static readonly string DC_FORMAT = "dc:Format";
        public static readonly string DC_IDENTIFIER = "dc:Identifier";
        public static readonly string DC_SOURCE = "dc:Source";
        public static readonly string DC_LANGUAGE = "dc:Language";
        public static readonly string DC_RELATION = "dc:Relation";
        public static readonly string DC_COVERAGE = "dc:Coverage";
        public static readonly string DC_RIGHTS = "dc:Rights";
        // X-Metadata
        public static readonly string DTB_SOURCE_DATE = "dtb:sourceDate";
        public static readonly string DTB_SOURCE_EDITION = "dtb:sourceEdition";
        public static readonly string DTB_SOURCE_PUBLISHER = "dtb:sourcePublisher";
        public static readonly string DTB_SOURCE_RIGHTS = "dtb:sourceRights";
        public static readonly string DTB_SOURCE_TITLE = "dtb:sourceTitle";
        public static readonly string DTB_MULTIMEDIA_TYPE = "dtb:multimediaType";
        public static readonly string DTB_MULTIMEDIA_CONTENT = "dtb:multimediaContent";
        public static readonly string DTB_NARRATOR = "dtb:narrator";
        public static readonly string DTB_PRODUCER = "dtb:producer";
        public static readonly string DTB_PRODUCED_DATE = "dtb:producedDate";
        public static readonly string DTB_REVISION = "dtb:revision";
        public static readonly string DTB_REVISION_DATE = "dtb:revisionDate";
        public static readonly string DTB_REVISION_DESCRIPTION = "dtb:revisionDescription";
        public static readonly string DTB_TOTAL_TIME = "dtb:totalTime";
        public static readonly string DTB_AUDIO_FORMAT = "dtb:audioFormat";

        public static readonly string GENERATOR = "generator";
        public static readonly string OBI_XUK_VERSION = "obi:xukversion";
        public static readonly string OBI_DAISY3ExportPath = "obi:DAISY3.0ExportPath";
        public static readonly string OBI_DAISY2ExportPath = "obi:DAISY2.02ExportPath";
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
        /// Get an entry from the list of DAISY entries, or null if custom.
        /// </summary>
        public static MetadataEntryDescription GetDAISYEntry(string name)
        {
            return GetDAISYEntries().ContainsKey(name) ? GetDAISYEntries()[name] : null;
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
            DAISY_ENTRIES[Metadata.DC_CREATOR] = new MetadataEntryDescription(
                Metadata.DC_CREATOR,
                MetadataOccurrence.Recommended,
                Localizer.Message("dc_creator_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_SUBJECT] = new MetadataEntryDescription(
                Metadata.DC_SUBJECT,
                MetadataOccurrence.Recommended,
                Localizer.Message("dc_subject_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_DESCRIPTION] = new MetadataEntryDescription(
                Metadata.DC_DESCRIPTION,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_description_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_PUBLISHER] = new MetadataEntryDescription(
                Metadata.DC_PUBLISHER,
                MetadataOccurrence.Required,
                Localizer.Message("dc_publisher_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_CONTRIBUTOR] = new MetadataEntryDescription(
                Metadata.DC_CONTRIBUTOR,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_contributor_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_DATE] = new MetadataEntryDescription(
                Metadata.DC_DATE,
                MetadataOccurrence.Required,
                Localizer.Message("dc_date_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_TYPE] = new MetadataEntryDescription(
                Metadata.DC_TYPE,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_type_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_FORMAT] = new MetadataEntryDescription(
                Metadata.DC_FORMAT,
                MetadataOccurrence.Required,
                Localizer.Message("dc_format_description"),
                true, true);
            DAISY_ENTRIES[Metadata.DC_IDENTIFIER] = new MetadataEntryDescription(
                Metadata.DC_IDENTIFIER,
                MetadataOccurrence.Required,
                Localizer.Message("dc_identifier_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_SOURCE] = new MetadataEntryDescription(
                Metadata.DC_SOURCE,
                MetadataOccurrence.Recommended,
                Localizer.Message("dc_source"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_LANGUAGE] = new MetadataEntryDescription(
                Metadata.DC_LANGUAGE,
                MetadataOccurrence.Required,
                Localizer.Message("dc_language_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_RELATION] = new MetadataEntryDescription(
                Metadata.DC_RELATION,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_relation_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_COVERAGE] = new MetadataEntryDescription(
                Metadata.DC_COVERAGE,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_coverage_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DC_RIGHTS] = new MetadataEntryDescription(
                Metadata.DC_RIGHTS,
                MetadataOccurrence.Optional,
                Localizer.Message("dc_rights_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DTB_SOURCE_DATE] = new MetadataEntryDescription(
                Metadata.DTB_SOURCE_DATE,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_source_date_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_SOURCE_EDITION] = new MetadataEntryDescription(
                Metadata.DTB_SOURCE_EDITION,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_source_edition_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_SOURCE_PUBLISHER] = new MetadataEntryDescription(
                Metadata.DTB_SOURCE_PUBLISHER,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_source_publisher_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_SOURCE_RIGHTS] = new MetadataEntryDescription(
                Metadata.DTB_SOURCE_RIGHTS,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_source_rights_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_SOURCE_TITLE] = new MetadataEntryDescription(
                Metadata.DTB_SOURCE_TITLE,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_source_title_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_MULTIMEDIA_TYPE] = new MetadataEntryDescription(
                Metadata.DTB_MULTIMEDIA_TYPE,
                MetadataOccurrence.Required,
                Localizer.Message("dtb_multimedia_type_description"),
                false, true);
            DAISY_ENTRIES[Metadata.DTB_MULTIMEDIA_CONTENT] = new MetadataEntryDescription(
                Metadata.DTB_MULTIMEDIA_CONTENT,
                MetadataOccurrence.Required,
                Localizer.Message("dtb_multimedia_content_description"),
                false, true);
            DAISY_ENTRIES[Metadata.DTB_NARRATOR] = new MetadataEntryDescription(
                Metadata.DTB_NARRATOR,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_narrator_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DTB_PRODUCER] = new MetadataEntryDescription(
                Metadata.DTB_PRODUCER,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_producer_description"),
                true, false);
            DAISY_ENTRIES[Metadata.DTB_PRODUCED_DATE] = new MetadataEntryDescription(
                Metadata.DTB_PRODUCED_DATE,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_produced_date_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_REVISION] = new MetadataEntryDescription(
                Metadata.DTB_REVISION,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_revision_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_REVISION_DATE] = new MetadataEntryDescription(
                Metadata.DTB_REVISION_DATE,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_revision_date_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_REVISION_DESCRIPTION] = new MetadataEntryDescription(
                Metadata.DTB_REVISION_DESCRIPTION,
                MetadataOccurrence.Optional,
                Localizer.Message("dtb_description_description"),
                false, false);
            DAISY_ENTRIES[Metadata.DTB_TOTAL_TIME] = new MetadataEntryDescription(
                Metadata.DTB_TOTAL_TIME,
                MetadataOccurrence.Required,
                Localizer.Message("dtb_total_time_description"),
                false, true);
            DAISY_ENTRIES[Metadata.DTB_AUDIO_FORMAT] = new MetadataEntryDescription(
                Metadata.DTB_AUDIO_FORMAT,
                MetadataOccurrence.Recommended,
                Localizer.Message("dtb_audio_format_description"),
                true, true);
            DAISY_ENTRIES[Metadata.GENERATOR] = new MetadataEntryDescription(
                Metadata.GENERATOR,
                MetadataOccurrence.Required,
                Localizer.Message("generator_description"),
                false, true);
            DAISY_ENTRIES[Metadata.OBI_XUK_VERSION] = new MetadataEntryDescription(
                Metadata.OBI_XUK_VERSION,
                MetadataOccurrence.Required,
                Localizer.Message("obi_xuk_version_description"),
                false, true);
            DAISY_ENTRIES[Metadata.OBI_DAISY3ExportPath] = new MetadataEntryDescription (
                Metadata.OBI_DAISY3ExportPath,
                MetadataOccurrence.Optional,
                Localizer.Message ( "obi_DAISY3ExportPath_Description" ),
                false, true);
            DAISY_ENTRIES[Metadata.OBI_DAISY2ExportPath] = new MetadataEntryDescription (
                Metadata.OBI_DAISY2ExportPath,
                MetadataOccurrence.Optional,
                Localizer.Message ( "obi_DAISY2ExportPath_Description" ),
                false, true);
        }
    }
}