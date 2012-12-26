using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Strip
{
  
    public class ImportAssetEventArgs
    {
        private SectionNode mSectionNode;  // the section in which the phrase is to be added
        private string mAssetPath;      // the path of the asset to add
        private int mIndex;             // index at which to insert the phrase node

        public SectionNode SectionNode
        {
            get  { return mSectionNode; }
        }

        public string AssetPath
        {
            get { return mAssetPath; }
        }

        public int Index
        {
            get { return mIndex; }
        }

        public ImportAssetEventArgs(SectionNode sectionNode, string assetPath, int index)
        {
            mSectionNode = sectionNode;
            mAssetPath = assetPath;
            mIndex = index;
        }
    }
}