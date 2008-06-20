using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;

namespace Bobi.Audio
{
    public enum PlayerState { Stopped, Playing, Paused };

	public class Player
	{
        private Device device;                                  // output device
        private PlayerState state;                              // player state

        private urakawa.media.data.audio.AudioMediaData audio;  // the audio currently playing
        private Stream audioStream;                             // audio stream
        private SecondaryBuffer soundBuffer;                    // DX playback buffer
        private int bufferSize;                                 // Size of buffer created for playing
        private int refreshLength;                              // length of buffer to be refreshed during playing which is half of buffer size
        private Thread refreshThread;                           // thread for refreshing buffer while playing 
        private int frameSize;
        private int channels;
        private int sampleRate;

        private int startPosition;                             // start position before playback starts?
        private int bufferStopPosition;                         // used by refresh thread for stop position in buffer, value is negative till refreshing of buffer is going on
        private int pausePosition;                              // position where playback is paused

        private int length;
        private int prevBytePosition;
        private bool isEndOfAsset;
        private int bufferCheck;
        private int played;



        public Player(Control handle)
        {
            SetDefaultOutputDevice(handle);
            this.state = PlayerState.Stopped;
        }


        /// <summary>
        /// The audio data currently playing.
        /// </summary>
        public urakawa.media.data.audio.AudioMediaData Audio { get { return this.audio; } }

        
        public int CurrentBytePosition
        {
            get { return GetCurrentBytePosition(); }
            set { SetCurrentBytePosition(value); }
        }

        public double CurrentTimePosition
        {
            get { return GetCurrentTimePosition(); }
            set { SetCurrentTimePosition(value); }
        }

        /// <summary>
        /// Pauses playing asset. 
        /// Resumes from paused position with resume command or starts from begining/specified start position with play command.
        /// </summary>
        public void Pause()
        {
            if (this.state == PlayerState.Playing)
            {
                this.pausePosition = GetCurrentBytePosition();
                StopPlayback();
                this.state = PlayerState.Paused;
            }
        }

        /// <summary>
        /// Play an asset from beginning to end
        /// </summary>
        public void Play(urakawa.media.data.audio.AudioMediaData audio)
        {
            if (this.state == PlayerState.Stopped || this.state == PlayerState.Paused)
            {
                this.startPosition = 0;
                InitPlay(audio, 0, 0);
            }
        }

        /// <summary>
        /// Play an asset from a specified time position its to end
        /// </summary>
        public void Play(urakawa.media.data.audio.AudioMediaData audio, double from)
        {
            if (this.state == PlayerState.Stopped || this.state == PlayerState.Paused)
            {
                if (audio != null && audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() > 0)
                {
                    urakawa.media.data.audio.PCMFormatInfo info = audio.getPCMFormat();
                    int position = CalculationFunctions.AdaptToFrame(
                        CalculationFunctions.ConvertTimeToByte(from, (int)info.getSampleRate(), info.getBlockAlign()),
                        info.getBlockAlign());
                    if (position >= 0 && position <= audio.getPCMLength())
                    {
                        this.startPosition = position;
                        InitPlay(audio, position, 0);
                    }
                    else
                    {
                        throw new Exception("Start Position is out of bounds of Audio Asset");
                    }
                }
            }
        }

        /// <summary>
        /// Play audio from a specified time position up to another specified time position
        /// </summary>
        public void Play(urakawa.media.data.audio.AudioMediaData audio, double from, double to)
        {
            if (this.state == PlayerState.Stopped || this.state == PlayerState.Paused)
            {
                if (audio != null && audio.getAudioDuration().getTimeDeltaAsMillisecondFloat() > 0)
                {
                    urakawa.media.data.audio.PCMFormatInfo info = audio.getPCMFormat();
                    int startPosition = CalculationFunctions.AdaptToFrame(
                        CalculationFunctions.ConvertTimeToByte(from, (int)info.getSampleRate(), info.getBlockAlign()),
                        info.getBlockAlign());
                    int endPosition = CalculationFunctions.AdaptToFrame(
                        CalculationFunctions.ConvertTimeToByte(to, (int)info.getSampleRate(), info.getBlockAlign()),
                        info.getBlockAlign());
                    if (startPosition >= 0 && startPosition < endPosition && endPosition <= audio.getPCMLength())
                    {
                        InitPlay(audio, startPosition, endPosition);
                    }
                    else
                    {
                        throw new Exception("Start/end positions out of bounds of audio asset.");
                    }
                }
            }
        }

        /// <summary>
        /// Resume from paused position if player is in paused state
        /// </summary>
        public void Resume()
        {
            if (this.state == PlayerState.Paused)
            {
                int from = CalculationFunctions.AdaptToFrame(this.pausePosition, (int)this.audio.getPCMFormat().getBlockAlign());
                if (from >= 0 && from < this.audio.getPCMLength())
                {
                    this.startPosition = from;
                    InitPlay(this.audio, from, 0);
                }
                else
                {
                    throw new Exception("Start Position is out of bounds of Audio Asset");
                }
            }
        }

        /// <summary>
        /// Current state of the player.
        /// </summary>
        public PlayerState State { get { return this.state; } }

        /// <summary>
        /// Stop playback.
        /// </summary>
        public void Stop()
        {
            if (this.state != PlayerState.Stopped)
            {
                StopPlayback();
                // Events.Audio.Player.StateChangedEventArgs e = new Events.Audio.Player.StateChangedEventArgs(mState);
                this.state = PlayerState.Stopped;
            }
        }


        /// Current playback position in bytes.
        private int GetCurrentBytePosition()
        {
            int playPosition = 0;
            int currentPosition = 0;
            if (this.audio != null && this.audio.getPCMLength() > 0)
            {
                if (this.state == PlayerState.Playing)
                {
                    playPosition = this.soundBuffer.PlayPosition;
                    // if refreshing of buffer has finished and player is near end of asset
                    if (this.bufferStopPosition != -1)
                    {
                        int subtractor = this.bufferStopPosition >= playPosition ? this.bufferStopPosition - playPosition :
                            this.bufferStopPosition + this.soundBuffer.Caps.BufferBytes - playPosition;
                        currentPosition = this.audio.getPCMLength() - subtractor;
                    }
                    else if (m_BufferCheck % 2 == 1)
                    {   // takes the lPlayed position and subtract the part of buffer played from it
                        int subtractor = (2 * this.refreshLength) - playPosition;
                        currentPosition = this.played - subtractor;
                    }
                    else
                    {
                        int subtractor = (3 * this.refreshLength) - playPosition;
                        currentPosition = this.played - subtractor;
                    }
                    if (currentPosition >= this.audio.getPCMLength())
                    {
                        currentPosition = this.audio.getPCMLength() -
                            Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(100, this.sampleRate, this.frameSize));
                    }
                }
                else if (this.state == PlayerState.Paused)
                {
                    currentPosition = this.pausePosition;
                }
                currentPosition = CalculationFunctions.AdaptToFrame(currentPosition, this.frameSize);
            }
            //if (this.prevBytePosition > currentPosition && mFwdRwdRate >= 0)
            //    return this.prevBytePosition;

            this.prevBytePosition = currentPosition;
            return currentPosition;
        }

        // Current position of play cursor to be used by CurrentTimePosition property
        private double GetCurrentTimePosition()
        {
            return this.audio != null ?
                CalculationFunctions.ConvertByteToTime(GetCurrentBytePosition(), this.sampleRate, this.frameSize) : 0;
        }

        // Called when new audio is passed to player for playback.
        // Initializes all asset dependent members excluding stream dependent members
        private void InitAudio(urakawa.media.data.audio.AudioMediaData audio)
        {
            this.audio = audio;
            WaveFormat format = new WaveFormat();
            BufferDescription desc = new BufferDescription();
            urakawa.media.data.audio.PCMFormatInfo info = audio.getPCMFormat();
            this.frameSize = info.getBlockAlign();
            this.channels = info.getNumberOfChannels();
            this.sampleRate = (int)info.getSampleRate();
            format.AverageBytesPerSecond = (int)info.getSampleRate() * info.getBlockAlign();
            format.BitsPerSample = Convert.ToInt16(info.getBitDepth());
            format.BlockAlign = Convert.ToInt16(info.getBlockAlign());
            format.Channels = Convert.ToInt16(info.getNumberOfChannels());
            format.FormatTag = WaveFormatTag.Pcm;
            format.SamplesPerSecond = (int)info.getSampleRate();
            desc.Format = format;
            this.bufferSize = format.AverageBytesPerSecond;
            this.refreshLength = this.bufferSize / 2;
            desc.BufferBytes = this.bufferSize;
            desc.GlobalFocus = true;
            this.soundBuffer = new SecondaryBuffer(desc, this.device);
            this.pausePosition = 0;
        }

        // Convenience function to start playback of an asset
        // First initialise player with asset followed by starting playback using PlayAssetStream function
        private void InitPlay(urakawa.media.data.audio.AudioMediaData audio, int from, int to)
        {
            InitAudio(audio);
            PlayAssetStream(from, to);
        }

        // Called to start playback when player is already initialised with an asset
        // Initialises all member variables dependent on asset stream and fill play buffers with data
        private void PlayAssetStream(int from, int to)
        {
            urakawa.media.data.audio.PCMFormatInfo info = this.audio.getPCMFormat();
            from = CalculationFunctions.AdaptToFrame(from, info.getBlockAlign());
            to = CalculationFunctions.AdaptToFrame(to, info.getBlockAlign());
            this.length = to == 0 ? this.audio.getPCMLength() : to;
            this.prevBytePosition = from;
            this.isEndOfAsset = false;
            this.audioStream = this.audio.getAudioData();
            this.audioStream.Position = from;
            this.soundBuffer.Write(0, this.audioStream, this.bufferSize, 0);
            this.played = from + this.bufferSize;
            // trigger events (modified JQ)
            this.state = PlayerState.Playing;
            this.soundBuffer.Play(0, BufferPlayFlags.Looping);
            this.bufferCheck = 1;
            this.refreshThread = new Thread(new ThreadStart(RefreshBuffer));
            this.refreshThread.Start();
        }

        void SetCurrentBytePosition(int position)
        {
            if (position < 0) position = 0;
            if (position > this.audio.getPCMLength()) position = this.audio.getPCMLength() - 100;
            if (this.state == PlayerState.Playing)
            {
                Stop();
                Thread.Sleep(30);
                this.startPosition = position;
                InitPlay(this.audio, position, 0);
            }
            else if (this.state == PlayerState.Paused)
            {
                this.startPosition = position;
                this.pausePosition = position;
            }
        }

        // Set the current time position
        private void SetCurrentTimePosition(double position)
        {
            SetCurrentBytePosition(CalculationFunctions.ConvertTimeToByte(position, this.sampleRate, this.frameSize));
        }

        // Set the output device to the first one found.
        private void SetDefaultOutputDevice(Control handle)
        {
            DevicesCollection devices = new DevicesCollection();
            if (devices.Count == 0) throw new Exception("No output device found!");
            this.device = new Device(devices[0].DriverGuid);
            this.device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
        }

        /// <summary>
        /// Stop the playback and revert to normal playback mode.
        /// </summary>
        private void StopPlayback()
        {
            this.soundBuffer.Stop();
            if (this.refreshThread != null && this.refreshThread.IsAlive) this.refreshThread.Abort();
            this.bufferStopPosition = -1;
            this.audioStream.Close();
        }






        #region private members






        // Member variables changed more than ones ( in one asset session ) by functions in AudioPlayer class
        private bool mIsFwdRwd;                // flag indicating forward or rewind playback is going on
        private int m_BufferCheck; // integer to indicate which part of buffer is to be refreshed front or rear, value is odd for refreshing front part and even for refreshing rear
        private long m_lPlayed;         // Length of audio asset in bytes which had been played ( loadded to SoundBuffer )
        private long m_lPausePosition; // holds pause position in bytes to allow play resume playback from there
        private long m_lResumeToPosition; // In case play ( from, to ) function is used, holds the end position i.e. "to"  for resuming playback
        private bool m_IsEndOfAsset; // variable required to signal monitoring timer to trigger end of asset event, flag is set for a moment and again reset


        #endregion











       


        /// <summary>
        ///  Thread function which is responsible for refreshing half of sound buffer after every 0.5 second and also for stopping play at end of asset
        /// <see cref=""/>
        /// </summary>
        private void RefreshBuffer()
        {
            int read;
            double margin = CalculationFunctions.ConvertByteToTime(1, this.sampleRate, this.frameSize);
            while (this.played < (this.length - margin))
            {
                if (this.soundBuffer.Status.BufferLost) this.soundBuffer.Restore();
                Thread.Sleep(50);
                // check if play cursor is in second half , then refresh first half else second
                // refresh front part for odd count
                if ((this.bufferCheck % 2) == 1 && this.soundBuffer.PlayPosition > this.refreshLength)
                {
                    this.soundBuffer.Write(0, this.audioStream, this.refreshLength, 0);
                    this.played += this.refreshLength;
                    this.bufferCheck++;
                }
                else if ((this.bufferCheck % 2 == 0) && this.soundBuffer.PlayPosition < this.refreshLength)
                {
                    this.soundBuffer.Write(this.refreshLength, this.audioStream, this.refreshLength, 0);
                    this.played += this.refreshLength;
                    this.bufferCheck++;
                }
            }
            this.isEndOfAsset = false;
            int diff = this.played - this.length;
            this.bufferStopPosition = -1;
            if (this.bufferCheck == 1)
            {
                this.bufferStopPosition = Convert.ToInt32(this.bufferSize - diff);
            }
            else if ((m_BufferCheck % 2) == 0)
            {
                // if last refresh is to Front, BufferCheck is even and stop position is at front of buffer.
                this.bufferStopPosition = Convert.ToInt32(this.refreshLength - diff);
            }
            else if ((m_BufferCheck % 2) == 1)
            {
                this.bufferStopPosition = Convert.ToInt32(this.bufferSize - diff);
            }

            int CurrentPlayPosition;
            CurrentPlayPosition = this.soundBuffer.PlayPosition;
            int StopMargin = Convert.ToInt32(CalculationFunctions.ConvertTimeToByte(70, this.sampleRate, this.frameSize));

            if (this.bufferStopPosition < StopMargin) this.bufferStopPosition = StopMargin;

            while (CurrentPlayPosition < (this.bufferStopPosition - StopMargin) || CurrentPlayPosition > (this.bufferStopPosition))
            {
                Thread.Sleep(50);
                CurrentPlayPosition = this.soundBuffer.PlayPosition;
            }


            // Stopping process begins
            this.bufferStopPosition = -1;
            this.pausePosition = 0;
            this.soundBuffer.Stop();
            this.audioStream.Close();

            this.state = PlayerState.Stopped;
            this.isEndOfAsset = true;
            // RefreshBuffer ends
        }
    }
}
