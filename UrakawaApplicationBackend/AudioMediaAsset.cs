
using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

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

// constructor to initialise above listed member variables
		public AudioMediaAsset (string sPath) :base (sPath)
		{
			Init  (sPath) ;
		}


// function to initialise the member variables
		void Init (string sPath)
		{

//declare   array variable of size 4 as the max chunk in header is 4 bytes long
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
			//convert from mod 256 to mod 10
			return Ar[0] + (Ar[1] * 256) + (Ar[2] *256 *256) + (Ar[3] *256 *256 *256) ;
		}
		
		public int [] ConvertFromDecimal (long lVal) 
		{
// convert  mod 10 to 4 byte array each of mod 256
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


		public long LengthData
		{
			get
			{
return m_LengthData ;
			}
			set
			{
m_LengthData = value ;
			}
		}
// deletes a section of audio streamfrom physical asset by takin byte position as parameter
		public  IAudioMediaAsset  DeleteChunk(long byteBeginPosition, long byteEndPosition)
		{
			// checks for valid parameters
			if (byteBeginPosition < byteEndPosition && byteEndPosition < m_LengthData)
			{
				// opens the original file for reading

				BinaryReader br = new BinaryReader (File.OpenRead(m_sFilePath)) ;

				// creats a new temporary wave file for manipulation
				BinaryWriter bw = new BinaryWriter (File.Create(m_sFilePath + "tmp")) ;
				
				FileInfo file = new FileInfo (m_sFilePath) ;

				//the Position is originally excluding header so it is adapted  also for frame size
				byteBeginPosition = AdaptToFrame (byteBeginPosition) + 44 ;
				byteEndPosition = AdaptToFrame (byteEndPosition) + 44 ;

				// sets the initial position
				br.BaseStream.Position = 0 ;
				bw.BaseStream.Position = 0 ;

				//copy the bytes from original file to temporary files and skip the part which is  to be deleted
				for (long i = 0 ; i< file.Length; i=i+m_FrameSize)
				{
					if (i == byteBeginPosition)
					{
						i = byteEndPosition   ;
						br.BaseStream.Position = byteEndPosition ;
					}
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}

				// gets the length property of temporary file and update it in header 
				// The privat members representing length are also updated
				//try 
				//{
					FileInfo tempfile = new FileInfo (m_sFilePath + "tmp") ;
				
				//}
				//catch
				//{
//MessageBox.Show	 ("problem in reading lengtyh") ;
				//}
				
				
				m_LengthByte = tempfile.Length	 ;
				//m_LengthByte = m_LengthByte - (byteEndPosition - byteBeginPosition) ;
				m_LengthTime = ConvertByteToTime (m_LengthByte) ;

				UpdateLengthHeader (m_LengthByte, bw);

				/*
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
							*/
				br.Close() ;
				bw.Close() ;

				// Delete the original file and rename the temporary file to name of original file
				FileInfo pfile = new FileInfo (m_sFilePath) ;
				file.Delete () ;
				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				nfile.MoveTo (m_sFilePath) ;

				AudioMediaAsset am = new AudioMediaAsset (m_sFilePath) ;
				return am ;
			}
			else
			{
MessageBox.Show("invalid parameters in DeleteChunk") ;
				AudioMediaAsset am = new AudioMediaAsset (m_sFilePath) ;
				return am ;
			}
		}

		// delete by taking time as parameter
		public 		IAudioMediaAsset DeleteChunk(double timeBeginPosition, double timeEndPosition)
		{
			// convert the time data to byte data and pass it as parameter to original byte function
			long lBeginPos = ConvertTimeToByte (timeBeginPosition) ;
			long lEndPos = ConvertTimeToByte (timeEndPosition) ;

			return DeleteChunk(lBeginPos, lEndPos);
		}

		//Copy audio chunnk in RAM
		// This funtion creates a virtual audio file in RAM with all header information
		public byte [] GetChunk(long byteBeginPosition, long byteEndPosition)
		{
			if (byteBeginPosition < byteEndPosition&& byteEndPosition < m_LengthData)
			{
				byteBeginPosition = AdaptToFrame (byteBeginPosition) + 44 ;
				byteEndPosition = AdaptToFrame (byteEndPosition) + 44 ;

				BinaryReader br = new BinaryReader (File.OpenRead(m_sFilePath)) ;

				// declare byte array  to be returned as chunk
				byte [] arByte = new byte [byteEndPosition - byteBeginPosition + 44] ;

				// copies header
				br.BaseStream.Position = 0 ;
				long i ;
				for (i=0 ; i<44 ; i++  )
				{
					arByte [i] = br.ReadByte () ;
				}

				// copies marked audio chunk from file to byte array
				br.BaseStream.Position = byteBeginPosition ;

				long lCount = byteEndPosition - byteBeginPosition ;
				for (i= i ; i< lCount ; i++)
				{
					arByte [i] = br.ReadByte() ;
				}

				// update length field (4 to 7 )in header
				for (i = 0; i<4 ; i++)
				{

					arByte [4+i ] =(Convert.ToByte (ConvertFromDecimal (m_LengthByte)[i])) ;

				}
				long TempLength = m_LengthByte - 44 ;
				for (i = 0; i<4 ; i++)
				{
				
					arByte [40+i]				= (Convert.ToByte (ConvertFromDecimal (TempLength)[i])) ;

				}

				br.Close () ;
				return arByte ;
			}
			else
			{
MessageBox.Show("invalid parameters") ;
				byte [] b  = new byte [1];
				return b ;
			}
// end function get chunk 
		}


		public byte []  GetChunk(double timeBeginPosition, double timeEndPosition)
		{
			long lBeginPos = ConvertTimeToByte (timeBeginPosition) ;
			long lEndPos = ConvertTimeToByte (timeEndPosition) ;

			return GetChunk(lBeginPos, lEndPos) ;
		}

		public IAudioMediaAsset InsertByteBuffer(byte [] bBuffer, long bytePosition)
		{
//  allow to manipulate only if format  is compatible and parameters are valid
			if (CheckStreamsFormat(bBuffer) == true && bytePosition < m_LengthData)
			{
				bytePosition = AdaptToFrame (bytePosition) + 44 ;

				// opens the original file and creates a temporary file for manipulation
				BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath) );
				br.BaseStream.Position = 0 ;
				BinaryWriter bw = new BinaryWriter(File.Create(m_sFilePath + "tmp")) ;  
				bw.BaseStream.Position = 0 ;

				//copy the bytes before marked position in temporary file
				long lCount = bytePosition ;
				long i ;
				for (i= 0 ; i< lCount ; i = i + m_FrameSize)  
				{
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}


				// copies the audio stream in byte array to temporary file 
				// copied from 44 th byte excluding header
				lCount = bBuffer.LongLength - 44 ;

				for (i= 0 ;i < lCount ; i++) 
				{
					bw.Write (bBuffer [i+44]) ;
				}

				// copies the chunck after insertion position in original file to temp file
				br.BaseStream.Position = bytePosition ;

			
				lCount = m_LengthByte - bytePosition ;

				for (i = 0 ; i< lCount ; i = i +m_FrameSize )
				{
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}

// updates to members and header
				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				m_LengthByte = nfile.Length ;
				m_LengthTime = ConvertByteToTime (m_LengthByte) ;

				UpdateLengthHeader (m_LengthByte, bw) ;
				br.Close() ;
				bw.Close() ;


// deletes original file and rename temp file to original file
				FileInfo pfile = new FileInfo (m_sFilePath) ;
				pfile.Delete () ;

				nfile.MoveTo (m_sFilePath) ;

AudioMediaAsset am = new AudioMediaAsset (m_sFilePath) ;
				return am ;
			}
				// main if statement
			else
			{
				MessageBox.Show ("cannot manipulate . Audio streams are of different format or invalid input parameters are passed") ;	
				AudioMediaAsset am = new AudioMediaAsset (m_sFilePath) ;
				return am ;
			}
			//  end function insert byte position
		}


		public IAudioMediaAsset InsertByteBuffer(byte []  bBuffer, double timePosition)
		{
			long lTimePos = ConvertTimeToByte (timePosition) ;

			return InsertByteBuffer( bBuffer, lTimePos) ;
		}


		void UpdateLengthHeader (long Length, BinaryWriter bw)
		{
m_LengthData = Length - 44 ;
			m_LengthTime = ConvertByteToTime (m_LengthData) ; 
m_lSize = Length ;

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
			return Convert.ToInt64(( dTime * m_SamplingRate  * m_FrameSize)/1000) ;
		}

		double ConvertByteToTime (long lByte)
		{
			long lTemp = (1000 * lByte) / (m_SamplingRate * m_Channels * (m_BitDepth/8)) ;
			return Convert.ToDouble (lTemp) ;
		}

		// compare the format of two streams and return bool 
		public bool CheckStreamsFormat(byte [] bBuffer)
		{
			BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath)) ;

			br.BaseStream.Position = 22 ;

			for (int i = 22 ; i <36 ; i ++)
			{
				if (br.ReadByte() != bBuffer [i])
				{
					br.Close() ;
					return false ;
				}
			}
			br.Close() ;
			return true;
		}

// check the format as above function but for assets instead of byte array
		public bool CheckStreamsFormat(IAudioMediaAsset asset)
		{
			byte [] bBuffer = new byte [44] ;
			BinaryReader brBuffer = new BinaryReader (File.OpenRead(asset.Path)) ;
			brBuffer.BaseStream.Position = 0 ;
			for (int i = 0 ; i< 44 ; i++)
			{
				bBuffer [i] = brBuffer.ReadByte () ;
			}
brBuffer.Close() ;
return CheckStreamsFormat(bBuffer) ;
		}
		
// Phrase detection starts here	
		// function to compute the amplitude of a small chunck of samples


		long BlockSum (BinaryReader br,long Pos, int Block, int FrameSize, int 
			Channels) 
		{
			long sum = 0;
			long SubSum ;
			for (int i = 0 ; i< Block ; i = i + FrameSize)
			{
				br.BaseStream.Position = i+ Pos ;
				SubSum = 0 ;
				if (FrameSize == 1)
				{
					SubSum = Convert.ToInt64((br.ReadByte ()) );
					
					// FrameSize 1 ends
				}
				else if (FrameSize == 2)
				{
					if (Channels == 1)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256 );
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;
					}
					// FrameSize 2 ends
				}
				else if (FrameSize == 4)
				{
					if (Channels == 1)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256)  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256 * 256)  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256 * 256 * 256)  ;
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256)  ;
						// second channel
						SubSum = SubSum + 
							Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + 
							(Convert.ToInt64(br.ReadByte() ) * 256)  ;
					}
					// FrameSize 4 ends
				}
				sum = sum + SubSum ;
				// Outer, For ends
			}
			sum = sum / (Block / FrameSize) ;
			return sum ;
		}


// Detect phrases by taking silent wave file as reference

		public long [] DetectPhrases (IAudioMediaAsset Ref, long PhraseLength , long BeforePhrase) 
		{
			if (CheckStreamsFormat(Ref) == true)
			{

				// adapt values to frame size
				PhraseLength = AdaptToFrame (PhraseLength) ;
				BeforePhrase = AdaptToFrame (BeforePhrase) ;
				BinaryReader brRef = new BinaryReader (File.OpenRead (Ref.Path  )) ;		
				//FileInfo file = new FileInfo (Ref.Path) ;
				//long lSize = file.Length ;
				long lSize = Ref.LengthByte ;
				int Block ;

				if (Ref.SampleRate >22500)
				{
					Block = 96 ;
				}
				else
				{
					Block = 48 ;
				}
				brRef.BaseStream.Position = 44 ;
				long lLargest = 0 ;
				long lBlockSum ;			
lSize = ((lSize / Block)*Block)-4;
				for (int j = 44 ;j < (lSize / Block); j = j + 1)
				{
					lBlockSum = BlockSum(brRef , j , Block, Ref.FrameSize, Ref.Channels) ;	
					if (lLargest < lBlockSum)
					{
						lLargest = lBlockSum ;
					}
				}
			
				long SilVal = Convert.ToInt64(lLargest * 1.01) ;
				brRef.Close () ;


				// Detection starts here

				BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath)) ;		
				//FileInfo file1 = new FileInfo (m_sFilePath) ;
				//lSize = file1.Length ;
				lSize = m_LengthByte ;


				br.BaseStream.Position = 44 ;
				long  lCountSilGap = PhraseLength / Block;
				long lSum = 0 ;
				ArrayList alPhrases = new ArrayList () ;
				long lCheck= 0 ;
lSize = ((lSize / Block) * Block) - 4;
				for (int j = 44 ; j< (lSize / Block); j++)
				{
					lSum = BlockSum (br, (j*Block) + 44, Block, m_FrameSize, m_Channels) ;
					if (lSum < SilVal)
					{
						lCheck ++ ;
					}
					else
					{
						if (lCheck > lCountSilGap)
						{
							
							alPhrases.Add((j * Block) - BeforePhrase) ;
							lCheck = 0 ;
						}
					}

					// end outer For
				}

				br.Close () ;
				long lArraySize = alPhrases.Count ;
				long [] lArray = new long [lArraySize] ;

				for (int i= 0 ; i< lArraySize ; i++)
				{
					lArray [i] = Convert.ToInt64 (alPhrases[i]) ;
				}
			
				return   lArray ;
// end of check wave format
			}
MessageBox.Show("Reference file is of different wave format. An null exception will be thrown") ;
return null; 
			// Phrase detection byte ends
		}

		// phrase detection with respect to time;
		public double [] DetectPhrases (IAudioMediaAsset Ref, double PhraseLength , double BeforePhrase) 
		{
			
long lPhraseLength = ConvertTimeToByte (PhraseLength) ;
			long lBeforePhrase = ConvertTimeToByte (BeforePhrase) ;


return ConvertToTimeArray (DetectPhrases(Ref ,lPhraseLength, lBeforePhrase)) ;

//long [] lTempArray = (DetectPhrases (Ref, lPhraseLength , lBeforePhrase) ); 
//long lCount = lTempArray.LongLength ;
//double [] d = new double [lCount] ;

			//for (long l = 0 ; l < lCount ; l ++)
			//{
//d[l] = ConvertByteToTime(lTempArray [l]) ;
			//}
			
			//return d;
			
		}

		double [] ConvertToTimeArray (long [] lArray)
		{
long lCount = lArray.LongLength ;
			double [] ArrayReturn = new double [lCount] ;
			for (int i = 0 ; i< lCount ; i ++)
			{
ArrayReturn [i] = ConvertByteToTime (lArray [i]) ;
			}
return ArrayReturn ;
		}
// class ends here
	}
}
