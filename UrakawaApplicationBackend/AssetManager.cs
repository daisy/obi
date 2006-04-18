using System;

namespace UrakawaApplicationBackend
{
	/*
	public 	class AssetManager :  IAssetManager 
	{	
	
		public string m_sDirPath;

		public ArrayList getAssets(string sDirPath, string sFilter)
		{
			DirectoryInfo dir = new DirectoryInfo(sDirPath);
			FileInfo[] bmpfiles = dir.GetFiles(sFilter) ;
			ArrayList m_alTemp = new ArrayList () ;
			
			foreach( FileInfo f in bmpfiles)
			{
				m_alTemp.Add(f.FullName);
			}
			return m_alTemp;
		}
		public ArrayList getAssets(Object assetType)
		{
			ArrayList alTemp = null ;
			return alTemp;

			
			

			
				
			

		}

		public void deleteAsset(IMediaAsset assetToDelete)
		{
			
			FileInfo m_file= new FileInfo	 (assetToDelete.FilePath);
			if ( m_file.Exists)
			{
				m_file.Delete();
			}
			else
			{
				MessageBox.Show("File could not be deleted");
			}
		}

		//parameter string newName must contain extension of file also like abc.wav
		public IMediaAsset renameAsset(IMediaAsset source, String newName)
		{
			FileInfo file= new FileInfo (source.FilePath);		
			string sTemp = file.DirectoryName + "\\" + newName ;
			if (file.Exists)
			{
				try
				{
					file.MoveTo (sTemp);
				}
				catch
				{
					MessageBox.Show ("Cannot rename the file");
				}
					
				source.FilePath = sTemp ;
				return source ;
			}
			else
			{
				MessageBox.Show("Original file not found");
				return null;
			}
		}

		public IMediaAsset copyAsset(IMediaAsset source, IMediaAsset dest, bool replaceIfExisting)
		{
			FileInfo file= new FileInfo (source.FilePath);		
			try
			{
				file.CopyTo(dest.FilePath,replaceIfExisting);
			}
			catch
			{
				MessageBox.Show ("Can not copy the file");
			}
			return dest;
		}
		public void test ()
		{
			MessageBox.Show("It is working");
		}
	}
	*/
}
