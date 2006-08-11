using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Strip
{
    public delegate void RequestToImportAssetHandler(object sender, ImportAssetEventArgs e);

    public class ImportAssetEventArgs
    {
        private CoreNode mSectionNode;  // the section in which the phrase is to be added
        private string mAssetPath;      // the path of the asset to add
        private int mIndex;             // index at which to insert the phrase node

        public CoreNode SectionNode
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

        public ImportAssetEventArgs(CoreNode sectionNode, string assetPath, int index)
        {
            mSectionNode = sectionNode;
            mAssetPath = assetPath;
            mIndex = index;
        }
    }
}