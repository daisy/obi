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
    public class DAISY202Import
    {
        private string m_NccPath;
        private string m_NccBaseDirectory;
        private ObiPresentation m_Presentation;            // target presentation
        private SectionNode m_CurrentSection;               // section currently populated
        private Stack<Obi.SectionNode> m_OpenSectionNodes;  // these are section nodes that could accept children
        private Dictionary<SectionNode, string> m_SectionNodesToSmilReferenceMap;
        private Dictionary<string, EmptyNode> m_NccReferenceToPageMap = new Dictionary<string, EmptyNode>();
        private AudioFormatConvertorSession m_AudioConversionSession;
        private Dictionary<string, FileDataProvider> m_OriginalAudioFile_FileDataProviderMap = new Dictionary<string, FileDataProvider>(); // maps original audio file refered by smil to FileDataProvider of sdk.
        
        private Settings m_Settings ;
        protected List<TreeNode> TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();

        public DAISY202Import(string nccPath, ObiPresentation presentation, Settings settings)
        {
            m_NccPath = nccPath;
            m_NccBaseDirectory = Path.GetDirectoryName(nccPath);
            m_OpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            m_Presentation = presentation;
            m_SectionNodesToSmilReferenceMap = new Dictionary<SectionNode, string>();
            m_AudioConversionSession = new AudioFormatConvertorSession(m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                m_Presentation.MediaDataManager.DefaultPCMFormat,
                false,
                false);
            m_Settings = settings;
        }

        private List<string> m_ErrorsList = new List<string>();
        public List<string> ErrorsList { get { return m_ErrorsList; } }

        public static String GrabTitle(string filePath)
        {
            Uri fileUri = new Uri(filePath);
            return GrabTitle(fileUri);
        }

        public static String GrabTitle(Uri fileUri)
        {
            XmlTextReader source = GetXmlReader(fileUri);
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            String title = null;
            try
            {
                title = source.ReadToFollowing("title") ? source.ReadString() : Localizer.Message("default_project_title");
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                source.Close();
            }
            return title;
        }

        private static XmlTextReader GetXmlReader(Uri uri)
        {
            XmlTextReader reader = new XmlTextReader(uri.ToString());
            reader.XmlResolver = null;
            return reader;
        }

        public void ImportFromXHTML()
        {
            try
            {
                XmlDocument nccDocument = XmlReaderWriterHelper.ParseXmlDocument(m_NccPath, false, false);

                XmlNode bodyNode = nccDocument.GetElementsByTagName("body")[0];
                ParseNccDocument(bodyNode);
                
                AppendPhrasesFromSmil();
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void ParseNccDocument(XmlNode node)
        {
            if ( node.ChildNodes.Count == 0 ) return ;
            foreach (XmlNode n in node.ChildNodes)
            {
                                    if (n.NodeType == XmlNodeType.Element)
                    {
                        if (n.LocalName == "h1" || n.LocalName == "h2" || n.LocalName == "h3" ||
                            n.LocalName == "h4" || n.LocalName == "h5" || n.LocalName == "h6")
                        {
                            //foundHeadings = true;
                            Obi.SectionNode section = CreateSectionNode(n);
                            if (section  != null && section.Level < 6) m_OpenSectionNodes.Push(section );
                            m_CurrentSection = section ;
                            m_SectionNodesToSmilReferenceMap.Add(section, GetSmilReferenceString(n));
                            
                        }
                        else if (n.LocalName == "p" || n.LocalName == "span")
                        {
                            string classAttr = n.Attributes.GetNamedItem("class").Value;
                            if (classAttr == "page" || classAttr == "page-normal")
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
                ParseNccDocument(n);
            }
        }

        private string GetSmilReferenceString(XmlNode xNode)
        {
            XmlNode anchorNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xNode, true, "a", xNode.NamespaceURI);
            XmlNode attr = anchorNode.Attributes.GetNamedItem("href");
            return attr != null? attr.Value : null;
        }

        // Add a page of the given kind; parse the content to get the page number.
        private void addPage(XmlNode node, PageKind kind)
        {
            if (m_CurrentSection == null) throw new Exception(Localizer.Message("error_adding_page_number"));
            string pageNumberString = GetTextContent(node);
            PageNumber number = null;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber > 0) number = new PageNumber(pageNumber, kind);
            }
            if (number != null)
            {
                EmptyNode n = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                n.PageNumber = number;
                m_CurrentSection.AppendChild(n);

                //extract id

                string strId = node.Attributes.GetNamedItem("id").Value;
                if (strId != null) m_NccReferenceToPageMap.Add(strId, n);
            }
        }


                    private SectionNode CreateSectionNode(XmlNode node)
        {
            int level = int.Parse(node.LocalName.Substring(1));
            SectionNode parent = getAvailableParent(level);
            SectionNode section = m_Presentation.CreateSectionNode();
            section.Label = GetTextContent(node);
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
            string allText = node.InnerText ;
            
            return allText;
        
}

private void AppendPhrasesFromSmil ()
{
    if ( m_SectionNodesToSmilReferenceMap.Count == 0 ) return ;
    foreach (SectionNode section in m_SectionNodesToSmilReferenceMap.Keys )
    {   
        
        string smilReferenceString = m_SectionNodesToSmilReferenceMap[section];
        string[] StringArray = smilReferenceString.Split('#');
        string smilFileName = StringArray[0];
        string strId = StringArray[1];
        string fullSmilFilePath = Path.Combine( Path.GetDirectoryName(m_NccPath), smilFileName ) ;
        if (!File.Exists(fullSmilFilePath)) continue;
        XmlDocument smilDocument = XmlReaderWriterHelper.ParseXmlDocument(fullSmilFilePath, false,false);
        Console.WriteLine(section.Label);
        XmlNode mainSeqNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument.DocumentElement, true, "seq", smilDocument.DocumentElement.NamespaceURI);
        ParseSmilDocument(section, mainSeqNode, smilFileName, strId);
    }
}

        private void ParseSmilDocument(SectionNode section, XmlNode xNode, string smilFileName, string strId)
        {
            if (xNode.ChildNodes.Count == 0) return;
            foreach (XmlNode n in xNode.ChildNodes)
            {
                if (n.LocalName == "par")
                {
                    EmptyNode page = null;
                    EmptyNode originalPageNode = null;
                    XmlNode txtNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(n, true, "text", n.NamespaceURI);
                    if (txtNode != null)
                    {
                        string src = txtNode.Attributes.GetNamedItem("src").Value;
                        string nccID = src.Split('#')[1];
                        
                        if (m_NccReferenceToPageMap.ContainsKey(nccID)) originalPageNode = m_NccReferenceToPageMap[nccID];
                    }
                    XmlNode seqNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(n, true, "seq", n.NamespaceURI);
                    if (seqNode != null)
                    {
                        foreach (XmlNode audioNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(seqNode, true, "audio", seqNode.NamespaceURI, false))
                        {
                            EmptyNode newNode = CreatePhraseNodeFromAudioElement(section, audioNode);
                            if (page == null) page = newNode;
                        }
                        if (originalPageNode != null && page != null)
                        {
                            page.PageNumber = originalPageNode.PageNumber;
                            Console.WriteLine("1 " + page.PageNumber);
                            Console.WriteLine(smilFileName);
                            originalPageNode.Detach();
                        }
                        
                    }
                    else
                    {//1
                        foreach (XmlNode audioNode in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(n, true, "audio", seqNode.NamespaceURI, false))
                        {//2
                            EmptyNode newNode = CreatePhraseNodeFromAudioElement(section, audioNode);
                            if (page == null) page = newNode;
                        }//-2
                        if (originalPageNode != null && page != null)
                        {//2
                            page.PageNumber = originalPageNode.PageNumber;
                            Console.WriteLine("2 " + page.PageNumber);
                            originalPageNode.Detach();
                        }//-2
                    }//-1
                    if (page == null) ParseSmilDocument(section, n, smilFileName, strId);
                }
                else if (n.LocalName == "audio")
                {
                    CreatePhraseNodeFromAudioElement(section, n);
                    ParseSmilDocument(section, n, smilFileName, strId);
                }
                
            }
        }

        private PhraseNode CreatePhraseNodeFromAudioElement(SectionNode section, XmlNode audioNode)
        {
            PhraseNode phrase = m_Presentation.CreatePhraseNode();
            
            section.AppendChild(phrase);
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
            if (newClipE.IsGreaterThan(wavClip.MediaDuration))
            {
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
            double diff = clipE.AsTimeSpan.TotalMilliseconds - duration.AsTimeSpan.TotalMilliseconds;
            if (diff > m_Settings.ImportToleranceForAudioInMs && treeNode != null)
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



    }
}
