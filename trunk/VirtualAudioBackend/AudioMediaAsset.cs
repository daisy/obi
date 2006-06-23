using System;
using System.Collections;

namespace VirtualAudioBackend
{
	public class AudioMediaAsset: MediaAsset, IAudioMediaAsset
	{
		/// <summary>
		/// Sample rate in Hertz (support at least up to 44100 Hz.)
		/// </summary>
		public int SampleRate
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Number of channels (1 = mono or 2 = stereo.)
		/// </summary>
		public int Channels
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Bit depth is either 8 or 16.
		/// </summary>
		public int BitDepth
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Length of the audio asset in milliseconds.
		/// </summary>
		public double LengthInMilliseconds
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Size of the audio data in bytes.
		/// </summary>
		public long AudioLengthInBytes
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Constructor for an empty AudioMediaAsset. The format is specified by the arguments and there is no initial audio data.
		/// </summary>
		/// <param name="channels">Number of channels (1 or 2.)</param>
		/// <param name="bitDepth">Bit depth (8 or 16?)</param>
		/// <param name="sampleRate">Sample rate in Hz.</param>
		public AudioMediaAsset(int channels, int bitDepth, int sampleRate)
		{
		}

		/// <summary>
		/// Constructor for an audio asset from existing clips.
		/// </summary>
		/// <param name="clips">The list of <see cref="AudioClip"/>s.</param>
		public AudioMediaAsset(ArrayList clips)
		{
		}

		/// <summary>
		/// Make a copy of the asset, sharing the same format and data.
		/// </summary>
		/// <returns>The new, identical asset.</returns>
		public AudioMediaAsset Copy()
		{
			return null;
		}

		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		public override void Delete()
		{
		}

		/// <summary>
		/// Append new bytes at the end. The data is considered to have the correct format.
		/// A new AudioClip object is created.
		/// This is a simple case of InsertByteBuffer, where the position is the end of the data.
		/// </summary>
		/// <param name="buffer">The data to append.</param>
		public void AppendByteBuffer(byte[] buffer)
		{
		}
	}
}
