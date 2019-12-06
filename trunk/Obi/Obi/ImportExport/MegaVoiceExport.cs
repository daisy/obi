using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using urakawa;
using urakawa.core;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.xuk;
using urakawa.daisy;
using urakawa.daisy.export;
using AudioLib;

namespace Obi.ImportExport
{
    public class MegaVoiceExport : DAISY3_ObiExport
    {
        protected int m_AudioFileSectionLevel;
        protected Dictionary<XmlNode, urakawa.core.TreeNode> m_AnchorXmlNodeToReferedNodeMap = new Dictionary<XmlNode, urakawa.core.TreeNode>();
        protected Dictionary<urakawa.core.TreeNode, string> m_Skippable_UpstreamIdMap = new Dictionary<TreeNode, string>();
        protected Dictionary<XmlDocument, string> m_AnchorSmilDoc_SmileFileNameMap = new Dictionary<XmlDocument, string>();
        private string m_MegaVoiceExportPath = null;

        public MegaVoiceExport(ObiPresentation presentation, string exportDirectory, List<string> navListElementNamesList, bool encodeToMp3, double mp3BitRate,
            SampleRate sampleRate, bool stereo, bool skipACM, int audioFileSectionLevel, string megaVoiceExportPath)
            : base(presentation, exportDirectory, navListElementNamesList, encodeToMp3, mp3BitRate,
            sampleRate, stereo, skipACM, audioFileSectionLevel)
        {
            m_Filename_Content = null;
            m_AudioFileSectionLevel = audioFileSectionLevel;
            GeneratorName = "Obi";
            m_MegaVoiceExportPath = megaVoiceExportPath;
        }


protected override void CreateOpfDocument()
        {
            base.CreateOpfDocument();
            try
            {
                if (m_MegaVoiceExportPath != null)
                {
                    if (!Directory.Exists(m_MegaVoiceExportPath))
                    {
                        Directory.CreateDirectory(m_MegaVoiceExportPath);
                    }
                    foreach (string str in m_FilesList_SmilAudio)
                    {
                        String strFilePath = Path.Combine(m_OutputDirectory, str);
                        System.Windows.Forms.MessageBox.Show(strFilePath);
                        if (File.Exists(strFilePath))
                        {
                            string destinationFilePath = Path.Combine(m_MegaVoiceExportPath, Path.GetFileName(strFilePath));


                            File.Copy(strFilePath, destinationFilePath, true);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

    }
}
