using System.Collections.Generic;
using System.Text;
//using urakawa.media.data.audio;


namespace AudioLib
{
    /// <summary>
    /// Implementations of this interface can be used to resample WAV data, 
    /// or to uncompress WAV or MP3 data.
    /// </summary>
    public interface IWavFormatConverter
    {
        /// <summary>
        /// Returns progress information in the form of an integer from 0 to 100.
        /// </summary>
        int ProgressInfo
        {
            get;
        }

        /// <summary>
        /// Resamples the given wav file using the specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// Only works with uncompressed wav data.
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">cannot be null</param>
        /// <returns> absolute path to the new wave file </returns>
        string ConvertSampleRate ( string sourceFile, string destinationDirectory, int destChannels, int destSanplingRate, int destBitDepth );

        /// <summary>
        /// Uncompress the given wav file using the optional specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">can be null (in which case the PCM format of the given source is used)</param>
        /// <returns> absolute path to the new wave file </returns>
        string UnCompressWavFile(string sourceFile, string destinationDirectory, int destChannels ,int destSanplingRate, int destBitDepth) ;

        /// <summary>
        /// Uncompress the given mp3 file using the optional specified PCM format,
        /// and stores the result in the given directory with a random filename (the full path is returned by this method).
        /// </summary>
        /// <param name="sourceFile">cannot be null</param>
        /// <param name="destinationDirectory">cannot be null</param>
        /// <param name="destinationPCMFormat">can be null (in which case the PCM format of the given source is used)</param>
        /// <returns> absolute path to the new mp3 file </returns>
        string UnCompressMp3File(string sourceFile, string destinationDirectory, int destChannels ,int destSanplingRate, int destBitDepth) ;
    }
}
