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
using urakawa.media.timing;
using AudioLib;


namespace Obi.ImportExport
{
    public class DAISY3_ObiImport : urakawa.daisy.import.Daisy3_Import
    {
        protected ObiPresentation m_Presentation;
        private Session m_Session;
        private Settings m_Settings;
        private urakawa.metadata.Metadata m_TitleMetadata;
        private urakawa.metadata.Metadata m_IdentifierMetadata;
        protected List<PhraseNode> m_PhrasesWithTruncatedAudio;
        private List<string> m_ErrorsList;
        private urakawa.property.channel.TextChannel m_textChannel;
        private urakawa.property.channel.AudioChannel m_audioChannel;
        protected Dictionary<string, SectionNode> m_XmlIdToSectionNodeMap = new Dictionary<string, SectionNode>();
        protected Dictionary<string, EmptyNode> m_XmlIdToPageNodeMap = new Dictionary<string, EmptyNode>();
        

        public DAISY3_ObiImport(Session session, Settings settings, string bookfile, string outDir, bool skipACM, SampleRate audioProjectSampleRate, bool stereo)
            : base(bookfile, outDir, skipACM, audioProjectSampleRate, stereo, false, true)
        {
            m_Session = session;
            m_Settings = settings;
            XukPath = Path.Combine(m_outDirectory, "project.obi");
            if (System.IO.Path.GetExtension(bookfile).ToLower() == ".opf") this.AudioNCXImport = true;

            PopulateMetadatasToRemoveList();
            m_PhrasesWithTruncatedAudio = new List<PhraseNode>();
            m_ErrorsList = new List<string>();
            base.IsRenameOfProjectFileAndDirsAllowedAfterImport= false;
        }

        public List<string> ErrorsList { get { return m_ErrorsList; } }

        //protected override void CreateProjectFileAndDirectory()
        //{
        //if (!Directory.Exists(m_outDirectory))
        //{
        //Directory.CreateDirectory(m_outDirectory);
        //}
        //m_Xuk_FilePath = m_Session.Path;
        //}

        protected override void initializeProject(string path)
        {

            //m_Project = new Project();
            m_Project = m_Session.Presentation.Project;
#if false //(DEBUG)
            m_Project.PrettyFormat = true;
#else
            m_Project.PrettyFormat = false;
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
            m_XmlIdToSectionNodeMap = new Dictionary<string, SectionNode>();
            m_XmlIdToPageNodeMap = new Dictionary<string, EmptyNode>();
            
        }

        protected override TreeNode CreateTreeNodeForNavPoint(TreeNode parentNode, XmlNode navPoint)
        {
            SectionNode treeNode = m_Presentation.CreateSectionNode();

            //= parentNode.Presentation.TreeNodeFactory.Create();
            //parentNode.AppendChild(treeNode);
            if (navPoint.LocalName == "docTitle")
            {

                ((ObiNode)m_Presentation.RootNode).Insert(treeNode, 0);
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
            XmlNode textNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navPoint, true, "text", navPoint.NamespaceURI);
            //xmlProp.LocalName = "level";//+":" + textNode.InnerText;
            // create urakawa tree node

            //TextMedia textMedia = parentNode.Presentation.MediaFactory.CreateTextMedia();
            //textMedia.Text = textNode.InnerText;
            //treeNode.Label = textNode.InnerText;
            string strLabel = textNode.InnerText;
            if (strLabel.Contains("\n")) strLabel = strLabel.Replace("\n", "");
            treeNode.Label = strLabel;
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
        private Dictionary<EmptyNode, string> m_AnchorNodeSmilRefMap = new Dictionary<EmptyNode, string>();
        private Dictionary<EmptyNode, List<string>> m_Skippable_IdMap = new Dictionary<EmptyNode, List<string>>();
        protected override TreeNode CreateTreeNodeForAudioNode(TreeNode navPointTreeNode, bool isHeadingNode, XmlNode smilNode, string fullSmilPath)
        {
            PhraseNode audioWrapperNode = m_Presentation.CreatePhraseNode();
            if (smilNode == null || !m_SmilXmlNodeToTreeNodeMap.ContainsKey(smilNode))
            {
                ((SectionNode)navPointTreeNode).AppendChild(audioWrapperNode);

                XmlNode seqParent = smilNode != null ? smilNode.ParentNode : null;
                while (seqParent != null)
                {
                    if ((seqParent.Name == "seq" || seqParent.Name == "par") && (seqParent.Attributes != null && seqParent.Attributes.GetNamedItem("customTest") != null)) break;
                    seqParent = seqParent.ParentNode;

                }

                string strClass = null;

                if (seqParent != null && seqParent.Attributes.GetNamedItem("class") != null
                && (strClass = seqParent.Attributes.GetNamedItem("class").Value) != null
                && (strClass == EmptyNode.Annotation || strClass == EmptyNode.EndNote || strClass == EmptyNode.Footnote
                || strClass == EmptyNode.ProducerNote || strClass == EmptyNode.Sidebar || strClass == EmptyNode.Note))
                {
                    audioWrapperNode.SetRole(EmptyNode.Role.Custom, strClass);
                    if (!m_Skippable_IdMap.ContainsKey(audioWrapperNode))
                    {
                        ObiNode preceedingNode = audioWrapperNode.PrecedingNode;
                        if (preceedingNode == null || preceedingNode is SectionNode || ((EmptyNode)preceedingNode).CustomRole != audioWrapperNode.CustomRole)
                        {
                            m_Skippable_IdMap.Add(audioWrapperNode, new List<string>());
                            XmlNode seqChild = seqParent;
                            while (seqChild != null && seqChild != smilNode)
                            {
                                if (seqChild.Attributes.GetNamedItem("id") != null) m_Skippable_IdMap[audioWrapperNode].Add(Path.GetFileName(fullSmilPath) + "#" + seqChild.Attributes.GetNamedItem("id").Value);
                                seqChild = seqChild.FirstChild;
                            }


                            AssignSkippableToAnchorNode();
                        }
                    }
                }
                else if (seqParent != null && seqParent.Attributes != null && seqParent.Attributes.GetNamedItem("customTest") != null)
                {

                    XmlNode anchorNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(seqParent, true, "a", seqParent.NamespaceURI);
                    if (anchorNode != null)
                    {

                        string strReference = anchorNode.Attributes.GetNamedItem("href").Value;
                        audioWrapperNode.SetRole(EmptyNode.Role.Anchor, null);
                        if (!m_AnchorNodeSmilRefMap.ContainsKey(audioWrapperNode))
                        {
                            string[] refArray = strReference.Split('#');
                            if (refArray.Length == 1 || string.IsNullOrEmpty(refArray[0])) strReference = Path.GetFileName(fullSmilPath) + "#" + refArray[refArray.Length - 1];
                            m_AnchorNodeSmilRefMap.Add(audioWrapperNode, strReference);

                        }


                    }
                }
            }
            else
            {

                ((SectionNode)navPointTreeNode).InsertAfter(audioWrapperNode, m_SmilXmlNodeToTreeNodeMap[smilNode]);
                m_SmilXmlNodeToTreeNodeMap[smilNode] = audioWrapperNode;
            }

            return audioWrapperNode;

        }

        protected override TreeNode AddAnchorNode(TreeNode navPointTreeNode, XmlNode smilNode, string fullSmilPath)
        {
            XmlNode seqParent = smilNode != null ? smilNode.ParentNode : null;
            while (seqParent != null)
            {
                if (seqParent.Name == "seq" || seqParent.Attributes.GetNamedItem("customTest") != null) break;
                seqParent = seqParent.ParentNode;

            }
            if (seqParent == null || XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilNode, true, "audio", smilNode.NamespaceURI) == null) return null;

            EmptyNode anchor = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
            ((SectionNode)navPointTreeNode).AppendChild(anchor);

            if (smilNode != null)
            {
                string strReference = smilNode.Attributes.GetNamedItem("href").Value;
                anchor.SetRole(EmptyNode.Role.Anchor, null);
                if (!m_AnchorNodeSmilRefMap.ContainsKey(anchor))
                {
                    string[] refArray = strReference.Split('#');
                    if (refArray.Length == 1 || string.IsNullOrEmpty(refArray[0])) strReference = Path.GetFileName(fullSmilPath) + "#" + refArray[refArray.Length - 1];
                    m_AnchorNodeSmilRefMap.Add(anchor, strReference);

                }
            }
            return anchor;
        }

        private void AssignSkippableToAnchorNode()
        {
            List<EmptyNode> nodeToRemove = new List<EmptyNode>();
            foreach (EmptyNode anchor in m_AnchorNodeSmilRefMap.Keys)
            {
                string strRef = m_AnchorNodeSmilRefMap[anchor];
                foreach (EmptyNode skippable in m_Skippable_IdMap.Keys)
                {
                    List<string> idList = m_Skippable_IdMap[skippable];
                    foreach (string id in idList)
                    {
                        if (strRef == id)
                        {
                            anchor.AssociatedNode = skippable;
                            nodeToRemove.Add(anchor);
                        }
                    }

                }
            }
            foreach (EmptyNode n in nodeToRemove) m_AnchorNodeSmilRefMap.Remove(n);
        }

        protected override void AddPagePropertiesToAudioNode(TreeNode audioWrapperNode, XmlNode pageTargetNode)
        {
            string strKind = pageTargetNode.Attributes.GetNamedItem("type").Value;
            PageKind kind = strKind == "front" ? PageKind.Front :
                strKind == "normal" ? PageKind.Normal :
                PageKind.Special;

            string pageNumberString = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(pageTargetNode, true, "text", pageTargetNode.NamespaceURI).InnerText;
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
                EmptyNode node = (EmptyNode)audioWrapperNode;
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
            XmlNode headingAudio = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navLabelXmlNode, true, "audio", navLabelXmlNode.NamespaceURI);
            //XmlNode textNode = XmlDocumentHelper.GetFirstChildElementWithName(navLabelXmlNode, true, "text", navLabelXmlNode.NamespaceURI);
            if (headingAudio == null)
            {
                Console.WriteLine("NCX Heading node is null for " + navLabelXmlNode.InnerText);
                return null;
            }
            double headingClipBegin = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipBegin").Value)).AsMilliseconds);
            double headingClipEnd = Math.Abs((new urakawa.media.timing.Time(headingAudio.Attributes.GetNamedItem("clipEnd").Value)).AsMilliseconds);

            double audioClipBegin = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipBegin").Value)).AsMilliseconds);
            double audioClipEnd = Math.Abs((new urakawa.media.timing.Time(audioXmlNode.Attributes.GetNamedItem("clipEnd").Value)).AsMilliseconds);

            if (((SectionNode)navPointTreeNode).PhraseChild(0) != phraseTreeNode
                && headingAudio.Attributes.GetNamedItem("src").Value == audioXmlNode.Attributes.GetNamedItem("src").Value
                && Math.Abs(headingClipBegin - audioClipBegin) <= 1
                && Math.Abs(headingClipEnd - audioClipEnd) <= 1)
            {
                ((EmptyNode)phraseTreeNode).Role_ = EmptyNode.Role.Heading;
            }
            return phraseTreeNode;
        }

        public static void getTitleFromOpfFile(string opfFilePath, ref string dc_Title, ref string dc_Identifier)
        {
            string opfTitle = "";
            string identifier = "";
            XmlDocument opfFileDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(opfFilePath, false, false);

            XmlNode packageNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfFileDoc.DocumentElement, true, "package", null);
            string uidAttribute = packageNode.Attributes.GetNamedItem("unique-identifier").Value;
            XmlNodeList listOfChildrenOfDCMetadata = opfFileDoc.GetElementsByTagName("dc-metadata");
            if (listOfChildrenOfDCMetadata == null || listOfChildrenOfDCMetadata.Count == 0)
            {
                listOfChildrenOfDCMetadata = opfFileDoc.GetElementsByTagName("metadata");
            }
            foreach (XmlNode xnode in listOfChildrenOfDCMetadata)
            {
                foreach (XmlNode node in xnode.ChildNodes)
                {

                    if (node.Name.ToLower () == "dc:title" && string.IsNullOrEmpty(opfTitle))
                    {
                        opfTitle = node.InnerText;

                    }
                    if (node.Name.ToLower () == "dc:identifier" && string.IsNullOrEmpty(identifier)
                        && node.Attributes.GetNamedItem("id") != null && node.Attributes.GetNamedItem("id").Value == uidAttribute)
                    {
                        identifier = node.InnerText;
                    }

                    if (!string.IsNullOrEmpty(opfTitle) && !string.IsNullOrEmpty(identifier)) break;

                }
            }
            if (!string.IsNullOrEmpty(opfTitle)) dc_Title = opfTitle;
            if (!string.IsNullOrEmpty(identifier)) dc_Identifier = identifier;
        }

        public static void getTitleFromDtBookFile(string dtBookFilePath, ref string dc_Title, ref string dc_Identifier)
        {
            string dtbBookTitle = "";
            string identifier = "";
            XmlDocument dtbookFileDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(dtBookFilePath, false, false);

            XmlNodeList listOfChildren = dtbookFileDoc.GetElementsByTagName("meta");
            foreach (XmlNode node in listOfChildren)
            {
                XmlAttributeCollection metaAttr = node.Attributes;

                if (metaAttr == null || metaAttr.Count <= 0)
                {
                    return ;
                }

                XmlNode attrName = metaAttr.GetNamedItem("name");
                XmlNode attrContent = metaAttr.GetNamedItem("content");

                if (attrName != null && !String.IsNullOrEmpty(attrName.Value) && attrContent != null && !String.IsNullOrEmpty(attrContent.Value))
                {
                    Console.WriteLine(attrName.Value + " " + attrContent.Value);
                    if (attrName.Value.ToLower() == "dc:title" && string.IsNullOrEmpty(dtbBookTitle ))
                    {
                        dtbBookTitle = attrContent.Value;
                    }
                    if (attrName.Value.ToLower() == "dc:identifier" && string.IsNullOrEmpty(identifier))
                    {
                        identifier = attrContent.Value;
                    }
                    if (!string.IsNullOrEmpty(dtbBookTitle) && !string.IsNullOrEmpty(identifier)) break;
                }
            }
            Console.WriteLine("title : " + dtbBookTitle + ", identifier:" + identifier);
            if (!string.IsNullOrEmpty(dtbBookTitle)) dc_Title = dtbBookTitle;
            if (!string.IsNullOrEmpty(identifier)) dc_Identifier = identifier;
        }

        public static bool IsEPUBPublication(string filePath)
        {
            if (Path.GetExtension(filePath).ToLower() == ".epub")
            {
                return true;
            }
            else
            {
            
            XmlDocument fileDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(filePath, false, false);

            XmlNodeList listOfChildren = fileDoc.GetElementsByTagName("package");
            foreach (XmlNode node in listOfChildren)
            {
                XmlNode EPUBNamespace = node.Attributes.GetNamedItem("xmlns") ;
                if (EPUBNamespace != null
                    && EPUBNamespace.Value == "http://www.idpf.org/2007/opf")
                {
                    return true ;
                }
            }      
            return false ;
            }
        }


        protected override void parseContentDocument(string filePath, Project project, XmlNode xmlNode, TreeNode parentTreeNode, string dtdUniqueResourceId, DocumentMarkupType docMarkupType)
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
                        
                        XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "body", null);

                        if (bodyElement == null)
                        {
                            bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "book", null);
                        }

                        if (bodyElement != null)
                        {
                            Presentation presentation = m_Project.Presentations.Get(0);
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = bodyElement.NamespaceURI;
                            
                            parseContentDocument(filePath, project, bodyElement, parentTreeNode, null, docMarkupType);
                        }
                        //parseContentDocument(((XmlDocument)xmlNode).DocumentElement, parentTreeNode);
                        break;
                    }
                case XmlNodeType.Element:
                    {
                        Presentation presentation = m_Project.Presentations.Get(0);

                        TreeNode treeNode = null;

                        if (parentTreeNode == null)
                        {
                            parentTreeNode = GetFirstTreeNodeForXmlDocument (presentation, xmlNode) ;
                            //parentTreeNode = presentation.RootNode;
                        }
                        if (parentTreeNode != null)
                        {
                            treeNode = CreateAndAddTreeNodeForContentDocument(parentTreeNode, 
                                xmlNode,
                                Path.GetFileName(filePath));
                            if (treeNode != null && treeNode is EmptyNode && ((EmptyNode)treeNode).PageNumber != null)
                            {
                                string strfRefID = Path.GetFileName(filePath) + "#" + xmlNode.Attributes.GetNamedItem("id").Value;
                                m_XmlIdToPageNodeMap.Add(strfRefID, (EmptyNode)treeNode);
                            }
                            //parentTreeNode.AppendChild(treeNode);
                        }
                        if (xmlNode.ParentNode != null && xmlNode.ParentNode.NodeType == XmlNodeType.Document)
                        {
                            presentation.PropertyFactory.DefaultXmlNamespaceUri = xmlNode.NamespaceURI;
                        }

                        XmlProperty xmlProp = null;
                        if (treeNode != null)
                        {
                            xmlProp = presentation.PropertyFactory.CreateXmlProperty();
                            treeNode.AddProperty(xmlProp);


                            // we get rid of element name prefixes, we use namespace URIs instead.
                            // check inherited NS URI

                            string nsUri = treeNode.Parent != null ?
                                treeNode.Parent.GetXmlNamespaceUri() :
                                xmlNode.NamespaceURI; //presentation.PropertyFactory.DefaultXmlNamespaceUri

                            if (xmlNode.NamespaceURI != nsUri)
                            {
                                nsUri = xmlNode.NamespaceURI;
                                xmlProp.SetQName(xmlNode.LocalName, nsUri == null ? "" : nsUri);
                            }
                            else
                            {
                                xmlProp.SetQName(xmlNode.LocalName, "");
                            }


                            //string nsUri = treeNode.GetXmlNamespaceUri();
                            // if xmlNode.NamespaceURI != nsUri
                            // => xmlProp.GetNamespaceUri() == xmlNode.NamespaceURI
                        }

                        
                        
                        if (parentTreeNode is SectionNode
                            && (xmlNode.LocalName == "h1" || xmlNode.LocalName == "h2" || xmlNode.LocalName == "h3"
                            || xmlNode.LocalName == "h4" || xmlNode.LocalName == "h5" || xmlNode.LocalName == "h6" || xmlNode.LocalName == "HD"))
                        {
                            ((SectionNode)parentTreeNode).Label = xmlNode.InnerText;
                            Console.WriteLine(xmlNode.InnerText);
                            if (xmlNode.Attributes.GetNamedItem("id") != null)
                            {
                                string strfRefID = Path.GetFileName(filePath) + "#" + xmlNode.Attributes.GetNamedItem("id").Value;
                                if (!m_XmlIdToSectionNodeMap.ContainsKey(strfRefID))
                                {
                                    m_XmlIdToSectionNodeMap.Add(strfRefID, (SectionNode)parentTreeNode);
                                }
                            }

                        }
                        if (treeNode != null && treeNode is SectionNode && xmlNode.LocalName == "doctitle")
                        {
                            ((SectionNode)treeNode).Label = xmlNode.InnerText;
                        }


                        if (RequestCancellation) return;
                        foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
                        {
                            parseContentDocument(filePath, project, childXmlNode, treeNode != null && treeNode is SectionNode ? treeNode : parentTreeNode, null, docMarkupType);
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

        protected virtual TreeNode GetFirstTreeNodeForXmlDocument (Presentation presentation, XmlNode xmlNode)
        {
            return presentation.RootNode;
        }

        protected virtual TreeNode CreateAndAddTreeNodeForContentDocument(TreeNode parentNode, XmlNode node, string contentFileName)
        {
        
            TreeNode createdNode = null;
            //Console.WriteLine(node.LocalName);
            if (node.LocalName.StartsWith("level") || node.LocalName == "doctitle" )
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
            else if ( parentNode is SectionNode
                && node.LocalName == "pagenum" )
            {
                EmptyNode treeNode = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                createdNode = treeNode;
                ((SectionNode)parentNode).AppendChild(treeNode);

                PageNumber number = null;
                
                string strKind = node.Attributes.GetNamedItem("page").Value;
                string pageNumberString = node.InnerText;

                PageKind kind = strKind == "front" ? PageKind.Front :
                strKind == "normal" ? PageKind.Normal :
                PageKind.Special;

                
                if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
                {//2
                    number = new PageNumber(pageNumberString);
                }//-2
                else if (kind == PageKind.Front || kind == PageKind.Normal)
                {//2
                    int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                    if (pageNumber > 0)
                    {//3
                        number = new PageNumber(pageNumber, kind);
                    }//-3
                }//-2
                
                    if (number != null)
                    {
                        ((EmptyNode)treeNode).PageNumber = number;

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
            m_MetadataItemsToExclude.Add(Metadata.OBI_EPUB3ExportPath);
            m_MetadataItemsToExclude.Add(Metadata.GENERATOR);
            m_MetadataItemsToExclude.Add("dtb:generator");
        }

        protected override void RemoveMetadataItemsToBeExcluded(Project project)
        {
            base.RemoveMetadataItemsToBeExcluded(project);

            // now make sure that there is single identifier and no duplicate title with the same value
            foreach (urakawa.metadata.Metadata m in project.Presentations.Get(0).Metadatas.ContentsAs_ListCopy)
            {
                if (m_IdentifierMetadata != null && m_IdentifierMetadata.NameContentAttribute.Name == m.NameContentAttribute.Name
                    && m_IdentifierMetadata.NameContentAttribute.Value != m.NameContentAttribute.Value)
                {
                    m_Presentation.Metadatas.Remove(m);
                }

                if (m_TitleMetadata != null && m_TitleMetadata != m
                    && m_TitleMetadata.NameContentAttribute.Name == m.NameContentAttribute.Name && m_TitleMetadata.NameContentAttribute.Value == m.NameContentAttribute.Value)
                {
                    project.Presentations.Get(0).Metadatas.Remove(m);
                }

            }
        }

        protected override void clipEndAdjustedToNull(Time clipB, Time clipE, Time duration, TreeNode treeNode)
        {
            double diff = clipE.AsMilliseconds - duration.AsMilliseconds;
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

        /*
        protected override bool addAudioWavWithEndOfFileTolerance(urakawa.media.data.audio.codec.WavAudioMediaData mediaData, urakawa.data.FileDataProvider dataProv, Time clipB, Time clipE, TreeNode treeNode)
        {
            bool isClipEndError = true;

            uint dataLength = 0;
            Stream wavStream = null;
            AudioLibPCMFormat PCMFormat = null;
            try
            {
                wavStream = dataProv.OpenInputStream();
                PCMFormat = AudioLibPCMFormat.RiffHeaderParse(wavStream, out dataLength);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.ToString());
                isClipEndError = true;
            }
            finally
            {
                if (wavStream != null) wavStream.Close();
            }

            Time fileDuration = new Time(PCMFormat.ConvertBytesToTime((long)dataLength));
            if (clipB.IsLessThan(clipE))
            {//1
                double diff = clipE.AsMilliseconds - fileDuration.AsMilliseconds;
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
                    //if (diff > 100 && treeNode != null )
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
                        m_ErrorsList.Add(Localizer.Message("DAISY3_ObiImport_ErrorsList_expected_clip_end") + clipE.Format_H_MN_S_MS() + Localizer.Message("DAISY3_ObiImport_ErrorsList_imported_clip_end") + fileDuration.Format_H_MN_S_MS());
                    }
                }//-2

            }//-1
            else
            {//1
                Console.WriteLine("clip begin is larger than clip end");
            }//-1
            return isClipEndError;
        }
        */


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
            if (TreenodesWithoutManagedAudioMediaData != null && TreenodesWithoutManagedAudioMediaData.Count > 0)
            {
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
}
