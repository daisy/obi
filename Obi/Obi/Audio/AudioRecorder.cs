using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.Threading;

using urakawa.media.data.audio;

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

        private double mCurrentTime; // Time in milliseconds
        
        public event Events.Audio.Recorder.StateChangedHandler StateChanged;
		public event Events.Audio.Recorder.UpdateVuMeterHandler UpdateVuMeterFromRecorder;
        public event Events.Audio.Recorder.ResetVuMeterHandler ResetVuMeter ;


		//member variables
        // member variables initialised only once in a session
        private string sProjectDirectory;  //the directory to hold the recorded files
        InputDevice mDevice;
        private const int NumberRecordNotifications = 16; // number of notifications in capture buffer 


        // member variables which change whenever recording of a new asset starts
        AudioMediaData mAsset; // Asset currently  being  recorded
                private int m_Channels;
        private int m_bitDepth;
        private int m_SampleRate;
        private int m_FrameSize;
        WaveFormat InputFormat; // DX wave format object for DX buffers etc.
        private string m_sFileName; // Full file path of file being recorded
        private CaptureBuffer applicationBuffer; // DX Capture buffer for recording
        private bool Capturing; // Flag to indicate status of capturing
        private Notify applicationNotify;  // DX notification Object to setup capture buffer notifications
        private int m_iCaptureBufferSize; // Size of capture buffer
        private int m_iNotifySize; // size of bytes between two notifications
                private BufferPositionNotify[] PositionNotify; // array containing notification  position in capture buffer
                internal byte[] arUpdateVM; // array for updating VuMeter
        internal int m_UpdateVMArrayLength; // Length of Vumeter array


// member variables  which are re assigned during recording
       AudioRecorderState mState; //  state of AudioRecorder
       //the variables for current position and current time for VuMeter
       long CurrentPositionInByte;
       private double mTime;

        private int NextCaptureOffset; // Offset in DX capture buffer
        private long SampleCount ; // Count of total bytes being recorded at an instance of time
        private AutoResetEvent NotificationEvent = null;
        private Thread NotifyThread = null;
        /*                
		private static readonly AudioRecorder instance = new AudioRecorder();
				public static AudioRecorder Instance
		{ 
			get 
			{ 
				return th; 
			} 
		}
        */
		// constructor, made private by JQ 
		public AudioRecorder()
		{
                        mState = AudioRecorderState.Idle;
                        PositionNotify = new BufferPositionNotify[NumberRecordNotifications + 1];
            Capturing = false;
            SampleCount = 0;
                                    
		}


        /// <summary>
        /// Current time since beginning of recording(?)
        /// </summary>
        public double CurrentTime
        {
            get { return mCurrentTime; }
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

        public string AssetsDirectory
        {
            get
            {
                return sProjectDirectory;
            }
            set
            {
                sProjectDirectory = value;
            }
        }
		
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

		public void StartListening(AudioMediaData  asset)
		{
            Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
			mState = AudioRecorderState.Listening;
			StateChanged(this, e);

			m_Channels = asset.getPCMFormat ().getNumberOfChannels () ;
			m_bitDepth = asset.getPCMFormat ().getBitDepth ()  ;
			m_SampleRate = (int)  asset.getPCMFormat ().getSampleRate ()  ;
            m_FrameSize = (m_bitDepth / 8) * m_Channels;
            
            mAsset = asset;
			InputFormat = GetInputFormat();
            m_sFileName = sProjectDirectory + "\\" + "Listen.wav";
             
			CreateCaptureBuffer();
			InitRecording(true);
		}
		
		//it will start actual recording, append if there is data 
		//in the wave file through the RecordCaptureData()
		public void StartRecording(AudioMediaData  asset)
		{	
            Events.Audio.Recorder.StateChangedEventArgs e = new Events.Audio.Recorder.StateChangedEventArgs(mState);
			mState = AudioRecorderState.Recording;
			StateChanged(this, e);

			m_Channels = asset.getPCMFormat ().getNumberOfChannels () ;
			m_SampleRate = (int)  asset.getPCMFormat ().getSampleRate ()  ;
			m_bitDepth = asset.getPCMFormat ().getBitDepth ()  ;
            m_FrameSize = (m_bitDepth / 8) * m_Channels;

            mAsset = asset;
					    InputFormat = GetInputFormat();
            
             m_sFileName = GetFileName();
			BinaryWriter bw = new BinaryWriter(File.Create(m_sFileName));
			CreateRIFF(bw);
			CreateCaptureBuffer();
			InitRecording(true);
		}
        bool WasListening = false;
		// this is to stop the recording
		// desc:  this will first check the condition and stops the recording and then capture any left  overs recorded data which is not saved
        public void StopRecording()
        {
            if (mState == AudioRecorderState.Listening)
                WasListening = true;
            else
                WasListening = false;

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

		
		public WaveFormat GetInputFormat()
		{				
            
            InputFormat.Channels = Convert.ToInt16(m_Channels );
            InputFormat.SamplesPerSecond =  m_SampleRate ;
            InputFormat.BitsPerSample = Convert.ToInt16(m_bitDepth );
            InputFormat.AverageBytesPerSecond = m_SampleRate * m_FrameSize ;
            InputFormat.BlockAlign = Convert.ToInt16(m_FrameSize );

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
				
				// Create a notification event, for when the sound stops playing
				NotificationEvent = new AutoResetEvent(false);
                NotifyThread.Start();
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
            	mCurrentTime = CalculationFunctions.ConvertByteToTime(CurrentPositionInByte, m_SampleRate, m_FrameSize);

            
			LockSize = ReadPos - NextCaptureOffset;
			if (LockSize < 0)
				LockSize += m_iCaptureBufferSize;
			// Block align lock size so that we are always write on a boundary
			LockSize -= (LockSize % m_iNotifySize);
			if (0 == LockSize)
				return;

            CaptureData = new byte [ LockSize ] ;

			// Read the capture buffer.
            //try
            //{
                CaptureData = (byte[]) applicationBuffer.Read ( NextCaptureOffset , typeof(byte) , LockFlag.None , LockSize );
            //}
            //catch (System.Exception Ex)
            //{
                //MessageBox.Show( "Size" + ( m_iCaptureBufferSize.ToString () ) +  "Cap" + ( NextCaptureOffset.ToString () ) + "Log" + ( LockSize.ToString () ) + Ex.ToString());
            //}

            // make update vumeter array length equal to CaptureData length
                if (CaptureData.Length != arUpdateVM.Length
                    && CaptureData.Length < CalculationFunctions.ConvertTimeToByte(125 , m_SampleRate, m_FrameSize))
                {

                    m_UpdateVMArrayLength = CaptureData.Length;
                    Array.Resize(ref arUpdateVM, CaptureData.Length);
                }

            // copy Capture data to an array and update it to VuMeter
                Array.Copy( CaptureData , arUpdateVM, m_UpdateVMArrayLength);
                UpdateVuMeterFromRecorder(this, new Events.Audio.Recorder.UpdateVuMeterEventArgs());

                if (mState != AudioRecorderState.Listening)
                {
                    FileInfo fi = new FileInfo(m_sFileName);
                    if (fi.Exists)
                    {
                        BinaryWriter Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
                        // Write the data into the wav file");	   
                        Writer.BaseStream.Position = (long)(fi.Length);
                        //Writer.Seek(0, SeekOrigin.End);			
                        Writer.Write(CaptureData, 0, CaptureData.Length);
                        Writer.Close();
                        Writer = null;
                    }
                }
			NotifyThread = null;	
			// Update the number of samples, in bytes, of the file so far.
			//SampleCount+= datalength;
			SampleCount+=(long)CaptureData.Length;
			// Move the capture offset along
			NextCaptureOffset+= CaptureData.Length ; 
			NextCaptureOffset %= m_iCaptureBufferSize; // Circular buffer
            // Below lines commented to eliminate listen file on 2 Fev 2007
            //long mLength = (long)fi.length ;
            // instead of above line following line is used
            long mLength = (long)SampleCount;

            mTime = Audio.CalculationFunctions.ConvertByteToTime(mLength, m_SampleRate, m_FrameSize );
}

		private long GetCurrentPositioninBytes
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

        public double TimeOfAsset
        {
            get
            {
                    return GetCurrentAssetTime();
            }
        }

        private double GetCurrentAssetTime()
        {
            if (mState == AudioRecorderState.Recording && applicationBuffer != null )
            {
                if (applicationBuffer.Capturing)
                {
                    int CapturePos;
                    int ReadPos;
                    applicationBuffer.GetCurrentPosition(out CapturePos, out ReadPos);
                    long mPosition = (long)CapturePos;
                    mPosition = mPosition - NextCaptureOffset;
                    double TimeDifference = CalculationFunctions.ConvertByteToTime(mPosition, m_SampleRate, m_FrameSize);
                    return mTime + TimeDifference;
                }
            }
            
            return 0;
        }


		
		public void InitRecording(bool SRecording)
		{	
			//if no device is set then it is informed then no device is set
			//if(null == m_cApplicationDevice)
			if (mDevice.Capture == null) throw new Exception("no device is set for recording");
			//format of the capture buffer and the input format is compared
			//if not sam2e then it is informed that formats do not match
			if(applicationBuffer.Format.ToString() != InputFormat.ToString())
				throw new Exception("formats do not match");

			if(SRecording)
			{	
				CreateCaptureBuffer();
				applicationBuffer.Start(true);//it will set the looping till the stop is used

                // following lines added to initialise and set array length forupdating VuMeter
                m_UpdateVMArrayLength = m_iCaptureBufferSize / 20;
                m_UpdateVMArrayLength = Convert.ToInt32(CalculationFunctions.AdaptToFrame(Convert.ToInt32(m_UpdateVMArrayLength), m_FrameSize));
                arUpdateVM = new byte[m_UpdateVMArrayLength];

			}
			else
			{
                				applicationBuffer.Stop();
				RecordCapturedData();

                // condition for listening added to eleminate listen file on 2 Feb 2007
                if (WasListening == false)
                {
				BinaryWriter Writer = new BinaryWriter(File.OpenWrite(m_sFileName));
                FileInfo RecordedFile = new FileInfo(m_sFileName);
//				long Audiolength = (long)(SampleCount+44);
                long Audiolength = RecordedFile.Length - 8;
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 4 ;
					Writer.Write (Convert.ToByte (CalculationFunctions.ConvertFromDecimal (Audiolength)[i])) ;
				}
                Audiolength = Audiolength - 36;
				for (int i = 0; i<4 ; i++)
				{
					Writer.BaseStream.Position = i + 40 ;
					Writer.Write (Convert.ToByte (CalculationFunctions.ConvertFromDecimal ( Audiolength )[i])) ;
				}
				Writer.Close();	// Close the file now.
				//Set the writer to null.
				Writer = null;
                Audiolength = 0;
                ///-///
                                mAsset.appendAudioDataFromRiffWave(m_sFileName);
                                }

                SampleCount = 0;
                
			}
            // reset VuMeter
            if (ResetVuMeter != null)
                ResetVuMeter(this, new Obi.Events.Audio.Recorder.UpdateVuMeterEventArgs());
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

        public InputDevice InputDevice
        {
            get { return mDevice; }
            set { mDevice = value; }
        }
    }//end of AudioRecorder Class
}//end
