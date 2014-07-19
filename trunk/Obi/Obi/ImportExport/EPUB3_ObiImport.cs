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

namespace Obi.ImportExport
{
    /// <summary>
    /// 
    /// </summary>
    public class EPUB3_ObiImport : DAISY3_ObiImport
    {
        public EPUB3_ObiImport(Session session, Settings settings, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate, bool stereo)
            : base(session, settings, bookfile, outDir, skipACM, audioProjectSampleRate, stereo)
        {
            AudioNCXImport = false;            
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

            
            Presentation spinePresentation = m_Project.Presentations.Get(0);

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

            Presentation spineItemPresentation = null;

            int index = -1;
            
            foreach (string docPath in spineOfContentDocuments)
            {
                index++;

                reportProgress(-1, String.Format(UrakawaSDK_daisy_Lang.ReadXMLDoc, docPath));
                System.Windows.Forms.MessageBox.Show(docPath);
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

                TreenodesWithoutManagedAudioMediaData = new List<TreeNode>();


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

                                if (urlDecoded.IndexOf('#') > 0)
                                {
                                    string[] srcParts = urlDecoded.Split('#');
                                    if (srcParts.Length != 2)
                                    {
                                        continue;
                                    }

                                    string fullTextRefPath = Path.Combine(Path.GetDirectoryName(fullOverlayPath),
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
                                    if(m_XmlIdToSectionNodeMap.ContainsKey (urlDecoded))  textTreeNode = m_XmlIdToSectionNodeMap[urlDecoded];
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
                                if (m_XmlIdToPageNodeMap.ContainsKey(urlDecoded))
                                {
                                    EmptyNode pgNode =  m_XmlIdToPageNodeMap[urlDecoded];
                                    pgNumber = pgNode.PageNumber;
                                    
                                    if(pgNode.IsRooted)  pgNode.Detach();
                                }
                            }
                            
                            if (section != null  && audioNode != null)
                            {
                            PhraseNode audioWrapperNode = m_Presentation.CreatePhraseNode();
                            
                                section.AppendChild(audioWrapperNode);
                                addAudio(audioWrapperNode , audioNode, false, fullOverlayPath);
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
                /*
                spineItemProject.PrettyFormat = m_XukPrettyFormat;

                SaveXukAction action = new SaveXukAction(spineItemProject, spineItemProject, new Uri(xuk_FilePath), true);
                action.ShortDescription = UrakawaSDK_daisy_Lang.SavingXUKFile;
                action.LongDescription = UrakawaSDK_daisy_Lang.SerializeDOMIntoXUKFile;

                action.Progress += new EventHandler<ProgressEventArgs>(
                    delegate(object sender, ProgressEventArgs e)
                    {

                        double val = e.Current;
                        double max = e.Total;

                        int percent = -1;
                        if (val != max)
                        {
                            percent = (int)((val / max) * 100);
                        }

                        reportProgress_Throttle(percent, val + "/" + max);
                        //reportProgress(-1, action.LongDescription);
                        
                        if (RequestCancellation)
                        {
                            e.Cancel();
                        }
                    }
                    );
                    

                action.Finished += new EventHandler<FinishedEventArgs>(
                    delegate(object sender, FinishedEventArgs e)
                    {
                        reportProgress(100, UrakawaSDK_daisy_Lang.XUKSaved);
                    }
                    );
                action.Cancelled += new EventHandler<CancelledEventArgs>(
                    delegate(object sender, CancelledEventArgs e)
                    {
                        reportProgress(0, UrakawaSDK_daisy_Lang.CancelledXUKSaving);
                    }
                    );

                action.DoWork();










                //if (first)
                //{
                //    Presentation presentation = m_Project.Presentations.Get(0);
                //    XmlProperty xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                //    xmlProp.LocalName = "book";
                //    presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                //    xmlProp.NamespaceUri = presentation.PropertyFactory.DefaultXmlNamespaceUri;
                //    TreeNode treeNode = presentation.TreeNodeFactory.Create();
                //    treeNode.AddProperty(xmlProp);
                //    presentation.RootNode = treeNode;

                //    first = false;
                //}

                //foreach (XmlNode childOfBody in bodyElement.ChildNodes)
                //{
                //    parseContentDocument(childOfBody, m_Project.Presentations.Get(0).RootNode, fullDocPath);
                //}
 */                   
            }
            

        }

        protected override TreeNode GetFirstTreeNodeForXmlDocument ( Presentation presentation, XmlNode xmlNode)
        {
            XmlNode xNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "section", null);
            int level = ParseToFineHtmlHeadingLevel(xNode);
            
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

        private int ParseToFineHtmlHeadingLevel(XmlNode xNode)
        {
            //Console.WriteLine("xml node name :" + xNode.LocalName);
            if (xNode.LocalName.StartsWith ("h")
                && (xNode.LocalName == "h1" || xNode.LocalName == "h2" || xNode.LocalName == "h3" 
                || xNode.LocalName == "h4" || xNode.LocalName == "h5" || xNode.LocalName == "h6" ))
            {
                string strLevel = xNode.LocalName.Replace("h", "");
                //Console.WriteLine("str level " + strLevel);
                return int.Parse( strLevel) ;
            }
            else if (xNode.ChildNodes.Count > 0 )
            {
                //Console.WriteLine("child nodes" ); 
                foreach ( XmlNode n in xNode.ChildNodes )
                {
                    //Console.WriteLine ("parsing child node: " + n.LocalName);
                    if (n is XmlElement)  return ParseToFineHtmlHeadingLevel(n);
                }
            }
                
                    return - 1 ;
                
            
        }

        protected override TreeNode CreateAndAddTreeNodeForContentDocument(TreeNode parentNode, XmlNode node, bool isFirstSectionOfDoc)
        {
            TreeNode createdNode = null;
            //Console.WriteLine(node.LocalName);
            if (node.LocalName == "doctitle" || node.LocalName == "section")
            {
                //Console.WriteLine("creating section ");
                SectionNode treeNode = m_Presentation.CreateSectionNode();
                createdNode = treeNode;
                if (parentNode is ObiRootNode)
                {
                    ((ObiNode)parentNode).AppendChild(treeNode);
                }
                else
                {
                    ((SectionNode)parentNode).AppendChild(treeNode);
                }
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

                }

            }
            return createdNode;
        }


    }
}