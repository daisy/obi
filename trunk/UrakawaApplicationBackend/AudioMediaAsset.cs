using System;
using System.IO;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for AudioMeeiaAsset.
	/// </summary>
	public class AudioMediaAsset : MediaAsset , IAudioMediaAsset  
	{
		public AudioMediaAsset (string sPath) :base (sPath)
									 {
Init  (sPath) ;
									 }

		void Init (string sPath)
		{
			int [] Ar = new int[4] ;
Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;
BinaryReader br = new  BinaryReader (File.OpenRead(sPath)) ;

// length in bytes
			br.BaseStream.Position = 4 ;

			for (int i = 0; i<4 ; i++)
			{
Ar [i] = br.ReadByte() ;

			}
m_LengthByte  = ConvertToDecimal (Ar) ;



// channels
Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

br.BaseStream.Position = 22 ;
			for (int i= 0 ; i<2 ; i++)
			{
Ar [i] = br.ReadByte() ;
			}

			m_Channels = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

			// Sampling rate
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 24 ;
			for (int i= 0 ; i<4 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_SamplingRate = Convert.ToInt32 (ConvertToDecimal (Ar)) ;
		
//Frame size
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 32 ;
			for (int i= 0 ; i<2 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_FrameSize  = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

//bit depth
			Ar [0] = Ar [1] = Ar[2] = Ar [3] = 0 ;

			br.BaseStream.Position = 34 ;
			for (int i= 0 ; i<2 ; i++)
			{
				Ar [i] = br.ReadByte() ;
			}

			m_BitDepth = Convert.ToInt32 (ConvertToDecimal (Ar)) ;

			//size in time
			m_LengthTime = Convert.ToDouble ((m_LengthByte * 1000)/ (m_SamplingRate * m_FrameSize));

br.Close() ;
		}

		long ConvertToDecimal (int [] Ar)
		{
return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}
		
double m_LengthTime ;
			long m_LengthByte ;
			int m_Channels ;
			int m_SamplingRate ;
		int m_FrameSize ;
int m_BitDepth ;

		public double LengthInMilliseconds
		{
			get
			{
return m_LengthTime  ;
			}
		}

		public int SampleRate
		{
			get
			{
return m_SamplingRate ;
			}
		}

		public int Channels
		{
			get
			{
return m_Channels ;
			}
		}

		public int FrameSize
		{
			get
			{
return m_FrameSize ;
			}
		}

		public int BitDepth
		{
			get
			{
return m_BitDepth ;
			}
		}


		public void DeleteChunk(long byteBeginPosition, long byteEndPosition)
		{

		}


	}
}
