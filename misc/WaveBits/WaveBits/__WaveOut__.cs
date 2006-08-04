using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace WaveBits
{
    /// <summary>
    /// Let's enjoy playing wave files with winmm.dll!
    /// Most definitions are taken from MMSystem.h.
    /// With some help from http://www.codeproject.com/audio/wavefiles.asp
    /// </summary>
    public class __WaveOut__
    {
        /// <summary>
        /// Play a sound file on the default device.
        /// </summary>
        /// <param name="file">The file to play.</param>
        public static void Play(WaveFile file)
        {
            IntPtr deviceHandle = new IntPtr();
            WAVEFORMATEX format = new WAVEFORMATEX(file);
            WaveDelegate feed = new WaveDelegate(WaveFeed);
            uint err = waveOutOpen(out deviceHandle, WAVE_MAPPER, format, feed, 0, CALLBACK_FUNCTION);
            Console.WriteLine("PLAY/Open = <{0}> ({1})", GetErrorText(err), err);
        }

        private static void WaveFeed(IntPtr handle, uint message, uint user, ref WAVEHDR header, uint param2)
        {
            switch (message)
            {
                case MM_WOM_OPEN:
                    Console.WriteLine("!!! OPEN !!!");
                    break;
                case MM_WOM_CLOSE:
                    Console.WriteLine("!!! CLOSE !!!");
                    break;
                case MM_WOM_DONE:
                    Console.WriteLine("!!! DONE !!!");
                    break;
                default:
                    Console.WriteLine("??? {0}", message);
                    break;
            }
        }

        /// <summary>
        /// Get the number of devices that can play audio.
        /// </summary>
        /// <returns>Number of devices.</returns>
        [DllImport("winmm.dll")]
        private static extern uint waveOutGetNumDevs();

        [StructLayout(LayoutKind.Sequential)]
        public struct WAVEHDR
        {
            public IntPtr mData;         // pointer to the waveform buffer
            public uint mBufferLength;   // length in bytes of the data buffer
            public uint mBytesRecorded;  // when used for input
            public IntPtr mUser;         // user data
            public uint mFlags;          // see flags below
            public uint mLoops;          // number of times to play a loop
            public IntPtr mNext;         // reserved
            public uint reserved;        // reserved
        }

        public const int WAVE_MAPPER = -1;  // default wave out device

        public const uint WHDR_DONE = 0x00000001;       // done bit
        public const uint WHDR_PREPARED = 0x00000002;   // set if this header has been prepared
        public const uint WHDR_BEGINLOOP = 0x00000004;  // loop start block
        public const uint WHDR_ENDLOOP = 0x00000008;    // loop end block
        public const uint WHDR_INQUEUE = 0x00000010;    // reserved for driver

        /// <summary>
        /// Callback for playing a sound.
        /// </summary>
        /// <param name="hdrvr"></param>
        /// <param name="uMsg"></param>
        /// <param name="dwUser"></param>
        /// <param name="wavhdr"></param>
        /// <param name="dwParam2"></param>
        public delegate void WaveDelegate(IntPtr handle, uint message, uint user, ref WAVEHDR wavhdr, uint param2);

        public const uint CALLBACK_FUNCTION = 0x00030000;  // Callback is a FARPROC
        
        public const uint MM_WOM_OPEN = 0x3BB;
        public const uint MM_WOM_CLOSE = 0x3BC;
        public const uint MM_WOM_DONE = 0x3BD;

        public const ushort WAVE_FORMAT_PCM = 1;  // PCM is the only format tag that we know about

        [StructLayout(LayoutKind.Sequential)]
        public class WAVEFORMATEX
        {
            private ushort mFormatTag;      // 1 for PCM (see above)
            private ushort mChannels;       // mono or stereo
            private uint mSamplesPerSec;    // sample rate
            private uint mAvgBytesPerSec;   // bitrate
            private ushort mBlockAlign;     // block alignment, in bytes
            private ushort mBitsPerSample;  // bits per sample
            private ushort mSize;           // unused field (size for extra information)

            public WAVEFORMATEX(WaveFile file)
            {
                mFormatTag = WAVE_FORMAT_PCM;
                mChannels = file.Format.Channels;
                mSamplesPerSec = file.Format.SampleRate;
                mAvgBytesPerSec = file.Format.ByteRate;
                mBlockAlign = file.Format.BlockAlign;
                mBitsPerSample = file.Format.BitsPerSample;
                mSize = 0;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="waveOut"></param>
        /// <param name="deviceId"></param>
        /// <param name="format"></param>
        /// <param name="callback"></param>
        /// <param name="delegateInstance"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern uint waveOutOpen(out IntPtr waveOutHandle, int deviceId, WAVEFORMATEX format, WaveDelegate callback,
            uint delegateInstance, uint flags);

        /// <summary>
        /// Prepare a data block for playback.
        /// </summary>
        /// <param name="waveOut">Handle to the output device</param>
        /// <param name="waveOutHeader">Header of the data block to be prepared</param>
        /// <param name="size">Size of the WaveHdr structure.</param>
        /// <returns>The error code.</returns>
        [DllImport("winmm.dll")]
        public static extern uint waveOutPrepareHeader(IntPtr waveOutHandle, ref WAVEHDR waveOutHeader, uint size);

        private static void PrepareHeader(IntPtr waveOutHandle, ref WAVEHDR waveOutHeader, uint size)
        {
            uint err = waveOutPrepareHeader(waveOutHandle, ref waveOutHeader, size);
            if (err != 0)
            {
                throw new Exception("Error preparing header: " + GetErrorText(err) + ".");
            }
        }

        /// <summary>
        /// Cleanup a header before disposing of it.
        /// </summary>
        /// <param name="waveOut"></param>
        /// <param name="waveOutHeader"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        [DllImport("winmm.dll")]
        private static extern uint waveOutUnprepareHeader(IntPtr waveOut, ref WAVEHDR waveOutHeader, uint size);

        /// <summary>
        /// Get a textual message for a given error code.
        /// </summary>
        /// <param name="error">The error code.</param>
        /// <param name="str">The string in which to return the message.</param>
        /// <param name="textLen">The length of the allocated string</param>
        /// <returns>An error code (sigh...)</returns>
        [DllImport("winmm.dll")]
        private static extern uint waveOutGetErrorText(uint error, [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder str,
            uint textLen);

        private const uint MAXERRORLENGTH = 256;  // all error messages are shorter than this

        /// <summary>
        /// Friendlier version of waveOutGetErrorText.
        /// Throw an exception if in trouble.
        /// </summary>
        /// <param name="error">The error code.</param>
        /// <returns>The corresponding message.</returns>
        private static string GetErrorText(uint error)
        {
            StringBuilder str = new StringBuilder((int)MAXERRORLENGTH);
            uint err = waveOutGetErrorText(error, str, MAXERRORLENGTH);
            if (err != 0)
            {
                throw new Exception(String.Format("Can't get error message for error #{0}?!", error));
            }
            return str.ToString();
        }
    }
}
