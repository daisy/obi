using System;
using System.Collections;
using Microsoft.DirectX;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Microsoft.DirectX.DirectSound;
using UrakawaApplicationBackend.events.audioRecorderEvents;

namespace UrakawaApplicationBackend

{
	
	public class AudioRecorder:IAudioRecorder
	{
		// member variables
		private ArrayList m_aSelect = new ArrayList();
		private ArrayList m_aGuid = new ArrayList();
		private Capture m_cApplicationDevice ;
		private int m_iCaptureBufferSize ;
		private int m_iNotifySize ;					
		private Guid m_gCaptureDeviceGuid = Guid.Empty;  
		private bool[] m_bInputFormatSupported = new bool[12];
		private ArrayList m_aformats= new ArrayList();
		private int Index;
		private BufferPositionNotify[] PositionNotify = new BufferPositionNotify[NumberRecordNotifications + 1];  
		private const int NumberRecordNotifications	= 16;
		private AutoResetEvent NotificationEvent	= null;
		private CaptureBuffer applicationBuffer ;
		private Notify applicationNotify ;
		private Thread NotifyThread ;
		private BinaryWriter Writer ;
		private int NextCaptureOffset ;
		private WaveFormat InputFormat; 
		private long SampleCount ;
		private bool Capturing = false;
		private CaptureBufferDescription dsc = new CaptureBufferDescription();
		private string m_sFileName = "F:\\My Documents\\6.wav";
			internal static string ProjectDirectory  ;
		private static int m_ConstructorCounter =0;
		private short m_bitDepth;
		private int m_SampleRate;
		internal int m_FrameSize;
		internal int m_Channels;
		//private long Length;//length of the file, so that append can be implemented
		private byte[] CaptureData;
		private bool mEventsEnabled = true;
		private AudioRecorderState mState;
		private Microsoft.DirectX.DirectSound.Device mInputDevice = null;
		private AudioMediaAsset m_AudioMediaAsset;
		UpdateVuMeterFromRecorder ob_UpdateVuMeter = new UpdateVuMeterFromRecorder();
		VuMeter ob_VuMeter;
		AudioMediaAsset OldAsset; 


		//for input device
		public Microsoft.DirectX.DirectSound.Device InputDevice
		{
			get
			{
				return mInputDevice;
			}
			set
			{
				mInputDevice = value;
			}
		}

		//this property is used for SampleRate
		public int SampleRate
		{
			get
			{
				if((m_SampleRate == 11025) &&((m_SampleRate != 22050) || (m_SampleRate != 44100)))
					return m_SampleRate;
				else if(((m_SampleRate != 11025) ||(m_SampleRate != 44100)) &&(m_SampleRate == 22050))
					return m_SampleRate;
				else if(((m_SampleRate != 11025) ||(m_SampleRate != 22050)) &&(m_SampleRate == 44100))
					return m_SampleRate;
				else
					throw new Exception("invalid sample rate");
			}
			set
			{
				m_SampleRate = value;
			}
		}
		//this will set the bit depth as 8 or 16
		public short BitDepth
		{
			get
			{
				if((m_bitDepth == 16) && (m_bitDepth != 8) || (m_bitDepth!= 16) &&(m_bitDepth == 8))
				{
					return m_bitDepth;
				}
				else
				{
					throw new Exception("invalid bit depth");
				}

			}
			set
			{
				m_bitDepth = value;
			}
		}
		//this will set the number of channels to 1 or 2 through GetInputFormat(int Index)
		public int Channels
		{
			get
			{
				if((m_Channels ==1 && m_Channels != 2)|| (m_Channels !=1 && m_Channels == 2))	
				{
					return m_Channels;
				}
				else
				{
					throw new Exception("invalid channels");
				}


			}
			set
			{	
					m_Channels = value;
}
				}
				




		public AudioRecorderState State
		{
			get
			{
				return mState;

			}
		}


		public AudioRecorder()
		{
			Index = 0;
			if(0 == m_ConstructorCounter)
			{
				m_ConstructorCounter++;
				mState = AudioRecorderState.Idle;
				Capturing= true;
			}
			else
			{
				throw new Exception("This class is Singleton");
			}			
		}
		
			
		// Set VuMeter object
		public 		void SetVuMeterObject ( VuMeter ob_VuMeterArg )
		{
			ob_VuMeter = ob_VuMeterArg ;
			ob_UpdateVuMeter.UpdateVuMeterEvent+= new DUpdateVuMeterEventHandller (ob_VuMeter.CatchUpdateVuMeterEvent);
		}

		public VuMeter VuMeterObject
		{
			get
			{
				return ob_VuMeter ;
			}
			set
			{
				SetVuMeterObject (value) ;
			}
		}

//		 array for update current amplitude to VuMeter
		internal byte [] arUpdateVM ;
		internal int m_UpdateVMArrayLength ;



		void FireEvent(StateChanged mStateChanged)
		{
			CatchEvents mCatchEvent = new CatchEvents();
			if(mEventsEnabled == true)
			{
				mStateChanged.OnStateChangedEvent+=new DStateChangedEventHandller(mCatchEvent.CatchOnStateChangedEvent);
				mStateChanged.NotifyChange(this, mStateChanged);
			}
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

		public void SetInputDevice()
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();
			Microsoft.DirectX.DirectSound.Device mDevice = new Device(devices[Index].DriverGuid);
			mInputDevice = mDevice;
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
			//m_iCaptureBufferSize = 0;
			//m_iNotifySize = 0;
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
			
			

			if(m_iIndex < 12)
			{	
				m_bitDepth = m_wFormat.BitsPerSample;
				m_SampleRate = m_wFormat.SamplesPerSecond;
				m_Channels = m_wFormat.Channels;
			}
			else
			{
				throw new Exception("index out of range");
			}
			
				
				m_FrameSize = m_wFormat.BlockAlign;
			

				return Info.format;
		}
		
		internal string GenerateFileName (string ext, string sDir)
		{
			int i = 0 ;
			string sTemp ;
			sTemp = sDir + "\\" + i.ToString() + ext ;
			//FileInfo file = new FileInfo(sTemp) ;

			while (File.Exists(sTemp) && i<90000)
			{
				i++;
				sTemp = sDir + "\\" + i.ToString() + ext ;

			}

			if (i<90000)
			{
				return sTemp ;
			}
			else
			{
				return null ;
			}
		}

			public string GetFileName()
			{
				ProjectDirectory = AssetManager.ProjectDirectory;
				string name= GenerateFileName(".wav", ProjectDirectory);
				m_sFileName = ProjectDirectory+"\\"+name;
				return m_sFileName;
			}
		
		//it creates the buffer for recording
		public void CreateCaptureBuffer(int Index)
		{

			
			// Desc: Creates a capture buffer and sets the format 
			if (null != applicationNotify)
			{
				applicationNotify.Dispose();
				applicationNotify = null;
			}
			if (null != applicationBuffer)
			{
				applicationBuffer.Dispose();
				applicationBuffer = null;
			}
			InputFormat = GetInputFormat(Index);
			if(0 == InputFormat.Channels)
				return;
			m_iNotifySize = (1024 > InputFormat.AverageBytesPerSecond / 8) ? 1024 : (InputFormat.AverageBytesPerSecond / 8);
			m_iNotifySize -= m_iNotifySize % InputFormat.BlockAlign;   

			// Set the buffer sizes
			m_iCaptureBufferSize = m_iNotifySize * NumberRecordNotifications;

			// calculate the size of VuMeter Update array length
			m_UpdateVMArrayLength = m_iCaptureBufferSize/ 50 ;
			
			CalculationFunctions cf = new CalculationFunctions();
			m_UpdateVMArrayLength = Convert.ToInt32 (cf.AdaptToFrame ( Convert.ToInt32 ( m_UpdateVMArrayLength ),  m_FrameSize)  );
			arUpdateVM = new byte [ m_UpdateVMArrayLength ] ;

			// Create the capture buffer
			dsc.BufferBytes = m_iCaptureBufferSize;
			
			InputFormat.FormatTag = WaveFormatTag.Pcm;
			
			dsc.Format = InputFormat; // Set the format during creatation
			
			m_cApplicationDevice= InitDirectSound();
			
			applicationBuffer = new CaptureBuffer(dsc, m_cApplicationDevice);


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
		
		//  Copies data from the capture buffer to the output buffer 
		public void RecordCapturedData( ) 
		{	
			int ReadPos ;
			int CapturePos ;
			int LockSize ;
			applicationBuffer.GetCurrentPosition(out CapturePos, out ReadPos);
			ReadPos = CapturePos;

			if (ReadPos < ((m_iCaptureBufferSize)- m_UpdateVMArrayLength  ) )
			{
				Array.Copy ( applicationBuffer.Read(ReadPos , typeof (byte) , LockFlag.None , m_UpdateVMArrayLength  ) , arUpdateVM , m_UpdateVMArrayLength  ) ;				
				ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
			}
			LockSize = ReadPos - NextCaptureOffset;
			if (LockSize < 0)
				LockSize += m_iCaptureBufferSize;
			// Block align lock size so that we are always write on a boundary
			LockSize -= (LockSize % m_iNotifySize);
			if (0 == LockSize)
				return;
			// Read the capture buffer.
			CaptureData = (byte[])applicationBuffer.Read(NextCaptureOffset, typeof(byte), LockFlag.None, LockSize);
			FileInfo fi = new FileInfo(m_sFileName);
			Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
			// Write the data into the wav file");	   
			Writer.BaseStream.Position = (long)(fi.Length);
			//Writer.Seek(0, SeekOrigin.End);			
			Writer.Write(CaptureData, 0, CaptureData.Length);
			Writer.Close();
//			Writer = null;
			// Update the number of samples, in bytes, of the file so far.
			//SampleCount+= datalength;
			SampleCount+=(long)CaptureData.Length;

			// Move the capture offset along
			NextCaptureOffset+= CaptureData.Length ; 
			NextCaptureOffset %= m_iCaptureBufferSize; // Circular buffer
		}
		
		//it will create a wave skeleton for the wave file
		public void CreateRIFF(BinaryWriter Writer, string FileName)
		{	
			// Open up the wave file for writing.
			InputFormat = GetInputFormat(Index);
			// Set up file with RIFF chunk info.
			char[] ChunkRiff = {'R','I','F','F'};
			char[] ChunkType = {'W','A','V','E'};
			char[] ChunkFmt	= {'f','m','t',' '};
			char[] ChunkData = {'d','a','t','a'};
			short shPad = 1; // File padding
			 int nFormatChunkLength = 0x10; 
			// Format chunk length.
			short shBytesPerSample  = 0;
			// Figure out how many bytes there will be per sample.
			if (8 == InputFormat.BitsPerSample && 1 == InputFormat.Channels)
				shBytesPerSample= 1;
			else if ((8 == InputFormat.BitsPerSample && 2 == InputFormat.Channels) || (16 == InputFormat.BitsPerSample && 1 == InputFormat.Channels))
				shBytesPerSample = 2;
			else if (16 == InputFormat.BitsPerSample && 2 == InputFormat.Channels)
				shBytesPerSample = 4;
			// Fill in the riff info for the wave file.
			Writer.Write(ChunkRiff);
			Writer.Write(1);
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
			Writer.BaseStream.Position = 36;
			Writer.Write(ChunkData);
			Writer.BaseStream.Position = 40;
			Writer.Write(0);	// The sample length will be written in later
			Writer.Close();
			//Writer = null;
}
		

		
		public void StartListening(IAudioMediaAsset asset)
		{
			StateChanged mStateChanged = new StateChanged(mState );
			mState = AudioRecorderState.Listening;
			FireEvent(mStateChanged);			
			//m_AudioMediaAsset = new AudioMediaAsset(asset.Path);
//			m_AudioMediaAsset = asset;
			m_sFileName= GetFileName();
			OldAsset = new AudioMediaAsset(asset.Path);
			Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
			CreateRIFF(Writer, m_sFileName);
			InitRecording(true);
		}

		public void StartRecording(IAudioMediaAsset asset)
		{
			m_sFileName = GetFileName();
			OldAsset= new AudioMediaAsset(asset.Path);
			Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
			CreateRIFF(Writer, m_sFileName);
			InitRecording(true);
		}

		//it will start actual recording, append if there is data 
		//in the wave file through the RecordCaptureData()
		public void InitRecording(bool SRecording)
		{
			//if no device is set then it is informed then no device is set
			if(null == m_cApplicationDevice)
				throw new Exception("no device is set for recording");
			//format of the capture buffer and the input format is compared
			//if not same then it is informed that formats do not match

			if(dsc.Format.ToString() != InputFormat.ToString())
				throw new Exception("formats do not match");

			if(SRecording)
			{
				StateChanged mStateChanged = new StateChanged(mState );
				mState = AudioRecorderState.Recording;
				FireEvent(mStateChanged);
				CreateCaptureBuffer(Index);
				applicationBuffer.Start(true);//it will set the looping till the stop is used
			}
			else
			{	
				applicationBuffer.Stop();
				RecordCapturedData();
				Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
				long Audiolength = (long)(SampleCount+44);
				CalculationFunctions cf = new CalculationFunctions();
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 4 ;
					Writer.Write (Convert.ToByte (cf.ConvertFromDecimal (Audiolength)[i])) ;

				}
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 40 ;
					Writer.Write (Convert.ToByte (cf.ConvertFromDecimal (SampleCount )[i])) ;
				}
				Writer.Close();	// Close the file now.
				Writer = null;	// Set the writer to null.
				//m_AudioMediaAsset = new AudioMediaAsset(ProjectDirectory+"\\"+m_sFileName);
				m_AudioMediaAsset = new AudioMediaAsset(m_sFileName);
				if(OldAsset.SizeInBytes >44)
				{
					OldAsset.MergeWith(m_AudioMediaAsset);
					m_AudioMediaAsset = OldAsset;
				}
			}
		}



			
		
		
		//it stops the recording
		public void StopRecording()
		{
			// name:  this is to stop the recording
			// desc:  this will first check the condition and stops the recording and then capture any left  overs recorded data which is not saved
			StateChanged mStateChanged = new StateChanged(mState);
			mState = AudioRecorderState.Idle;
			FireEvent(mStateChanged);

			Capturing = false;
			NotificationEvent.Set();
				
			if(null != applicationBuffer)
				if (applicationBuffer.Capturing)
					InitRecording(false);
		}			
	}
}
