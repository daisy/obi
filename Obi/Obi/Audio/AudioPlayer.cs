using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace Obi.Audio
{
    /// <summary>
    /// The three states of the audio player.
    /// NotReady: the player is not ready, for whatever reason.
    /// Playing: sound is currently playing.
    /// Paused: playback was interrupted and can be resumed.
    /// Stopped: player is idle.
    /// </summary>
    public enum AudioPlayerState { NotReady, Stopped, Playing, Paused };

	public class AudioPlayer
	{
		// Events of the audio player (JQ)
		public event Events.Audio.Player.EndOfAudioAssetHandler EndOfAudioAsset;
		public event Events.Audio.Player.StateChangedHandler StateChanged;
		public event Events.Audio.Player.UpdateVuMeterHandler UpdateVuMeter;

		// declare member variables
		private Assets.AudioMediaAsset m_Asset ;
		private SecondaryBuffer SoundBuffer;
  private OutputDevice mDevice;
		
		private BufferDescription BufferDesc = null;	
		private int m_BufferCheck ;

        // Size of buffer created for playing
		private int m_SizeBuffer ;

        // length of buffer to be refreshed during playing
		private int m_RefreshLength ;

        // Total length of audio asset being played
		private long m_lLength;

        // Length of audio asset in bytes which had been played
		private long m_lPlayed ;
		private Thread RefreshThread;

        // variable to hold stop position in buffer after audio asset is about to end and all refreshing is finished
        int m_BufferStopPosition= -1 ;

        // step count to be used for compressing in fast play
		private int m_Step = 1;
		internal int m_FrameSize ;
		internal int m_Channels ;
		private int m_SamplingRate ;
        private int m_VolumeLevel ;
		private AudioPlayerState  m_State;
		private int m_CompAddition = 0 ;
		private long m_lClipByteCount ;

		private VuMeter ob_VuMeter;

        // monitoring timer to trigger events independent of refresh thread
        private System.Windows.Forms.Timer MoniteringTimer = new System.Windows.Forms.Timer();

        // Flag to trigger end of asset events, flag is set for a moment and again reset
        private bool m_IsEndOfAsset = false ;


		private static readonly AudioPlayer mInstance = new AudioPlayer();

		public static AudioPlayer Instance
		{
			get { return mInstance; }
		}

		// JQ changed constructor to be private (singleton)
		private AudioPlayer()
        {

			m_State = AudioPlayerState.Stopped;
            ob_VuMeter = null;
            MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
            MoniteringTimer.Interval = 200;
            // events associated with local function so as to avoid null exceptions            
            StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(CatchEvents);
            EndOfAudioAsset += new Obi.Events.Audio.Player.EndOfAudioAssetHandler(CatchEvents);
		}

		// bool variable to enable or disable event
		public bool m_EventsEnabled = true ;

        /// <summary>
        /// The Vu meter associated with the player.
        /// </summary>
		public VuMeter VuMeter
		{
			get { return ob_VuMeter; }
            set
            {
                ob_VuMeter = value;
            }
		}

		void TriggerStateChangedEvent(Events.Audio.Player.StateChangedEventArgs e)
		{
			if (m_EventsEnabled) StateChanged(this, e);
		}

		// array for update current amplitude to VuMeter
		internal byte [] arUpdateVM ;
		internal int m_UpdateVMArrayLength ;

		// gets the current AudioPlayer state
		public AudioPlayerState State
		{
			get
			{
				return m_State ;
			}
		}

        public OutputDevice OutputDevice
        {
            get { return mDevice; }
        }

		public Assets.AudioMediaAsset CurrentAsset
		{
			get
			{
				return m_Asset ;
			}
		}

        public int OutputVolume
        {
            get
            {
                return m_VolumeLevel;
            }
            set
            {
                SetVolumeLevel(value);
            }
        }

		public int CompFactor
		{
			get
			{
				return m_Step ;
			}
			set
			{
				Set_m_Step (value) ;
			}
		}

		public long CurrentBytePosition
		{
			get
			{
				return GetCurrentBytePosition () ;
			}
			set
			{
				SetCurrentBytePosition (value) ;
			}
		}

		public double CurrentTimePosition
		{
			get
			{
				return GetCurrentTimePosition () ;
			}
			set
			{
				SetCurrentTimePosition (value) ;
			}
		}

        // Sets the output volume
        void SetVolumeLevel(int VolLevel)
        {
            m_VolumeLevel = VolLevel;

            if (SoundBuffer != null)
                SoundBuffer.Volume = m_VolumeLevel;

        }


		// checks the input value of compression factor and sets it for fast play
		// Default value  is 10 i.e. 80% time compression
		void Set_m_Step(int l_Step)
		{
			if (l_Step == 1)
			{
				//m_FastPlay = false ;
			}
			else if (l_Step >2&& l_Step <20)
			{
				//m_FastPlay = true ;
				m_Step = l_Step ;
			}
			else
			{
				throw new Exception ("Invalid Compression Factor") ;
			}
		}

        private List<OutputDevice> mOutputDevicesList = null;

        public List<OutputDevice> OutputDevices
        {
            get
            {
                if (mOutputDevicesList == null)
                {
                    DevicesCollection devices = new DevicesCollection();
                    mOutputDevicesList = new List<OutputDevice>(devices.Count);
                    foreach (DeviceInformation info in devices)
                    {
                        mOutputDevicesList.Add(new OutputDevice(info.Description, new Device(info.DriverGuid)));
                    }
                }
                return mOutputDevicesList;
            }
        }


        /// <summary>
        /// Set the device to be used by the player.
        /// </summary>
        public void SetDevice(Control handle, OutputDevice device)
        {
            mDevice = device;
            mDevice.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
        }

		/// <summary>
		/// Set the device that matches this name; if it could not be found, default to the first one.
        /// Throw an exception if no devices were found.
		/// </summary>
		public void SetDevice(Control FormHandle, string name)
		{
            List<OutputDevice> devices = OutputDevices;
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            if (found != null)
            {
                SetDevice(FormHandle, found);
            }
            else if (devices.Count > 0)
            {
                SetDevice(FormHandle, devices[0]);
            }
            else
            {
                throw new Exception("No output device available.");
            }
        }

		

		public void Play(Assets.AudioMediaAsset asset )
		{
			m_StartPosition   = 0 ;
			m_State  = AudioPlayerState.NotReady ;
			m_Asset = asset as Assets.AudioMediaAsset;

            if (m_Asset.AudioLengthInBytes != 0)
                InitPlay(0, 0);
            else
                SimulateEmptyAssetPlaying();

		}

		void InitPlay(long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (m_State != AudioPlayerState.Playing )
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, m_Asset.FrameSize);
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, m_Asset.FrameSize);
                m_SamplingRate = m_Asset.SampleRate;

                // lEndPosition = 0 means that file is played to end
                if (lEndPosition != 0)
                {
                    m_lLength = (lEndPosition) - lStartPosition;
                }
                else
                {
                    // folowing one line is modified on 2 Aug 2006
                    //m_lLength = (m_Asset .SizeInBytes  - lStartPosition ) ;
                    m_lLength = (m_Asset.SizeInBytes);
                }

                WaveFormat newFormat = new WaveFormat();
                BufferDesc = new BufferDescription();
                BufferDesc.ControlVolume = true;

                // retrieve format from file
                m_FrameSize = m_Asset.FrameSize;
                m_Channels = m_Asset.Channels;
                newFormat.AverageBytesPerSecond = m_Asset.SampleRate * m_Asset.FrameSize;
                newFormat.BitsPerSample = Convert.ToInt16(m_Asset.BitDepth);
                newFormat.BlockAlign = Convert.ToInt16(m_Asset.FrameSize);
                newFormat.Channels = Convert.ToInt16(m_Asset.Channels);

                newFormat.FormatTag = WaveFormatTag.Pcm;

                newFormat.SamplesPerSecond = m_Asset.SampleRate;

                // loads  format to buffer description
                BufferDesc.Format = newFormat;

                // calculate size of buffer so as to contain 1 second of audio
                m_SizeBuffer = m_Asset.SampleRate * m_Asset.FrameSize;
                m_RefreshLength = (m_Asset.SampleRate / 2) * m_Asset.FrameSize;
                /*
                if (m_SizeBuffer > m_lLength - lStartPosition)
                {
                    m_SizeBuffer = Convert.ToInt32(m_lLength - lStartPosition);
                    m_RefreshLength = m_SizeBuffer / 2;
                    m_RefreshLength = ( m_RefreshLength / m_FrameSize ) * m_FrameSize ;
                }
                */
                // calculate the size of VuMeter Update array length
                m_UpdateVMArrayLength = m_SizeBuffer / 20;
                m_UpdateVMArrayLength = Convert.ToInt32(CalculationFunctions.AdaptToFrame(Convert.ToInt32(m_UpdateVMArrayLength), m_FrameSize));
                arUpdateVM = new byte[m_UpdateVMArrayLength];
                // reset the VuMeter (if set)
                if (ob_VuMeter != null) ob_VuMeter.Reset();

                // sets the calculated size of buffer
                BufferDesc.BufferBytes = m_SizeBuffer;

                // Global focus is set to true so that the sound can be played in background also
                BufferDesc.GlobalFocus = true;

                // initialising secondary buffer
                // SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
                SoundBuffer = new SecondaryBuffer(BufferDesc, mDevice.Device);

                // Compensate played length due to the skip of frames during compression
                if (m_Step != 1)
                {
                    m_CompAddition = (m_RefreshLength * 2) / m_Step;
                    m_CompAddition = Convert.ToInt32(CalculationFunctions.AdaptToFrame(m_CompAddition, m_FrameSize));
                }

                // Load from file to memory
                LoadStream(true);

                    SoundBuffer.Write(0, m_MemoryStream, m_SizeBuffer, 0);
                
                // Adds the length (count) of file played into a variable
                // Folowing one line was modified on 2 Aug 2006 i.e lStartPosition is added
                m_lPlayed = m_SizeBuffer + (2 * m_CompAddition) + lStartPosition;


                // trigger  events (modified JQ)
                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
                m_State = AudioPlayerState.Playing;
            TriggerStateChangedEvent(e);
                    
                MoniteringTimer.Enabled = true;
                // starts playing
                SoundBuffer.Play(0, BufferPlayFlags.Looping);
                m_BufferCheck = 1;

                //initialise and start thread for refreshing buffer
                RefreshThread = new Thread(new ThreadStart(RefreshBuffer));
                RefreshThread.Start();
            }// end of state check
			// end of function
		}


		
		void RefreshBuffer ()
		{
		
			int ReadPosition;
			
			// variable to prevent least count errors in clip end time
            long SafeMargin = CalculationFunctions.ConvertTimeToByte(1, m_SamplingRate, m_FrameSize);


			while (m_lPlayed < m_lLength - SafeMargin )
			{//1
				if (SoundBuffer.Status.BufferLost  )
					SoundBuffer.Restore () ;

				
				Thread.Sleep (50) ;

                if (ob_VuMeter != null)
                {
                    ReadPosition = SoundBuffer.PlayPosition;

                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(SoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);
                        //if ( m_EventsEnabled == true)
                        //ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
                        UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ
                    }
                }
				// check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
				if ((m_BufferCheck% 2) == 1 &&  SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
						LoadStream (false) ;
							SoundBuffer.Write (0 , m_MemoryStream, m_RefreshLength, 0) ;
					m_lPlayed = m_lPlayed + m_RefreshLength+ m_CompAddition;
					m_BufferCheck++ ;
				}//-1
                    // refresh Rear half of buffer for even count
				else if ((m_BufferCheck % 2 == 0) &&  SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
						LoadStream (false) ;
							SoundBuffer.Write (m_RefreshLength, m_MemoryStream, m_RefreshLength, 0)  ;
					m_lPlayed = m_lPlayed + m_RefreshLength+m_CompAddition ;
					m_BufferCheck++ ;
					// end of even/ odd part of buffer;
				}//-1
					
				// end of while
			}


             m_BufferStopPosition= -1 ;
            if (m_BufferCheck == 1 )
            {
                m_BufferStopPosition = Convert.ToInt32(m_MemoryStreamPosition);
            }

            // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
            else if ((m_BufferCheck % 2) == 0)
            {
                m_BufferStopPosition = Convert.ToInt32 (m_MemoryStreamPosition);
            }
            // if last refresh is at Rear half part then stop position is more than refresh length
            else if ((m_BufferCheck % 2) == 1 )
            {
                m_BufferStopPosition = Convert.ToInt32 (m_MemoryStreamPosition+ m_RefreshLength);
            }

            int CurrentPlayPosition;
            CurrentPlayPosition = SoundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32 (CalculationFunctions.ConvertTimeToByte( 70 , m_SamplingRate, m_FrameSize));

            if ( m_BufferStopPosition < StopMargin)
                m_BufferStopPosition = StopMargin; 

             while (CurrentPlayPosition < (m_BufferStopPosition - StopMargin) || CurrentPlayPosition > ( m_BufferStopPosition ))
                {
                    Thread.Sleep(50);
                    CurrentPlayPosition = SoundBuffer.PlayPosition;
                }

            /*
			// calculate time to stop according to remaining data
			int time ;
			long lRemaining = (m_lPlayed - m_lLength);
			double dTemp ;
			
			dTemp= ((m_RefreshLength + m_RefreshLength - lRemaining			)*1000)/m_RefreshLength ;
			time = Convert.ToInt32(dTemp * 0.48 );

			//if (m_FastPlay == true)
			//time = (time-250) * (1- (2/m_Step));

			Thread.Sleep (time) ;
			*/
			// Stopping process begins
                m_BufferStopPosition = -1 ;
			SoundBuffer.Stop () ;
			if (ob_VuMeter != null) ob_VuMeter.Reset () ;
            //SoundBuffer = null;
				m_br.Close();
				//ob_EndOfAudioAsset.NotifyEndOfAudioAsset ( this , ob_EndOfAudioAsset) ;

			// changes the state and trigger events
			//ob_StateChanged = new StateChanged (m_State) ;

			//EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());  // JQ
                
            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);
            m_IsEndOfAsset = true;
			// RefreshBuffer ends
		}
		
		public void Play(Assets.AudioMediaAsset  asset, double timeFrom)
		{
            m_State = AudioPlayerState.NotReady;
			m_Asset = asset as Assets.AudioMediaAsset;
            if (m_Asset.AudioLengthInBytes > 0)
            {
                long lPosition = CalculationFunctions.ConvertTimeToByte(timeFrom, m_Asset.SampleRate, m_Asset.FrameSize);
                lPosition = CalculationFunctions.AdaptToFrame(lPosition, m_Asset.FrameSize);
                if (lPosition >= 0 && lPosition <= m_Asset.AudioLengthInBytes)
                {
                    m_StartPosition = lPosition;
                    InitPlay(lPosition, 0);
                }
                else throw new Exception("Start Position is out of bounds of Audio Asset");
            }
            else    // if m_Asset.AudioLengthInBytes= 0 i.e. empty asset
            {
                SimulateEmptyAssetPlaying ();
            }

		}


        ///<summary>
        /// Function for simulating playing for assets with no audio
        /// </summary>
        ///
        private void SimulateEmptyAssetPlaying()
        {
            
            Events.Audio.Player.StateChangedEventArgs  e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Playing;
            TriggerStateChangedEvent(e);

            Thread.Sleep(50);

            e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Stopped;
            TriggerStateChangedEvent(e);

            // trigger end of asset event
            if (m_EventsEnabled == true)
                EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
        }
		// contains the end position  to play to be used in starting playing  after seeking
		long lByteTo = 0 ;		
		private void Play(Assets.AudioMediaAsset asset , double timeFrom, double timeTo)
		{
			m_Asset = asset as Assets.AudioMediaAsset;
			long lStartPosition = CalculationFunctions.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, m_Asset .FrameSize) ;
			long lEndPosition = CalculationFunctions.ConvertTimeToByte (timeTo , m_Asset.SampleRate, m_Asset.FrameSize) ;
			lByteTo = lEndPosition ;
			// check for valid arguments
			if (lStartPosition>0 && lStartPosition < lEndPosition && lEndPosition <= m_Asset.AudioLengthInBytes)
			{
				InitPlay ( lStartPosition, lEndPosition );
			}
			else
			{
				MessageBox.Show("Arguments out of range") ;
			}
		}
        private long m_lPausePosition ;
		public void Pause()
		{
			if (m_State.Equals(AudioPlayerState .Playing))
			{
                m_lPausePosition = GetCurrentBytePosition ();
                StopFunction();

//				SoundBuffer.Stop () ;
				// Change the state and trigger event
                
				Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State) ;
                m_State = AudioPlayerState.Paused;
				TriggerStateChangedEvent(e);
                
			}
		}

		public void Resume()
		{
			if (m_State.Equals(AudioPlayerState.Paused))
			{
                
                long lPosition = CalculationFunctions.AdaptToFrame( m_lPausePosition , m_Asset.FrameSize);
                if (lPosition >= 0 && lPosition < m_Asset.AudioLengthInBytes)
                {
                    m_StartPosition = lPosition;
                    InitPlay(lPosition, 0);
                }
                else
                    throw new Exception("Start Position is out of bounds of Audio Asset");

                //Play(m_Asset, m_dPausePosition);

                // comment following three state change event lines because  event  is set to playing by init play
                //m_State = AudioPlayerState.Playing;
				//Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
				//TriggerStateChangedEvent(e) ;
				
			}			
		}

		public void Stop()
		{
			if (m_State != AudioPlayerState.Stopped   )			
			{
				//SoundBuffer.Stop();
                //RefreshThread.Abort();
				//if (ob_VuMeter != null) ob_VuMeter.Reset();			
                StopFunction();
			}
            if ( m_br != null )
            m_br.Close();
            
			Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);
		}

        private void StopFunction ()
        {
            m_BufferStopPosition = -1;
            SoundBuffer.Stop();

            if ( RefreshThread.IsAlive )
                RefreshThread.Abort();

				if (ob_VuMeter != null) ob_VuMeter.Reset();			

            m_br.Close();
        }

		internal long GetCurrentBytePosition()
		{
			int PlayPosition  = SoundBuffer.PlayPosition;
			
			long lCurrentPosition ;

            if (m_BufferStopPosition != -1)
            {
                lCurrentPosition = m_Asset.AudioLengthInBytes  -  (m_BufferStopPosition - PlayPosition);
            }
            //if (PlayPosition < m_RefreshLength) // Avn: changed on19 Dec 2006 for improving get position for pause
            else if (m_BufferCheck % 2 == 1)
			{ 
				// takes the lPlayed position and subtract the part of buffer played from it
				lCurrentPosition = m_lPlayed - ( 2 * m_RefreshLength) + PlayPosition ;
			}
			else
			{
                lCurrentPosition = m_lPlayed - (3 * m_RefreshLength) + PlayPosition;
			}

            if ( lCurrentPosition >= m_Asset.AudioLengthInBytes )
                lCurrentPosition = m_Asset.AudioLengthInBytes - Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, m_SamplingRate, m_FrameSize)); 

			return lCurrentPosition ;
		}

		internal double GetCurrentTimePosition()
		{	
			return CalculationFunctions.ConvertByteToTime (GetCurrentBytePosition() , m_SamplingRate , m_FrameSize);
		}
		
		long m_StartPosition  ;

		void SetCurrentBytePosition (long localPosition) 
		{
			if (localPosition < 0)
				localPosition = 0;

			if (localPosition > m_Asset.AudioLengthInBytes)
				localPosition = m_Asset.AudioLengthInBytes - 100;


			m_EventsEnabled = false ;

			if (SoundBuffer.Status.Looping)
			{

					Stop();
					Thread.Sleep (30) ;
					m_StartPosition = localPosition ;
					InitPlay(localPosition , 0);
				
			}		
			
			else if(m_State.Equals (AudioPlayerState .Paused ) )
			{

					//Stop();
					m_StartPosition = localPosition ;
                    m_lPausePosition = localPosition;
					//Thread.Sleep (20) ;
					//InitPlay(localPosition , 0);
					//Thread.Sleep(30) ;
					//SoundBuffer.Stop () ;
					// Stop () also change the m_Stateto stopped so change it to paused
					//m_State=AudioPlayerState .Paused;
				
				
			}
			m_EventsEnabled = true ;

			// end of set byte position
		}


		void SetCurrentTimePosition (double localPosition) 
		{
			long lTemp = CalculationFunctions.ConvertTimeToByte (localPosition, m_SamplingRate, m_FrameSize);
			SetCurrentBytePosition(lTemp) ;
		}

		MemoryStream m_MemoryStream = new MemoryStream () ;
		BinaryReader m_br  ;
		int m_ClipIndex   ;
		Assets.AudioClip ob_Clip ;

		void LoadStream (bool boolInit  )
		{
			m_MemoryStream.Position = 0 ;
			if (boolInit == true)
			{
				m_StartPosition  = CalculationFunctions.AdaptToFrame (m_StartPosition , m_FrameSize) ;
				double dStartPosition = CalculationFunctions.ConvertByteToTime (m_StartPosition , m_SamplingRate , m_FrameSize) ;
				ArrayList alInfo = new ArrayList (m_Asset.FindClipToProcess(dStartPosition)) ;
				m_ClipIndex = Convert.ToInt32 (alInfo [0] );
				//ob_Clip = m_Asset.m_alClipList [m_ClipIndex] as Assets.AudioClip;
                ob_Clip = m_Asset.Clips[m_ClipIndex];
				double dPositionInClip = Convert.ToDouble (alInfo [1]) + ob_Clip.BeginTime ;
				m_br =new BinaryReader (File.OpenRead(ob_Clip.Path)) ;
				long lPositionInClip = CalculationFunctions.ConvertTimeToByte (dPositionInClip , m_SamplingRate , m_FrameSize) + 44;
                lPositionInClip = CalculationFunctions.AdaptToFrame(lPositionInClip, m_FrameSize);
                m_br.BaseStream.Position = lPositionInClip  ;
                m_lClipByteCount = lPositionInClip - ob_Clip.BeginByte ;
                for (long l = 0; l < ob_Clip.LengthInBytes && l < 2 * (m_RefreshLength); l = l + m_FrameSize)
                {
                    SkipFrames();
                    m_MemoryStream.Write(m_br.ReadBytes(m_FrameSize), 0, m_FrameSize);
                    m_lClipByteCount = m_lClipByteCount + m_FrameSize;
                    ReadNextClip();

                    if ( m_lClipByteCount >=ob_Clip.LengthInBytes  && m_ClipIndex == m_Asset.Clips.Count - 1)
                    break;
                }   
			}
			else
			{
				long l ;
				for (l = 0 ; l < (m_RefreshLength ) && m_lClipByteCount  < ob_Clip.LengthInBytes  ; l = l+m_FrameSize ) 
				{
					SkipFrames () ;
					m_MemoryStream.Write (m_br.ReadBytes(m_FrameSize), 0 , m_FrameSize) ;
					m_lClipByteCount = m_lClipByteCount + m_FrameSize ;
					ReadNextClip () ;
				}
			}
            m_MemoryStreamPosition = m_MemoryStream.Position;
			m_MemoryStream.Position = 0 ;
		}
        long m_MemoryStreamPosition = 0;
		void ReadNextClip ()
		{
			if ( m_lClipByteCount >= ob_Clip.LengthInBytes)
			{
				//if (m_ClipIndex <m_Asset.m_alClipList.Count - 1)
                if (m_ClipIndex < m_Asset.Clips.Count - 1)
				{
					m_ClipIndex++ ;
					//ob_Clip = m_Asset.m_alClipList [m_ClipIndex] as Assets.AudioClip;
                    ob_Clip = m_Asset.Clips[m_ClipIndex];	
					m_br =new BinaryReader (File.OpenRead(ob_Clip.Path)) ;
					m_br.BaseStream.Position = ob_Clip.BeginByte + 44;
					m_lClipByteCount = 0 ;
				}
			}
		}

		void SkipFrames()
		{
			if (m_Step != 1)
			{
				if (m_MemoryStream.Position %    (m_Step * m_FrameSize) == 0 )
				{
					m_br.ReadBytes(m_FrameSize) ;
					m_br.ReadBytes(m_FrameSize) ;
					m_lClipByteCount = m_lClipByteCount + m_FrameSize  + m_FrameSize;
				}
			}
		}

        private void MoniteringTimer_Tick(object sender, EventArgs e)
        {
            if ( m_IsEndOfAsset == true)
            {
                m_IsEndOfAsset = false;
                MoniteringTimer.Enabled = false;

                if (m_EventsEnabled == true) 
                EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());

                
            }

        }


        // function to catch events so as to avoid null  reference exceptions
        private void CatchEvents(object sender, EventArgs e)
        {
            //System.Media.SystemSounds.Asterisk.Play();
        }


		// End Class
	}
	// End NameSpace
}
