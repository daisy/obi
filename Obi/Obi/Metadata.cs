using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    /// <summary>
    /// Contain the complete metadata for a project.
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
        public static readonly string OBI_ASSETS_DIR = "obi:assetsdir";
        public static readonly string OBI_AUDIO_CHANNELS = "obi:audioChannels";
        public static readonly string OBI_BIT_DEPTH = "obi:bitDepth";
        public static readonly string OBI_SAMPLE_RATE = "obi:sampleRate";
        public static readonly string OBI_XUK_VERSION = "obi:xukversion";
    }

    public enum MetadataOccurrence { Required, Recommended, Optional };

    public class MetadataEntryDescription
    {
        private string mName;
        private MetadataOccurrence mOccurrence;
        private string mDescription;
        private bool mRepeatable;
        private List<List<string>> mAddlAttrs;

        private static readonly List<MetadataEntryDescription> DAISY_ENTRIES = new List<MetadataEntryDescription>(0);
        
        public MetadataEntryDescription(string name, MetadataOccurrence occurrence, string description, bool repeatable)
        {
            mName = name;
            mOccurrence = occurrence;
            mDescription = description;
            mRepeatable = repeatable;
            mAddlAttrs = new List<List<string>>(0);
        }

        public void AddAddlAttr(string name, string description)
        {
            List<string> attr = new List<string>(2);
            attr.Add(name);
            attr.Add(description);
            mAddlAttrs.Add(attr);
        }

        public void AddAddlAttr(string name, string description, List<string> values)
        {
            List<string> attr = new List<string>(values.Count + 2);
            attr.Add(name);
            attr.Add(description);
            attr.AddRange(values);
            mAddlAttrs.Add(attr);
        }

        public string Name { get { return mName; } }
        public MetadataOccurrence Occurrence { get { return mOccurrence; } }
        public string Description { get { return mDescription; } }
        public bool Repeatable { get { return mRepeatable; } }

        public static List<MetadataEntryDescription> GetDAISYEntries()
        {
            if (DAISY_ENTRIES.Count == 0) CreateDAISYEntries();
            return DAISY_ENTRIES;
        }

        public override string ToString() { return mName; }

        private static void CreateDAISYEntries()
        {
            // Required publication metadata
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