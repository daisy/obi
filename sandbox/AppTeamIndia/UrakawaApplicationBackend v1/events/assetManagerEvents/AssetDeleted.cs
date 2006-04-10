using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.assetManagerEvents
{
    class AssetDeleted : AssetManagerEvent
    {
		private string mEventName;

		public AssetDeleted(string eventName)
		{
			mEventName = eventName;
		}

		public string EventName
		{
			get
			{
				return mEventName;
			}
	
		}
    }
}
