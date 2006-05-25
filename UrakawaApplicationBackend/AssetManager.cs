using System;
using System.IO;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using UrakawaApplicationBackend.events.assetManagerEvents ;

namespace UrakawaApplicationBackend
{
	
	public 	class AssetManager :  IAssetManager 
	{	
	
// string to hold path of project directory
		private string m_sDirPath;

//hash table to hold paths of assets being managed
		public Hashtable m_htAssetList = new Hashtable();

//static counter to implement singleton
private static int m_ConstructorCounter =0;

CatchEvents ob_CatchEvents = new CatchEvents () ;

//constructor
		public AssetManager (string sProjectDir)
		{
			if (m_ConstructorCounter == 0)
			{
				m_sDirPath = sProjectDir ;
				m_ConstructorCounter++ ;
			}
			else
			{
throw new Exception("This class is Singleton");
				
				}
			}
		

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

		//returns hashtable containing pat of assets from project directory
		// Parameter should be of extension of file e.g. ".wav"
			public Hashtable GetAssets(string assetType)
		{
			DirectoryInfo dir = new DirectoryInfo(m_sDirPath);
				string sTemp = "*" + assetType ;
			FileInfo[] bmpfiles = dir.GetFiles(sTemp) ;
			Hashtable htTemp = new Hashtable () ;
			
			foreach( FileInfo f in bmpfiles)
			{
				htTemp.Add(f.Name, f.FullName);
			}

			return htTemp ;
			}

//Delete asset from Project directory and hash table
		public void DeleteAsset(IMediaAsset assetToDelete)
		{
			
			FileInfo m_file= new FileInfo	 (assetToDelete.Path);
			if ( m_file.Exists && m_htAssetList.ContainsValue(assetToDelete.Path) )
			{
			m_htAssetList.Remove(assetToDelete.Name) ;

				try
				{
					m_file.Delete();
				}
				catch
				{
MessageBox.Show("Error in deleting file") ;
				}
		
AssetDeleted ob_AssetDeleted = new AssetDeleted (assetToDelete) ;
ob_AssetDeleted.AssetDeletedEvent+=new DAssetDeletedEvent (ob_CatchEvents.CatchAssetDeletedEvent ) ;
ob_AssetDeleted.NotifyAssetDeleted ( this , ob_AssetDeleted) ;
			}
			else
			{
				MessageBox.Show("File could not be deleted");
			}
		}


		//parameter string assetName must contain extension of file also like abc.wav
string m_sTempPath ;
		public IMediaAsset GetAsset(string assetName)
		{
			
			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();
			
			string  sTemp ;
			m_sTempPath = null ;

//find the path from hash table using key 
				while (en.MoveNext() )
				{
					if (assetName == en.Key.ToString() )
					{
sTemp = en.Value.ToString() ;
						m_sTempPath= sTemp ;
						
break ;
					}
					
				}
//if found, create object using path and return
			if (m_sTempPath != null)
			{
				MediaAsset ob =new MediaAsset  (m_sTempPath) ;
				return ob;
			}
			else
			{

MessageBox.Show("Key not found in hash table") ;
return null ;
			}

		}
//specify newName without extension
public IMediaAsset RenameAsset(IMediaAsset asset, String newName)
		{
	string OldName = asset.Name ;
			FileInfo file= new FileInfo (asset.Path);		
	string ext = file.Extension ;
			string sTemp = file.DirectoryName + "\\" + newName + ext;

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

AssetRenamed ob_AssetRenamed = new AssetRenamed (asset, OldName) ;
ob_AssetRenamed.AssetRenamedEvent+= new DAssetRenamedEvent (ob_CatchEvents.CatchAssetRenamedEvent) ; 
ob_AssetRenamed.NotifyAssetRenamed ( this, ob_AssetRenamed ) ;

				return asset ;
			}
			else
			{
				MessageBox.Show("Original file not found");
				return null;
			}
		}

//create a new asset of specified without updating hash table
		//string type must contain valid extension like .wav
		public IMediaAsset NewAsset(string assetType)
		{
			string sTemp = GenerateFileName (assetType, m_sDirPath);

			if (sTemp != null)
			{
				try
				{
					File.Create (sTemp) ;
				}
				catch
				{
					MessageBox.Show("Exeption! file could not be created") ;
				}
				MediaAsset m= new MediaAsset (sTemp) ;
				return m ;
			}
			else
			{
MessageBox.Show("File could not be created") ;
return null ;
			}
					}

		//create local asset from some external asset
public 		IMediaAsset AddAsset(string assetType, string assetPath)
		{
			string sTemp = GenerateFileName (assetType, m_sDirPath);

			if (sTemp != null)
			{
				try
				{
					//File.Create (sTemp) ;
					File.Copy(assetPath, sTemp,true) ;
				}
				catch
				{
MessageBox.Show("Exeption! Asset could not be created") ;
				}

				MediaAsset m= new MediaAsset (sTemp) ;
				try
				{
					m_htAssetList.Add(m.Name, m.Path) ; 
				}
				catch
				{
MessageBox.Show("Exeption! can not add duplicate in hash table") ;
				}
				return m ;
			}
			else
			{
				MessageBox.Show("Asset could not be created and added") ;
				return null ;
			}
		}


//generate file name for use in other methods
		//name wil range to 0.ext to 89999.ext whichever is available;
		public string GenerateFileName (string ext, string sDir)
				{
int i = 0 ;
			string sTemp ;
sTemp = sDir + "\\" + i.ToString() + ext ;
//FileInfo file = new FileInfo(sTemp) ;

					while (File.Exists(sTemp) && i<90000)
					{
i++;
						sTemp = sDir + "\\" + i.ToString() + ext ;

					}

			if (i<90000)
			{
				return sTemp ;
			}
			else
			{
return null ;
			}

				}

		public void addAssets(Hashtable assetURLs)
		{

			IDictionaryEnumerator en = assetURLs.GetEnumerator();

			while (en.MoveNext())
			{
				try
				{
					m_htAssetList.Add(en.Key, en.Value) ;
				}
				catch
				{
MessageBox.Show("Exxeption! Assets of same name exist in hash table.") ;
				}
			}

		}

//create a new  file in project directory and copy contents of source file to it.
  public IMediaAsset CopyAsset(IMediaAsset asset)
				{
			FileInfo file= new FileInfo (asset.Path);		
			string sTemp = GenerateFileName (file.Extension, m_sDirPath) ;
			try
			{
				file.CopyTo(sTemp,false);
			}
			catch
			{
				MessageBox.Show ("Can not copy the file");
			}
			MediaAsset ob = new MediaAsset (sTemp);
			return ob;
		}
		
		public void test ()
		{
			MessageBox.Show("It is working");
		}
	}
}
