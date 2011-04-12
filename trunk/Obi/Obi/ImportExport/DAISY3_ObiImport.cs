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
using AudioLib ;


namespace Obi.ImportExport
{
    public class DAISY3_ObiImport : urakawa.daisy.import.Daisy3_Import
    {
        ObiPresentation m_Presentation;
        Session m_Session;
        

        public DAISY3_ObiImport(Session session, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate)
            : base(bookfile, outDir, skipACM, audioProjectSampleRate)
        {
            m_Session = session;
            XukPath = Path.Combine(m_outDirectory, "project.obi");
            if ( System.IO.Path.GetExtension(bookfile).ToLower() == ".opf")     this.AudioNCXImport = true;
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

            //m_ImageChannel = m_Presentation.ChannelFactory.CreateImageChannel();
            //m_ImageChannel.Name = "The Image Channel";

            /*string dataPath = presentation.DataProviderManager.DataFileDirectoryFullPath;
           if (Directory.Exists(dataPath))
           {
               Directory.Delete(dataPath, true);
           }*/
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

        public static string getTitleFromOpfFile(string opfFilePath)
        {
            string opfTitle = "";
            XmlDocument opfFileDoc = new XmlDocument();
            opfFileDoc.Load(opfFilePath);
            XmlNodeList listOfChildrenOfDCMetadata = opfFileDoc.GetElementsByTagName("dc-metadata");
            foreach (XmlNode xnode in listOfChildrenOfDCMetadata)
            {
                foreach (XmlNode node in xnode.ChildNodes)
                {
                    if (node.Name == "dc:Title")
                        opfTitle = node.InnerText;
                    break;
                }
            }
            return opfTitle;
        }

        public static string getTitleFromDtBookFile(string dtBookFilePath)
        {
            string dtbBookTitle = "";
            XmlDocument dtbookFileDoc = new XmlDocument();
            dtbookFileDoc.Load(dtBookFilePath);
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
                    dtbBookTitle = attrContent.Value;                
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



    }
}
