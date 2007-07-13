using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

using urakawa.media.data.audio  ;
 

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

    /// <summary>
    /// <see cref=""/>
    ///   Playback modes for AudioPlayer
    /// Normal: for normal playback
    /// FastForward: For playing small chunks while jumping forward
    /// Rewind: for playing small chunks while jumping backward
        /// </summary>
    public enum PlaybackModes { Normal, FastForward, Rewind} ;

	public class AudioPlayer
	{
		// Events of the audio player (JQ)
		public event Events.Audio.Player.EndOfAudioAssetHandler EndOfAudioAsset;
		public event Events.Audio.Player.StateChangedHandler StateChanged;
		public event Events.Audio.Player.UpdateVuMeterHandler UpdateVuMeter;

		// declare member variables
		private AudioMediaData  m_Asset ;
        private Stream m_AssetStream;
		private SecondaryBuffer m_SoundBuffer;
  private OutputDevice mDevice;
		
        // integer to indicate which part of buffer is to be refreshed front or rear
		private int m_BufferCheck ;

        // Size of buffer created for playing
		private int m_SizeBuffer ;

        // length of buffer to be refreshed during playing which is half of buffer size
		private int m_RefreshLength ;

        // Total length of audio asset being played
		private long m_lLength;

        // Length of audio asset in bytes which had been played
		private long m_lPlayed ;

        // thread for refreshing buffer while playing 
		private Thread RefreshThread;

        // variable to hold stop position in buffer after audio asset is about to end and all refreshing is finished , it will remain -1 till refreshing is going on
        int m_BufferStopPosition ;

        private long m_lPausePosition; // holds pause position in bytes to allow play resume playback from there
        private long m_lResumeToPosition ; // In case play ( from, to ) function is used, holds the end position i.e. "to"  for resuming playback

		internal int m_FrameSize ; 
		internal int m_Channels ;
		private int m_SamplingRate ;
        private int m_VolumeLevel ;
		private AudioPlayerState  m_State;
        private PlaybackModes m_PlaybackMode ;
        private int m_FwdRwdRate  ; // holds skip time multiplier for forward / rewind mode
        private float m_fFastPlayFactor ; /// fholds fast play multiplier
        private bool m_IsFwdRwd ; // Is true if Playback mode is not normal and Forward / rewind playback is going on

        // bool variable to enable or disable event
        public bool m_EventsEnabled ; // sometimes  it is required to suppress events, this is done by this variable
		private VuMeter ob_VuMeter;

        // monitoring timer to trigger events independent of refresh thread
        private System.Windows.Forms.Timer MoniteringTimer = new System.Windows.Forms.Timer();

        // Flag to trigger end of asset events, flag is set for a moment and again reset
        private bool m_IsEndOfAsset  ; // variable required to signal monitoring timer to trigger end of asset event


		private static readonly AudioPlayer mInstance = new AudioPlayer();

		public static AudioPlayer Instance
		{
			get { return mInstance; }
		}

		// JQ changed constructor to be private (singleton)
		private AudioPlayer()
        {

			m_State = AudioPlayerState.NotReady ;
            ob_VuMeter = null;
            MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
            MoniteringTimer.Interval = 200;
           mPreviewTimer.Tick += new System.EventHandler(this.PreviewTimer_Tick );
           mPreviewTimer.Interval = 100;

           m_PlaybackMode = PlaybackModes.Normal;
            m_FwdRwdRate = 1;
            m_fFastPlayFactor = 1;
            m_IsFwdRwd = false;
            m_EventsEnabled = true;
            m_lResumeToPosition = 0 ;
  m_BufferStopPosition= -1  ;
  m_IsEndOfAsset = false;

            // events associated with local function so as to avoid null exceptions            
            StateChanged += new Obi.Events.Audio.Player.StateChangedHandler(CatchEvents);
            EndOfAudioAsset += new Obi.Events.Audio.Player.EndOfAudioAssetHandler(CatchEvents);
		}

		
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
                if (m_IsFwdRwd == true )
                    return AudioPlayerState.Playing;
                else
				return m_State ;
			}
		}

        /// <summary>
        /// <see cref=""/>
        /// Gets and Sets Currently active playback mode in AudioPlayer
        ///  Playback mode returns to normal on pause or stop
        /// </summary>
        public PlaybackModes PlaybackMode
        {
            get
            {
                return  m_PlaybackMode;
            }
            set
            {
                SetPlaybackMode(value);
            }
        }

        /// <summary>
        /// <see cref=""/>
        ///  Forward / Rewind rate
        ///  whenever set to 0, Playback mode sets to normal
        /// </summary>
        public int PlaybackFwdRwdRate
        {
            get
            {
                return m_FwdRwdRate ;
            }
            set 
            {
                m_FwdRwdRate = value ;
                if ( m_FwdRwdRate == 0    &&    m_PlaybackMode != PlaybackModes.Normal )
                    SetPlaybackMode ( PlaybackModes.Normal ) ;
            }
        }
        


        public OutputDevice OutputDevice
        {
            get { return mDevice; }
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

        /// <summary>
        /// <see cref=""/>
        ///  ets and Sets the play speed with respect to normal play sppeed
        /// </summary>
        public float FastPlayFactor
        {
            get
            {
                return m_fFastPlayFactor;
            }
            set
            {
                                SetPlayFrequency(value);
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

            if (m_SoundBuffer != null)
                m_SoundBuffer.Volume = m_VolumeLevel;

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
            m_State = AudioPlayerState.Stopped;
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
                m_State = AudioPlayerState.Stopped;
            }
            else if (devices.Count > 0)
            {
                SetDevice(FormHandle, devices[0]);
                m_State = AudioPlayerState.Stopped;
            }
            else
            {
                m_State = AudioPlayerState.NotReady;
                throw new Exception("No output device available.");
            }
        }

        void SetPlayFrequency(float l_frequency)
        {
            if (m_SoundBuffer != null
                && m_PlaybackMode == PlaybackModes.Normal)
            {
                try
                {
                    m_SoundBuffer.Frequency = (int)(m_SoundBuffer.Format.SamplesPerSecond * l_frequency);
                    m_fFastPlayFactor = l_frequency;
                }
                catch (System.Exception Ex)
                {
                    MessageBox.Show("Unable to change fastplay rate " + Ex.ToString());
                }
            }
            else
                m_fFastPlayFactor = l_frequency;
        }


        private void SetPlaybackMode(PlaybackModes l_PlaybackMode)
        {
            if (m_State == AudioPlayerState.Playing      ||     m_IsFwdRwd  )
            {
                                long RestartPos = 0;
                                                                    RestartPos = GetCurrentBytePosition();
                    
                StopFunction();
                m_State = AudioPlayerState.Paused ;
                m_PlaybackMode = l_PlaybackMode;
                InitPlay ( RestartPos , 0 ) ;
            }
            else if (m_State == AudioPlayerState.Paused   ||   m_State == AudioPlayerState.Stopped )
            {
                m_PlaybackMode = l_PlaybackMode;
            }
        }


public 		 void InitPlay(long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (m_State != AudioPlayerState.Playing )
            {
            
                    InitialiseWithAsset ( Data) ;

                    if (m_PlaybackMode  == PlaybackModes.Normal)
                        PlayAssetStream( lStartPosition , lEndPosition);
                    else if (m_PlaybackMode  == PlaybackModes.FastForward)
                    {
                                                FastForward( lStartPosition );
                    }
                    else if (m_PlaybackMode == PlaybackModes.Rewind)
                    {
                        if (lStartPosition == 0)
                            lStartPosition = m_Asset.getPCMLength();
                        Rewind(lStartPosition);
                    }
            }// end of state check
			// end of function
		}
        public AudioMediaData Data;
        private void InitialiseWithAsset(AudioMediaData Asset)
        {
            if (m_State != AudioPlayerState.Playing)
            {
                m_Asset = Asset;
                WaveFormat newFormat = new WaveFormat();
                 BufferDescription  BufferDesc = new BufferDescription();
                
                // retrieve format from asset
                m_FrameSize = m_Asset.getPCMFormat().getBlockAlign();
                m_Channels = m_Asset.getPCMFormat().getNumberOfChannels();
                newFormat.AverageBytesPerSecond = (int)m_Asset.getPCMFormat().getSampleRate() * m_Asset.getPCMFormat().getBlockAlign();
                                newFormat.BitsPerSample = Convert.ToInt16(m_Asset.getPCMFormat().getBitDepth());
                newFormat.BlockAlign = Convert.ToInt16(m_Asset.getPCMFormat().getBlockAlign());
                newFormat.Channels = Convert.ToInt16(m_Asset.getPCMFormat().getNumberOfChannels());

                newFormat.FormatTag = WaveFormatTag.Pcm;

                newFormat.SamplesPerSecond = (int)m_Asset.getPCMFormat().getSampleRate();

                // loads  format to buffer description
                BufferDesc.Format = newFormat;

                // enable buffer description properties
                BufferDesc.ControlVolume = true;
                BufferDesc.ControlFrequency = true;

                // calculate size of buffer so as to contain 1 second of audio
                m_SizeBuffer = (int)m_Asset.getPCMFormat().getSampleRate() * m_Asset.getPCMFormat().getBlockAlign();
                m_RefreshLength = (int)(m_Asset.getPCMFormat().getSampleRate() / 2) * m_Asset.getPCMFormat().getBlockAlign();

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
                // m_SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
                m_SoundBuffer = new SecondaryBuffer(BufferDesc, mDevice.Device);

                SetPlayFrequency(m_fFastPlayFactor);

            }// end of state check
                            } // end function

        private void PlayAssetStream(long lStartPosition, long lEndPosition)
        {
            if (m_State != AudioPlayerState.Playing)
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, m_Asset.getPCMFormat().getBlockAlign());
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, m_Asset.getPCMFormat().getBlockAlign());
                m_SamplingRate = (int)m_Asset.getPCMFormat().getSampleRate();

                // lEndPosition = 0 means that file is played to end
                if (lEndPosition != 0)
                {
                    m_lLength = (lEndPosition); // -lStartPosition;
                }
                else
                {
                    // folowing one line is modified on 2 Aug 2006
                    //m_lLength = (m_Asset .SizeInBytes  - lStartPosition ) ;
                    m_lLength = (m_Asset.getPCMLength());
                }


                // initialize M_lPlayed for this asset
                m_lPlayed = lStartPosition;

                m_IsEndOfAsset = false;
                
                m_AssetStream = m_Asset.getAudioData();
                m_AssetStream.Position = lStartPosition;
                
                m_SoundBuffer.Write(0, m_AssetStream, m_SizeBuffer, 0);

                // Adds the length (count) of file played into a variable
                m_lPlayed += m_SizeBuffer;

                // trigger  events (modified JQ)
                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
                m_State = AudioPlayerState.Playing;
                TriggerStateChangedEvent(e);

                MoniteringTimer.Enabled = true;
                // starts playing
                m_SoundBuffer.Play(0, BufferPlayFlags.Looping);
                m_BufferCheck = 1;

                //initialise and start thread for refreshing buffer
                RefreshThread = new Thread(new ThreadStart(RefreshBuffer));
                RefreshThread.Start();


            }
        } // function ends

		void RefreshBuffer ()
		{
		
			int ReadPosition;
			
			// variable to prevent least count errors in clip end time
            long SafeMargin = CalculationFunctions.ConvertTimeToByte(1, m_SamplingRate, m_FrameSize);


			while (  m_lPlayed < ( m_lLength - SafeMargin ) )
			{//1
				if (m_SoundBuffer.Status.BufferLost  )
					m_SoundBuffer.Restore () ;

				
				Thread.Sleep (50) ;

                if (ob_VuMeter != null)
                {
                    ReadPosition = m_SoundBuffer.PlayPosition;

                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(m_SoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);
                        //if ( m_EventsEnabled == true)
                        //UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ // temp for debugging tk
                    }
                }
				// check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
				if ((m_BufferCheck% 2) == 1 &&  m_SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
													m_SoundBuffer.Write (0 , m_AssetStream , m_RefreshLength, 0) ;
                    					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
				}//-1
                    // refresh Rear half of buffer for even count
				else if ((m_BufferCheck % 2 == 0) &&  m_SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
													m_SoundBuffer.Write (m_RefreshLength,  m_AssetStream, m_RefreshLength, 0)  ;
                            					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
					// end of even/ odd part of buffer;
                    				}//-1

				// end of while
			}
            
            m_IsEndOfAsset = false;
            int LengthDifference = (int)(m_lPlayed - m_lLength  );
             m_BufferStopPosition= -1 ;
                         // if there is no refresh after first load thenrefresh maps directly  
                        if  ( m_BufferCheck == 1  )
                            {
                m_BufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference );
            }

            // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
            else if ((m_BufferCheck % 2) == 0)
            {
                m_BufferStopPosition = Convert.ToInt32(m_RefreshLength - LengthDifference );
                            }
            else if ((m_BufferCheck >  1) && (m_BufferCheck % 2) == 1)
            {
                m_BufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
            }
            
            int CurrentPlayPosition;
            CurrentPlayPosition = m_SoundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32 (CalculationFunctions.ConvertTimeToByte( 70 , m_SamplingRate, m_FrameSize));
            StopMargin = (int)  (StopMargin * m_fFastPlayFactor);

            if ( m_BufferStopPosition < StopMargin)
                m_BufferStopPosition = StopMargin; 

             while (CurrentPlayPosition < (m_BufferStopPosition - StopMargin) || CurrentPlayPosition > ( m_BufferStopPosition ))
                {
                    Thread.Sleep(50);
                    CurrentPlayPosition = m_SoundBuffer.PlayPosition;
                }

			
			// Stopping process begins
                                m_BufferStopPosition = -1 ;
                m_lPausePosition = 0;
			m_SoundBuffer.Stop () ;
			if (ob_VuMeter != null) ob_VuMeter.Reset () ;
            m_AssetStream.Close();

			// changes the state and trigger events
            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Stopped;

			TriggerStateChangedEvent(e);

            if (m_EventsEnabled)
                m_IsEventEnabledDelayedTillTimer= true;
            else
                m_IsEventEnabledDelayedTillTimer= false;

            m_IsEndOfAsset = true;

			// RefreshBuffer ends
		}
        private bool m_IsEventEnabledDelayedTillTimer= true ;

        ///<summary>
        /// Function for simulating playing for assets with no audio
        /// </summary>
        ///
        private void SimulateEmptyAssetPlaying()
        {
            
            m_Channels = m_Asset.getPCMFormat ().getNumberOfChannels ()  ;
            m_FrameSize = m_Asset.getPCMFormat ().getNumberOfChannels ()  * (m_Asset.getPCMFormat ().getBitDepth ()  / 8);
            m_SamplingRate = (int)  m_Asset.getPCMFormat ().getSampleRate () ;

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
//            System.Media.SystemSounds.Asterisk.Play();
        }

        /// <summary>
        ///  Pauses playing asset. 
        /// Resumes from paused position with resume command or starts from begining/specified start position with play command.
        /// /<see cref=""/>
        /// </summary>
		public void Pause()
		{
            // API state is used
			if ( State.Equals(AudioPlayerState .Playing)  )
			{
                
                    m_lPausePosition = GetCurrentBytePosition();
                    if (!m_IsFwdRwd)
                        m_lResumeToPosition = m_lLength;
                    else
                        m_lResumeToPosition = 0;

                StopFunction();

				// Change the state and trigger event
				Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State) ;
                m_State = AudioPlayerState.Paused;
				TriggerStateChangedEvent(e);
                
			}
		}

        /// <summary>
        ///  Resumes from paused position if player is in paused state
        /// <see cref=""/>
        /// </summary>
		public void Resume()
		{
            // API state will be used for public functions
            			if ( State.Equals(AudioPlayerState.Paused))
			{
                
                long lPosition = CalculationFunctions.AdaptToFrame( m_lPausePosition , m_Asset.getPCMFormat ().getBlockAlign ()  );
                long lEndPosition = CalculationFunctions.AdaptToFrame( m_lResumeToPosition , m_Asset.getPCMFormat().getBlockAlign());

                if (lPosition >= 0 && lPosition < m_Asset.getPCMLength () )
                {
                    m_StartPosition = lPosition;
                                        InitPlay(lPosition, lEndPosition );
                }
                else
                    throw new Exception("Start Position is out of bounds of Audio Asset");
			}			
		}

		public void Stop()
		{
            // API state is used
			if ( State != AudioPlayerState.Stopped )			
			{
				                StopFunction();
			}
            
        m_lPausePosition = 0;
			Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(m_State);
            m_State = AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);
		}

        private void StopFunction ()
        {
            if (m_PlaybackMode  != PlaybackModes.Normal)
                StopForwardRewind();


            
            m_SoundBuffer.Stop();
                        if (RefreshThread != null &&    RefreshThread.IsAlive )
                            RefreshThread.Abort();

                        m_BufferStopPosition = -1;

            				if (ob_VuMeter != null) ob_VuMeter.Reset();

                                        m_AssetStream.Close();
                    }

private 		 long GetCurrentBytePosition()
{
    int PlayPosition = 0;
    long lCurrentPosition = 0;

    if (m_Asset.getPCMLength() > 0)
    {
        if (m_State == AudioPlayerState.Playing)
        {
            PlayPosition = m_SoundBuffer.PlayPosition;
            // if refreshing of buffer has finished and player is near end of asset
            if (m_BufferStopPosition != -1)
            {
                int subtractor = (m_BufferStopPosition - PlayPosition);
                lCurrentPosition = m_Asset.getPCMLength() - subtractor;
            }
            //if (PlayPosition < m_RefreshLength) // Avn: changed on19 Dec 2006 for improving get position for pause
            else if (m_BufferCheck % 2 == 1)
            {
                // takes the lPlayed position and subtract the part of buffer played from it
                int subtractor = (2 * m_RefreshLength) - PlayPosition;

                lCurrentPosition = m_lPlayed - subtractor;
            }
            else
            {
                int subtractor = (3 * m_RefreshLength) - PlayPosition;
                lCurrentPosition = m_lPlayed - subtractor;
            }

            if (lCurrentPosition >= m_Asset.getPCMLength())
                lCurrentPosition = m_Asset.getPCMLength() - Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, m_SamplingRate, m_FrameSize));


        }
    }
    else if (m_State == AudioPlayerState.Paused)
        lCurrentPosition = m_lPausePosition;

        lCurrentPosition = CalculationFunctions.AdaptToFrame(lCurrentPosition, m_FrameSize);


        if (m_PlaybackMode != PlaybackModes.Normal)
            lCurrentPosition = m_lChunkStartPosition;
        
    return lCurrentPosition;
}

		private  double GetCurrentTimePosition()
		{	
			return CalculationFunctions.ConvertByteToTime (GetCurrentBytePosition() , m_SamplingRate , m_FrameSize);
		}
		
		long m_StartPosition  ;

		void SetCurrentBytePosition (long localPosition) 
		{
			if (localPosition < 0)
				localPosition = 0;

			if (localPosition > m_Asset.getPCMLength ()  )
				localPosition = m_Asset.getPCMLength ()  - 100;


			m_EventsEnabled = false ;

			if (State == AudioPlayerState.Playing  )
			{
                					Stop();
					Thread.Sleep (30) ;
					m_StartPosition = localPosition ;
                    					InitPlay(localPosition , 0);
				
			}		
			
			else if(m_State.Equals (AudioPlayerState .Paused ) )
			{
					m_StartPosition = localPosition ;
                    m_lPausePosition = localPosition;
																																	}
			m_EventsEnabled = true ;

			// end of set byte position
		}


		void SetCurrentTimePosition (double localPosition) 
		{
			long lTemp = CalculationFunctions.ConvertTimeToByte (localPosition, m_SamplingRate, m_FrameSize);
			SetCurrentBytePosition(lTemp) ;
		}


        //  FastForward , Rewind playback modes
        // timer for playing chunks at interval
        System.Windows.Forms.Timer mPreviewTimer = new System.Windows.Forms.Timer();
        
        // position for starting chunk play
private          long m_lChunkStartPosition = 0;

        public void Rewind( long lStartPosition  )
        {
                        // let's play backward!
            if ( m_PlaybackMode  !=  PlaybackModes.Normal )
            {
                m_lChunkStartPosition = lStartPosition ;
                                m_EventsEnabled = false;
                                m_IsFwdRwd = true;
                                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();
                
                            }
        }
        

        public void FastForward(long lStartPosition   )
        {

            // let's play forward!
            if (m_PlaybackMode != PlaybackModes.Normal)
            {
                m_lChunkStartPosition = lStartPosition;
                m_EventsEnabled = false;
                m_IsFwdRwd = true;
                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();
            }
        }

        

        ///Preview timer tick function
        private void PreviewTimer_Tick(object sender, EventArgs e)
        { //1
            
            double StepInMs = 3000 * m_FwdRwdRate  ;
            long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)  m_Asset.getPCMFormat().getSampleRate ()  , m_Asset.getPCMFormat().getBlockAlign ());
            int PlayChunkLength = 1200;
            long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte( PlayChunkLength , (int)m_Asset.getPCMFormat().getSampleRate(), m_Asset.getPCMFormat().getBlockAlign());
            mPreviewTimer.Interval = PlayChunkLength + 50;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if ( m_PlaybackMode  == PlaybackModes.FastForward  )
            { //2
                if (( m_Asset.getPCMLength () - ( lStepInBytes + m_lChunkStartPosition ) ) >  lPlayChunkLength )
                { //3
                    if (m_lChunkStartPosition > 0)
                    {
                        m_lChunkStartPosition += lStepInBytes;
                    }
                    else
                        m_lChunkStartPosition = m_FrameSize;

                    PlayStartPos = m_lChunkStartPosition;
PlayEndPos  = m_lChunkStartPosition + lPlayChunkLength  ;
PlayAssetStream  (  PlayStartPos, PlayEndPos);

if (m_lChunkStartPosition > m_Asset.getPCMLength())
    m_lChunkStartPosition = m_Asset.getPCMLength();
                                    } //-3
                else
                { //3
                    Stop();
                    if (m_EventsEnabled)
                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                } //-3
            } //-2
            else if ( m_PlaybackMode  ==  PlaybackModes.Rewind )
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                if (m_lChunkStartPosition >  0 )
                { //3
                    if (m_lChunkStartPosition < m_Asset.getPCMLength())
                        m_lChunkStartPosition -= lStepInBytes;
                    else
                        m_lChunkStartPosition = m_Asset.getPCMLength() - lPlayChunkLength  ;

                    PlayStartPos = m_lChunkStartPosition ;
                    PlayEndPos = m_lChunkStartPosition +  lPlayChunkLength;
                    PlayAssetStream (PlayStartPos, PlayEndPos);
                    
                    if (m_lChunkStartPosition < 0)
                        m_lChunkStartPosition = 0;
                } //-3
                else
                {
                    Stop();
                    if (m_EventsEnabled)
                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                    }
                } //-2
                            } //-1
       

        /// <summary>
        /// function to stop Fast Forward / Rewind and also stop preview timer
        /// </summary>
        private void StopForwardRewind()
        {
            if (m_PlaybackMode  != PlaybackModes.Normal || mPreviewTimer.Enabled == true)
            {
                mPreviewTimer.Enabled = false;
m_FwdRwdRate                 = 1 ;
m_lChunkStartPosition = 0;
m_IsFwdRwd = false;
m_PlaybackMode  = PlaybackModes.Normal;
                                m_EventsEnabled = true;
            }
                
        }


        private void MoniteringTimer_Tick(object sender, EventArgs e)
        {
            if ( m_IsEndOfAsset == true)
            {
                m_IsEndOfAsset = false;
                MoniteringTimer.Enabled = false;

                if (m_IsEventEnabledDelayedTillTimer)
                {
                    EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                    }
            if (m_EventsEnabled == true)
                m_IsEventEnabledDelayedTillTimer= true;
            else
                m_IsEventEnabledDelayedTillTimer= false;
            }

        }


        // function to catch events so as to avoid null  reference exceptions
        private void CatchEvents(object sender, EventArgs e)
        {
            //System.Media.SystemSounds.Asterisk.Play();
        }


    } // End Class
	}
