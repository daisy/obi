using System;

namespace AudioLib.Events.VuMeter
{
	public delegate void UpdateFormsHandler(object sender, UpdateFormsEventArgs e);

	/// <summary>
	/// Summary description for UpdateForms.
	/// </summary>
	public class UpdateFormsEventArgs: EventArgs
	{
	}
}
