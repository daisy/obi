using System;

namespace AudioLib.Events.Recorder
{
	public delegate void UpdateVuMeterHandler(object sender, UpdateVuMeterEventArgs e);
    public delegate void ResetVuMeterHandler (object sender, UpdateVuMeterEventArgs e);

	public class UpdateVuMeterEventArgs : EventArgs
	{
	}
}
