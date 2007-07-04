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
    /// NotReady: the player is not ready, i.e. it has no device to play.
    /// Playing: sound is currently playing.
    /// Paused: playback was interrupted and can be resumed or stopped.
    /// Stopped: player is stopped and ready for playing.
    /// </summary>
    public enum PlayerState { NotReady, Stopped, Playing, Paused };

    /// <summary>
    /// Playback modes for AudioPlayer
    /// Normal: play back at a normal rate.
    /// FastForward: play faster.
    /// Rewind: play faster and backward.
    /// </summary>
    public enum PlaybackModes { Normal, FastForward, Rewind} ;

    public class StateChangedEventArgs : EventArgs
    {
        public PlayerState PreviousState;
        public StateChangedEventArgs(PlayerState prev) { PreviousState = prev; }
    }

    public delegate void StateChangedHandler(Player player, StateChangedEventArgs e);
    public delegate void EndOfAudioAssetHandler(Player player, EventArgs e);
    // public delegate void UpdateVuMeterHandler(Player player, EventArgs e);

    /// <summary>
    /// The audio player class can play one audio media data at a time.
    /// </summary>
	public class Player
	{
		public event EndOfAudioAssetHandler EndOfAudioAsset;  // the end of the asset was reached
		public event StateChangedHandler StateChanged;        // the state of the player has changed
		
        #region private members

        private Stream mAudioStream;                          // stream for the current audio media data
        private BufferDescription mBufferDesc;                // buffer description for DirectX playback
        private AudioMediaData mCurrentAudioMediaData;        // audio media data currently being played
        private OutputDevice mDevice;                         // device for playback
        private bool mEventsEnabled;                          // flag to temporarily enable/disable events
        private System.Windows.Forms.Timer mMonitoringTimer;  // monitoring timer to send independent of refresh thread
        private long mPausePosition;                          // paused position (in bytes)
        System.Windows.Forms.Timer mPreviewTimer;             // timer for playing chunks when ffwd/rwding.
        private SecondaryBuffer mSoundBuffer;                 // DirectX playback buffer
        private long mStartPosition;                          // playback start position (in bytes)
        private PlayerState mState;                           // current playback state


        /* TODO: enable VU meter

        public event UpdateVuMeterHandler UpdateVuMeter;      // send updates to the VU meter

        internal int m_UpdateVMArrayLength;
        private VuMeter ob_VuMeter;
        internal byte[] arUpdateVM;                     // array for update current amplitude to VuMeter
        
        */

        #endregion


        #region audio properties

        /// <summary>
        /// Frame size of the current audio media data.
        /// </summary>
        private int FrameSize
        {
            get
            {
                if (mCurrentAudioMediaData == null) throw new Exception("No current audio media data!");
                return mCurrentAudioMediaData.getPCMFormat().getBlockAlign();
            }
        }

        /// <summary>
        /// Sample rate of the current audio media data.
        /// </summary>
        private int SampleRate
        {
            get
            {
                if (mCurrentAudioMediaData == null) throw new Exception("No current audio media data!");
                return (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate();
            }
        }

        #endregion


        /// <summary>
        /// Create a new audio player
        /// </summary>
        public Player()
        {
            mAudioStream = null;
            mBufferDesc = null;
            mCurrentAudioMediaData = null;
            mDevice = null;
            mEventsEnabled = true;
            mMonitoringTimer = new System.Windows.Forms.Timer();
            mMonitoringTimer.Tick += new System.EventHandler(this.MonitoringTimer_Tick);
            mMonitoringTimer.Interval = 200;
            mPausePosition = 0;
            mPreviewTimer = new System.Windows.Forms.Timer();
            mPreviewTimer.Tick += new System.EventHandler(this.mPreviewTimer_Tick);
            mPreviewTimer.Interval = 100;
            mSoundBuffer = null;
            mStartPosition = 0;
            mState = PlayerState.NotReady;


            // ob_VuMeter = null;
            m_PlaybackMode = PlaybackModes.Normal;
            m_FwdRwdRate = 1;
            m_fFastPlayFactor = 1;
        }


        /// <summary>
        /// Byte position of the player.
        /// </summary>
        public long CurrentBytePosition
        {
            get
            {
                if (mCurrentAudioMediaData == null) throw new Exception("No current audio media data!");
                long currentPosition = 0;
                if (mCurrentAudioMediaData.getPCMLength() > 0)
                {
                    if (mState == PlayerState.Playing)
                    {
                        int playPosition = mSoundBuffer.PlayPosition;
                        if (m_BufferStopPosition != -1)
                        {
                            int subtractor = (m_BufferStopPosition - playPosition);
                            currentPosition = mCurrentAudioMediaData.getPCMLength() - subtractor;
                        }
                        else if (m_BufferCheck % 2 == 1)
                        {
                            // takes the lPlayed position and subtract the part of buffer played from it
                            int subtractor = (2 * m_RefreshLength) - playPosition;
                            currentPosition = m_lPlayed - subtractor;
                        }
                        else
                        {
                            int subtractor = (3 * m_RefreshLength) - playPosition;
                            currentPosition = m_lPlayed - subtractor;
                        }
                        if (currentPosition >= mCurrentAudioMediaData.getPCMLength())
                        {
                            currentPosition = mCurrentAudioMediaData.getPCMLength() -
                                Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, SampleRate, FrameSize));
                        }
                    }
                }
                // TODO: check if this is correct
                else if (mState == PlayerState.Paused)
                {
                    currentPosition = mPausePosition;
                }
                currentPosition = CalculationFunctions.AdaptToFrame(currentPosition, FrameSize);
                return m_PlaybackMode != PlaybackModes.Normal ? m_lChunkStartPosition : currentPosition;
            }
            set
            {
                if (mCurrentAudioMediaData == null) throw new Exception("No current audio media data!");
                if (value < 0) throw new Exception("Cannot set position before the beginning!");
                if (value > mCurrentAudioMediaData.getPCMLength()) throw new Exception("Cannot set position after the end!");
                mEventsEnabled = false;
                if (mSoundBuffer.Status.Looping)
                {
                    Stop();
                    Thread.Sleep(30);
                    mStartPosition = value;
                    InitPlay(value, 0);
                }
                else if (mState.Equals(PlayerState.Paused))
                {
                    mStartPosition = value;
                    mPausePosition = value;
                }
                mEventsEnabled = true;
            }
        }

        /// <summary>
        /// Get or set the current time position
        /// </summary>
        public double CurrentTimePosition
        {
            get { return CalculationFunctions.ConvertByteToTime(CurrentBytePosition, SampleRate, FrameSize); }
            set { CurrentBytePosition = CalculationFunctions.ConvertTimeToByte(value, SampleRate, FrameSize); }
        }

        /// <summary>
        /// Get the currently selected device.
        /// </summary>
        public OutputDevice OutputDevice
        {
            get { return mDevice; }
        }

        /// <summary>
        /// Play an audio media data object from beginning to end.
        /// </summary>
        public void Play(AudioMediaData newAudioMediaData)
        {
            InitialiseWithAsset(newAudioMediaData);
            PlayDataStream(0, 0);
        }

        /// <summary>
        /// Set the device to be used by the player.
        /// </summary>
        public void SetDevice(Control handle, OutputDevice device)
        {
            if (mState == PlayerState.Playing || mState == PlayerState.Paused) Stop();
            mDevice = device;
            mDevice.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
            ChangeState(PlayerState.Stopped);
        }

        /// <summary>
        /// Set the device that matches this name; if it could not be found, default to the first one.
        /// Throw an exception if no devices were found.
        /// </summary>
        public void SetDevice(Control FormHandle, string name)
        {
            List<OutputDevice> devices = OutputDevices;
            if (devices.Count == 0) throw new Exception("No output device available.");
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            SetDevice(FormHandle, found == null ? devices[0] : found);
        }

        /// <summary>
        /// Get the current state of the player.
        /// </summary>
        public PlayerState State
        {
            // TODO: why is IsFwdRwd special?!
            get { return m_IsFwdRwd ? PlayerState.Playing : mState; }
        }


        /// <summary>
        /// Change the state of the player and send an event if anyone is listening.
        /// </summary>
        private void ChangeState(PlayerState newState)
        {
            if (newState != mState)
            {
                StateChangedEventArgs e = new StateChangedEventArgs(mState);
                mState = newState;
                if (mEventsEnabled && StateChanged != null) StateChanged(this, e);
            }
        }

        /// <summary>
        /// Tick of the monitoring timer; check for the end of the audio asset.
        /// </summary>
        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            if (m_IsEndOfAsset)
            {
                m_IsEndOfAsset = false;
                mMonitoringTimer.Enabled = false;
                if (m_IsEventEnabledDelayedTillTimer && EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
                m_IsEventEnabledDelayedTillTimer = mEventsEnabled;
            }
        }






        
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
		
		private int m_VolumeLevel ;
        private PlaybackModes m_PlaybackMode ;
        private int m_FwdRwdRate = 1 ;
        private float m_fFastPlayFactor = 1;
        private bool m_IsFwdRwd = false;




        // Flag to trigger end of asset events, flag is set for a moment and again reset
        private bool m_IsEndOfAsset = false ;

		

        /// <summary>
        /// Gets and Sets Currently active playback mode in AudioPlayer
        /// Playback mode returns to normal on pause or stop
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




        void SetPlayFrequency(float l_frequency)
        {
            if (mSoundBuffer != null
                &&     m_PlaybackMode == PlaybackModes.Normal )
            {
                m_fFastPlayFactor = l_frequency;
                mSoundBuffer.Frequency = (int)  ( mSoundBuffer.Format.SamplesPerSecond * m_fFastPlayFactor ) ;
            }
        }


        private void SetPlaybackMode(PlaybackModes l_PlaybackMode)
        {
            if (mState == PlayerState.Playing      ||     m_IsFwdRwd  )
            {
                                long RestartPos = 0;
                                                                    RestartPos = CurrentBytePosition;
                    
                StopFunction();
                mState = PlayerState.NotReady;
                m_PlaybackMode = l_PlaybackMode;
                InitPlay ( RestartPos , 0 ) ;
            }
            else if (mState == PlayerState.Paused   ||   mState == PlayerState.Stopped )
            {
                m_PlaybackMode = l_PlaybackMode;
            }
        }




        private void InitPlay(long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (mState != PlayerState.Playing )
            {
            
                    //InitialiseWithAsset ( Data) ;

                    if (m_PlaybackMode  == PlaybackModes.Normal)
                        PlayDataStream( lStartPosition , lEndPosition);
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

        
        
        
        private void InitialiseWithAsset(AudioMediaData newAudioMediaData)
        {
            if (mState != PlayerState.Playing)
            {
                mCurrentAudioMediaData = newAudioMediaData;
                WaveFormat format = new WaveFormat();
                mBufferDesc = new BufferDescription();
                mBufferDesc.ControlVolume = true;

                // retrieve format from asset
                format.AverageBytesPerSecond = SampleRate * FrameSize;
                format.BitsPerSample = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getBitDepth());
                format.BlockAlign = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                format.Channels = Convert.ToInt16(mCurrentAudioMediaData.getPCMFormat().getNumberOfChannels());

                format.FormatTag = WaveFormatTag.Pcm;

                format.SamplesPerSecond = (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate();

                // loads  format to buffer description
                mBufferDesc.Format = format;

                // enable buffer description properties
                mBufferDesc.ControlVolume = true;
                mBufferDesc.ControlFrequency = true;

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
                mBufferDesc.BufferBytes = m_SizeBuffer;

                // Global focus is set to true so that the sound can be played in background also
                mBufferDesc.GlobalFocus = true;

                // initialising secondary buffer
                // SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
                mSoundBuffer = new SecondaryBuffer(mBufferDesc, mDevice.Device);

                mSoundBuffer.Frequency = (int)(mSoundBuffer.Format.SamplesPerSecond * m_fFastPlayFactor);
            }// end of state check
                            } // end function



        private void PlayDataStream(long lStartPosition, long lEndPosition)
        {
            if (mState != PlayerState.Playing)
            {
                // Adjust the start and end position according to frame size
                lStartPosition = CalculationFunctions.AdaptToFrame(lStartPosition, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                lEndPosition = CalculationFunctions.AdaptToFrame(lEndPosition, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
                
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
                
                mSoundBuffer.Write(0, mAudioStream, m_SizeBuffer, 0);

                // Adds the length (count) of file played into a variable
                m_lPlayed += m_SizeBuffer;

                // trigger  events (modified JQ)
                ChangeState(PlayerState.Playing);

                if ( m_PlaybackMode != PlaybackModes.Normal )
                m_IsFwdRwd = true;

                mMonitoringTimer.Enabled = true;
                // starts playing
                mSoundBuffer.Play(0, BufferPlayFlags.Looping);
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
            long SafeMargin = CalculationFunctions.ConvertTimeToByte(1, SampleRate, FrameSize);


			while ( ( m_lPlayed < m_lLength - SafeMargin )    &&     ( m_IsLastRefresh == false ) )
			{//1
				if (mSoundBuffer.Status.BufferLost  )
					mSoundBuffer.Restore () ;

				
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
				if ((m_BufferCheck% 2) == 1 &&  mSoundBuffer.PlayPosition > m_RefreshLength) 
				{//2
						//LoadStream (false) ;
							mSoundBuffer.Write (0 , mAudioStream , m_RefreshLength, 0) ;
                    // following one line commented on 22 April 2007 , m_lPlayed update is moved to loadStream  function 
					m_lPlayed = m_lPlayed + m_RefreshLength ;
					m_BufferCheck++ ;
				}//-1
                    // refresh Rear half of buffer for even count
				else if ((m_BufferCheck % 2 == 0) &&  mSoundBuffer.PlayPosition < m_RefreshLength)
				{//1
						//LoadStream (false) ;
							mSoundBuffer.Write (m_RefreshLength,  mAudioStream, m_RefreshLength, 0)  ;
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
            CurrentPlayPosition = mSoundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32 (CalculationFunctions.ConvertTimeToByte( 70 , SampleRate, FrameSize));

            if ( m_BufferStopPosition < StopMargin)
                m_BufferStopPosition = StopMargin; 

             while (CurrentPlayPosition < (m_BufferStopPosition - StopMargin) || CurrentPlayPosition > ( m_BufferStopPosition ))
                {
                    Thread.Sleep(50);
                    CurrentPlayPosition = mSoundBuffer.PlayPosition;
                }

			
			// Stopping process begins
                                m_BufferStopPosition = -1 ;
                mPausePosition = 0;
			mSoundBuffer.Stop () ;
			// if (ob_VuMeter != null) ob_VuMeter.Reset () ;
            mAudioStream.Close();

			// changes the state and trigger events
            ChangeState(PlayerState.Stopped);

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
            ChangeState(PlayerState.Playing);
            Thread.Sleep(50);
            ChangeState(PlayerState.Stopped);
            if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
        }

		
        public void Pause()
		{
			if (mState.Equals(PlayerState .Playing) || m_PlaybackMode != PlaybackModes.Normal)
			{
                mPausePosition = CurrentBytePosition;
                StopFunction();
                ChangeState(PlayerState.Paused);
			}
		}

		public void Resume()
		{
            			if (mState.Equals(PlayerState.Paused))
			{
                
                long lPosition = CalculationFunctions.AdaptToFrame( mPausePosition , mCurrentAudioMediaData.getPCMFormat ().getBlockAlign ()  );
                if (lPosition >= 0 && lPosition < mCurrentAudioMediaData.getPCMLength () )
                {
                    mStartPosition = lPosition;
                                        InitPlay(lPosition, 0);
                }
                else
                    throw new Exception("Start Position is out of bounds of Audio Asset");
			}			
		}

		public void Stop()
		{
			if (mState != PlayerState.Stopped || m_PlaybackMode != PlaybackModes.Normal)			
			{
				//SoundBuffer.Stop();
                //RefreshThread.Abort();
				//if (ob_VuMeter != null) ob_VuMeter.Reset();			
                StopFunction();
			}
            mPausePosition = 0;
            ChangeState(PlayerState.Stopped);
		}

        private void StopFunction ()
        {
            if (m_PlaybackMode  != PlaybackModes.Normal)
                StopForwardRewind();


            
            mSoundBuffer.Stop();
                        if (RefreshThread != null &&    RefreshThread.IsAlive )
                            RefreshThread.Abort();

                        m_BufferStopPosition = -1;

            				//if (ob_VuMeter != null) ob_VuMeter.Reset();

                                        mAudioStream.Close();
                    }




        
        // position for starting chunk play
private          long m_lChunkStartPosition = 0;

        public void Rewind( long lStartPosition  )
        {
                        // let's play backward!
            if ( m_PlaybackMode  !=  PlaybackModes.Normal )
            {
                m_lChunkStartPosition = lStartPosition ;
                                mEventsEnabled = false;
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
                mEventsEnabled = false;
                mPreviewTimer.Interval = 100;
                mPreviewTimer.Start();
            }
        }



        ///Preview timer tick function
        private void mPreviewTimer_Tick(object sender, EventArgs e)
        { //1

            double StepInMs = 3000 * m_FwdRwdRate;
            long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate(), mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            int PlayChunkLength = 1200;
            long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte(PlayChunkLength, (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate(), mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            mPreviewTimer.Interval = PlayChunkLength + 100;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if (m_PlaybackMode == PlaybackModes.FastForward)
            { //2
                if ((mCurrentAudioMediaData.getPCMLength() - m_lChunkStartPosition) > lStepInBytes)
                { //3
                    PlayStartPos = m_lChunkStartPosition;
                    PlayEndPos = m_lChunkStartPosition + lPlayChunkLength;
                    PlayDataStream(PlayStartPos, PlayEndPos);
                    m_lChunkStartPosition += lStepInBytes;
                } //-3
                else
                { //3
                    Stop();
                    if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
                } //-3
            } //-2
            else if (m_PlaybackMode == PlaybackModes.Rewind)
            { //2
                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
                if (m_lChunkStartPosition > lPlayChunkLength)
                { //3
                    PlayStartPos = m_lChunkStartPosition - lPlayChunkLength;
                    PlayEndPos = m_lChunkStartPosition;
                    PlayDataStream(PlayStartPos, PlayEndPos);
                    m_lChunkStartPosition -= lStepInBytes;
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
                                mEventsEnabled = true;
            }
                
        }


    }
}
