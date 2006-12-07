using System;
using System.Windows.Forms;
using System.IO;

namespace Obi.Assets
{
    /// <summary>
    /// Only Audio is known at the moment. Anything else is "other" and out of scope.
    /// </summary>
    public enum MediaType { Audio, Other };

    /// <summary>
    /// Base class for media assets.
    /// </summary>
    public abstract class MediaAsset
    {
        protected string mName;
        protected MediaType mMediaType;
        protected long mSizeInBytes;
        internal AssetManager mAssManager;

        /// <summary>
        /// Name of the asset. Unique in the manager.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Type of the asset.
        /// </summary>
        public MediaType Type
        {
            get { return mMediaType; }
        }

        /// <summary>
        /// Total size in bytes of the asset.
        /// </summary>
        public long SizeInBytes
        {
            get { return mSizeInBytes; }
            set { mSizeInBytes = value; }
        }

        /// <summary>
        /// Manager for this asset.
        /// </summary>
        public AssetManager Manager
        {
            get { return mAssManager; }
            set { mAssManager = value; }
        }

        /// <summary>
        /// Copy the asset.
        /// </summary>
        /// <returns>The copy</returns>
        public abstract MediaAsset Copy();

        /// <summary>
        /// Delete the asset.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Merge with another asset (normally, the next one in sequence.)
        /// </summary>
        /// <param name="next">The asset to merge with.</param>
        public abstract void MergeWith(MediaAsset next);
    }
}