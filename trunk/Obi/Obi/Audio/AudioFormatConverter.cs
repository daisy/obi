using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using AudioLib;

namespace Obi.Audio
{
    public class AudioFormatConverter
    {
        public static string[] ConvertFile(string[] fileName, Presentation presentation) 
        {
            AudioLib.WavFormatConverter audioConverter = new WavFormatConverter(true);
            int samplingRate = (int)presentation.DataManager.getDefaultPCMFormat().getSampleRate();
            int channels = presentation.DataManager.getDefaultPCMFormat().getNumberOfChannels();
            int bitDepth = presentation.DataManager.getDefaultPCMFormat().getBitDepth();
            int numberOfFiles = fileName.Length;
            string convertedFiles = null;
            string[] listOfConvertedFiles = new string[numberOfFiles];
            string filePath;

            for (int i = 0; i < numberOfFiles; i++)
            {

                filePath = Path.GetDirectoryName(fileName[i]);
                try
                {
                    if (fileName[i].EndsWith(".wav"))
                    {
                        Stream wavStream = null;
                        wavStream = File.Open(fileName[i], FileMode.Open, FileAccess.Read, FileShare.Read);
                        urakawa.media.data.audio.PCMDataInfo newFilePCMInfo = urakawa.media.data.audio.PCMDataInfo.parseRiffWaveHeader(wavStream);
                        if (wavStream != null) wavStream.Close();
                        if (newFilePCMInfo.getSampleRate() == samplingRate && newFilePCMInfo.getNumberOfChannels() == channels && newFilePCMInfo.getBitDepth() == bitDepth)
                        {
                            convertedFiles = fileName[i];
                        }
                        else
                            convertedFiles = audioConverter.ConvertSampleRate(fileName[i], filePath, channels, samplingRate, bitDepth);
                    }
                    else if (fileName[i].EndsWith(".mp3"))
                    {
                        convertedFiles = audioConverter.UnCompressMp3File(fileName[i], filePath, channels, samplingRate, bitDepth);
                    }
                    listOfConvertedFiles[i] = convertedFiles;
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            return listOfConvertedFiles;
        }

    }
}
