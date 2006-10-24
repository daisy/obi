using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Threading;

namespace Obi.Audio
{
    /// <summary>
    /// The three states of the audio recorder.
    /// NotReady: the recorder is not ready to record, for whatever reason.
    /// Idle: the recorder is ready to record.
    /// Listening: the recording is listening but not writing any data.
    /// Recording: sound is currently being recorded.
    /// </summary>
    public enum AudioRecorderState { NotReady, Idle, Listening, Recording };

	public class AudioRecorder
	{
		public event Events.Audio.Recorder.StateChangedHandler StateChanged;
		public event Events.Audio.Recorder.UpdateVuMeterHandler UpdateVuMeterFromRecorder;

		//member variables
		//the directory to hold the recorded files
		private string sProjectDirectory; 
		//the variables for current position and current time for VuMeter
		long CurrentPositionInByte ;
		private double dCurrentTime;
        private double mTime;

        

		public double CurrentTime
		{
			get
			{
				return dCurrentTime;
			}
		}

		// basic elements of WaveFormat
		internal int m_Channels;
		internal int m_bitDepth;
		internal int m_SampleRate;
		internal int m_FrameSize;
		//  state of AudioRecorder
		AudioRecorderState mState;
		//variables from DirectX.DirectSound
		// Microsoft.DirectX.DirectSound.Device m_InputDevice;
        InputDevice mDevice;
		private bool Capturing;
		// private Capture m_cApplicationDevice;
		private int m_iNotifySize;
		private int m_iCaptureBufferSize;
		private const int NumberRecordNotifications = 16;
		private BufferPositionNotify[] PositionNotify = new BufferPositionNotify[NumberRecordNotifications + 1];  
		private int NextCaptureOffset;
		private long SampleCount = 0;
		private AutoResetEvent NotificationEvent = null;
		private Thread NotifyThread = null;
		private CaptureBuffer applicationBuffer;	
		private Notify applicationNotify;
		WaveFormat InputFormat;

		private string m_sFileName;
		
		//		 array for update current amplitude to VuMeter
		internal byte [] arUpdateVM ;
		internal int m_UpdateVMArrayLength ;

		VuMeter ob_VuMeter;
		
		Assets.AudioMediaAsset m_AudioMediaAsset; 
		
		//this is the audio asset which will be used to store the old asset
		Assets.AudioMediaAsset mAsset;
		

		private static readonly AudioRecorder instance = new AudioRecorder();
		
		public static AudioRecorder Instance
		{ 
			get 
			{ 
				return instance; 
			} 
		}

		// constructor, made private by JQ 
		private AudioRecorder()
		{
            mState = AudioRecorderState.Idle;
            Capturing = false;
            ob_VuMeter = null;
		}

		public int SampleRate
		{
			get
			{
				return m_SampleRate;

			}
		}
		
		public int BitDepth
		{
			get
			{
				return Convert.ToInt16 (m_bitDepth);
			}
		}
		
		public int Channels
		{
			get
			{
				return m_Channels;
			}
		}
				
		//state of the AudioRecorder
		public AudioRecorderState State
		{
			get
			{
				return mState;
			}
		}

		// returns a list of input devices
		/*public ArrayList GetInputDevices()
		{
			// gathers the available capture devices
			CaptureDevicesCollection devices = new CaptureDevicesCollection();  
			
			//ArrayList to collect all the availabel devices
			ArrayList m_devicesList = new ArrayList();
			foreach (DeviceInformation info in devices)
			{
				m_devicesList.Add(info.Description);
			}
			return m_devicesList;
		}*/

        private List<InputDevice> mInputDevicesList = null;

        public List<InputDevice> InputDevices
        {
            get
            {
                if (mInputDevicesList == null)
                {
                    CaptureDevicesCollection devices = new CaptureDevicesCollection();
                    mInputDevicesList = new List<InputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        mInputDevicesList.Add(new InputDevice(info.Description, new Capture(info.DriverGuid)));
                    }
                }
                return mInputDevicesList;
            }
        }

		public void StartListening(Assets.AudioMediaAsset asset)
		{
            Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
			mState = AudioRecorderState.Listening;
			StateChanged(this, e);
			m_Channels = asset.Channels;
			m_bitDepth = asset.BitDepth;
			m_SampleRate = asset.SampleRate;
			// mAsset = new Assets.AudioMediaAsset(m_Channels, m_bitDepth, m_SampleRate);
			// mAsset = asset.Copy() as Assets.AudioMediaAsset;
            // Assets.AssetManager manager = asset.Manager as Assets.AssetManager;
            // sProjectDirectory= manager.AssetsDirectory ;
            mAsset = asset;
            sProjectDirectory = asset.Manager.AssetsDirectory;
			InputFormat = GetInputFormat();
			m_sFileName = sProjectDirectory+"\\"+"Listen.wav";
			BinaryWriter ListenWriter  = new BinaryWriter(File.Create(m_sFileName));
			CreateRIFF(ListenWriter);
			CreateCaptureBuffer();
			InitRecording(true);
		}
		
		//it will start actual recording, append if there is data 
		//in the wave file through the RecordCaptureData()
		public void StartRecording(Assets.AudioMediaAsset asset)
		{	
            Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
			mState = AudioRecorderState.Recording;
			StateChanged(this, e);
			m_Channels = asset.Channels;
			m_SampleRate = asset.SampleRate;
			m_bitDepth = asset.BitDepth;
			// mAsset = new Assets.AudioMediaAsset(m_Channels, m_bitDepth, m_SampleRate);  // why create a new asset here?
			// mAsset = asset.Copy() as Assets.AudioMediaAsset;
			// Assets.AssetManager manager = mAsset.Manager as Assets.AssetManager;
            mAsset = asset;
			sProjectDirectory= asset.Manager.AssetsDirectory ;
		    InputFormat = GetInputFormat();
            if (File.Exists(sProjectDirectory+"\\"+"Listen.wav"))
                File.Delete(sProjectDirectory+"\\"+"Listen.wav");
            // m_sFileName = GetFileName();
            m_sFileName = asset.Manager.UniqueFileName(".wav");
			BinaryWriter bw = new BinaryWriter(File.Create(m_sFileName));
			CreateRIFF(bw);
			CreateCaptureBuffer();
			InitRecording(true);
		}

		// this is to stop the recording
		// desc:  this will first check the condition and stops the recording and then capture any left  overs recorded data which is not saved
        public void StopRecording()
        {
            if (mState == AudioRecorderState.Recording || mState == AudioRecorderState.Listening)
            {
                Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
                mState = AudioRecorderState.Idle;
                StateChanged(this, e);
                if (null != NotificationEvent)
                {
                    Capturing = false;
                    NotificationEvent.Set();
                }
                if (null != applicationBuffer)
                    if (applicationBuffer.Capturing)
                        InitRecording(false);
                FileInfo fi = new FileInfo(m_sFileName);
                if (File.Exists(sProjectDirectory + "\\" + "Listen.wav"))
                    File.Delete(sProjectDirectory + "\\" + "Listen.wav");
                if (File.Exists(m_sFileName))
                    if (fi.Length == 44)
                        File.Delete(m_sFileName);
            }
        }

        /// <summary>
        /// Stop recording when something went wrong.
        /// </summary>
        internal void EmergencyStop()
        {
            Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
            mState = AudioRecorderState.Idle;
            StateChanged(this, e);
            if (null != NotificationEvent)
            {
                Capturing = false;
                NotificationEvent.Set();
            }
            if (null != applicationBuffer)
            {
                if (applicationBuffer.Capturing)
                {
                    applicationBuffer.Stop();
                }
            }
            FileInfo fi = new FileInfo(m_sFileName);
            if (File.Exists(sProjectDirectory + "\\" + "Listen.wav"))
                File.Delete(sProjectDirectory + "\\" + "Listen.wav");
            if (File.Exists(m_sFileName))
                if (fi.Length == 44)
                    File.Delete(m_sFileName);
        }


		/*public Capture InitDirectSound(int Index)
		{
			CaptureDevicesCollection devices = new CaptureDevicesCollection();
			Guid mGuid  = Guid.Empty;
			mGuid = devices[Index].DriverGuid;
			m_cApplicationDevice = new Capture(mGuid);
			return m_cApplicationDevice;
		}*/	

		

		
		
		public WaveFormat GetInputFormat()
		{				
			m_AudioMediaAsset = new Assets.AudioMediaAsset(m_Channels, m_bitDepth, m_SampleRate);
			InputFormat.Channels = Convert.ToInt16(m_AudioMediaAsset.Channels);
			InputFormat.SamplesPerSecond = m_AudioMediaAsset.SampleRate;
			InputFormat.BitsPerSample = Convert.ToInt16(m_AudioMediaAsset.BitDepth);
			InputFormat.AverageBytesPerSecond = m_AudioMediaAsset.SampleRate * m_AudioMediaAsset.FrameSize;
			InputFormat.BlockAlign = Convert.ToInt16(m_AudioMediaAsset.FrameSize);
			m_FrameSize = m_AudioMediaAsset.FrameSize;
			//			m_Channels = m_AudioMediaAsset.Channels;
			//m_SampleRate =  m_AudioMediaAsset.SampleRate;
			return InputFormat;
		}

		public void CreateRIFF(BinaryWriter Writer)
		{	
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
			m_sFileName= GenerateFileName(".wav", sProjectDirectory);
			return m_sFileName;
		}
		
		public VuMeter VuMeterObject
		{
			get
			{
				return ob_VuMeter ;
			}
			set
			{
				ob_VuMeter = value;
			}
		}

		public void CreateCaptureBuffer()
		{	
			// Desc: Creates a capture buffer and sets the format 
			CaptureBufferDescription dsc = new CaptureBufferDescription();
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
			if(0 == InputFormat.Channels)
				return;
			m_iNotifySize = (1024 > InputFormat.AverageBytesPerSecond / 8) ? 1024 : (InputFormat.AverageBytesPerSecond / 8);
			m_iNotifySize -= m_iNotifySize % InputFormat.BlockAlign;   
			// Set the buffer sizes
			m_iCaptureBufferSize = m_iNotifySize * NumberRecordNotifications;
			//calculate the size of VuMeter Update array length
			/*
						m_UpdateVMArrayLength = m_iCaptureBufferSize/ 50 ;
						CalculationFunctions cf = new CalculationFunctions();
						m_UpdateVMArrayLength = Convert.ToInt32 (cf.AdaptToFrame ( Convert.ToInt32 ( m_UpdateVMArrayLength ),  m_FrameSize)  );
						arUpdateVM = new byte [ m_UpdateVMArrayLength ] ;
			*/			
			// Create the capture buffer
			dsc.BufferBytes = m_iCaptureBufferSize;
			InputFormat.FormatTag = WaveFormatTag.Pcm;
			// Set the format during creatation
			dsc.Format = InputFormat; 
			// applicationBuffer = new CaptureBuffer(dsc, this.m_cApplicationDevice);
            applicationBuffer = new CaptureBuffer(dsc, mDevice.Capture);
            NextCaptureOffset = 0;
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
			// Setup the notification POSITIONS 
			for (int i = 0; i < NumberRecordNotifications; i++)
			{
				PositionNotify[i].Offset = (m_iNotifySize * i) + m_iNotifySize - 1;
                PositionNotify[i].EventNotifyHandle = NotificationEvent.SafeWaitHandle.DangerousGetHandle();
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
				//NotificationEvent = new AutoResetEvent(false);
				NotificationEvent.WaitOne(Timeout.Infinite, true);
				//waits for infinite time span before recieving a signal
				RecordCapturedData();
			}
		}
		
		//  Copies data from the capture buffer to the output buffer 
		public void RecordCapturedData( ) 
		{	
			int ReadPos ;
			byte[] CaptureData = null;
			int CapturePos ;
			int LockSize ;
			applicationBuffer.GetCurrentPosition(out CapturePos, out ReadPos);
                        long mPosition = (long)CapturePos;
            CurrentPositionInByte = SampleCount + mPosition;
            	dCurrentTime = CalculationFunctions.ConvertByteToTime(CurrentPositionInByte, m_SampleRate, m_FrameSize);
			m_UpdateVMArrayLength = m_iCaptureBufferSize/ 50 ;
			m_UpdateVMArrayLength = Convert.ToInt32 (CalculationFunctions.AdaptToFrame ( Convert.ToInt32 ( m_UpdateVMArrayLength ),  m_FrameSize)  );

			arUpdateVM = new byte [ m_UpdateVMArrayLength ] ;
			ReadPos = CapturePos;

			if (ReadPos < ((m_iCaptureBufferSize)- m_UpdateVMArrayLength  ) )
			{
				Array.Copy ( applicationBuffer.Read(ReadPos , typeof (byte) , LockFlag.None , m_UpdateVMArrayLength  ) , arUpdateVM , m_UpdateVMArrayLength  ) ;				
				//ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
				UpdateVuMeterFromRecorder(this, new Events.Audio.Recorder.UpdateVuMeterEventArgs());
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
			BinaryWriter Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
			// Write the data into the wav file");	   
			Writer.BaseStream.Position = (long)(fi.Length);
			//Writer.Seek(0, SeekOrigin.End);			
			Writer.Write(CaptureData, 0, CaptureData.Length);
			Writer.Close();
			Writer = null;
			NotifyThread = null;	
			// Update the number of samples, in bytes, of the file so far.
			//SampleCount+= datalength;
			SampleCount+=(long)CaptureData.Length;
			// Move the capture offset along
			NextCaptureOffset+= CaptureData.Length ; 
			NextCaptureOffset %= m_iCaptureBufferSize; // Circular buffer
            long mLength = (long)fi.Length;
            mTime = Audio.CalculationFunctions.ConvertByteToTime(mLength, m_SampleRate, mAsset.FrameSize);
}

		internal long GetCurrentPositioninBytes
		{
			get
			{
				return CurrentPositionInByte;
			}
			set
			{
				CurrentPositionInByte= value;
			}
		}

		internal double  GetTime
		{
			get
			{
				return dCurrentTime;
			}
			set
			{
				dCurrentTime = value;
			}
		}

        public double TimeOfAsset
        {
            get
            {
            return mTime;
            }
            set{
                mTime = value;
            }
        }






		
		public void InitRecording(bool SRecording)
		{	
			//if no device is set then it is informed then no device is set
			//if(null == m_cApplicationDevice)
			if (mDevice.Capture == null) throw new Exception("no device is set for recording");
			//format of the capture buffer and the input format is compared
			//if not same then it is informed that formats do not match
			if(applicationBuffer.Format.ToString() != InputFormat.ToString())
				throw new Exception("formats do not match");

			if(SRecording)
			{	
				CreateCaptureBuffer();
				applicationBuffer.Start(true);//it will set the looping till the stop is used
			}
			else
			{	
				applicationBuffer.Stop();
				RecordCapturedData();
				BinaryWriter Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
				long Audiolength = (long)(SampleCount+44);
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 4 ;
					Writer.Write (Convert.ToByte (CalculationFunctions.ConvertFromDecimal (Audiolength)[i])) ;
				}
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 40 ;
					Writer.Write (Convert.ToByte (CalculationFunctions.ConvertFromDecimal (SampleCount )[i])) ;
				}
				Writer.Close();	// Close the file now.
				//Set the writer to null.
				Writer = null;	
				SampleCount = 0;
				Audiolength = 0;
				Assets.AudioClip NewRecordedClip = new Assets.AudioClip(m_sFileName);
				mAsset.AddClip(NewRecordedClip);
                mAsset.Manager.AddedClip(NewRecordedClip);
			}
		}

		/*public void SetInputDeviceForRecording(Control FormHandle, int Index)
		{
			CaptureDevicesCollection devices  = new CaptureDevicesCollection();
			Microsoft.DirectX.DirectSound.Device mDevice = new  Microsoft.DirectX.DirectSound.Device ( devices[Index].DriverGuid);
			mDevice .SetCooperativeLevel(FormHandle, CooperativeLevel.Priority);
			m_InputDevice = mDevice;
		}

		public void SetInputDeviceForRecording(Control FormHandle, string name)
		{
			SetInputDeviceForRecording(FormHandle, 0);
		}*/

        /*public void SetDevice(Control handle, InputDevice device)
        {
            mDevice = device;
            //mDevice.Capture.SetCooperativeLevel(handle, CooperativeLevel.Priority);
        }*/

        public void SetDevice(Control handle, string name)
        {
            List<InputDevice> devices = InputDevices;
            InputDevice found = devices.Find(delegate(InputDevice d) { return d.Name == name; });
            if (found != null)
            {
                mDevice = found;
            }
            else if (devices.Count > 0)
            {
                mDevice = devices[0];
            }
            else
            {
                throw new Exception("No input device available.");
            }
        }

		/*public Device InputDevice
		{
			get
			{
				return m_InputDevice;
			}
			set
			{
				m_InputDevice = value;
			}
		}*/

        public InputDevice InputDevice
        {
            get { return mDevice; }
            set { mDevice = value; }
        }
    }//end of AudioRecorder Class
}//end
