
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

		protected double m_LengthTime ;
		//protected long m_lSize  ;
		protected int m_Channels ;
		protected int m_SamplingRate ;
		protected int m_FrameSize ;
		protected int m_BitDepth ;
		protected long m_AudioLengthBytes;

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
/*
			// length in bytes
			br.BaseStream.Position = 4 ;

			for (int i = 0; i<4 ; i++)
			{
				Ar [i] = Convert.ToInt32 (br.ReadByte()) ;

			}
			m_lSize   = ConvertToDecimal (Ar) ;
*/

			// AudioLengthBytes

			br.BaseStream.Position = 40 ;

			for (int i = 0; i<4 ; i++)
			{
				Ar [i] = Convert.ToInt32(br.ReadByte() );

			}
			m_AudioLengthBytes = ConvertToDecimal (Ar) ;

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
			m_LengthTime = Convert.ToDouble ((m_AudioLengthBytes * 1000)/ (m_SamplingRate * m_FrameSize));

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
/*
		public long LengthByte
		{
			get
			{
				return m_lSize  ;
			}
			set
			{
				m_lSize  = value ;
			}
		}
*/
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


		public long AudioLengthBytes
		{
			get
			{
return m_AudioLengthBytes;
			}
			set
			{
m_AudioLengthBytes= value ;
			}
		}
// deletes a section of audio streamfrom physical asset by takin byte position as parameter
		public  void DeleteChunk(long byteBeginPosition, long byteEndPosition)
		{
			// checks for valid parameters
			if (byteBeginPosition < byteEndPosition && byteEndPosition < m_AudioLengthBytes )
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



				br.Close() ;
				bw.Close() ;

				// Delete the original file and rename the temporary file to name of original file
				FileInfo pfile = new FileInfo (m_sFilePath) ;
				file.Delete () ;
				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				nfile.MoveTo (m_sFilePath) ;

				// gets the length property of temporary file and update it in header 
				// The privat members representing length are also updated
				

					FileInfo tempfile = new FileInfo (m_sFilePath ) ;
				
				
				m_lSize  = tempfile.Length	 ;
m_AudioLengthBytes = m_lSize - 44 ;
				m_LengthTime = ConvertByteToTime (m_AudioLengthBytes) ;


BinaryWriter bw1 = new BinaryWriter (File.OpenWrite(m_sFilePath)) ;
				UpdateLengthHeader (m_lSize , bw1);
bw1.Close() ;
				
				// end of check if
			}
			else
			{
MessageBox.Show("invalid parameters in DeleteChunk") ;
				
			}
		}

		// delete by taking time as parameter
		public 		void DeleteChunk(double timeBeginPosition, double timeEndPosition)
		{
			// convert the time data to byte data and pass it as parameter to original byte function
			long lBeginPos = ConvertTimeToByte (timeBeginPosition) ;
			long lEndPos = ConvertTimeToByte (timeEndPosition) ;

			DeleteChunk(lBeginPos, lEndPos);
		}

		//Copy audio chunnk in RAM
		// This funtion creates a virtual audio file in RAM with all header information
		public byte [] GetChunk(long byteBeginPosition, long byteEndPosition)
		{
			if (byteBeginPosition < byteEndPosition&& byteEndPosition < m_AudioLengthBytes)
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

					arByte [4+i ] =(Convert.ToByte (ConvertFromDecimal (m_lSize )[i])) ;

				}
				long TempLength = m_lSize  - 44 ;
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

		public void InsertByteBuffer(byte [] bBuffer, long bytePosition)
		{
//  allow to manipulate only if format  is compatible and parameters are valid
			if (CheckStreamsFormat(bBuffer) == true && bytePosition < m_AudioLengthBytes )
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

			
				lCount = m_lSize  - bytePosition ;

				for (i = 0 ; i< lCount ; i = i +m_FrameSize )
				{
					bw.Write(br.ReadBytes(m_FrameSize)) ;
				}


				br.Close() ;
				bw.Close() ;


				// deletes original file and rename temp file to original file
				FileInfo pfile = new FileInfo (m_sFilePath) ;
				pfile.Delete () ;


				
				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				nfile.MoveTo (m_sFilePath) ;

// updates to members and header
				FileInfo nefile = new FileInfo (m_sFilePath ) ;
				m_lSize  = nefile.Length ;
				


				BinaryWriter bw1 = new BinaryWriter(File.OpenWrite(m_sFilePath )) ;  
				UpdateLengthHeader (m_lSize , bw1) ;
bw1.Close() ;


				
			}
				// main if statement
			else
			{
				MessageBox.Show ("cannot manipulate . Audio streams are of different format or invalid input parameters are passed") ;	
				

			}
			//  end function insert byte position
		}


		public void InsertByteBuffer(byte []  bBuffer, double timePosition)
		{
			long lTimePos = ConvertTimeToByte (timePosition) ;

			InsertByteBuffer( bBuffer, lTimePos) ;
		}


		void UpdateLengthHeader (long Length, BinaryWriter bw)
		{
m_AudioLengthBytes= Length - 44 ;
			m_LengthTime = ConvertByteToTime (m_AudioLengthBytes) ; 
m_lSize = Length ;

			// update length field (4 to 7 )in header
			for (int i = 0; i<4 ; i++)
			{
				bw.BaseStream.Position = i + 4 ;
				bw.Write (Convert.ToByte (ConvertFromDecimal (Length)[i])) ;

			}
			long TempLength = AudioLengthBytes ;
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
			long lTemp = (1000 * lByte) / (m_SamplingRate *m_FrameSize ) ;
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
		
// Phrase detection functions starts here	
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
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256 );						SubSum = (SubSum * 256)/65792 ;
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;SubSum = SubSum/2 ;
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
						
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256)  ;
							
						// second channel
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;												SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256)  ;						
SubSum = (SubSum * 256 ) / (65792  * 2);
						
					}
					// FrameSize 4 ends
				}
				sum = sum + SubSum ;
				

				// Outer, For ends
			}
			
			
			
			sum = sum / (Block / FrameSize) ;

	//MessageBox.Show(sum.ToString()) ;
			return sum ;
		}

		public long GetSilenceAmplitude (IAudioMediaAsset Ref) 
		{
			BinaryReader brRef = new BinaryReader (File.OpenRead (Ref.Path  )) ;		
			//FileInfo file = new FileInfo (Ref.Path) ;
			//long lSize = file.Length ;

			// creates counter of size equal to file size
			long lSize = Ref.SizeInBytes ;

			// Block size of audio chunck which is least count of detection
			int Block ;

			// determine the Block  size
			if (Ref.SampleRate >22500)
			{
				Block = 96 ;
			}
			else
			{
				Block = 48 ;
			}

			//set reading position after the header
			brRef.BaseStream.Position = 44 ;
			long lLargest = 0 ;
			long lBlockSum ;			

			// adjust the  lSize to avoid reading beyond file length
			lSize = ((lSize / Block)*Block)-4;

			// loop to end of file reading collective value of  samples in Block and determine highest value denoted by lLargest
			// Step size is the Block size
			for (int j = 44 ;j < (lSize / Block); j = j + Block)
			{
				//  BlockSum is function to retrieve average amplitude in  Block
				lBlockSum = BlockSum(brRef , j , Block, Ref.FrameSize, Ref.Channels) ;	
				if (lLargest < lBlockSum)
				{
					lLargest = lBlockSum ;
				}
			}
			long SilVal = Convert.ToInt64(lLargest );
			SilVal = SilVal + 4 ;
			brRef.Close () ;



return SilVal ;
		}


// Detect phrases by taking silent wave file as reference

		public long [] DetectPhrases (long SilVal, long PhraseLength , long BeforePhrase) 
		{

				// adapt values to frame size
				PhraseLength = AdaptToFrame (PhraseLength) ;
				BeforePhrase = AdaptToFrame (BeforePhrase) ;

				// Block size of audio chunck which is least count of detection
				int Block ;

// determine the Block  size
				if (m_SamplingRate  >22500)
				{
					Block = 96 ;
				}
				else
				{
					Block = 48 ;
				}


				// Detection starts here

				BinaryReader br = new BinaryReader (File.OpenRead (m_sFilePath)) ;		
				//FileInfo file1 = new FileInfo (m_sFilePath) ;
				//lSize = file1.Length ;

// Gets the count of file size
				long lSize = m_lSize  ;


				br.BaseStream.Position = 44 ;

// count chunck of silence which trigger phrase detection
				long  lCountSilGap = PhraseLength / Block;
				long lSum = 0 ;
				ArrayList alPhrases = new ArrayList () ;
				long lCheck= 0 ;

// adjustment to prevent end of file exception
lSize = ((lSize / Block) * Block) - 4;

// scanning of file starts
				for (int j = 44 ; j< (lSize / Block); j++)
				{
// decodes audio chunck inside block
					lSum = BlockSum (br, (j*Block) + 44, Block, m_FrameSize, m_Channels) ;

// conditional triggering of phrase detection
					if (lSum < SilVal)
					{
						lCheck ++ ;
					}
					else
					{
						// checks the length of silence
						if (lCheck > lCountSilGap)
						{
							
							alPhrases.Add((j * Block) - BeforePhrase) ;
							lCheck = 0 ;
						}
					}

					// end outer For
				}


				br.Close () ;

// converts ArrayList to long array and return
				long lArraySize = alPhrases.Count ;
				long [] lArray = new long [lArraySize] ;

				for (int i= 0 ; i< lArraySize ; i++)
				{
					lArray [i] = Convert.ToInt64 (alPhrases[i]) ;
				}
			
				return   lArray ;

			// Phrase detection byte ends
		}

		// phrase detection with respect to time;
		public double [] DetectPhrases (long SilVal, double PhraseLength , double BeforePhrase) 
		{
			
long lPhraseLength = ConvertTimeToByte (PhraseLength) ;
			long lBeforePhrase = ConvertTimeToByte (BeforePhrase) ;


return ConvertToTimeArray (DetectPhrases(SilVal,lPhraseLength, lBeforePhrase)) ;

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



		// The function opens two files (target file and source file)simultaneously  and copies from one to other frame by frame
		//This is done to reduce load on system memory as wave file may be as big as it go out of scope of RAM
		// IAudioMediaAsset Target is destination file where the stream has to be inserted and  TargetBytePos is insertion position in bytes excluding header
		//IAudioMediaAsset Source is asset from which data is taken between Positions in bytes (StartPos and EndPos) , these are also excluding header length
		public void InsertAudio ( long TargetBytePos, IAudioMediaAsset Source, long StartPos, long EndPos)
		{
			// checks the compatibility of formats of two assets and validity of parameters
			if (CheckStreamsFormat (Source)== true&& TargetBytePos < (m_AudioLengthBytes ) && StartPos < EndPos && EndPos < Source.AudioLengthBytes )
				// braces holds whole  function
			{
				// opens Target asset and Source asset for reading and create temp file for manipulation
				BinaryReader brTarget = new BinaryReader (File.OpenRead(m_sFilePath))  ;
				
				BinaryReader brSource = new BinaryReader (File.OpenRead(Source.Path ))  ;

				BinaryWriter bw = new BinaryWriter (File.Create(m_sFilePath + "tmp")) ;

				// adapt positions to frame size
				TargetBytePos = AdaptToFrame (TargetBytePos) + 44; 
				StartPos = Source.AdaptToFrame(StartPos) + 44;
				EndPos = Source.AdaptToFrame (EndPos) + 44;
				int Step = FrameSize ;

				// copies audio stream before the insertion point into temp file
				bw.BaseStream.Position = 0 ;
				brTarget.BaseStream.Position = 0 ;
				long lCount = TargetBytePos ;
				long i = 0 ;
				for (i = 0 ; i < lCount ; i=i+Step) 
				{
					bw.Write(brTarget.ReadBytes(Step)) ;

				}

				// copies the audio stream data (between marked positions) from scource file  to temp file
				brSource.BaseStream.Position = StartPos ;
				lCount = lCount + (EndPos - StartPos) ;
				for (i = i ; i< lCount ; i= i +Step)
				{
					bw.Write(brSource.ReadBytes(Step)) ;
				}

				// copies the remaining data after insertion point in Target asset into temp file
				FileInfo file = new FileInfo (m_sFilePath) ;
				lCount = file.Length - 44+ (EndPos - StartPos);

				brTarget.BaseStream.Position = TargetBytePos;

				for (i = i; i< lCount; i= i+Step)
				{
					try
					{
						bw.Write(brTarget.ReadBytes(Step)) ;
					}
					catch
					{
						MessageBox.Show("Problem   "+ i) ;
					}
				}


				// close all the reading and writing objects
				brTarget.Close() ;
				brSource.Close() ;
				bw.Close() ;

				//  Delete the target file and rename the temp file to target file
				FileInfo pfile = new FileInfo (m_sFilePath) ;
				pfile.Delete () ;

				FileInfo nfile = new FileInfo (m_sFilePath + "tmp") ;
				nfile.MoveTo (m_sFilePath);


				// Update lenth fields in header of temp file and also members of Target asset
				FileInfo  filesize = new FileInfo (m_sFilePath) ;
				m_lSize= filesize.Length;
				
BinaryWriter bw1 = new BinaryWriter (File.OpenWrite(m_sFilePath )) ;
				UpdateLengthHeader (m_lSize, bw1) ;
				bw1.Close() ;
			}
			else
			{
				MessageBox.Show("Can not manipulate. files are of different format or invalid input parameters passed") ;
				
			}

			// End insert  function
		}


		// same as above function but take time position as parameter instead of byte position
		public void InsertAudio (double timeTargetPos, IAudioMediaAsset Source, double timeStartPos, double timeEndPos)
		{
			long lTargetPos = ConvertTimeToByte (timeTargetPos) ;
			long lStartPos = ConvertTimeToByte (timeStartPos) ;
			long lEndPos = ConvertTimeToByte (timeEndPos) ;

			InsertAudio ( lTargetPos, Source, lStartPos, lEndPos) ;
		}


// class ends here
	}
}
