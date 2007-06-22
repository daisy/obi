using System;
using Microsoft.DirectX.DirectSound;

namespace Zaboom.Audio
{
    /// <summary>
    /// Small wrapper around DirectX devices to make it a little more friendly.
    /// </summary>
	public abstract class AudioDevice
	{
        private string name;

        protected AudioDevice(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// An informative string (i.e. name) for this device.
        /// </summary>
        public string Name { get { return name; } }

        public override string ToString() { return name; }
	}

    public class OutputDevice: AudioDevice
    {
        private Device device;

        public OutputDevice(string name, Device device)
            : base(name)
        {
            this.device = device;
        }

        /// <summary>
        /// The actual device object
        /// </summary>
        public Device Device { get { return device; } }
    }

    public class InputDevice : AudioDevice
    {
        private Capture capture;

        public InputDevice(string name, Capture capture)
            : base(name)
        {
            this.capture = capture;
        }

        public Capture Device { get { return capture; } }
    }
}
