using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Assets
{
    /// <summary>
    /// The project directory for the asset manager cannot be created.
    /// </summary>
    public class CannotCreateDirectoryException : Exception
    {
        private string mDirectory;

        /// <summary>
        /// The requested project directory path.
        /// </summary>
        public string Directory
        {
            get { return mDirectory; }
        }

        /// <summary>
        /// Create a new exception.
        /// </summary>
        /// <param name="message">The corresponding error message.</param>
        /// <param name="e">The inner exception.</param>
        public CannotCreateDirectoryException(string directory, Exception e)
            : base(String.Format(Localizer.Message("project_directory_creation_error"), directory), e)
        {
            mDirectory = directory;
        }
    }

    /// <summary>
    /// The asset manager does not support this media type.
    /// </summary>
    public class UnsupportedMediaTypeException : Exception
    {
        private Type mType;

        /// <summary>
        /// The type that is unsupported.
        /// </summary>
        public Type UnsupportedType
        {
            get { return mType; }
        }

        /// <summary>
        /// Create the exception.
        /// </summary>
        /// <param name="type">The unsupported type.</param>
        public UnsupportedMediaTypeException(Type type)
            : base(String.Format("Unsupported media type: {0}.", type))
        {
            mType = type;
        }
    }

    /// <summary>
    /// Generic asset exception.
    /// </summary>
    public abstract class AssetException : Exception
    {
        private MediaAsset mAsset;

        /// <summary>
        /// The asset concerned by this exception.
        /// </summary>
        public MediaAsset Asset
        {
            get { return mAsset; }
        }

        /// <summary>
        /// Create the exception.
        /// </summary>
        /// <param name="asset">The asset concerned by this exception.</param>
        /// <param name="message">The exception message.</param>
        public AssetException(MediaAsset asset, string message)
            : base(message)
        {
            mAsset = asset;
        }
    }

    /// <summary>
    /// An asset is already managed.
    /// </summary>
    public class AlreadyManagedAssetException : AssetException
    {
        /// <summary>
        /// Create the exception.
        /// </summary>
        /// <param name="asset">The asset that is already managed.</param>
        public AlreadyManagedAssetException(MediaAsset asset)
            : base(asset, String.Format("Asset {0} is already managed.", asset.Name))
        {
        }
    }

    /// <summary>
    /// An asset is not managed.
    /// </summary>
    public class UnmanagedAssetException : AssetException
    {
        /// <summary>
        /// Create the exception.
        /// </summary>
        /// <param name="asset">The asset that is not managed.</param>
        public UnmanagedAssetException(MediaAsset asset)
            : base(asset, String.Format("Asset {0} is not managed.", asset.Name))
        {
        }

        /// <summary>
        /// Create the exception.
        /// </summary>
        /// <param name="asset">The asset that is not managed.</param>
        /// <param name="message">A custom message.</param>
        public UnmanagedAssetException(MediaAsset asset, string message)
            : base(asset, message)
        {
        }
    }
}
