using System;

namespace VirtualAudioBackend.events.VuMeterEvents
{
public delegate  void dResetEvent ( object sender , VuMeterEvent  ob_ResetEvent );
	/// <summary>
	/// Summary description for Reset.
	/// </summary>
	public class Reset  : VuMeterEvent 
	{
		public event dResetEvent ResetEvent ;

		public void TriggerReset ( object sender, VuMeterEvent  ob_ResetEvent )
		{
if (ResetEvent != null)
	ResetEvent (sender , ob_ResetEvent) ;
		}
		
	}
}
