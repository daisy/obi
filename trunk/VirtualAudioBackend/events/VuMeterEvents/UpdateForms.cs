using System;

namespace VirtualAudioBackend.events.VuMeterEvents
{
	public delegate void DUpdateFormsEvent ( object sender , UpdateForms Update) ;
	/// <summary>
	/// Summary description for UpdateForms.
	/// </summary>
	public class UpdateForms : VuMeterEvent 
	{
		public event DUpdateFormsEvent UpdateFormsEvent;

		public void NotifyUpdateForms ( object sender , UpdateForms Update )
		{
			if ( UpdateFormsEvent   != null) 
				UpdateFormsEvent ( sender , Update) ;
		}
		
	}
}
