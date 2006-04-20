using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;


namespace UrakawaApplicationBackend
{
	
	public 	class AssetManager :  IAssetManager 
	{	
	
		public string m_sDirPath;

		Hashtable m_htAssetList = new Hashtable();


		public Hashtable Assets
		{
			get
			{
				return m_htAssetList ;		
			}
		}
/*
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
		}*/

		
			public Hashtable GetAssets(Type assetType)
		{
			DirectoryInfo dir = new DirectoryInfo(m_sDirPath);
			FileInfo[] bmpfiles = dir.GetFiles(assetType.ToString()) ;
			Hashtable htTemp = new Hashtable () ;
			
			foreach( FileInfo f in bmpfiles)
			{
				htTemp.Add(f.Name, f.FullName);
			}

			
			return htTemp ;
			}

		public void DeleteAsset(IMediaAsset assetToDelete)
		{
			
			FileInfo m_file= new FileInfo	 (assetToDelete.Path);
			if ( m_file.Exists && m_htAssetList.ContainsValue(assetToDelete.Path) )
			{
			m_htAssetList.Remove(assetToDelete.Name) ;
				m_file.Delete();
		
			}
			else
			{
				MessageBox.Show("File could not be deleted");
			}
		}


		//parameter string newName must contain extension of file also like abc.wav

public IMediaAsset RenameAsset(IMediaAsset asset, String newName)
		{
			FileInfo file= new FileInfo (asset.Path);		
			string sTemp = file.DirectoryName + "\\" + newName ;

			if (file.Exists && m_htAssetList.ContainsValue(asset.Path)  )
			{
				try
				{
					file.MoveTo (sTemp);
				}
				catch
				{
					MessageBox.Show ("Cannot rename the file");
				}
					
m_htAssetList.Remove(asset.Name) ;


				asset.Path = sTemp ;
				FileInfo file1 = new FileInfo (asset.Path);		
				m_htAssetList.Add(file1.Name, file1.FullName ) ;
				return asset ;
			}
			else
			{
				MessageBox.Show("Original file not found");
				return null;
			}
		}

		public IMediaAsset NewAsset(string assetType)
		{
			string sTemp = GenerateFileName (assetType, m_sDirPath);

			if (sTemp != null)
			{
				File.Create (sTemp) ;
				MediaAsset m= new MediaAsset (sTemp) ;
				return m ;
			}
			else
			{
return null ;
			}
					}

public 		IMediaAsset AddAsset(string assetType, string assetPath)
		{
			string sTemp = GenerateFileName (assetType, assetPath);

			if (sTemp != null)
			{
				File.Create (sTemp) ;

				MediaAsset m= new MediaAsset (sTemp) ;
m_htAssetList.Add(m.Name, m.Path) ; 
				return m ;
			}
			else
			{
				return null ;
			}
		}


		string GenerateFileName (string ext, string sDir)
				{
int i = 0 ;
			string sTemp ;
sTemp = sDir + "\\" + i.ToString() + "." + ext ;
//FileInfo file = new FileInfo(sTemp) ;

					while (File.Exists(sTemp) && i<10000)
					{
i++;
						sTemp = m_sDirPath + "\\" + i.ToString() + ".wav" ;

					}

			if (i<10000)
			{
				return sTemp ;
			}
			else
			{
return null ;
			}

				}

/*
		public IMediaAsset copyAsset(IMediaAsset asset, IMediaAsset dest, bool replaceIfExisting)
		{
			FileInfo file= new FileInfo (asset.FilePath);		
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
		}*/
	}
}
