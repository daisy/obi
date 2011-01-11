using System;
using urakawa.media.data;
using urakawa.media.data.audio;

namespace Obi.Audio
{
    /// <summary>
    /// Convenience functions for audio editing and management
    /// </summary>
    /// sdk2
    //public class DataManager: MediaDataManager
    //{
    //    public override string getXukNamespaceUri() { return DataModelFactory.NS; }

    //    /// <summary>
    //    /// Create an empty audio media and manage it.
    //    /// </summary>
    //    /// <returns>The new managed empty media.</returns>
    //    public ManagedAudioMedia CreateEmptyAudioMedia()
    //    {
    //        AudioMediaData emptyData = (AudioMediaData)
    //            getMediaDataFactory().createMediaData(typeof(AudioMediaData));
    //        addMediaData(emptyData);
    //        ManagedAudioMedia emptyAudioMedia = (ManagedAudioMedia)
    //            getPresentation().getMediaFactory().createAudioMedia();
    //        emptyAudioMedia.setMediaData(emptyData);
    //        return emptyAudioMedia;
    //    }

    //    /// <summary>
    //    /// Copy a managed audio media and add it to the same manager.
    //    /// </summary>
    //    /// <param name="original">The original audio media data object.</param>
    //    /// <returns>The managed copy.</returns>
    //    public ManagedAudioMedia CopyAndManage(ManagedAudioMedia original)
    //    {
    //        ManagedAudioMedia copy = original.copy();
    //        //addMediaData(copy.getMediaData());
    //        return copy;
    //    }

    //    /// <summary>
    //    /// Merge two managed audio media objects.
    //    /// </summary>
    //    /// <param name="first">First audio media object (gets modified in place.)</param>
    //    /// <param name="second">Second audio media object (removed from its manager.)</param>
    //    public static void MergeAndManage(ManagedAudioMedia first, ManagedAudioMedia second)
    //    {
    //        first.mergeWith(second);
    //        second.getMediaData().delete();
    //    }

    //    /// <summary>
    //    /// Split an audio media object and manage the result.
    //    /// </summary>
    //    /// <param name="audio">The audio object to split.</param>
    //    /// <param name="time">The split time.</param>
    //    /// <returns>The new audio.</returns>
    //    public static ManagedAudioMedia SplitAndManage(ManagedAudioMedia audio, double time)
    //    {
    //        ManagedAudioMedia result = audio.split(new urakawa.media.timing.Time(time));
    //        audio.getMediaData().getMediaDataManager().addMediaData(result.getMediaData());
    //        return result;
    //    }
    //}
}
