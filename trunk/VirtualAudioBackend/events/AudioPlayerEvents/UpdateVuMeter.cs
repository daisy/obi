using System;
using VirtualAudioBackend.events.AudioPlayerEvents ;

namespace VirtualAudioBackend
{
	public delegate void UpdateVuMeterHandler(object sender, UpdateVuMeter e);  // JQ
	//public delegate void DUpdateVuMeterEvent ( object sender , UpdateVuMeter Update) ;

	/// <summary>
	/// Summary description for UpdateVuMeter.
	/// </summary>
	public class UpdateVuMeter : AudioPlayerEvent
	{
/*		public event  DUpdateVuMeterEvent UpdateVuMeterEvent ;

		public void NotifyUpdateVuMeter ( object sender , UpdateVuMeter Update )
		{
			if ( UpdateVuMeterEvent != null)
				UpdateVuMeterEvent ( sender , Update) ;
		}
*/
	}
}
