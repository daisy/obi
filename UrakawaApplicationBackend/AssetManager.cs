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
		private Hashtable m_htAssetList = new Hashtable();

// static variable for directory to be used in other classes of namespace
		internal static string ProjectDirectory  ;

		//static counter to implement singleton
		private static int m_ConstructorCounter =0;

		CatchEvents ob_CatchEvents = new CatchEvents () ;

		// Please implement this
		public IAssetManager Instance
		{
			get
			{
				return this;
			}
		}
		
		//constructor [should be private]
		public AssetManager (string sProjectDir)
		{
			if (m_ConstructorCounter == 0)
			{
				m_sDirPath = sProjectDir ;
				ProjectDirectory = m_sDirPath ;

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

				

		//returns hashtable containing pat of assets from project directory
		// Parameter should be of extension of file e.g. ".wav"
		public Hashtable GetAssets(TypeOfMedia assetType)
		{
			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();
			
			
Hashtable htTemp = new Hashtable () ;
MediaAsset m ;
			//find the path from hash table using key 
			while (en.MoveNext() )
			{
m =en.Value as MediaAsset ;

				if (m.MediaType.Equals (assetType)) 
				{
htTemp.Add ( en.Key , en.Value) ; 
				}
					
			}
			return htTemp ;
		}

		//Delete asset from Project directory and hash table
		public void DeleteAsset(IMediaAsset assetToDelete)
		{
			
			FileInfo m_file= new FileInfo	 (assetToDelete.Path);
			if ( m_file.Exists && m_htAssetList.ContainsKey(assetToDelete.Name) )
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

		public IMediaAsset RemoveAsset(IMediaAsset assetToRemove)
		{
			if (  m_htAssetList.ContainsKey(assetToRemove.Name) )
			{
				m_htAssetList.Remove(assetToRemove.Name) ;
				return assetToRemove ;
			}
			else
			{
				MessageBox.Show ("Asset not in Hash Table") ;
				return null;
			}
		}
		
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
					MediaAsset m = en.Value as MediaAsset ;
return m ;					
						
					//break ;
				}
					
			}


			/*
			//if found, create object using path and return
			if (m_sTempPath != null)
			{
				MediaAsset ob =new MediaAsset  (m_sTempPath) ;
				return ob;
			}
			*/
			


				MessageBox.Show("Key not found in hash table") ;
				return null ;
			

		}
		//specify newName without extension
		public IMediaAsset RenameAsset(IMediaAsset asset, String newName)
		{
			
			string OldName = asset.Name ;
			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();
			
			while (en.MoveNext() )
			{	
				if (en.Key.ToString ()  == asset.Name)
				{
					
m_htAssetList.Remove (en.Key) ;
					asset.Name =newName ;
					m_htAssetList.Add (asset.Name , asset) ;

					AssetRenamed ob_AssetRenamed = new AssetRenamed (asset, OldName) ;
					ob_AssetRenamed.AssetRenamedEvent+= new DAssetRenamedEvent (ob_CatchEvents.CatchAssetRenamedEvent) ; 
					ob_AssetRenamed.NotifyAssetRenamed ( this, ob_AssetRenamed ) ;

					return asset ;

				}
			}
			MessageBox.Show ("Key not found, null will be returned") ;
return null;

// end of function
		}

		
		//string type must contain valid extension like .wav
		// Parameter changed to enum TypeOfMedia
		public IMediaAsset NewAsset(TypeOfMedia  assetType)
		{

			string sTemp = null;
			if (assetType.Equals (TypeOfMedia.Audio) )
			{
				sTemp = GenerateFileName (".wav", m_sDirPath);
			}

				if (sTemp != null)
				{
					

					
						//File.Create (sTemp) ;
						BinaryWriter bw = new BinaryWriter ( File.Create(sTemp)) ;
					

					


					

					bw.BaseStream.Position = 0 ;
byte b = Convert.ToByte (1) ;
					for (int i = 0 ; i< 44 ; i++) 
					{
bw.Write (b) ;
					}
					b= Convert.ToByte(44) ;
					bw.BaseStream.Position = 7 ;
					bw.Write (b) ;
bw.Close () ;

					AudioMediaAsset am= new AudioMediaAsset (sTemp) ;
					m_htAssetList.Add (am.Name, am) ;
					return am ;
				}
				else
				{
					MessageBox.Show("File could not be created and newAsset will throw a null exeption") ;
					return null ;
				}
			
		
	}
			//create local asset from some external asset
			public 		IMediaAsset AddAsset(TypeOfMedia assetType, string assetPath)
			{
				string sTemp  = null ;
				if ( assetType.Equals(TypeOfMedia.Audio)  )
				{
					sTemp = GenerateFileName (".wav", m_sDirPath);
				}	


				if (sTemp != null)
				{
					try
					{
						//File.Create (sTemp) ;
						File.Copy(assetPath, sTemp,true) ;
					}
					catch
					{
						MessageBox.Show("Exeption! file could not be created in add asset in asset manager") ;
					}

					AudioMediaAsset am= new AudioMediaAsset (sTemp) ;

					try
					{
						m_htAssetList.Add(am.Name, am) ; 
					}
					catch
					{
						MessageBox.Show("Exeption! can not add duplicate in hash table") ;
					}
					return am ;
				}
				else
				{
					MessageBox.Show("Asset could not be created and added. A null exeption will be thrown") ;
					return null ;
				}
// end of function 
	}


//generate file name for use in other methods
		//name wil range to 0.ext to 89999.ext whichever is available;
		internal string GenerateFileName (string ext, string sDir)
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
				return null ;
			}
			AudioMediaAsset ob = new AudioMediaAsset (sTemp);
	  m_htAssetList.Add ( ob.Name ,ob) ;
			return ob;
		}
		
		internal void test ()
		{
AudioMediaAsset am = new AudioMediaAsset  ("c:\\atest\\a\\Num.wav") ;
			object o = am ;
MediaAsset m = o as MediaAsset ;
//m_htAssetList.Add  (1 , am) ;
MessageBox.Show (m.GetType().ToString () );

			MessageBox.Show("It is working");
		}
		
	
		// end of class
	}
// end of namespace
}
