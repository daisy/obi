using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    public delegate void ErrorHandler(string message);

    /// <summary>
    /// Visitor to recreate assets from media objects in the tree.
    /// </summary>
    class AssetCreator: ICoreNodeVisitor
    {
        private Assets.AssetManager mAssManager;  // the asset manager which handles the assets
        private ErrorHandler mErrorHandler;       // error handler called when an asset cannot be created.

        /// <summary>
        /// Create a new asset visitor for a given asset manager.
        /// </summary>
        public AssetCreator(Assets.AssetManager assManager, ErrorHandler errorHandler)
        {
            mAssManager = assManager;
            mErrorHandler = errorHandler;
        }

        #region ICoreNodeVisitor Members

        /// <summary>
        /// Do nothing.
        /// </summary>
        public void postVisit(ICoreNode node)
        {
        }

        /// <summary>
        /// Recreate an asset from a sequential media object on the audio channel of a phrase node.
        /// </summary>
        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.Phrase)
            {
                SequenceMedia media = (SequenceMedia)Project.GetMediaForChannel((CoreNode)node, Project.AudioChannel);
                List<Assets.AudioClip> clips = new List<Assets.AudioClip>(media.getCount());
                for (int i = 0; i < media.getCount(); ++i)
                {
                    AudioMedia audio = (AudioMedia)media.getItem(i);
                    try
                    {
                        // location is an URI relative to the asset manager location
                        Uri assetUri = new Uri(mAssManager.BaseURI, audio.getLocation().Location);
                        string location = System.Text.RegularExpressions.Regex.Replace(assetUri.LocalPath, @"^\\\\localhost\\", "");
                        clips.Add(new Assets.AudioClip(location, audio.getClipBegin().getAsMilliseconds(),
                            audio.getClipEnd().getAsMilliseconds()));
                    }
                    catch (Exception e)
                    {
                        mErrorHandler(e.Message);
                    }
                }
                if (clips.Count == media.getCount())
                {
                    Assets.AudioMediaAsset asset = mAssManager.NewAudioMediaAsset(clips);
                    //mAssManager.RenameAsset(asset,
                    //    ((TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel)).getText());
                    AssetProperty assProp =
                        (AssetProperty)node.getPresentation().getPropertyFactory().createProperty("AssetProperty",
                        ObiPropertyFactory.ObiNS);
                    assProp.Asset = asset;
                    node.setProperty(assProp);
                }
                // else
                // {
                //     make an invalid object.
                // }
            }
            return true;
        }

        #endregion
    }
}
