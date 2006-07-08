using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UrakawaApplicationBackend;
using urakawa.core;
using urakawa.media;
using urakawa.project;

namespace Zaboom
{
    class Project: urakawa.project.Project
    {
        private AssetManager mAssManager;
        private Channel mAudioChannel;
        private Channel mTextChannel;

        public Project(AssetManager manager)
        {
            mAssManager = manager;
            
            mAudioChannel = getPresentation().getChannelFactory().createChannel("audio channel");
            getPresentation().getChannelsManager().addChannel(mAudioChannel);
            mTextChannel = getPresentation().getChannelFactory().createChannel("text channel");
            getPresentation().getChannelsManager().addChannel(mTextChannel);
            getPresentation().setMediaFactory(new AssetMediaFactory());
        }

        /// <summary>
        /// Append a new node from an audio file and return its name.
        /// </summary>
        /// <param name="path">The path of the audio file to add.</param>
        /// <returns>The name of the audio node that was added.</returns>
        /// <exception cref="Exception">When the asset cannot be created from the file.</exception>
        public string AddFile(string path)
        {
            AudioMediaAsset asset = new AudioMediaAsset(path);
            if (!asset.ValidateAudio())
                throw new Exception("Could not read audio asset from file `" + path + "', probably not a WAVE file.");
            asset = (AudioMediaAsset)mAssManager.CopyAsset(asset);
            mAssManager.RenameAsset(asset, Path.GetFileNameWithoutExtension(path));
            CoreNode node = getPresentation().getCoreNodeFactory().createNode();
            ChannelsProperty chprop = (ChannelsProperty)node.getProperty(typeof(ChannelsProperty));
            //AssetMediaFactory factory = (AssetMediaFactory)getPresentation().getMediaFactory();
            //AudioAsset audio = (AudioAsset)factory.createMedia(MediaType.AUDIO);
            AudioAsset audio = (AudioAsset)getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
            audio.Asset = asset;
            chprop.setMedia(mAudioChannel, audio);
            TextMedia text = (TextMedia)getPresentation().getMediaFactory().createMedia(MediaType.TEXT);
            text.setText(asset.Name);
            chprop.setMedia(mTextChannel, text);
            getPresentation().getRootNode().appendChild(node);
            return asset.Name;
        }
    }
}
