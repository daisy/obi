using System;
using System.Collections;
using System.Text;

using urakawa.core;
using urakawa.media;
using VirtualAudioBackend;

namespace Obi.Visitors
{
    class AssetVisitor: ICoreNodeVisitor
    {
        #region ICoreNodeVisitor Members

        public void postVisit(ICoreNode node)
        {
        }

        /// <summary>
        /// Recreate an asset from a sequential media object on a the audio channel of a phrase node.
        /// </summary>
        public bool preVisit(ICoreNode node)
        {
            if (Project.GetNodeType((CoreNode)node) == NodeType.Phrase)
            {
                SequenceMedia media = (SequenceMedia)Project.GetMediaForChannel((CoreNode)node, Project.AudioChannel);
                ArrayList clips = new ArrayList(media.getCount()); 
                for (int i = 0; i < media.getCount(); ++i)
                {
                    AudioMedia audio = (AudioMedia)media.getItem(i);
                    clips.Add(new AudioClip(audio.getLocation().Location, audio.getClipBegin().getAsMilliseconds(),
                        audio.getClipEnd().getAsMilliseconds()));
                }
                AudioMediaAsset asset = new AudioMediaAsset(clips);
                asset.Name = ((TextMedia)Project.GetMediaForChannel((CoreNode)node, Project.AnnotationChannel)).getText();
                AssetProperty assProp =
                    (AssetProperty)node.getPresentation().getPropertyFactory().createProperty("AssetProperty",
                    ObiPropertyFactory.ObiNS);
                assProp.Asset = asset;
                node.setProperty(assProp);
            }
            return true;
        }

        #endregion
    }
}
