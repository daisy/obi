using System;

namespace VirtualAudioBackend.events.AudioRecorderEvents
{
	public delegate void UpdateVuMeterFromRecorderHandler(object sender, UpdateVuMeterFromRecorder e);

	/// <summary>
	/// Recorder want an update in the VU meter.
	/// </summary>
	public class UpdateVuMeterFromRecorder :AudioRecorderEvent
	{
	}
}
