using System;
using System.Windows.Forms;
using System.Collections;
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
                    throw new Exception(String.Format(Localizer.Message("project_directory_creation_error"), mAssetsDirectory), e);
                }
            }
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
            return (AudioMediaAsset)NameAddAsset(new AudioMediaAsset(channels, bitDepth, sampleRate));
        }

        /// <summary>
        /// Create a new AudioMediaAsset object from a list of clips and add it to the list of managed assets.
        /// </summary>
        /// <param name="clips">The array of <see cref="AudioClip"/>s.</param>
        /// <returns>The newly created asset.</returns>
        //public AudioMediaAsset NewAudioMediaAsset(ArrayList clips)
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
        /// <returns>The asset created.</returns>
        public AudioMediaAsset ImportAudioMediaAsset(string path)
        {
            List<AudioClip> clips = new List<AudioClip>(1);
            AudioClip clip = AudioClip.ImportClip(path, this);
            clips.Add(clip);
            AudioMediaAsset asset = NewAudioMediaAsset(clips);
            if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
            mFiles[clip.Path].Add(clip);
            return asset;
        }

        /// <summary>
        /// Try to add an existing asset.
        /// Throw an exception if it was already managed.
        /// </summary>
        /// <param name="asset">The asset to add.</param>
        /// <returns>True if the asset could be added.</returns>
        public bool AddAsset(MediaAsset asset)
		{
            if (asset.Manager != null)
            {
                throw new Exception(String.Format("Asset {0} is already managed.", asset.Name));
            }
            if (!mAssets.ContainsKey(asset.Name))
            {
                mAssets.Add(asset.Name, asset);
                asset.Manager = this;
                if (asset.Type == MediaType.Audio)
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
        public Dictionary<string, MediaAsset> GetAssets(MediaType type)
        {
            return type == MediaType.Audio ? mAssets : new  Dictionary<string, MediaAsset>();
        }

        /// <summary>
        /// Get a managed asset given its name.
        /// Return null if no such asset is found.
        /// </summary>
		public Assets.MediaAsset GetAsset(string name)
        {
            return mAssets.ContainsKey(name) ? mAssets[name] : null;
        }

        /// <summary>
        /// Remove a managed asset.
        /// Throw an exception if the asset was not managed in the first place.
        /// </summary>
        public void RemoveAsset(MediaAsset asset)
		{
            if (!mAssets.ContainsKey(asset.Name))
            {
                throw new Exception(String.Format("Asset {0} is not managed, cannot remove.", asset.Name));
            }
            if (asset.Type == MediaType.Audio)
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
        /// Delete a managed asset (remove first, then actually delete.)
        /// Throw an exception if the asset was not managed in the first place.
        /// </summary>
        /// <param name="assetToDelete"></param>
		public void DeleteAsset(Assets.MediaAsset asset)
		{
            RemoveAsset(asset);
            asset.Delete();
		}

        /// <summary>
        /// Copy a(n un)managed asset and add the (renamed) copy to the manager.
        /// </summary>
        /// <returns>The copy.</returns>
		public MediaAsset CopyAsset(MediaAsset asset)
		{
            MediaAsset copy = asset.Copy();
            RenameCopy(copy);
            AddAsset(copy);
            return copy;
		}

        /// <summary>
        /// Rename the copy of an asset, keeping a name as close to the original asset as possible.
        /// </summary>
        private void RenameCopy(MediaAsset asset)
        {
            while (mAssets.ContainsKey(asset.Name))
            {
                asset.Name += "*";
            }
        }

        /// <summary>
        /// Try to rename an asset handled by the asset manager.
        /// If the new name is taken, no change occurs.
        /// Throw an exception if the asset could not be found in the table.
        /// </summary>
        /// <returns>The old name before the change.</returns>
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
        /// Name and add an asset to the manager.
        /// </summary>
        /// <param name="asset">The (unnamed) asset to add.</param>
        /// <returns>The same asset with a new name and now managed.</returns>
        private MediaAsset NameAddAsset(MediaAsset asset)
        {
            asset.Name = UniqueName();
            AddAsset(asset);
            return asset;
            /*asset.Manager = this;
            mAssets.Add(asset.Name, asset);
            if (asset.Type == MediaType.Audio)
            {
                foreach (AudioClip clip in ((AudioMediaAsset)asset).Clips)
                {
                    if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
                    mFiles[clip.Path].Add(clip);
                }
            }
            return asset;*/
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
        /// Rename the asset with the given name and insure that rename takes place.
        /// If the name is not available, generate a new name using the given name as a basis (e.g. "Foo*" if "Foo" is taken.)
        /// </summary>
        /// <param name="asset">The asset to rename; must be managed.</param>
        /// <param name="name">The new name for the asset.</param>
        /// <exception cref="Exception"/>
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
        /// <exception cref="Exception"/>
        private void AssertAssetIsManaged(MediaAsset asset)
        {
            if (!mAssets.ContainsKey(asset.Name))
            {
                throw new Exception(String.Format("No asset named `{0}' in the manager", asset.Name));
            }
            if (asset != mAssets[asset.Name])
            {
                throw new Exception(String.Format("Asset named `{0}' differs from asset with the same name in the manager.",
                    asset.Name));
            }
            if (asset.Manager != this)
            {
                throw new Exception(String.Format("Asset named `{0}' does not think it is managed by the current asset manager.",
                    asset.Name));
            }
        }

        /// <summary>
        /// Get a list of all unsued file paths in the asset directory.
        /// </summary>
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
        /// Split an audio asset at a given time and add the result to the manager with a correct name.
        /// </summary>
        /// <returns>The new asset.</returns>
        public AudioMediaAsset SplitAudioMediaAsset(AudioMediaAsset asset, double time)
        {
            /*if (asset.Type == MediaType.Audio)
            {
                foreach (AudioClip clip in ((AudioMediaAsset)asset).Clips)
                {
                    if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
                    mFiles[clip.Path].Add(clip);
                }
            }*/
            return (AudioMediaAsset)NameAddAsset(asset.Split(time));
        }

        /// <summary>
        /// Merge two audio assets. The first one is modified in place.
        /// The second one is not managed any more, but its files are still there so there is no change.
        /// </summary>
        internal void MergeAudioMediaAssets(AudioMediaAsset asset, AudioMediaAsset next)
        {
            asset.MergeWith(next);
            mAssets.Remove(next.Name);
            next.Manager = null;
        }

        /// <summary>
        /// A clip was added to a managed asset.
        /// </summary>
        internal void AddedClip(AudioClip clip)
        {
            if (!mFiles.ContainsKey(clip.Path)) mFiles[clip.Path] = new List<AudioClip>();
            mFiles[clip.Path].Add(clip);
        }
    }
}