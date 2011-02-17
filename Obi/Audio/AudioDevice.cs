using System;
using Microsoft.DirectX.DirectSound;

namespace Obi.Audio
{
    /*
    /// <summary>
    /// Small wrapper around DirectX devices to make it a little more friendly.
    /// </summary>
	public abstract class AudioDevice
	{
        private string mName;

        /// <summary>
        /// An informative string (i.e. name) for this device.
        /// </summary>
        public string Name { get { return mName; } }

		protected AudioDevice(string name) { mName = name; }

        public override string ToString() { return mName; }
	}

    public class OutputDevice: AudioDevice
    {
        private Device mDevice;

        /// <summary>
        /// The actual device object
        /// </summary>
        public Device Device
        {
            get { return mDevice; }
        }

        public OutputDevice(string name, Device device)
            : base(name)
        {
            mDevice = device;
        }
    }

    public class InputDevice : AudioDevice
    {
        private Capture mCapture;

        public Capture Capture
        {
            get { return mCapture; }
        }

        public InputDevice(string name, Capture capture)
            : base(name)
        {
            mCapture = capture;
        }
    }
    */
}
