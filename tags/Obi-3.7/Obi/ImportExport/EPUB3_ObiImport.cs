using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;

using urakawa;
using urakawa.core;
using urakawa.data;
using urakawa.ExternalFiles;
using urakawa.property.xml;
using urakawa.xuk;
using urakawa.daisy;
using urakawa.daisy.import;
using AudioLib;
//using ICSharpCode.SharpZipLib.Zip;
using Jaime.Olivares;

namespace Obi.ImportExport
{
    /// <summary>
    /// 
    /// </summary>
    public class EPUB3_ObiImport : DAISY3_ObiImport
    {
        Session m_session = null;
        private bool m_IsTOCFromNavDoc;

        public EPUB3_ObiImport(Session session, Settings settings, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate, bool stereo)
            : base(session, settings, bookfile, outDir, skipACM, audioProjectSampleRate, stereo)
        {
            m_session = session;
            AudioNCXImport = false;
            m_IsTOCFromNavDoc = false;
        }

        protected override void parseContentDocuments(List<string> spineOfContentDocuments,
            Dictionary<string, string> spineAttributes,
            List<Dictionary<string, string>> spineItemsAttributes,
            string coverImagePath, string navDocPath)
        {
            if (spineOfContentDocuments == null || spineOfContentDocuments.Count <= 0)
            {
                return;
            }
            // assign obi project file path to xuk path to prevent creation of xukspine file.
            XukPath = m_session.Path;
            //Console.WriteLine(XukPath);
            
            Presentation spinePresentation = m_Project.Presentations.Get(0);

            m_IsTOCFromNavDoc = false;
            if (navDocPath != null)
            {
                string fullNavPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), navDocPath);
                
                XmlDocument navDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(fullNavPath, true, true);
                ParseNCXDocument(navDoc);
                m_IsTOCFromNavDoc = true;
            }
            //spinePresentation.RootNode.GetOrCreateXmlProperty().SetQName("spine", "");

            //if (!string.IsNullOrEmpty(m_OPF_ContainerRelativePath))
            //{
                //spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute(OPF_ContainerRelativePath, "", m_OPF_ContainerRelativePath);
            //}

            //foreach (KeyValuePair<string, string> spineAttribute in spineAttributes)
            //{
                //spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute(spineAttribute.Key, "", spineAttribute.Value);
            //}

            //if (m_PackagePrefixAttr != null)
            //{
                //spinePresentation.RootNode.GetOrCreateXmlProperty().SetAttribute("prefix", "", m_PackagePrefixAttr.Value);
            //}

            // Audio files may be shared between chapters of a book!
            
            m_OriginalAudioFile_FileDataProviderMap.Clear();
            TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();
            Presentation spineItemPresentation = null;

            int index = -1;
            
            foreach (string docPath in spineOfContentDocuments)
            {
                index++;

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ReadXMLDoc, docPath));
                //System.Windows.Forms.MessageBox.Show(docPath);
                //DirectoryInfo opfParentDir = Directory.GetParent(m_Book_FilePath);
                //string dirPath = opfParentDir.ToString();
                string fullDocPath = Path.Combine(Path.GetDirectoryName(m_Book_FilePath), docPath);

                fullDocPath = FileDataProvider.NormaliseFullFilePath(fullDocPath).Replace('/', '\\');

                if (!File.Exists(fullDocPath))
                {
#if DEBUG
                    Debugger.Break();
#endif //DEBUG
                    continue;
                }

                addOPF_GlobalAssetPath(fullDocPath);

                ///TreeNode spineChild = spinePresentation.TreeNodeFactory.Create();
                //TextMedia txt = spinePresentation.MediaFactory.CreateTextMedia();
                //txt.Text = docPath; // Path.GetFileName(fullDocPath);
                //spineChild.GetOrCreateChannelsProperty().SetMedia(spinePresentation.ChannelsManager.GetOrCreateTextChannel(), txt);
                //spinePresentation.RootNode.AppendChild(spineChild);

                //spineChild.GetOrCreateXmlProperty().SetQName("metadata", "");

                //foreach (KeyValuePair<string, string> spineItemAttribute in spineItemsAttributes[index])
                //{
                    //spineChild.GetOrCreateXmlProperty().SetAttribute(spineItemAttribute.Key, "", spineItemAttribute.Value);
                //}

                string ext = Path.GetExtension(fullDocPath);

                if (docPath == coverImagePath)
                {
                    DebugFix.Assert(ext.Equals(DataProviderFactory.IMAGE_SVG_EXTENSION, StringComparison.OrdinalIgnoreCase));

                    //spineChild.GetOrCreateXmlProperty().SetAttribute("cover-image", "", "true");
                }

                if (docPath == navDocPath)
                {
                    DebugFix.Assert(
                        ext.Equals(DataProviderFactory.XHTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                        || ext.Equals(DataProviderFactory.HTML_EXTENSION, StringComparison.OrdinalIgnoreCase));

                    //spineChild.GetOrCreateXmlProperty().SetAttribute("nav", "", "true");
                }

                if (
                    !ext.Equals(DataProviderFactory.XHTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.HTML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.DTBOOK_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(DataProviderFactory.XML_EXTENSION, StringComparison.OrdinalIgnoreCase)
                    )
                {
                    DebugFix.Assert(ext.Equals(DataProviderFactory.IMAGE_SVG_EXTENSION, StringComparison.OrdinalIgnoreCase));

                    bool notExistYet = true;
                    foreach (ExternalFileData externalFileData in m_Project.Presentations.Get(0).ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
                    {
                        if (!string.IsNullOrEmpty(externalFileData.OriginalRelativePath))
                        {
                            bool notExist = docPath != externalFileData.OriginalRelativePath;
                            notExistYet = notExistYet && notExist;
                            if (!notExist)
                            {
                                break;
                            }
                        }
                    }

                    DebugFix.Assert(notExistYet);

                    if (notExistYet)
                    {
                        ExternalFileData externalData = null;
                        if (docPath == coverImagePath)
                        {
                            externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create
                                <CoverImageExternalFileData>();
                        }
                        else
                        {
                            externalData = m_Project.Presentations.Get(0).ExternalFilesDataFactory.Create
                                <GenericExternalFileData>();
                        }
                        if (externalData != null)
                        {
                            externalData.InitializeWithData(fullDocPath, docPath, true, null);

                            addOPF_GlobalAssetPath(fullDocPath);
                        }
                    }

                    continue;
                }

                //spineChild.GetOrCreateXmlProperty().SetAttribute("xuk", "", "true");

                XmlDocument xmlDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(fullDocPath, true, true);

                if (RequestCancellation) return;

                ///m_PublicationUniqueIdentifier = null;
                //m_PublicationUniqueIdentifierNode = null;

                //Project spineItemProject = new Project();
                //spineItemProject.PrettyFormat = m_XukPrettyFormat;

                //string dataFolderPrefix = FileDataProvider.EliminateForbiddenFileNameCharacters(docPath);
                //spineItemPresentation = spineItemProject.AddNewPresentation(new Uri(m_outDirectory),
                    
                    //dataFolderPrefix
                    //);

                //PCMFormatInfo pcmFormat = spineItemPresentation.MediaDataManager.DefaultPCMFormat; //.Copy();
                //pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
                //pcmFormat.Data.NumberOfChannels = m_audioStereo ? (ushort)2 : (ushort)1;
                //spineItemPresentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

                //TextChannel textChannel = spineItemPresentation.ChannelFactory.CreateTextChannel();
                //textChannel.Name = "The Text Channel";
                //DebugFix.Assert(textChannel == spineItemPresentation.ChannelsManager.GetOrCreateTextChannel());

                //AudioChannel audioChannel = spineItemPresentation.ChannelFactory.CreateAudioChannel();
                //audioChannel.Name = "The Audio Channel";
                //DebugFix.Assert(audioChannel == spineItemPresentation.ChannelsManager.GetOrCreateAudioChannel());

                //ImageChannel imageChannel = spineItemPresentation.ChannelFactory.CreateImageChannel();
                //imageChannel.Name = "The Image Channel";
                //DebugFix.Assert(imageChannel == spineItemPresentation.ChannelsManager.GetOrCreateImageChannel());

                //VideoChannel videoChannel = spineItemPresentation.ChannelFactory.CreateVideoChannel();
                //videoChannel.Name = "The Video Channel";
                //DebugFix.Assert(videoChannel == spineItemPresentation.ChannelsManager.GetOrCreateVideoChannel());


                if (m_AudioConversionSession != null)
                {
                    
                    RemoveSubCancellable(m_AudioConversionSession);
                    m_AudioConversionSession = null;
                }

                m_AudioConversionSession = new AudioFormatConvertorSession(
                                       m_Presentation.DataProviderManager.DataFileDirectoryFullPath,
                   m_Presentation.MediaDataManager.DefaultPCMFormat,
                   m_autoDetectPcmFormat,
                   true );

                //AddSubCancellable(m_AudioConversionSession);

                


                if (RequestCancellation) return;

                if (parseContentDocParts(fullDocPath, m_Project, xmlDoc, docPath, DocumentMarkupType.NA))
                {
                    return; // user cancel
                }
                



                /*
                string title = GetTitle(spineItemPresentation);
                if (!string.IsNullOrEmpty(title))
                {
                    spineChild.GetOrCreateXmlProperty().SetAttribute("title", "", title);
                }


                if (false) // do not copy metadata from project to individual chapter
                {
                    foreach (Metadata metadata in m_Project.Presentations.Get(0).Metadatas.ContentsAs_Enumerable)
                    {
                        Metadata md = spineItemPresentation.MetadataFactory.CreateMetadata();
                        md.NameContentAttribute = metadata.NameContentAttribute.Copy();

                        foreach (MetadataAttribute metadataAttribute in metadata.OtherAttributes.ContentsAs_Enumerable)
                        {
                            MetadataAttribute mdAttr = metadataAttribute.Copy();
                            md.OtherAttributes.Insert(md.OtherAttributes.Count, mdAttr);
                        }

                        spineItemPresentation.Metadatas.Insert(spineItemPresentation.Metadatas.Count, md);
                    }
                }
                */

                foreach (KeyValuePair<string, string> spineItemAttribute in spineItemsAttributes[index])
                {
                    if (spineItemAttribute.Key == "media-overlay")
                    {
                        string opfDirPath = Path.GetDirectoryName(m_Book_FilePath);
                        string overlayPath = spineItemAttribute.Value;


                        reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ParsingMediaOverlay, overlayPath));


                        string fullOverlayPath = Path.Combine(opfDirPath, overlayPath);
                        if (!File.Exists(fullOverlayPath))
                        {
                            continue;
                        }

                        XmlDocument overlayXmlDoc = XmlReaderWriterHelper.ParseXmlDocument(fullOverlayPath, false, false);

                        SectionNode section = m_Presentation.FirstSection;
                        IEnumerable<XmlNode> textElements = XmlDocumentHelper.GetChildrenElementsOrSelfWithName(overlayXmlDoc, true, "text", null, false);
                        if (textElements == null)
                        {
                        continue;
                        }
                        // audio list replaced by text list
                        //IEnumerable<XmlNode> audioElements = XmlDocumentHelper.GetChildrenElementsOrSelfWithName(overlayXmlDoc, true, "audio", null, false);
                        //if (audioElements == null)
                        //{
                            //continue;
                        //}

                        //foreach (XmlNode audioNode in audioElements)
                        foreach (XmlNode textNode in textElements)
                        {
                            XmlAttributeCollection attrs = textNode.Attributes;
                            if (attrs == null)
                            {
                                continue;
                            }
                            //XmlAttributeCollection attrs = audioNode.Attributes;
                            //if (attrs == null)
                            //{
                                //continue;
                            //}
                            XmlNode attrSrc = attrs.GetNamedItem("src");
                            if (attrSrc == null)
                            {
                                continue;
                            }

                            //XmlNode attrBegin = attrs.GetNamedItem("clipBegin");
                            //XmlNode attrEnd = attrs.GetNamedItem("clipEnd");

                            //string overlayDirPath = Path.GetDirectoryName(fullOverlayPath);
                            //string fullAudioPath = Path.Combine(overlayDirPath, attrSrc.Value);

                            //if (!File.Exists(fullAudioPath))
                            //{
                            //    continue;
                            //}


                            //if (RequestCancellation) return;
                            //reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.DecodingAudio, Path.GetFileName(fullAudioPath)));


                            TreeNode textTreeNode = null;
                            XmlNode audioNode = null ;
                            PageNumber pgNumber = null;
                            //XmlNodeList children = audioNode.ParentNode.ChildNodes;
                            XmlNodeList children = textNode.ParentNode.ChildNodes;
                            foreach (XmlNode child in children)
                            {
                                //if (child == audioNode)
                                //{
                                    //continue;
                                //}
                                //if (child.LocalName != "text")
                                //{
                                    //continue;
                                //}
                                if (child.LocalName == "audio")
                                {
                                    audioNode = child ;
                                }

                                //XmlAttributeCollection textAttrs = child.Attributes;
                                XmlAttributeCollection textAttrs = textNode.Attributes;
                                if (textAttrs == null)
                                {
                                    continue;
                                }

                                XmlNode textSrc = textAttrs.GetNamedItem("src");
                                if (textSrc == null)
                                {
                                    continue;
                                }

                                string urlDecoded = FileDataProvider.UriDecode(textSrc.Value);
                                string contentFilesDirectoryPath = Path.GetDirectoryName(fullOverlayPath);
                                if (urlDecoded.StartsWith("./")) urlDecoded = urlDecoded.Remove(0, 2);
                                if (urlDecoded.StartsWith("../"))
                                {
                                    urlDecoded = urlDecoded.Remove(0, 3);
                                    contentFilesDirectoryPath = Path.GetDirectoryName(contentFilesDirectoryPath);
                                }
                                if (urlDecoded.IndexOf('#') > 0)
                                {
                                    string[] srcParts = urlDecoded.Split('#');
                                    if (srcParts.Length != 2)
                                    {
                                        continue;
                                    }

                                    string fullTextRefPath = Path.Combine(contentFilesDirectoryPath,
                                        srcParts[0]);
                                    fullTextRefPath =
                                        FileDataProvider.NormaliseFullFilePath(fullTextRefPath).Replace('/', '\\');

                                    if (!fullTextRefPath.Equals(fullDocPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //#if DEBUG
                                        //                                Debugger.Break();
                                        //#endif //DEBUG

                                        continue;
                                    }

                                    string txtId = srcParts[1];
                                    
                                    //textTreeNode = spineItemPresentation.RootNode.GetFirstDescendantWithXmlID(txtId);
                                    // replacing it
                                    //System.Windows.Forms.MessageBox.Show("retrieving: " + urlDecoded);
                                    if (m_XmlIdToSectionNodeMap.ContainsKey(urlDecoded))
                                    {
                                        textTreeNode = m_XmlIdToSectionNodeMap[urlDecoded];
                                    }
                                    else if (m_XmlIdToSectionNodeMap.ContainsKey(srcParts[0]))
                                    {
                                        textTreeNode = m_XmlIdToSectionNodeMap[srcParts[0]];
                                    }
                                    if (textTreeNode != null)
                                    {
                                        section = (SectionNode)textTreeNode;
                                        //System.Windows.Forms.MessageBox.Show(((SectionNode)textTreeNode).Label);
                                    }
                                }
                                else
                                {
                                    string fullTextRefPath = Path.Combine(Path.GetDirectoryName(fullOverlayPath),
                                        urlDecoded);
                                    fullTextRefPath =
                                        FileDataProvider.NormaliseFullFilePath(fullTextRefPath).Replace('/', '\\');

                                    if (!fullTextRefPath.Equals(fullDocPath, StringComparison.OrdinalIgnoreCase))
                                    {
                                        //#if DEBUG
                                        //                                Debugger.Break();
                                        //#endif //DEBUG

                                        continue;
                                    }

                                    textTreeNode = spineItemPresentation.RootNode;
                                }
                                if (m_XmlIdToPageNodeMap.ContainsKey(urlDecoded)
                                    && m_XmlIdToPageNodeMap[urlDecoded]!= null )
                                {
                                    EmptyNode pgNode =  m_XmlIdToPageNodeMap[urlDecoded];
                                    pgNumber = pgNode.PageNumber;

                                    // if the section does not match then the parent of the page node should become the section.
                                    if (pgNode.IsRooted
                                        && textTreeNode == null
                                        && pgNode.Parent != section)
                                    {
                                        section = pgNode.ParentAs<SectionNode>();
                                        //Console.WriteLine("text node is null");
                                    }       
                                    if(pgNode.IsRooted)  pgNode.Detach();
                                    // the phrases following the page phrase in smil will refer to same content doc ID. so to avoid reassigning page, the page node in dictionary is assigned to null.
                                    m_XmlIdToPageNodeMap[urlDecoded] = null;
                                }
                            }
                            
                            if (section != null  && audioNode != null)
                            {
                            PhraseNode audioWrapperNode = m_Presentation.CreatePhraseNode();
                            
                                section.AppendChild(audioWrapperNode);
                                addAudio(audioWrapperNode , audioNode, false, fullOverlayPath);
                                if (audioWrapperNode.Duration == 0 && !TreenodesWithoutManagedAudioMediaData.Contains(audioWrapperNode))
                                {   
                                    TreenodesWithoutManagedAudioMediaData.Add(audioWrapperNode);
                                }
                                if (pgNumber != null)
                                {
                                    audioWrapperNode.PageNumber = pgNumber;
                                    pgNumber = null;
                                }
                            }

                            audioNode = null;
                        }
                    }
                }


                //spinePresentation.MediaDataManager.DefaultPCMFormat = spineItemPresentation.MediaDataManager.DefaultPCMFormat; //copied!

                //string xuk_FilePath = GetXukFilePath_SpineItem(m_outDirectory, docPath, title, index);

                //string xukFileName = Path.GetFileName(xuk_FilePath);
                //spineChild.GetOrCreateXmlProperty().SetAttribute("xukFileName", "", xukFileName);

                //deleteDataDirectoryIfEmpty();
                //string dataFolderPath = spineItemPresentation.DataProviderManager.DataFileDirectoryFullPath;
                //spineItemPresentation.DataProviderManager.SetCustomDataFileDirectory(Path.GetFileNameWithoutExtension(xuk_FilePath));

                //string newDataFolderPath = spineItemPresentation.DataProviderManager.DataFileDirectoryFullPath;
                //DebugFix.Assert(Directory.Exists(newDataFolderPath));

                //if (newDataFolderPath != dataFolderPath)
                {
                    /*
                    try
                    {
                        if (Directory.Exists(newDataFolderPath))
                        {
                            FileDataProvider.TryDeleteDirectory(newDataFolderPath, false);
                        }
                        
                        Directory.Move(dataFolderPath, newDataFolderPath);
                    }
                    catch (Exception ex)
                    {
#if DEBUG
                        Debugger.Break();
#endif // DEBUG
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex.StackTrace);

                        spineItemPresentation.DataProviderManager.SetCustomDataFileDirectory(dataFolderPrefix);
                    }
                         */
                }
                            }
                            
        }

        protected override void RemoveMetadataItemsToBeExcluded(Project project)
        {
            base.RemoveMetadataItemsToBeExcluded(project);
            AdaptMetadataAccordingToDAISYMetadata();
        }

        private void AdaptMetadataAccordingToDAISYMetadata()
        {
            Dictionary<string,string> metadataNamesMap = new Dictionary<string,string> () ;

            foreach (string name in Metadata.DAISY3MetadataNames)
            {
                metadataNamesMap.Add(name.ToLower(), name);
            }

            for (int i = 0; i < m_Presentation.Metadatas.Count; i++)
            {
                urakawa.metadata.Metadata md = m_Presentation.Metadatas.Get(i);
                if (md != null && md.NameContentAttribute != null
                    && metadataNamesMap.ContainsKey(md.NameContentAttribute.Name))
                {
                    md.NameContentAttribute.Name = metadataNamesMap[md.NameContentAttribute.Name];
                    //Console.WriteLine(md.NameContentAttribute.Name);
                }

            }
        }

        protected override TreeNode GetFirstTreeNodeForXmlDocument(Presentation presentation, XmlNode xmlNode)
        {
            int level = -1;
            XmlNode xNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "section", null);

            if (xNode != null)
            {
                XmlNode headingNode = ParseToFineHtmlHeadingNode(xNode);


                if (headingNode != null)
                {
                    string strLevel = headingNode.LocalName.Replace("h", "");
                    //Console.WriteLine("str level " + strLevel);
                    level = int.Parse(strLevel);
                }
            }
            
            if (level <= 1)
            {
                return presentation.RootNode;
            }
            else
            {
                List<SectionNode> sectionsList = new List<SectionNode>(m_XmlIdToSectionNodeMap.Values);

                for (int i = sectionsList.Count-1 ; i >= 0; i--)
                {
                    if (sectionsList[i].Level < level)
                    {
                        
                        return sectionsList[i];
                    }
                }
                return presentation.RootNode;
            }
        }

        private XmlNode ParseToFineHtmlHeadingNode(XmlNode xNode)
        {
            //Console.WriteLine("xml node name :" + xNode.LocalName);
            if (xNode.LocalName.StartsWith ("h")
                && (xNode.LocalName == "h1" || xNode.LocalName == "h2" || xNode.LocalName == "h3" 
                || xNode.LocalName == "h4" || xNode.LocalName == "h5" || xNode.LocalName == "h6" ))
            {
                return xNode;
            }
            else if (xNode.ChildNodes.Count > 0 )
            {
                //Console.WriteLine("child nodes" ); 
                foreach ( XmlNode n in xNode.ChildNodes )
                {
                    //Console.WriteLine ("parsing child node: " + n.LocalName);
                    if (n is XmlElement)
                    {
                        XmlNode newNode = ParseToFineHtmlHeadingNode(n);
                        if (newNode != null) return newNode;
                    }
                }
            }
                
                    return null;
                
            
        }

        protected override TreeNode CreateAndAddTreeNodeForContentDocument(TreeNode parentNode, XmlNode node, string contentFileName)
        {
            TreeNode createdNode = null;
            //Console.WriteLine(node.LocalName);
            if (node.LocalName == "doctitle" || node.LocalName == "section")
            {
                //Console.WriteLine("creating section ");
                if (m_IsTOCFromNavDoc)
                {//1
                    Console.WriteLine("TOC from nav");
                    XmlNode htmlNode = ParseToFineHtmlHeadingNode(node);
                    if (htmlNode != null)
                    {
                        if (htmlNode.Attributes.GetNamedItem("id") != null)
                        {
                            string strReference = contentFileName + "#" + htmlNode.Attributes.GetNamedItem("id").Value;
                            Console.WriteLine(strReference);
                            if (m_XmlIdToSectionNodeMap.ContainsKey(strReference))
                            {
                                //System.Windows.Forms.MessageBox.Show(m_XmlIdToSectionNodeMap[strReference].Label);
                                return m_XmlIdToSectionNodeMap[strReference];
                            }
                        }
                    }
                }//-1
                else
                {//1
                    SectionNode treeNode = m_Presentation.CreateSectionNode();
                    createdNode = treeNode;
                    if (parentNode is ObiRootNode)
                    {//2
                        ((ObiNode)parentNode).AppendChild(treeNode);
                    }//-2
                    else
                    {//2
                        ((SectionNode)parentNode).AppendChild(treeNode);
                    }//-2
                }//-1
            }
            else if (parentNode is SectionNode
                && (node.LocalName == "span" && node.Attributes.GetNamedItem("type", "http://www.idpf.org/2007/ops") != null
                && node.Attributes.GetNamedItem("type", "http://www.idpf.org/2007/ops").Value == "pagebreak"))
            {
                EmptyNode treeNode = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                createdNode = treeNode;
                ((SectionNode)parentNode).AppendChild(treeNode);

                PageNumber number = null;


                XmlNode pageTitle = node.Attributes.GetNamedItem("title");
                string pageNumberString = pageTitle != null ? pageTitle.Value :
                    node.InnerText;
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                number = new PageNumber(pageNumber, PageKind.Normal);
                

                if (number != null)
                {
                    ((EmptyNode)treeNode).PageNumber = number;
                    Console.WriteLine("Page " + pageNumber.ToString() + ", Added to Section: " + ((SectionNode)parentNode).Label);
                }

            }
            return createdNode;
        }

        public override void ParseNCXDocument(XmlDocument ncxDocument)
        {
            XmlNodeList navNodesList = ncxDocument.GetElementsByTagName("nav");
            XmlNode TOCNode = null;
            string ns_EPUB = "http://www.idpf.org/2007/ops";
            foreach (XmlNode n in navNodesList)
            {
                System.Xml.XmlNode attr = n.Attributes.GetNamedItem("type", ns_EPUB);

                if (attr != null
                    && attr.Value == "toc")
                {
                    TOCNode = n;

                    break;
                }
            }

            ParseTOC(TOCNode, (ObiRootNode)m_Presentation.RootNode);
            //System.Windows.Forms.MessageBox.Show("nav doc parsing complete");
        }


        private void ParseTOC(XmlNode xNode, ObiNode parentNode)
        {
            if (xNode.Name == "li")
            {
                SectionNode section = m_Presentation.CreateSectionNode();
                parentNode.AppendChild(section);
                parentNode = section;


            }
            else if (xNode is XmlText)
            {
                if (parentNode is SectionNode) 
                {
                    SectionNode parentSection = (SectionNode)parentNode ;
                    parentSection.Label = xNode.InnerText;
                    
            }
            }
            else if (xNode.Name == "a")
            {
                if (parentNode is SectionNode)
                {
                    XmlNode hrefNode = xNode.Attributes.GetNamedItem("href");
                    string headingReference = hrefNode.Value;
                    if (!m_XmlIdToSectionNodeMap.ContainsKey(headingReference))
                    {
                        m_XmlIdToSectionNodeMap.Add(headingReference, (SectionNode)parentNode);
                    }
                    
                }
            }
            if (xNode.ChildNodes.Count == 0)
            {
                return;
            }
            foreach (XmlNode n in xNode.ChildNodes)
            {
                ParseTOC(n, parentNode);
            }
        }


        public static string unzipEPubAndGetOpfPath(string EPUBFullPath)
        {
            //if (RequestCancellation) return;
            string parentDirectoryFullPath = Directory.GetParent(EPUBFullPath).ToString();

            string unzipDirectory = Path.Combine(
                parentDirectoryFullPath,
            Path.GetFileNameWithoutExtension(EPUBFullPath) + "_ZIP"
            );

            FileDataProvider.TryDeleteDirectory(unzipDirectory, true);

            ZipStorer zip = ZipStorer.Open(File.OpenRead(EPUBFullPath), FileAccess.Read);

            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
            foreach (ZipStorer.ZipFileEntry entry in dir)
            {
                //if (RequestCancellation) return;
                //reportProgress_Throttle(-1, String.Format(UrakawaSDK_daisy_Lang.Unzipping, entry.FilenameInZip));

                string unzippedFilePath = unzipDirectory + Path.DirectorySeparatorChar + entry.FilenameInZip;
                if (Path.GetExtension(unzippedFilePath).ToLower() == ".opf"
                    || Path.GetExtension(unzippedFilePath).ToLower() == ".xml")
                {
                    string unzippedFileDir = Path.GetDirectoryName(unzippedFilePath);
                    if (!Directory.Exists(unzippedFileDir))
                    {
                        FileDataProvider.CreateDirectory(unzippedFileDir);
                    }

                    zip.ExtractFile(entry, unzippedFilePath);
                }
            }
            
            zip.Dispose();


            string containerPath = Path.Combine(unzipDirectory,
                                                @"META-INF" + Path.DirectorySeparatorChar + @"container.xml");

            string opfFullPath = null;
            if (!File.Exists(containerPath))
            {
#if DEBUG
                Debugger.Break();
#endif
                DirectoryInfo dirInfo = new DirectoryInfo(unzipDirectory);
                
                FileInfo[] opfFiles = dirInfo.GetFiles("*.opf", SearchOption.AllDirectories);

                foreach (FileInfo fileInfo in opfFiles)
                {
                    
                    opfFullPath = Path.Combine(unzipDirectory, fileInfo.FullName);

                    //m_OPF_ContainerRelativePath = null;

                    
                    break;
                }
            }
            else
            {
                //parseContainerXML(containerPath);
                XmlDocument containerDoc = XmlReaderWriterHelper.ParseXmlDocument(containerPath, false, false);
                XmlNode rootFileNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(containerDoc, true, @"rootfile",
                                                                     containerDoc.DocumentElement.NamespaceURI);

#if DEBUG
            XmlNode mediaTypeAttr = rootFileNode.Attributes.GetNamedItem(@"media-type");
            DebugFix.Assert(mediaTypeAttr != null && mediaTypeAttr.Value == @"application/oebps-package+xml");
#endif

                XmlNode fullPathAttr = rootFileNode.Attributes.GetNamedItem(@"full-path");

                string rootDirectory = Path.GetDirectoryName(containerPath);
                rootDirectory = rootDirectory.Substring(0, rootDirectory.IndexOf(@"META-INF"));

                string OPF_ContainerRelativePath = FileDataProvider.UriDecode(fullPathAttr.Value);

                if (OPF_ContainerRelativePath.StartsWith(@"./"))
                {
                    OPF_ContainerRelativePath = OPF_ContainerRelativePath.Substring(2);
                }

                opfFullPath = Path.Combine(rootDirectory, OPF_ContainerRelativePath.Replace('/', '\\'));


            }
            return opfFullPath;
        }


    }
}