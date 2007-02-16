using System;
using System.Windows.Forms;
using System.IO;

namespace Obi.Assets
{
    /// <summary>
    /// Only Audio is known at the moment. Anything else is "other" and out of scope.
    /// </summary>
    public enum MediaType { Audio, Other };

    /// <summary>
    /// Base class for media assets.
    /// </summary>
    public abstract class MediaAsset
    {
        protected string mName;
        protected MediaType mMediaType;
        protected long mSizeInBytes;
        internal AssetManager mAssManager;

        /// <summary>
        /// Name of the asset. Unique in the manager.
        /// </summary>
        public string Name
        {
            get { return mName; }
            set { mName = value; }
        }

        /// <summary>
        /// Type of the asset.
        /// </summary>
        public MediaType Type
        {
            get { return mMediaType; }
        }

        /// <summary>
        /// Total size in bytes of the asset.
        /// </summary>
        public long SizeInBytes
        {
            get { return mSizeInBytes; }
            set { mSizeInBytes = value; }
        }

        /// <summary>
        /// Manager for this asset.
        /// </summary>
        public AssetManager Manager
        {
            get { return mAssManager; }
            set { mAssManager = value; }
        }

        /// <summary>
        /// Copy the asset.
        /// </summary>
        /// <returns>The copy</returns>
        public abstract MediaAsset Copy();

        /// <summary>
        /// Delete the asset.
        /// </summary>
        public abstract void Delete();

        /// <summary>
        /// Merge with another asset (normally, the next one in sequence.)
        /// </summary>
        /// <param name="next">The asset to merge with.</param>
        public abstract void MergeWith(MediaAsset next);

        /// <summary>
        /// Format a time value. If less than a minute, display seconds and milliseconds.
        /// If less than an hour, display minutes and seconds. Otherwise show hh:mm:ss.
        /// </summary>
        /// <param name="time">The time to display (in milliseconds.)</param>
        /// <returns>The formatted string.</returns>
        public static string FormatTime(double time)
        {
            return time < 60000.0 ? FormatTime_ss_ms(time) :
                // time < 3600000.0 ? FormatTime_mm_ss(time) :
                FormatTime_hh_mm_ss(time);
        }

        /// <summary>
        /// Convenient function to format a milliseconds time into hh:mm:ss format.
        /// </summary>
        /// <param name="time">The time in milliseconds.</param>
        /// <returns>The time in hh:mm:ss format (fractions of seconds are discarded.)</returns>
        public static string FormatTime_hh_mm_ss(double time)
        {
            int s = Convert.ToInt32(time / 1000.0);
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(s / 60);
            str = (m % 60).ToString("00") + ":" + str;
            int h = m / 60;
            return h.ToString("00") + ":" + str;
        }

        private static string FormatTime_mm_ss(double time)
        {
            int s = Convert.ToInt32(Math.Floor(time / 1000.0));
            string str = (s % 60).ToString("00");
            int m = Convert.ToInt32(Math.Floor(s / 60.0));
            return m.ToString("00") + ":" + str;
        }

        private static string FormatTime_ss_ms(double time)
        {
            time /= 1000.0;
            return time.ToString("0.00") + "s";
        }
    }
}