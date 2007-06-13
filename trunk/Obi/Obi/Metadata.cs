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
        public static readonly string OBI_XUK_VERSION = "obi:xukversion";

        // private Project mProject;                               // project that this metadata belongs to
        // private List<MetadataItem> mTemplates;                  // templates for metadata
        // private Dictionary<string, List<MetadataItem>> mItems;  // defined items


        /*
        /// <summary>
        /// List of required metadata items which are currently missing.
        /// </summary>
        public List<MetadataItem> Missing
        {
            get
            {
                return mTemplates.FindAll(delegate(MetadataItem item)
                {
                    return item.Occurrence == MetadataOccurrence.Required &&
                        mItems[item.Name].Count == 0;
                });
            }
        }

        /// <summary>
        /// The lift of metadata items that can currently be added.
        /// </summary>
        public List<MetadataItem> CanAdd
        {
            get
            {
                return mTemplates.FindAll(delegate(MetadataItem item)
                {
                    return item.Repeatable || mItems[item.Name].Count > 0;
                });
            }
        }

        /// <summary>
        /// Create a new empty metadata list following a given list of templates.
        /// </summary>
        public Metadata(Project project, List<MetadataItem> templates)
        {
            mProject = project;
            mTemplates = templates;
            mItems = new Dictionary<string, List<MetadataItem>>();
            foreach (MetadataItem item in mTemplates) mItems[item.Name] = new List<MetadataItem>();
        }

        /// <summary>
        /// Create a list of templates for DAISY metadata.
        /// </summary>
        public static List<MetadataItem> DaisyTemplates()
        {
            List<MetadataItem> templates = new List<MetadataItem>();
            templates.Add(new MetadataItem(SimpleMetadata.MetaTitle, Localizer.Message("meta_dc_title"),
                MetadataOccurrence.Required, true));
            templates.Add(new MetadataItem("dc:Creator", Localizer.Message("meta_dc_creator"),
                MetadataOccurrence.Recommended, true));
            return templates;
        }

        /// <summary>
        /// Add a metadata item.
        /// Raise an exception if this does not conform to the metadata model. 
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void Add(MetadataItem item)
        {
            if (!item.Repeatable && mItems[item.Name].Count > 0)
            {
                throw new Exception(String.Format("Metadata item {0} cannot be repeated.", item.Name));
            }
            mItems[item.Name].Add(item);
        }

        /// <summary>
        /// If the user wants to delete an item which is required (and for which there are no other items)
        /// then a warning should be issued.
        /// </summary>
        internal bool DeleteWarning(MetadataItem item)
        {
            return
                item.Occurrence == MetadataOccurrence.Required &&
                mItems[item.Name].Count == 1;
        }

        /// <summary>
        /// Delete an item.
        /// </summary>
        /// <param name="item">The item to delete.</param>
        internal void Delete(MetadataItem item)
        {
            mItems[item.Name].Remove(item);
        }

        /// <summary>
        /// Update the project simple metadata from the full metadata.
        /// </summary>
        public void UpdateSimpleMetadata()
        {
            if (mItems[SimpleMetadata.MetaTitle].Count > 0)
                mProject.Metadata.Title = mItems[SimpleMetadata.MetaTitle][0].Content;
        }
         * 
         */
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