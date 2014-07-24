using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
//using AudioLib;
using AudioLib;
using urakawa;
using urakawa.data;

namespace Obi.Audio
{
    //sdk2
    public class AudioFormatConverter
    {
        private static bool m_IsRequestCancellation;
        public static bool IsRequestCancellation
        {
            get { return m_IsRequestCancellation; }
            set { m_IsRequestCancellation = value; }
        }

        public static string[] ConvertFiles(string[] fileName, Presentation presentation)
        {
            m_IsRequestCancellation = false;
            int numberOfFiles = fileName.Length;
            string convertedFile = null;
            List<string> listOfConvertedFiles = new List<string>();


            for (int i = 0; i < numberOfFiles; i++)
            {
                if (IsRequestCancellation) return null;
                convertedFile = ConvertedFile(fileName[i], presentation);
                if (convertedFile != null) listOfConvertedFiles.Add(convertedFile);
            }
            string[] returnArray = new string[listOfConvertedFiles.Count];
            for (int i = 0; i < listOfConvertedFiles.Count; i++) returnArray[i] = listOfConvertedFiles[i];

            return returnArray;
        }
        public static string ConvertedFile(string filePath, Presentation pres)
        {
            AudioLib.WavFormatConverter audioConverter = new WavFormatConverter(true, true);
            int samplingRate = (int)pres.MediaDataManager.DefaultPCMFormat.Data.SampleRate;
            int channels = pres.MediaDataManager.DefaultPCMFormat.Data.NumberOfChannels;
            int bitDepth = pres.MediaDataManager.DefaultPCMFormat.Data.BitDepth;
            string directoryPath = pres.DataProviderManager.DataFileDirectoryFullPath;
            string convertedFile = null;
            try
            {
                if (Path.GetExtension(filePath).ToLower() == ".wav")
                {
                    Stream wavStream = null;
                    wavStream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                    uint dataLength;
                    AudioLibPCMFormat newFilePCMInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);
                    if (wavStream != null) wavStream.Close();
                    if (newFilePCMInfo.SampleRate == samplingRate && newFilePCMInfo.NumberOfChannels == channels && newFilePCMInfo.BitDepth == bitDepth)
                    {
                        convertedFile = filePath;
                    }
                    else
                    {
                        AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat((ushort)channels, (uint)samplingRate, (ushort)bitDepth);
                        AudioLibPCMFormat originalPCMFormat = null;
                        convertedFile = audioConverter.ConvertSampleRate(filePath, directoryPath, pcmFormat, out originalPCMFormat);
                    }
                }
                else if (Path.GetExtension(filePath).ToLower() == ".mp3")
                {
                    AudioLibPCMFormat pcmFormat = new AudioLibPCMFormat((ushort)channels, (uint)samplingRate, (ushort)bitDepth);
                    AudioLibPCMFormat originalPCMFormat = null;
                    convertedFile = audioConverter.UnCompressMp3File(filePath, directoryPath, pcmFormat, out originalPCMFormat);
                }
                else
                {
                    MessageBox.Show(string.Format(Localizer.Message("AudioFormatConverter_Error_FileExtentionNodSupported"), filePath), Localizer.Message("Caption_Error"));
                    return null;
                }
                // rename converted file to original file if names are different
                if (Path.GetFileName(filePath) != Path.GetFileName(convertedFile))
                {
                    string newConvertedFilePath = Path.Combine(Path.GetDirectoryName(convertedFile), Path.GetFileNameWithoutExtension(filePath) + ".wav");
                    for (int i = 0; i < 99999 && File.Exists(newConvertedFilePath); i++)
                    {
                        newConvertedFilePath = Path.Combine(Path.GetDirectoryName(convertedFile), i.ToString() + Path.GetFileNameWithoutExtension(filePath) + ".wav");
                        if (!File.Exists(newConvertedFilePath))
                        {
                            MessageBox.Show(string.Format(Localizer.Message("Import_AudioFormat_RenameFile"), Path.GetFileNameWithoutExtension(filePath) + ".wav", Path.GetFileName(newConvertedFilePath)),
                                Localizer.Message("Caption_Information"),
                                MessageBoxButtons.OK);
                            break;
                        }
                    }
                    File.Move(convertedFile, newConvertedFilePath);
                    convertedFile = newConvertedFilePath;
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
            return convertedFile;
        }

        private static System.ComponentModel.BackgroundWorker m_SpeechWorker = null;
        private static Settings m_Settings;
        private static AudioLib.TextToSpeech m_Tts;


        public static List<string> InstalledTTSVoices
        {
            get
            {
                if (m_Tts != null)
                {
                    return m_Tts.InstalledVoices;
                }
                else
                {
                    return new List<string>() ;
                }
            }
        }

        public static void InitializeTTS(Settings settings, AudioLibPCMFormat PCMFormat)
        {
            m_Settings = settings;
            
            m_Tts = new TextToSpeech(PCMFormat);
        }

        public static void  Speak( string    text, string filePath, Settings settings, AudioLibPCMFormat PCMFormat)
        {
            try
            {
                if (m_Tts != null && m_Tts.IsSynthesizerSpeaking)
                {
                    if (!string.IsNullOrEmpty(filePath))
                        return;
                    else
                        m_Tts.PauseAndDispose();
                    
                    m_Tts = null;
                }
                m_Settings = settings;
                if (m_SpeechWorker != null) m_SpeechWorker.DoWork -= new System.ComponentModel.DoWorkEventHandler(m_SpeechWorker_DoWork);
                m_SpeechWorker = new System.ComponentModel.BackgroundWorker();
                
                if(m_Tts == null )  InitializeTTS(settings,PCMFormat );
                List<string> inputStrings = new List<string>();
                inputStrings.Add(text);
                inputStrings.Add(filePath);

                if (string.IsNullOrEmpty(filePath))
                {
                    m_SpeechWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(m_SpeechWorker_DoWork);
                    m_SpeechWorker.RunWorkerAsync(inputStrings);
                }
                else
                {
                    GenerateSpeech(inputStrings);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void m_SpeechWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            GenerateSpeech((List<string>)e.Argument);
        }


        private static void GenerateSpeech (List<string> inputStrings )
        {
            if (m_Tts.InstalledVoices.Count == 0) return;
            //List<string> inputStrings = (List<string>)e.Argument;
            string selectedVoice = string.IsNullOrEmpty(m_Settings.Audio_TTSVoice) ? m_Tts.InstalledVoices[0] : m_Settings.Audio_TTSVoice;
            m_Tts.SpeakString(selectedVoice, inputStrings[0], inputStrings[1]);
            
        }

        public static void TestVoice(string text, string voice, Settings settings)
        {
            AudioLib.AudioLibPCMFormat audioFormat = new AudioLibPCMFormat((ushort)settings.AudioChannels, (uint)settings.AudioSampleRate, (ushort)settings.AudioBitDepth);
            m_Tts = new TextToSpeech(audioFormat);
            m_Tts.SpeakString(voice, text,null);
        }

        public enum AudioProcessingKind { Amplify, Normalize, SoundTouch } ;
        /// <summary>
        /// Create a copy of original audio file, process it and return the path of the copy
        /// </summary>
        /// <param name="processingKind"></param>
        /// <param name="presentation"></param>
        /// <param name="sourcePath"></param>
        /// <param name="processingFactor"></param>
        /// <returns></returns>
        public static void ProcessAudio(AudioProcessingKind processingKind, Presentation presentation, string sourcePath, float processingFactor)
        {

            AudioLib.DualCancellableProgressReporter audioProcess = null;
            if (processingKind == AudioProcessingKind.Amplify)
            {
                double amp = Convert.ToDouble(processingFactor);
                audioProcess = new WavAmplify(sourcePath, amp);
                
            }
            else if (processingKind == AudioProcessingKind.Normalize)
            {
                audioProcess = new WavNormalize(sourcePath, processingFactor);
            }
            else if (processingKind == AudioProcessingKind.SoundTouch)
            {
                audioProcess = new WavSoundTouch(sourcePath, processingFactor);
            }
            if (audioProcess != null)
            {
            audioProcess.DoWork();            
            }
        }

    }
}
