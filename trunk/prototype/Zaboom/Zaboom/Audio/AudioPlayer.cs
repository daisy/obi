using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

using urakawa.media.data.audio  ;
 

namespace Zaboom.Audio
{
    /// <summary>
    /// The four states of the audio player.
    /// NotReady: the player is not ready, for whatever reason.
    /// Playing: sound is currently playing.
    /// Paused: playback was interrupted and can be resumed.
    /// Stopped: player is idle.
    /// </summary>
    public enum PlayerState { NotReady, Stopped, Playing, Paused };

    /// <summary>
    /// Playback modes for AudioPlayer
    /// Normal: for normal playback
    /// FastForward: For playing small chunks while jumping forward
    /// Rewind: for playing small chunks while jumping backward
    /// </summary>
    public enum PlaybackModes { Normal, FastForward, Rewind} ;

    public class StateChangedEventArgs : EventArgs
    {
        public PlayerState PreviousState;
        public StateChangedEventArgs(PlayerState prev) { PreviousState = prev; }
    }

    public delegate void StateChangedHandler(Player player, StateChangedEventArgs e);
    public delegate void EndOfAudioAssetHandler(Player player, EventArgs e);
    public delegate void UpdateVuMeterHandler(Player player, EventArgs e);

    /// <summary>
    /// The audio player class can play one audio media data at a time.
    /// </summary>
	public class Player
	{
		public event EndOfAudioAssetHandler EndOfAudioAsset;  // the end of the asset was reached
		public event StateChangedHandler StateChanged;        // the state of the player has changed
		public event UpdateVuMeterHandler UpdateVuMeter;      // placeholder

        #region private members

        private AudioMediaData mCurrentAudioMediaData;  // audio media data currently being played
        private Stream mAudioStream;                    // stream for the current audio media data

        /* TODO: enable VU meter

        internal int m_UpdateVMArrayLength;
        private VuMeter ob_VuMeter;
        internal byte[] arUpdateVM;                     // array for update current amplitude to VuMeter
        
        */

        #endregion

        /// <summary>
        /// Create a new audio player
        /// </summary>
        public Player()
        {
            m_State = PlayerState.Stopped;
            // ob_VuMeter = null;
            MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
            MoniteringTimer.Interval = 200;
            mPreviewTimer.Tick += new System.EventHandler(this.PreviewTimer_Tick);
            mPreviewTimer.Interval = 100;
            m_PlaybackMode = PlaybackModes.Normal;
            m_FwdRwdRate = 1;
            m_fFastPlayFactor = 1;
        }

        /// <summary>
        /// Change the state of the player and send an event if anyone is listening.
        /// </summary>
        private void ChangeState(PlayerState newState)
        {
            if (newState != m_State)
            {
                StateChangedEventArgs e = new StateChangedEventArgs(m_State);
                m_State = newState;
                if (StateChanged != null) StateChanged(this, e);
            }
        }









        private SecondaryBuffer SoundBuffer;
        private OutputDevice mDevice;
        private BufferDescription BufferDesc = null;

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

        // variable to hold stop position in buffer after audio asset is about to end and all refreshing is finished
        int m_BufferStopPosition= -1 ;

        // flag to indicate last refresh has been done and do not refresh again
        private bool m_IsLastRefresh = false;
		
		internal int m_FrameSize ;
		internal int m_Channels ;
		private int m_SamplingRate ;
        private int m_VolumeLevel ;
		private PlayerState  m_State;
        private PlaybackModes m_PlaybackMode ;
        private int m_FwdRwdRate = 1 ;
        private float m_fFastPlayFactor = 1;
        private bool m_IsFwdRwd = false;


        // monitoring timer to trigger events independent of refresh thread
        private System.Windows.Forms.Timer MoniteringTimer = new System.Windows.Forms.Timer();

        // Flag to trigger end of asset events, flag is set for a moment and again reset
        private bool m_IsEndOfAsset = false ;


		private static readonly Player mInstance = new Player();

		public static Player Instance
		{
			get { return mInstance; }
		}

		// bool variable to enable or disable event
		public bool m_EventsEnabled = true ;

        /*
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
        */

		
		// gets the current AudioPlayer state
		public PlayerState State
		{
			get
			{
                if (m_IsFwdRwd == true )
                    return PlayerState.Playing;
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

            if (SoundBuffer != null)
                SoundBuffer.Volume = m_VolumeLevel;

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

        void SetPlayFrequency(float l_frequency)
        {
            if (SoundBuffer != null
                &&     m_PlaybackMode == PlaybackModes.Normal )
            {
                m_fFastPlayFactor = l_frequency;
                SoundBuffer.Frequency = (int)  ( SoundBuffer.Format.SamplesPerSecond * m_fFastPlayFactor ) ;
            }
        }


        private void SetPlaybackMode(PlaybackModes l_PlaybackMode)
        {
            if (m_State == PlayerState.Playing      ||     m_IsFwdRwd  )
            {
                                long RestartPos = 0;
                                                                    RestartPos = GetCurrentBytePosition();
                    
                StopFunction();
                m_State = PlayerState.NotReady;
                m_PlaybackMode = l_PlaybackMode;
                InitPlay ( RestartPos , 0 ) ;
            }
            else if (m_State == PlayerState.Paused   ||   m_State == PlayerState.Stopped )
            {
                m_PlaybackMode = l_PlaybackMode;
            }
        }

public 		 void InitPlay(long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (m_State != PlayerState.Playing )
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
                            lStartPosition = mCurrentAudioMediaData.getPCMLength();
                        Rewind(lStartPosition);
                    }
            }// end of state check
			// end of function
		}
        public AudioMediaData Data;
        private void InitialiseWithAsset(AudioMediaData Asset)
        {
            if (m_State != PlayerState.Playing)
            {
                mCurrentAudioMediaData = Asset;
                WaveFormat newFormat = new WaveFormat();
                BufferDesc = new BufferDescription();
                BufferDesc.ControlVolume = true;

                // retrieve format from asset
                m_FrameSize = mCurrentAudioMediaData.getPCMFormat().getBlockAlign();
                m_Channels = mCurrentAudioMediaData.getPCMFormat().getNumberOfChannels();
                newFormat.AverageBytesPerSecond = (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate() * mCurrentAudioMediaData.getPCMFormat().getBlockAlign();
                                newFormat.BitsPerSample = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getBitDepth());
                newFormat.BlockAlign = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                newFormat.Channels = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getNumberOfChannels());

                newFormat.FormatTag = WaveFormatTag.Pcm;

                newFormat.SamplesPerSecond = (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate();

                // loads  format to buffer description
                BufferDesc.Format = newFormat;

                // enable buffer description properties
                BufferDesc.ControlVolume = true;
                BufferDesc.ControlFrequency = true;

                // calculate size of buffer so as to contain 1 second of audio
                m_SizeBuffer = (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate() * mCurrentAudioMediaData.getPCMFormat().getBlockAlign();
                m_RefreshLength = (int)(mCurrentAudioMediaData.getPCMFormat().getSampleRate() / 2) * mCurrentAudioMediaData.getPCMFormat().getBlockAlign();

                // calculate the size of VuMeter Update array length
                // m_UpdateVMArrayLength = m_SizeBuffer / 20;
                // m_UpdateVMArrayLength = Convert.ToInt32(CalculationFunctions.AdaptToFrame(Convert.ToInt32(m_UpdateVMArrayLength), m_FrameSize));
                // arUpdateVM = new byte[m_UpdateVMArrayLength];
                // reset the VuMeter (if set)
                // if (ob_VuMeter != null) ob_VuMeter.Reset();

                // sets the calculated size of buffer
                BufferDesc.BufferBytes = m_SizeBuffer;

                // Global focus is set to true so that the sound can be played in background also
                BufferDesc.GlobalFocus = true;

                // initialising secondary buffer
                // SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
                SoundBuffer = new SecondaryBuffer(BufferDesc, mDevice.Device);

                SoundBuffer.Frequency = (int)(SoundBuffer.Format.SamplesPerSecond * m_fFastPlayFactor);
            }// end of state check
                            } // end function

        private void PlayAssetStream(long lStartPosition, long lEndPosition)
        {
            if (m_State != PlayerState.Playing)
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                m_SamplingRate = (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate();

                // lEndPosition = 0 means that file is played to end
                if (lEndPosition != 0)
                {
                    m_lLength = (lEndPosition); // -lStartPosition;
                }
                else
                {
                    // folowing one line is modified on 2 Aug 2006
                    //m_lLength = (m_Asset .SizeInBytes  - lStartPosition ) ;
                    m_lLength = (mCurrentAudioMediaData.getPCMLength());
                }


                // initialize M_lPlayed for this asset
                m_lPlayed = lStartPosition;

                m_IsEndOfAsset = false;
                // following line added  for fast play update
                m_IsLastRefresh = false;
                
                mAudioStream = mCurrentAudioMediaData.getAudioData();
                mAudioStream.Position = lStartPosition;
                
                SoundBuffer.Write(0, mAudioStream, m_SizeBuffer, 0);

                // Adds the length (count) of file played into a variable
                m_lPlayed += m_SizeBuffer;

                // trigger  events (modified JQ)
                ChangeState(PlayerState.Playing);

                if ( m_PlaybackMode != PlaybackModes.Normal )
                m_IsFwdRwd = true;

                MoniteringTimer.Enabled = true;
                // starts playing
                SoundBuffer.Play(0, BufferPlayFlags.Looping);
                m_BufferCheck = 1;

                //initialise and start thread for refreshing buffer
                RefreshThread = new Thread(new ThreadStart(RefreshBuffer));
                RefreshThread.Start();


            }
        }

		void RefreshBuffer ()
		{
		
			// int ReadPosition;
			
			// variable to prevent least count errors in clip end time
            long SafeMargin = CalculationFunctions.ConvertTimeToByte(1, m_SamplingRate, m_FrameSize);


			while ( ( m_lPlayed < m_lLength - SafeMargin )    &&     ( m_IsLastRefresh == false ) )
			{//1
				if (SoundBuffer.Status.BufferLost  )
					SoundBuffer.Restore () ;

				
				Thread.Sleep (50) ;

                /*if (ob_VuMeter != null)
                {
                    ReadPosition = SoundBuffer.PlayPosition;

                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(SoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);
                        //if ( m_EventsEnabled == true)
                        //ob_UpdateVuMeter.NotifyUpdateVuMeter ( this, ob_UpdateVuMeter ) ;
                        //UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ // temp for debugging tk
                    }
                }*/
				// check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
				if ((m_BufferCheck% 2) == 1 &&  SoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
						//LoadStream (false) ;
							SoundBuffer.Write (0 , mAudioStream , m_RefreshLength, 0) ;
                    // following one line commented on 22 April 2007 , m_lPlayed update is moved to loadStream  function 
					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
				}//-1
                    // refresh Rear half of buffer for even count
				else if ((m_BufferCheck % 2 == 0) &&  SoundBuffer.PlayPosition < m_RefreshLength)
				{//1
						//LoadStream (false) ;
							SoundBuffer.Write (m_RefreshLength,  mAudioStream, m_RefreshLength, 0)  ;
                            // following one line commented on 22 April 2007 , m_lPlayed update is moved to loadStream  function 
					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
					// end of even/ odd part of buffer;
                    				}//-1

				// end of while
			}
            
            m_IsEndOfAsset = false;
            int LengthDifference = (int)(m_lPlayed - m_lLength  );
             m_BufferStopPosition= -1 ;
                         // if there is no refresh after first load thenrefresh maps directly  or
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
            //Thread.Sleep(500);
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

			
			// Stopping process begins
                                m_BufferStopPosition = -1 ;
                m_lPausePosition = 0;
			SoundBuffer.Stop () ;
			// if (ob_VuMeter != null) ob_VuMeter.Reset () ;
            mAudioStream.Close();

			// changes the state and trigger events
            ChangeState(PlayerState.Stopped);

            if (m_EventsEnabled)
                m_IsEventEnabledDelayedTillTimer= true;
            else
                m_IsEventEnabledDelayedTillTimer= false;

            m_IsEndOfAsset = true;

			// RefreshBuffer ends
		}
        private bool m_IsEventEnabledDelayedTillTimer= true ;
        //private int m_MemoryStreamPosition  =0 ; // tk temporary kept, will be removed.


        ///<summary>
        /// Function for simulating playing for assets with no audio
        /// </summary>
        ///
        private void SimulateEmptyAssetPlaying()
        {
            // m_Channels = mCurrentAudioMediaData.getPCMFormat ().getNumberOfChannels ()  ;
            // m_FrameSize = mCurrentAudioMediaData.getPCMFormat ().getNumberOfChannels ()  * (mCurrentAudioMediaData.getPCMFormat ().getBitDepth ()  / 8);
            // m_SamplingRate = (int)  mCurrentAudioMediaData.getPCMFormat ().getSampleRate () ;
            ChangeState(PlayerState.Playing);
            Thread.Sleep(50);
            ChangeState(PlayerState.Stopped);
            if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
        }


		// contains the end position  to play to be used in starting playing  after seeking
		long lByteTo = 0 ;		
        private long m_lPausePosition ;
		
        public void Pause()
		{
			if (m_State.Equals(PlayerState .Playing) || m_PlaybackMode != PlaybackModes.Normal)
			{
                m_lPausePosition = GetCurrentBytePosition();
                StopFunction();
                ChangeState(PlayerState.Paused);
			}
		}

		public void Resume()
		{
            			if (m_State.Equals(PlayerState.Paused))
			{
                
                long lPosition = CalculationFunctions.AdaptToFrame( m_lPausePosition , mCurrentAudioMediaData.getPCMFormat ().getBlockAlign ()  );
                if (lPosition >= 0 && lPosition < mCurrentAudioMediaData.getPCMLength () )
                {
                    m_StartPosition = lPosition;
                                        InitPlay(lPosition, 0);
                }
                else
                    throw new Exception("Start Position is out of bounds of Audio Asset");
			}			
		}

		public void Stop()
		{
			if (m_State != PlayerState.Stopped || m_PlaybackMode != PlaybackModes.Normal)			
			{
				//SoundBuffer.Stop();
                //RefreshThread.Abort();
				//if (ob_VuMeter != null) ob_VuMeter.Reset();			
                StopFunction();
			}
            m_lPausePosition = 0;
            ChangeState(PlayerState.Stopped);
		}

        private void StopFunction ()
        {
            if (m_PlaybackMode  != PlaybackModes.Normal)
                StopForwardRewind();


            
            SoundBuffer.Stop();
                        if (RefreshThread != null &&    RefreshThread.IsAlive )
                            RefreshThread.Abort();

                        m_BufferStopPosition = -1;

            				//if (ob_VuMeter != null) ob_VuMeter.Reset();

                                        mAudioStream.Close();
                    }

private 		 long GetCurrentBytePosition()
		{
            int PlayPosition = 0;
            long lCurrentPosition = 0 ;

            if (mCurrentAudioMediaData.getPCMLength() > 0)
            {
                if ( m_State == PlayerState.Playing)
                {
                    PlayPosition = SoundBuffer.PlayPosition;

                    if (m_BufferStopPosition != -1)
                    {
                        int subtractor = (m_BufferStopPosition - PlayPosition);
                        lCurrentPosition = mCurrentAudioMediaData.getPCMLength() - subtractor;
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

                    if (lCurrentPosition >= mCurrentAudioMediaData.getPCMLength())
                        lCurrentPosition = mCurrentAudioMediaData.getPCMLength() - Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, m_SamplingRate, m_FrameSize));


                }
            }
            else if (m_State == PlayerState.Paused)
                lCurrentPosition = m_lPausePosition;

                        lCurrentPosition = CalculationFunctions.AdaptToFrame(lCurrentPosition, m_FrameSize);
                
                        if (m_PlaybackMode != PlaybackModes.Normal)
                        lCurrentPosition = m_lChunkStartPosition;

			return lCurrentPosition ;
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

			if (localPosition > mCurrentAudioMediaData.getPCMLength ()  )
				localPosition = mCurrentAudioMediaData.getPCMLength ()  - 100;


			m_EventsEnabled = false ;

			if (SoundBuffer.Status.Looping)
			{

					Stop();
					Thread.Sleep (30) ;
					m_StartPosition = localPosition ;
					InitPlay(localPosition , 0);
				
			}		
			
			else if(m_State.Equals (PlayerState .Paused ) )
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
                                mPreviewTimer.Interval = 100;
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
                mPreviewTimer.Interval = 100;
                mPreviewTimer.Start();
            }
        }

        

        ///Preview timer tick function
        private void PreviewTimer_Tick(object sender, EventArgs e)
        { //1
            
            double StepInMs = 3000 * m_FwdRwdRate  ;
            long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)  mCurrentAudioMediaData.getPCMFormat().getSampleRate ()  , mCurrentAudioMediaData.getPCMFormat().getBlockAlign ());
            int PlayChunkLength = 1200;
            long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte( PlayChunkLength , (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate(), mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            mPreviewTimer.Interval = PlayChunkLength + 100;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if ( m_PlaybackMode  == PlaybackModes.FastForward  )
            { //2
                if (( mCurrentAudioMediaData.getPCMLength () - m_lChunkStartPosition ) >  lStepInBytes )
                { //3
                    PlayStartPos = m_lChunkStartPosition;
PlayEndPos  = m_lChunkStartPosition + lPlayChunkLength  ;
PlayAssetStream  (  PlayStartPos, PlayEndPos);
m_lChunkStartPosition += lStepInBytes ;
                                    } //-3
                else
                { //3
                    Stop();
                    if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
                                } //-3
            } //-2
            else if ( m_PlaybackMode  ==  PlaybackModes.Rewind )
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                if (m_lChunkStartPosition > lPlayChunkLength)
                { //3
                    PlayStartPos = m_lChunkStartPosition - lPlayChunkLength;
                    PlayEndPos = m_lChunkStartPosition;
                    PlayAssetStream (PlayStartPos, PlayEndPos);
                    m_lChunkStartPosition -= lStepInBytes ;
                } //-3
                else
                {
                    Stop();
                    if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
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
                    if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
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
