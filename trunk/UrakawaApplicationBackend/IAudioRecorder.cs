using System;
using System.Collections;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace UrakawaApplicationBackend
{
	public interface IAudioRecorder
	{
		// get the capture devices 
		// this will return the capture devices list 
		ArrayList GetInputDevice();
		
		// return the capture device guid for a selected device
		object SetInputDeviceGuid();

		// get the wave formats based on the sample rate and bit depth
		// pass the index  and ref WaveFormat as the parameters
		//allowed values: 1 and 2 for the channels
		//allowed values for the sample rates: 22050, 44100, 88200, 96000
		//allowed value for the bit depth is 16
		void GetWaveFormatFromIndex(int Index, ref WaveFormat format);

		// returns the array list of the formats based on the sample rate, channel  and the bit depth
		ArrayList GetFormatList();

		// a struct will be used in the class to get the wave format
//information 
	//struct FormatInfo  
		
		//// takes the WaveFormat as a parameter and converts it to string 
		// this is static method which will be used in the class 
		//static string sConvertWaveFormatToString(WaveFormat format);
		


		//throws AudioRecorderException 
//		void record(IAudioMediaAsset wave);
//		void stopRecording();
		//void setAudioRecorderListener(eventListeners.IAudioRecorderEventListener listener);

		//return value should be of type Collection<IAudioRecorderListener>
		//ArrayList getAudioRecorderListeners();

		//eventListeners.IAudioPlayerEventListener getAudioRecorderListener(int i);        

		//returns states such as recording, idle, initializing
		//the return value type will be something like AudioRecorderStateType instead of Object
//		Object getState();
	}
	public class  AudioRecord:  IAudioRecorder
	{
		public ArrayList m_aSelect = new ArrayList();
		
		public ArrayList GetInputDevice()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();  // gathers the available capture devices
			foreach (DeviceInformation info in devices)
			{
				m_aSelect.Add(info.Description);
			}
			return m_aSelect;
		}
		public object  SetInputDeviceGuid()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();
			ArrayList m_aGuid = new ArrayList();
			m_aGuid = GetInputDevice();
			Guid CaptureDeviceGuid = Guid.Empty;  
			if(0<m_aGuid.Count)
			{
				CaptureDeviceGuid = devices[0].DriverGuid;
			}
			return CaptureDeviceGuid;
		}
		// structure is used for the wave format info 
		public struct FormatInfo  
		{
			public WaveFormat format;
			public override string ToString()
			{
				return  sConvertWaveFormatToString(format);
			}
		};
		
		public static string sConvertWaveFormatToString(WaveFormat format)
		{
			return format.SamplesPerSecond + " Hz, " + 
				format.BitsPerSample + "-bit " + 
				((format.Channels == 1) ? "Mono" : "Stereo");
		}

		public ArrayList	m_aformats = new ArrayList();
		public bool[]	m_bInputFormatSupported = new bool[8];
			
	
		public void GetWaveFormatFromIndex(int Index, ref WaveFormat format)
		{
			// Name: GetWaveFormatFromIndex()
			// Desc: Returns 8 different wave formats based on Index
			int SampleRate = Index / 2;
			int iType = Index %2 ;
			switch (SampleRate)
			{
				case 0: format.SamplesPerSecond = 22050; break;
				case 1: format.SamplesPerSecond = 44100; break;
				case 2: format.SamplesPerSecond = 208200; break;
				case 3: format.SamplesPerSecond = 96000; break;
			}
			switch (iType)
			{
				case 0: format.BitsPerSample = 16; 
					format.Channels = 1; 
					break;
				case 1: format.BitsPerSample =  16; 
					format.Channels = 2; 
					break;		
			}
			format.BlockAlign = (short)(format.Channels * (format.BitsPerSample / 8)); 
			// BlockAlign Retrieves and sets the minimum atomic unit of data, in bytes, for the format type.
			
			format.AverageBytesPerSecond = format.BlockAlign * format.SamplesPerSecond;
			// AverageBytesPerSecond Retrieves and sets the required average data-transfer rate, in bytes per second, for the format type.

		}
		
		public ArrayList GetFormatList()
		{
			FormatInfo  info			= new FormatInfo();			

			string	FormatName	= string.Empty;
			WaveFormat	format			= new WaveFormat();
			for (int iIndex = 0; iIndex < m_bInputFormatSupported.Length; iIndex++)
			{
				
				//Turn the index into a WaveFormat then turn that into a
				// string and put the string in the listbox
				GetWaveFormatFromIndex(iIndex, ref format);
				info.format = format;
				m_aformats.Add(info);
			}
			return m_aformats;
		}

	}
}
