using System;
using System.Windows.Forms;
using System.Collections;
using VirtualAudioBackend.events.AssetManagerEvents;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Implementation of the asset manager.
	/// </summary>
	public class AssetManager: IAssetManager
	{

		// member variables
		// hold path of project directory
		
		private string m_sDirPath;

		
		//hash table to hold paths of assets being managed
		private Hashtable m_htAssetList = new Hashtable();


		//		static variable for directory to be used in other classes of namespace
		internal static string static_sProjectDirectory  ;

// hash table to contain list of all existing assets
internal static Hashtable static_htExists  = new Hashtable ();

// object for catch class
		CatchEvents ob_Catch = new CatchEvents();

		/// <summary>
		/// Create the asset manager taking as argument the project directory where the data should live.
		/// </summary>
		public AssetManager(string projectDirectory)
		{
			m_sDirPath = projectDirectory ;
			static_sProjectDirectory   = projectDirectory  ;
		}

		/// <summary>
		/// Create a new empty AudioMediaAsset object with the given parameters and add it to the list of managed assets.
		/// </summary>
		/// <param name="channels">Number of channels</param>
		/// <param name="bitDepth">Bit depth</param>
		/// <param name="sampleRate">Sample rate</param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(int channels, int bitDepth, int sampleRate)
		{
			AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset (channels , bitDepth , sampleRate) ;
			ob_AudioMediaAsset.Name = NewMediaAssetName () ;
			ob_AudioMediaAsset.m_AssetManager = this ;
			m_htAssetList.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;
static_htExists.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;   
			return ob_AudioMediaAsset ;
		}

		/// <summary>
		/// Create a new AudioMediaAsset object from a list of clips and add it to the list of managed assets.
		/// </summary>
		/// <param name="clips">The array of <see cref="AudioClip"/>s.</param>
		/// <returns>The newly created asset.</returns>
		public AudioMediaAsset NewAudioMediaAsset(ArrayList clips)
		{
			if (clips != null)
			{
				//AudioClip ob_AudioClip = clips[0] as AudioClip;
				//AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset ( ob_AudioClip.Channels , ob_AudioClip.BitDepth , ob_AudioClip.SampleRate) ;
				// below line is to be deleted
				AudioMediaAsset ob_AudioMediaAsset = new AudioMediaAsset ( 0, 0, 0) ;
				ob_AudioMediaAsset.Name = NewMediaAssetName () ;
				ob_AudioMediaAsset.m_AssetManager = this ;
				m_htAssetList.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;
				static_htExists.Add (ob_AudioMediaAsset.Name, ob_AudioMediaAsset) ;   
				return ob_AudioMediaAsset ;
			}
			else
			{
MessageBox.Show ("AudioMediaAsset can not be created with this function as clip list is empty") ;
				return null ;
			}
		}

		/// <summary>
		/// Produce a unique name for IMediaAsset
		/// </summary>
		public string NewMediaAssetName()
		{

			long i = 0 ;

			string sTemp ;

			sTemp = "amMediaAsset" ;
			string sTempName ;
			sTempName = sTemp + i.ToString () ;

			while ( static_htExists.ContainsKey (sTempName)  && i<900000)
			{

				i++;
				sTempName = sTemp + i.ToString () ;
				

			}


			if (i<900000)

			{
			
				return sTempName ;

			}
			else
			{
				return null ;
			}
		}
		
			
		

		#region IAssetManager Members

		public Hashtable Assets
		{
			get
			{
				return m_htAssetList ;
			}
		}

		public void AddAsset(IMediaAsset asset)
		{
m_htAssetList.Add (asset.Name, asset) ;
			static_htExists.Add (asset.Name, asset) ;
		}

		public Hashtable GetAssets(VirtualAudioBackend.MediaType assetType)
		{

			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();
			
			Hashtable htTemp = new Hashtable () ;

			MediaAsset m ;
			//find the asset from hash table using key 
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
	

		public IMediaAsset GetAsset(string assetName)
		{
		
			IDictionaryEnumerator en = m_htAssetList.GetEnumerator();

			//find the asset from hash table using key 
			while (en.MoveNext() )
			{
				if (assetName == en.Key.ToString() )
				{
					MediaAsset m = en.Value as MediaAsset ;
return m ;					
						
					//break ;
				}
					
			}
MessageBox.Show ("Asset not found in hashtable") ;
			return null;
		}

		public IMediaAsset NewAsset(VirtualAudioBackend.MediaType assetType)
		{
			return null;
		}

		public void DeleteAsset(IMediaAsset assetToDelete)
		{
		
			if (  m_htAssetList.ContainsKey(assetToDelete.Name) )			{
				m_htAssetList.Remove(assetToDelete.Name) ;
static_htExists.Remove(assetToDelete.Name) ;
assetToDelete= null ;
				
				AssetDeleted ob_AssetDeleted = new AssetDeleted(assetToDelete);
				ob_AssetDeleted.AssetDeletedEvent+= new DAssetDeletedEvent(ob_Catch.CatchAssetDeletedEvent);
				ob_AssetDeleted.NotifyAssetDeleted(this, ob_AssetDeleted);
				
			}
			else
			{
	
				MessageBox.Show ("Asset not in Hash Table and cannot be deleted") ;
				
			}

		}

		public void RemoveAsset(IMediaAsset assetToRemove)
		{
MediaAsset MediaAssetToRemove = assetToRemove as MediaAsset ;
			if (  m_htAssetList.ContainsKey(MediaAssetToRemove.Name) )			{
				m_htAssetList.Remove(MediaAssetToRemove.Name) ;
MediaAssetToRemove.m_AssetManager = null ;

			}
			else
				MessageBox.Show ("Asset cannot be removed from hashtable");
			
		}

		public IMediaAsset CopyAsset(IMediaAsset asset)
		{
			return null;
		}

		public string RenameAsset(IMediaAsset asset, String newName)
		{
			string OldName = asset.Name;
			IDictionaryEnumerator mEnumerator = m_htAssetList.GetEnumerator();
			while(mEnumerator.MoveNext())
			{
				if(mEnumerator.Key.ToString() == asset.Name)
				{
					m_htAssetList.Remove(mEnumerator.Key);
					asset.Name = newName;
					m_htAssetList.Add(asset.Name, asset);
				}
			}
			
			AssetRenamed ob_AssetRenamed =  new AssetRenamed(asset, OldName);
			ob_AssetRenamed.AssetRenamedEvent+= new DAssetRenamedEvent(ob_Catch.CatchAssetRenamedEvent);
			ob_AssetRenamed.NotifyAssetRenamed(this, ob_AssetRenamed);
			return OldName;
		}
		}

		#endregion


	}

