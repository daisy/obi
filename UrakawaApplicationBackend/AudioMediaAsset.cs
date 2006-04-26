using System;
using System.IO;
using System.Windows.Forms;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for AudioMeeiaAsset.
	/// </summary>
	public class AudioMediaAsset : MediaAsset , IAudioMediaAsset  
	{

		// member variables 

		double m_LengthTime ;
		long m_LengthByte ;
		int m_Channels ;
		int m_SamplingRate ;
		int m_FrameSize ;
		int m_BitDepth ;
		public long m_LengthData ;


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
				Ar [i] = Convert.ToInt32 (br.ReadByte()) ;

			}
			m_LengthByte  = ConvertToDecimal (Ar) ;


			// length data

			br.BaseStream.Position = 40 ;

			for (int i = 0; i<4 ; i++)
			{
				Ar [i] = Convert.ToInt32(br.ReadByte() );

			}
			m_LengthData  = ConvertToDecimal (Ar) ;

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

		public long AdaptToFrame  (long lVal)
		{
			long  lTemp = lVal / m_FrameSize ;
			return lTemp * m_FrameSize ;
		}

		public long ConvertToDecimal (int [] Ar)
		{
			return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}
		
		public int [] ConvertFromDecimal (long lVal) 
		{
			int [] Result = new int [4] ;
			Result [0] = Result [1] = Result [2] = Result [3] = 0;
			for (int i = 0 ;i<4 ; i++)
			{
				Result [i] = Convert.ToInt32 (lVal % 256 );
				lVal = lVal / 256 ;
			}
			return  Result ;
		}

		public double LengthInMilliseconds
		{
			get
			{
				return m_LengthTime  ;
			}
			set
			{
				m_LengthTime = value ;
			}
		}

		public long LengthByte
		{
			get
			{
				return m_LengthByte ;
			}
			set
			{
				m_LengthByte = value ;
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
			BinaryReader br = new BinaryReader (File.OpenRead(m_sFilePath)) ;
			BinaryWriter bw = new BinaryWriter (File.Create("c:\\t\\temp.wav")) ;
			FileInfo file = new FileInfo (m_sFilePath) ;

			byteBeginPosition = AdaptToFrame (byteBeginPosition) + 44 ;
			byteEndPosition = AdaptToFrame (byteEndPosition) + 44 ;

			br.BaseStream.Position = 0 ;
			bw.BaseStream.Position = 0 ;

			for (long i = 0 ; i< file.Length; i=i+m_FrameSize)
			{
				if (i == byteBeginPosition)
				{
					i = byteEndPosition   ;
					br.BaseStream.Position = byteEndPosition ;
				}
				bw.Write(br.ReadBytes(m_FrameSize)) ;
			}

			FileInfo tempfile = new FileInfo (m_sFilePath + "tmp") ;
			m_LengthByte = tempfile.Length	 ;
			//m_LengthByte = m_LengthByte - (byteEndPosition - byteBeginPosition) ;
			m_LengthTime = ConvertByteToTime (m_LengthByte) ;
			// update length field (4 to 7 )in header
			for (int i = 0; i<4 ; i++)
			{
				bw.BaseStream.Position = i + 4 ;
				bw.Write (Convert.ToByte (ConvertFromDecimal (m_LengthByte)[i])) ;

			}
			long TempLength = m_LengthByte - 44 ;
			for (int i = 0; i<4 ; i++)
			{
				bw.BaseStream.Position = i + 40 ;
				bw.Write (Convert.ToByte (ConvertFromDecimal (TempLength)[i])) ;

			}
			br.Close() ;
			bw.Close() ;

			FileInfo pfile = new FileInfo (m_sFilePath) ;
			file.Delete () ;
			FileInfo nfile = new FileInfo ("c:\\t\\Temp.wav") ;
			nfile.MoveTo (m_sFilePath) ;
			
		}

		// delete by taking time as parameter
		public 		void DeleteChunk(double timeBeginPosition, double timeEndPosition)
		{
			long lBeginPos = ConvertTimeToByte (timeBeginPosition) ;
			long lEndPos = ConvertTimeToByte (timeEndPosition) ;

			DeleteChunk(lBeginPos, lEndPos);
		}

		//Copy audio chunnk in RAM
		public byte [] GetChunk(long byteBeginPosition, long byteEndPosition)
		{

			byteBeginPosition = AdaptToFrame (byteBeginPosition) + 44 ;
			byteEndPosition = AdaptToFrame (byteEndPosition) + 44 ;

			BinaryReader br = new BinaryReader (File.OpenRead(m_sFilePath)) ;

			byte [] arByte = new byte [byteEndPosition - byteBeginPosition + 44] ;

			br.BaseStream.Position = 0 ;
			long i ;
			for (i=0 ; i<44 ; i++  )
			{
				arByte [i] = br.ReadByte () ;
			}

			br.BaseStream.Position = byteBeginPosition ;

			long lCount = byteEndPosition - byteBeginPosition ;
			for (i= i ; i< lCount ; i++)
			{
				arByte [i] = br.ReadByte() ;
			}
			br.Close () ;
			return arByte ;
		}


		public byte []  GetChunk(double timeBeginPosition, double timeEndPosition)
		{
			long lBeginPos = ConvertTimeToByte (timeBeginPosition) ;
			long lEndPos = ConvertTimeToByte (timeEndPosition) ;

			return GetChunk(lBeginPos, lEndPos) ;
		}

		public void InsertByteBuffer(byte [] Buffer, long bytePosition)
		{

			if (CheckStreamsFormat(Buffer) == true)
			{
				bytePosition = AdaptToFrame (bytePosition) + 44 ;

				BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath) );
				br.BaseStream.Position = 0 ;
				BinaryWriter bw = new BinaryWriter(File.Create(m_sFilePath + "tmp")) ;  
				bw.BaseStream.Position = 0 ;

				long lCount = bytePosition ;
				long i ;
				for (i= 0 ; i< lCount ; i = i + m_FrameSize)  
				{
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}


				lCount = Buffer.LongLength - 44 ;

				for (i= 0 ;i < lCount ; i++) 
				{
					bw.Write (Buffer [i+44]) ;
				}

				br.BaseStream.Position = bytePosition ;

			
				lCount = m_LengthByte - bytePosition ;

				for (i = 0 ; i< lCount ; i = i +m_FrameSize )
				{
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}

				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				m_LengthByte = nfile.Length ;
				m_LengthTime = ConvertByteToTime (m_LengthByte) ;

				UpdateLengthHeader (m_LengthByte, bw) ;
				br.Close() ;
				bw.Close() ;


				FileInfo pfile = new FileInfo (m_sFilePath) ;
				pfile.Delete () ;

				nfile.MoveTo (m_sFilePath) ;
			}
				// main if statement
			else
			{
				MessageBox.Show ("Audio streams are of different format. cannot manipulate them") ;	
			}
			// insert byte position function ends 
		}


		public void InsertByteBuffer(byte []  Buffer, double timePosition)
		{
			long lTimePos = ConvertTimeToByte (timePosition) ;

			InsertByteBuffer( Buffer, lTimePos) ;
		}


		void UpdateLengthHeader (long Length, BinaryWriter bw)
		{

			// update length field (4 to 7 )in header
			for (int i = 0; i<4 ; i++)
			{
				bw.BaseStream.Position = i + 4 ;
				bw.Write (Convert.ToByte (ConvertFromDecimal (Length)[i])) ;

			}
			long TempLength = Length - 44 ;
			for (int i = 0; i<4 ; i++)
			{
				bw.BaseStream.Position = i + 40 ;
				bw.Write (Convert.ToByte (ConvertFromDecimal (TempLength)[i])) ;

			}

		}

		// function for converting time into bytes
		long ConvertTimeToByte (double dTime)
		{
			return Convert.ToInt64(( dTime * m_SamplingRate  * m_Channels * (m_BitDepth / 8))/1000) ;
		}

		double ConvertByteToTime (long lByte)
		{
			long lTemp = (1000 * lByte) / (m_SamplingRate * m_Channels * (m_BitDepth/8)) ;
			return Convert.ToDouble (lTemp) ;
		}

		// compare the format of two streams and return bool 
		public bool CheckStreamsFormat(byte [] Buffer)
		{
			BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath)) ;

			br.BaseStream.Position = 22 ;

			for (int i = 22 ; i <36 ; i ++)
			{
				if (br.ReadByte() != Buffer [i])
				{
					br.Close() ;
					return false ;
				}
			}
			br.Close() ;
			return true;
		}

		public bool CheckStreamsFormat(IAudioMediaAsset asset)
		{
			byte [] Buffer = new byte [44] ;
			BinaryReader brBuffer = new BinaryReader (File.OpenRead(asset.Path)) ;
			brBuffer.BaseStream.Position = 0 ;
			for (int i = 0 ; i< 44 ; i++)
			{
				Buffer [i] = brBuffer.ReadByte () ;
			}
brBuffer.Close() ;
return CheckStreamsFormat(Buffer) ;
		}
		
	

// class ends here
	}
}
