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
			set;
		}

		long LengthByte
		{
get;
			set;
		}

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

//Added later for frame size of wav file
		int FrameSize
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

		long LengthData
		{
			get;
			set;
		}

		/// <summary>
		/// Insert a byte buffer in an asset at a given byte position.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="buffer">The data to insert. The actual "byte buffer" type has yet to be specified.</param>
		/// <param name="bytePosition">The position inside the asset to insert at, in bytes.</param>
		 // byte position is excluding header length
		// return type is changed to IAudioMediaAsset from void
		 IAudioMediaAsset InsertByteBuffer(byte [] Buffer, long bytePosition) ;


		/// <summary>
		/// Insert a byte buffer in an asset at a given timee position.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="buffer">The data to insert. The actual "byte buffer" type has yet to be specified.</param>
		/// <param name="timePosition">The position inside the asset to insert at, in millesconds.</param>
		/// /// // time position is excluding header length
		/// return type is changed from void to IAudioMediaAsset
		IAudioMediaAsset InsertByteBuffer(byte []  Buffer, double timePosition);

		/// <summary>
		/// Get a byte buffer between two points in the asset.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="byteBeginPosition">Begin position (in bytes.)</param>
		/// <param name="byteEndPosition">End position (in bytes.)</param>
		/// <returns>A byte buffer (specific type to be determined.)</returns>
		/// // the returned byte array is like virtual wave file in RAM with proper header
		/// // byte position is excluding header length
		byte [] GetChunk(long byteBeginPosition, long byteEndPosition);

		/// <summary>
		/// Get a byte buffer between two points in the asset.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="timeBeginPosition">Begin position (in milliseconds.)</param>
		/// <param name="timeEndPosition">End position (in milliseconds.)</param>
		/// <returns>A byte buffer (specific type to be determined.)</returns>
		/// // All the positions are relative to 44 th bit of header i.e header length is excluded
		byte []  GetChunk(double timeBeginPosition, double timeEndPosition);

		/// <summary>
		/// Delete a byte buffer between two points in the asset.
		/// The data at the end of the asset must be moved back.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="byteBeginPosition">Begin position (in bytes.)</param>
		/// <param name="byteEndPosition">End position (in bytes.)</param>
		/// /// // byte position is excluding header length
		/// return type changed to IAudioMediaAsset from void
		IAudioMediaAsset DeleteChunk(long byteBeginPosition, long byteEndPosition);
		
		/// <summary>
		/// Delete a byte buffer between two points in the asset.
		/// The data at the end of the asset must be moved back.
		/// Throw an exception in case of problem (memory error, position is out of bounds, etc.)
		/// </summary>
		/// <param name="timeBeginPosition">Begin position (in milliseconds.)</param>
		/// <param name="timeEndPosition">End position (in milliseconds.)</param>
		/// /// // time position is excluding header length
		/// // return type changed to IAudioMediaAsset from void
		IAudioMediaAsset DeleteChunk(double timeBeginPosition, double timeEndPosition);


// function for detecting phrases  in IAudioMediaAsset  by taking byte position as input and output
long [] DetectPhrases (IAudioMediaAsset Ref, long PhraseLength , long BeforePhrase) ;


// function for detecting phrases  in IAudioMediaAsset  by taking time position in milliseconds as input and output
		double [] DetectPhrases (IAudioMediaAsset Ref, double PhraseLength , double BeforePhrase) ;


		
//  adjust the positions according to frame size to avoid any overlapping of channels etc
long AdaptToFrame  (long lVal);

// Decode the wave header to decimal format
long ConvertToDecimal (int [] Ar);

// Encode the Decimal format to wave header format
		int [] ConvertFromDecimal (long lVal)  ;

// Compare and checks the format of two audio streams for their compatibility
bool CheckStreamsFormat(byte [] Buffer);

// Compare and checks the format of audio assets  for compatibility
		bool CheckStreamsFormat(IAudioMediaAsset asset) ;
	}
}
