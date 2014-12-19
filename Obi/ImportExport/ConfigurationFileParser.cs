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

        private string m_ExportDirectory;
        public string ExportDirectory { get { return m_ExportDirectory; } }

        private ExportFormat m_ExportStandards;
        public ExportFormat ExportStandards { get { return m_ExportStandards ; }}

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

            XmlNode exportNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc.DocumentElement, true, "export", xmlDoc.DocumentElement.NamespaceURI);

            foreach (XmlNode n in exportNode.ChildNodes)
            {
                if (n.LocalName == "directory")
                {
                    m_ExportDirectory = n.InnerText.Trim() ;
                }
                else if (n.LocalName == "standard")
                {
                    string strStandard = n.InnerText.Trim ();
                    m_ExportStandards = strStandard == "daisy2.02" ? ExportFormat.DAISY2_02 :
                        strStandard == "daisy3" ? ExportFormat.DAISY3_0 :
                        ExportFormat.EPUB3;
                }
                else if (n.LocalName == "audioencoding")
                {
                    string strEncoding = n.InnerText.Trim();
                    m_EncodeExportedAudioFiles = strEncoding.ToLower() != "wav";

                    if (m_EncodeExportedAudioFiles)
                    {
                        m_EncodingAudioFileFormat = (strEncoding.ToLower() == "mp4" || strEncoding.ToLower() == "m4a") ? AudioLib.AudioFileFormats.MP4 :
                            AudioLib.AudioFileFormats.MP3;
                    }
                }
                else if (n.LocalName == "bitrate")
                {
                    string strBitrate = n.InnerText.Trim();
                    m_ExportEncodingBitrate = int.Parse(strBitrate);
                }
                else if (n.LocalName == "audiosamplerate")
                {
                    string strSampleRate = n.InnerText;
                    m_ExportSampleRate = strSampleRate == "44100" ? AudioLib.SampleRate.Hz44100 :
                        strSampleRate == "22050" ? AudioLib.SampleRate.Hz22050
                        : AudioLib.SampleRate.Hz11025;
                }
                else if (n.LocalName == "audiochannels")
                {
                    string strChannels= n.InnerText;
                    m_ExportChannels = int.Parse(strChannels.Trim());
                }
            }
            xmlDoc = null;
        }

    }
}
