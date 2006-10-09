using System;

namespace UrakawaApplicationBackend.events.audioRecorderEvents
{
public delegate void DUpdateVuMeterEventHandller ( AudioRecorder Recorder , UpdateVuMeterFromRecorder Update) ;  	
	/// <summary>
	/// Summary description for UpdateVuMeterFromRecorder.
	/// </summary>

	public class UpdateVuMeterFromRecorder :AudioRecorderEvent
	{
		public event DUpdateVuMeterEventHandller UpdateVuMeterEvent;
		public void NotifyUpdateVuMeter ( AudioRecorder Recorder, UpdateVuMeterFromRecorder Update )
		{
			if ( UpdateVuMeterEvent != null)
				UpdateVuMeterEvent ( Recorder, Update) ;
		}


		
			

			
		
	}
}
