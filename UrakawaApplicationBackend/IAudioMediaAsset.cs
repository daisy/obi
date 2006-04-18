using System;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// TODO: define the interface for silence detection.
	/// </summary>
	public interface IAudioMediaAsset: IMediaAsset
	{
		/// <summary>
		/// Length of the audio asset in milliseconds.
		/// </summary>
		double LengthInMilliseconds
		{
			get;
		}

		/// <summary>
		/// Sample rate in Hertz (support at least up to 44100 Hz.)
		/// </summary>
		uint SampleRate
		{
			get;
		}

		/// <summary>
		/// Number of channels (1 = mono or 2 = stereo.)
		/// </summary>
		uint Channels
		{
			get;
		}

		/// <summary>
		/// Bit depth is either 8 or 16.
		/// </summary>
		uint BitDepth
		{
			get;
		}

		/// <summary>
		/// Insert a byte buffer in an asset at a given byte position.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="buffer">The data to insert. The actual "byte buffer" type has yet to be specified.</param>
		/// <param name="bytePosition">The position inside the asset to insert at, in bytes.</param>
		void InsertByteBuffer(Object buffer, ulong bytePosition);

		/// <summary>
		/// Insert a byte buffer in an asset at a given timee position.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="buffer">The data to insert. The actual "byte buffer" type has yet to be specified.</param>
		/// <param name="timePosition">The position inside the asset to insert at, in millesconds.</param>
		void InsertByteBuffer(Object buffer, double timePosition);

		/// <summary>
		/// Get a byte buffer between two points in the asset.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="byteBeginPosition">Begin position (in bytes.)</param>
		/// <param name="byteEndPosition">End position (in bytes.)</param>
		/// <returns>A byte buffer (specific type to be determined.)</returns>
		Object GetChunk(ulong byteBeginPosition, ulong byteEndPosition);

		/// <summary>
		/// Get a byte buffer between two points in the asset.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="timeBeginPosition">Begin position (in milliseconds.)</param>
		/// <param name="timeEndPosition">End position (in milliseconds.)</param>
		/// <returns>A byte buffer (specific type to be determined.)</returns>
		Object GetChunk(double timeBeginPosition, double timeEndPosition);

		/// <summary>
		/// Delete a byte buffer between two points in the asset.
		/// The data at the end of the asset must be moved back.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="byteBeginPosition">Begin position (in bytes.)</param>
		/// <param name="byteEndPosition">End position (in bytes.)</param>
		void DeleteChunk(ulong byteBeginPosition, ulong byteEndPosition);
		
		/// <summary>
		/// Delete a byte buffer between two points in the asset.
		/// The data at the end of the asset must be moved back.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="timeBeginPosition">Begin position (in milliseconds.)</param>
		/// <param name="timeEndPosition">End position (in milliseconds.)</param>
		void DeleteChunk(double timeBeginPosition, double timeEndPosition);
	}
}
