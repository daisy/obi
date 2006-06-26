using System;
using System.Collections;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Audio media assets are in this case uncompressed audio files.
	/// In the future this will be the super class for various kinds of audio files.
	/// </summary>
	public interface IAudioMediaAsset: IMediaAsset
	{
		/// <summary>
		/// Sample rate in Hertz (support at least up to 44100 Hz.)
		/// </summary>
		int SampleRate
		{
			get;
		}

		/// <summary>
		/// Number of channels (1 = mono or 2 = stereo.)
		/// </summary>
		int Channels
		{
			get;
		}

		/// <summary>
		/// Bit depth is either 8 or 16.
		/// </summary>
		int BitDepth
		{
			get;
		}

		/// <summary>
		/// Length of the audio asset in milliseconds.
		/// </summary>
		double LengthInMilliseconds
		{
			get;
		}

		/// <summary>
		/// Size of the audio data in bytes.
		/// </summary>
		long AudioLengthInBytes
		{
			get;
		}

		/// <summary>
		/// Append new bytes at the end. The data is considered to have the correct format.
		/// This is a simple case of InsertByteBuffer, where the position is the end of the data.
		/// </summary>
		/// <param name="data">The data to append.</param>
		void AppendBytes(byte[] data);

		/// <summary>
		/// Get a chunk of a data from the asset and return it as a new asset.
		/// </summary>
		/// <param name="beginPosition">Start position of the chunk in bytes.</param>
		/// <param name="endPosition">End position of the chunk in bytes.</param>
		/// <returns>A new asset with the same data.</returns>
		IAudioMediaAsset GetChunk(long beginPosition, long endPosition);

		/// <summary>
		/// Get a chunk of data from the asset and return it as a new asset.
		/// </summary>
		/// <param name="beginTime">Start time of the chunk in milliseconds.</param>
		/// <param name="endTime">End time of the chunk in milliseconds.</param>
		/// <returns>A new asset with the same data.</returns>
		IAudioMediaAsset GetChunk(double beginTime, double endTime);

		/// <summary>
		/// Insert data from an existing asset.
		/// </summary>
		/// <param name="chunk">The chunk of data to insert.</param>
		/// <param name="position">The insertion point as a byte position.</param>
		void InsertAsset(IAudioMediaAsset chunk, long position);

		/// <summary>
		/// Insert data from an existing asset.
		/// </summary>
		/// <param name="chunk">The chunk of data to insert.</param>
		/// <param name="time">The insertion point in milliseconds.</param>
		void InsertAsset(IAudioMediaAsset chunk, double time);

		/// <summary>
		/// Delete a chunk of audio from an asset and return the deleted part as a new asset (see GetChunk.)
		/// </summary>
		/// <param name="beginPosition">Start position of the chunk in bytes.</param>
		/// <param name="endPosition">End position of the chunk in bytes.</param>
		/// <returns>The deleted chunk.</returns>
		IAudioMediaAsset DeleteChunk(long beginPosition, long endPosition);

		/// <summary>
		/// Delete a chunk of audio from an asset and return the deleted part as a new asset (see GetChunk.)
		/// </summary>
		/// <param name="beginTime">Start time of the chunk in milliseconds.</param>
		/// <param name="endTime">End time of the chunk in milliseconds.</param>
		/// <returns>The deleted chunk.</returns>
		IAudioMediaAsset DeleteChunk(double beginTime, double endTime);

		/// <summary>
		/// Split the asset at the given time and return the second half as a new asset.
		/// This asset is now the first half.
		/// </summary>
		/// <param name="position">Split position in bytes.</param>
		/// <returns>The second half of the asset.</returns>
		IAudioMediaAsset Split(long position);

		/// <summary>
		/// Split the asset at the given time and return the second half as a new asset.
		/// This asset is now the first half.
		/// </summary>
		/// <param name="time">Split position in bytes.</param>
		/// <returns>The second half of the asset.</returns>
		IAudioMediaAsset Split(double time);

		/// <summary>
		/// Detect the maximum amplitude value in an prerecorded silent file.
		/// </summary>
		/// <param name="silenceRef">A silence file of same format as the asset.</param>
		/// <returns>The amplitude value, suitable for use by ApplyPhraseDetection().</returns>
		long GetSilenceAmplitude(IAudioMediaAsset silenceRef);

		/// <summary>
		/// Split an audio asset into phrases using a sentence detection algorithm.
		/// The first phrase may have leading silence, other phrases have no leading silence.
		/// All phrases may have trailing silence. The asset is left unmodified.
		/// </summary>
		/// <param name="threshold">The maximum level of what is considered silence.</param>
		/// <param name="length">The minimum length of silence between phrases (in bytes.)</param>
		/// <param name="before">The amount of silence at the beginning of a phrase (in bytes.)</param>
		/// <returns>The list of new audio assets in order.</returns>
		ArrayList ApplyPhraseDetection(long threshold, long length, long before);

		/// <summary>
		/// Split an audio asset into phrases using a sentence detection algorithm.
		/// The first phrase may have leading silence, other phrases have no leading silence.
		/// All phrases may have trailing silence. The asset is left unmodified.
		/// </summary>
		/// <param name="threshold">The maximum level of what is considered silence.</param>
		/// <param name="length">The minimum length of silence between phrases (in milliseconds.)</param>
		/// <param name="before">The amount of silence at the beginning of a phrase (in milliseconds.)</param>
		/// <returns>The list of new audio assets in order.</returns>
		ArrayList ApplyPhraseDetection(long threshold, double length, double before);
	}
}