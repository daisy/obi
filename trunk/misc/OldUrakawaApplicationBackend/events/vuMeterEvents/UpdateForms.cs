using System;

namespace UrakawaApplicationBackend.events.vuMeterEvents
{
public delegate void DUpdateFormsEvent ( VuMeter ob_VuMeter , UpdateForms Update) ;
	/// <summary>
	/// Summary description for UpdateForms.
	/// </summary>
	public class UpdateForms : VuMeterEvent 
	{
	public event DUpdateFormsEvent UpdateFormsEvent;

		public void NotifyUpdateForms ( VuMeter ob_VuMeter , UpdateForms Update )
		{
if ( UpdateFormsEvent   != null) 
UpdateFormsEvent ( ob_VuMeter , Update) ;
		}

			
		
	}
}
