using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using urakawa.xuk;
using urakawa.daisy;

namespace Obi.ImportExport
{
    public class ConfigurationFileParser
    {
        public class ExportParameters
        {
            public ExportParameters(ExportFormat exportStandards, 
                string exportDirectory, 
                bool encodeExportedAudioFiles, 
                AudioLib.AudioFileFormats encodingAudioFileFormat, 
                AudioLib.SampleRate exportSampleRate, 
                int exportChannels, 
                double exportEncodingBitrate)
            {
                m_ExportStandards = exportStandards;
                m_ExportDirectory = exportDirectory;
                m_EncodeExportedAudioFiles = encodeExportedAudioFiles;
                m_EncodingAudioFileFormat = encodingAudioFileFormat;
                m_ExportSampleRate = exportSampleRate;
                m_ExportChannels = exportChannels;
                m_ExportEncodingBitrate = exportEncodingBitrate;
            }

            private string m_ExportDirectory;
            public string ExportDirectory { get { return m_ExportDirectory; } }

            private ExportFormat m_ExportStandards;
            public ExportFormat ExportStandards { get { return m_ExportStandards; } }

            private bool m_EncodeExportedAudioFiles = false;
            public bool EncodeExportedAudioFiles { get { return m_EncodeExportedAudioFiles; } }

            private AudioLib.AudioFileFormats m_EncodingAudioFileFormat = AudioLib.AudioFileFormats.MP3;
            public AudioLib.AudioFileFormats EncodingAudioFileFormat { get { return m_EncodingAudioFileFormat; } }

            private AudioLib.SampleRate m_ExportSampleRate;
            public AudioLib.SampleRate ExportSampleRate { get { return m_ExportSampleRate; } }

            private int m_ExportChannels;
            public int ExportChannels { get { return m_ExportChannels; } }

            private double m_ExportEncodingBitrate;
            public double ExportEncodingBitrate { get { return m_ExportEncodingBitrate; } }

        }

        private string m_ConfigurationFilePath;

        public ConfigurationFileParser(string filePath)
        {
            m_ConfigurationFilePath = filePath;
        }

        public static ConfigurationFileParser GetConfigurationFileInstance(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                ConfigurationFileParser configurationFileInstance = new ConfigurationFileParser(filePath);
                configurationFileInstance.ParseXml();
                return configurationFileInstance;
            }
            return null;
        }

        public string ConfigurationFilePath { get { return m_ConfigurationFilePath; } }

        private string m_ObiProjectDirectoryPath;
        public string ObiProjectDirectoryPath { get { return m_ObiProjectDirectoryPath; } }

        private int m_ImportSampleRate;
        public int ImportSampleRate { get { return m_ImportSampleRate; } }

        private AudioLib.SampleRate m_ImportSampleRateEnum;
        public AudioLib.SampleRate ImportSampleRateEnum { get { return m_ImportSampleRateEnum; } }

        private int m_ImportChannels;
        public int ImportChannels { get { return m_ImportChannels; } }

        private ExportParameters m_DAISY3ExportParameters = null;
        public ExportParameters DAISY3ExportParameters { get { return m_DAISY3ExportParameters; } }

        private ExportParameters m_DAISY202ExportParameters = null;
        public ExportParameters DAISY202ExportParameters { get { return m_DAISY202ExportParameters; } }

        private ExportParameters m_EPUB3ExportParameters = null;
        public ExportParameters EPUB3ExportParameters { get { return m_EPUB3ExportParameters; } }

        public void ParseXml()
        {
            XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_ConfigurationFilePath, false, false);
            XmlNode importNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc.DocumentElement, true, "import", xmlDoc.DocumentElement.NamespaceURI);
            foreach (XmlNode n in importNode.ChildNodes)
            {
                string innerText = n.InnerText;

                if (n.LocalName == "obiprojectdirectory")
                {
                    m_ObiProjectDirectoryPath = innerText.Trim();
                }
                else if (n.LocalName == "audiosamplerate")
                {
                    string strSampleRate = innerText.Trim();
                    m_ImportSampleRate = int.Parse(strSampleRate);
                    m_ImportSampleRateEnum = strSampleRate == "44100" ? AudioLib.SampleRate.Hz44100 :
                        strSampleRate == "22050" ? AudioLib.SampleRate.Hz22050
                        : AudioLib.SampleRate.Hz11025;
                }
                else if (n.LocalName == "audiochannels")
                {
                    string strChannels= innerText.Trim();
                    m_ImportChannels = int.Parse(strChannels.Trim());
                }
            }

            foreach (XmlNode exportNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(xmlDoc.DocumentElement, true, "export", xmlDoc.DocumentElement.NamespaceURI, false))
            {
                ExportFormat exportStandards = ExportFormat.DAISY3_0;
                string exportDirectory = null;
                bool encodeExportedAudioFiles = false;
                AudioLib.AudioFileFormats encodingAudioFileFormat = AudioLib.AudioFileFormats.WAV;
                AudioLib.SampleRate exportSampleRate = AudioLib.SampleRate.Hz44100;
                int exportChannels = 1;
                double exportEncodingBitrate = 64;
                foreach (XmlNode n in exportNode.ChildNodes)
                {
                    if (n.LocalName == "directory")
                    {
                        exportDirectory = n.InnerText.Trim();
                    }
                    else if (n.LocalName == "standard")
                    {
                        string strStandard = n.InnerText.Trim();
                        exportStandards = strStandard == "daisy2.02" ? ExportFormat.DAISY2_02 :
                            strStandard == "daisy3" ? ExportFormat.DAISY3_0 :
                            ExportFormat.EPUB3;
                    }
                    else if (n.LocalName == "audioencoding")
                    {
                        string strEncoding = n.InnerText.Trim();
                        encodeExportedAudioFiles = strEncoding.ToLower() != "wav";

                        if (encodeExportedAudioFiles)
                        {
                            encodingAudioFileFormat = (strEncoding.ToLower() == "mp4" || strEncoding.ToLower() == "m4a") ? AudioLib.AudioFileFormats.MP4 :
                                AudioLib.AudioFileFormats.MP3;
                        }
                    }
                    else if (n.LocalName == "bitrate")
                    {
                        string strBitrate = n.InnerText.Trim();
                        exportEncodingBitrate = int.Parse(strBitrate);
                    }
                    else if (n.LocalName == "audiosamplerate")
                    {
                        string strSampleRate = n.InnerText;
                        exportSampleRate = strSampleRate == "44100" ? AudioLib.SampleRate.Hz44100 :
                            strSampleRate == "22050" ? AudioLib.SampleRate.Hz22050
                            : AudioLib.SampleRate.Hz11025;
                    }
                    else if (n.LocalName == "audiochannels")
                    {
                        string strChannels = n.InnerText;
                        exportChannels = int.Parse(strChannels.Trim());
                    }
                }
                ExportParameters exportObject = new ExportParameters(exportStandards,
                    exportDirectory,
                    encodeExportedAudioFiles,
                    encodingAudioFileFormat,
                    exportSampleRate,
                    exportChannels,
                    exportEncodingBitrate);

                // assign export parameters to respective properties
                if (exportObject.ExportStandards == ExportFormat.DAISY3_0)
                {
                    m_DAISY3ExportParameters = exportObject;
                }
                else if (exportObject.ExportStandards == ExportFormat.DAISY2_02)
                {
                    m_DAISY202ExportParameters = exportObject;
                }
                else if (exportObject.ExportStandards == ExportFormat.EPUB3)
                {
                    m_EPUB3ExportParameters = exportObject;
                }

            }
            xmlDoc = null;
        }

    }
}
