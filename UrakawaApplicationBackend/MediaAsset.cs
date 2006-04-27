using System;
using System.IO;
namespace UrakawaApplicationBackend
{
	
	/// <summary>
	/// Summary description for MediaAsset.
	/// </summary>
	public class MediaAsset: IMediaAsset
	{
		public string m_sFilePath ;
		private string m_sFileName ;
		private FileInfo m_flFile ;
		private long m_lSize ;
		private string m_sMediaType ;

//constructor
		public MediaAsset (string sPath)
		{
m_sFilePath = sPath  ;
FileInfo m_flFile = new FileInfo(sPath) ;

m_sFileName = m_flFile .Name ;
			m_lSize = m_flFile .Length ;
m_sMediaType = m_flFile .Extension ;
			
		}

		public string Name
		{
			get
			{
				return m_sFileName  ;
			}
			set
			{
				m_sFileName = value ;
			}
		}

		public string Path 
		{
			get
			{
				return m_sFilePath;
			}
			set
			{
				m_sFilePath = value ;
			}
		}

public 			FileInfo file
		{
			get
			{
return m_flFile ;
	}
		}

		public long SizeInBytes
		{
			get
			{
return m_lSize ;
			}
			set
			{
m_lSize = value ;
			}
		}

		public string MediaType
		{
			get
			{
return m_sMediaType ;
			}
		}

		/*
		/// Validate the asset by performing an integrity check.
		/// <returns>True if the asset was found to be valid, false otherwise.</returns>
		public bool Validate()
			{
					try
			{
				FileInfo FileName = new FileInfo(m_sFilePath);
						m_sFileName = FileName.ToString();
				FileStream fs = new FileStream(m_sFileName, FileMode.Open);
				BinaryReader Reader = new BinaryReader(fs);
						long RiffStartPos = Reader.BaseStream.Position ;
						RiffStartPos = 0;
						long RiffEndPpos = Reader.BaseStream.Position;
						RiffEndPpos = 3;
						string sRiff = string.Empty;
						string rRiff = string.Empty;
						for(long i = RiffStartPos; i<= RiffEndPpos ; i++)
						{
							sRiff = Reader.ReadChar().ToString();
							rRiff = rRiff+sRiff;
						}

						long LenPos = Reader.BaseStream.Position;
						LenPos = 4;
						long LenEndPos = Reader.BaseStream.Position;
						LenEndPos = 7;
						int bLen = 0;
						int brLen = 0;
			
						for(long i = LenPos; i <= LenEndPos ; i ++)
						{
							bLen = Reader.ReadByte();
							brLen = bLen + brLen;
						}

						long WaveStartPos = Reader.BaseStream.Position;
						WaveStartPos = 8;
						long WaveEndPos = Reader.BaseStream.Position;
						WaveEndPos = 11;
						string sWave = string.Empty;
						string rWave = string.Empty;
						for(long i = WaveStartPos; i <= WaveEndPos ; i ++)
						{
							sWave = Reader.ReadChar().ToString();
							rWave = rWave+sWave;
						}

						long fmtStartPos = Reader.BaseStream.Position;
						fmtStartPos = 12;
						long fmtEndPos = Reader.BaseStream.Position;
						fmtEndPos = 15;
						string sfmt = string.Empty;
						string rfmt = string.Empty;
						for(long i = fmtStartPos; i <= fmtEndPos ; i ++)
						{
							sfmt = Reader.ReadChar().ToString();
							sfmt = rfmt+sfmt;
						}

						long fLenStartPos = Reader.BaseStream.Position;
						fLenStartPos = 16;
						long fLenEndPos = Reader.BaseStream.Position;
						fLenEndPos = 19;
						int fLen = 0;
						int rfLen = 0;
						for(long i = fLenStartPos; i <= fLenEndPos ; i ++)
						{
							fLen = Reader.ReadByte();
							rfLen = rfLen + fLen;
						}

						long PadStartPos = Reader.BaseStream.Position;
						PadStartPos = 20;
						long PadEndPos = Reader.BaseStream.Position;
						PadEndPos = 21;
						int pLen = 0;
						int rpLen = 0;
						for(long i = PadStartPos; i <= PadEndPos ; i ++)
						{
							pLen = Reader.ReadByte();
							rpLen = rpLen + pLen;
						}
						MessageBox.Show(rpLen.ToString());

						if(rfLen == 16 || rRiff == "RIFF" && rWave == "WAVE" && 
		rfmt== "fmt" && rpLen==1)
				
		{
		return true;
		}
			else
		{
							fs.Close();
			return false;
		}
	
		}

		catch(Exception ex)
		{
		string Caption = "Validation ";
		MessageBox.Show(ex.ToString(), Caption);
		}
			//End Validate
		}
		*/

// End of Class
		}
	

}
