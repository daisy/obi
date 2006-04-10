using System;
using System.Collections;
using System.Text;

namespace urakawaApplication.events.audioPlayerEvents
{
    class EndOfFile : AudioPlayerEvent
    {
        private string mFileName;

		public EndOfFile(string filename)
		{
			mFileName = filename;
		}

		public string FileName
		{
			get
			{
				return mFileName;
			}
		}
    }
}
