using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Diagnostics;

using urakawa;
using urakawa.core;
using urakawa.property.xml;
using urakawa.daisy;
using urakawa.daisy.import;
using urakawa.media.data.audio;
using urakawa.media.timing ;
using AudioLib  ;


namespace Obi.ImportExport
{
    public class DAISY3_ObiImport : urakawa.daisy.import.Daisy3_Import
    {
        private ObiPresentation m_Presentation;
        private Session m_Session;
        private urakawa.metadata.Metadata m_TitleMetadata;
        private urakawa.metadata.Metadata m_IdentifierMetadata;

        public DAISY3_ObiImport(Session session, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate)
            : base(bookfile, outDir, skipACM, audioProjectSampleRate)
        {
            m_Session = session;
            XukPath = Path.Combine(m_outDirectory, "project.obi");
            if ( System.IO.Path.GetExtension(bookfile).ToLower() == ".opf")     this.AudioNCXImport = true;
            
            PopulateMetadatasToRemoveList();
        }

        //protected override void CreateProjectFileAndDirectory()
        //{
            //if (!Directory.Exists(m_outDirectory))
            //{
                //Directory.CreateDirectory(m_outDirectory);
            //}
            //m_Xuk_FilePath = m_Session.Path;
        //}

        protected override void initializeProject()
        {
            
            //m_Project = new Project();
            Project = m_Session.Presentation.Project;
#if (DEBUG)
            Project.SetPrettyFormat(true);
#else
            Project.SetPrettyFormat(false);
#endif

            //Presentation presentation = m_Project.AddNewPresentation(new Uri(m_outDirectory), Path.GetFileName(m_Book_FilePath));
            m_Presentation = m_Session.Presentation;

            PCMFormatInfo pcmFormat = m_Presentation.MediaDataManager.DefaultPCMFormat.Copy();
            pcmFormat.Data.SampleRate = (ushort)m_audioProjectSampleRate;
            m_Presentation.MediaDataManager.DefaultPCMFormat = pcmFormat;

            m_Presentation.MediaDataManager.EnforceSinglePCMFormat = true;

            //m_textChannel = m_Presentation.ChannelFactory.CreateTextChannel();
            //m_textChannel.Name = "The Text Channel";
            m_textChannel = m_Presentation.ChannelsManager.GetOrCreateTextChannel();

            //m_audioChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
            //m_audioChannel.Name = "The Audio Channel";
            m_audioChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();

            m_TitleMetadata = m_Presentation.GetFirstMetadataItem(Metadata.DC_TITLE);
            m_IdentifierMetadata = m_Presentation.GetFirstMetadataItem(Metadata.DC_IDENTIFIER);
        }

        protected override TreeNode CreateTreeNodeForNavPoint(TreeNode parentNode, XmlNode navPoint)
        {
            SectionNode treeNode = m_Presentation.CreateSectionNode () ;

                //= parentNode.Presentation.TreeNodeFactory.Create();
            //parentNode.AppendChild(treeNode);
            if (navPoint.LocalName == "docTitle")
            {
                
                ((ObiNode)m_Presentation.RootNode).Insert(treeNode,0);
            }
            else
            {
                if (parentNode is ObiRootNode)
                {
                    ((ObiNode)parentNode).AppendChild(treeNode);
                }
                else
                {
                    ((SectionNode)parentNode).AppendChild(treeNode);
                }
            }
            //XmlProperty xmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
            //treeNode.AddProperty(xmlProp);
            XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navPoint, true, "text", navPoint.NamespaceURI);
            //xmlProp.LocalName = "level";//+":" + textNode.InnerText;
            // create urakawa tree node

            //TextMedia textMedia = parentNode.Presentation.MediaFactory.CreateTextMedia();
            //textMedia.Text = textNode.InnerText;
            treeNode.Label = textNode.InnerText; ;
            //ChannelsProperty cProp = parentNode.Presentation.PropertyFactory.CreateChannelsProperty();
            //cProp.SetMedia(m_textChannel, textMedia);

            //TreeNode txtWrapperNode = parentNode.Presentation.TreeNodeFactory.Create();
            //txtWrapperNode.AddProperty(cProp);
            //treeNode.AppendChild(txtWrapperNode);

            //XmlProperty TextNodeXmlProp = parentNode.Presentation.PropertyFactory.CreateXmlProperty();
            //txtWrapperNode.AddProperty(TextNodeXmlProp);
            //TextNodeXmlProp.LocalName = "hd";

            return treeNode;
        }

        protected override TreeNode CreateTreeNodeForAudioNode(TreeNode navPointTreeNode, bool isHeadingNode, XmlNode smilNode)
        {
            PhraseNode audioWrapperNode = m_Presentation.CreatePhraseNode ();
            if ( smilNode == null || !m_SmilXmlNodeToTreeNodeMap.ContainsKey (smilNode))
            {
            ((SectionNode)navPointTreeNode).AppendChild(audioWrapperNode);
            }
            else
            {
                ((SectionNode)navPointTreeNode).InsertAfter(audioWrapperNode, m_SmilXmlNodeToTreeNodeMap[smilNode]);
                m_SmilXmlNodeToTreeNodeMap[smilNode]=audioWrapperNode;
            }
            
            return audioWrapperNode;
            /*
            if (isHeadingNode)
            {
                foreach (TreeNode txtNode in navPointTreeNode.Children.ContentsAs_YieldEnumerable)
                {
                    if (txtNode.GetTextMedia() != null)
                    {
                        audioWrapperNode = txtNode;
                        break;
                    }
                }
            }
            else
            {
                if (navPointTreeNode == null) return null;
                audioWrapperNode = navPointTreeNode.Presentation.TreeNodeFactory.Create();

                navPointTreeNode.AppendChild(audioWrapperNode);
            }
            //XmlProperty xmlProp = navPointTreeNode.Presentation.PropertyFactory.CreateXmlProperty();
            //audioWrapperNode.AddProperty(xmlProp);
            //xmlProp.LocalName = "phrase"; // +":" + navPointTreeNode.GetTextFlattened(false);
            return audioWrapperNode;
             */ 
        }

        protected override void AddPagePropertiesToAudioNode(TreeNode audioWrapperNode, XmlNode pageTargetNode)
        {
            string strKind = pageTargetNode.Attributes.GetNamedItem("type").Value;
            PageKind kind = strKind == "front" ? PageKind.Front :
                strKind == "normal" ? PageKind.Normal :
                PageKind.Special;

            string pageNumberString = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            PageNumber number = null;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber > 0)
                {
                    number = new PageNumber(pageNumber, kind);
                }
                else
                {
                    pageNumberString = pageTargetNode.Attributes.GetNamedItem("value") != null ? pageTargetNode.Attributes.GetNamedItem("value").Value : "";
                    pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                    if (pageNumber > 0) number = new PageNumber(pageNumber, kind);
                }
            }
            if (number != null)
            {
                EmptyNode node = (EmptyNode)audioWrapperNode ;
                node.PageNumber = number;
                
            }

            /*
            TextMedia textMedia = audioWrapperNode.Presentation.MediaFactory.CreateTextMedia();
            textMedia.Text = XmlDocumentHelper.GetFirstChildElementWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
            ChannelsProperty cProp = audioWrapperNode.Presentation.PropertyFactory.CreateChannelsProperty();
            cProp.SetMedia(m_textChannel, textMedia);
            audioWrapperNode.AddProperty(cProp);
            System.Xml.XmlAttributeCollection pageAttributes = pageTargetNode.Attributes;
            if (pageAttributes != null)
            {
                XmlProperty xmlProp = audioWrapperNode.GetXmlProperty();
                foreach (System.Xml.XmlAttribute attr in pageAttributes)
                {
                    xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                }
            }
*/
        }

        protected override TreeNode CheckAndAssignForHeadingAudio(TreeNode navPointTreeNode, TreeNode phraseTreeNode, XmlNode audioXmlNode)
        {
            XmlNode navLabelXmlNode = m_NavPointNode_NavLabelMap[navPointTreeNode];
            XmlNode headingAudio = XmlDocumentHelper.GetFirstChildElementWithName(navLabelXmlNode, true, "audio", navLabelXmlNode.NamespaceURI);
            //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navLabelXmlNode, true, "text", navLabelXmlNode.NamespaceURI);
            if (headingAudio == null)
            {
                Console.WriteLine("NCX Heading node is null for " + navLabelXmlNode.InnerText);
                return null;
            }
            double headingClipBegin = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipBegin").Value)).AsTimeSpan.TotalMilliseconds);
            double headingClipEnd = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipEnd").Value)).AsTimeSpan.TotalMilliseconds);

            double audioClipBegin = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipBegin").Value)).AsTimeSpan.TotalMilliseconds);
            double audioClipEnd = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipEnd").Value)).AsTimeSpan.TotalMilliseconds);

            if ( ((SectionNode)navPointTreeNode).PhraseChild(0) != phraseTreeNode 
                &&   headingAudio.Attributes.GetNamedItem("src").Value == audioXmlNode.Attributes.GetNamedItem("src").Value
                &&    Math.Abs (headingClipBegin - audioClipBegin) <= 1
                && Math.Abs (headingClipEnd - audioClipEnd) <=1 )
            {
                ((EmptyNode)phraseTreeNode).Role_ = EmptyNode.Role.Heading; 
            }
            return phraseTreeNode;
        }

        public static void getTitleFromOpfFile(string opfFilePath, ref string dc_Title, ref string dc_Identifier)
        {
            string opfTitle = "";
            string identifier = "";
            XmlDocument opfFileDoc = urakawa.xuk.OpenXukAction.ParseXmlDocument(opfFilePath, false);

            XmlNode packageNode = XmlDocumentHelper.GetFirstChildElementWithName(opfFileDoc.DocumentElement, true, "package", null);
            string uidAttribute = packageNode.Attributes.GetNamedItem("unique-identifier").Value;
            XmlNodeList listOfChildrenOfDCMetadata = opfFileDoc.GetElementsByTagName("dc-metadata");
            foreach (XmlNode xnode in listOfChildrenOfDCMetadata)
            {
                foreach (XmlNode node in xnode.ChildNodes)
                {

                    if (node.Name == "dc:Title" && string.IsNullOrEmpty(opfTitle))
                    {
                        opfTitle = node.InnerText;
                        
                    }
                    if (node.Name == "dc:Identifier" &&  string.IsNullOrEmpty(identifier) 
                        && node.Attributes.GetNamedItem("id") != null && node.Attributes.GetNamedItem("id").Value == uidAttribute)
                    {
                        identifier = node.InnerText;
                    }

                    if (!string.IsNullOrEmpty(opfTitle) && !string.IsNullOrEmpty(identifier)) break;
                    
                }
            }
            if (!string.IsNullOrEmpty(opfTitle))    dc_Title= opfTitle;
            if (!string.IsNullOrEmpty(identifier))  dc_Identifier = identifier;
        }

        public static string getTitleFromDtBookFile(string dtBookFilePath)
        {
            string dtbBookTitle = "";
            XmlDocument dtbookFileDoc = urakawa.xuk.OpenXukAction.ParseXmlDocument(dtBookFilePath, false);
            
            XmlNodeList listOfChildren = dtbookFileDoc.GetElementsByTagName("meta");
            foreach (XmlNode node in listOfChildren)
            {
                XmlAttributeCollection metaAttr = node.Attributes;

                if (metaAttr == null || metaAttr.Count <= 0)
                {
                    return "";
                }

                XmlNode attrName = metaAttr.GetNamedItem("name");
                XmlNode attrContent = metaAttr.GetNamedItem("content");

                if (attrName != null && !String.IsNullOrEmpty(attrName.Value) && attrContent != null && !String.IsNullOrEmpty(attrContent.Value))
                {
                    Console.WriteLine(attrName.Value + " " + attrContent.Value);
                    if (attrName.Value == "dc:Title")
                    {
                        dtbBookTitle = attrContent.Value;
                        break;
                    }
                }
            }
            return dtbBookTitle;
        }


        protected override void parseContentDocument(XmlNode xmlNode, TreeNode parentTreeNode, string filePath)
        {
            if (RequestCancellation) return;

            XmlNodeType xmlType = xmlNode.NodeType;
            switch (xmlType)
            {
                case XmlNodeType.Attribute:
                    {
                        System.Diagnostics.Debug.Fail("Calling this method with an XmlAttribute should never happen !!");
                        break;
                    }
                case XmlNodeType.Document:
                    {
                        /*
                        XmlDocument xmlDoc = ((XmlDocument)xmlNode);
                        XmlNodeList styleSheetNodeList = xmlDoc.SelectNodes
                                                      ("/processing-instruction(\"xml-stylesheet\")");
                        if (styleSheetNodeList != null && styleSheetNodeList.Count > 0)
                        {
                            AddStyleSheetsToXuk(styleSheetNodeList);
                        }
                        */
                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlNode, true, "body", null);

                        if (bodyElement == null)
                        {
                            bodyElement = XmlDocumentHelper.GetFirstChildElementWithName(xmlNode, true, "book", null);
                        }

                        if (bodyElement != null)
                        {
                            Presentation presentation = Project.Presentations.Get(0);
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                            /*
                            // preserve internal DTD if it exists in dtbook 
                            string strInternalDTD = ExtractInternalDTD(((XmlDocument)xmlNode).DocumentType);
                            if (strInternalDTD != null)
                            {
                                byte[] bytesArray = System.Text.Encoding.UTF8.GetBytes(strInternalDTD);
                                MemoryStream ms = new MemoryStream(bytesArray);
ExternalFiles.ExternalFileData dtdEfd = presentation.ExternalFilesDataFactory.Create<ExternalFiles.DTDExternalFileData>();
                                dtdEfd.InitializeWithData(ms, "DTBookLocalDTD.dtd", false);
                            }
                            */
                            parseContentDocument(bodyElement, parentTreeNode, filePath);
                        }
                        //parseContentDocument(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {
                        Presentation presentation = Project.Presentations.Get(0);

                        TreeNode treeNode = null ;

                        if (parentTreeNode == null)
                        {
                            //treeNode = presentation.TreeNodeFactory.Create();
                            //presentation.RootNode = treeNode;
                            parentTreeNode = presentation.RootNode;
                        }
                         if ( parentTreeNode != null )
                        {
                            treeNode = CreateAndAddTreeNodeForContentDocument(parentTreeNode, xmlNode);
                            //parentTreeNode.AppendChild(treeNode);
                        }
                        XmlProperty xmlProp = null ;
                        if (treeNode != null)
                        {
                            xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                            treeNode.AddProperty(xmlProp);
                            xmlProp.LocalName = xmlNode.LocalName;
                        }
                        if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                        }

                        if (xmlNode.NamespaceURI != presentation.PropertyFactory.DefaultXmlNamespaceUri && xmlProp != null)
                        {
                            xmlProp.NamespaceUri = xmlNode.NamespaceURI;
                        }

                        string updatedSRC = null;
                        /*
                        if (xmlNode.LocalName == "img")
                        {
                            XmlNode getSRC = xmlNode.Attributes.GetNamedItem("src");
                            if (getSRC != null)
                            {
                                string relativePath = xmlNode.Attributes.GetNamedItem("src").Value;
                                if (!relativePath.StartsWith("http://"))
                                {
                                    
                                    string parentPath = Directory.GetParent(filePath).FullName;
                                    string imgSourceFullpath = Path.Combine(parentPath, relativePath);

                                    if (File.Exists(imgSourceFullpath))
                                    {
                                        updatedSRC = Path.GetFullPath(imgSourceFullpath).Replace(
                                            Path.GetDirectoryName(m_Book_FilePath), "");
                                        if (updatedSRC.StartsWith("" + Path.DirectorySeparatorChar))
                                        {
                                            updatedSRC = updatedSRC.Remove(0, 1);
                                        }


                                        //ChannelsProperty chProp = presentation.PropertyFactory.CreateChannelsProperty();
                                        //treeNode.AddProperty(chProp);
                                        ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();

                                        urakawa.media.data.image.ImageMediaData imageData =
                                            CreateImageMediaData(presentation, Path.GetExtension(imgSourceFullpath));
                                        imageData.InitializeImage(imgSourceFullpath, updatedSRC);
                                        media.data.image.ManagedImageMedia managedImage =
                                            presentation.MediaFactory.CreateManagedImageMedia();
                                        managedImage.MediaData = imageData;
                                        chProp.SetMedia(m_ImageChannel, managedImage);
                                    }
                                    else
                                    {
                                        ExternalImageMedia externalImage = presentation.MediaFactory.CreateExternalImageMedia();
                                        externalImage.Src = relativePath;

                                        ChannelsProperty chProp = treeNode.GetOrCreateChannelsProperty();
                                        chProp.SetMedia(m_ImageChannel, externalImage);
                                    }
                                     
                                }
                         
                            }
                         
                        }
                        */

                        /*
                        XmlAttributeCollection attributeCol = xmlNode.Attributes;

                        if (attributeCol != null)
                        {
                            for (int i = 0; i < attributeCol.Count; i++)
                            {
                                XmlNode attr = attributeCol.Item(i);
                                if (attr.LocalName != "smilref"
                                    && attr.LocalName != "imgref") // && attr.Name != "xmlns:xsi" && attr.Name != "xml:space"
                                {
                                    if (attr.Name.Contains(":"))
                                    {
                                        string[] splitArray = attr.Name.Split(':');

                                        if (splitArray[0] == "xmlns")
                                        {
                                            if (xmlNode.LocalName == "book" || treeNode.Parent == null)
                                            {
                                                xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                            }
                                        }
                                        else
                                        {
                                            xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                        }
                                    }
                                    else if (updatedSRC != null && attr.LocalName == "src")
                                    {
                                        xmlProp.SetAttribute(attr.LocalName, "", updatedSRC);
                                    }
                                    else
                                    {
                                        if (attr.LocalName == "xmlns")
                                        {
                                            if (attr.Value != presentation.PropertyFactory.DefaultXmlNamespaceUri)
                                            {
                                                xmlProp.SetAttribute(attr.LocalName, "", attr.Value);
                                            }
                                        }
                                        else if (string.IsNullOrEmpty(attr.NamespaceURI)
                                            || attr.NamespaceURI == presentation.PropertyFactory.DefaultXmlNamespaceUri)
                                        {
                                            xmlProp.SetAttribute(attr.LocalName, "", attr.Value);
                                        }
                                        else
                                        {
                                            xmlProp.SetAttribute(attr.Name, attr.NamespaceURI, attr.Value);
                                        }
                                    }
                                }
                            }
                        }
                        */
                        if (parentTreeNode is SectionNode 
                            &&  (xmlNode.LocalName == "h1" || xmlNode.LocalName == "h2" || xmlNode.LocalName == "h3"
                            || xmlNode.LocalName == "h4" || xmlNode.LocalName == "h5" || xmlNode.LocalName == "h6" || xmlNode.LocalName == "HD" ))
                        {
                            ((SectionNode)parentTreeNode).Label = xmlNode.InnerText;
                        }
                        if (treeNode != null && treeNode is SectionNode && xmlNode.LocalName == "doctitle")
                        {
                            ((SectionNode)treeNode).Label = xmlNode.InnerText;
                        }


                        if (RequestCancellation) return;
                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseContentDocument(childXmlNode, treeNode!= null && treeNode is SectionNode? treeNode: parentTreeNode, filePath);
                        }
                        break;
                    }
                case XmlNodeType.Whitespace:
                case XmlNodeType.CDATA:
                case XmlNodeType.SignificantWhitespace:
                case XmlNodeType.Text:
                    {
                        /*
                        Presentation presentation = Project.Presentations.Get(0);

                        if (xmlType == XmlNodeType.Whitespace)
                        {
                            bool onlySpaces = true;
                            for (int i = 0; i < xmlNode.Value.Length; i++)
                            {
                                if (xmlNode.Value[i] != ' ')
                                {
                                    onlySpaces = false;
                                    break;
                                }
                            }
                            if (!onlySpaces)
                            {
                                break;
                            }
                            //else
                            //{
                            //    int l = xmlNode.Value.Length;
                            //}
                        }
#if DEBUG
                        if (xmlType == XmlNodeType.CDATA)
                        {
                            Debugger.Break();
                        }

                        if (xmlType == XmlNodeType.SignificantWhitespace)
                        {
                            Debugger.Break();
                        }
#endif
                        //string text = xmlNode.Value.Trim();
                        string text = System.Text.RegularExpressions.Regex.Replace(xmlNode.Value, @"\s+", " ");

                        Debug.Assert(!string.IsNullOrEmpty(text));

#if DEBUG
                        if (text.Length != xmlNode.Value.Length)
                        {
                            int debug = 1;
                            //Debugger.Break();
                        }

                        if (string.IsNullOrEmpty(text))
                        {
                            Debugger.Break();
                        }
                        if (xmlType != XmlNodeType.Whitespace && text == " ")
                        {
                            int debug = 1;
                            //Debugger.Break();
                        }
#endif
                        if (string.IsNullOrEmpty(text))
                        {
                            break;
                        }
                        urakawa.media.TextMedia textMedia = presentation.MediaFactory.CreateTextMedia();
                        textMedia.Text = text;

                        urakawa.property.channel.ChannelsProperty cProp = presentation.PropertyFactory.CreateChannelsProperty();
                        cProp.SetMedia(m_textChannel, textMedia);


                        int counter = 0;
                        foreach (XmlNode childXmlNode in xmlNode.ParentNode.ChildNodes)
                        {
                            XmlNodeType childXmlType = childXmlNode.NodeType;
                            if (childXmlType == XmlNodeType.Text
                                || childXmlType == XmlNodeType.Element
                                || childXmlType == XmlNodeType.Whitespace
                                || childXmlType == XmlNodeType.SignificantWhitespace
                                || childXmlType == XmlNodeType.CDATA)
                            {
                                counter++;
                            }
                        }
                        if (counter == 1)
                        {
                            parentTreeNode.AddProperty(cProp);
                        }
                        else
                        {
                            TreeNode txtWrapperNode = presentation.TreeNodeFactory.Create();
                            txtWrapperNode.AddProperty(cProp);
                            parentTreeNode.AppendChild(txtWrapperNode);
                        }
                        */
                        break;
                         
                    }
                default:
                    {
                        return;
                    }
            }
        }

        private TreeNode CreateAndAddTreeNodeForContentDocument(TreeNode parentNode, XmlNode node)
        {
                        TreeNode createdNode = null;
                        //Console.WriteLine(node.LocalName);
                        if (node.LocalName.StartsWith("level") || node.LocalName == "doctitle")
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
            else if (node.LocalName == "pagenum" && parentNode is SectionNode)
            {
                EmptyNode treeNode = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                createdNode = treeNode;
                ((SectionNode)parentNode).AppendChild(treeNode);

                string strKind  = node.Attributes.GetNamedItem("page").Value;
                string pageNumberString = node.InnerText;

                PageKind kind = strKind == "front" ? PageKind.Front :
                strKind == "normal" ? PageKind.Normal :
                PageKind.Special;

                PageNumber number = null;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber > 0)
                {
                    number = new PageNumber(pageNumber, kind);
                }
                if (number != null)
                {   
                     ((EmptyNode) treeNode).PageNumber = number;

                }
            }
            }
            return createdNode;
        }

        private List<string> m_MetadataItemsToExclude = new List<string>();
        protected override List<string> MetadataItemsToExclude { get { return m_MetadataItemsToExclude; } }

        private void PopulateMetadatasToRemoveList()
        {
            m_MetadataItemsToExclude.Add(Metadata.DC_FORMAT);
            m_MetadataItemsToExclude.Add(Metadata.DTB_AUDIO_FORMAT);
            m_MetadataItemsToExclude.Add(Metadata.DTB_MULTIMEDIA_CONTENT);
            m_MetadataItemsToExclude.Add(Metadata.DTB_MULTIMEDIA_TYPE);
            m_MetadataItemsToExclude.Add(Metadata.OBI_DAISY2ExportPath);
            m_MetadataItemsToExclude.Add(Metadata.OBI_DAISY3ExportPath);
            m_MetadataItemsToExclude.Add(Metadata.GENERATOR);
        }

        protected override void RemoveMetadataItemsToBeExcluded()
        {
            base.RemoveMetadataItemsToBeExcluded();

            // now make sure that there is single identifier and no duplicate title with the same value
            foreach (urakawa.metadata.Metadata m in m_Presentation.Metadatas.ContentsAs_ListCopy)
            {
                if (m_IdentifierMetadata != null && m_IdentifierMetadata.NameContentAttribute.Name == m.NameContentAttribute.Name
                    && m_IdentifierMetadata.NameContentAttribute.Value != m.NameContentAttribute.Value)
                {
                    m_Presentation.Metadatas.Remove(m);
                }

                if (m_TitleMetadata != null && m_TitleMetadata != m
                    && m_TitleMetadata.NameContentAttribute.Name == m.NameContentAttribute.Name && m_TitleMetadata.NameContentAttribute.Value == m.NameContentAttribute.Value)
                {
                    m_Presentation.Metadatas.Remove(m);
                }

            }
        }


        protected override bool addAudioWavWithEndOfFileTolerance(urakawa.media.data.audio.codec.WavAudioMediaData mediaData, urakawa.data.FileDataProvider dataProv, Time clipB, Time clipE, TreeNode treeNode)
        {
            bool isClipEndError = true;

            uint dataLength = 0 ;
            Stream wavStream = null;
            AudioLibPCMFormat PCMFormat = null ;
            try
            {
                wavStream = dataProv.OpenInputStream();
                 PCMFormat = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isClipEndError = true ;
            }
            finally
            {
                if(wavStream != null )  wavStream.Close () ;
            }

            Time fileDuration = new Time(PCMFormat.ConvertBytesToTime((long) dataLength));
            if (clipB.IsLessThan (clipE))
            {//1
                double diff =  clipE.AsTimeSpan.TotalMilliseconds - fileDuration.AsTimeSpan.TotalMilliseconds;
                if (clipB.IsLessThan(fileDuration))
                {//2
                    try
                    {//3
                        mediaData.AppendPcmData(dataProv, clipB, fileDuration);
                        isClipEndError = false;
                        Console.WriteLine("Obi: clip end adjust according to file length : Clip end" + clipE.AsTimeSpan.ToString() + " File length:" + fileDuration.AsTimeSpan.ToString());
                    }//-3
                    catch (Exception ex)
                    {//3
                        isClipEndError = true;
                    }//-3
                    // to do: add obi specific code here
                    if (diff > 100 && treeNode != null )
                    {
                        EmptyNode eNode = (EmptyNode)treeNode;
                        eNode.TODO = true;
                        eNode.Role_ = EmptyNode.Role.Custom ;
                        eNode.CustomRole = "Truncated audio";
                    }
                }//-2
                
            }//-1
            else
            {//1
                Console.WriteLine("clip begin is larger than clip end");
            }//-1
            return isClipEndError;
        }



        private void ReplaceExternalAudioMediaPhraseWithEmptyNode (TreeNode node)
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
            }
             
        }

        public void CorrectExternalAudioMedia()
        {
            if (TreenodesWithoutManagedAudioMediaData == null || TreenodesWithoutManagedAudioMediaData.Count == 0) return;
            for (int i = 0; i < TreenodesWithoutManagedAudioMediaData.Count; i++)
            {   
                if ( TreenodesWithoutManagedAudioMediaData[i] is PhraseNode )
                {
                    PhraseNode phrase = (PhraseNode)TreenodesWithoutManagedAudioMediaData[i];
                    ReplaceExternalAudioMediaPhraseWithEmptyNode(phrase);
                }
            }
        }

    }
}
