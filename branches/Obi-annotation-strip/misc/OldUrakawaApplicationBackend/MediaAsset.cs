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
		protected string m_sFileName ;
		private FileInfo m_flFile ;
		protected long m_lSize ;
		private TypeOfMedia m_MediaType ;

		//constructor
		public MediaAsset (string sPath)
		{
			m_sFilePath = sPath  ;
			FileInfo m_flFile = new FileInfo(sPath) ;

			m_sFileName = m_flFile .Name ;
			m_lSize = m_flFile .Length ;

			m_MediaType = 			GetMediaType (m_flFile .Extension ) ;
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

		public TypeOfMedia  MediaType
		{
			get
			{
				return m_MediaType ;
			}
		}

		TypeOfMedia GetMediaType(string sExt)
		{
			if (sExt == ".wav" || sExt == ".mp3")
				return TypeOfMedia.Audio ;
			else if (sExt == ".txt")
				return TypeOfMedia.Text ;
			else
				return TypeOfMedia.Unknown ;
		}

		public  void Delete()
		{

		}

		public virtual IMediaAsset MergeWith(IMediaAsset next)
		{
			return null;
		}
	}


	// End of Class
}
	


