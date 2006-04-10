using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections;

namespace urakawaApplication
{
    public interface IAssetManager
    {
				
        //IAssetManager getInstance();

        //return value is some sort of Collection<MediaAsset>
         ArrayList getAssets();

        //returns value is some sort of Collection<MediaAssetSubType>
         //the parameter type will be something like MediaAssetType instead of Object
          ArrayList getAssets(Object assetType);

         //throws AssetManagerException 
         //IMediaAsset getAsset(String assetLocalName);

        // throws AssetManagerException 
         //IMediaAsset getAsset(Uri assetURL);

        // throws AssetManagerException [example: initializes a new outputstrem and gives it a name, feeds a recorded audiostream into the file]
         ////the parameter type will be something like MediaAssetType instead of Object
         //IMediaAsset newAsset(Object assetType);

        // throws AssetManagerException 
         void deleteAsset(IMediaAsset assetToDelete);

        // throws AssetManagerException 
         //IMediaAsset copyAsset(IMediaAsset source, IMediaAsset dest, bool replaceIfExisting);

        //throws AssetManagerException 
         //IMediaAsset renameAsset(IMediaAsset source, String newName);

        //throws AssetManagerException 
         //void addAsset(Uri assetURL);

        //parameter is some sort of Collection<URL>
        //void addAssets(ArrayList assetURLs);

		
    }
	class AssetManager :  IAssetManager 
	{
		
	
			public string m_sDirPath;

		public ArrayList getAssets()
		{
			DirectoryInfo dir = new DirectoryInfo(@m_sDirPath);
			FileInfo[] bmpfiles = dir.GetFiles("*.*") ;
			ArrayList m_alTemp= null;
			
			foreach( FileInfo f in bmpfiles)
			{
				m_alTemp.Add(f.FullName);
			}
			return m_alTemp;
		}
		public ArrayList getAssets(Object assetType)
		{
			DirectoryInfo dir = new DirectoryInfo(@m_sDirPath);
			FileInfo[] bmpfiles = dir.GetFiles(assetType.ToString()) ;
			ArrayList m_alTemp= null;
			
			foreach( FileInfo f in bmpfiles)
			{
				m_alTemp.Add(f.FullName);
			}
			return m_alTemp;
		}

		public void deleteAsset(IMediaAsset assetToDelete)
		{
			//FileInfo m_file= new FileInfo	 (assetToDelete.FilePath);
//m_file.Delete();
		}

		
	}
}
