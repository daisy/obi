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

    /// <summary>
    /// The audio player class can play one audio media data at a time.
    /// </summary>
	public class Player
	{
		public event EndOfAudioAssetHandler EndOfAudioAsset;  // the end of the asset was reached
		public event StateChangedHandler StateChanged;        // the state of the player has changed
		
        #region private members

        private bool mAtEndOfAsset;                           // flag to trigger end of asset event
        private bool mAtLastRefresh = false;                  // flag to notify the end of the refresh process
        private Stream mAudioStream;                          // stream for the current audio media data
        private int mBufferCheck;                             // indicates whether to check the front or rear of the buffer
        private BufferDescription mBufferDesc;                // buffer description for DirectX playback
        private int mBufferSize;                              // size of the playback buffer
        private int mBufferStopPosition;                      // position where the buffer refresh has stopped
        private AudioMediaData mCurrentAudioMediaData;        // audio media data currently being played
		private OutputDevice mDevice;                         // device for playback
        private bool mEnableEvents;                           // flag to temporarily enable/disable events
        private float mFastPlayFactor;                        // playback rate (changes the pitch)
        private int mFwdRwdRate;                              // rate of fast-forward/rewinding
        private bool mIsEventEnabledDelayedTillTimer = true;  // ???
        private System.Windows.Forms.Timer mMonitoringTimer;  // monitoring timer to send independent of refresh thread
        private long mPausePosition;                          // paused position (in bytes)
        private PlaybackModes mPlaybackMode;                  // current playback mode
        private long mPlayed;                                 // amount of data already played
        private long mPlayUntil;                              // position until which data is played
        private System.Windows.Forms.Timer mPreviewTimer;     // timer for playing chunks when ffwd/rwding.
        private int mRefreshSize;                             // how much of the buffer to refresh
        private Thread mRefreshThread;                        // thread for refreshing the buffer while playing
        private SecondaryBuffer mSoundBuffer;                 // DirectX playback buffer
        private long mStartPosition;                          // playback start position (in bytes)
        private PlayerState mState;                           // current playback state


        private static readonly int BUFFER_SIZE_IN_SECONDS = 1;       // 1-second playback buffer
        private static readonly int MONITORING_TIMER_INTERVAL = 200;  // interval for the monitoring timer (in ms)
        private static readonly int PREVIEW_TIMER_INTERVAL = 100;     // interval for the preview timer (in ms)
        private static readonly int SLEEP_INCREMENT = 50;             // time in ms for threads to sleep

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
            mCurrentAudioMediaData = null;
            mDevice = null;
            mEnableEvents = true;
            mFastPlayFactor = 1.0f;
            mFwdRwdRate = 1;
            mMonitoringTimer = new System.Windows.Forms.Timer();
            mMonitoringTimer.Tick += new System.EventHandler(this.MonitoringTimer_Tick);
            mMonitoringTimer.Interval = MONITORING_TIMER_INTERVAL;
            mPlaybackMode = PlaybackModes.Normal;
            mPreviewTimer = new System.Windows.Forms.Timer();
            mPreviewTimer.Tick += new System.EventHandler(this.mPreviewTimer_Tick);
            mPreviewTimer.Interval = PREVIEW_TIMER_INTERVAL;
            mState = PlayerState.NotReady;

            mPausePosition = 0;
            mStartPosition = 0;
        }

        public bool CanPlay { get { return mState == PlayerState.Stopped || mState == PlayerState.Paused; } }
        public bool CanPause { get { return mState == PlayerState.Playing; } }
        public bool CanResume { get { return mState == PlayerState.Paused; } }
        public bool CanStop { get { return mState == PlayerState.Playing || mState == PlayerState.Paused; } }

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
                        if (mBufferStopPosition != -1)
                        {
                            int subtractor = (mBufferStopPosition - playPosition);
                            currentPosition = mCurrentAudioMediaData.getPCMLength() - subtractor;
                        }
                        else if (mBufferCheck % 2 == 1)
                        {
                            // takes the lPlayed position and subtract the part of buffer played from it
                            int subtractor = (2 * mRefreshSize) - playPosition;
                            currentPosition = mPlayed - subtractor;
                        }
                        else
                        {
                            int subtractor = (3 * mRefreshSize) - playPosition;
                            currentPosition = mPlayed - subtractor;
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
                return mPlaybackMode != PlaybackModes.Normal ? m_lChunkStartPosition : currentPosition;
            }
            set
            {
                if (mCurrentAudioMediaData == null) throw new Exception("No current audio media data!");
                if (value < 0) throw new Exception("Cannot set position before the beginning!");
                if (value > mCurrentAudioMediaData.getPCMLength()) throw new Exception("Cannot set position after the end!");
                mEnableEvents = false;
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
                mEnableEvents = true;
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
            if (CanPlay)
            {
                Stop();
                SetAudioMediaData(newAudioMediaData);
                PlayDataStream(0, 0);
            }
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
            get { return mPlaybackMode != PlaybackModes.Normal ? PlayerState.Playing : mState; }
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
                if (mEnableEvents && StateChanged != null) StateChanged(this, e);
            }
        }

        /// <summary>
        /// Create the necessary buffers for playback.
        /// </summary>
        private void CreateBuffersForCurrentAudioMediaData()
        {
            WaveFormat format = CreateWaveFormatForCurrentAudioMediaData();
            mBufferSize = format.AverageBytesPerSecond * BUFFER_SIZE_IN_SECONDS;
            mRefreshSize = mBufferSize / 2;

            mBufferDesc = new BufferDescription(format);
            mBufferDesc.ControlVolume = true;
            mBufferDesc.ControlFrequency = true;
            mBufferDesc.BufferBytes = mBufferSize;
            mBufferDesc.GlobalFocus = true;

            mSoundBuffer = new SecondaryBuffer(mBufferDesc, mDevice.Device);
            mSoundBuffer.Frequency = (int)Math.Round(SampleRate * mFastPlayFactor);
        }

        /// <summary>
        /// Create a DirectX WaveFormat object for the current audio media data.
        /// </summary>
        private WaveFormat CreateWaveFormatForCurrentAudioMediaData()
        {
            WaveFormat format = new WaveFormat();
            format.AverageBytesPerSecond = SampleRate * FrameSize;
            format.BitsPerSample = (short)mCurrentAudioMediaData.getPCMFormat().getBitDepth();
            format.BlockAlign = (short)FrameSize;
            format.Channels = (short)mCurrentAudioMediaData.getPCMFormat().getNumberOfChannels();
            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = SampleRate;
            return format;
        }

        /// <summary>
        /// Tick of the monitoring timer; check for the end of the audio asset.
        /// </summary>
        private void MonitoringTimer_Tick(object sender, EventArgs e)
        {
            if (mAtEndOfAsset)
            {
                mAtEndOfAsset = false;
                mMonitoringTimer.Enabled = false;
                if (mIsEventEnabledDelayedTillTimer && EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
                mIsEventEnabledDelayedTillTimer = mEnableEvents;
            }
        }

        /// <summary>
        /// Play data stream from one position to another.
        /// </summary>
        /// <param name="lEndPosition">Use 0 to play until the end.</param>
        private void PlayDataStream(long startPos, long endPos)
        {
            //if (mState != PlayerState.Stopped) throw new Exception("Player is not stopped.");
            startPos = CalculationFunctions.AdaptToFrame(startPos, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            endPos = CalculationFunctions.AdaptToFrame(endPos, mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            mPlayed = startPos;
            mPlayUntil = endPos == 0 ? mCurrentAudioMediaData.getPCMLength() : endPos;
            mAtEndOfAsset = false;
            mAtLastRefresh = false;
            mAudioStream = mCurrentAudioMediaData.getAudioData();
            mAudioStream.Position = startPos;
            mSoundBuffer.Write(0, mAudioStream, mBufferSize, 0);
            mPlayed += mBufferSize;
            ChangeState(PlayerState.Playing);
            mMonitoringTimer.Enabled = true;
            mSoundBuffer.Play(0, BufferPlayFlags.Looping);
            mBufferCheck = 1;
            //initialise and start thread for refreshing buffer
            mRefreshThread = new Thread(new ThreadStart(RefreshBuffer));
            mRefreshThread.Start();
        }

        /// <summary>
        /// Refresh the playback buffer with data from the stream.
        /// </summary>
        private void RefreshBuffer()
        {
            // variable to prevent least count errors in clip end time
            long safeMargin = CalculationFunctions.ConvertTimeToByte(1, SampleRate, FrameSize);
            while ((mPlayed < mPlayUntil - safeMargin) && !mAtLastRefresh)
            {
                if (mSoundBuffer.Status.BufferLost) mSoundBuffer.Restore();
                Thread.Sleep(SLEEP_INCREMENT);
                // refresh the part of the buffer which the play cursor is not currently in (odd = front part)
                if ((mBufferCheck % 2 == 1) && mSoundBuffer.PlayPosition > mRefreshSize)
                {
                    mSoundBuffer.Write(0, mAudioStream, mRefreshSize, 0);
                    mPlayed += mRefreshSize;
                    mBufferCheck++;
                }
                else if ((mBufferCheck % 2 == 0) && mSoundBuffer.PlayPosition < mRefreshSize)
                {
                    mSoundBuffer.Write(mRefreshSize, mAudioStream, mRefreshSize, 0);
                    mPlayed += mRefreshSize;
                    mBufferCheck++;
                }
            }
            mAtEndOfAsset = false;
            mBufferStopPosition = (mBufferCheck % 2 == 0 ? mRefreshSize : mBufferSize) - ((int)mPlayed - (int)mPlayUntil);
            int StopMargin = (int)CalculationFunctions.ConvertTimeToByte(70, SampleRate, FrameSize);
            if (mBufferStopPosition < StopMargin) mBufferStopPosition = StopMargin;
            int currentPos = mSoundBuffer.PlayPosition;
            while (currentPos < (mBufferStopPosition - StopMargin) || currentPos > mBufferStopPosition)
            {
                Thread.Sleep(SLEEP_INCREMENT);
                currentPos = mSoundBuffer.PlayPosition;
            }
            mBufferStopPosition = -1;
            mPausePosition = 0;
            mSoundBuffer.Stop();
            mAudioStream.Close();
            ChangeState(PlayerState.Stopped);
            mIsEventEnabledDelayedTillTimer = mEnableEvents;
            mAtEndOfAsset = true;
        }

        /// <summary>
        /// Set a new audio media data object.
        /// </summary>
        private void SetAudioMediaData(AudioMediaData newAudioMediaData)
        {
            if (mState != PlayerState.Stopped) throw new Exception("Player is not stopped.");
            mCurrentAudioMediaData = newAudioMediaData;
            CreateBuffersForCurrentAudioMediaData();
        }





        






		
		private int m_VolumeLevel ;





		

        /// <summary>
        /// Gets and Sets Currently active playback mode in AudioPlayer
        /// Playback mode returns to normal on pause or stop
        /// </summary>
        public PlaybackModes PlaybackMode
        {
            get
            {
                return  mPlaybackMode;
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
                return mFwdRwdRate ;
            }
            set 
            {
                mFwdRwdRate = value ;
                if ( mFwdRwdRate == 0    &&    mPlaybackMode != PlaybackModes.Normal )
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
                return mFastPlayFactor;
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
                &&     mPlaybackMode == PlaybackModes.Normal )
            {
                mFastPlayFactor = l_frequency;
                mSoundBuffer.Frequency = (int)  ( mSoundBuffer.Format.SamplesPerSecond * mFastPlayFactor ) ;
            }
        }


        private void SetPlaybackMode(PlaybackModes l_PlaybackMode)
        {
            if (mState == PlayerState.Playing      ||     mPlaybackMode != PlaybackModes.Normal  )
            {
                                long RestartPos = 0;
                                                                    RestartPos = CurrentBytePosition;
                    
                StopFunction();
                mState = PlayerState.NotReady;
                mPlaybackMode = l_PlaybackMode;
                InitPlay ( RestartPos , 0 ) ;
            }
            else if (mState == PlayerState.Paused   ||   mState == PlayerState.Stopped )
            {
                mPlaybackMode = l_PlaybackMode;
            }
        }




        private void InitPlay(long lStartPosition, long lEndPosition)
		{
            //if (m_State == AudioPlayerState.Stopped || m_State == AudioPlayerState.NotReady)
                if (mState != PlayerState.Playing )
            {
            
                    //InitialiseWithAsset ( Data) ;

                    if (mPlaybackMode  == PlaybackModes.Normal)
                        PlayDataStream( lStartPosition , lEndPosition);
                    else if (mPlaybackMode  == PlaybackModes.FastForward)
                    {
                                                FastForward( lStartPosition );
                    }
                    else if (mPlaybackMode == PlaybackModes.Rewind)
                    {
                        if (lStartPosition == 0)
                            lStartPosition = mCurrentAudioMediaData.getPCMLength();
                        Rewind(lStartPosition);
                    }
            }// end of state check
			// end of function
		}

        
        
        







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
			if (mState.Equals(PlayerState .Playing) || mPlaybackMode != PlaybackModes.Normal)
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
			if (mState != PlayerState.Stopped || mPlaybackMode != PlaybackModes.Normal)			
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
            if (mPlaybackMode  != PlaybackModes.Normal)
                StopForwardRewind();


            
            mSoundBuffer.Stop();
                        if (mRefreshThread != null &&    mRefreshThread.IsAlive )
                            mRefreshThread.Abort();

                        mBufferStopPosition = -1;

            				//if (ob_VuMeter != null) ob_VuMeter.Reset();

                                        mAudioStream.Close();
                    }




        
        // position for starting chunk play
private          long m_lChunkStartPosition = 0;

        public void Rewind( long lStartPosition  )
        {
                        // let's play backward!
            if ( mPlaybackMode  !=  PlaybackModes.Normal )
            {
                m_lChunkStartPosition = lStartPosition ;
                                mEnableEvents = false;
                                mPreviewTimer.Interval = 100;
                mPreviewTimer.Start();
                
                            }
        }
        

        public void FastForward(long lStartPosition   )
        {

            // let's play forward!
            if (mPlaybackMode != PlaybackModes.Normal)
            {
                m_lChunkStartPosition = lStartPosition;
                mEnableEvents = false;
                mPreviewTimer.Interval = 100;
                mPreviewTimer.Start();
            }
        }



        ///Preview timer tick function
        private void mPreviewTimer_Tick(object sender, EventArgs e)
        { //1

            double StepInMs = 3000 * mFwdRwdRate;
            long lStepInBytes = CalculationFunctions.ConvertTimeToByte(StepInMs, (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate(), mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            int PlayChunkLength = 1200;
            long lPlayChunkLength = CalculationFunctions.ConvertTimeToByte(PlayChunkLength, (int)mCurrentAudioMediaData.getPCMFormat().getSampleRate(), mCurrentAudioMediaData.getPCMFormat().getBlockAlign());
            mPreviewTimer.Interval = PlayChunkLength + 100;

            long PlayStartPos = 0;
            long PlayEndPos = 0;
            if (mPlaybackMode == PlaybackModes.FastForward)
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
            else if (mPlaybackMode == PlaybackModes.Rewind)
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
            if (mPlaybackMode  != PlaybackModes.Normal || mPreviewTimer.Enabled == true)
            {
                mPreviewTimer.Enabled = false;
mFwdRwdRate                 = 1 ;
m_lChunkStartPosition = 0;
mPlaybackMode  = PlaybackModes.Normal;
                                mEnableEvents = true;
            }
                
        }


    }
}
