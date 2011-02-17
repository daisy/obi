using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using AudioLib;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using urakawa.media.data.audio;

// TODO change all longs to ints

namespace Obi.Audio
{
//    /// <summary>
//    /// The four states of the audio player.
//    /// NotReady: the player has no output device set yet.
//    /// Playing: sound is currently playing.
//    /// Paused: playback was paused and can be resumed.
//    /// Stopped: player is idle.
//    /// </summary>
//    public enum AudioPlayerState { NotReady, Stopped, Playing, Paused };

//    public class AudioPlayer
//    {
//        #region private members

//        // member variables initialised once
//        private OutputDevice mDevice;
//        private System.Windows.Forms.Timer MoniteringTimer; // monitoring timer to trigger events independent of refresh thread
//                System.Windows.Forms.Timer mPreviewTimer ; // timer for playing chunks at interval during Forward/Rewind



//        // Member variables to be re initialised only at time of starting  playback of an asset
//        private Stream mAudioStream;           // audio stream
//                private AudioMediaData mCurrentAudio;  // the audio currently playing
//        private SecondaryBuffer mSoundBuffer;  // DX playback buffer
//                private int m_SizeBuffer; // Size of buffer created for playing
//                        private int m_RefreshLength;         // length of buffer to be refreshed during playing which is half of buffer size
//                                private long m_lLength;         // Total length of audio asset being played
//                                private Thread RefreshThread; // thread for refreshing buffer while playing 
//                                private int mFrameSize;
//                                private int m_Channels;
//        private int mSampleRate;
//        internal byte[] arUpdateVM; // array for update current amplitude to VuMeter
//        private int m_UpdateVMArrayLength; // length of VuMeter update array ( may be removed )


//        // Member variables changed more than ones ( in one asset session ) by functions in AudioPlayer class
//        private bool mIsFwdRwd;                // flag indicating forward or rewind playback is going on
//        private AudioPlayerState mState;       // player state
//        private int m_BufferCheck; // integer to indicate which part of buffer is to be refreshed front or rear, value is odd for refreshing front part and even for refreshing rear
//        private long m_lPlayed;         // Length of audio asset in bytes which had been played ( loadded to SoundBuffer )
//        private long mPausePosition; // holds pause position in bytes to allow play resume playback from there
//        private long m_lResumeToPosition; // In case play ( from, to ) function is used, holds the end position i.e. "to"  for resuming playback

//        private long m_lChunkStartPosition = 0; // position for starting chunk play in forward/Rewind

//        // Member variables changed by user 
//                                        private int m_VolumeLevel;
//        private int mFwdRwdRate; // holds skip time multiplier for forward / rewind mode , value is 0 for normal playback,  positive  for FastForward and negetive  for Rewind
//        private float m_fFastPlayFactor; /// fholds fast play multiplier
//        private bool m_IsPreviewing; // Is true when playback is used for previewing a  selection or marking.


//        // changed by AudioPlayer functions for a short time like enabling/disabling events, stopping buffer etc.
//        public bool mEventsEnabled;            // flag to temporarily enable or disable events
//        private bool m_IsEndOfAsset; // variable required to signal monitoring timer to trigger end of asset event, flag is set for a moment and again reset
//        private int mBufferStopPosition;       // used by refresh thread for stop position in buffer, value is negetive till refreshing of buffer is going on


//        #endregion

//        public event Events.Audio.Player.EndOfAudioAssetHandler EndOfAudioAsset;
//        public event Events.Audio.Player.StateChangedHandler StateChanged;
//        public event Events.Audio.Player.UpdateVuMeterHandler UpdateVuMeter;
//        public event Events.Audio.Player.ResetVuMeterHandler ResetVuMeter;


//        /// <summary>
//        /// Create a new player. It doesn't have an output device yet.
//        /// </summary>
//        public AudioPlayer()
//        {
//            mState = AudioPlayerState.NotReady;
//                        MoniteringTimer = new System.Windows.Forms.Timer();
//            MoniteringTimer.Tick += new System.EventHandler(this.MoniteringTimer_Tick);
//            MoniteringTimer.Interval = 200;

//            mPreviewTimer = new System.Windows.Forms.Timer(); 
//            mPreviewTimer.Tick += new System.EventHandler(this.PreviewTimer_Tick);
//            mPreviewTimer.Interval = 100;
//            //mPlaybackMode = PlaybackMode.Normal;
//            mFwdRwdRate = 0 ;
//            m_fFastPlayFactor = 1;
//            mIsFwdRwd = false;
//            mEventsEnabled = true;
//            m_lResumeToPosition = 0;
//            mBufferStopPosition = -1;
//            m_IsEndOfAsset = false;
//            m_IsPreviewing = false;
//        }


//        /// <summary>
//        /// The audio data currently playing.
//        /// </summary>
//        public AudioMediaData CurrentAudio { get { return mCurrentAudio; } }

//        /// <summary>
//        /// Currently used output device.
//        /// </summary>
//        public OutputDevice OutputDevice { get { return mDevice; } }

//        /// <summary>
//        /// Current state of the player.
//        /// </summary>
//        public AudioPlayerState State
//        {
//            get
//            {
//                if (mIsFwdRwd) return AudioPlayerState.Playing;
//                else 
//                    return mState;
//            }
//        }

//        // The player sometimes reports a bogus position before the current position while playing,
//        // so remember where we were last to avoid going backward randomly.
//        long  mPrevBytePosition;

//        // Get the current playback position in bytes.
//        private long GetCurrentBytePosition()
//        {
//            int PlayPosition = 0;
//            long lCurrentPosition = 0;
//            if (mCurrentAudio != null && mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits)  > 0)
//            {//1
//                if (mState == AudioPlayerState.Playing)
//                {//2
//                    PlayPosition = mSoundBuffer.PlayPosition;
//                    // if refreshing of buffer has finished and player is near end of asset
//                    if (mBufferStopPosition != -1)
//                    {//3
//                        int subtractor = 0;
//                        if (mBufferStopPosition >= PlayPosition)
//                            subtractor = (mBufferStopPosition - PlayPosition);
//                        else
//                            subtractor = mBufferStopPosition + (mSoundBuffer.Caps.BufferBytes - PlayPosition);

//                        lCurrentPosition = m_lLength - subtractor;
//                    }//-3
//                    else if (m_BufferCheck % 2 == 1)
//                    {//3
//                        // takes the lPlayed position and subtract the part of buffer played from it
//                        int subtractor = (2 * m_RefreshLength) - PlayPosition;
//                        lCurrentPosition = m_lPlayed - subtractor;
//                    }//-3
//                    else
//                    {//3
//                        int subtractor = (3 * m_RefreshLength) - PlayPosition;
//                        lCurrentPosition = m_lPlayed - subtractor;
//                    }//-3
//                    if (lCurrentPosition >= mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits))
//                    {//3
//                        lCurrentPosition = mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits)  -
//                            Convert.ToInt32(ObiCalculationFunctions.ConvertTimeToByte(100, mSampleRate, mFrameSize));
//                    }//-3
//                    if (mPrevBytePosition > lCurrentPosition && mFwdRwdRate >= 0) return mPrevBytePosition;
//                    mPrevBytePosition = lCurrentPosition;
//                }//-2
//                else if (mState == AudioPlayerState.Paused)
//                {//2
//                    lCurrentPosition = mPausePosition;
//                }//-2
//                if (mFwdRwdRate != 0) lCurrentPosition = m_lChunkStartPosition;
//                lCurrentPosition = ObiCalculationFunctions.AdaptToFrame(lCurrentPosition, mFrameSize);
//            }//-1
//            return lCurrentPosition;
//        }


//        /// <summary>
//        /// Set a new playback mode i.e. one of Normal, FastForward, Rewind 
//        /// </summary>
//        /// <param name="mode">The new mode.</param>
//                    private void SetPlaybackMode( int rate )
//            {
//            if (rate != mFwdRwdRate )
//            {
//                                                    if (State == AudioPlayerState.Playing)
//                {
//                                        long restartPos = GetCurrentBytePosition();
//                    StopPlayback();
//                    mState = AudioPlayerState.Paused;
//                    mFwdRwdRate = rate ;
                    
//                    InitPlay( mCurrentAudio ,  restartPos, 0);
//                }
//                else if (mState == AudioPlayerState.Paused || mState == AudioPlayerState.Stopped)
//                {
//                    mFwdRwdRate = rate  ;
//                }
//            }
//        }


///// <summary>
//        /// Stop the playback and revert to normal playback mode.
//        /// </summary>
//        private void StopPlayback()
//        {
//            StopForwardRewind();
//            mSoundBuffer.Stop();
//            if (RefreshThread != null && RefreshThread.IsAlive) RefreshThread.Abort();
//            mBufferStopPosition = -1;
//            if (ResetVuMeter != null)
//                ResetVuMeter(this, new Obi.Events.Audio.Player.UpdateVuMeterEventArgs());

//            if ( mAudioStream != null )  mAudioStream.Close();
//        }

//        /// <summary>
//        /// Convenience method for sending a state change event only if events are enabled and there is a listener.
//        /// </summary>
//        private void TriggerStateChangedEvent(Events.Audio.Player.StateChangedEventArgs e)
//        {
//            if (mEventsEnabled && StateChanged != null) StateChanged(this, e);
//        }


//        /// <summary>
//        /// Forward / Rewind rate.
//        /// 0 for normal playback
//        /// negative integer for Rewind
//        /// positive integer for FastForward
//        /// </summary>
//        public int PlaybackFwdRwdRate
//        {
//            get { return mFwdRwdRate; }
//            set { SetPlaybackMode(value); }
//        }

//        /// <summary>
//        /// Indicate if playback is previewing
//        /// <see cref=""/>
//        /// </summary>
//        public bool IsPreviewing { get { return m_IsPreviewing; } }

//        public int OutputVolume
//        {
//            get
//            {
//                return m_VolumeLevel;
//            }
//            set
//            {
//                SetVolumeLevel(value);
//            }
//        }

//        /// <summary>
//        /// <see cref=""/>
//        ///  ets and Sets the play speed with respect to normal play sppeed
//        /// </summary>
//        public float FastPlayFactor
//        {
//            get
//            {
//                return m_fFastPlayFactor;
//            }
//            set
//            {
//                                SetPlayFrequency(value);
//            }
//        }


//        public long CurrentBytePosition
//        {
//            get
//            {
//                return GetCurrentBytePosition () ;
//            }
//            set
//            {
//                SetCurrentBytePosition (value) ;
//            }
//        }

//        /// <summary>
//        /// When playing, current playback position (either playing or stopped.)
//        /// 0 when not playing.
//        /// </summary>
//        public double CurrentTimePosition
//        {
//            get
//            {
//                return mCurrentAudio == null ? 0 :
//                    ObiCalculationFunctions.ConvertByteToTime(GetCurrentBytePosition(), mSampleRate, mFrameSize);
//            }
//            set
//            {
//                SetCurrentTimePosition(value);
//            }
//        }

//        // Sets the output volume
//        void SetVolumeLevel(int VolLevel)
//        {
//            m_VolumeLevel = VolLevel;

//            if (mSoundBuffer != null)
//                mSoundBuffer.Volume = m_VolumeLevel;

//        }


		
//        private List<OutputDevice> mOutputDevicesList = null;

//        public List<OutputDevice> OutputDevices
//        {
//            get
//            {
//                if (mOutputDevicesList == null)
//                {
//                    DevicesCollection devices = new DevicesCollection();
//                    mOutputDevicesList = new List<OutputDevice>(devices.Count);
//                    foreach (DeviceInformation info in devices)
//                    {
//                        mOutputDevicesList.Add(new OutputDevice(info)); //info.Description, new Device(info.DriverGuid)
//                    }
//                }
//                return mOutputDevicesList;
//            }
//        }
        
               
        
//        /// <summary>
//        /// Set the device to be used by the player.
//        /// </summary>
//        public void SetDevice(Control handle, OutputDevice device)
//        {
//            mDevice = device;
//            mDevice.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
//            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Stopped;
//            TriggerStateChangedEvent(e);
//        }

//        /// <summary>
//        /// Set the device that matches this name; if it could not be found, default to the first one.
//        /// Throw an exception if no devices were found.
//        /// </summary>
//        public void SetDevice(Control FormHandle, string name)
//        {
//            List<OutputDevice> devices = OutputDevices;
//            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
//            if (found != null)
//            {
//                SetDevice(FormHandle, found);

//                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//                mState = AudioPlayerState.Stopped;
//                TriggerStateChangedEvent(e);
//            }
//            else if (devices.Count > 0)
//            {
//                SetDevice(FormHandle, devices[0]);
//                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//                mState = AudioPlayerState.Stopped;
//                TriggerStateChangedEvent(e);
//            }
//            else
//            {
//                mState = AudioPlayerState.NotReady;
//                throw new Exception("No output device available.");
//            }
//        }
//        /// <summary>
//        ///  Set playback frequency i.e. playback rate
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="l_frequency"></param>
//        void SetPlayFrequency(float l_frequency)
//        {
//            if (mSoundBuffer != null
//                && mFwdRwdRate == 0 )
//            {
//                try
//                {
//                    mSoundBuffer.Frequency = (int)(mSoundBuffer.Format.SamplesPerSecond * l_frequency);
//                    m_fFastPlayFactor = l_frequency;
//                }
//                catch (System.Exception Ex)
//                {
//                    MessageBox.Show("Unable to change fastplay rate " + Ex.ToString());
//                }
//            }
//            else
//                m_fFastPlayFactor = l_frequency;
//        }


//        /// <summary>
//        ///  Plays an asset from beginning to end
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="asset"></param>
//        public void Play( AudioMediaData asset)
//        {
//            // This is public function so API state will be used
//            if (State == AudioPlayerState.Stopped || State == AudioPlayerState.Paused)
//            {
//                mStartPosition = 0;

//                if (asset != null       &&   asset.AudioDuration.AsTimeSpan.TotalMilliseconds != 0)
//                    InitPlay(asset ,  0, 0);
//                else
//                    SimulateEmptyAssetPlaying();

//            }
//        }



//        /// <summary>
//        ///  Plays an asset from a specified time position its to end
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="asset"></param>
//        /// <param name="timeFrom"></param>
//        public void Play(AudioMediaData  asset, double timeFrom)
//        {
//                        // it is public function so API state will be used
//            if (State == AudioPlayerState.Stopped || State == AudioPlayerState.Paused)
//            {
//                                if (asset != null    &&     asset.AudioDuration.AsTimeSpan.TotalMilliseconds > 0)
//                {
//                    long lPosition = ObiCalculationFunctions.ConvertTimeToByte(timeFrom, (int)asset.PCMFormat.Data.SampleRate, asset.PCMFormat.Data.BlockAlign);
//                    lPosition = ObiCalculationFunctions.AdaptToFrame(lPosition, asset.PCMFormat.Data.BlockAlign);
//                    if (lPosition >= 0 && lPosition <=
//                        asset.PCMFormat.Data.ConvertTimeToBytes(asset.AudioDuration.AsLocalUnits)
//                        )
//                    {
//                                                mStartPosition = lPosition;
//                        InitPlay( asset , lPosition, 0);
//                    }
//                    else throw new Exception("Start Position is out of bounds of Audio Asset");
//                }
//                else    // if m_Asset.AudioLengthInBytes= 0 i.e. empty asset
//                {
//                    SimulateEmptyAssetPlaying();
//                }
//            }
//        }


//        /// <summary>
//        /// Play an asset from a specified time position upto another specified time position
//        /// </summary>
//        public void Play(AudioMediaData asset, double from, double to)
//        {
//            System.Diagnostics.Debug.Assert(mState == AudioPlayerState.Stopped || mState == AudioPlayerState.Paused, "Already playing?!");
//            if (asset != null && asset.AudioDuration.AsTimeSpan.TotalMilliseconds > 0)
//            {
//                long startPosition =
//                    ObiCalculationFunctions.ConvertTimeToByte(from, (int)asset.PCMFormat.Data.SampleRate, asset.PCMFormat.Data.BlockAlign);
//                startPosition = ObiCalculationFunctions.AdaptToFrame(startPosition, asset.PCMFormat.Data.BlockAlign);
//                long endPosition =
//                    ObiCalculationFunctions.ConvertTimeToByte(to, (int)asset.PCMFormat.Data.SampleRate, asset.PCMFormat.Data.BlockAlign);
//                endPosition = ObiCalculationFunctions.AdaptToFrame(endPosition, asset.PCMFormat.Data.BlockAlign);
//                if (startPosition >= 0 && startPosition < endPosition && endPosition <=
//                    asset.PCMFormat.Data.ConvertTimeToBytes(asset.AudioDuration.AsLocalUnits)
//                    )
//                {
//                    InitPlay(asset, startPosition, endPosition);
//                }
//                else
//                {
//                    throw new Exception("Start/end positions out of bounds of audio asset.");
//                }
//            }
//            else
//            {
//                SimulateEmptyAssetPlaying();
//            }
//        }

       
//        // Get a byte position from a time in ms. for the given PCM format info.
//        private long BytePositionFromTime(double time, PCMFormatInfo info)
//        {
//            ushort align = info.Data.BlockAlign;
//            return ObiCalculationFunctions.AdaptToFrame(ObiCalculationFunctions.ConvertTimeToByte(time, (int)info.Data.SampleRate, align), align);
//        }

//        /// <summary>
//        /// Starts a preview playback
//        /// playback time returns back to restore time after previewing
//        /// end of asset is not triggered after previewing
//        /// pause and stop functions work as same as that during normal playback
//        /// </summary>
//        public void PlayPreview(AudioMediaData asset, double from, double timeTo, double RestoreTime)
//        {
//            // it is public function so API state will be used
//            if (State == AudioPlayerState.Stopped || State == AudioPlayerState.Paused)
//            {
//                if (   asset != null  &&    asset.AudioDuration.AsTimeSpan.TotalMilliseconds > 0)
//                {
//                    long lStartPosition = ObiCalculationFunctions.ConvertTimeToByte(from, (int)asset.PCMFormat.Data.SampleRate, asset.PCMFormat.Data.BlockAlign);
//                    lStartPosition = ObiCalculationFunctions.AdaptToFrame(lStartPosition, asset.PCMFormat.Data.BlockAlign);
//                    long lEndPosition = ObiCalculationFunctions.ConvertTimeToByte(timeTo, (int)asset.PCMFormat.Data.SampleRate, asset.PCMFormat.Data.BlockAlign);
//                    lEndPosition = ObiCalculationFunctions.AdaptToFrame(lEndPosition, asset.PCMFormat.Data.BlockAlign);
//                    // check for valid arguments
//                    if (lStartPosition < 0) lStartPosition = 0;

//                    long pcmLength = asset.PCMFormat.Data.ConvertTimeToBytes(asset.AudioDuration.AsLocalUnits);

//                    if (lEndPosition > pcmLength)
//                        lEndPosition = pcmLength;

//                    if ( mFwdRwdRate == 0  )
//                    {
//                        m_IsPreviewing = true ;
//                        m_StateBeforePreview = State;
                            
//                        InitPlay(asset, lStartPosition, lEndPosition);

//                        if (RestoreTime >= 0 && RestoreTime < asset.AudioDuration.AsTimeSpan.TotalMilliseconds)
//                            m_PreviewStartPosition = ObiCalculationFunctions.ConvertTimeToByte(RestoreTime, (int)mSampleRate, mFrameSize);
//                        else
//                            m_PreviewStartPosition = 0;

//                            mEventsEnabled = false;

//                    }
//                    else
//                        throw new Exception("Start Position is out of bounds of Audio Asset");
//                }
//                else
//                {
//                    SimulateEmptyAssetPlaying();
//                }
//            }
//        }

//        /// <summary>
//        ///  convenience function to start playback of an asset
//        ///  first initialise player with asset followed by starting playback using PlayAssetStream function
//        /// </summary>
//        /// <param name="asset"></param>
//        /// <param name="lStartPosition"></param>
//        /// <param name="lEndPosition"></param>
//        private void InitPlay(AudioMediaData asset, long lStartPosition, long lEndPosition)
//        {
//            if (mState != AudioPlayerState.Playing)
//            {
//                InitialiseWithAsset(asset);
//                if (mFwdRwdRate == 0)
//                {
//                    PlayAssetStream(lStartPosition, lEndPosition);
//                }
//                else if (mFwdRwdRate > 0)
//                {
//                    FastForward(lStartPosition);
//                }
//                else if (mFwdRwdRate < 0)
//                {
//                    if (lStartPosition == 0) lStartPosition = mAudioStream.Length;
//                    Rewind(lStartPosition);
//                }
//            }// end of state check
//            // end of function
//        }

//        private AudioPlayerState  m_StateBeforePreview ;
//        private long m_PreviewStartPosition ;

        
//        /// <summary>
//        ///  Called when a new asset is passed to player for playback 
//        ///  initialises all asset dependent members excluding stream dependent members
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="audio"></param>
//        private void InitialiseWithAsset(AudioMediaData audio)
//        {
//            if (mState != AudioPlayerState.Playing)
//            {
//                mCurrentAudio = audio;
//                WaveFormat newFormat = new WaveFormat();
//                 BufferDescription  BufferDesc = new BufferDescription();
                
//                // retrieve format from asset
//                mFrameSize = mCurrentAudio.PCMFormat.Data.BlockAlign;
//                m_Channels = mCurrentAudio.PCMFormat.Data.NumberOfChannels ;
//                newFormat.AverageBytesPerSecond = (int)mCurrentAudio.PCMFormat.Data.SampleRate* mCurrentAudio.PCMFormat.Data.BlockAlign;
//                                newFormat.BitsPerSample = Convert.ToInt16(mCurrentAudio.PCMFormat.Data.BitDepth);
//                newFormat.BlockAlign = Convert.ToInt16(mCurrentAudio.PCMFormat.Data.BlockAlign);
//                newFormat.Channels = Convert.ToInt16(mCurrentAudio.PCMFormat.Data.NumberOfChannels);

//                newFormat.FormatTag = WaveFormatTag.Pcm;

//                newFormat.SamplesPerSecond = (int)mCurrentAudio.PCMFormat.Data.SampleRate;

//                // loads  format to buffer description
//                BufferDesc.Format = newFormat;

//                // enable buffer description properties
//                BufferDesc.ControlVolume = true;
//                BufferDesc.ControlFrequency = true;

//                // calculate size of buffer so as to contain 1 second of audio
//                m_SizeBuffer = (int)mCurrentAudio.PCMFormat.Data.SampleRate * mCurrentAudio.PCMFormat.Data.BlockAlign;
//                m_RefreshLength = (int)(mCurrentAudio.PCMFormat.Data.SampleRate/ 2) * mCurrentAudio.PCMFormat.Data.BlockAlign;

//                // calculate the size of VuMeter Update array length
//                m_UpdateVMArrayLength = m_SizeBuffer / 20;
//                m_UpdateVMArrayLength = Convert.ToInt32(ObiCalculationFunctions.AdaptToFrame(Convert.ToInt32(m_UpdateVMArrayLength), mFrameSize));
//                arUpdateVM = new byte[m_UpdateVMArrayLength];
//                // reset the VuMeter (if set)
//                if (ResetVuMeter != null)
//                    ResetVuMeter ( this , new Obi.Events.Audio.Player.UpdateVuMeterEventArgs () ) ;

//                // sets the calculated size of buffer
//                BufferDesc.BufferBytes = m_SizeBuffer;

//                // Global focus is set to true so that the sound can be played in background also
//                BufferDesc.GlobalFocus = true;

//                // initialising secondary buffer
//                // m_SoundBuffer = new SecondaryBuffer(BufferDesc, SndDevice);
//                mSoundBuffer = new SecondaryBuffer(BufferDesc, mDevice.Device);

//                SetPlayFrequency(m_fFastPlayFactor);

//            }// end of state check
//                            } // end function

//        /// <summary>
//        ///  Called to start playback when player is already initialised with an asset
//        ///  Initialises all member variables dependent on asset stream and fill play buffers with data
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="lStartPosition"></param>
//        /// <param name="lEndPosition"></param>
//        private void PlayAssetStream(long lStartPosition, long lEndPosition)
//        {
//            if (mState != AudioPlayerState.Playing)
//            {
//                // Adjust the start and end position according to frame size
//                lStartPosition = ObiCalculationFunctions.AdaptToFrame(lStartPosition, mCurrentAudio.PCMFormat.Data.BlockAlign);
//                lEndPosition = ObiCalculationFunctions.AdaptToFrame(lEndPosition, mCurrentAudio.PCMFormat.Data.BlockAlign);
//                mSampleRate = (int)mCurrentAudio.PCMFormat.Data.SampleRate;
                
//                // lEndPosition = 0 means that file is played to end
//                if (lEndPosition != 0)
//                {
//                    m_lLength = (lEndPosition); // -lStartPosition;
//                }
//                else
//                {
//                    // folowing one line is modified on 2 Aug 2006
//                    //m_lLength = (m_Asset .SizeInBytes  - lStartPosition ) ;
//                    m_lLength = (mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits));
//                }

//                mPrevBytePosition = lStartPosition;
//                // initialize M_lPlayed for this asset
//                m_lPlayed = lStartPosition;

//                m_IsEndOfAsset = false;
                
//                mAudioStream = mCurrentAudio.OpenPcmInputStream();
//                mAudioStream.Position = lStartPosition;
                
//                mSoundBuffer.Write(0, mAudioStream, m_SizeBuffer, 0);

//                // Adds the length (count) of file played into a variable
//                m_lPlayed += m_SizeBuffer;

//                // trigger  events (modified JQ)
//                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//                mState = AudioPlayerState.Playing;
//                TriggerStateChangedEvent(e);

//                MoniteringTimer.Enabled = true;
//                // starts playing
//                try
//                    {
//                    mSoundBuffer.Play ( 0, BufferPlayFlags.Looping );
//                    }
//                catch (System.Exception ex)
//                    {
//                    EmergencyStopForSoundBufferProblem ();
//                    System.Windows.Forms.MessageBox.Show ( string.Format( Localizer.Message ( "Player_TryAgain" ) , "\n\n" , ex.ToString ()) ) ;
//                    return;
//                    }
//                m_BufferCheck = 1;

//                //initialise and start thread for refreshing buffer
//                RefreshThread = new Thread(new ThreadStart(RefreshBuffer));
//                RefreshThread.Start();


//            }
//        } // function ends

//        /// <summary>
//        ///  Thread function which is responsible for refreshing half of sound buffer after every 0.5 second and also for stopping play at end of asset
//        /// <see cref=""/>
//        /// </summary>
//        private void RefreshBuffer()
//        {

//            int ReadPosition;

//            // variable to prevent least count errors in clip end time
//            long SafeMargin = ObiCalculationFunctions.ConvertTimeToByte(1, mSampleRate, mFrameSize);


//            while (m_lPlayed < (m_lLength - SafeMargin))
//            {//1
//                if (mSoundBuffer.Status.BufferLost)
//                    mSoundBuffer.Restore();


//                Thread.Sleep(50);

//                if (UpdateVuMeter != null)
//                {
//                    ReadPosition = mSoundBuffer.PlayPosition;

//                    if (ReadPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
//                    {
//                        Array.Copy(mSoundBuffer.Read(ReadPosition, typeof(byte), LockFlag.None, m_UpdateVMArrayLength), arUpdateVM, m_UpdateVMArrayLength);
//                        if (mEventsEnabled == true)
//                            UpdateVuMeter(this, new Events.Audio.Player.UpdateVuMeterEventArgs());  // JQ // temp for debugging tk
//                    }
//                }
//                // check if play cursor is in second half , then refresh first half else second
//                // refresh front part for odd count
//                if ((m_BufferCheck % 2) == 1 && mSoundBuffer.PlayPosition > m_RefreshLength)
//                {//2
//                    mSoundBuffer.Write(0, mAudioStream, m_RefreshLength, 0);
//                    m_lPlayed = m_lPlayed + m_RefreshLength;
//                    m_BufferCheck++;
//                }//-1
//                // refresh Rear half of buffer for even count
//                else if ((m_BufferCheck % 2 == 0) && mSoundBuffer.PlayPosition < m_RefreshLength)
//                {//1
//                    mSoundBuffer.Write(m_RefreshLength, mAudioStream, m_RefreshLength, 0);
//                    m_lPlayed = m_lPlayed + m_RefreshLength;
//                    m_BufferCheck++;
//                    // end of even/ odd part of buffer;
//                }//-1

//                // end of while
//            }

//            m_IsEndOfAsset = false;
//            int LengthDifference = (int)(m_lPlayed - m_lLength);
//            mBufferStopPosition = -1;
//            // if there is no refresh after first load thenrefresh maps directly  
//            if (m_BufferCheck == 1)
//            {
//                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
//            }

//            // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
//            else if ((m_BufferCheck % 2) == 0)
//            {
//                mBufferStopPosition = Convert.ToInt32(m_RefreshLength - LengthDifference);
//            }
//            else if ((m_BufferCheck > 1) && (m_BufferCheck % 2) == 1)
//            {
//                mBufferStopPosition = Convert.ToInt32(m_SizeBuffer - LengthDifference);
//            }

//            int CurrentPlayPosition;
//            CurrentPlayPosition = mSoundBuffer.PlayPosition;
//            int StopMargin = Convert.ToInt32(ObiCalculationFunctions.ConvertTimeToByte(70, mSampleRate, mFrameSize));
//            StopMargin = (int)(StopMargin * m_fFastPlayFactor);

//            if (mBufferStopPosition < StopMargin)
//                mBufferStopPosition = StopMargin;

//            while (CurrentPlayPosition < (mBufferStopPosition - StopMargin) || CurrentPlayPosition > (mBufferStopPosition))
//            {
//                Thread.Sleep(50);
//                CurrentPlayPosition = mSoundBuffer.PlayPosition;

//                if (UpdateVuMeter != null)
//                    {
//                    // trigger VuMeter events in this trailing part. Need cleanup, should be placed in another function to avoid duplicacy. But first it should work.
//                    if (CurrentPlayPosition < ((m_SizeBuffer) - m_UpdateVMArrayLength))
//                        {
//                        Array.Copy ( mSoundBuffer.Read ( CurrentPlayPosition, typeof ( byte ), LockFlag.None, m_UpdateVMArrayLength ), arUpdateVM, m_UpdateVMArrayLength );
//                        if (mEventsEnabled == true && UpdateVuMeter != null)
//                            UpdateVuMeter ( this, new Events.Audio.Player.UpdateVuMeterEventArgs () );  
//                        }
//                    }

//            }


//            // Stopping process begins
//            mBufferStopPosition = -1;
//            mPausePosition = 0;
//            mSoundBuffer.Stop();
//            if (ResetVuMeter != null)
//                ResetVuMeter(this, new Obi.Events.Audio.Player.UpdateVuMeterEventArgs());

//            if(mAudioStream != null ) mAudioStream.Close();

//            // changes the state and trigger events
//            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Stopped;

//            TriggerStateChangedEvent(e);

//            if (mEventsEnabled)
//                m_IsEventEnabledDelayedTillTimer = true;
//            else
//                m_IsEventEnabledDelayedTillTimer = false;

//            m_IsEndOfAsset = true;

//            PreviewPlaybackStop();
//            // RefreshBuffer ends
//        }
//        private bool m_IsEventEnabledDelayedTillTimer= true ;

//        private void PreviewPlaybackStop()
//        {
//                        if (m_IsPreviewing)
//            {
//                m_IsPreviewing = false;
//                m_IsEndOfAsset = false;
//                mEventsEnabled = true;
                
//                if (m_StateBeforePreview == AudioPlayerState.Paused)
//                {
//                    Events.Audio.Player.StateChangedEventArgs e = new Obi.Events.Audio.Player.StateChangedEventArgs( AudioPlayerState.Playing );
//                    mState = AudioPlayerState.Paused;
//                    mPausePosition = m_PreviewStartPosition;
//                    TriggerStateChangedEvent(e);
//                                    }
//                else if (m_StateBeforePreview == AudioPlayerState.Stopped )
//                {
//                    Events.Audio.Player.StateChangedEventArgs e = new Obi.Events.Audio.Player.StateChangedEventArgs(AudioPlayerState.Playing);
//                                                            TriggerStateChangedEvent(e);
//                }
//            }
//        }


//        ///<summary>
//        /// Function for simulating playing for assets with no audio
//        /// </summary>
//        ///
//        private void SimulateEmptyAssetPlaying()
//        {
//            if (mCurrentAudio != null)
//            {
//                m_Channels = mCurrentAudio.PCMFormat.Data.NumberOfChannels;
//                mFrameSize = mCurrentAudio.PCMFormat.Data.NumberOfChannels* (mCurrentAudio.PCMFormat.Data.BitDepth/ 8);
//                mSampleRate = (int)mCurrentAudio.PCMFormat.Data.SampleRate;
//            } 

//            Events.Audio.Player.StateChangedEventArgs  e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Playing;
//            TriggerStateChangedEvent(e);
            

//            Thread.Sleep(50);

//            e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Stopped;
//            TriggerStateChangedEvent(e);

//            // trigger end of asset event
//            if (mEventsEnabled == true
//                && EndOfAudioAsset != null)
//                {
//                EndOfAudioAsset ( this, new Events.Audio.Player.EndOfAudioAssetEventArgs () );
//                }
////            System.Media.SystemSounds.Asterisk.Play();
//        }

//        /// <summary>
//        /// Pause from stopped state, in order to reset the pause position after preview.
//        /// </summary>
//        public void PauseFromStopped(double time)
//        {
//            if (State == AudioPlayerState.Stopped)
//            {
//                m_lResumeToPosition = 0;
//                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//                mState = AudioPlayerState.Paused;
//                SetCurrentTimePosition(time);
//                TriggerStateChangedEvent(e);
//            }
//        }

//        /// <summary>
//        ///  Pauses playing asset. 
//        /// Resumes from paused position with resume command or starts from begining/specified start position with play command.
//        /// /<see cref=""/>
//        /// </summary>
//        public void Pause()
//        {
//            // API state is used
//            if ( State.Equals(AudioPlayerState .Playing)  )
//            {
//                if (m_IsPreviewing)
//                    m_IsPreviewing = false;

//                    mPausePosition = GetCurrentBytePosition();
//                    if (!mIsFwdRwd)
//                        m_lResumeToPosition = m_lLength;
//                    else
//                        m_lResumeToPosition = 0;

//                StopPlayback();

//                // Change the state and trigger event
//                Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState) ;
//                mState = AudioPlayerState.Paused;
//                TriggerStateChangedEvent(e);
                
//            }
//        }

//        /// <summary>
//        ///  Resumes from paused position if player is in paused state
//        /// <see cref=""/>
//        /// </summary>
//        public void Resume()
//        {
//            // API state will be used for public functions
//                        if ( State.Equals(AudioPlayerState.Paused))
//            {
                
//                long lPosition = ObiCalculationFunctions.AdaptToFrame( mPausePosition , mCurrentAudio.PCMFormat.Data.BlockAlign);
//                long lEndPosition = ObiCalculationFunctions.AdaptToFrame( m_lResumeToPosition , mCurrentAudio.PCMFormat.Data.BlockAlign);

//                if (lPosition >= 0 && lPosition < mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits))
//                {
//                    mStartPosition = lPosition;
//                                        InitPlay( mCurrentAudio , lPosition, lEndPosition );
//                }
//                else
//                    throw new Exception("Start Position is out of bounds of Audio Asset");
//            }			
//        }

//        public void Stop()
//        {
//            // API state is used
//            if ( State != AudioPlayerState.Stopped )			
//            {
//                if (m_IsPreviewing)
//                    m_IsPreviewing = false;

//                                StopPlayback();
//            }
            
//        mPausePosition = 0;
//            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Stopped;
//            TriggerStateChangedEvent(e);
//        }


//        private void EmergencyStopForSoundBufferProblem ()
//            {
//            if (m_IsPreviewing)
//                    m_IsPreviewing = false;
//            ////
//                StopForwardRewind ();
//                mSoundBuffer.Stop ();
//                if (RefreshThread != null && RefreshThread.IsAlive) RefreshThread.Abort ();
//                mBufferStopPosition = -1;
//                if (ResetVuMeter != null)
//                    ResetVuMeter ( this, new Obi.Events.Audio.Player.UpdateVuMeterEventArgs () );

//                if(mAudioStream != null)  mAudioStream.Close ();
//        ////
			
//        mPausePosition = 0;
//            Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
//            mState = AudioPlayerState.Stopped;
//            TriggerStateChangedEvent(e);
	
//            }



//        long mStartPosition;

//        // Set the current position in the player in bytes.
//        void SetCurrentBytePosition(long position)
//        {
//        if (mState != AudioPlayerState.Stopped || mCurrentAudio != null)
//            {
//            if (position < 0) position = 0;
//            if (position > mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits))
//                position = mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits)  - 100;
//            mEventsEnabled = false;
//            if (State == AudioPlayerState.Playing)
//                {
//                Stop ();
//                Thread.Sleep ( 30 );
//                mStartPosition = position;
//                InitPlay ( mCurrentAudio, position, 0 );
//                }
//            else if (mState.Equals ( AudioPlayerState.Paused ))
//                {
//                mStartPosition = position;
//                mPausePosition = position;
//                }
//            mEventsEnabled = true;
//            }
//        }


//        // Set the current time position in milliseconds
//        private void SetCurrentTimePosition(double timeMs) 
//        {
//            SetCurrentBytePosition(ObiCalculationFunctions.ConvertTimeToByte(timeMs, mSampleRate, mFrameSize));
//        }


//        //  FastForward , Rewind playback modes
//        /// <summary>
//        ///  Starts playing small chunks of audio while jumping backward in audio asset
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="lStartPosition"></param>
//        private  void Rewind( long lStartPosition  )
//        {
//                                    // let's play backward!
//            if ( mFwdRwdRate !=  0 )
//            {
//                                m_lChunkStartPosition = lStartPosition ;
//                                                mEventsEnabled = false;
//                                mIsFwdRwd = true;
//                                mPreviewTimer.Interval = 50;
//                mPreviewTimer.Start();
                
//                            }
//        }
        

//        /// <summary>
//        ///  Starts playing small chunks while jumping forward in audio asset
//        /// <see cref=""/>
//        /// </summary>
//        /// <param name="lStartPosition"></param>
//        private void FastForward(long lStartPosition   )
//        {

//            // let's play forward!
//            if (mFwdRwdRate !=  0  )
//            {
//                m_lChunkStartPosition = lStartPosition;
//                mEventsEnabled = false;
//                mIsFwdRwd = true;
//                mPreviewTimer.Interval = 50;
//                mPreviewTimer.Start();
//            }
//        }

        

//        ///Preview timer tick function
//        private void PreviewTimer_Tick(object sender, EventArgs e)
//        { //1
            
//            double StepInMs = Math.Abs( 4000 * mFwdRwdRate   ) ;
//            long lStepInBytes = ObiCalculationFunctions.ConvertTimeToByte(StepInMs, (int)  mCurrentAudio.PCMFormat.Data.SampleRate, mCurrentAudio.PCMFormat.Data.BlockAlign);
//            int PlayChunkLength = 1200;
//            long lPlayChunkLength = ObiCalculationFunctions.ConvertTimeToByte( PlayChunkLength , (int)mCurrentAudio.PCMFormat.Data.SampleRate, mCurrentAudio.PCMFormat.Data.BlockAlign);
//            mPreviewTimer.Interval = PlayChunkLength + 50;

//            long PlayStartPos = 0;
//            long PlayEndPos = 0;
//            if ( mFwdRwdRate > 0 )
//            { //2

//                long pcmLength = mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits);

//                if ((pcmLength - (lStepInBytes + m_lChunkStartPosition)) > lPlayChunkLength)
//                { //3
//                    if (m_lChunkStartPosition > 0)
//                    {
//                        m_lChunkStartPosition += lStepInBytes;
//                    }
//                    else
//                        m_lChunkStartPosition = mFrameSize;

//                    PlayStartPos = m_lChunkStartPosition;
//PlayEndPos  = m_lChunkStartPosition + lPlayChunkLength  ;
//PlayAssetStream  (  PlayStartPos, PlayEndPos);

//if (m_lChunkStartPosition > mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits))
//    m_lChunkStartPosition = mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits);
//                                    } //-3
//                else
//                { //3
//                    Stop();
//                    if (mEventsEnabled
//                        && EndOfAudioAsset != null)
//                        {
//                        EndOfAudioAsset ( this, new Events.Audio.Player.EndOfAudioAssetEventArgs () );
//                        }

//                                } //-3
//            } //-2
//            else if ( mFwdRwdRate <  0 )
//            { //2
//                //if (m_lChunkStartPosition > (lStepInBytes ) && lPlayChunkLength <= m_Asset.getPCMLength () )
//                if (m_lChunkStartPosition >  0 )
//                { //3
//                    if (m_lChunkStartPosition < mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits))
//                        m_lChunkStartPosition -= lStepInBytes;
//                    else
//                        m_lChunkStartPosition = mCurrentAudio.PCMFormat.Data.ConvertTimeToBytes(mCurrentAudio.AudioDuration.AsLocalUnits)  - lPlayChunkLength;

//                    PlayStartPos = m_lChunkStartPosition ;
//                    PlayEndPos = m_lChunkStartPosition +  lPlayChunkLength;
//                    PlayAssetStream (PlayStartPos, PlayEndPos);
                    
//                    if (m_lChunkStartPosition < 0)
//                        m_lChunkStartPosition = 0;
//                } //-3
//                else
//                {
//                    Stop();
//                    if (mEventsEnabled
//                        && EndOfAudioAsset != null)
//                        {
//                        EndOfAudioAsset ( this, new Events.Audio.Player.EndOfAudioAssetEventArgs () );
//                        }
//                                    }
//                } //-2
//                            } //-1


//                            /// <summary>
//                            /// Stop rewinding or forwarding, including the preview timer.
//                            /// </summary>
//                            private void StopForwardRewind()
//                            {
//                                if (mFwdRwdRate != 0 || mPreviewTimer.Enabled)
//                                {
//                                    mPreviewTimer.Enabled = false;
//                                                    //m_FwdRwdRate = 0 ;
//                                    m_lChunkStartPosition = 0;
//                                    mIsFwdRwd = false;
//                                    mEventsEnabled = true;
//                                }
//                            }



//        private void MoniteringTimer_Tick(object sender, EventArgs e)
//        {
//            if ( m_IsEndOfAsset == true)
//            {
//                m_IsEndOfAsset = false;
//                MoniteringTimer.Enabled = false;

//                if (m_IsEventEnabledDelayedTillTimer)
//                {
//                    if (EndOfAudioAsset != null)
//                        EndOfAudioAsset(this, new Events.Audio.Player.EndOfAudioAssetEventArgs());
                                            
//                                    }
//            if (mEventsEnabled == true)
//                m_IsEventEnabledDelayedTillTimer= true;
//            else
//                m_IsEventEnabledDelayedTillTimer= false;
//            }

//        }


//    }
}
