using System;
using urakawa.media;
using UrakawaApplicationBackend;

namespace Zaboom
{
    class AudioAsset : AudioMedia
    {
        private AudioMediaAsset mAsset;

        public AudioMediaAsset Asset
        {
            get
            {
                return mAsset;
            }
            set
            {
                mAsset = value;
                setClipBegin(new Time());
                setClipEnd(new Time((long)Math.Round(mAsset.LengthInMilliseconds)));
                setLocation(new MediaLocation(mAsset.Path));
            }
        }

        public AudioAsset()
        {
        }
    }
}
