using System.Collections.Generic;
using urakawa.media.data;

namespace Obi.Audio
{
    public class PhraseDetection
    {
        public static readonly double DEFAULT_GAP = 500.0;              // default gap for phrase detection
        public static readonly double DEFAULT_LEADING_SILENCE = 100.0;  // default leading silence

        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, long length, long before)
        {
            /*AudioClip ob_Clip;
            // AssetList is list of assets returned by phrase detector
            ArrayList alAssetList = new ArrayList();
            // clipList is clip list for each return asset
            ArrayList alClipList;
            AudioMediaAsset ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);

            // apply phrase detection on each clip in clip list of this asset
            for (int i = 0; i < mClips.Count; ++i)
            {
                ob_Clip = mClips[i];
                alClipList = ob_Clip.DetectPhrases(threshold, length, before);
                if (Convert.ToBoolean(alClipList[0]) == false)
                {
                    ob_Asset.AddClip(alClipList[1] as AudioClip);
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                    }
                }
                else
                {
                    if (ob_Clip.BeginTime + 3000 < (alClipList[1] as AudioClip).BeginTime)
                    {
                        ob_Asset.AddClip(ob_Clip.CopyClipPart(0, (alClipList[1] as AudioClip).BeginTime - ob_Clip.BeginTime));
                        if (i == 0)
                            alAssetList.Add(ob_Asset);
                    }
                    if (i != 0)
                        alAssetList.Add(ob_Asset);
                    for (int j = 1; j < alClipList.Count - 1; j++)
                    {
                        ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
                        ob_Asset.AddClip(alClipList[j] as AudioClip);
                        alAssetList.Add(ob_Asset);
                    }
                    ob_Asset = new AudioMediaAsset(mChannels, mBitDepth, mSamplingRate);
                    if (alClipList.Count > 2)
                        ob_Asset.AddClip(alClipList[alClipList.Count - 1] as AudioClip);
                    if (i == mClips.Count - 1 && ob_Asset.Clips != null)
                    {
                        alAssetList.Add(ob_Asset);
                    }
                }
            }
            List<AudioMediaAsset> RetList = new List<AudioMediaAsset>();
            for (int n = 0; n < alAssetList.Count; n++)
            {
                RetList.Add(alAssetList[n] as AudioMediaAsset);
            }
            return RetList;*/
            return new List<ManagedAudioMedia>();
        }

        public static List<ManagedAudioMedia> Apply(ManagedAudioMedia audio, long threshold, double length, double before)
        {
            return new List<ManagedAudioMedia>();
        }

        /// <summary>
        /// Get the maximum amplitude of silence in a given "silent" asset.
        /// </summary>
        /// <returns>The amplitude.</returns>
        public static long GetSilenceAmplitude(ManagedAudioMedia audio)
        {
            long max = 0;
            /*foreach (AudioClip clip in mClips)
            {
                long amplitude = clip.GetClipSilenceAmplitude();
                if (amplitude > max) max = amplitude;
            }
            max = max + 10;*/
            return max;
        }

    }
}
