using System;
using System.Collections;
using System.Text;

namespace UrakawaApplicationBackend
{
	/// <summary>
	/// The three states of the audio player.
	/// Playing: sound is currently playing.
	/// Paused: playback was interrupted and can be resumed.
	/// Stopped: player is idle.
	/// </summary>
	public enum AudioPlayerState { Playing, Paused, Stopped };

	/// <summary>
	/// Audio player interface.
	/// </summary>
    public interface IAudioPlayer
    {
		/// <summary>
		/// Get and set the current output device.
		/// </summary>
		// Replace Object by the actual audio device class
		Object OutputDevice
		{
			get;
			set;
		}

		/// <summary>
		/// Get and set the current byte position.
		/// Setting the position allows to play from any point in the sound.
		/// Must handle the case where we try to set the position after the end or before the beginning.
		/// When playing or paused, the position can be anywhere between the start and the end of the asset being played.
		/// When stopped, the position is 0.
		/// </summary>
		double CurrentBytePosition
		{
			get;
			set;
		}		

		/// <summary>
		/// Same as above, but the unit is milliseconds and not bytes.
		/// </summary>
		double CurrentTimePosition
		{
			get;
			set;
		}

		/// <summary>
		/// The current state of the player (playing, paused or stopped.)
		/// </summary>
		AudioPlayerState State
		{
			get;
		}

		/// <summary>
		/// Return a list of devices available for sound playing.
		/// The device for the audio player can be set from one of these values.
		/// </summary>
		/// <returns>The list of available output devices.</returns>
		ArrayList GetOutputDevices();  

		/// <summary>
		/// Start playing an asset from the beginning to the end.
		/// The output device must be set before playing. (This applies to all forms of the Play method.)
		/// </summary>
		/// <param name="asset">The asset to play.</param>
		void Play(IAudioMediaAsset asset);

		/// <summary>
		/// Play an asset starting from a given time.
		/// </summary>
		/// <param name="asset">The asset to play.</param>
		/// <param name="timeFrom">The time to start from.</param>
		void Play(IAudioMediaAsset asset, double timeFrom);

		/// <summary>
		/// Play an asset starting from a given time and ending at a given time.
		/// </summary>
		/// <param name="asset">The asset to play.</param>
		/// <param name="timeForm">The time to start from.</param>
		/// <param name="timeTo">The time to end at.</param>
		void Play(IAudioMediaAsset asset, double timeForm, double timeTo);

		/// <summary>
		/// Play an audio buffer (presumably from a selection.)
		/// Note: the type of the buffer needs to be a byte buffer type, with enough information to allow playback
		/// (sample rate and bit depth at least?)
		/// </summary>
		/// <param name="buffer">The buffer to play.</param>
		void Play(Object buffer);

		/// <summary>
		/// Pause playing. No effect if already paused, or if no sound is playing.
		/// State after Pause() can only be Paused or Stopped. 
		/// </summary>
		void Pause();

		/// <summary>
		/// Resume playing. No effect if not previously paused.
		/// State after Resume() can only be Playing or Stopped.
		/// </summary>
		void Resume();

		/// <summary>
		/// Stop playing, even if the player was paused.
		/// State after Stop() can only be Stopped.
		/// </summary>
		void Stop();
    }
}
