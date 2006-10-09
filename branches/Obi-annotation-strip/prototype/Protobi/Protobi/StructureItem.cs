using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    public abstract class StructureItem
    {
        private string mLabel;

        public string Label
        {
            get { return mLabel; }
            set { mLabel = value; }
        }

        public StructureItem(string label)
        {
            mLabel = label;
        }
    }

    public class StructureHead : StructureItem
    {
        private HeadingLevel mLevel;

        public HeadingLevel Level { get { return mLevel; } }

        public StructureHead(string label, uint level)
            : base(label)
        {
            mLevel = new HeadingLevel(level);
        }
    }

    public class PageItem: StructureItem
    {
        public PageItem(string label)
            : base(label)
        {
        }
    }

    public class HeadingLevel
    {
        private uint mLevel;

        public uint Level
        {
            get { return mLevel; }
            set { mLevel = value; }
        }

        public HeadingLevel(uint level)
        {
            mLevel = level;
        }

        public override string ToString()
        {
            if (mLevel == 0)
            {
                return Localizer.GetString("heading_title");
            }
            else
            {
                return String.Format(Localizer.GetString("heading_level"), mLevel);
            }
        }
    }
}
