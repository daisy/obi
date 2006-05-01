using System;
using System.Collections;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
namespace UrakawaApplicationBackend
{
	
	public class AudioRecorder:IAudioRecorder
	{
		private ArrayList m_aSelect = new ArrayList();
		private ArrayList m_aGuid = new ArrayList();
		private Capture m_cApplicationDevice = null;
		private int m_iCaptureBufferSize = 0;
		private int m_iNotifySize = 0;					
		private Guid m_gCaptureDeviceGuid = Guid.Empty;  
		// returns a list of input devices
		public ArrayList GetInputDevice()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();  // gathers the available capture devices
			foreach (DeviceInformation info in devices)
			{
				m_aSelect.Add(info.Description);
			}
			return m_aSelect;
		}

		//returns the guid of the selected device, in this case the default 
		//device guid has been returned
		public Guid SetInputDeviceGuid()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();
			m_aGuid = GetInputDevice();
			m_gCaptureDeviceGuid = devices[1].DriverGuid;
			return m_gCaptureDeviceGuid;
		}
		
		//this return type is capture, where the device guid is used to set a capture which
		//will be later on used to create the capture buffer for recording
		public Capture InitDirectSound()
		{
			m_iCaptureBufferSize = 0;
			m_iNotifySize = 0;
			// Create DirectSound.Capture using the preferred capture device
			m_gCaptureDeviceGuid = SetInputDeviceGuid();
			m_cApplicationDevice = new Capture(m_gCaptureDeviceGuid);
			return m_cApplicationDevice;
		}
		
			
	}
}

	

