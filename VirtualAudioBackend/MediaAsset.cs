using System;
using System.Windows.Forms;
using System.IO;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Virtual edits implementation of IMediaAsset.
	/// </summary>
	public abstract class MediaAsset: IMediaAsset
	{
		#region IMediaAsset Members

// member variables of class
private string m_sName ;
		private MediaType m_eMediaType ;

protected long m_lSizeInBytes ;
		internal AssetManager m_AssetManager ;


		public string Name
		{
			get
			{
				return m_sName ;
			}
			set
			{
				m_sName = value ;
			}
		}

		public MediaType MediaType
		{
			get
			{
				return m_eMediaType ;
			}
		}

		public long SizeInBytes
		{
			get
			{
				return m_lSizeInBytes ;
			}
			set
			{
				m_lSizeInBytes = value ;
			}
		}

		public IAssetManager AssetManager
		{
			get
			{
				return m_AssetManager ;
			}
		}

		public void Add(IAssetManager manager)
		{
manager.Assets.Add (m_sName, this) ;
			m_AssetManager = manager as AssetManager;
		}

		public void Remove()
		{
			if (  m_AssetManager.Assets.ContainsKey(this.Name) )			{
				m_AssetManager.Assets.Remove(this.Name) ;
				m_AssetManager = null ;
			}
else
				MessageBox.Show ("MediaAsset can not be removed from AssetManager") ;
		}

		public abstract void Delete();
		
		public abstract void MergeWith(IMediaAsset next);

		#endregion
	}
}