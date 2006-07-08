using System;
using urakawa.media;

namespace Zaboom
{
    class AssetMediaFactory : MediaFactory
    {
        public override IMedia createMedia(urakawa.media.MediaType type)
        {
            if (type == MediaType.AUDIO)
            {
                return new AudioAsset();
            }
            else
            {
                return base.createMedia(type);
            }
        }
    }
}
