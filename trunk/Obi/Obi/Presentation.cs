using System;
using System.IO;
using urakawa.media.data.audio;

namespace Obi
{
    public class Presentation: urakawa.Presentation
    {
        public Presentation()
            : base()
        {
            setRootNode(new RootNode(this));
        }

        /// <summary>
        /// Root node of the presentation.
        /// </summary>
        public RootNode RootNode { get { return (RootNode)getRootNode(); } }

        /// <summary>
        /// The media data manager for the project.
        /// </summary>
        public Audio.DataManager DataManager { get { return (Audio.DataManager)getMediaDataManager(); } }


        /// <summary>
        /// Create a phrase node belonging to this presentation.
        /// </summary>
        public PhraseNode CreatePhraseNode()
        {
            return (PhraseNode)getTreeNodeFactory().createNode(PhraseNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
        }

        /// <summary>
        /// Convenience method to create a new plain phrase node from a file.
        /// </summary>
        public PhraseNode CreatePhraseNode(string path) { return CreatePhraseNode(ImportAudioFromFile(path)); }

        /// <summary>
        /// Create a section node belonging to this presentation.
        /// </summary>
        public SectionNode CreateSectionNode()
        {
            return (SectionNode)getTreeNodeFactory().createNode(SectionNode.XUK_ELEMENT_NAME, DataModelFactory.NS);
        }


        // Create a new phrase node from an audio media.
        private PhraseNode CreatePhraseNode(urakawa.media.data.audio.ManagedAudioMedia audio)
        {
            PhraseNode node = CreatePhraseNode();
            node.Audio = audio;
            return node;
        }

        // Create a title section with a string as its title.
        private void CreateTitleSection(string title)
        {
            SectionNode node = CreateSectionNode();
            node.Label = title;
            RootNode.AppendChild(node);
        }

        // Create a media object from a sound file.
        private ManagedAudioMedia ImportAudioFromFile(string path)
        {
            if (!DataManager.getEnforceSinglePCMFormat())
            {
                Stream input = File.OpenRead(path);
                PCMDataInfo info = PCMDataInfo.parseRiffWaveHeader(input);
                input.Close();
                getMediaDataManager().getDefaultPCMFormat().setBitDepth(info.getBitDepth());
                getMediaDataManager().getDefaultPCMFormat().setNumberOfChannels(info.getNumberOfChannels());
                getMediaDataManager().getDefaultPCMFormat().setSampleRate(info.getSampleRate());
                DataManager.setEnforceSinglePCMFormat(true);
            }
            AudioMediaData data = (AudioMediaData)getMediaDataFactory().createMediaData(typeof(AudioMediaData));
            data.appendAudioDataFromRiffWave(path);
            ManagedAudioMedia media = (ManagedAudioMedia)getMediaFactory().createAudioMedia();
            media.setMediaData(data);
            return media;
        }
    }
}
