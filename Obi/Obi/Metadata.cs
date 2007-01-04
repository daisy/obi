using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class Metadata
    {
        private List<MetadataItem> mTemplates;
        private List<MetadataItem> mItems;

        public List<MetadataItem> Templates
        {
            get { return mTemplates; }
        }

        public List<MetadataItem> Items
        {
            get { return mItems; }
        }

        public Metadata()
        {
            mTemplates = new List<MetadataItem>();
            mItems = new List<MetadataItem>();
            mTemplates.Add(new MetadataItem("dc:Title", Localizer.Message("meta_dc_title"),
                MetadataOccurrence.Required, true));
            mTemplates.Add(new MetadataItem("dc:Creator", Localizer.Message("meta_dc_creator"),
                MetadataOccurrence.Recommended, true));
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

        public override string  ToString()
        {
            return mName;
        }
    }
}