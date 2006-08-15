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
		// private Microsoft.DirectX.DirectSound.Device SndDevice = null;
		private BufferDescription BufferDesc = null;	
		private int m_BufferCheck ;
		private int m_SizeBuffer ;
		private int m_RefreshLength ;
		private long m_lLength;
		private long m_lPlayed ;
		private Thread RefreshThread;
		private bool m_PlayFile;
		private bool m_FastPlay = false;
		private int m_Step = 1;
		internal int m_FrameSize ;
		internal int m_Channels ;
		private int m_SamplingRate ;
		private AudioPlayerState  m_State;
		private int m_CompAddition = 0 ;
		private long m_lClipByteCount ;

		private VuMeter ob_VuMeter;

		private static readonly AudioPlayer mInstance = new AudioPlayer();

		public static AudioPlayer Instance
		{
			get { return mInstance; }
		}

		// JQ changed constructor to be private (singleton)
		private AudioPlayer()
		{
			m_PlayFile = true ;
			m_FastPlay = false ;
			m_State = AudioPlayerState.Stopped;
			ob_VuMeter = null;  // JQ
		}

		// bool variable to enable or disable event
		bool m_EventsEnabled = true ;

		public VuMeter VuMeterObject
		{
			get
			{
				return ob_VuMeter;
			}
			set
			{
				ob_VuMeter = value;  // the vu meter should then be told to listen to this audio player
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
		
		// Output  device object
		/*public Microsoft.DirectX.DirectSound.Device OutputDevice
		{
			get
			{
				return SndDevice ;
			}
			set
			{
				SndDevice = value ;
			}
		}*/

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

		/*public ArrayList GetOutputDevices()
		{
			CollectOutputDevices() ;
			ArrayList OutputDevices = new ArrayList();
			for (int i = 0; i < devList.Count; i++) 
			{
				OutputDevices.Add(devList[i].Description);
			}
			return OutputDevices ;
		}*/

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

            /*public void SetDevice (Control FormHandle, int Index)
            {
                Microsoft.DirectX.DirectSound.Device dSound = new  Microsoft.DirectX.DirectSound.Device(devList[Index].DriverGuid);
                dSound.SetCooperativeLevel(FormHandle, CooperativeLevel.Priority);
                SndDevice  = dSound ;
            }*/

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

		/*DevicesCollection devList ;
		DevicesCollection  CollectOutputDevices()
		{
			devList = new DevicesCollection();
			return devList  ;
		}*/

		public void Play(Assets.AudioMediaAsset asset )
		{
			m_StartPosition   = 0 ;
			m_State  = AudioPlayerState.NotReady ;
			m_Asset = asset as Assets.AudioMediaAsset;
			InitPlay(0, 0);
		}

		void InitPlay(long lStartPosition, long lEndPosition)
		{
            if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
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

                if (m_SizeBuffer > m_lLength - lStartPosition)
                {
                    m_SizeBuffer = Convert.ToInt32(m_lLength - lStartPosition);
                    m_RefreshLength = m_SizeBuffer / 2;
                    m_RefreshLength = ( m_RefreshLength / m_FrameSize ) * m_FrameSize ;
                }

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

                // check for fast play
                int reduction = 0;
                if (m_FastPlay == false)
                {
                    SoundBuffer.Write(0, m_MemoryStream, m_SizeBuffer, 0);
                }
                else
                {
                    // for fast play buffer is filled in parts with new part overlapping previous part
                    for (int i = 0; i < m_SizeBuffer; i = i + (m_Step * m_FrameSize))
                    {
                        SoundBuffer.Write(i, m_MemoryStream, m_Step * m_FrameSize, 0);
                        i = i - (2 * m_FrameSize);
                        // compute the difference in bytes skipped
                        reduction = reduction + (2 * m_FrameSize);
                    }

                }
                // Adds the length (count) of file played into a variable
                // Folowing one line was modified on 2 Aug 2006 i.e lStartPosition is added
                m_lPlayed = m_SizeBuffer + reduction + (2 * m_CompAddition) + lStartPosition;


                m_PlayFile = true;

                // trigger  events (modified JQ)
                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
                m_State = AudioPlayerState.Playing;
                StateChanged(this, e);
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
			// variable to count byte difference in compressed and non compressed data of audio file
			int reduction = 0 ;
			while (m_lPlayed < m_lLength)
			{//1
				if (SoundBuffer.Status.BufferLost  )
					SoundBuffer.Restore () ;

				reduction = 0 ;
				Thread.Sleep (50) ;
				ReadPosition = SoundBuffer.PlayPosition  ;

				if (ReadPosition < ((m_SizeBuffer)- m_UpdateVMArrayLength  ) )
				{
					Array.Copy ( SoundBuffer.Read (ReadPosition , typeof (byte) , LockFlag.None , m_UpdateVMArrayLength  ) , arUpdateVM , m_UpdateVMArrayLength  ) ;				
					//if ( m_EventsEnabled == true)
					//ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
					UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ
				}
				// check if play cursor is in second half , then refresh first half else second
				if ((m_BufferCheck% 2) == 1 &&  SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
					// Checks if file is played or byteBuffer is played
					if (m_PlayFile == true)
					{//3
						LoadStream (false) ;
						// check for normal or fast play
						if (m_FastPlay == false)
						{//4
							SoundBuffer.Write (0 , m_MemoryStream, m_RefreshLength, 0) ;
						}//-4
						else
						{//4
							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{
								SoundBuffer.Write ( i, m_MemoryStream , m_Step*m_FrameSize, 0) ;
								i = i-(2*m_FrameSize );
								reduction = reduction + (2* m_FrameSize);
							}//-4

						}//-3
					}//-2

					m_lPlayed = m_lPlayed + m_RefreshLength+ reduction+ m_CompAddition;

					m_BufferCheck++ ;
				}//-1
				else if ((m_BufferCheck % 2 == 0) &&  SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
					if (m_PlayFile == true)
					{//2
						LoadStream (false) ;
						if (m_FastPlay == false)
						{//3
							SoundBuffer.Write (m_RefreshLength, m_MemoryStream, m_RefreshLength, 0)  ;
						}//-3
						else
						{//3
							for (int i = 0 ; i< m_RefreshLength; i=i+(m_Step*m_FrameSize))
							{//4
								SoundBuffer.Write ( i+ m_RefreshLength, m_MemoryStream, m_Step*m_FrameSize, 0) ;
								i = i-(2*m_FrameSize) ;
								reduction = reduction + (2* m_FrameSize);
								
							}						//-4
							
							// end of FastPlay check
						}//-3
					}//-2
					

					m_lPlayed = m_lPlayed + m_RefreshLength+ reduction+m_CompAddition ;

					m_BufferCheck++ ;

					// end of even/ odd part of buffer;
				}//-1
				
					
				// end of while
			}
			// calculate time to stop according to remaining data
			int time ;
			long lRemaining = (m_lPlayed - m_lLength);
			double dTemp ;
			
			dTemp= ((m_RefreshLength + m_RefreshLength - lRemaining			)*1000)/m_RefreshLength ;
			time = Convert.ToInt32(dTemp * 0.48 );

			//if (m_FastPlay == true)
			//time = (time-250) * (1- (2/m_Step));

			Thread.Sleep (time) ;
			
			// Stopping process begins
			SoundBuffer.Stop () ;
			if (ob_VuMeter != null) ob_VuMeter.Reset () ;
			if (m_PlayFile == true)
			{
				m_br.Close();
				//ob_EndOfAudioAsset.NotifyEndOfAudioAsset ( this , ob_EndOfAudioAsset) ;
			}
			

			//Stop () ;
			// changes the state and trigger events
			//ob_StateChanged = new StateChanged (m_State) ;

			EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());  // JQ


			Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
			m_State= AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);

			// RefreshBuffer ends
		}
		
		public void Play(Assets.AudioMediaAsset  asset, double timeFrom)
		{
			m_Asset = asset as Assets.AudioMediaAsset;
			long lPosition = CalculationFunctions.ConvertTimeToByte (timeFrom, m_Asset .SampleRate, m_Asset .FrameSize) ;
			lPosition = CalculationFunctions.AdaptToFrame(lPosition, m_Asset .FrameSize) ;
			if(lPosition>=0   && lPosition < m_Asset.AudioLengthInBytes)
			{
							m_StartPosition   =  lPosition ;
				InitPlay ( lPosition, 0 );
			}
			else
			throw new Exception ("Start Position is out of bounds of Audio Asset") ;
				
			
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

		public void Pause()
		{
			if (m_State.Equals(AudioPlayerState .Playing))
			{
				SoundBuffer.Stop () ;
				// Change the state and trigger event
				Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State) ;
				m_State= AudioPlayerState .Paused;
				TriggerStateChangedEvent(e);
			}
		}

		public void Resume()
		{
			if (m_State.Equals(AudioPlayerState.Paused))
			{
				Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
				m_State= AudioPlayerState .Playing ;
				TriggerStateChangedEvent(e) ;
				SoundBuffer.Play(0, BufferPlayFlags.Looping);
			}			
		}

		public void Stop()
		{
			if (!m_State.Equals(AudioPlayerState .Stopped))			
			{
				SoundBuffer.Stop();
				RefreshThread.Abort();
				if (ob_VuMeter != null) ob_VuMeter.Reset();			
				if (m_PlayFile) m_br.Close();
			}
			Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
			m_State = AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);
		}

		internal long GetCurrentBytePosition()
		{
			int PlayPosition  = SoundBuffer.PlayPosition;
			
			long lCurrentPosition ;
			if (PlayPosition < m_RefreshLength)
			{ 
				// takes the lPlayed position and subtract the part of buffer played from it
				lCurrentPosition = m_lPlayed - ( 2 * m_RefreshLength) + PlayPosition ;
			}
			else
			{
				lCurrentPosition = m_lPlayed - (3 * m_RefreshLength) + PlayPosition ;
			}
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
				if ( m_PlayFile== true)
				{

					Stop();
					Thread.Sleep (30) ;
					m_StartPosition = localPosition ;
					InitPlay(localPosition , 0);
				}
				
			}		
			
			else if(m_State.Equals (AudioPlayerState .Paused ) )
			{
				if (m_PlayFile == true)
				{
					Stop();
					m_StartPosition = localPosition ;
					Thread.Sleep (20) ;
					InitPlay(localPosition , 0);
					Thread.Sleep(30) ;
					SoundBuffer.Stop () ;
					// Stop () also change the m_Stateto stopped so change it to paused
					m_State=AudioPlayerState .Paused;
				}
				
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
			m_MemoryStream.Position = 0 ;
		}

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

		// End Class
	}
	// End NameSpace
}
