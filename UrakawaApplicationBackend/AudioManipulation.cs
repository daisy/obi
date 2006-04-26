using System;
using System.IO;
using System.Windows.Forms;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// Summary description for AudioManupulation.
	/// </summary>
	public class AudioManipulation
	{

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


// The function opens two files (target file and source file)simultaneously  and copies from one to other frame by frame
//This is done to reduce load on system memory as wave file may be as big as it go out of scope of RAM
		// IAudioMediaAsset Target is destination file where the stream has to be inserted and  TargetBytePos is insertion position in bytes excluding header
		//IAudioMediaAsset Source is asset from which data is taken between Positions in bytes (StartPos and EndPos) , these are also excluding header length
		public void InsertAudio (IAudioMediaAsset Target, long TargetBytePos, IAudioMediaAsset Source, long StartPos, long EndPos)
		{
			// checks the compatibility of formats of two assets and validity of parameters
			if (Target.CheckStreamsFormat (Source)== true&& TargetBytePos < (Target.LengthData ) && StartPos < EndPos && EndPos < Source.LengthData)
				// braces holds whole  function
			{
// opens Target asset and Source asset for reading and create temp file for manipulation
				BinaryReader brTarget = new BinaryReader (File.OpenRead(Target.Path ))  ;
				BinaryReader brSource = new BinaryReader (File.OpenRead(Source.Path ))  ;

				BinaryWriter bw = new BinaryWriter (File.Create(Target.Path + "tmp")) ;

// adapt positions to frame size
				TargetBytePos = Target.AdaptToFrame (TargetBytePos) + 44; 
				StartPos = Source.AdaptToFrame(StartPos) + 44;
				EndPos = Source.AdaptToFrame (EndPos) + 44;
				int Step = Target.FrameSize ;

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
				FileInfo file = new FileInfo (Target.Path) ;
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

				// Update lenth fields in header of temp file and also members of Target asset
				FileInfo  filesize = new FileInfo (Target.Path + "tmp") ;
				lCount = filesize.Length;
				Target.LengthByte = lCount ;
				Target.LengthInMilliseconds = ConvertByteToTime (lCount, Target) ;
Target.LengthData = Target.LengthByte - 44 ;
				// update length field (4 to 7 )in header
				for ( i = 0; i<4 ; i++)
				{
					bw.BaseStream.Position = i + 4 ;
					bw.Write (Convert.ToByte (ConvertFromDecimal (lCount)[i])) ;

				}
				long TempLength = lCount - 44 ;
				for ( i = 0; i<4 ; i++)
				{
					bw.BaseStream.Position = i + 40 ;
					bw.Write (Convert.ToByte (ConvertFromDecimal (TempLength)[i])) ;

				}			

// close all the reading and writing objects
				brTarget.Close() ;
				brSource.Close() ;
				bw.Close() ;

//  Delete the target file and rename the temp file to target file
				FileInfo pfile = new FileInfo (Target.Path ) ;
				pfile.Delete () ;

				FileInfo nfile = new FileInfo (Target.Path + "tmp") ;
				nfile.MoveTo (Target.Path);
			}
			else
			{
MessageBox.Show("Two files are of different format. Can not manipulate them or invalid parameters") ;
			}

// End insert  function
			}

// same as above function but take time position as parameter instead of byte position
		public void InsertAudio (IAudioMediaAsset Target, double timeTargetPos, IAudioMediaAsset Source, double timeStartPos, double timeEndPos)
		{
long lTargetPos = ConvertTimeToByte (timeTargetPos, Target) ;
			long lStartPos = ConvertTimeToByte (timeStartPos, Target) ;
long lEndPos = ConvertTimeToByte (timeEndPos, Target) ;

			InsertAudio (Target , lTargetPos, Source, lStartPos, lEndPos) ;
		}


		long ConvertTimeToByte (double dTime, IAudioMediaAsset pass)
		{
double dTemp =(dTime * pass.SampleRate * pass.Channels* (pass.BitDepth/ 8))/1000 ;
			return Convert.ToInt64(dTemp) ;
		}			


		double ConvertByteToTime (long lByte, IAudioMediaAsset pass)
		{
			long lTemp = (1000 * lByte) / (pass.SampleRate* pass.FrameSize) ;
			return Convert.ToDouble (lTemp) ;
		}
			
		// End of Class
	}
}
