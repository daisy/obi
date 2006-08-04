using System;

namespace VirtualAudioBackend.events.VuMeterEvents
{
	public delegate void ResetHandler(object sender, Reset e);

	/// <summary>
	/// Summary description for Reset.
	/// </summary>
	public class Reset  : VuMeterEvent 
	{
	}
}
