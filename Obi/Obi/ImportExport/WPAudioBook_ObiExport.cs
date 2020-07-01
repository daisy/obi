using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    public class WPAudioBook_ObiExport: Daisy3_Export
    {
        protected int m_AudioFileSectionLevel;
        protected Dictionary<XmlNode, urakawa.core.TreeNode> m_AnchorXmlNodeToReferedNodeMap = new Dictionary<XmlNode, urakawa.core.TreeNode>();
        protected Dictionary<urakawa.core.TreeNode, string> m_Skippable_UpstreamIdMap = new Dictionary<TreeNode, string>();
        protected Dictionary<XmlDocument, string> m_AnchorSmilDoc_SmileFileNameMap = new Dictionary<XmlDocument, string>();

        protected const string m_Filename_Manifest = "publication.json";
        protected const string m_Filename_EntryFile = "index.html";
        JArray M_PlaylistArray = null;
        int m_BookDuration = 0;

        public WPAudioBook_ObiExport(ObiPresentation presentation, string exportDirectory, List<string> navListElementNamesList, bool encodeToMp3, double mp3BitRate,
            SampleRate sampleRate, bool stereo, bool skipACM, int audioFileSectionLevel)
            : base(presentation, exportDirectory, navListElementNamesList, encodeToMp3, mp3BitRate,
            sampleRate, stereo,
            skipACM, false, true)
        {
            m_Filename_Content = null;
            m_AudioFileSectionLevel = audioFileSectionLevel;
            GeneratorName = "Obi";
            M_PlaylistArray = new JArray();
            m_BookDuration = 0;
        }


        private bool m_AlwaysIgnoreIndentation = false;
        public bool AlwaysIgnoreIndentation
        {
            get { return m_AlwaysIgnoreIndentation; }
            set { m_AlwaysIgnoreIndentation = value; }
        }


        protected override bool doesTreeNodeTriggerNewSmil(TreeNode node)
        {
            return node is SectionNode
                && IsSectionEmpty((SectionNode)node);
        }

        protected bool IsSectionEmpty(SectionNode section)
        {
            double duration = 0;
            for (int i = 0; i < section.PhraseChildCount; i++)
            {
                EmptyNode eNode = section.PhraseChild(i);
                if (eNode is PhraseNode && eNode.Used) duration += eNode.Duration;
            }
            return duration > 0;
        }

        public override void ConfigureAudioFileDelegates()
        {
            m_TriggerDelegate = delegate(urakawa.core.TreeNode node) { return node is SectionNode && ((SectionNode)node).Level <= m_AudioFileSectionLevel; };
            m_SkipDelegate = delegate(urakawa.core.TreeNode node) { return !((ObiNode)node).Used; };
        }

        protected override void CreateOpfDocument()
        {
            // collect relevant metadata
            ObiPresentation presentation = (ObiPresentation)m_Presentation;

            urakawa.metadata.Metadata identifierMetadata = presentation.GetFirstMetadataItem("dc:Identifier");
            string bookIdentifier = identifierMetadata != null ? identifierMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata titleMetadata = presentation.GetFirstMetadataItem("dc:Title");
            string bookTitle = titleMetadata != null ? titleMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata publisherMetadata = presentation.GetFirstMetadataItem("dc:Publisher");
            string bookPublisher = publisherMetadata != null ? publisherMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata createrMetadata = presentation.GetFirstMetadataItem("dc:Creator");
            string bookAuthor = createrMetadata != null ? createrMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata narratorMetadata = presentation.GetFirstMetadataItem("dtb:narrator");
            string bookReadBy = narratorMetadata != null ? narratorMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata publishedDateMetadata = presentation.GetFirstMetadataItem("dc:Date");
            string bookPublishDate = publishedDateMetadata != null ? publishedDateMetadata.NameContentAttribute.Value : "";

            urakawa.metadata.Metadata languageMetadata = presentation.GetFirstMetadataItem("dc:Language");
            string bookLanguage = languageMetadata != null ? languageMetadata.NameContentAttribute.Value : "";

            JProperty context = new JProperty("@context", new JArray("https://schema.org", "https://www.w3.org/ns/pub-context"));
            JProperty accessMode = new JProperty("accessMode", new JArray("textual", "auditory"));
            JProperty accessibilityFeature = new JProperty("accessibilityFeature", new JArray("readingOrder", "tableOfContents"));
            JProperty accessibilitySummary = new JProperty("accessibilitySummary", "This is an audio book with table of contents.");

            JObject accessSufficientObject = new JObject( new JProperty("type", "ItemList"),
                new JProperty("itemListElement", new JArray("auditory")),
                new JProperty ("description", "All content in audio"));
            JProperty accessModeSufficient = new JProperty("accessModeSufficient", new JArray(accessSufficientObject));

            JObject audioBookObject = new JObject(context,
                new JProperty("conformsTo", "https://www.w3.org/TR/audiobooks"),
            new JProperty("type", "Audiobook"),
            new JProperty("id", bookIdentifier),
            //new JProperty("url", "https://w3c.github.io/wpub/experiments/audiobook/"),
            new JProperty("name", bookTitle),
            new JProperty("author", bookAuthor),
            new JProperty("readBy", bookReadBy),
            new JProperty("publisher", bookPublisher),
            new JProperty("inLanguage", bookLanguage),
            new JProperty("datePublished", bookPublishDate),
            new JProperty("duration", "PT" + m_BookDuration.ToString() + "S"),
            accessibilityFeature,
            accessibilitySummary,
            accessMode,
            accessModeSufficient);
            //new JProperty("license", "https://creativecommons.org/publicdomain/zero/1.0/")
            

            // resources
            JArray resourcesArray = new JArray();

            // create cover and add to resources
            XmlDocument coverDocument = CreateStub_NavigationDocument();
            XmlNode headNode = coverDocument.GetElementsByTagName("head")[0];
            XmlNode titleNode = coverDocument.CreateElement(null, "title", headNode.NamespaceURI);
            titleNode.AppendChild(coverDocument.CreateTextNode(bookTitle));
            headNode.AppendChild(titleNode);

            XmlNode bodyNode = coverDocument.GetElementsByTagName("body")[0];
            XmlNode h1Node = coverDocument.CreateElement(null, "h1", bodyNode.NamespaceURI);
            h1Node.AppendChild(coverDocument.CreateTextNode(bookTitle));
            bodyNode.AppendChild(h1Node);

            XmlNode h2Node = coverDocument.CreateElement(null,"h2", bodyNode.NamespaceURI);
            h2Node.AppendChild(coverDocument.CreateTextNode("By " + bookAuthor));
            bodyNode.AppendChild(h2Node);

            string coverFileName = "cover.html";
            XmlReaderWriterHelper.WriteXmlDocument(coverDocument, Path.Combine(m_OutputDirectory, coverFileName), AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null);

            // add the JObject to array of resources
            JObject coverObject = new JObject(new JProperty("rel", "cover"),
                new JProperty("url", coverFileName),
                new JProperty("encodingFormat", "text/html"),
                new JProperty("name", "Cover"));

            resourcesArray.Add(coverObject);

            JObject tocObject = new JObject(new JProperty("rel", "contents"),
                new JProperty("url", m_Filename_EntryFile),
                new JProperty("encodingFormat", "text/html"));

            resourcesArray.Add(tocObject);

            
            Console.WriteLine("Count of list: " + m_FilesList_SmilAudio.Count.ToString());
            
            audioBookObject.Add(new JProperty("resources", resourcesArray));
            audioBookObject.Add(new JProperty("readingOrder", M_PlaylistArray));

            // serialize to file
            string filePath = Path.Combine(m_OutputDirectory, m_Filename_Manifest);
            if (File.Exists(filePath)) File.Delete(filePath);
            FileStream fs = File.Open(filePath, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            JsonWriter jw = new JsonTextWriter(sw);
            jw.Formatting = Newtonsoft.Json.Formatting.Indented;
            JsonSerializer serial = new JsonSerializer();
            serial.Serialize(jw, audioBookObject);
            jw.Close();

        }

        protected override void CreateDTBookDocument()
        {
            CreateListOfLevelsForAudioNCX();
            
        }

        protected void CreateListOfLevelsForAudioNCX()
        {
            m_ListOfLevels = new List<TreeNode>();
            TreeNode rNode = m_Presentation.RootNode;
            //m_ListOfLevels.Add(rNode);
            //System.Windows.Forms.MessageBox.Show(rNode.ToString());
            rNode.AcceptDepthFirst(
                                delegate(TreeNode n)
                                {
                                    if (doesTreeNodeTriggerNewSmil(n)) m_ListOfLevels.Add(n);
                                    return true;
                                },
                                delegate(urakawa.core.TreeNode n) { });
        }

        protected override void CreateExternalFiles()
        {
            //base.CreateExternalFiles();
            //RemoveUnnecessaryExternalFiles();
        }

        private void RemoveUnnecessaryExternalFiles()
        {
            //foreach (string externalFileName in m_FilesList_ExternalFiles)
            
            for(int i=0 ; i< m_FilesList_ExternalFiles.Count; i++)
            {
                string externalFileName = m_FilesList_ExternalFiles[i];
string ext = Path.GetExtension(externalFileName);
if (urakawa.data.DataProviderFactory.CSS_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
    || urakawa.data.DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
    || urakawa.data.DataProviderFactory.DTD_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
{
    m_FilesList_ExternalFiles.Remove(externalFileName);
}
}
        }

        protected override void CreateNcxAndSmilDocuments()
        {
            XmlDocument navigationDocument = CreateStub_NavigationDocument();
            XmlNode headNode = navigationDocument.GetElementsByTagName("head")[0];

            //XmlNode scriptNode = navigationDocument.CreateElement(null, "script", headNode.NamespaceURI);
            //headNode.AppendChild(scriptNode);
            //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, scriptNode, "type", "application/ld+json");

            //JProperty context = new JProperty("@context", new JArray("https://schema.org", "https://www.w3.org/ns/wp-context"));
            //JObject audioBookObject = new JObject(context,
                            //new JProperty("conformsTo", "https://www.w3.org/TR/audiobooks/"),
                        //new JProperty("url", m_Filename_Manifest));
            //string strJObject = JsonConvert.SerializeObject(audioBookObject);
            //scriptNode.AppendChild(navigationDocument.CreateTextNode(strJObject));

            urakawa.metadata.Metadata titleMetadata = ((ObiPresentation)m_Presentation).GetFirstMetadataItem("dc:Title");
            string bookTitle = titleMetadata != null ? titleMetadata.NameContentAttribute.Value : "";

            XmlNode titleNode = navigationDocument.CreateElement("title", headNode.NamespaceURI);
            titleNode.AppendChild(navigationDocument.CreateTextNode(bookTitle));
            headNode.AppendChild(titleNode);

            XmlNode linkNode = navigationDocument.CreateElement("link", headNode.NamespaceURI);
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, linkNode, "rel", "publication");
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, linkNode, "href", m_Filename_Manifest);
            headNode.AppendChild(linkNode);

            // add TOC elements
            Dictionary<urakawa.core.TreeNode, XmlNode> headingNodeToXmlNodeMap = new Dictionary<TreeNode,XmlNode>();

            XmlNode bodyNode = navigationDocument.GetElementsByTagName("body")[0];
            XmlNode NavNode = navigationDocument.CreateElement("nav", bodyNode.NamespaceURI);
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, NavNode, "role", "doc-toc");
            bodyNode.AppendChild(NavNode);

            // add itle as first heading.
            XmlNode h1Node = navigationDocument.CreateElement(null, "h1", NavNode.NamespaceURI);
            h1Node.AppendChild(navigationDocument.CreateTextNode(bookTitle));
            NavNode.AppendChild(h1Node);

            XmlNode ulNode = navigationDocument.CreateElement("ul", bodyNode.NamespaceURI);
            NavNode.AppendChild(ulNode);
            
            m_FilesList_SmilAudio = new List<string>();
            urakawa.core.TreeNode parentTreeNode = null;

            Console.WriteLine(m_ListOfLevels.ToString());
            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
            {
                SectionNode section = null;
                urakawaNode.AcceptDepthFirst(
                delegate(urakawa.core.TreeNode n)
                {
                    if (n is SectionNode)
                    {
                        section = (SectionNode)n;
                    }
                        urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);
                        if (externalAudio != null)
                        {//0

                            string audioFileName = AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src);
                            Console.WriteLine("file name: " + audioFileName);
                            // add audio file name in audio files list for use in opf creation 
                            if (!m_FilesList_SmilAudio.Contains(audioFileName))
                            {//1
                                m_FilesList_SmilAudio.Add(audioFileName);

                                int timeDuration = (int)(section.Duration / 1000);
                                m_BookDuration += timeDuration;
                                M_PlaylistArray.Add(new JObject(new JProperty("url", audioFileName),
                    new JProperty("encodingFormat", "audio/mpeg"),
                    new JProperty("duration", "PT" + timeDuration + "S"),
                    new JProperty("name", section.Label)));

                                // add to HTML TOC
                                parentTreeNode = section.Parent;
                                XmlNode liNode = null;

                                if (parentTreeNode == section.Root)
                                {//2
                                    Console.WriteLine("if (n.Parent == n.Root ): " + section.Label);
                                    liNode = navigationDocument.CreateElement("li", bodyNode.NamespaceURI);
                                    ulNode.AppendChild(liNode);
                                }//-2
                                else if (headingNodeToXmlNodeMap.ContainsKey(parentTreeNode))
                                {//2
                                    Console.WriteLine("else if (headingNodeToXmlNodeMap.ContainsKey(section.Parent))" + section.Label);
                                    XmlNode liParent = headingNodeToXmlNodeMap[parentTreeNode];
                                    XmlNode ulChild = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(liParent, false, "ul", null);

                                    if (ulChild == null)
                                    {//3
                                        ulChild = navigationDocument.CreateElement(null, "ul", NavNode.NamespaceURI);
                                        liParent.AppendChild(ulChild);
                                    }//-3
                                    liNode = navigationDocument.CreateElement("li", bodyNode.NamespaceURI);
                                    ulChild.AppendChild(liNode);

                                }//-2
                                else
                                {//2
                                    // search up the node
                                    parentTreeNode = section;
                                    int counter = 0;
                                    while (parentTreeNode != null && counter <= 6)
                                    {   //3
                                        parentTreeNode = parentTreeNode.Parent;
                                        if (parentTreeNode != null && headingNodeToXmlNodeMap.ContainsKey(parentTreeNode))
                                        {//4
                                            XmlNode liParent = headingNodeToXmlNodeMap[parentTreeNode];
                                            XmlNode ulChild = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(liParent, false, "ul", null);
                                            if (ulChild == null)
                                            {
                                                ulChild = navigationDocument.CreateElement(null, "ul", NavNode.NamespaceURI);
                                                liParent.AppendChild(ulChild);
                                            }
                                            liNode = navigationDocument.CreateElement(null, "li", NavNode.NamespaceURI);
                                            ulChild.AppendChild(liNode);

                                            break;
                                        }//-4
                                        counter++;
                                    }//-3

                                    if (parentTreeNode == null || counter > 7)
                                    {//3
                                        liNode = navigationDocument.CreateElement(null, "li", NavNode.NamespaceURI);
                                        ulNode.AppendChild(liNode);

                                    }//-3

                                        

                                }//-2
                                headingNodeToXmlNodeMap.Add(section, liNode);
                                XmlNode anchorNode = navigationDocument.CreateElement(null, "a", NavNode.NamespaceURI);
                                anchorNode.AppendChild(navigationDocument.CreateTextNode(section.Label));
                                XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, anchorNode, "href", audioFileName);
                                liNode.AppendChild(anchorNode);
                            }//-1
                        }//-0
                        return true;
                    
                },
                delegate(urakawa.core.TreeNode n) { });
            }

            XmlReaderWriterHelper.WriteXmlDocument(navigationDocument, Path.Combine(m_OutputDirectory, m_Filename_EntryFile), AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null);
        }

        protected XmlDocument CreateStub_NavigationDocument()
        {
            XmlDocument navigationDocument = new XmlDocument();
            navigationDocument.XmlResolver = null;

            navigationDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            //navigationDocument.AppendChild(navigationDocument.CreateDocumentType("html",
            //"-//W3C//DTD XHTML 1.0 Transitional//EN",
            //"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
            //null));

            XmlNode rootNode = navigationDocument.CreateElement(null,
                "html",
                "http://www.w3.org/1999/xhtml");

            navigationDocument.AppendChild(rootNode);

            //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, rootNode, "xmlns:epub", "http://www.idpf.org/2007/ops");
            //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, rootNode, "version", "2005-1");
            //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, rootNode,
            //XmlReaderWriterHelper.XmlLang,
            //(string.IsNullOrEmpty(m_Presentation.Language)
            //? "en-US"
            //: m_Presentation.Language));


            XmlNode headNode = navigationDocument.CreateElement(null, "head", rootNode.NamespaceURI);
            rootNode.AppendChild(headNode);

            XmlNode bodyNode = navigationDocument.CreateElement(null, "body", rootNode.NamespaceURI);
            rootNode.AppendChild(bodyNode);

            return navigationDocument;
        }
        void CreateNcxAndSmilDocuments_old()
        {
            XmlDocument ncxDocument = CreateStub_NcxDocument();

            XmlNode ncxRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "ncx", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "navMap", null); //ncxDocument.GetElementsByTagName("navMap")[0];
            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_SmilAudio = new List<string>();
            m_SmilFileNameCounter = 0;
            List<XmlNode> playOrderList_Sorted = new List<XmlNode>();
            int totalPageCount = 0;
            int maxNormalPageNumber = 0;
            int maxDepth = 1;
            Time smilElapseTime = new Time();
            List<string> ncxCustomTestList = new List<string>();
            List<urakawa.core.TreeNode> specialParentNodesAddedToNavList = new List<urakawa.core.TreeNode>();
            bool isDocTitleAdded = false;

            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);

            ////System.Windows.Forms.MessageBox.Show(m_ListOfLevels.Count.ToString());
            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
            //for ( int nodeCounter = 0 ; nodeCounter < m_ListOfLevels.Count ; nodeCounter++ )
            {
                //urakawa.core.TreeNode urakawaNode  = m_ListOfLevels[nodeCounter] ;

                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                string smilFileName = null;
                XmlNode navPointNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                urakawa.core.TreeNode special_UrakawaNode = null;
                Time durationOfCurrentSmil = new Time();
                XmlNode mainSeq = null;
                XmlNode Seq_SpecialNode = null;
                //bool IsPageAdded = false;
                string firstPar_id = null;
                bool shouldAddNewSeq = false;
                string par_id = null;
                List<string> currentSmilCustomTestList = new List<string>();
                Stack<urakawa.core.TreeNode> specialParentNodeStack = new Stack<urakawa.core.TreeNode>();
                Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();
                SectionNode section = (SectionNode)urakawaNode;

                bool isBranchingActive = false;
                urakawa.core.TreeNode branchStartTreeNode = null;
                ////System.Windows.Forms.MessageBox.Show(urakawaNode.GetTextFlattened(false));
                urakawaNode.AcceptDepthFirst(
            delegate(urakawa.core.TreeNode n)
            {

                if (RequestCancellation) return false;
                
                //QualifiedName currentQName = n.GetXmlElementQName();

                //if (IsHeadingNode(n))
                //if (n is EmptyNode &&  ((EmptyNode)n).Role_ == EmptyNode.Role.Heading )
                //{
                    //currentHeadingTreeNode = n;
                //}

                //if (currentQName != null &&
                        //currentQName.LocalName != urakawaNode.GetXmlElementQName().LocalName
                        //&& doesTreeNodeTriggerNewSmil(n))
                if ( n is SectionNode 
                    && (!m_ListOfLevels.Contains(n)  || m_ListOfLevels.IndexOf(n) > m_ListOfLevels.IndexOf(urakawaNode)) )
                    {

                        return false;
                    }
                    urakawa.media.ExternalAudioMedia externalAudio = GetExternalAudioMedia(n);
                    if (externalAudio == null
                        || (n is EmptyNode
                        && (((EmptyNode)n).Role_ == EmptyNode.Role.Silence || !((EmptyNode)n).Used)))
                    {
                        return true;
                    }

                //if ((IsHeadingNode(n) || IsEscapableNode(n) || IsSkippableNode(n))
                    if (n is EmptyNode && (((EmptyNode)n).Role_ == EmptyNode.Role.Heading || ((EmptyNode)n).Role_ == EmptyNode.Role.Page || ((EmptyNode)n).Role_ == EmptyNode.Role.Anchor || IsSkippable((EmptyNode)n) )
                    && (special_UrakawaNode != n))
                {
                    // if this candidate special node is child of existing special node then ad existing special node to stack for nesting.
                    if (special_UrakawaNode != null && Seq_SpecialNode != null
                        && n.IsDescendantOf(special_UrakawaNode))
                    {
                        //specialParentNodeStack.Push(special_UrakawaNode);
                        specialSeqNodeStack.Push(Seq_SpecialNode);
                    }
                    special_UrakawaNode = n;
                    shouldAddNewSeq = true;
                }

                Time urakawaNodeDur = urakawaNode.GetDurationOfManagedAudioMediaFlattened();
                //if (currentHeadingTreeNode == null && urakawaNodeDur != null && urakawaNodeDur.AsTimeSpan == TimeSpan.Zero)
                if (urakawaNodeDur != null &&
                    urakawaNodeDur.IsEqualTo(Time.Zero)
                    || externalAudio == null)
                {
                    return true;
                    // carry on processing following lines. and in case this is not true, skip all the following lines
                }


                //QualifiedName qName = currentHeadingTreeNode != null ? currentHeadingTreeNode.GetXmlElementQName() : null;
                //bool isDoctitle_ = (qName != null && qName.LocalName == "doctitle");

                // create smil stub document
                if (smilDocument == null)
                {
                    smilDocument = CreateStub_SmilDocument();
                    mainSeq = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeq, "id", GetNextID(ID_SmilPrefix));
                    smilFileName = GetNextSmilFileName;
                    //m_ProgressPercentage += Convert.ToInt32((m_SmilFileNameCounter / m_ListOfLevels.Count) * 100 * 0.7);
                    //reportProgress(m_ProgressPercentage, String.Format(UrakawaSDK_daisy_Lang.CreatingSmilFiles, m_SmilFileNameCounter, m_ListOfLevels.Count));
                }
                

                // create smil nodes

                if (shouldAddNewSeq)
                {
                    if (Seq_SpecialNode != null)
                    {
                        if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                        {
                            Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                        }
                        Seq_SpecialNode = null;
                    }

                    Seq_SpecialNode = smilDocument.CreateElement(null, "seq", mainSeq.NamespaceURI);
                    string strSeqID = "";
                    // specific handling of IDs for notes for allowing predetermined refered IDs

                    string specialLocalName = special_UrakawaNode.HasXmlProperty
                                                  ? special_UrakawaNode.GetXmlElementLocalName()
                                                  : null;

                    if (specialLocalName == "note"
                        || specialLocalName == "annotation")
                    {
                        strSeqID = ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value;
                    }
                    else
                    {
                        strSeqID = GetNextID(ID_SmilPrefix);
                    }
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "id", strSeqID);
                    //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", special_UrakawaNode.GetXmlElementQName().LocalName);
                    if (n is EmptyNode &&  ((EmptyNode)n).Role_ == EmptyNode.Role.Page )  XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", "pagenum");
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Custom && special_UrakawaNode == n)
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", ((EmptyNode)n).CustomRole);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", ((EmptyNode)n).CustomRole);
                        if (!currentSmilCustomTestList.Contains(((EmptyNode)n).CustomRole)) currentSmilCustomTestList.Add(((EmptyNode)n).CustomRole);
                        
                    }
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Anchor && ((EmptyNode)n).AssociatedNode != null)
                    {
                        if (IsAnnoref((EmptyNode)n))
                        {
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", "annoref");
                            if (!currentSmilCustomTestList.Contains("annoref")) currentSmilCustomTestList.Add("annoref");
                                                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", "annoref");
                        }
                        else
                        {
                            
                            XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", "noteref");
                            if (!currentSmilCustomTestList.Contains("noteref")) currentSmilCustomTestList.Add("noteref");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", "noteref");
                        }
                    }
                    
                    if (((EmptyNode)special_UrakawaNode).Role_ == EmptyNode.Role.Anchor && ((EmptyNode)special_UrakawaNode).AssociatedNode != null)
                    {
                        m_AnchorXmlNodeToReferedNodeMap.Add(Seq_SpecialNode, ((EmptyNode)special_UrakawaNode).AssociatedNode);
                        XmlNode anchorNode = smilDocument.CreateElement(null, "a", Seq_SpecialNode.NamespaceURI);
                        Seq_SpecialNode.AppendChild(anchorNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "");
                        if (m_Skippable_UpstreamIdMap.Count > 0 && m_Skippable_UpstreamIdMap.ContainsKey(((EmptyNode) special_UrakawaNode).AssociatedNode))
                        {
                            anchorNode.Attributes.GetNamedItem("href").Value = m_Skippable_UpstreamIdMap[((EmptyNode) special_UrakawaNode).AssociatedNode];
                        }
                        if (!m_AnchorSmilDoc_SmileFileNameMap.ContainsKey(smilDocument))  m_AnchorSmilDoc_SmileFileNameMap.Add(smilDocument, smilFileName);
                        //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "#" + ID_SmilPrefix + m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("idref").Value.Replace("#", ""));

                        //isBranchingActive = true;

                        //branchStartTreeNode = GetReferedTreeNode(special_UrakawaNode);

                    }
                    if (IsSkippable((EmptyNode)n))
                    {
                        
                        bool foundAnchor = false;
                        foreach (XmlNode xn in m_AnchorXmlNodeToReferedNodeMap.Keys)
                        {
                            if (m_AnchorXmlNodeToReferedNodeMap[xn] == n)
                            {
                                
                                XmlNode anchorNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xn, true, "a", xn.NamespaceURI); 
                                //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", smilFileName + "#" + strSeqID);
                                //anchorNode.Attributes.GetNamedItem("href").Value= smilFileName + "#" + strSeqID;

                                anchorNode.Attributes.GetNamedItem("href").Value =smilFileName + "#" + strSeqID;
                                foundAnchor = true;
                                //break;
                            }
                        }
                        if ( !foundAnchor ) m_Skippable_UpstreamIdMap.Add ( n, smilFileName + "#" + strSeqID);
                    }

                    if ( n is EmptyNode &&  ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", special_UrakawaNode.GetXmlElementQName().LocalName);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", "pagenum");

                        

                        if (!currentSmilCustomTestList.Contains("pagenum"))
                        {
                            currentSmilCustomTestList.Add("pagenum");
                        }
                    }

                    // add smilref reference to seq_special  in dtbook document
                    //if (!IsAudioNCX
                        //&& (IsEscapableNode(special_UrakawaNode) || IsSkippableNode(special_UrakawaNode)))
                    //{
                        //XmlNode dtbEscapableNode = m_TreeNode_XmlNodeMap[special_UrakawaNode];
                        //XmlDocumentHelper.CreateAppendXmlAttribute(m_DTBDocument, dtbEscapableNode, "smilref", smilFileName + "#" + strSeqID);
                    //}

                    // decide the parent node to which this new seq node is to be appended.
                    if (specialSeqNodeStack.Count == 0)
                    {
                        mainSeq.AppendChild(Seq_SpecialNode);
                    }
                    else
                    {
                        specialSeqNodeStack.Peek().AppendChild(Seq_SpecialNode);
                    }

                    shouldAddNewSeq = false;
                }

                XmlNode parNode = smilDocument.CreateElement(null, "par", mainSeq.NamespaceURI);

                // decide the parent node for this new par node.
                // if node n is child of current specialParentNode than append to it 
                //else check if it has to be appended to parent of this special node in stack or to main seq.
                if (special_UrakawaNode != null && (special_UrakawaNode == n || (((EmptyNode)n).Role_ == EmptyNode.Role.Custom && ((EmptyNode)n).CustomRole == ((EmptyNode)special_UrakawaNode).CustomRole)))
                {
                    Seq_SpecialNode.AppendChild(parNode);
                }
                else
                {
                    bool IsParNodeAppended = false;
                    string strReferedID = par_id;
                    if (specialParentNodeStack.Count > 0)
                    {
                        // check and pop stack till specialParentNode of   iterating node n is found in stack
                        // the loop is also used to assign value of last imidiate seq or par to end attribute of parent seq while pop up
                        while (specialParentNodeStack.Count > 0 && !n.IsDescendantOf(special_UrakawaNode))
                        {
                            if (Seq_SpecialNode != null
                                            &&
                                            strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID + ".end";
                            }
                            strReferedID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                            special_UrakawaNode = specialParentNodeStack.Pop();
                            Seq_SpecialNode = specialSeqNodeStack.Pop();
                        }

                        // if parent of node n is retrieved from stack, apend the par node to it.
                        if (n.IsDescendantOf(special_UrakawaNode))
                        {
                            Seq_SpecialNode.AppendChild(parNode);
                            IsParNodeAppended = true;
                            
                        }
                        //System.Windows.Forms.MessageBox.Show ( "par_ id " + par_id + " count " + specialParentNodeStack.Count.ToString ());
                    }// stack > 0 check ends

                    if (specialSeqNodeStack.Count == 0 && !IsParNodeAppended)
                    {
                        mainSeq.AppendChild(parNode);
                        special_UrakawaNode = null;
                        if (Seq_SpecialNode != null)
                        {
                            //if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem ( "end" ) != null)
                            if (strReferedID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                            {
                                //System.Windows.Forms.MessageBox.Show ( par_id == null ? "null" : par_id );
                                //Seq_SpecialNode.Attributes.GetNamedItem ( "end" ).Value = "DTBuserEscape;" + par_id + ".end";
                                Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + strReferedID + ".end";
                            }
                            Seq_SpecialNode = null;
                        }
                    }// check of append to main seq ends

                }


                par_id = GetNextID(ID_SmilPrefix);
                // check and assign first par ID
                /* commented, it is assigned only if external audio media is not null as this is audio NCX book
                if (firstPar_id == null)
                {
                    //if (currentHeadingTreeNode != null
                        //&& (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                    if ( (section.Heading == null && n.Parent == section && !IsNcxNativeNodeAdded)
                        || ( n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Heading ))
                    {
                        firstPar_id = par_id;
                    }
                }
                */
                XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, parNode, "id", par_id);


                XmlNode SmilTextNode = null;
                //not required in audio ncx book
                /*
                if ((section.Heading == null && n == section.PhraseChild(0))
                    || (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Heading))
                {
                    SmilTextNode = smilDocument.CreateElement(null, "text", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "id", GetNextID(ID_SmilPrefix));
                    //string dtbookID = m_TreeNode_XmlNodeMap[n].Attributes != null ? m_TreeNode_XmlNodeMap[n].Attributes.GetNamedItem("id").Value : m_TreeNode_XmlNodeMap[n.Parent].Attributes.GetNamedItem("id").Value;
                    //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src", m_Filename_Content + "#" + dtbookID);
                    parNode.AppendChild(SmilTextNode);
                }
                 */ 
                 
                if (externalAudio != null)
                {
                    // assign the ID of first created par in smil document as firstPar_id so that its entry can go in content node in ncx.
                    if (firstPar_id == null) firstPar_id = par_id;

                    string audioFileName = AddSectionNameToAudioFile? AddSectionNameToAudioFileName(externalAudio.Src,section.Label): Path.GetFileName(externalAudio.Src);
                    XmlNode audioNode = smilDocument.CreateElement(null, "audio", mainSeq.NamespaceURI);
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, audioNode, "src", audioFileName);
                    parNode.AppendChild(audioNode);

                    // add audio file name in audio files list for use in opf creation 

                    if (!m_FilesList_SmilAudio.Contains(audioFileName)) m_FilesList_SmilAudio.Add(audioFileName);

                    // add to duration 
                    durationOfCurrentSmil.Add(externalAudio.Duration);
                }

                // if node n is pagenum, add to pageList
                if (n is EmptyNode &&  ((EmptyNode)n).Role_ == EmptyNode.Role.Page )
                {
                    if (!currentSmilCustomTestList.Contains("pagenum"))
                    {
                        currentSmilCustomTestList.Add("pagenum");
                    }

                    XmlNode pageListNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(ncxDocument, true, "pageList", null);
                    if (pageListNode == null)
                    {
                        pageListNode = ncxDocument.CreateElement(null, "pageList", ncxRootNode.NamespaceURI);
                        
                        ncxRootNode.InsertAfter(pageListNode, navMapNode);
                    }

                    XmlNode pageTargetNode = ncxDocument.CreateElement(null, "pageTarget", pageListNode.NamespaceURI);
                    pageListNode.AppendChild(pageTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "class", "pagenum");
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "playOrder", "");
                    string strTypeVal = ((EmptyNode)n).PageNumber.Kind.ToString ().ToLower();
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "type", strTypeVal);
                    //string strPageValue = n.GetXmlProperty().GetAttribute("pageText").Value ;
                    string strPageValue = ((EmptyNode)n).PageNumber.Number.ToString();
                    ++totalPageCount;

                    playOrderList_Sorted.Add(pageTargetNode);

                    if (strTypeVal == "normal" || strTypeVal == "front")
                    {
                        int tmp;
                        bool success = int.TryParse(strPageValue, out tmp);
                        if (success && maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
                    }
                    if (strTypeVal != "special")
                    {
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, pageTargetNode, "value", strPageValue);
                    }

                    XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", pageListNode.NamespaceURI);
                    pageTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = ncxDocument.CreateElement(null, "text", pageListNode.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(
                        ncxDocument.CreateTextNode(((EmptyNode)n).PageNumber.Unquoted));

                    if (externalAudio != null)
                    {
                        XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", pageListNode.NamespaceURI);
                        navLabelNode.AppendChild(audioNodeNcx);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", 
                            AddSectionNameToAudioFile? AddSectionNameToAudioFileName(externalAudio.Src,section.Label): Path.GetFileName(externalAudio.Src));
                    }

                    XmlNode contentNode = ncxDocument.CreateElement(null, "content", pageListNode.NamespaceURI);
                    pageTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);
                    ////System.Windows.Forms.MessageBox.Show("Page ");
                    // add reference to par in dtbook document
                    
                }
                    //obi: commented for now
                
            else if (special_UrakawaNode != null && n == special_UrakawaNode
                && n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Custom )
                    //&&  m_NavListElementNamesList.Contains(((EmptyNode)n).CustomRole) && !specialParentNodesAddedToNavList.Contains(special_UrakawaNode))
            {
                EmptyNode eSpecialNode = (EmptyNode)special_UrakawaNode;
                string navListNodeName = eSpecialNode.CustomRole;
                specialParentNodesAddedToNavList.Add(special_UrakawaNode);
                XmlNode navListNode = null;

                //= getFirstChildElementsWithName ( ncxDocument, true, "navList", null );
                foreach (XmlNode xn in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(ncxRootNode, true, "navList", ncxRootNode.NamespaceURI, true))
                {
                    if (xn.Attributes.GetNamedItem("class").Value == navListNodeName)
                    {
                        navListNode = xn;
                    }
                }

                if (navListNode == null)
                {
                    navListNode = ncxDocument.CreateElement(null, "navList", ncxRootNode.NamespaceURI);
                    ncxRootNode.AppendChild(navListNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navListNode, "class", navListNodeName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navListNode, "id", GetNextID(ID_NcxPrefix));

                    XmlNode mainNavLabel = ncxDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                    navListNode.AppendChild(mainNavLabel);
                    XmlNode mainTextNode = ncxDocument.CreateElement(null, "text", navListNode.NamespaceURI);
                    mainNavLabel.AppendChild(mainTextNode);
                    mainTextNode.AppendChild(ncxDocument.CreateTextNode(navListNodeName));
                }

                XmlNode navTargetNode = ncxDocument.CreateElement(null, "navTarget", navListNode.NamespaceURI);
                navListNode.AppendChild(navTargetNode);

                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "id", GetNextID(ID_NcxPrefix));
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "class", navListNodeName);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navTargetNode, "playOrder", "");

                playOrderList_Sorted.Add(navTargetNode);


                XmlNode navLabelNode = ncxDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                navTargetNode.AppendChild(navLabelNode);

                XmlNode txtNode = ncxDocument.CreateElement(null, "text", navTargetNode.NamespaceURI);
                navLabelNode.AppendChild(txtNode);
                txtNode.AppendChild(
                    ncxDocument.CreateTextNode(((EmptyNode)n).CustomRole ));

                // create audio node only if external audio media is not null
                if (externalAudio != null)
                {
                    XmlNode audioNodeNcx = ncxDocument.CreateElement(null, "audio", navTargetNode.NamespaceURI);
                    navLabelNode.AppendChild(audioNodeNcx);
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                    XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNodeNcx, "src", 
                        AddSectionNameToAudioFile? AddSectionNameToAudioFileName(externalAudio.Src,section.Label): Path.GetFileName(externalAudio.Src));
                }

                XmlNode contentNode = ncxDocument.CreateElement(null, "content", navTargetNode.NamespaceURI);
                navTargetNode.AppendChild(contentNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + par_id);
            
            }
            
                if (!IsNcxNativeNodeAdded)
                {
                    if (!isDocTitleAdded)
                    {
                        if (urakawaNode == m_ListOfLevels[0] 
                            &&  ((section.Heading == null && n is EmptyNode) || section.Heading == n) )
                        {
                            string txtMedia = urakawaNode.GetTextFlattened();
                             externalAudio = GetExternalAudioMedia(n);

                            XmlNode docNode = ncxDocument.CreateElement(null,
                                "docTitle",
                                 ncxRootNode.NamespaceURI);

                            //XmlNode navMapNode = XmlDocumentHelper.GetFirstChildElementWithName(ncxDocument, true, "navMap", null);

                            ncxRootNode.InsertBefore(docNode, navMapNode);

                            XmlNode docTxtNode = ncxDocument.CreateElement(null, "text", docNode.NamespaceURI);
                            docNode.AppendChild(docTxtNode);
                            docTxtNode.AppendChild(
                            ncxDocument.CreateTextNode(txtMedia));

                            if (externalAudio != null)
                            {
                                // create audio node
                                XmlNode docAudioNode = ncxDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                                docNode.AppendChild(docAudioNode);
                                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                                XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, docAudioNode, "src", 
                                    AddSectionNameToAudioFile? AddSectionNameToAudioFileName(externalAudio.Src,section.Label): Path.GetFileName(externalAudio.Src));
                            }


                            isDocTitleAdded = true;
                            //IsNcxNativeNodeAdded = true;
                        }
                    }

                    if ( (urakawaNode is SectionNode &&  section.Heading == null && n is EmptyNode ) || 
                        ( n is EmptyNode &&  ((EmptyNode)n).Role_ == EmptyNode.Role.Heading ) )
                    {
                        /*
                        // find node for heading


                        int indexOf_n = 0;

                        if (n.GetXmlElementQName() != null && (n.IsDescendantOf(currentHeadingTreeNode) || n == currentHeadingTreeNode))
                        {
                            //indexOf_n = audioNodeIndex;
                            //indexOf_n = 0;
                        }
                        else
                        {
                            return true;
                        }
                        */

                        string txtMedia = urakawaNode.GetTextFlattened();
                        ////System.Windows.Forms.MessageBox.Show("nav point " + txtMedia);
                        externalAudio = GetExternalAudioMedia(n);

                        // first create navPoints
                        navPointNode = ncxDocument.CreateElement(null, "navPoint", navMapNode.NamespaceURI);
                        if (currentHeadingTreeNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "class", "heading");
                        string strNavPointID = GetNextID(ID_NcxPrefix) ;
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "id",strNavPointID );
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, navPointNode, "playOrder", "");


                        //add the navpoint id to smil text in par of corresponding smil file
                        if ( SmilTextNode != null)  XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src", m_Filename_Ncx+ "#" + strNavPointID);
                        //urakawa.core.TreeNode parentNode = GetParentLevelNode(urakawaNode);
                        urakawa.core.TreeNode parentNode = urakawaNode.Parent;

                        if (parentNode == null || parentNode == m_Presentation.RootNode)
                        {
                            navMapNode.AppendChild(navPointNode);
                        }
                        else if (treeNode_NavNodeMap.ContainsKey(parentNode))
                        {
                            treeNode_NavNodeMap[parentNode].AppendChild(navPointNode);
                        }
                        else // surch up for node
                        {
                            int counter = 0;
                            parentNode = urakawaNode;
                            while (parentNode != null && counter <= 6)
                            {
                                //parentNode = GetParentLevelNode(parentNode);
                                parentNode = parentNode.Parent;
                                if (parentNode != null && treeNode_NavNodeMap.ContainsKey(parentNode))
                                {
                                    treeNode_NavNodeMap[parentNode].AppendChild(navPointNode);
                                    break;
                                }
                                counter++;
                            }

                            if (parentNode == null || counter > 7)
                            {
                                navMapNode.AppendChild(navPointNode);
                            }
                        }


                        treeNode_NavNodeMap.Add(urakawaNode, navPointNode);

                        // create navLabel
                        XmlNode navLabel = ncxDocument.CreateElement(null, "navLabel", navPointNode.NamespaceURI);
                        navPointNode.AppendChild(navLabel);

                        // create text node
                        XmlNode txtNode = ncxDocument.CreateElement(null, "text", navMapNode.NamespaceURI);
                        navLabel.AppendChild(txtNode);
                        //if (currentHeadingTreeNode != null)
                            txtNode.AppendChild(
                            ncxDocument.CreateTextNode(txtMedia));
                        
                        // create audio node
                        XmlNode audioNode = ncxDocument.CreateElement(null, "audio", navMapNode.NamespaceURI);
                        navLabel.AppendChild(audioNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, audioNode, "src", 
                            AddSectionNameToAudioFile?AddSectionNameToAudioFileName(externalAudio.Src,section.Label): Path.GetFileName(externalAudio.Src));
                        
                        //following was old code
                        //navPointNode = CreateNavPointWithoutContentNode(ncxDocument, urakawaNode, currentHeadingTreeNode, n, treeNode_NavNodeMap);
                        playOrderList_Sorted.Add(navPointNode);

                        // add content node
                        if (firstPar_id != null)
                        {
                            XmlNode contentNode = ncxDocument.CreateElement(null, "content", navMapNode.NamespaceURI);
                            navPointNode.AppendChild(contentNode);
                            XmlDocumentHelper.CreateAppendXmlAttribute(ncxDocument, contentNode, "src", smilFileName + "#" + firstPar_id);
                            ////System.Windows.Forms.MessageBox.Show("Navpoint");
                        }
                        int navPointDepth = GetDepthOfNavPointNode(ncxDocument, navPointNode);
                        if (maxDepth < navPointDepth) maxDepth = navPointDepth;

                        IsNcxNativeNodeAdded = true;
                    }
                }

                if (isBranchingActive)
                {
                    //IsBranchAssigned = true;

                    //durationOfCurrentSmil.Add(CreateFollowupNoteAndAnnotationNodes(smilDocument, mainSeq, branchStartTreeNode, smilFileName, currentSmilCustomTestList, ncxCustomTestList));

                    isBranchingActive = false;

                }
                string localName = n.HasXmlProperty ? n.GetXmlElementLocalName() : null;
                string localNameSpecial = (special_UrakawaNode != null && special_UrakawaNode.HasXmlProperty) ? special_UrakawaNode.GetXmlElementLocalName() : null;

                if (localName == "sent"
                        && localNameSpecial == "note"
                        || localNameSpecial == "annotation")
                {

                    return false;
                }

                return true;
            },
                    delegate(urakawa.core.TreeNode n) { });

                // make specials to null
                special_UrakawaNode = null;
                if (Seq_SpecialNode != null)
                {
                    if (par_id != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                    {
                        Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + par_id + ".end";
                    }
                }
                while (specialSeqNodeStack.Count > 0)
                {
                    string str_RefferedSeqID = Seq_SpecialNode.Attributes.GetNamedItem("id").Value;
                    Seq_SpecialNode = specialSeqNodeStack.Pop();
                    if (str_RefferedSeqID != null && Seq_SpecialNode.Attributes.GetNamedItem("end") != null)
                        Seq_SpecialNode.Attributes.GetNamedItem("end").Value = "DTBuserEscape;" + str_RefferedSeqID + ".end";
                    //System.Windows.Forms.MessageBox.Show ( "last " + smilFileName + " id " + par_id);
                }
                Seq_SpecialNode = null;

                if (RequestCancellation) return;
                // add metadata to smil document and write to file.
                if (smilDocument != null)
                {
                    // update duration in seq node
                    XmlNode mainSeqNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild; //smilDocument.GetElementsByTagName("body")[0].FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "dur", FormatTimeString(durationOfCurrentSmil));
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeqNode, "fill", "remove");
                    AddMetadata_Smil(smilDocument, FormatTimeString(smilElapseTime), currentSmilCustomTestList);
                    
                    XmlReaderWriterHelper.WriteXmlDocument(smilDocument, Path.Combine(m_OutputDirectory, smilFileName),AlwaysIgnoreIndentation? GetXmlWriterSettings (false): null);

                    smilElapseTime.Add(durationOfCurrentSmil);
                    m_FilesList_Smil.Add(smilFileName);
                    smilDocument = null;

                    // add smil custon test list items to ncx custom test list
                    foreach (string customTestName in currentSmilCustomTestList)
                    {
                        
                        if (!ncxCustomTestList.Contains(customTestName))
                            ncxCustomTestList.Add(customTestName);
                    }

                }

            }

            // assign play orders 
            Dictionary<string, string> playOrder_ReferenceMap = new Dictionary<string, string>();
            int playOrderCounter = 1;
            ////System.Windows.Forms.MessageBox.Show(playOrderList_Sorted.Count.ToString() );
            foreach (XmlNode xn in playOrderList_Sorted)
            {
                XmlNode referedContentNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xn, false, "content", xn.NamespaceURI);

                string contentNode_Src = referedContentNode.Attributes.GetNamedItem("src").Value;
                
                if (playOrder_ReferenceMap.ContainsKey(contentNode_Src))
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = playOrder_ReferenceMap[contentNode_Src];
                }
                else
                {
                    xn.Attributes.GetNamedItem("playOrder").Value = playOrderCounter.ToString();
                    playOrder_ReferenceMap.Add(contentNode_Src, playOrderCounter.ToString());
                    ++playOrderCounter;
                    //System.Windows.Forms.MessageBox.Show ( contentNode_Src );
                }
            }
            // rewrite the smil files that has anchor references
            foreach (XmlDocument sd in m_AnchorSmilDoc_SmileFileNameMap.Keys)
            {
                XmlReaderWriterHelper.WriteXmlDocument(sd, Path.Combine(m_OutputDirectory, m_AnchorSmilDoc_SmileFileNameMap[sd]), 
                    (AlwaysIgnoreIndentation ? GetXmlWriterSettings(false): null ));
            }
            m_AnchorSmilDoc_SmileFileNameMap = null;
            if (RequestCancellation)
            {
                //m_DTBDocument = null;
                ncxDocument = null;
                return;
            }
            //SaveXukAction.WriteXmlDocument(m_DTBDocument, Path.Combine(m_OutputDirectory, m_Filename_Content));

            if (RequestCancellation)
            {
                //m_DTBDocument = null;
                ncxDocument = null;
                return;
            }
            // write ncs document to file
            m_TotalTime = new Time(smilElapseTime);
            AddMetadata_Ncx(ncxDocument, totalPageCount.ToString(), maxNormalPageNumber.ToString(), maxDepth.ToString(), ncxCustomTestList);
            XmlReaderWriterHelper.WriteXmlDocument(ncxDocument, Path.Combine(m_OutputDirectory, m_Filename_Ncx),AlwaysIgnoreIndentation? GetXmlWriterSettings(false): null);
        }

        public bool IsSkippable(EmptyNode node)
        {

            if (node.Role_ == EmptyNode.Role.Custom && (node.PrecedingNode == null || node.PrecedingNode is SectionNode || node.Role_ != ((EmptyNode)node.PrecedingNode).Role_ || node.CustomRole != ((EmptyNode)node.PrecedingNode).CustomRole)
                && (node.CustomRole == EmptyNode.Annotation
                || node.CustomRole == EmptyNode.EndNote
                || node.CustomRole == EmptyNode.Footnote
                || node.CustomRole == EmptyNode.Note
                || node.CustomRole == EmptyNode.Sidebar
                || node.CustomRole == EmptyNode.ProducerNote))
            {
                return true;
            }
            return false;
        }

        protected bool IsAnnoref(EmptyNode node)
        {
            if (node.Role_ == EmptyNode.Role.Anchor
                && node.AssociatedNode != null
                && node.AssociatedNode.CustomRole == EmptyNode.Annotation)
            {
                return true;
            }
            return false;
        }

        public XmlWriterSettings GetXmlWriterSettings (bool indentation)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = Encoding.UTF8;
            if (indentation)
            {
                settings.Indent = true;
                settings.IndentChars = "\t";
            }
            else
            {
                settings.Indent = false;
            }
            return settings;            
        }



    }
}
