using System;

namespace AudioLib.Events.VuMeter
{
	public delegate void ResetHandler(object sender, ResetEventArgs e);

	/// <summary>
	/// Summary description for Reset.
	/// </summary>
	public class ResetEventArgs: EventArgs
	{
	}
}
