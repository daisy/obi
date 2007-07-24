using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using urakawa.media.data.audio;

// TODO change all longs to ints

namespace Obi.Audio
{
    /// <summary>
    /// The four states of the audio player.
    /// NotReady: the player has no output device set yet.
    /// Playing: sound is currently playing.
    /// Paused: playback was paused and can be resumed.
    /// Stopped: player is idle.
    /// </summary>
    public enum AudioPlayerState { NotReady, Stopped, Playing, Paused };

    /// <summary>
    /// Playback modes for AudioPlayer
    /// Normal: for normal playback
    /// FastForward: For playing small chunks while jumping forward
    /// Rewind: for playing small chunks while jumping backward
    /// </summary>
    public enum PlaybackMode { Normal, FastForward, Rewind };


	public class AudioPlayer
	{
        #region private members

        private Stream mAudioStream;           // audio stream
        private int mBufferStopPosition;       // stop position in buffer
        private AudioMediaData mCurrentAudio;  // the audio currently playing
        public bool mEventsEnabled;            // flag to temporarily enable or disable events
        private bool mIsFwdRwd;                // forward or rewind playback is going on
        private PlaybackMode mPlaybackMode;    // current playback mode
        private SecondaryBuffer mSoundBuffer;  // DX playback buffer
        private AudioPlayerState mState;       // player state
        
        private VuMeter ob_VuMeter;            // to be removed
        
        private OutputDevice mDevice;
        // integer to indicate which part of buffer is to be refreshed front or rear
        private int m_BufferCheck;
        // Size of buffer created for playing
        private int m_SizeBuffer;
        // length of buffer to be refreshed during playing which is half of buffer size
        private int m_RefreshLength;
        // Total length of audio asset being played
        private long m_lLength;
        // Length of audio asset in bytes which had been played
        private long m_lPlayed;
        // thread for refreshing buffer while playing 
        private Thread RefreshThread;
        private long m_lPausePosition; // holds pause position in bytes to allow play resume playback from there
        private long m_lResumeToPosition; // In case play ( from, to ) function is used, holds the end position i.e. "to"  for resuming playback
        internal int m_FrameSize;
        internal int m_Channels;
        private int m_SamplingRate;
        private int m_VolumeLevel;
        private int m_FwdRwdRate; // holds skip time multiplier for forward / rewind mode
        private float m_fFastPlayFactor; /// fholds fast play multiplier
        // monitoring timer to trigger events independent of refresh thread
        private System.Windows.Forms.Timer MoniteringTimer = new System.Windows.Forms.Timer();
        // Flag to trigger end of asset events, flag is set for a moment and again reset
        private bool m_IsEndOfAsset; // variable required to signal monitoring timer to trigger end of asset event
        // array for update current amplitude to VuMeter
        internal byte[] arUpdateVM;
        internal int m_UpdateVMArrayLength;

        #endregion

        public event Events.Audio.Player.EndOfAudioAssetHandler EndOfAudioAsset;
        public event Events.Audio.Player.StateChangedHandler StateChanged;
        public event Events.Audio.Player.UpdateVuMeterHandler UpdateVuMeter;


        /// <summary>
        /// Create a new player. It doesn't have an output device yet.
        /// </summary>
        public AudioPlayer()
        {
            mState = AudioPlayerState.NotReady;
            ob_VuMeter = null;
            MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
            MoniteringTimer.Interval = 200;
            mPreviewTimer.Tick += new System.EventHandler(this.PreviewTimer_Tick);
            mPreviewTimer.Interval = 100;
            mPlaybackMode = PlaybackMode.Normal;
            m_FwdRwdRate = 1;
            m_fFastPlayFactor = 1;
            mIsFwdRwd = false;
            mEventsEnabled = true;
            m_lResumeToPosition = 0;
            mBufferStopPosition = -1;
            m_IsEndOfAsset = false;
        }


        /// <summary>
        /// The audio data currently playing.
        /// </summary>
        public AudioMediaData CurrentAudio { get { return mCurrentAudio; } }

        /// <summary>
        /// Currently used output device.
        /// </summary>
        public OutputDevice OutputDevice { get { return mDevice; } }

        /// <summary>
        /// Get and set the active playback mode.
                /// </summary>
        public PlaybackMode PlaybackMode
        {
            get { return mPlaybackMode; }
            set 
            {
                                SetPlaybackMode(value); 
            }
        }

        /// <summary>
        /// Current state of the player.
        /// </summary>
        public AudioPlayerState State
        {
            get
            {
                if (mIsFwdRwd) return AudioPlayerState.Playing;
                else 
                    return mState;
            }
        }

        /// <summary>
        /// Get the current playback position in bytes.
        /// </summary>
        private long GetCurrentBytePosition()
        {
            int PlayPosition = 0;
            long lCurrentPosition = 0;
            if (mCurrentAudio.getPCMLength() > 0)
            {
                if (mState == AudioPlayerState.Playing)
                {
                    PlayPosition = mSoundBuffer.PlayPosition;
                    // if refreshing of buffer has finished and player is near end of asset
                    if (mBufferStopPosition != -1)
                    {
                        int subtractor = (mBufferStopPosition - PlayPosition);
                        lCurrentPosition = mCurrentAudio.getPCMLength() - subtractor;
                    }
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
                    if (lCurrentPosition >= mCurrentAudio.getPCMLength())
                    {
                        lCurrentPosition = mCurrentAudio.getPCMLength() -
                            Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, m_SamplingRate, m_FrameSize));
                    }
                }
                        else if (mState == AudioPlayerState.Paused)
            {
                lCurrentPosition = m_lPausePosition;
            }
                        if (mPlaybackMode != PlaybackMode.Normal) lCurrentPosition = m_lChunkStartPosition;
            lCurrentPosition = CalculationFunctions.AdaptToFrame(lCurrentPosition, m_FrameSize);
        }
            return lCurrentPosition;
        }

        /// <summary>
        /// Set a new playback mode.
        /// </summary>
        /// <param name="mode">The new mode.</param>
        private void SetPlaybackMode(PlaybackMode mode)
        {
            if (mode != mPlaybackMode)
            {
                                                    if (State == AudioPlayerState.Playing)
                {
                                        long restartPos = GetCurrentBytePosition();
                    StopPlayback();
                    mState = AudioPlayerState.Paused;
                    mPlaybackMode = mode;
                    
                    InitPlay( mCurrentAudio ,  restartPos, 0);
                }
                else if (mState == AudioPlayerState.Paused || mState == AudioPlayerState.Stopped)
                {
                    mPlaybackMode = mode;
                }
            }
        }

        /// <summary>
        /// Stop rewinding or forwarding, including the preview timer.
        /// </summary>
        private void StopForwardRewind()
        {
            if (mPlaybackMode != PlaybackMode.Normal || mPreviewTimer.Enabled)
            {
                mPreviewTimer.Enabled = false;
//                m_FwdRwdRate = 1;
                m_lChunkStartPosition = 0;
                mIsFwdRwd = false;
                                mEventsEnabled = true;
            }
        }

        /// <summary>
        /// Stop the playback and revert to normal playback mode.
        /// </summary>
        private void StopPlayback()
        {
            StopForwardRewind();
            mSoundBuffer.Stop();
            if (RefreshThread != null && RefreshThread.IsAlive) RefreshThread.Abort();
            mBufferStopPosition = -1;
            if (ob_VuMeter != null) ob_VuMeter.Reset();  // TODO replace with an event
            mAudioStream.Close();
        }

        /// <summary>
        /// Convenience method for sending a state change event only if events are enabled and there is a listener.
        /// </summary>
		private void TriggerStateChangedEvent(Events.Audio.Player.StateChangedEventArgs e)
		{
			if (mEventsEnabled && StateChanged != null) StateChanged(this, e);
		}



        /// <summary>
        /// The Vu meter associated with the player.
        /// </summary>
        /// TODO this must be removed; the audio player communicates with any VU meter through events.
        public VuMeter VuMeter
        {
            get { return ob_VuMeter; }
            set { ob_VuMeter = value; }
        }







        /// <summary>
        /// Forward / Rewind rate.
        /// Whenever set to 0, Playback mode resets to normal
        /// </summary>
        public int PlaybackFwdRwdRate
        {
            get
            {
                return m_FwdRwdRate;
            }
            set
            {
                m_FwdRwdRate = value;
                if (m_FwdRwdRate == 0 && mPlaybackMode != PlaybackMode.Normal)
                {
                    MessageBox.Show("FwdRwd rate is 0 ");
                    SetPlaybackMode(PlaybackMode.Normal);
                }
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

            if (mSoundBuffer != null)
                mSoundBuffer.Volume = m_VolumeLevel;

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
            mState = AudioPlayerState.Stopped;
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
                mState = AudioPlayerState.Stopped;
            }
            else if (devices.Count > 0)
            {
                SetDevice(FormHandle, devices[0]);
                mState = AudioPlayerState.Stopped;
            }
            else
            {
                mState = AudioPlayerState.NotReady;
                throw new Exception("No output device available.");
            }
        }

        void SetPlayFrequency(float l_frequency)
        {
            if (mSoundBuffer != null
                && mPlaybackMode == PlaybackMode.Normal)
            {
                try
                {
                    mSoundBuffer.Frequency = (int)(mSoundBuffer.Format.SamplesPerSecond * l_frequency);
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


        /// <summary>
        ///  Plays an asset from beginning to end
        /// <see cref=""/>
        /// </summary>
        /// <param name="asset"></param>
        public void Play( AudioMediaData asset)
        {
            // This is public function so API state will be used
            if (State == AudioPlayerState.Stopped || State == AudioPlayerState.Paused)
            {
                m_StartPosition = 0;
                //m_State = AudioPlayerState.NotReady;
                
                if ( asset.getAudioDuration().getTimeDeltaAsMillisecondFloat() != 0)
                    InitPlay(asset ,  0, 0);
                else
                    SimulateEmptyAssetPlaying();

            }
        }



        /// <summary>
        ///  Plays an asset from a specified time position its to end
        /// <see cref=""/>
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="timeFrom"></param>
        public void Play(AudioMediaData  asset, double timeFrom)
        {
            // it is public function so API state will be used
            if (State == AudioPlayerState.Stopped || State == AudioPlayerState.Paused)
            {
                //m_State = AudioPlayerState.NotReady;
                if (asset.getAudioDuration().getTimeDeltaAsMillisecondFloat() > 0)
                {
                    long lPosition = CalculationFunctions.ConvertTimeToByte(timeFrom, (int)asset.getPCMFormat().getSampleRate(), asset.getPCMFormat().getBlockAlign());
                    lPosition = CalculationFunctions.AdaptToFrame(lPosition, asset.getPCMFormat().getBlockAlign());
                    if (lPosition >= 0 && lPosition <= asset.getPCMLength())
                    {
                                                m_StartPosition = lPosition;
                        InitPlay( asset , lPosition, 0);
                    }
                    else throw new Exception("Start Position is out of bounds of Audio Asset");
                }
                else    // if m_Asset.AudioLengthInBytes= 0 i.e. empty asset
                {
                    SimulateEmptyAssetPlaying();
                }
            }
        }


        /// <summary>
        ///  Plays an asset from a specified time position upto another specified time position
        /// <see cref=""/>
        /// </summary>
        /// <param name="asset"></param>
        /// <param name="timeFrom"></param>
        /// <param name="timeTo"></param>
        public void Play(AudioMediaData asset, double timeFrom, double timeTo)
        {
            //m_Asset = asset as Assets.AudioMediaAsset; //tk

            if ( asset.getAudioDuration().getTimeDeltaAsMillisecondFloat() > 0)
            {
                long lStartPosition = CalculationFunctions.ConvertTimeToByte(timeFrom, (int) asset.getPCMFormat().getSampleRate(), asset.getPCMFormat().getBlockAlign());
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, asset.getPCMFormat().getBlockAlign());
                long lEndPosition = CalculationFunctions.ConvertTimeToByte(timeTo, (int)asset.getPCMFormat().getSampleRate(), asset.getPCMFormat().getBlockAlign());
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition,  asset.getPCMFormat().getBlockAlign());
                // check for valid arguments
                if (lStartPosition > 0 && lStartPosition < lEndPosition && lEndPosition <= asset.getPCMLength())
                {
                    InitPlay( asset , lStartPosition, lEndPosition);
                }
                else
                    throw new Exception("Start Position is out of bounds of Audio Asset");
            }
            else
            {
                SimulateEmptyAssetPlaying();
            }
        }


private void InitPlay(AudioMediaData asset ,   long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (mState != AudioPlayerState.Playing )
            {
                                InitialiseWithAsset (asset ) ;

                    if (mPlaybackMode  == PlaybackMode.Normal)
                        PlayAssetStream( lStartPosition , lEndPosition);
                    else if (mPlaybackMode  == PlaybackMode.FastForward)
                    {
                                                FastForward( lStartPosition );
                    }
                    else if (mPlaybackMode == PlaybackMode.Rewind)
                    {
                                                if (lStartPosition == 0)
                            lStartPosition = mCurrentAudio.getPCMLength();
                        Rewind(lStartPosition);
                    }
            }// end of state check
			// end of function
		}


        private void InitialiseWithAsset(AudioMediaData audio)
        {
            if (mState != AudioPlayerState.Playing)
            {
                mCurrentAudio = audio;
                WaveFormat newFormat = new WaveFormat();
                 BufferDescription  BufferDesc = new BufferDescription();
                
                // retrieve format from asset
                m_FrameSize = mCurrentAudio.getPCMFormat().getBlockAlign();
                m_Channels = mCurrentAudio.getPCMFormat().getNumberOfChannels();
                newFormat.AverageBytesPerSecond = (int)mCurrentAudio.getPCMFormat().getSampleRate() * mCurrentAudio.getPCMFormat().getBlockAlign();
                                newFormat.BitsPerSample = Convert.ToInt16(mCurrentAudio.getPCMFormat().getBitDepth());
                newFormat.BlockAlign = Convert.ToInt16(mCurrentAudio.getPCMFormat().getBlockAlign());
                newFormat.Channels = Convert.ToInt16(mCurrentAudio.getPCMFormat().getNumberOfChannels());

                newFormat.FormatTag = WaveFormatTag.Pcm;

                newFormat.SamplesPerSecond = (int)mCurrentAudio.getPCMFormat().getSampleRate();

                // loads  format to buffer description
                BufferDesc.Format = newFormat;

                // enable buffer description properties
                BufferDesc.ControlVolume = true;
                BufferDesc.ControlFrequency = true;

                // calculate size of buffer so as to contain 1 second of audio
                m_SizeBuffer = (int)mCurrentAudio.getPCMFormat().getSampleRate() * mCurrentAudio.getPCMFormat().getBlockAlign();
                m_RefreshLength = (int)(mCurrentAudio.getPCMFormat().getSampleRate() / 2) * mCurrentAudio.getPCMFormat().getBlockAlign();

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
                mSoundBuffer = new SecondaryBuffer(BufferDesc, mDevice.Device);

                SetPlayFrequency(m_fFastPlayFactor);

            }// end of state check
                            } // end function

        private void PlayAssetStream(long lStartPosition, long lEndPosition)
        {
            if (mState != AudioPlayerState.Playing)
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, mCurrentAudio.getPCMFormat().getBlockAlign());
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, mCurrentAudio.getPCMFormat().getBlockAlign());
                m_SamplingRate = (int)mCurrentAudio.getPCMFormat().getSampleRate();
                
                // lEndPosition = 0 means that file is played to end
                if (lEndPosition != 0)
                {
                    m_lLength = (lEndPosition); // -lStartPosition;
                }
                else
                {
                    // folowing one line is modified on 2 Aug 2006
                    //m_lLength = (m_Asset .SizeInBytes  - lStartPosition ) ;
                    m_lLength = (mCurrentAudio.getPCMLength());
                }


                // initialize M_lPlayed for this asset
                m_lPlayed = lStartPosition;

                m_IsEndOfAsset = false;
                
                mAudioStream = mCurrentAudio.getAudioData();
                mAudioStream.Position = lStartPosition;
                
                mSoundBuffer.Write(0, mAudioStream, m_SizeBuffer, 0);

                // Adds the length (count) of file played into a variable
                m_lPlayed += m_SizeBuffer;

                // trigger  events (modified JQ)
                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
                mState = AudioPlayerState.Playing;
                TriggerStateChangedEvent(e);

                MoniteringTimer.Enabled = true;
                // starts playing
                mSoundBuffer.Play(0, BufferPlayFlags.Looping);
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
				if (mSoundBuffer.Status.BufferLost  )
					mSoundBuffer.Restore () ;

				
				Thread.Sleep (50) ;

                if (ob_VuMeter != null)
                {
                    ReadPosition = mSoundBuffer.PlayPosition;

                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
                    {
                        Array.Copy(mSoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);
                        //if ( m_EventsEnabled == true)
                        //UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ // temp for debugging tk
                    }
                }
				// check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
				if ((m_BufferCheck% 2) == 1 &&  mSoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
													mSoundBuffer.Write (0 , mAudioStream , m_RefreshLength, 0) ;
                    					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
				}//-1
                    // refresh Rear half of buffer for even count
				else if ((m_BufferCheck % 2 == 0) &&  mSoundBuffer.PlayPosition < m_RefreshLength)
				{//1
													mSoundBuffer.Write (m_RefreshLength,  mAudioStream, m_RefreshLength, 0)  ;
                            					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
					// end of even/ odd part of buffer;
                    				}//-1

				// end of while
			}
            
            m_IsEndOfAsset = false;
            int LengthDifference = (int)(m_lPlayed - m_lLength  );
             mBufferStopPosition= -1 ;
                         // if there is no refresh after first load thenrefresh maps directly  
                        if  ( m_BufferCheck == 1  )
                            {
                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference );
            }

            // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
            else if ((m_BufferCheck % 2) == 0)
            {
                mBufferStopPosition = Convert.ToInt32(m_RefreshLength - LengthDifference );
                            }
            else if ((m_BufferCheck >  1) && (m_BufferCheck % 2) == 1)
            {
                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
            }
            
            int CurrentPlayPosition;
            CurrentPlayPosition = mSoundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32 (CalculationFunctions.ConvertTimeToByte( 70 , m_SamplingRate, m_FrameSize));
            StopMargin = (int)  (StopMargin * m_fFastPlayFactor);

            if ( mBufferStopPosition < StopMargin)
                mBufferStopPosition = StopMargin; 

             while (CurrentPlayPosition < (mBufferStopPosition - StopMargin) || CurrentPlayPosition > ( mBufferStopPosition ))
                {
                    Thread.Sleep(50);
                    CurrentPlayPosition = mSoundBuffer.PlayPosition;
                }

			
			// Stopping process begins
                                mBufferStopPosition = -1 ;
                m_lPausePosition = 0;
			mSoundBuffer.Stop () ;
			if (ob_VuMeter != null) ob_VuMeter.Reset () ;
            mAudioStream.Close();

			// changes the state and trigger events
            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
            mState = AudioPlayerState.Stopped;

			TriggerStateChangedEvent(e);

            if (mEventsEnabled)
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
            
            m_Channels = mCurrentAudio.getPCMFormat ().getNumberOfChannels ()  ;
            m_FrameSize = mCurrentAudio.getPCMFormat ().getNumberOfChannels ()  * (mCurrentAudio.getPCMFormat ().getBitDepth ()  / 8);
            m_SamplingRate = (int)  mCurrentAudio.getPCMFormat ().getSampleRate () ;

            Events.Audio.Player.StateChangedEventArgs  e = new Events.Audio.Player.StateChangedEventArgs(mState);
            mState = AudioPlayerState.Playing;
            TriggerStateChangedEvent(e);
            

            Thread.Sleep(50);

            e = new Events.Audio.Player.StateChangedEventArgs(mState);
            mState = AudioPlayerState.Stopped;
            TriggerStateChangedEvent(e);

            // trigger end of asset event
            if (mEventsEnabled == true)
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
                    if (!mIsFwdRwd)
                        m_lResumeToPosition = m_lLength;
                    else
                        m_lResumeToPosition = 0;

                StopPlayback();

				// Change the state and trigger event
				Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState) ;
                mState = AudioPlayerState.Paused;
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
                
                long lPosition = CalculationFunctions.AdaptToFrame( m_lPausePosition , mCurrentAudio.getPCMFormat ().getBlockAlign ()  );
                long lEndPosition = CalculationFunctions.AdaptToFrame( m_lResumeToPosition , mCurrentAudio.getPCMFormat().getBlockAlign());

                if (lPosition >= 0 && lPosition < mCurrentAudio.getPCMLength () )
                {
                    m_StartPosition = lPosition;
                                        InitPlay( mCurrentAudio , lPosition, lEndPosition );
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
				                StopPlayback();
			}
            
        m_lPausePosition = 0;
			Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
            mState = AudioPlayerState.Stopped;
			TriggerStateChangedEvent(e);
		}



		public double GetCurrentTimePosition()
		{	
			return CalculationFunctions.ConvertByteToTime (GetCurrentBytePosition() , m_SamplingRate , m_FrameSize);
		}
		
		long m_StartPosition  ;

		void SetCurrentBytePosition (long localPosition) 
		{
			if (localPosition < 0)
				localPosition = 0;

			if (localPosition > mCurrentAudio.getPCMLength ()  )
				localPosition = mCurrentAudio.getPCMLength ()  - 100;


			mEventsEnabled = false ;

			if (State == AudioPlayerState.Playing  )
			{
                					Stop();
					Thread.Sleep (30) ;
					m_StartPosition = localPosition ;
                    					InitPlay( mCurrentAudio , localPosition , 0);
				
			}		
			
			else if(mState.Equals (AudioPlayerState .Paused ) )
			{
					m_StartPosition = localPosition ;
                    m_lPausePosition = localPosition;
																																	}
			mEventsEnabled = true ;

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
            if ( mPlaybackMode  !=  PlaybackMode.Normal )
            {
                                m_lChunkStartPosition = lStartPosition ;
                                                mEventsEnabled = false;
                                mIsFwdRwd = true;
                                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();
                
                            }
        }
        

        public void FastForward(long lStartPosition   )
        {

            // let's play forward!
            if (mPlaybackMode != PlaybackMode.Normal)
            {
                m_lChunkStartPosition = lStartPosition;
                mEventsEnabled = false;
                mIsFwdRwd = true;
                mPreviewTimer.Interval = 50;
                mPreviewTimer.Start();
            }
        }

        

        ///Preview timer tick function
        private void PreviewTimer_Tick(object sender, EventArgs e)
        { //1
            
            double StepInMs = 4000 * m_FwdRwdRate  ;
            long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)  mCurrentAudio.getPCMFormat().getSampleRate ()  , mCurrentAudio.getPCMFormat().getBlockAlign ());
            int PlayChunkLength = 1200;
            long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte( PlayChunkLength , (int)mCurrentAudio.getPCMFormat().getSampleRate(), mCurrentAudio.getPCMFormat().getBlockAlign());
            mPreviewTimer.Interval = PlayChunkLength + 50;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if ( mPlaybackMode  == PlaybackMode.FastForward  )
            { //2
                if (( mCurrentAudio.getPCMLength () - ( lStepInBytes + m_lChunkStartPosition ) ) >  lPlayChunkLength )
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

if (m_lChunkStartPosition > mCurrentAudio.getPCMLength())
    m_lChunkStartPosition = mCurrentAudio.getPCMLength();
                                    } //-3
                else
                { //3
                    Stop();
                    if (mEventsEnabled)
                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                } //-3
            } //-2
            else if ( mPlaybackMode  ==  PlaybackMode.Rewind )
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                if (m_lChunkStartPosition >  0 )
                { //3
                    if (m_lChunkStartPosition < mCurrentAudio.getPCMLength())
                        m_lChunkStartPosition -= lStepInBytes;
                    else
                        m_lChunkStartPosition = mCurrentAudio.getPCMLength() - lPlayChunkLength  ;

                    PlayStartPos = m_lChunkStartPosition ;
                    PlayEndPos = m_lChunkStartPosition +  lPlayChunkLength;
                    PlayAssetStream (PlayStartPos, PlayEndPos);
                    
                    if (m_lChunkStartPosition < 0)
                        m_lChunkStartPosition = 0;
                } //-3
                else
                {
                    Stop();
                    if (mEventsEnabled)
                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                    }
                } //-2
                            } //-1
       



        private void MoniteringTimer_Tick(object sender, EventArgs e)
        {
            if ( m_IsEndOfAsset == true)
            {
                m_IsEndOfAsset = false;
                MoniteringTimer.Enabled = false;

                if (m_IsEventEnabledDelayedTillTimer)
                {
                    if (EndOfAudioAsset != null)
                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                            
                                    }
            if (mEventsEnabled == true)
                m_IsEventEnabledDelayedTillTimer= true;
            else
                m_IsEventEnabledDelayedTillTimer= false;
            }

        }


    }
}
