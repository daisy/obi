using System;
using System.Collections;

namespace VirtualAudioBackend
{
	public class AudioMediaAsset: MediaAsset, IAudioMediaAsset
	{
		public int SampleRate
		{
			get
			{
				return 0;
			}
		}

		public int Channels
		{
			get
			{
				return 0;
			}
		}

		public int BitDepth
		{
			get
			{
				return 0;
			}
		}

		public double LengthInMilliseconds
		{
			get
			{
				return 0;
			}
		}

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
		public override IMediaAsset Copy()
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

		public void AppendBytes(byte[] data)
		{
		}

		public IAudioMediaAsset GetChunk(long beginPosition, long endPosition)
		{
			return null;
		}

		public IAudioMediaAsset GetChunk(double beginTime, double endTime)
		{
			return null;
		}

		public void InsertAsset(IAudioMediaAsset chunk, long position)
		{
		}

		public void InsertAsset(IAudioMediaAsset chunk, double time)
		{
		}

		public IAudioMediaAsset DeleteChunk(long beginPosition, long endPosition)
		{
			return null;
		}

		public IAudioMediaAsset DeleteChunk(double beginTime, double endTime)
		{
			return null;
		}

		public override void MergeWith(IMediaAsset next)
		{
		}

		public IAudioMediaAsset Split(long position)
		{
			return null;
		}

		public IAudioMediaAsset Split(double time)
		{
			return null;
		}

		public long GetSilenceAmplitude(IAudioMediaAsset silenceRef)
		{
			return 0;
		}

		public ArrayList ApplyPhraseDetection(long threshold, long length, long before)
		{
			return null;
		}

		public ArrayList ApplyPhraseDetection(long threshold, double length, double before)
		{
			return null;
		}
	}
}