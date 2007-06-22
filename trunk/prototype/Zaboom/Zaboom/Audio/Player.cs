using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using urakawa.media.data.audio;

namespace Zaboom.Audio
{
	public class Player
	{
		public event EndOfAudioAssetHandler EndOfAudioAsset;
		public event StateChangedHandler StateChanged;
		
        #region private members

        private static readonly int BUFFER_LENGTH_SECONDS = 1;  // buffer length in seconds
        private static readonly int NOTIFICATIONS = 4;          // number of notifications per buffer

        private int bufferSize;                            // size of the audio buffer
        private AudioMediaData currentMedia;               // media object currently being played
        private OutputDevice device;                       // device to play to
        private BufferPositionNotify[] endNotifyPosition;  // notification for the end of the data
        private int lengthRead;                            // length of data read so far
        private int nextOffset;                            // next offset in the notification positions
        private Notify notify;                             // used to send notifications while playing
        private AutoResetEvent notifyEvent;                // the actual notification event
        private BufferPositionNotify[] notifyPosition;     // positions of data to refill
        private int notifySize;                            // size of the data to refill for a notification
        private bool notifyStop;                           // the notify thread shoud stop
        private Thread notifyThread;                       // thread handling the notifications
        private BinaryReader reader;                       // reader for the audio data
        private SecondaryBuffer soundBuffer;               // sound buffer for playback
        private AudioPlayerState state;                    // current state of the player

        #endregion

        public Player()
        {
            CurrentMedia = null;
            notifyPosition = new BufferPositionNotify[NOTIFICATIONS + 1];
            endNotifyPosition = new BufferPositionNotify[1];
        }

        /// <summary>
        /// Get or set the currently playing audio media data object.
        /// </summary>
        public AudioMediaData CurrentMedia
        {
            get { return currentMedia; }
            set
            {
                currentMedia = value;
                if (value != null)
                {
                    lengthRead = 0;
                    soundBuffer = BufferForMedia;
                    reader = new BinaryReader(currentMedia.getAudioData());
                    _State = AudioPlayerState.Stopped;
                }
                else
                {
                    _State = AudioPlayerState.NotReady;
                }
            }
        }

        /// <summary>
        /// Get the currently chosen input device.
        /// </summary>
        public OutputDevice OutputDevice { get { return OutputDevice; } }

        /// <summary>
        /// Get the list of available output devices.
        /// </summary>
        public List<OutputDevice> OutputDevices
        {
            get
            {
                DevicesCollection devices = new DevicesCollection();
                List<OutputDevice> outputDevices = new List<OutputDevice>(devices.Count);
                foreach (DeviceInformation info in devices)
                {
                    outputDevices.Add(new OutputDevice(info.Description, new Device(info.DriverGuid)));
                }
                return outputDevices;
            }
        }

        /// <summary>
        /// Play the current audio media data object from beginning to end.
        /// </summary>
        public void Play()
        {
            if (state == AudioPlayerState.Stopped || state == AudioPlayerState.Paused)
            {
                nextOffset = 0;
                for (int i = 0; i < NOTIFICATIONS; ++i) lengthRead += RefillBuffer();
                soundBuffer.SetCurrentPosition(0);
                soundBuffer.Play(0, BufferPlayFlags.Looping);
                _State = AudioPlayerState.Playing;
            }
        }

        /// <summary>
        /// Set output devide to the first one found.
        /// </summary>
        public void SetOutputDevice(Control handle)
        {
            List<OutputDevice> devices = OutputDevices;
            if (devices.Count == 0) throw new Exception("No output device available!");
            SetOutputDevice(handle, devices[0]);
        }

        /// <summary>
        /// Set the device to be used by the player.
        /// </summary>
        public void SetOutputDevice(Control handle, OutputDevice device)
        {
            this.device = device;
            this.device.Device.SetCooperativeLevel(handle, CooperativeLevel.Priority);
        }

        /// <summary>
        /// Set the device that matches this name; if it could not be found, default to the first one.
        /// Throw an exception if no devices were found.
        /// </summary>
        public void SetOutputDevice(Control handle, string name)
        {
            List<OutputDevice> devices = OutputDevices;
            if (devices.Count == 0) throw new Exception("No output device available!");
            OutputDevice found = devices.Find(delegate(OutputDevice d) { return d.Name == name; });
            SetOutputDevice(handle, found == null ? devices[0] : found);
        }

        /// <summary>
        /// Get the current state of the audio player.
        /// </summary>
        public AudioPlayerState State { get { return state; } }

        /// <summary>
        /// Stop playback.
        /// </summary>
        public void Stop()
        {
            if (state != AudioPlayerState.Stopped)
            {
                soundBuffer.Stop();
                reader.Close();
                _State = AudioPlayerState.Stopped;
            }
        }


        /// <summary>
        /// Get a buffer description for the current media object.
        /// </summary>
        private BufferDescription BufferDescriptionForMedia
        {
            get
            {
                BufferDescription desc = new BufferDescription(WaveFormatForMedia);
                bufferSize = BUFFER_LENGTH_SECONDS * desc.Format.AverageBytesPerSecond;
                bufferSize -= bufferSize % desc.Format.BlockAlign;
                notifySize = bufferSize / NOTIFICATIONS;
                notifySize -= notifySize % desc.Format.BlockAlign;
                desc.BufferBytes = bufferSize;
                desc.ControlPositionNotify = true;
                desc.GlobalFocus = true;
                desc.LocateInSoftware = true;
                desc.LocateInHardware = false;
                return desc;
            }
        }

        /// <summary>
        /// Create the secondary buffer for the current media object.
        /// </summary>
        private SecondaryBuffer BufferForMedia
        {
            get
            {
                SecondaryBuffer buffer = new SecondaryBuffer(BufferDescriptionForMedia, device.Device);
                if (notifyThread != null && notifyThread.IsAlive)
                {
                    notifyStop = true;
                    notifyThread.Join();
                }
                notifyStop = false;
                notifyThread = new Thread(new ThreadStart(RefillBufferWorker));
                notifyThread.Name = "Audio buffer notify thread";
                notifyThread.Priority = ThreadPriority.Highest;
                notifyThread.Start();
                notifyEvent = new AutoResetEvent(false);
                for (int i = 0; i < NOTIFICATIONS; ++i)
                {
                    notifyPosition[i].Offset = notifySize * (i + 1) - 1;
                    notifyPosition[i].EventNotifyHandle = notifyEvent.SafeWaitHandle.DangerousGetHandle();
                }
                endNotifyPosition[0].EventNotifyHandle = notifyEvent.SafeWaitHandle.DangerousGetHandle();
                notify = new Notify(buffer);
                notify.SetNotificationPositions(notifyPosition, NOTIFICATIONS);
                return buffer;
            }
        }

        /// <summary>
        /// Update the state of the audio player and send an event if anyone is listening.
        /// </summary>
        private AudioPlayerState _State
        {
            set
            {
                if (state != value)
                {
                    StateChangedEventArgs e = new StateChangedEventArgs(state);
                    state = value;
                    if (StateChanged != null) StateChanged(this, e);
                }
            }
        }

        /// <summary>
        /// Worker that refreshes the buffer as long as there is data in the stream.
        /// </summary>
        private void RefillBufferWorker()
        {
            while (lengthRead < currentMedia.getPCMLength())
            {
                notifyEvent.WaitOne(Timeout.Infinite, true);
                lengthRead += RefillBuffer();
                System.Diagnostics.Debug.Print("Read {0} out of {1}", lengthRead, currentMedia.getPCMLength());
            }


            // endNotifyPosition[0].Offset = (nextOffset - notifySize) % bufferSize;
            // soundBuffer.Stop();
            // notify.SetNotificationPositions(endNotifyPosition, 1);
            // soundBuffer.Play(0, BufferPlayFlags.Looping);

            // TODO: clean this up, and see whether above is necessary
            int endPosition = currentMedia.getPCMLength() % bufferSize;
            int lastPosition, playPosition, writePosition;
            soundBuffer.GetCurrentPosition(out playPosition, out writePosition);
            lastPosition = playPosition;
            int remaining = endPosition - playPosition;
            if (remaining < 0) remaining += bufferSize;
            System.Diagnostics.Debug.Print("* At {0}, going to {1}, remaining {2}", playPosition, endPosition, remaining);
            while (remaining > 0)
            {
                notifyEvent.WaitOne(Timeout.Infinite, true);
                soundBuffer.GetCurrentPosition(out playPosition, out writePosition);
                int diff = playPosition - lastPosition;
                lastPosition = playPosition;
                if (diff < 0) diff += bufferSize;
                remaining -= diff;
                System.Diagnostics.Debug.Print("At {0}, going to {1}, read {2}, remaining {3}",
                    playPosition, endPosition, diff, remaining);
            }

            Stop();
            if (EndOfAudioAsset != null) EndOfAudioAsset(this, new EventArgs());
        }

        /// <summary>
        /// Refill the next part of the buffer with data from the file.
        /// </summary>
        /// <returns>The amount of data actually read.</returns>
        private int RefillBuffer()
        {
            byte[] buffer = new byte[notifySize];
            int read = reader.Read(buffer, 0, notifySize);
            if (read < notifySize) Array.Clear(buffer, read, notifySize - read);
            soundBuffer.Write(nextOffset, buffer, LockFlag.None);
            nextOffset = (nextOffset + notifySize) % bufferSize;
            return read;
        }

        /// <summary>
        /// Get a wave format object from the current media.
        /// </summary>
        private WaveFormat WaveFormatForMedia
        {
            get
            {
                WaveFormat format = new WaveFormat();
                format.AverageBytesPerSecond = Convert.ToInt32(currentMedia.getPCMFormat().getByteRate());
                format.BitsPerSample = Convert.ToInt16(currentMedia.getPCMFormat().getBitDepth());
                format.BlockAlign = Convert.ToInt16(currentMedia.getPCMFormat().getBlockAlign());
                format.Channels = Convert.ToInt16(currentMedia.getPCMFormat().getNumberOfChannels());
                format.FormatTag = WaveFormatTag.Pcm;
                format.SamplesPerSecond = Convert.ToInt32(currentMedia.getPCMFormat().getSampleRate());
                return format;
            }
        }
    }

    /// <summary>
    /// The four states of the audio player.
    /// NotReady: doing something other that playing, being paused or stopped.
    /// Paused: playback was interrupted and can be resumed.
    /// Playing: sound is currently playing.
    /// Stopped: player is idle.
    /// </summary>
    public enum AudioPlayerState { NotReady, Paused, Playing, Stopped };

    public class StateChangedEventArgs : EventArgs
    {
        public AudioPlayerState PreviousState;
        public StateChangedEventArgs(AudioPlayerState prev) { PreviousState = prev; }
    }

    public delegate void StateChangedHandler(object sender, StateChangedEventArgs e);
    public delegate void EndOfAudioAssetHandler(object sender, EventArgs e);
}
