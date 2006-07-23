using System;

namespace VirtualAudioBackend.events.AudioRecorderEvents
{

	public delegate void DUpdateVuMeterEventHandller ( object sender , UpdateVuMeterFromRecorder Update) ;  	
	/// <summary>
	/// Summary description for UpdateVuMeterFromRecorder.
	/// </summary>

	public class UpdateVuMeterFromRecorder :AudioRecorderEvent
	{
		public event DUpdateVuMeterEventHandller UpdateVuMeterEvent;
		public void NotifyUpdateVuMeter ( object sender , UpdateVuMeterFromRecorder Update )
		{
			if ( UpdateVuMeterEvent != null)
				UpdateVuMeterEvent ( sender , Update) ;
		}


		
			

			
		
	}
}
