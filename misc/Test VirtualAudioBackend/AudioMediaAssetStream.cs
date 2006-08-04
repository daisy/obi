using System;
using System.IO;

namespace VirtualAudioBackend
{
	public class AudioMediaAssetStream: Stream
	{
		/// <summary>
		/// This stream can be read from.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// This stream cannot be written to.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// This stream can be seeked into.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Get/set the current position in the stream (in bytes.)
		/// </summary>
		public override long Position
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		/// <summary>
		/// Get the length of the stream (in bytes.)
		/// Should be the asset's length of audio data.
		/// </summary>
		public override long Length
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Get the asset that this stream plays.
		/// </summary>
		public AudioMediaAsset Asset
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Create a new stream for a given audio asset.
		/// </summary>
		/// <param name="asset">The asset for this stream.</param>
		public AudioMediaAssetStream(AudioMediaAsset asset)
		{
		}

		/// <summary>
		/// Read data from the asset into a byte buffer.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public override int Read(byte[] buffer, int offset, int count)
		{
			return 0;
		}

		/// <summary>
		/// No data can be written, so this should raise an exception or silently fail. Must be implemented.
		/// </summary>
		/// <param name="buffer"></param>
		/// <param name="offset"></param>
		/// <param name="count"></param>
		public override void Write(byte[] buffer, int offset, int count)
		{
		}

		/// <summary>
		/// Seek into a given offset.
		/// </summary>
		/// <param name="offset"></param>
		/// <param name="origin"></param>
		/// <returns></returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			return 0;
		}

		/// <summary>
		/// Set the length of the stream. This should be the length of the audio asset.
		/// </summary>
		/// <param name="value"></param>
		public override void SetLength(long value)
		{
		}

		/// <summary>
		/// Clear all buffers (if the input is buffered; otherwise, do nothing, but must be implemented.)
		/// </summary>
		public override void Flush()
		{
		}
	}
}
