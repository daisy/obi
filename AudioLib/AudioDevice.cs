using System;
using Microsoft.DirectX.DirectSound;

namespace AudioLib
{
    /// <summary>
    /// Small wrapper around DirectX devices to make it a little more friendly.
    /// </summary>
	public abstract class AudioDevice
	{
        private DeviceInformation mDevInfo;
        /// <summary>
        /// An informative string (i.e. name) for this device.
        /// </summary>
        public string Name { get { return mDevInfo.Description; } }

		protected AudioDevice(DeviceInformation devInfo) { mDevInfo = devInfo; }

        public override string ToString() { return Name; }
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

        public OutputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mDevice = new Device(devInfo.DriverGuid);
        }
    }

    public class InputDevice : AudioDevice
    {
        private Capture mCapture;

        public Capture Capture
        {
            get { return mCapture; }
        }

        public InputDevice(DeviceInformation devInfo)
            : base(devInfo)
        {
            mCapture = new Capture(devInfo.DriverGuid);
        }
    }
}
