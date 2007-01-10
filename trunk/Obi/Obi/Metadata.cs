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
        private Project mProject;                               // project that this metadata belongs to
        private List<MetadataItem> mTemplates;                  // templates for metadata
        private Dictionary<string, List<MetadataItem>> mItems;  // defined items

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
    }

    public enum MetadataOccurrence { Required, Recommended, Optional };

    public class MetadataItem
    {
        private string mName;                    // name of the metadata item, e.g. "dc:Contributor"
        private string mContent;                 // content string of the item
        private MetadataOccurrence mOccurrence;  // required, recommended or optional
        private bool mRepeatable;                // repeatable or not

        /// <summary>
        /// Get the name of the item.
        /// </summary>
        public string Name
        {
            get { return mName; }
        }

        /// <summary>
        /// Get/set the content, cannot set to an empty string (silently ignored.)
        /// </summary>
        public string Content
        {
            get { return mContent; }
            set { if (value != null && value != "") mContent = value; }
        }

        public MetadataOccurrence Occurrence
        {
            get { return mOccurrence; }
        }

        public bool Repeatable
        {
            get { return mRepeatable; }
        }

        public MetadataItem(string name, string content, MetadataOccurrence occurrence, bool repeatable)
        {
            mName = name;
            mContent = content;
            mOccurrence = occurrence;
            mRepeatable = repeatable;
        }

        public MetadataItem(MetadataItem template, string content)
        {
            mName = template.Name;
            mContent = content;
            mOccurrence = template.Occurrence;
            mRepeatable = template.Repeatable;
        }

        public override string  ToString()
        {
            return String.Format("{0} ({1})", mName, mOccurrence);
        }
    }
}