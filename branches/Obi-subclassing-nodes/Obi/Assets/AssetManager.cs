using System;
using System.Collections.Generic;
using System.IO;

namespace Obi.Assets
{
    /// <summary>
    /// The asset manager currently manages audio assets only.
    /// Keep track of audio files used by clips as well.
    /// </summary>
	public class AssetManager
	{		
		private string mAssetsDirectory;
        private Uri mBaseURI;

        /// <summary>
        /// Absolute path to the project directory.
        /// </summary>
		internal string AssetsDirectory
		{
			get { return mAssetsDirectory; }
		}

        /// <summary>
        /// Base URI of the asset manager directory.
        /// </summary>
        public Uri BaseURI
        {
            get { return mBaseURI; }
        }

        private Dictionary<string, MediaAsset> mAssets;      // the assets being managed indexed by their name
        private int mAssNameCounter;                         // counter for unique asset names
        private Dictionary<string, List<AudioClip>> mFiles;  // audio files and the list of assets referring to them
        private int mFileNameCounter;                        // counter for unique file names

        /// <summary>
        /// List of files in use and which clips use them.
        /// When an asset is removed, the clips are removed from this list but not the files.
        /// Files that have an empty list of clips can then be removed safely.
        /// </summary>
        internal Dictionary<string, List<AudioClip>> Files
        {
            get { return mFiles; }
        }

        /// <summary>
        /// Create the asset manager taking as argument the project directory where the data should live.
        /// The directory is created if it didn't exist; an exception is raised if a problem occurs.
        /// </summary>
        /// <param name="assetsDirectory">Absolute path to the assets directory.</param>
        /// <exception cref="CannotCreateDirectoryException"/>
        public AssetManager(string assetsDirectory)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "file";
            builder.Path = assetsDirectory + @"\";
            mBaseURI = builder.Uri;
            mAssetsDirectory = System.Text.RegularExpressions.Regex.Replace(mBaseURI.LocalPath, @"^\\\\localhost\\", "");
            mAssets = new Dictionary<string, MediaAsset>();
            mAssNameCounter = 0;
            mFiles = new Dictionary<string, List<AudioClip>>();
            mFileNameCounter = 0;
            if (!Directory.Exists(mAssetsDirectory))
            {
                try
                {
                    Directory.CreateDirectory(mAssetsDirectory);
                }
                catch (Exception e)
                {
                    throw new CannotCreateDirectoryException(mAssetsDirectory, e);
                }
            }
        }

        /// <summary>
        /// Create a new empty AudioMediaAsset object with the given parameters and add it to the list of managed assets.
        /// </summary>
        /// <param name="channels">Number of channels (1 or 2.)</param>
        /// <param name="bitDepth">Bit depth (8 or 16.)</param>
        /// <param name="sampleRate">Sample rate in Hertz (up to 48000)</param>
        /// <returns>The named and managed asset.</returns>
        public AudioMediaAsset NewAudioMediaAsset(int channels, int bitDepth, int sampleRate)
        {
            return (AudioMediaAsset)NameAddAsset(new AudioMediaAsset(channels, bitDepth, sampleRate));
        }

        /// <summary>
        /// Create a new AudioMediaAsset object from a list of clips and add it to the list of managed assets.
        /// </summary>
        /// <param name="clips">The list of <see cref="AudioClip"/>s.</param>
        /// <returns>The named and managed asset.</returns>
        public AudioMediaAsset NewAudioMediaAsset(List<AudioClip> clips)
        {
            AudioMediaAsset asset = (AudioMediaAsset)NameAddAsset(new AudioMediaAsset(clips));
            return asset;
        }

        /// <summary>
        /// Create an asset directly from a file and add it into the manager.
        /// Its file is copied to the asset manager directory.
        /// </summary>
        /// <param name="path">The path of the file to import.</param>
        /// <returns>The named and managed asset.</returns>
        public AudioMediaAsset ImportAudioMediaAsset(string path)
        {
            List<AudioClip> clips = new List<AudioClip>(1);
            AudioClip clip = AudioClip.ImportClip(path, this);
            clips.Add(clip);
            AudioMediaAsset asset = NewAudioMediaAsset(clips);
            AddClip(clip);
            return asset;
        }

        /// <summary>
        /// Try to add an existing asset.
        /// </summary>
        /// <param name="asset">The asset to add.</param>
        /// <returns>True if the asset could be added; normally it shouldn't return false.</returns>
        /// <exception cref="AlreadyManagedAssetException"/>
        public bool AddAsset(MediaAsset asset)
		{
            if (asset.Manager != null)
            {
                throw new AlreadyManagedAssetException(asset);
            }
            if (!mAssets.ContainsKey(asset.Name))
            {
                mAssets.Add(asset.Name, asset);
                asset.Manager = this;
                if (asset.GetType() == typeof(AudioMediaAsset))
                {
                    foreach (AudioClip clip in ((AudioMediaAsset)asset).Clips)
                    {
                        if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
                        mFiles[clip.Path].Add(clip);
                    }
                }
                return true;
            }
            return false;
		}

        /// <summary>
        /// Get a dictionary of all managed assets of a given type.
        /// Currenty, only audio assets are managed.
        /// </summary>
        /// <param name="type">The required media asset type.</param>
        /// <returns>A dictionary of assets.</returns>
        /// <exception cref="UnsupportedMediaTypeException"/>
        public Dictionary<string, MediaAsset> GetAssets(Type type)
        {
            if (type == typeof(AudioMediaAsset))
            {
                return mAssets;
            }
            else
            {
                throw new UnsupportedMediaTypeException(type);
            }
        }

        /// <summary>
        /// Get a managed asset given its name.
        /// </summary>
        /// <param name="name">Name of the requested asset.</param>
        /// <returns>The managed asset with this name, or null if none is found.</returns>
		public Assets.MediaAsset GetAsset(string name)
        {
            return mAssets.ContainsKey(name) ? mAssets[name] : null;
        }

        /// <summary>
        /// Remove a managed asset.
        /// When an audio asset is removed, its clip files are removed as well.
        /// </summary>
        /// <param name="asset">The asset to remove.</param>
        /// <exception cref="UnmanagedAssetException"/>
        public void RemoveAsset(MediaAsset asset)
		{
            if (!mAssets.ContainsKey(asset.Name))
            {
                throw new UnmanagedAssetException(asset);
            }
            if (asset.GetType() == typeof(AudioMediaAsset))
            {
                foreach (AudioClip clip in ((AudioMediaAsset)asset).Clips)
                {
                    mFiles[clip.Path].RemoveAt(mFiles[clip.Path].IndexOf(clip));
                }
            }
            mAssets.Remove(asset.Name);
            asset.Manager = null;
		}

        /// <summary>
        /// Delete a managed asset. The asset is removed from the manager, then actually deleted.
        /// </summary>
        /// <param name="asset">The asset to delete.</param>
        /// <exception cref="UnmanagedAssetException"/>
        public void DeleteAsset(Assets.MediaAsset asset)
		{
            RemoveAsset(asset);
            asset.Delete();
		}

        /// <summary>
        /// Copy a(n un)managed asset and add the (renamed) copy to the manager.
        /// </summary>
        /// <param name="asset">The asset to copy.</param>
        /// <returns>The renamed and managed copy.</returns>
		public MediaAsset CopyAsset(MediaAsset asset)
		{
            MediaAsset copy = asset.Copy();
            RenameCopy(copy);
            AddAsset(copy);
            return copy;
		}

        /// <summary>
        /// Since the name of an asset must be unique in the manager, this function renames
        /// the copy of an asset so that there is no confusion between original and copy.
        /// </summary>
        /// <param name="asset">The asset to rename.</param>
        private void RenameCopy(MediaAsset asset)
        {
            while (mAssets.ContainsKey(asset.Name))
            {
                asset.Name += "*";
            }
        }

        /// <summary>
        /// When we want to rename an asset, we have to make sure that there doesn't already
        /// exist another asset with the same name in the manager. If the new name is taken,
        /// no change occurs.
        /// </summary>
        /// <param name="asset">The asset to rename (which still has its old name.)</param>
        /// <param name="newName">The requested new name.</param>
        /// <returns>The name before the possible change.</returns>
        /// <exception cref="UnmanagedAssetException"/>
        public string RenameAsset(MediaAsset asset, String newName)
        {
            AssertAssetIsManaged(asset);
            string oldName = asset.Name;
            if (!mAssets.ContainsKey(newName))
            {
                mAssets.Remove(asset.Name);
                asset.Name = newName;
                mAssets.Add(asset.Name, asset);
            }
            return oldName;
        }

        /// <summary>
        /// Add an asset to the manager and give it a unique name.
        /// </summary>
        /// <param name="asset">The (unnamed) asset to add.</param>
        /// <returns>The same asset with a new name and now managed.</returns>
        /// <exception cref="AlreadyManagedAssetException"/>
        private MediaAsset NameAddAsset(MediaAsset asset)
        {
            asset.Name = UniqueName();
            AddAsset(asset);
            return asset;
        }

        /// <summary>
        /// Generate a unique name for an asset.
        /// </summary>
        /// <returns>The generated name.</returns>
        public string UniqueName()
        {
            string name;
            do
            {
                name = String.Format(Localizer.Message("asset_name"), mAssNameCounter.ToString("00000"));
                ++mAssNameCounter;
            }
            while(mAssets.ContainsKey(name));
            return name;
        }

        /// <summary>
        /// Generate a unique file name for a clip.
        /// </summary>
        /// <param name="ext">The file extension (including leading dot, allows for no extension.)</param>
        /// <returns>The generated name.</returns>
        public string UniqueFileName(string ext)
        {
            string name;
            do
            {
                name = String.Format("{0}clip_{1}{2}", mAssetsDirectory, mFileNameCounter.ToString("00000"), ext);
                ++mFileNameCounter;
            }
            while(File.Exists(name));
            return name;
        }

        /// <summary>
        /// Rename the asset with the given name and insure that renaming takes place.
        /// If the name is not available, generate a new name using the given name as a basis.
        /// </summary>
        /// <param name="asset">The asset to rename; must be managed.</param>
        /// <param name="name">The new name for the asset.</param>
        /// <exception cref="UnmanagedAssetException"/>
        public void InsureRename(MediaAsset asset, string name)
        {
            AssertAssetIsManaged(asset);
            while (mAssets.ContainsKey(name)) name += "*";
            mAssets.Remove(asset.Name);
            asset.Name = name;
            mAssets.Add(asset.Name, asset);
        }

        /// <summary>
        /// Assert that an asset is managed, i.e. there is an asset with the name in the manager and it is this one.
        /// </summary>
        /// <param name="asset">The asset to check.</param>
        /// <exception cref="UnmanagedAssetException"/>
        private void AssertAssetIsManaged(MediaAsset asset)
        {
            if (!mAssets.ContainsKey(asset.Name))
            {
                throw new UnmanagedAssetException(asset);
            }
            if (asset != mAssets[asset.Name])
            {
                throw new UnmanagedAssetException(asset,
                    String.Format("Asset named `{0}' differs from asset with the same name in the manager.", asset.Name));
            }
            if (asset.Manager != this)
            {
                throw new UnmanagedAssetException(asset,
                    String.Format("Asset named `{0}' does not think it is managed by the current asset manager.", asset.Name));
            }
        }

        /// <summary>
        /// Get a list of all unsued file paths in the asset directory.
        /// All clips that were backed by these files have been deleted
        /// so the files themselves can be safely deleted. 
        /// </summary>
        /// <returns>The list of file paths.</returns>
        public List<string> UnusedFilePaths()
        {
            List<string> unused = new List<string>();
            foreach (string path in Directory.GetFiles(mAssetsDirectory))
            {
                if (!mFiles.ContainsKey(path)) unused.Add(path);
            }
            return unused;
        }

        /// <summary>
        /// Split an audio asset at a given time and add the result to the manager with a unique name.
        /// </summary>
        /// <param name="asset">The asset to split.</param>
        /// <param name="time">The time in milliseconds when the split occurs.</param>
        /// <returns>The new asset.</returns>
        /// <exception cref="AlreadyManagedAssetException"/>
        public AudioMediaAsset SplitAudioMediaAsset(AudioMediaAsset asset, double time)
        {
            return (AudioMediaAsset)NameAddAsset(asset.Split(time));
        }

        /// <summary>
        /// Merge two audio assets. The first one is modified in place.
        /// The second one is not managed any more, but its files are still
        /// there so there is no change as far as clips are concerned.
        /// </summary>
        /// <param name="asset">The asset to modify.</param>
        /// <param name="next">The asset that it merges with.</param>
        /// <exception cref="UnmanagedAssertException"/>
        internal void MergeAudioMediaAssets(AudioMediaAsset asset, AudioMediaAsset next)
        {
            AssertAssetIsManaged(asset);
            AssertAssetIsManaged(next);
            asset.MergeWith(next);
            mAssets.Remove(next.Name);
            next.Manager = null;
        }

        /// <summary>
        /// A clip was added to a managed asset.
        /// </summary>
        /// <param name="clip">The added clip.</param>
        internal void AddClip(AudioClip clip)
        {
            if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
            mFiles[clip.Path].Add(clip);
        }
    }
}