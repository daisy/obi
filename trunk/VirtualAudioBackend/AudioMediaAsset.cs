using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;


namespace VirtualAudioBackend
{
	public class AudioMediaAsset: MediaAsset, IAudioMediaAsset
	{
		// member variables
//ArrayList is to be finally changed to internal
		public ArrayList m_alClipList = new ArrayList () ;
		private int m_Channels ;
		private int m_BitDepth ;
		private int m_SamplingRate ;
		private int m_FrameSize ;
		private long m_lAudioLengthInBytes = 0 ;
		private double m_dAudioLengthInTime = 0 ;

		private CalculationFunctions Calc = new CalculationFunctions  () ;
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

		public int BitDepth
		{
			get
			{
				return m_BitDepth ;
			}
		}

		internal int FrameSize
		{
			get
			{
				return m_FrameSize ;
			}
		}

		public double LengthInMilliseconds
		{
			get
			{
				return m_dAudioLengthInTime ;
			}
		}

		public long AudioLengthInBytes
		{
			get
			{
				return m_lAudioLengthInBytes ;
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

			m_Channels = channels ;
			m_BitDepth = bitDepth ;
			m_SamplingRate = sampleRate ;
			m_FrameSize = ( m_BitDepth / 8) * m_Channels ;
			m_eMediaType = MediaType.Audio ;
			
		}

		/// <summary>
		/// Constructor for an audio asset from existing clips.
		/// </summary>
		/// <param name="clips">The list of <see cref="AudioClip"/>s.</param>
		public AudioMediaAsset(ArrayList clips)
		{
m_eMediaType = MediaType.Audio ;
			AudioClip ob_AudioClip = clips[0] as AudioClip ;
			m_Channels = ob_AudioClip.Channels ;
			m_BitDepth = ob_AudioClip.BitDepth ;
			m_SamplingRate = ob_AudioClip.SampleRate ;
			m_FrameSize = ob_AudioClip.FrameSize ;
m_alClipList = clips ;
			m_dAudioLengthInTime  = 0 ;

			for (int i = 0 ; i< clips.Count; i++)
			{

				ob_AudioClip = clips [i] as AudioClip ;

				m_dAudioLengthInTime   = m_dAudioLengthInTime   + ob_AudioClip.LengthInTime ;

			}

			m_lAudioLengthInBytes = Calc.ConvertTimeToByte (m_dAudioLengthInTime, m_SamplingRate, m_FrameSize) ;
m_lSizeInBytes = m_lAudioLengthInBytes ;
		}

		/// <summary>
		/// Make a copy of the asset, sharing the same format and data.
		/// </summary>
		/// <returns>The new, identical asset.</returns>
		public override  IMediaAsset  Copy()
		{
			AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset ( this.Channels ,this.BitDepth , this.SampleRate) ;

if (this.Name != null) 
ob_AudioMediaAsset.Name  =Calc.GenerateNewAssetName (ob_AudioMediaAsset) ;

ob_AudioMediaAsset.m_eMediaType = m_eMediaType ;
			for (int i = 0 ; i < this.m_alClipList.Count ; i++ )
				ob_AudioMediaAsset.m_alClipList.Add (  this.m_alClipList [i] );

			ob_AudioMediaAsset.m_FrameSize = m_FrameSize ;
			ob_AudioMediaAsset.m_dAudioLengthInTime = m_dAudioLengthInTime ;
			ob_AudioMediaAsset.m_lAudioLengthInBytes = m_lAudioLengthInBytes ;
			ob_AudioMediaAsset.m_lSizeInBytes = m_lSizeInBytes  ;
			return ob_AudioMediaAsset ;
		}

		
		/// <summary>
		/// Remove the asset from the project, and actually delete all corresponding resources.
		/// Throw an exception if the asset could not be deleted.
		/// </summary>
		public override void Delete()
		{
if (m_AssetManager != null)
m_AssetManager.Assets.Remove (this.Name) ;

if (VirtualAudioBackend.AssetManager.static_htExists.ContainsKey (this.Name))
VirtualAudioBackend.AssetManager.static_htExists.Remove (this.Name) ;

m_alClipList = null ;
m_AssetManager = null ;
m_sName = null;

		}

		public void AppendBytes(byte[] data)
		{
		}

		public IAudioMediaAsset GetChunk(long beginPosition, long endPosition)
		{
			double dBeginTime  = Calc.ConvertByteToTime (beginPosition ,  m_SamplingRate , m_FrameSize) ;
			double dEndTime  = Calc.ConvertByteToTime (endPosition ,  m_SamplingRate , m_FrameSize) ;
			return GetChunk(dBeginTime, dEndTime) ;
		}

		public IAudioMediaAsset GetChunk(double beginTime, double endTime)
		{
ArrayList alNewClipList = new ArrayList () ;

			ArrayList alBeginList = new ArrayList (FindClipToProcess  ( beginTime) );
			int BeginClipIndex = Convert.ToInt32 (alBeginList [0]) ;
			double dBeginTimeMark = Convert.ToDouble(alBeginList [1]) ;

			ArrayList alEndList = new ArrayList (FindClipToProcess  ( endTime) );
			int EndClipIndex = Convert.ToInt32 (alEndList [0]) ;
			double dEndTimeMark = Convert.ToDouble(alEndList [1]) ;

// transfer clip to process to separate object
			AudioClip ob_BeginClip = m_alClipList[BeginClipIndex] as AudioClip ;


			if (BeginClipIndex == EndClipIndex)
			{
				AudioClip ob_NewClip= ob_BeginClip.CopyClipPart (dBeginTimeMark, dEndTimeMark) ;
				alNewClipList.Add (ob_NewClip) ;

			}
			else
			{
			
				AudioClip ob_EndClip = m_alClipList[EndClipIndex] as AudioClip ;

				if (dBeginTimeMark <ob_BeginClip.LengthInTime )
				{
					AudioClip ob_NewBeginClip= ob_BeginClip.CopyClipPart (dBeginTimeMark, ob_BeginClip.LengthInTime) ;
if (ob_NewBeginClip.Equals (null)  ) 
MessageBox.Show ("if (dBeginTimeMark <ob_BeginClip.LengthInTime )") ;

					alNewClipList.Add (ob_NewBeginClip) ;
				}
				

				for (int i = BeginClipIndex + 1 ; i < EndClipIndex ; i ++)
				{
					alNewClipList.Add (m_alClipList [i]) ;
					
				}

				if (dEndTimeMark > 0)
				{

					AudioClip ob_NewEndClip= ob_EndClip.CopyClipPart (0 ,dEndTimeMark) ;

					if (ob_NewEndClip.Equals (null)  ) 
						MessageBox.Show ("if (dEndTimeMark > 0)") ;


					alNewClipList.Add (ob_NewEndClip) ;
				}			

			}

AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset (alNewClipList) ;
/*
			AudioClip ac ;
			for (int i = 0 ; i <ob_AudioMediaAsset.m_alClipList.Count ; i++ )
			{
				ac = ob_AudioMediaAsset.m_alClipList[i] as AudioClip ;
				MessageBox.Show (ac.BeginTime.ToString ()) ;
				MessageBox.Show (ac.EndTime.ToString ()) ;
			}
			*/
			return ob_AudioMediaAsset;
			
		}

		public void InsertAsset(IAudioMediaAsset chunk, long position)
		{
double dPosition = Calc.ConvertByteToTime (position , m_SamplingRate , m_FrameSize) ;
InsertAsset(chunk, dPosition); 
		}

		public void InsertAsset(IAudioMediaAsset chunk, double time)
		{
			if (CompareAudioAssetFormat (this , chunk)== true && time <= m_dAudioLengthInTime && time >= 0)
			{
				AudioMediaAsset ob1 = new AudioMediaAsset ( this.Channels ,this.SampleRate , this.BitDepth) ;
				if (time > - 0 && time < m_dAudioLengthInTime)
				{
					ob1 = GetChunk (0 , time) as AudioMediaAsset;
					ob1.MergeWith (chunk) ;
					AudioMediaAsset ob2 = GetChunk (  time , this.LengthInMilliseconds) as AudioMediaAsset ;
					ob1.MergeWith (ob2) ;


				}
				else if (time==0) 
				{
					ob1 = chunk as AudioMediaAsset;
					ob1.MergeWith (this) ;
				}
				m_alClipList.Clear () ;
				for (int i = 0 ; i < ob1.m_alClipList.Count ; i++	) 
				{
					m_alClipList.Add (ob1.m_alClipList [i]) ;
				}
				m_dAudioLengthInTime = ob1.LengthInMilliseconds ;
				m_lAudioLengthInBytes = ob1.AudioLengthInBytes ;
				m_lSizeInBytes = ob1.SizeInBytes ;

				if (time == m_dAudioLengthInTime)
				{
					MergeWith (chunk) ;
				}

			} // end of main format check
			else
			{
throw new Exception ("Incompatible format or Insertion time not in asset range") ;
			}

				/*
				ArrayList alClipMark = new ArrayList (FindClipToProcess (time)) ;
				int InsertPositionClipIndex = Convert.ToInt32 (alClipMark [0]) ;
				double dInsertionClipTime = Convert.ToDouble (alClipMark [1]) ;

				// copy clips before insertion point to new ArrayList
				ArrayList alNewClipList = new ArrayList () ;

							for (int i = 0 ; i< InsertPositionClipIndex ; i++)
							{
				alNewClipList.Add (m_alClipList[i] ) ;  
							}
				MessageBox.Show ("Clips copied before index") ;
				AudioClip ob_ClipProcessed = m_alClipList [InsertPositionClipIndex] as  AudioClip ;
				bool boolInsertNewClip ;
							AudioClip ob_NewClip ;
							if (dInsertionClipTime == ob_ClipProcessed.LengthInTime)
							{
								ob_NewClip  = ob_ClipProcessed ;
								alNewClipList.Add (ob_ClipProcessed) ;
								boolInsertNewClip  = false ;
							}
							else if ( dInsertionClipTime == 0)
							{
								ob_NewClip = ob_ClipProcessed ;
								boolInsertNewClip  = true ;
							}
							else
							{
				AudioClip TempClip = ob_ClipProcessed.CopyClipPart (0 , dInsertionClipTime) ;
				alNewClipList.Add (TempClip) ;
				ob_NewClip = ob_ClipProcessed.CopyClipPart ( dInsertionClipTime, ob_ClipProcessed.LengthInTime) ;
				boolInsertNewClip  = true ;
							}


							// copy clips from chunk
				AudioMediaAsset ob_Chunk  = chunk as AudioMediaAsset ;
							for (int i = 0 ; i< ob_Chunk.m_alClipList.Count ; i++)
							{
				alNewClipList.Add (ob_Chunk.m_alClipList	[i]) ;
							}

				// insert new clip derived from processed clip
							if (boolInsertNewClip == true) 
							{
				alNewClipList.Add (ob_NewClip) ;
							}

							// Add rest of clips in original asset to new clip list
							for (int i = InsertPositionClipIndex+1 ; i < m_alClipList.Count ; i++) 
							{
				alNewClipList.Add (m_alClipList [i]) ;
							}

				m_alClipList.Clear () ;
							m_alClipList = alNewClipList ;
				m_dAudioLengthInTime = m_dAudioLengthInTime  + ob_Chunk.LengthInMilliseconds ;
				m_lAudioLengthInBytes = Calc.ConvertTimeToByte (m_dAudioLengthInTime , m_SamplingRate , m_FrameSize) ;
				m_lSizeInBytes = m_lAudioLengthInBytes  ;
				*/
				/*
				MessageBox.Show ("ClipList") ;
							AudioClip ac;

							for (int i = 0 ; i < m_alClipList.Count ; i++)
							{
				ac = m_alClipList [i] as AudioClip;
								MessageBox.Show (ac.BeginTime.ToString ()) ;
								MessageBox.Show (ac.EndTime.ToString ()) ;
							}
							*/
				// end of insert chunk function
			}

		public IAudioMediaAsset DeleteChunk(long beginPosition, long endPosition)
		{
double dBeginTime = Calc.ConvertByteToTime (beginPosition , m_SamplingRate , m_FrameSize) ;
			double dEndTime = Calc.ConvertByteToTime (endPosition , m_SamplingRate , m_FrameSize) ;
			return DeleteChunk(dBeginTime , dEndTime) ;
		}

		public IAudioMediaAsset DeleteChunk(double beginTime, double endTime)
		{
AudioMediaAsset ob_NewAsset = GetChunk (beginTime , endTime ) as AudioMediaAsset ; 

			AudioMediaAsset ob_FromtAsset  = new AudioMediaAsset (m_Channels , m_SamplingRate , m_BitDepth) ;
			AudioMediaAsset ob_RearAsset = new AudioMediaAsset (m_Channels , m_SamplingRate , m_BitDepth) ;

			if (beginTime != 0&& endTime != m_dAudioLengthInTime)
			{

				ob_FromtAsset= GetChunk (0 , beginTime)  as AudioMediaAsset;

				ob_RearAsset = GetChunk (endTime , m_dAudioLengthInTime) as AudioMediaAsset ;

				ob_FromtAsset.MergeWith (ob_RearAsset) ;
			}
			else if (beginTime != 0)
			{
ob_FromtAsset = GetChunk (0 , beginTime) as AudioMediaAsset; 
			}
			else if (endTime != m_dAudioLengthInTime)
			{
				ob_FromtAsset= GetChunk (endTime , m_dAudioLengthInTime) as AudioMediaAsset ;
			}

m_alClipList = ob_FromtAsset.m_alClipList ;
m_dAudioLengthInTime = ob_FromtAsset.LengthInMilliseconds ;
			m_lAudioLengthInBytes = ob_FromtAsset.AudioLengthInBytes ;
m_lSizeInBytes = ob_FromtAsset.SizeInBytes ;

ob_FromtAsset = null ;

return ob_NewAsset ;
/*
AudioClip ac ;
			for (int i = 0 ; i <ob_AudioMediaAsset.m_alClipList.Count ; i++ )
			{
ac = ob_AudioMediaAsset.m_alClipList[i] as AudioClip ;
			MessageBox.Show (ac.BeginTime.ToString ()) ;
							MessageBox.Show (ac.EndTime.ToString ()) ;
			}
			return ob_AudioMediaAsset;
			*/
		}

		private ArrayList FindClipToProcess  ( double Time)
		{
if (	Time > m_dAudioLengthInTime || Time < 0)  
throw new Exception ("find clip time is out of bound of Asset time") ;

			AudioClip ob_AudioClip = m_alClipList [0] as AudioClip;
			double TimeSum = 0 ;
			int Count = 0 ;
			while (TimeSum <= Time  && TimeSum < m_dAudioLengthInTime)
			{
				if (Count < m_alClipList.Count )
					ob_AudioClip = m_alClipList [Count] as AudioClip;
				else
				{

					break ;
				}				
				TimeSum = TimeSum + ob_AudioClip.LengthInTime ;
				Count++ ;
			}
			Count-- ;

			
			ob_AudioClip = m_alClipList [Count] as AudioClip;
			double NewClipTime = TimeSum - Time ;

			NewClipTime  = ob_AudioClip.LengthInTime  - NewClipTime   ;
			

ArrayList alReturnList = new ArrayList () ;
alReturnList.Add (Count) ; 


alReturnList.Add (NewClipTime) ; 

return alReturnList  ;
		}

		public override void MergeWith(IMediaAsset next)
		{
			AudioMediaAsset ob_AudioMediaAsset = next as AudioMediaAsset ;
			for (int i = 0 ; i < ob_AudioMediaAsset.m_alClipList.Count ; i++)
			{
m_alClipList.Add (ob_AudioMediaAsset.m_alClipList [i]) ;
			}
			m_dAudioLengthInTime = m_dAudioLengthInTime + ob_AudioMediaAsset.LengthInMilliseconds ;
m_lAudioLengthInBytes = m_lAudioLengthInBytes + ob_AudioMediaAsset.AudioLengthInBytes ;
m_lSizeInBytes = m_lSizeInBytes + ob_AudioMediaAsset.SizeInBytes ;
next = null ;
	}

		public IAudioMediaAsset Split(long position)
		{
			double dTime = Calc.ConvertByteToTime (position , m_SamplingRate , m_FrameSize) ;
			return Split(dTime) ;
		}

		public IAudioMediaAsset Split(double time)
		{
			// create new asset for clips after time specified in parameter
			
			AudioMediaAsset ob_AudioMediaAsset = GetChunk ( time, m_dAudioLengthInTime )  as AudioMediaAsset ;

//// modify original asset
ArrayList alMarksList = new ArrayList (FindClipToProcess  ( time) );
int ClipIndex = Convert.ToInt32 (alMarksList[0]) ;
			 double dClipTimeMark = Convert.ToDouble (alMarksList [1]) ;



			 AudioClip ob_AudioClip = m_alClipList [ClipIndex] as AudioClip ;

			if (dClipTimeMark > 0 && dClipTimeMark  < ob_AudioClip.LengthInTime)
			{
ob_AudioClip.Split (dClipTimeMark) ;
			}
			else if (dClipTimeMark == 0)
			{
ClipIndex-- ;
			}

// Remove clips after clip index
			for (int i = ClipIndex+1 ; i< m_alClipList.Count ; i++)
			{
m_alClipList.Remove (i) ;
			}

m_dAudioLengthInTime = m_dAudioLengthInTime  - ob_AudioMediaAsset.LengthInMilliseconds ;
m_lAudioLengthInBytes = m_lAudioLengthInBytes  - ob_AudioMediaAsset.AudioLengthInBytes ;
			m_lSizeInBytes = m_lAudioLengthInBytes ;

			return ob_AudioMediaAsset  ;

		}

		public long GetSilenceAmplitude(IAudioMediaAsset silenceRef)
		{
AudioMediaAsset ob_AudioMediaSilenceRef = silenceRef as AudioMediaAsset ;
AudioClip Ref= ob_AudioMediaSilenceRef.m_alClipList[0] as AudioClip ;
return Ref.GetClipSilenceAmplitude () ;
/*
			BinaryReader brRef = new BinaryReader (File.OpenRead (Ref.Path  )) ;		
			


			// creates counter of size equal to file size
			long lSize = Ref.EndByte + 44 ;

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
			brRef.BaseStream.Position = 44 + Ref.BeginByte;
			long lLargest = 0 ;
			long lBlockSum ;			

			// adjust the  lSize to avoid reading beyond file length
			lSize = ((lSize / Block)*Block)-4;

			// loop to end of file reading collective value of  samples in Block and determine highest value denoted by lLargest
			// Step size is the Block size
			for (long j = 44 + Ref.BeginByte;j < (lSize / Block); j = j + Block)
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
*/			
		}

		public ArrayList ApplyPhraseDetection(long threshold, long length, long before)
		{
double dLength = Calc.ConvertByteToTime ( length , m_SamplingRate , m_FrameSize ) ;
			double dBefore = Calc.ConvertByteToTime ( before , m_SamplingRate , m_FrameSize ) ;

			return ApplyPhraseDetection(threshold, dLength , dBefore) ;
		}

		public ArrayList ApplyPhraseDetection(long threshold, double length, double before)
		{
			
long lLength = Calc.ConvertTimeToByte (length , m_SamplingRate, m_FrameSize) ;
			long lBefore = Calc.ConvertTimeToByte (before , m_SamplingRate , m_FrameSize) ;


AudioClip ob_Clip ;
ArrayList alAssetList= new ArrayList() ;
ArrayList alClipList ; 
AudioMediaAsset ob_Asset =new AudioMediaAsset ( m_Channels , m_SamplingRate , m_BitDepth) ;

// apply phrase detection on each clip in clip list of this asset
			for (int i = 0 ; i < m_alClipList.Count ; i++)
			{
ob_Clip = m_alClipList [i] as AudioClip ;
				alClipList = ob_Clip.DetectPhrases(threshold , lLength , lBefore) ;
MessageBox.Show (alClipList.Count.ToString () + "Clip Count") ;
				if (Convert.ToBoolean (alClipList [0]) == false)
				{
MessageBox.Show ("bool is False") ;
					ob_Asset.AddClip (alClipList [1] as AudioClip) ;
				}
				else
				{
MessageBox.Show ("bool is true") ;
ob_Asset.AddClip (alClipList [1] as AudioClip) ;
alAssetList.Add (ob_Asset) ;
MessageBox.Show ("Asset Added before loop") ;
					for (int j = 2 ; j< alClipList.Count ; j++)
					{
						ob_Asset = new AudioMediaAsset (m_Channels , m_SamplingRate , m_BitDepth) ;
ob_Asset.AddClip (alClipList [j] as AudioClip) ;
alAssetList.Add (ob_Asset) ;
MessageBox.Show ("Asset added") ;
					}
					//ob_Asset = new AudioMediaAsset (m_Channels , m_SamplingRate , m_BitDepth) ;
					//ob_Asset.AddClip (alClipList [alClipList.Count- 1 ] as AudioClip) ;

					if (i == m_alClipList.Count - 1)
					{
//alAssetList.Add(ob_Asset) ;
					}
				} // bool if ends

			}


return alAssetList ;
/*
double dClipFromOrigin = 0 ;
double dPhraseFromOrigin = 0 ;


			for (int i = 0 ; i < m_alClipList.Count ; i++)
			{
				
ob_Clip = m_alClipList [i] as AudioClip ;
				alClipPhraseList = new ArrayList (ob_Clip.DetectPhrases( threshold , lLength , lBefore )  );
MessageBox.Show ("In clip loop") ;
				if (alClipPhraseList != null)
				{
					for (int j = 0 ; j < alClipPhraseList.Count ; j++)
					{

dPhraseFromOrigin = Calc.ConvertByteToTime( Convert.ToInt64(alClipPhraseList [j]), ob_Clip.SampleRate , ob_Clip.FrameSize)	+ dClipFromOrigin ;
	alPhraseList.Add (dPhraseFromOrigin) ;

					}
					
				}
				dClipFromOrigin  = dClipFromOrigin  + ob_Clip.LengthInTime ;
			} // end of copy ArrayList

double dAssetBeginTime ;
			double dAssetEndTime ;
ArrayList alReturnAssetList = new ArrayList () ;

			for (int i = 0 ; i < alPhraseList.Count ; i++)
			{

dAssetBeginTime = Convert.ToDouble (alPhraseList [i]) ;

if (i < alPhraseList.Count  - 1)
				dAssetEndTime = Convert.ToDouble (alPhraseList [i+1]) ; 
				else
dAssetEndTime  = m_dAudioLengthInTime ;
MessageBox.Show ("Before get chunk") ;
				MessageBox.Show (dAssetBeginTime .ToString ()) ;
				MessageBox.Show (dAssetEndTime.ToString ()) ;
alReturnAssetList.Add ( GetChunk (dAssetBeginTime , dAssetEndTime)) ; 
MessageBox.Show ("Asset created") ;
				
			}
			return alReturnAssetList ;
*/			

		}

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
						SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256 );						SubSum = (SubSum * 256)/65792 ;
					}
					else if (Channels == 2)
					{
						SubSum = Convert.ToInt64(br.ReadByte() )  ;
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;SubSum = SubSum/2 ;
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
						SubSum = SubSum + Convert.ToInt64(br.ReadByte() )  ;												SubSum = SubSum + (Convert.ToInt64(br.ReadByte() ) * 256)  ;						
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


		public void AddClip (AudioClip Clip )
		{
m_alClipList.Add (Clip) ;
m_dAudioLengthInTime = m_dAudioLengthInTime + Clip.LengthInTime ;
m_lAudioLengthInBytes = Calc.ConvertTimeToByte ( m_dAudioLengthInTime , m_SamplingRate , m_FrameSize) ;
m_lSizeInBytes = m_lAudioLengthInBytes ;
		}

		private bool CompareAudioAssetFormat (IAudioMediaAsset asset1 , IAudioMediaAsset asset2)
		{
if ( asset1.Channels == asset2.Channels && asset2.SampleRate == asset2.SampleRate && asset1.BitDepth == asset2.BitDepth)
	return true ;
			else
	return false ;

		}

	}// end of class
}