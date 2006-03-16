using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.assetManagerEvents
{
    class AssetRenamed : AssetManagerEvent
    {
		private string mOldName;
		private IMediaAsset mRenamedAsset;

		public AssetRenamed(string oldName, IMediaAsset newAsset)
		{
			mOldName = oldName;
			mRenamedAsset = newAsset;
		}

		public string OldName
		{
			get
			{
				return mOldName;
			}
		}

		public IMediaAsset RenamedAsset
		{
			get
			{
				return mRenamedAsset;
			}
		}
    }
}
