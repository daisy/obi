using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

namespace Obi.Events.Strip
{
    public delegate void RequestToImportAssetHandler(object sender, ImportAssetEventArgs e);

    // TODO: position of the phrase in this asset
    public class ImportAssetEventArgs
    {
        private CoreNode mSectionNode;  // the section in which the phrase is to be added
        private string mAssetPath;      // the path of the asset to add

        public CoreNode SectionNode
        {
            get
            {
                return mSectionNode;
            }
        }

        public string AssetPath
        {
            get
            {
                return mAssetPath;
            }
        }

        public ImportAssetEventArgs(CoreNode sectionNode, string assetPath)
        {
            mSectionNode = sectionNode;
            mAssetPath = assetPath;
        }
    }
}
