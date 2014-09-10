using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
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

        private AudioLib.SampleRate m_ImportSampleRate;
        public AudioLib.SampleRate ImportSampleRate { get { return m_ImportSampleRate; } }

        private int m_ImportChannels;
        public int ImportChannels { get { return m_ImportChannels; } }

        private string m_ExportDirectory;
        public string ExportDirectory { get { return m_ExportDirectory; } }

        private ExportFormat m_ExportStandards;
        public ExportFormat ExportStandards { get { return m_ExportStandards ; }}

        private AudioLib.SampleRate m_ExportSampleRate;
        public AudioLib.SampleRate ExportSampleRate { get { return m_ExportSampleRate; } }

        private int m_ExportChannels;
        public int ExportChannels { get { return m_ExportChannels; } }

        public void ParseXml()
        {
            XmlDocument xmlDoc = XmlReaderWriterHelper.ParseXmlDocument(m_ConfigurationFilePath, false, false);
            XmlNode importNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlDoc.DocumentElement, true, "import", xmlDoc.DocumentElement.NamespaceURI);
            foreach (XmlNode n in importNode.ChildNodes)
            {
                string innerText = n.InnerText;

                if (n.LocalName == "audiosamplerate")
                {
                    string strSampleRate = innerText.Trim();
                    m_ImportSampleRate = strSampleRate == "44100" ? AudioLib.SampleRate.Hz44100 :
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
