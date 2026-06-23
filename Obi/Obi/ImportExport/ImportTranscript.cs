using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Diagnostics;

using urakawa.core;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.audio.codec;
using urakawa.data;
using urakawa.media.timing;
using urakawa.property;
using urakawa.property.channel;
using urakawa.daisy;
using urakawa.daisy.import;
using urakawa.xuk;
using AudioLib;


namespace Obi.ImportExport
{
    public class ImportTranscript : DualCancellableProgressReporter
    {
        private string m_NccPath;
        private string m_NccBaseDirectory;
        private ObiPresentation m_Presentation;            // target presentation
        private SectionNode m_CurrentSection;               // section currently populated
        private Stack<Obi.SectionNode> m_OpenSectionNodes;  // these are section nodes that could accept children
      //  private Dictionary<SectionNode, string> m_SectionNodesToSmilReferenceMap;
        private Dictionary<string, EmptyNode> m_NccReferenceToPageMap = new Dictionary<string, EmptyNode>();
        private AudioFormatConvertorSession m_AudioConversionSession;
        private Dictionary<string, FileDataProvider> m_OriginalAudioFile_FileDataProviderMap = new Dictionary<string, FileDataProvider>(); // maps original audio file refered by smil to FileDataProvider of sdk.
        private int m_Counter_FrontPageCorection;
        private Settings m_Settings;
        protected List<TreeNode> TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();
        private string m_AudioFilePath;
        private List<PhraseNode> m_Phrases;

        public ImportTranscript(string nccPath, ObiPresentation presentation, Settings settings, string filePath)
        {
            m_NccPath = nccPath;
            m_NccBaseDirectory = Path.GetDirectoryName(nccPath);
            m_OpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            m_Presentation = presentation;
         //   m_SectionNodesToSmilReferenceMap = new Dictionary<SectionNode, string>();
            m_AudioConversionSession = new AudioFormatConvertorSession(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                m_Presentation.MediaDataManager.DefaultPCMFormat,
                false,
                false);
            m_Settings = settings;
            m_Counter_FrontPageCorection = 0;
            m_AudioFilePath = filePath;

        }

        private List<string> m_ErrorsList = new List<string>();
        public List<string> ErrorsList { get { return m_ErrorsList; } }

        public override void DoWork()
        {
            if (RequestCancellation) return;
            ImportFromXHTML();
        }

        public List<PhraseNode> Phrases
        {
            get { return m_Phrases; }
        }

        private void ImportFromXHTML()
        {
            try
            {
                XmlDocument nccDocument = null;
                if (m_Settings.Project_ImportNCCFileWithWindows1252Encoding)
                {
                    // some old files may have 7 bit encoding.
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    StreamReader sr = new StreamReader(m_NccPath, Encoding.GetEncoding(1252), false);
                    string xmlData = sr.ReadToEnd();
                    nccDocument = XmlReaderWriterHelper.ParseXmlDocumentFromString(xmlData, false, false);
                }
                else
                {
                    nccDocument = XmlReaderWriterHelper.ParseXmlDocument(m_NccPath, false, false);
                }

                XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(nccDocument.DocumentElement, true, "head", nccDocument.DocumentElement.NamespaceURI);
                PopulateMetadataFromNcc(headNode);
                XmlNode bodyNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(nccDocument.DocumentElement, true, "body", nccDocument.DocumentElement.NamespaceURI);
                ParseNccDocument(bodyNode);
                if (RequestCancellation) return;
                reportProgress(20, "");
             //   AppendPhrasesFromSmil();
                reportProgress(100, "");
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void PopulateMetadataFromNcc(XmlNode headNode)
        {
            Dictionary<string, string> referenceList = new Dictionary<string, string>();
            m_Phrases = new List<PhraseNode>();
            foreach (string s in Metadata.DAISY3MetadataNames)
            {
                if (s == Metadata.DC_IDENTIFIER || s == Metadata.DC_TITLE
                    || s == Metadata.DC_FORMAT || s == Metadata.DC_TYPE || s == Metadata.DTB_AUDIO_FORMAT
                    || s == Metadata.DTB_MULTIMEDIA_CONTENT || s == Metadata.DTB_MULTIMEDIA_TYPE || s == Metadata.DTB_TOTAL_TIME || s == Metadata.GENERATOR)
                {
                    continue;
                }
                string[] arrayString = s.Split(':');
                string name = (arrayString.Length > 1 ? arrayString[1] : arrayString[0]).ToLower();
                referenceList.Add(name, s);
                //Console.WriteLine(name);
            }
            foreach (XmlNode metaNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(headNode, true, "meta", headNode.NamespaceURI, false))
            {
                string name = metaNode.Attributes.GetNamedItem("name") != null ? metaNode.Attributes.GetNamedItem("name").Value : null;
                string content = metaNode.Attributes.GetNamedItem("content") != null ? metaNode.Attributes.GetNamedItem("content").Value : null;
                if (name == null) continue;
                string[] arrayString = name.Split(':');
                string truncatedName = (arrayString.Length > 1 ? arrayString[1] : arrayString[0]).ToLower();
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(content)
                    && referenceList.ContainsKey(truncatedName.ToLower()))
                {
                    m_Presentation.SetSingleMetadataItem(referenceList[truncatedName.ToLower()], content);
                }
            }
        }

        private void ParseNccDocument(XmlNode node)
        {
        //    string filePath = @"D:\Obi Books\The_Countries_New_Zealand.wav";
            if (node.ChildNodes.Count == 0) return;
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (n.LocalName == "h1" || n.LocalName == "h2" || n.LocalName == "h3" ||
                        n.LocalName == "h4" || n.LocalName == "h5" || n.LocalName == "h6")
                    {
                        //foundHeadings = true;
                        Obi.SectionNode section = CreateSectionNode(n);
                        if (section != null && section.Level < 6) m_OpenSectionNodes.Push(section);
                        m_CurrentSection = section;
                     //   m_SectionNodesToSmilReferenceMap.Add(section, GetSmilReferenceString(n));

                    }
                    else if (n.LocalName == "p" || n.LocalName == "span")
                    {
                        if (n.Attributes.GetNamedItem("class") != null)
                        {
                            string classAttr = n.Attributes.GetNamedItem("class").Value;
                            if (classAttr == "phrase")
                            {

                                string phraseStartTime = n.Attributes.GetNamedItem("data-start").Value;
                                string phraseEndTime = n.Attributes.GetNamedItem("data-end").Value;
                                if (phraseStartTime != null && phraseEndTime != null && m_AudioFilePath != null)
                                {
                                    double start = Convert.ToDouble(phraseStartTime);

                                    double end = Convert.ToDouble(phraseEndTime);
                                    XmlElement audio = AttachAudioToPhraseNode(m_AudioFilePath, start, end);
                                    PhraseNode phraseNode = CreatePhraseNodeFromAudioElement(m_CurrentSection, audio);
                                    m_Phrases.Add(phraseNode);
                                }
                                else
                                {
                                    EmptyNode empty = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                                    m_CurrentSection.AppendChild(empty);
                                }
                            }
                            else if (classAttr == "page" || classAttr == "page-normal")
                            {
                                addPage(n, PageKind.Normal);
                            }
                            else if (classAttr == "page-front")
                            {
                                addPage(n, PageKind.Front);
                            }
                            else if (classAttr == "page-special")
                            {
                                addPage(n, PageKind.Special);
                            }
                        }
                    }
                }
                ParseNccDocument(n);
            }
        }


        private void addPage(XmlNode node, PageKind kind)
        {

            if (m_CurrentSection == null) throw new Exception(Localizer.Message("error_adding_page_number"));
            string pageNumberString = GetTextContent(node);
            PageNumber number = null;
            bool markToDo = false;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {

                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber == 0 && kind == PageKind.Front) //if front page is not numeric, make it numeric with marking it to do
                {
                    m_Counter_FrontPageCorection++;
                    pageNumber = m_Counter_FrontPageCorection;
                    markToDo = true;

                }
                if (pageNumber > 0) number = new PageNumber(pageNumber, kind);
            }
            if (number != null)
            {
                EmptyNode n = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                n.PageNumber = number;
                m_CurrentSection.AppendChild(n);

                if (markToDo)
                {
                    n.TODO = true;
                    m_ErrorsList.Add(string.Format(Localizer.Message("DAISY2_ObiImport_ErrorsList_FrontPageCorrection"), pageNumberString, n.PageNumber.ToString()));
                }
                //extract id

                XmlNode attrID = node.Attributes.GetNamedItem("id");
                string strId = attrID != null ? attrID.Value : null;
                if (strId != null) m_NccReferenceToPageMap.Add(strId, n);
            }
        }


        private SectionNode CreateSectionNode(XmlNode node)
        {
            int level = int.Parse(node.LocalName.Substring(1));
            SectionNode parent = getAvailableParent(level);
            SectionNode section = m_Presentation.CreateSectionNode();
            string strLabel = GetTextContent(node);
            strLabel = strLabel.Replace("\n", "").Replace("\r", "").Replace("\t", "");
            section.Label = strLabel;
            //if no parent was found, then we must be an h1 sibling or first node
            if (parent == null)
            {
                if (node.LocalName != "h1") throw new Exception(Localizer.Message("wrong_heading_order"));
                m_Presentation.RootNode.AppendChild(section);
            }
            else parent.AppendChild(section);


            return section;
        }

        /// <summary>
        /// Go through the available section nodes and find one that is allowed to have children with the given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private SectionNode getAvailableParent(int level)
        {
            int sz = m_OpenSectionNodes.Count;
            for (int i = 0; i < sz; i++)
            {
                if (m_OpenSectionNodes.Peek().Level == level - 1) return m_OpenSectionNodes.Peek();
                //remove the ones we aren't going to use (the stack will have h1 at the bottom, h5 at the top)
                else m_OpenSectionNodes.Pop();
            }
            return null;
        }

        private string GetTextContent(XmlNode node)
        {
            //string elementName = source.Name;
            string allText = node.InnerText;

            return allText;

        }


        private PhraseNode CreatePhraseNodeFromAudioElement(SectionNode section, XmlNode audioNode)
        {
            PhraseNode phrase = m_Presentation.CreatePhraseNode();

            if (section != null)
            {
                section.AppendChild(phrase);
            }
            addAudio(phrase, audioNode, true);
            return phrase;
        }

        private void addAudio(TreeNode treeNode, XmlNode xmlNode, bool isSequence)
        {
            //if (RequestCancellation) return;

            string dirPath = m_NccBaseDirectory;

            XmlAttributeCollection audioAttrs = xmlNode.Attributes;

            if (audioAttrs == null || audioAttrs.Count == 0)
            {
                return;
            }
            XmlNode audioAttrSrc = audioAttrs.GetNamedItem("src");
            if (audioAttrSrc == null || String.IsNullOrEmpty(audioAttrSrc.Value))
            {
                return;
            }

            string src = FileDataProvider.UriDecode(audioAttrSrc.Value);

            XmlNode audioAttrClipBegin = audioAttrs.GetNamedItem("clip-begin");
            XmlNode audioAttrClipEnd = audioAttrs.GetNamedItem("clip-end");

            ObiPresentation presentation = m_Presentation;
            ManagedAudioMedia media = null;

            string fullPath = Path.Combine(dirPath, src);
            fullPath = FileDataProvider.NormaliseFullFilePath(fullPath).Replace('/', '\\');
            //addOPF_GlobalAssetPath(fullPath);

            if (src.EndsWith("wav", StringComparison.OrdinalIgnoreCase))
            {
                FileDataProvider dataProv = null;

                if (!File.Exists(fullPath))
                {
                    Debug.Fail("File not found: {0}", fullPath);
                    media = null;
                }
                else
                {
                    //bool deleteSrcAfterCompletion = false;

                    string fullWavPath = fullPath;

                    FileDataProvider obj;
                    m_OriginalAudioFile_FileDataProviderMap.TryGetValue(fullWavPath, out obj);

                    if (obj != null)  //m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullWavPath))
                    {
                        if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                        {
                            DebugFix.Assert(obj.Presentation != presentation);

                            Object appData = obj.AppData;

                            DebugFix.Assert(appData != null);

                            if (appData != null && appData is WavClip.PcmFormatAndTime)
                            {
                                m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                                m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(((WavClip.PcmFormatAndTime)appData).mFormat);
                            }
                        }

                        if (obj.Presentation != presentation)
                        {
                            dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);

                            //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(obj.DataFileFullPath)));

                            dataProv.InitByCopyingExistingFile(obj.DataFileFullPath);

                            m_OriginalAudioFile_FileDataProviderMap.Remove(fullWavPath);
                            m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                            Object appData = obj.AppData;

                            DebugFix.Assert(appData != null);

                            if (appData != null && appData is WavClip.PcmFormatAndTime)
                            {
                                dataProv.AppData = new WavClip.PcmFormatAndTime(((WavClip.PcmFormatAndTime)appData).mFormat, ((WavClip.PcmFormatAndTime)appData).mTime);
                            }
                        }
                        else
                        {
                            dataProv = obj; // m_OriginalAudioFile_FileDataProviderMap[fullWavPath];
                        }
                    }
                    else // create FileDataProvider
                    {
                        Stream wavStream = null;
                        try
                        {
                            wavStream = File.Open(fullWavPath, FileMode.Open, FileAccess.Read, FileShare.Read);

                            uint dataLength;
                            AudioLibPCMFormat pcmInfo = null;

                            pcmInfo = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);

                            if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                            {

                                m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                                m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(pcmInfo);
                            }


                            //if (RequestCancellation) return;

                            if (!presentation.MediaDataManager.DefaultPCMFormat.Data.IsCompatibleWith(pcmInfo))
                            {
                                wavStream.Close();
                                wavStream = null;

                                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ConvertingAudio, Path.GetFileName(fullWavPath)));
                                string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullWavPath);

                                //if (RequestCancellation) return;

                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                dataProv.InitByMovingExistingFile(newfullWavPath);

                                m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                //if (RequestCancellation) return;
                            }
                            else // use original wav file by copying it to data directory
                            {
                                dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                                //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullWavPath) + " = " + dataProv.DataFileRelativePath);
                                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(fullWavPath)));
                                dataProv.InitByCopyingExistingFile(fullWavPath);
                                m_OriginalAudioFile_FileDataProviderMap.Add(fullWavPath, dataProv);

                                //if (RequestCancellation) return;
                            }
                        }
                        finally
                        {
                            if (wavStream != null)
                            {
                                wavStream.Close();
                            }
                        }
                    }

                } // FileDataProvider  key check ends

                //if (RequestCancellation) return;

                media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd, treeNode);

            }
            else if (src.EndsWith("mp3", StringComparison.OrdinalIgnoreCase)
                || src.EndsWith("mp4", StringComparison.OrdinalIgnoreCase))
            {
                if (!File.Exists(fullPath))
                {
                    Debug.Fail("File not found: {0}", fullPath);
                    return;
                }

                //if (RequestCancellation) return;

                //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.DecodingAudio, Path.GetFileName(fullPath)));

                //if (RequestCancellation) return;

                string fullMp34PathOriginal = fullPath;

                FileDataProvider obj;
                m_OriginalAudioFile_FileDataProviderMap.TryGetValue(fullMp34PathOriginal, out obj);

                FileDataProvider dataProv = null;
                if (obj != null) //m_OriginalAudioFile_FileDataProviderMap.ContainsKey(fullMp3PathOriginal))
                {
                    if (m_AudioConversionSession.FirstDiscoveredPCMFormat == null)
                    {
                        DebugFix.Assert(obj.Presentation != presentation);

                        Object appData = obj.AppData;

                        DebugFix.Assert(appData != null);

                        if (appData != null && appData is WavClip.PcmFormatAndTime)
                        {
                            m_AudioConversionSession.FirstDiscoveredPCMFormat = new AudioLibPCMFormat();
                            m_AudioConversionSession.FirstDiscoveredPCMFormat.CopyFrom(((WavClip.PcmFormatAndTime)appData).mFormat);
                        }
                    }

                    if (obj.Presentation != presentation)
                    {
                        dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);

                        //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.CopyingAudio, Path.GetFileName(obj.DataFileFullPath)));

                        dataProv.InitByCopyingExistingFile(obj.DataFileFullPath);


                        m_OriginalAudioFile_FileDataProviderMap.Remove(fullMp34PathOriginal);
                        m_OriginalAudioFile_FileDataProviderMap.Add(fullMp34PathOriginal, dataProv);

                        Object appData = obj.AppData;

                        DebugFix.Assert(appData != null);

                        if (appData != null && appData is WavClip.PcmFormatAndTime)
                        {
                            dataProv.AppData = new WavClip.PcmFormatAndTime(((WavClip.PcmFormatAndTime)appData).mFormat, ((WavClip.PcmFormatAndTime)appData).mTime);
                        }
                    }
                    else
                    {
                        dataProv = obj; // m_OriginalAudioFile_FileDataProviderMap[fullMp3PathOriginal];
                    }
                }
                else
                {
                    string newfullWavPath = m_AudioConversionSession.ConvertAudioFileFormat(fullMp34PathOriginal);

                    dataProv = (FileDataProvider)presentation.DataProviderFactory.Create(DataProviderFactory.AUDIO_WAV_MIME_TYPE);
                    //Console.WriteLine("Source audio file to SDK audio file map (before creating SDK audio file): " + Path.GetFileName(fullMp34PathOriginal) + " = " + dataProv.DataFileRelativePath);
                    dataProv.InitByMovingExistingFile(newfullWavPath);

                    m_AudioConversionSession.RelocateDestinationFilePath(newfullWavPath, dataProv.DataFileFullPath);

                    m_OriginalAudioFile_FileDataProviderMap.Add(fullMp34PathOriginal, dataProv);

                    //if (RequestCancellation) return;
                }

                if (dataProv != null)
                {
                    //if (RequestCancellation) return;

                    //media = addAudioWav(newfullWavPath, true, audioAttrClipBegin, audioAttrClipEnd);
                    media = addAudioWav(dataProv, audioAttrClipBegin, audioAttrClipEnd, treeNode);

                    if (media == null)
                    {
                        Debugger.Break();
                    }
                }

            }

            //if (RequestCancellation) return;

            if (media == null)
            {
                if (!TreenodesWithoutManagedAudioMediaData.Contains(treeNode))
                    //{
                    TreenodesWithoutManagedAudioMediaData.Add(treeNode);
                //}

                Debug.Fail("Creating ExternalAudioMedia ??");

                Time timeClipBegin = null;

                ExternalAudioMedia exmedia = presentation.MediaFactory.CreateExternalAudioMedia();
                exmedia.Src = src;
                if (audioAttrClipBegin != null &&
                    !string.IsNullOrEmpty(audioAttrClipBegin.Value))
                {
                    timeClipBegin = new Time(0);
                    try
                    {
                        timeClipBegin = new Time(audioAttrClipBegin.Value);
                    }
                    catch (Exception ex)
                    {
                        string str = "CLIP BEGIN TIME PARSE FAIL: " + audioAttrClipBegin.Value;
                        Console.WriteLine(str);
                        Debug.Fail(str);
                    }
                    exmedia.ClipBegin = timeClipBegin;
                }
                if (audioAttrClipEnd != null &&
                    !string.IsNullOrEmpty(audioAttrClipEnd.Value))
                {
                    Time timeClipEnd = null;
                    try
                    {
                        timeClipEnd = new Time(audioAttrClipEnd.Value);
                    }
                    catch (Exception ex)
                    {
                        string str = "CLIP END TIME PARSE FAIL: " + audioAttrClipEnd.Value;
                        Console.WriteLine(str);
                        Debug.Fail(str);
                    }

                    if (timeClipEnd != null)
                    {
                        try
                        {
                            exmedia.ClipEnd = timeClipEnd;
                        }
                        catch (Exception ex)
                        {
                            string str = "CLIP TIME ERROR (end < begin): " + timeClipBegin + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + timeClipEnd + " (" + audioAttrClipEnd.Value + ")";
                            Console.WriteLine(str);
                            //Debug.Fail(str);
                        }
                    }
                }
            }

            //if (RequestCancellation) return;

            if (media != null)
            {
                ChannelsProperty chProp =
                    treeNode.GetChannelsProperty();
                if (chProp == null)
                {
                    chProp =
                        presentation.PropertyFactory.CreateChannelsProperty();
                    treeNode.AddProperty(chProp);
                }
                if (isSequence)
                {
#if ENABLE_SEQ_MEDIA
                    SequenceMedia mediaSeq = chProp.GetMedia(m_audioChannel) as SequenceMedia;
                    if (mediaSeq == null)
                    {
                        mediaSeq = presentation.MediaFactory.CreateSequenceMedia();
                        mediaSeq.AllowMultipleTypes = false;
                        chProp.SetMedia(m_audioChannel, mediaSeq);
                    }
                    mediaSeq.ChildMedias.Insert(mediaSeq.ChildMedias.Count, media);
#else
                    ManagedAudioMedia existingMedia = chProp.GetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel()) as ManagedAudioMedia;
                    if (existingMedia == null)
                    {
                        chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel(), media);
                    }
                    else
                    {
                        // WARNING: WavAudioMediaData implementation differs from AudioMediaData:
                        // the latter is naive and performs a stream binary copy, the latter is optimized and re-uses existing WavClips. 
                        //  WARNING 2: The audio data from the given parameter gets emptied !
                        existingMedia.AudioMediaData.MergeWith(media.AudioMediaData);

                    }
#endif //ENABLE_SEQ_MEDIA
                }
                else
                {

                    chProp.SetMedia(presentation.ChannelsManager.GetOrCreateAudioChannel(), media);
                }
            }
            else
            {
                Debug.Fail("Media could not be created !");
            }
        }


        private ManagedAudioMedia addAudioWav(FileDataProvider dataProv, XmlNode audioAttrClipBegin, XmlNode audioAttrClipEnd, TreeNode treeNode)
        {
            //if (m_autoDetectPcmFormat
            //&& m_AudioConversionSession.FirstDiscoveredPCMFormat != null
            //&& !m_AudioConversionSession.FirstDiscoveredPCMFormat.IsCompatibleWith(treeNode.Presentation.MediaDataManager.DefaultPCMFormat.Data))
            //{
            //PCMFormatInfo pcmFormat = treeNode.Presentation.MediaDataManager.DefaultPCMFormat; //.Copy();
            //pcmFormat.Data.CopyFrom(m_AudioConversionSession.FirstDiscoveredPCMFormat);
            //                
            //treeNode.Presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;
            //}

            //if (RequestCancellation) return null;

            Time clipB = Time.Zero;
            Time clipE = Time.MaxValue;

            if (audioAttrClipBegin != null &&
                !string.IsNullOrEmpty(audioAttrClipBegin.Value))
            {
                string strBeginValue = audioAttrClipBegin.Value.Replace("npt=", "");
                try
                {
                    clipB = new Time(strBeginValue);
                }
                catch (Exception ex)
                {
                    clipB = new Time(0);
                    string str = "CLIP BEGIN TIME PARSE FAIL: " + audioAttrClipBegin.Value;
                    Console.WriteLine(str);
                    Debug.Fail(str);
                }
            }
            if (audioAttrClipEnd != null &&
                !string.IsNullOrEmpty(audioAttrClipEnd.Value))
            {
                try
                {
                    string strEndValue = audioAttrClipEnd.Value.Replace("npt=", "");
                    clipE = new Time(strEndValue);
                }
                catch (Exception ex)
                {
                    clipE = new Time(0);
                    string str = "CLIP END TIME PARSE FAIL: " + audioAttrClipEnd.Value;
                    Console.WriteLine(str);
                    Debug.Fail(str);
                }
            }

            ManagedAudioMedia media = null;
            ObiPresentation presentation = m_Presentation;

            WavAudioMediaData mediaData =
                            (WavAudioMediaData)
                            presentation.MediaDataFactory.CreateAudioMediaData();

            //  mediaData.AudioDuration DOES NOT WORK BECAUSE DEPENDS ON WAVCLIPS LIST!!!
            WavClip wavClip = new WavClip(dataProv);

            Time newClipE = clipE.Copy();
            if (Math.Abs(newClipE.AsMilliseconds) > Math.Abs(wavClip.MediaDuration.AsMilliseconds))
            {
                Console.WriteLine("clip adjusted. file size: " + wavClip.MediaDuration.AsMilliseconds + "; clip end: " + newClipE.AsMilliseconds);
                clipEndAdjustedToNull(clipB, newClipE, wavClip.MediaDuration, treeNode);
                //newClipE = wavClip.MediaDuration;
                newClipE = null;
            }

            try
            {
                mediaData.AppendPcmData(dataProv, clipB, newClipE);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif //DEBUG
                Console.WriteLine("CLIP TIME ERROR1 (end < begin ?): " + clipB + " (" + (audioAttrClipBegin != null ? audioAttrClipBegin.Value : "N/A") + ") / " + clipE + " (" + (audioAttrClipEnd != null ? audioAttrClipEnd.Value : "N/A") + ") === " + wavClip.MediaDuration);
            }

            //if (RequestCancellation) return null;

            media = presentation.MediaFactory.CreateManagedAudioMedia();
            media.AudioMediaData = mediaData;

            return media;
        }

        protected virtual void clipEndAdjustedToNull(Time clipB, Time clipE, Time duration, TreeNode treeNode)
        {
            double diff = clipE.AsMilliseconds - duration.AsMilliseconds;
            if (diff > m_Settings.Project_ImportToleranceForAudioInMs && treeNode != null)
            {
                EmptyNode eNode = (EmptyNode)treeNode;
                eNode.TODO = true;
                if (eNode.Role_ == EmptyNode.Role.Plain)
                {
                    eNode.Role_ = EmptyNode.Role.Custom;
                    eNode.CustomRole = Localizer.Message("DAISY3_ObiImport_ErrorsList_truncated_audio");
                }
                m_ErrorsList.Add(Localizer.Message("DAISY3_ObiImport_ErrorsList_truncated_audio_in_phrase") + eNode.Index.ToString() + Localizer.Message("DAISY3_ObiImport_ErrorsList_in_section") + eNode.ParentAs<SectionNode>().Label);
                m_ErrorsList.Add(Localizer.Message("DAISY3_ObiImport_ErrorsList_expected_clip_end") + clipE.Format_H_MN_S_MS() + Localizer.Message("DAISY3_ObiImport_ErrorsList_imported_clip_end") + duration.Format_H_MN_S_MS());
            }
        }

        private void ReplaceExternalAudioMediaPhraseWithEmptyNode(TreeNode node)
        {
            if (node is PhraseNode)
            {
                PhraseNode phrase = (PhraseNode)node;
                SectionNode section = phrase.ParentAs<SectionNode>();
                Console.WriteLine("replacing phrase node with empty node due to  clip problem " + section.Label + " phrase index:" + phrase.Index.ToString());
                int phraseIndex = phrase.Index;
                EmptyNode emptyNode = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                emptyNode.CopyAttributes(phrase);
                phrase.Detach();

                section.Insert(emptyNode, phraseIndex);
                emptyNode.TODO = true;

                m_ErrorsList.Add(Localizer.Message("DAISY3_ObiImport_ErrorsList_error_no_audio") + phraseIndex.ToString() + Localizer.Message("DAISY3_ObiImport_ErrorsList_in_section") + section.Label);
            }

        }

        public void CorrectExternalAudioMedia()
        {
            if (TreenodesWithoutManagedAudioMediaData == null || TreenodesWithoutManagedAudioMediaData.Count == 0) return;
            for (int i = 0; i < TreenodesWithoutManagedAudioMediaData.Count; i++)
            {
                if (TreenodesWithoutManagedAudioMediaData[i] is PhraseNode)
                {
                    PhraseNode phrase = (PhraseNode)TreenodesWithoutManagedAudioMediaData[i];
                    ReplaceExternalAudioMediaPhraseWithEmptyNode(phrase);
                }
            }
        }




        public XmlElement AttachAudioToPhraseNode(string audioFilePath, double start, double end)
        {
            XmlDocument doc = new XmlDocument();

            XmlElement audio = doc.CreateElement("audio");

           // add src

            audio.SetAttribute("src", audioFilePath);

           // Clip begin

            audio.SetAttribute("clip-begin", $"npt={start:0.000}s");

           // clip end

            audio.SetAttribute("clip-end", $"npt={end:0.000}s");

            return audio;
        }



    }
}
