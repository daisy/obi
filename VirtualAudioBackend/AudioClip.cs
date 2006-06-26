using System;

namespace VirtualAudioBackend
{
	/// <summary>
	/// Audio clips.
	/// </summary>
	public class AudioClip
	{
		/// <summary>
		/// The path of the audio file that this clip is part of.
		/// </summary>
		public string Path
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Begin time of the clip in milliseconds.
		/// </summary>
		public double BeginTime
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// End time of the clip in milliseconds.
		/// </summary>
		public double EndTime
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// Create a new AudioClip object from an existing audio file.
		/// </summary>
		/// <param name="path">Path of the audio file.</param>
		/// <param name="beginTime">Begin time of the clip in milliseconds.</param>
		/// <param name="endTime">End time of the clip in milliseconds.</param>
		public AudioClip(string path, double beginTime, double endTime)
		{
		}

		/// <summary>
		/// Make a copy of the audio clip.
		/// </summary>
		/// <returns>A copy of the clip.</returns>
		public AudioClip Copy()
		{
			return null;
		}

		/// <summary>
		/// Split an audio clip at the given time (in millisecond.)
		/// </summary>
		/// <param name="time"></param>
		/// <returns>The new clip (second half); the first clip has been modified.</returns>
		public AudioClip Split(double time)
		{
			return null;
		}

		/// <summary>
		/// Merge two audio clips. The end time of this clip must match the begin time of the next, and of course the files must match.
		/// </summary>
		/// <param name="next">The next clip to merge with.</param>
		public void MergeWith(AudioClip next)
		{
		}
	}
}