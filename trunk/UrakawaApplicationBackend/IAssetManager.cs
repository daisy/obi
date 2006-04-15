using System;
using System.IO;
using System.Collections;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace urakawaApplication
{
    public interface IAssetManager
    {
				
        //IAssetManager getInstance();

        //return value is some sort of Collection<MediaAsset>
		//Parameter added: sDirPath for path of target directory and sFilter to act as filtering condition e.g. "*.*" to get all files, "*.wav" to get only wav files
         ArrayList getAssets(string sDirPath, string sFilter);

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
         IMediaAsset copyAsset(IMediaAsset source, IMediaAsset dest, bool replaceIfExisting);

        //throws AssetManagerException 
         IMediaAsset renameAsset(IMediaAsset source, String newName);

        //throws AssetManagerException 
         //void addAsset(Uri assetURL);

        //parameter is some sort of Collection<URL>
        //void addAssets(ArrayList assetURLs);

		
    }
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
}
