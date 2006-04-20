using System;
using System.IO;
namespace UrakawaApplicationBackend
{
	
	/// <summary>
	/// Summary description for MediaAsset.
	/// </summary>
	public class MediaAsset: IMediaAsset
	{
		private string m_sFilePath ;
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
		}

		public string MediaType
		{
			get
			{
return m_sMediaType ;
			}
		}


		}
	

}
