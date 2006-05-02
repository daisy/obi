using System;
using System.Collections;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using Microsoft.DirectX.AudioVideoPlayback;

namespace UrakawaApplicationBackend
{
	public class AudioPlayer : IAudioPlayer
	{
	
		Audio ob_Audio ;

		public Object OutputDevice
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		public double CurrentBytePosition
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}

		public double CurrentTimePosition
		{
			get
			{
				return ob_Audio.CurrentPosition;
			}
			set
			{
				ob_Audio.CurrentPosition = value;
			}
		}

		public AudioPlayerState State
		{
			get
			{
				return AudioPlayerState.Stopped;
			}
		}

		public ArrayList GetOutputDevices()
		{
			return null;
		}

		public void Play(IAudioMediaAsset asset)
		{
			ob_Audio = new Audio(asset.Path.ToString());
			ob_Audio.Play ();
		}
		
		public void Play(IAudioMediaAsset asset, double timeFrom)
		{
			ob_Audio = new Audio(asset.Path.ToString());			
			ob_Audio.CurrentPosition = timeFrom ;
			ob_Audio.Play ();
		}
		
		public void Play(IAudioMediaAsset asset, double timeForm, double timeTo)
		{
		}

		public void Play(Object buffer)
		{
		}

		public void Pause()
		{
		}

		public void Resume()
		{
		}

		public void Stop()
		{
			ob_Audio.Stop();
		}
	}
}
