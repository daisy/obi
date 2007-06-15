using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.media;

namespace Obi.Visitors
{
    public delegate void ErrorHandler(string message);

    /// <summary>
    /// Visitor to initialize phrases (create assets, counts pages, sets annotations.)
    /// </summary>
    /// <remarks>It reads in more stuff about phrases so the name does not really fit anymore.</remarks>
    class PhraseInitializer: ICoreNodeVisitor
    {
        private Assets.AssetManager mAssManager;  // the asset manager which handles the assets
        private ErrorHandler mErrorHandler;       // error handler called when an asset cannot be created.
        private int mPhrases;                     // number of phrases in the project
        private int mPages;                       // number of pages in the project
        private int mAnnotations;                 // number of annotations in the project

        /// <summary>
        /// The number of phrases in the project.
        /// </summary>
        public int Phrases
        {
            get { return mPhrases; }
        }

        /// <summary>
        /// The number of pages in the project.
        /// </summary>
        public int Pages
        {
            get { return mPages; }
        }

        /// <summary>
        /// The number of annotations in the project.
        /// </summary>
        public int Annotations
        {
            get { return mAnnotations; }
        }

        /// <summary>
        /// Create a new asset visitor for a given asset manager.
        /// </summary>
        public PhraseInitializer(Assets.AssetManager assManager, ErrorHandler errorHandler)
        {
            mAssManager = assManager;
            mErrorHandler = errorHandler;
            mPhrases = 0;
            mPages = 0;
            mAnnotations = 0;
        }

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
            if (node is PhraseNode)
            {
                ((PhraseNode)node).Asset = Assets.AudioMediaAsset.Empty;
                SequenceMedia media = (SequenceMedia)Project.GetMediaForChannel((CoreNode)node, Project.AUDIO_CHANNEL_NAME);
                if (media != null)
                {
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
                            ++mPhrases;
                        }
                        catch (Exception e)
                        {
                            mErrorHandler(e.Message);
                        }
                    }
                    if (clips.Count == media.getCount())
                    {
                        Assets.AudioMediaAsset asset = mAssManager.NewAudioMediaAsset(clips);
                        ((PhraseNode)node).Asset = asset;
                    }
                }
                // count pages, annotations, and updates the annotation on nodes that have one.
                if (((PhraseNode)node).PageProperty != null) ++mPages;
                string annotation =
                    ((TextMedia)Project.GetMediaForChannel((PhraseNode)node, Project.ANNOTATION_CHANNEL_NAME)).getText();
                if (annotation != null)
                {
                    ((PhraseNode)node).Annotation = annotation;
                    ++mAnnotations;
                }
            }
            return true;
        }
    }
}
