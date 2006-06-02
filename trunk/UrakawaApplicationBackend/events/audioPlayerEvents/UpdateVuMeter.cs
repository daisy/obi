using System;

namespace UrakawaApplicationBackend.events.audioPlayerEvents
{
public delegate void DUpdateVuMeterEvent ( AudioPlayer Player , UpdateVuMeter Update) ;

	/// <summary>
	/// Summary description for UpdateVuMeter.
	/// </summary>
	public class UpdateVuMeter
	{
		public event  DUpdateVuMeterEvent UpdateVuMeterEvent ;

		public void NotifyUpdateVuMeter ( AudioPlayer Player, UpdateVuMeter Update )
		{
if ( UpdateVuMeterEvent != null)
UpdateVuMeterEvent ( Player , Update) ;
		}

			
		
	}
}
