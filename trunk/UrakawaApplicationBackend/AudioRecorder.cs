using System;
using System.Collections;
using Microsoft.DirectX;
using System.Threading;
using System.IO;
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
		private int m_iIndex;
		private BufferPositionNotify[] PositionNotify = new BufferPositionNotify[NumberRecordNotifications + 1];  
		private const int NumberRecordNotifications	= 16;
		private AutoResetEvent NotificationEvent	= null;
		private CaptureBuffer applicationBuffer ;
		private Notify applicationNotify ;
		private Thread NotifyThread ;
		private BinaryWriter Writer ;
		private int m_iNextCaptureOffset ;
		private bool Recording = false;
		private WaveFormat InputFormat;
		private int m_iSampleCount ;
		private bool Capturing ;
		private int nLength ; // File length, minus first 8 bytes of RIFF description. This will be filled in later.
		private short shBytesPerSample ; // Bytes per sample.
		private int nFormatChunkLength;
		private FileStream WaveFile;
		string m_sFileName;
		AssetManager m_assetManager  = new AssetManager();
		private WaveFormat DefaultFormat;
		private short m_bitDepth;
		private int m_SamplesPerSecond;
		private int m_iChannels;	



		// this is used to get the default values for the channels as 1, 
		//BitDepth = 16
		// and SamplesPersecond as 44100
		public void DefaultValues()
		{
			int m_iBitDepthIndex = 9;
			DefaultFormat = GetInputFormat(m_iBitDepthIndex);
			m_bitDepth = DefaultFormat.BitsPerSample;
			m_iChannels = DefaultFormat.Channels;
			m_SamplesPerSecond = DefaultFormat.SamplesPerSecond;
		}

		//this property is used for SamplesPerSEcond as 44100	
		// this will use DefaultValues()
		public int SamplesPerSecond
		{
			get
			{
				return m_SamplesPerSecond;
			}
			set
			{
				m_SamplesPerSecond = value;
			}
		}
		// this will set the bit depth as 16 through DefaultValues()
		public short BitDepth
		{
			get
			{
				return m_bitDepth;
			}
			set
			{
				m_bitDepth = value;
			}
		}
		//this will set the number of channels to 1 through  DefaultValues()
		public int Channels
		{
			get
			{
				return m_iChannels;
			}
			set
			{
				m_iChannels = value;
			}
		}





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
		
		
		public string ToGetFileName(string m_sDirPath)
		{
			m_sFileName  = m_assetManager.GenerateFileName(wav, m_sDirPath);
			return m_sFileName;
		}
		// create a wavefile with all the basic of riff information
		// only the data lenghts will be be filled in later
		public void CreateRIFF(string FileName, string sProjectDir)
		{
			// Open up the wave file for writing.
			InputFormat = GetInputFormat(m_iIndex);
			FileName = ToGetFileName(sProjectDir);
			WaveFile = new FileStream(FileName, FileMode.Create);	
			Writer = new BinaryWriter(WaveFile);
			// Set up file with RIFF chunk info.
			char[] ChunkRiff = {'R','I','F','F'};
			char[] ChunkType = {'W','A','V','E'};
			char[] ChunkFmt	= {'f','m','t',' '};
			char[] ChunkData = {'d','a','t','a'};
			short shPad = 1; // File padding
			nLength = 0;
			int nFormatChunkLength = 0x10; 
			// Format chunk length.
			// Figure out how many bytes there will be per sample.
			if (8 == InputFormat.BitsPerSample && 1 == InputFormat.Channels)
				shBytesPerSample = 1;
			else if ((8 == InputFormat.BitsPerSample && 2 == InputFormat.Channels) || (16 == InputFormat.BitsPerSample && 1 == InputFormat.Channels))
				shBytesPerSample = 2;
			else if (16 == InputFormat.BitsPerSample && 2 == InputFormat.Channels)
				shBytesPerSample = 4;
			// Fill in the riff info for the wave file.
			Writer.Write(ChunkRiff);
			Writer.Write(nLength);
			Writer.Write(ChunkType);
			// Fill in the format info for the wave file.
			Writer.Write(ChunkFmt);
			Writer.Write(nFormatChunkLength);
			Writer.Write(shPad);
			Writer.Write(InputFormat.Channels);
			Writer.Write(InputFormat.SamplesPerSecond);
			Writer.Write(InputFormat.AverageBytesPerSecond);
			Writer.Write(shBytesPerSample);
			Writer.Write(InputFormat.BitsPerSample);
			// Now fill in the data chunk.
			Writer.Write(ChunkData);
			Writer.Write((int)0);	// The sample length will be written in later
			
		}

		public  CaptureBuffer   GetCaptureBufferFromIndex(int Index)
		{
			CaptureBufferDescription dsc = new CaptureBufferDescription(); 
			if (null != applicationBuffer)
			{
				applicationBuffer.Dispose();
				applicationBuffer = null;
			}
			InputFormat = GetInputFormat(Index);
							
			m_iNotifySize = (1024 > InputFormat.AverageBytesPerSecond / 8) ? 1024 : (InputFormat.AverageBytesPerSecond / 8);
			m_iNotifySize -= m_iNotifySize % InputFormat.BlockAlign;   
			// Set the buffer sizes
			m_iCaptureBufferSize = m_iNotifySize * NumberRecordNotifications;
			// Create the capture buffer
			dsc.BufferBytes = m_iCaptureBufferSize;
			InputFormat.FormatTag = WaveFormatTag.Pcm;
			dsc.Format = InputFormat; // Set the format during creatation
			m_cApplicationDevice= InitDirectSound();
			applicationBuffer = new CaptureBuffer(dsc, m_cApplicationDevice);
			return applicationBuffer;
		}
		public void CreateCaptureBuffer()
		{
			
			// Desc: Creates a capture buffer and sets the format 
			if (null != applicationNotify)
			{
				applicationNotify.Dispose();
				applicationNotify = null;
			}	
			InputFormat = GetInputFormat(m_iIndex);
			m_iNotifySize = (1024 > InputFormat.AverageBytesPerSecond / 8) ? 1024 : (InputFormat.AverageBytesPerSecond / 8);
			m_iNotifySize -= m_iNotifySize % InputFormat.BlockAlign;   
			applicationBuffer = GetCaptureBufferFromIndex(m_iIndex);
			InitNotifications();
		}
		public void InitNotifications()
		{
			// Name: InitNotifications()
			// Desc: Inits the notifications on the capture buffer which are handled
			//       in the notify thread.
			if (null == applicationBuffer)
				throw new NullReferenceException();
			//Create a thread to monitor the notify events
			if (null == NotifyThread)
			{
				NotifyThread = new Thread(new ThreadStart(WaitThread));										 			
				Capturing = true;
				NotifyThread.Start();
				// Create a notification event, for when the sound stops playing
				NotificationEvent = new AutoResetEvent(false);
			}
			// Setup the notification positions
			for (int i = 0; i < NumberRecordNotifications; i++)
			{
				PositionNotify[i].Offset = (m_iNotifySize * i) + m_iNotifySize - 1;
				PositionNotify[i].EventNotifyHandle = NotificationEvent.Handle;
			}
			applicationNotify = new Notify(applicationBuffer);
			// Tell DirectSound when to notify the app. The notification will come in the from 
			// of signaled events that are handled in the notify thread.
			applicationNotify.SetNotificationPositions(PositionNotify, NumberRecordNotifications);
		}
		private void WaitThread()
		{

		
		
			while(Capturing)
			{
				//Sit here and wait for a message to arrive
				NotificationEvent = new AutoResetEvent(false);
				NotificationEvent.WaitOne(Timeout.Infinite, true);
				//waits for infinite time span before recieving a signal
				RecordCapturedData();
			}
		}
		public void RecordCapturedData( ) 
		{
			// Desc: Copies data from the capture buffer to the output buffer 
			int ReadPos ;
			int CapturePos ;
			int LockSize ;

				
			applicationBuffer.GetCurrentPosition(out CapturePos, out ReadPos);
			LockSize = ReadPos - m_iNextCaptureOffset;
			if (LockSize < 0)
				LockSize += m_iCaptureBufferSize;
			// Block align lock size so that we are always write on a boundary
			LockSize -= (LockSize % m_iNotifySize);
			if (0 == LockSize)
				return;
			// Read the capture buffer.
			CaptureData = (byte[])applicationBuffer.Read(m_iNextCaptureOffset, typeof(byte), LockFlag.None, LockSize);
			// Write the data into the wav file");
			Writer = new BinaryWriter(WaveFile			);
			Writer.Write(CaptureData, 0, CaptureData.Length);
			// Update the number of samples, in bytes, of the file so far.
			m_iSampleCount += CaptureData.Length;
			// Move the capture offset along
			m_iNextCaptureOffset += CaptureData.Length; 
			m_iNextCaptureOffset %= m_iCaptureBufferSize; // Circular buffer
		}
		

		
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

		

		

		
		public AudioRecorderState State
		{
			get
			{
				return AudioRecorderState.Idle;
			}
		}

			



		
			
	}
}

	


