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
		private bool[] m_bInputFormatSupported = new bool[12];
		private ArrayList m_aformats= new ArrayList();
		
		public Device InputDevice
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public int Channels
		{
			get
			{
				return 1;
			}
			set
			{
			}
		}

		public int SampleRate
		{
			get
			{
				return 44100;
			}
			set
			{
			}
		}

		public int BitDepth
		{
			get
			{
				return 16;
			}
			set
			{
			}
		}

		public AudioRecorderState State
		{
			get
			{
				return AudioRecorderState.Idle;
			}
		}

		public void Record(IAudioMediaAsset asset)
		{
		}

		public void Stop()
		{
		}

		// returns a list of input devices
		public ArrayList GetInputDevices()
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
			m_aGuid = GetInputDevices();
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
// structure for format information
		private struct FormatInfo
		{
			public WaveFormat format;
			public override string ToString()
			{
				return  ConvertWaveFormatToString(format);
			}
		};
		
		//static method to convert the WaveFormat to string
		private static string ConvertWaveFormatToString(WaveFormat format)
		{
			// Name: ConvertWaveFormatToString()
			// Desc: Converts a wave format to a text string
			return format.SamplesPerSecond + " Hz, " + format.BitsPerSample + "-bit " + 
				((format.Channels == 1) ? "Mono" : "Stereo");
		}

		//Returns 12different wave formats based on Index
		public void GetWaveFormatFromIndex(int m_iIndex, ref WaveFormat m_wFormat)
		{
			int SampleRate = m_iIndex / 4;
			int iType = m_iIndex %4 ;
			switch (SampleRate)
			{
				case 0: m_wFormat.SamplesPerSecond = 11025; break;
				case 1:  m_wFormat.SamplesPerSecond = 22050; break;
				case 2: m_wFormat.SamplesPerSecond = 44100; break;
			}				
			switch (iType)
			{
				case 0: m_wFormat.BitsPerSample =  8; m_wFormat.Channels = 1; break;
				case 1: m_wFormat.BitsPerSample = 16; m_wFormat.Channels = 1; break;
				case 2: m_wFormat.BitsPerSample =  8; m_wFormat.Channels = 2; break;
				case 3: m_wFormat.BitsPerSample = 16; m_wFormat.Channels = 2; break;

			}
			// BlockAlign Retrieves and sets the minimum atomic unit of data, in bytes, for the format type.
			m_wFormat.BlockAlign = (short)(m_wFormat.Channels * (m_wFormat.BitsPerSample / 8)); 
			// AverageBytesPerSecond Retrieves and sets the required average data-transfer rate, in bytes per second, for the format type.			
			m_wFormat.AverageBytesPerSecond = m_wFormat.BlockAlign * m_wFormat.SamplesPerSecond;
}
//collects the list of all the formats in an ArrayList and returns the ArrayList
		public ArrayList GetFormatList()
		{
			FormatInfo  info			= new FormatInfo();			
			WaveFormat	m_wFormat			= new WaveFormat();
			for (int m_iIndex = 0; m_iIndex < m_bInputFormatSupported.Length; m_iIndex++)
			{
				//Turn the m_iIndex into a WaveFormat 
				GetWaveFormatFromIndex(m_iIndex, ref m_wFormat);
				info.format = m_wFormat;
				m_aformats.Add(info);
			}
			return m_aformats;
		}
		//it returns a selected format on the basis of the selected index
		public WaveFormat GetInputFormat(int m_iIndex)
		{
			WaveFormat m_wFormat = new WaveFormat();
			FormatInfo Info = new FormatInfo();
			m_aformats= GetFormatList();
			GetWaveFormatFromIndex(m_iIndex, ref m_wFormat);
			Info.format = m_wFormat;
			return Info.format;
		}
		
			
		}
	}

	

