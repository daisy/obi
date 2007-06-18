using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using urakawa;
using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.utilities;
using urakawa.metadata;
using urakawa.properties.channel;

namespace Zaboom
{
    public class Project: urakawa.Project
    {
        private bool canSave;

        public Project(string title, Uri uri)
            : base(uri)
        {
            canSave = true;
            Metadata metatitle = getMetadataFactory().createMetadata();
            metatitle.setName("zaboom:title");
            metatitle.setContent(title);
            appendMetadata(metatitle);
            Channel audio = getPresentation().getChannelFactory().createChannel(typeof(Channel).Name,
                urakawa.ToolkitSettings.XUK_NS);
            audio.setName(AUDIO_CHANNEL_NAME);
            getPresentation().getChannelsManager().addChannel(audio);
        }

        public void ImportAudioFile(string path)
        {
            // Stream input = File.OpenRead(path);
            // PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader(input);
            // input.Close();
            // getPresentation().getMediaDataManager().getDefaultPCMFormat().setSampleRate(info.getSampleRate());
            AudioMediaData data = (AudioMediaData)
                getPresentation().getMediaDataFactory().createMediaData(typeof(AudioMediaData));
            // data.getPCMFormat().setBitDepth(info.getBitDepth());
            // data.getPCMFormat().setNumberOfChannels(info.getNumberOfChannels());
            // data.getPCMFormat().setSampleRate(info.getSampleRate());
            data.appendAudioDataFromRiffWave(path);
            ManagedAudioMedia media = (ManagedAudioMedia)
                getPresentation().getMediaFactory().createMedia(MediaType.AUDIO);
            media.setMediaData(data);
            Channel audio = GetSingleChannelByName(AUDIO_CHANNEL_NAME);
            ChannelsProperty prop = getPresentation().getPropertyFactory().createChannelsProperty();
            prop.setMedia(audio, media);
            TreeNode node = getPresentation().getTreeNodeFactory().createNode();
            node.setProperty(prop);
            getPresentation().getRootNode().appendChild(node);
        }

        public void Save()
        {
            if (canSave)
            {
                string directory = Path.GetDirectoryName(getPresentation().getBaseUri().LocalPath);
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
                saveXUK(getPresentation().getBaseUri());
                canSave = false;
            }
        }

        public string Title { get { return GetSingleMetadataContentFor(META_TITLE_NAME); } }
        public string TitleSaved { get { return GetSingleMetadataContentFor(META_TITLE_NAME) + (canSave ? "*" : ""); } }

        #region utilities

        private static readonly string AUDIO_CHANNEL_NAME = "zaboom.audio";
        private static readonly string META_TITLE_NAME = "zaboom:title";

        private Channel GetSingleChannelByName(string name)
        {
            List<Channel> channels = getPresentation().getChannelsManager().getChannelByName(name);
            if (channels.Count != 1)
            {
                throw new Exception(String.Format("Expected 1 channel for {0}, got {1}.", name, channels.Count));
            }
            return channels[0];
        }

        private string GetSingleMetadataContentFor(string name)
        {
            List<Metadata> list = getMetadataList(name);
            if (list.Count != 1)
            {
                throw new Exception(String.Format("Expected 1 item for {0}, got {1}.", name, list.Count));
            }
            return list[0].getContent();
        }

        #endregion

    }
}
