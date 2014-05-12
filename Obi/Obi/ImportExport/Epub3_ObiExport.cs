using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using AudioLib;
using urakawa.ExternalFiles;
using urakawa.core;
using urakawa.daisy;
using urakawa.daisy.export.visitor;
using urakawa.daisy.import;
using urakawa.data;
using urakawa.media;
using urakawa.media.data;
using urakawa.media.data.audio;
using urakawa.media.data.image;
using urakawa.media.data.video;
using urakawa.media.timing;
using urakawa.metadata;
using urakawa.metadata.daisy;
using urakawa.property.alt;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.xuk;
using XmlAttribute = urakawa.property.xml.XmlAttribute;

namespace Obi.ImportExport
{

    public class Epub3_ObiExport : DAISY3_ObiExport 
    {
        private List<string> m_FilesList_Html = null;
        private List<string> m_SmilDurationForOpfMetadata = null;
        private readonly string m_OutputDirectoryName = null;
        private string m_EpubParentDirectoryPath = null;
        private readonly string m_Meta_infFileName = null;
        public const string NS_URL_EPUB = "http://www.idpf.org/2007/ops";

        public Epub3_ObiExport(ObiPresentation presentation, string exportDirectory, List<string> navListElementNamesList, bool encodeToMp3,ushort mp3BitRate ,
            SampleRate sampleRate, bool stereo, bool skipACM, int audioFileSectionLevel):
            base (presentation, exportDirectory, navListElementNamesList, encodeToMp3,mp3BitRate ,
            sampleRate, stereo, skipACM, audioFileSectionLevel)
        {
            m_EpubParentDirectoryPath = Path.Combine(m_OutputDirectory, presentation.Title.Substring (0, presentation.Title.Length > 8? 8: presentation.Title.Length )) ;
            m_OutputDirectoryName = "EPUB";
            m_OutputDirectory = Path.Combine(m_EpubParentDirectoryPath, m_OutputDirectoryName);
            if (!Directory.Exists(m_OutputDirectory))
            {
                Directory.CreateDirectory(m_OutputDirectory);
            }
              m_Meta_infFileName = "META-INF" ;  
        }
        
        protected override void CreateNcxAndSmilDocuments()
        {
            

                        XmlDocument navigationDocument = CreateStub_NavigationDocument();

            XmlNode navigationDocRootNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navigationDocument, true, "body", null); //ncxDocument.GetElementsByTagName("ncx")[0];
            XmlNode navNode = navigationDocument.CreateElement("nav", navigationDocRootNode.NamespaceURI);
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navNode, "epub:type", "toc", NS_URL_EPUB);
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navNode, "id", "toc");
            navigationDocRootNode.AppendChild(navNode);

            XmlNode h1Node = navigationDocument.CreateElement("h1", navNode.NamespaceURI);
            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, h1Node, "class", "title");
            navNode.AppendChild(h1Node);
            h1Node.AppendChild(navigationDocument.CreateTextNode("Table of contents")) ;

            Dictionary<urakawa.core.TreeNode, XmlNode> treeNode_NavNodeMap = new Dictionary<urakawa.core.TreeNode, XmlNode>();
            m_FilesList_Smil = new List<string>();
            m_FilesList_Html = new List<string>();
            m_SmilDurationForOpfMetadata = new List<string>();
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
            XmlNode navPageListNode = null;

            //m_ProgressPercentage = 20;
            reportProgress(-1, UrakawaSDK_daisy_Lang.CreateSmilAndNcxFiles);

            ////System.Windows.Forms.MessageBox.Show(m_ListOfLevels.Count.ToString());
            foreach (urakawa.core.TreeNode urakawaNode in m_ListOfLevels)
                        {
                
                bool IsNcxNativeNodeAdded = false;
                XmlDocument smilDocument = null;
                XmlDocument htmlDocument = null;
                string smilFileName = null;
                string htmlFileName = null;
                XmlNode ulHeadingsNode = null;
                urakawa.core.TreeNode currentHeadingTreeNode = null;
                urakawa.core.TreeNode special_UrakawaNode = null;
                Time durationOfCurrentSmil = new Time();
                XmlNode mainSeq = null;
                XmlNode Seq_SpecialNode = null;
                
                string firstPar_id = null;
                bool shouldAddNewSeq = false;
                string par_id = null;
                List<string> currentSmilCustomTestList = new List<string>();
                Stack<urakawa.core.TreeNode> specialParentNodeStack = new Stack<urakawa.core.TreeNode>();
                Stack<XmlNode> specialSeqNodeStack = new Stack<XmlNode>();
                SectionNode section = (SectionNode)urakawaNode;
                Dictionary<urakawa.core.TreeNode, XmlNode> headingNodeToXmlNodeMap = null;
                XmlNode htmlBodyNode = null;
                XmlNode sectionXmlNode = null;
                string strSectionID = null;

                bool isBranchingActive = false;
                urakawa.core.TreeNode branchStartTreeNode = null;
                ////System.Windows.Forms.MessageBox.Show(urakawaNode.GetTextFlattened(false));
                urakawaNode.AcceptDepthFirst(
            delegate(urakawa.core.TreeNode n)
            {
                
                if (RequestCancellation) return false;

                if (htmlDocument == null)
                {
                    
                    htmlDocument = CreateStub_XhtmlContentDocument("en-US", null, null);
                    //XmlNode htmlHeadNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(htmlDocument, true, "head", htmlDocument.DocumentElement.NamespaceURI).FirstChild;
                    XmlNode htmlHeadNode = htmlDocument.GetElementsByTagName("head")[0];
                    if (htmlHeadNode == null)
                        System.Windows.Forms.MessageBox.Show("head node is null ");
                    else
                    {
                        XmlNode titleNode = htmlDocument.CreateElement("title", htmlHeadNode.NamespaceURI);
                        titleNode.AppendChild(htmlDocument.CreateTextNode( ((SectionNode)urakawaNode).Label));
                        htmlHeadNode.AppendChild(titleNode);
                    }
                    //htmlBodyNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(htmlDocument, true, "body", null).FirstChild;
                    htmlBodyNode = htmlDocument.GetElementsByTagName("body")[0];
                    headingNodeToXmlNodeMap = new Dictionary<TreeNode, XmlNode>();
                    

                }
                // create nodes in xhtml content document.
                if (n is SectionNode)
                {
                    //System.Windows.Forms.MessageBox.Show(htmlDocument.ToString() );//+ " : " + htmlBodyNode.ToString());
                    sectionXmlNode = htmlDocument.CreateElement("section", htmlBodyNode.NamespaceURI);
                    if (headingNodeToXmlNodeMap.Count == 0)
                    {
                        htmlBodyNode.AppendChild(sectionXmlNode);
                    }
                    else if (headingNodeToXmlNodeMap.ContainsKey(n.Parent))
                    {
                        headingNodeToXmlNodeMap[n.Parent].AppendChild(sectionXmlNode);
                    }
                    
                    XmlDocumentHelper.CreateAppendXmlAttribute(htmlDocument, sectionXmlNode, "id", GetNextID(ID_DTBPrefix)) ;
                    XmlNode hNode = htmlDocument.CreateElement("h1", htmlBodyNode.NamespaceURI);
                    strSectionID = GetNextID(ID_DTBPrefix);
                    XmlDocumentHelper.CreateAppendXmlAttribute(htmlDocument, hNode , "id", strSectionID) ;
                    sectionXmlNode.AppendChild(hNode);
                    hNode.AppendChild(
                        htmlDocument.CreateTextNode(((SectionNode)n).Label )) ;
                    headingNodeToXmlNodeMap.Add(n, sectionXmlNode);
                }
                

                if (n is SectionNode
                    && (!m_ListOfLevels.Contains(n) || m_ListOfLevels.IndexOf(n) > m_ListOfLevels.IndexOf(urakawaNode)))
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
                if (n is EmptyNode && (((EmptyNode)n).Role_ == EmptyNode.Role.Heading || ((EmptyNode)n).Role_ == EmptyNode.Role.Page || ((EmptyNode)n).Role_ == EmptyNode.Role.Anchor || IsSkippable((EmptyNode)n))
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

                // create smil stub document
                if (smilDocument == null)
                {
                    smilDocument = CreateStub_SmilDocument();
                    mainSeq = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(smilDocument, true, "body", null).FirstChild;
                    XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, mainSeq, "id", GetNextID(ID_SmilPrefix));
                    smilFileName = GetNextSmilFileName;
                    htmlFileName = Path.GetFileNameWithoutExtension(smilFileName) + ".html";
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
                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page) XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "class", "pagenum");
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
                    
                    //if (IsSkippableNode(special_UrakawaNode))
                    if (((EmptyNode)special_UrakawaNode).Role_ == EmptyNode.Role.Anchor && ((EmptyNode)special_UrakawaNode).AssociatedNode != null)
                    {
                        m_AnchorXmlNodeToReferedNodeMap.Add(Seq_SpecialNode, ((EmptyNode)special_UrakawaNode).AssociatedNode);
                        XmlNode anchorNode = smilDocument.CreateElement(null, "a", Seq_SpecialNode.NamespaceURI);
                        Seq_SpecialNode.AppendChild(anchorNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "external", "false");
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, anchorNode, "href", "");
                        if (m_Skippable_UpstreamIdMap.Count > 0 && m_Skippable_UpstreamIdMap.ContainsKey(((EmptyNode)special_UrakawaNode).AssociatedNode))
                        {
                            anchorNode.Attributes.GetNamedItem("href").Value = m_Skippable_UpstreamIdMap[((EmptyNode)special_UrakawaNode).AssociatedNode];
                        }
                        if (!m_AnchorSmilDoc_SmileFileNameMap.ContainsKey(smilDocument)) m_AnchorSmilDoc_SmileFileNameMap.Add(smilDocument, smilFileName);
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
                                

                                anchorNode.Attributes.GetNamedItem("href").Value = smilFileName + "#" + strSeqID;
                                foundAnchor = true;
                                //break;
                            }
                        }
                        if (!foundAnchor) m_Skippable_UpstreamIdMap.Add(n, smilFileName + "#" + strSeqID);
                    }

                    if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                    {
                        //XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", special_UrakawaNode.GetXmlElementQName().LocalName);
                        XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, Seq_SpecialNode, "customTest", "pagenum");



                        if (!currentSmilCustomTestList.Contains("pagenum"))
                        {
                            currentSmilCustomTestList.Add("pagenum");
                        }
                    }

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

                    string audioFileName = AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src);
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
                if (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Page)
                {
                    if (!currentSmilCustomTestList.Contains("pagenum"))
                    {
                        currentSmilCustomTestList.Add("pagenum");
                    }

                    //XmlNode pageListNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navigationDocument, true, "pageList", null);
                    XmlNode ulPageNode = null;
                    if (navPageListNode == null)
                    {
                        navPageListNode = navigationDocument.CreateElement("nav", navigationDocRootNode.NamespaceURI);
                        navigationDocRootNode.AppendChild(navPageListNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navPageListNode, "epub:type", "page-list", NS_URL_EPUB);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navPageListNode, "id", "page-list");

                        XmlNode pageHeading = navigationDocument.CreateElement("h1", navPageListNode.NamespaceURI);
                        navPageListNode.AppendChild(pageHeading);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageHeading, "id", GetNextID(ID_NcxPrefix));
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageHeading, "class", "pagelist");
                        pageHeading.AppendChild(navigationDocument.CreateTextNode("List of pages"));

                        ulPageNode = navigationDocument.CreateElement("ul", navPageListNode.NamespaceURI);
                        navPageListNode.AppendChild(ulPageNode);
                    }
                    if (ulPageNode == null ) ulPageNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(navPageListNode, true, "ul", null);

                    //if (pageListNode == null)
                    //{
                        //pageListNode = navigationDocument.CreateElement(null, "pageList", navigationDocRootNode.NamespaceURI);

                        //navigationDocRootNode.InsertAfter(pageListNode, navNode);
                    //}

                    XmlNode pageTargetNodeLI = navigationDocument.CreateElement(null, "li", navPageListNode.NamespaceURI);
                    ulPageNode.AppendChild(pageTargetNodeLI);

                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageTargetNodeLI, "id", GetNextID(ID_NcxPrefix));
                    //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocu       ment, pageTargetNodeLI, "class", "pagenum");
                    //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageTargetNodeLI, "playOrder", "");
                    string strTypeVal = ((EmptyNode)n).PageNumber.Kind.ToString().ToLower();
                    //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageTargetNodeLI, "type", strTypeVal);
                    //string strPageValue = n.GetXmlProperty().GetAttribute("pageText").Value ;
                    string strPageValue = ((EmptyNode)n).PageNumber.Number.ToString();
                    ++totalPageCount;

                    playOrderList_Sorted.Add(pageTargetNodeLI);

                    if (strTypeVal == "normal" || strTypeVal == "front")
                    {
                        int tmp;
                        bool success = int.TryParse(strPageValue, out tmp);
                        if (success && maxNormalPageNumber < tmp) maxNormalPageNumber = tmp;
                    }
                    if (strTypeVal != "special")
                    {
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, pageTargetNodeLI, "value", strPageValue);
                    }
                    string strContentDocPageId = GetNextID(ID_DTBPrefix) ;
                    XmlNode spanNode = htmlDocument.CreateElement ("span", sectionXmlNode.NamespaceURI ) ;
                    sectionXmlNode.AppendChild(spanNode) ;
                    XmlDocumentHelper.CreateAppendXmlAttribute (htmlDocument, spanNode, "epub:type", "pagebreak") ;
                    XmlDocumentHelper.CreateAppendXmlAttribute (htmlDocument, spanNode, "id", strContentDocPageId) ;
                    spanNode.AppendChild(htmlDocument.CreateTextNode(((EmptyNode)n).PageNumber.Unquoted));
                    //XmlNode navLabelNode = navigationDocument.CreateElement(null, "navLabel", pageListNode.NamespaceURI);
                    //pageTargetNodeLI.AppendChild(navLabelNode);

                    //XmlNode txtNode = navigationDocument.CreateElement(null, "text", pageListNode.NamespaceURI);
                    //navLabelNode.AppendChild(txtNode);
                    //txtNode.AppendChild(
                        //navigationDocument.CreateTextNode(((EmptyNode)n).PageNumber.Unquoted));

                    //if (externalAudio != null)
                    //{
                        //XmlNode audioNodeNcx = navigationDocument.CreateElement(null, "audio", pageListNode.NamespaceURI);
                        //navLabelNode.AppendChild(audioNodeNcx);
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "src",
                            //AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src));
                    //}

                    XmlNode anchorPageNode = navigationDocument.CreateElement(null, "a", navPageListNode.NamespaceURI);
                    pageTargetNodeLI.AppendChild(anchorPageNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, anchorPageNode, "href", htmlFileName+ "#" + strContentDocPageId);
                anchorPageNode.AppendChild(
                        navigationDocument.CreateTextNode(((EmptyNode)n).PageNumber.Unquoted));
                    ////System.Windows.Forms.MessageBox.Show("Page ");
                    // add reference to par in dtbook document

                }
                //obi: commented for now
                    /*
                else if (special_UrakawaNode != null && n == special_UrakawaNode
                && n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Custom)
                //&&  m_NavListElementNamesList.Contains(((EmptyNode)n).CustomRole) && !specialParentNodesAddedToNavList.Contains(special_UrakawaNode))
                {
                    EmptyNode eSpecialNode = (EmptyNode)special_UrakawaNode;
                    string navListNodeName = eSpecialNode.CustomRole;
                    specialParentNodesAddedToNavList.Add(special_UrakawaNode);
                    XmlNode navListNode = null;

                    //= getFirstChildElementsWithName ( ncxDocument, true, "navList", null );
                    
                    foreach (XmlNode xn in XmlDocumentHelper.GetChildrenElementsOrSelfWithName(navigationDocRootNode, true, "navList", navigationDocRootNode.NamespaceURI, true))
                    {
                        
                        if (xn.Attributes.GetNamedItem("class").Value == navListNodeName)
                        {
                            navListNode = xn;
                        }
                    }
                    
                    if (navListNode == null)
                    {
                        navListNode = navigationDocument.CreateElement(null, "navList", navigationDocRootNode.NamespaceURI);
                        navigationDocRootNode.AppendChild(navListNode);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navListNode, "class", navListNodeName);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navListNode, "id", GetNextID(ID_NcxPrefix));

                        XmlNode mainNavLabel = navigationDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                        navListNode.AppendChild(mainNavLabel);
                        XmlNode mainTextNode = navigationDocument.CreateElement(null, "text", navListNode.NamespaceURI);
                        mainNavLabel.AppendChild(mainTextNode);
                        mainTextNode.AppendChild(navigationDocument.CreateTextNode(navListNodeName));
                    }

                    XmlNode navTargetNode = navigationDocument.CreateElement(null, "navTarget", navListNode.NamespaceURI);
                    navListNode.AppendChild(navTargetNode);

                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navTargetNode, "id", GetNextID(ID_NcxPrefix));
                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navTargetNode, "class", navListNodeName);
                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navTargetNode, "playOrder", "");

                    playOrderList_Sorted.Add(navTargetNode);


                    XmlNode navLabelNode = navigationDocument.CreateElement(null, "navLabel", navListNode.NamespaceURI);
                    navTargetNode.AppendChild(navLabelNode);

                    XmlNode txtNode = navigationDocument.CreateElement(null, "text", navTargetNode.NamespaceURI);
                    navLabelNode.AppendChild(txtNode);
                    txtNode.AppendChild(
                        navigationDocument.CreateTextNode(((EmptyNode)n).CustomRole));

                    // create audio node only if external audio media is not null
                    if (externalAudio != null)
                    {
                        XmlNode audioNodeNcx = navigationDocument.CreateElement(null, "audio", navTargetNode.NamespaceURI);
                        navLabelNode.AppendChild(audioNodeNcx);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNodeNcx, "src",
                            AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src));
                    }

                    XmlNode contentNode = navigationDocument.CreateElement(null, "content", navTargetNode.NamespaceURI);
                    navTargetNode.AppendChild(contentNode);
                    XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, contentNode, "src", smilFileName + "#" + par_id);
                    
                }
                    */
                // the following is operational
                if (!IsNcxNativeNodeAdded)
                {
                    if (!isDocTitleAdded)
                    {
                        //if (urakawaNode == m_ListOfLevels[0]
                            //&& ((section.Heading == null && n is EmptyNode) || section.Heading == n))
                        //{
                            //string txtMedia = urakawaNode.GetTextFlattened();
                            //externalAudio = GetExternalAudioMedia(n);

                            //XmlNode docNode = navigationDocument.CreateElement(null,
                                //"docTitle",
                                 //navigationDocRootNode.NamespaceURI);

                            //navigationDocRootNode.InsertBefore(docNode, navNode);

                            //XmlNode docTxtNode = navigationDocument.CreateElement(null, "text", docNode.NamespaceURI);
                            //docNode.AppendChild(docTxtNode);
                            //docTxtNode.AppendChild(
                            //navigationDocument.CreateTextNode(txtMedia));

                            //if (externalAudio != null)
                            //{
                                // create audio node
                                //XmlNode docAudioNode = navigationDocument.CreateElement(null, "audio", docNode.NamespaceURI);
                                //docNode.AppendChild(docAudioNode);
                                //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, docAudioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                                //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, docAudioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                                //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, docAudioNode, "src",
                                    //AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src));
                            //}


                            isDocTitleAdded = true;
                            
                        //}
                    }

                    if ((urakawaNode is SectionNode && section.Heading == null && n is EmptyNode) ||
                        (n is EmptyNode && ((EmptyNode)n).Role_ == EmptyNode.Role.Heading))
                    {
                        /*

                        */

                        string txtMedia = urakawaNode.GetTextFlattened();
                        ////System.Windows.Forms.MessageBox.Show("nav point " + txtMedia);
                        externalAudio = GetExternalAudioMedia(n);

                        // first create navPoints
                        ulHeadingsNode = navigationDocument.CreateElement(null, "ul", navNode.NamespaceURI);
                        
                        //if (currentHeadingTreeNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navPointNode, "class", "heading");
                        string strNavPointID = GetNextID(ID_NcxPrefix);
                        XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, ulHeadingsNode, "id", strNavPointID);
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, navPointNode, "playOrder", "");


                        //add the navpoint id to smil text in par of corresponding smil file
                        //if (SmilTextNode != null) XmlDocumentHelper.CreateAppendXmlAttribute(smilDocument, SmilTextNode, "src", m_Filename_Ncx + "#" + strNavPointID);
                        //urakawa.core.TreeNode parentNode = GetParentLevelNode(urakawaNode);
                        urakawa.core.TreeNode parentNode = urakawaNode.Parent;

                        if (parentNode == null || parentNode == m_Presentation.RootNode)
                        {
                            navNode.AppendChild(ulHeadingsNode);
                        }
                        else if (treeNode_NavNodeMap.ContainsKey(parentNode))
                        {
                            treeNode_NavNodeMap[parentNode].AppendChild(ulHeadingsNode);
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
                                    treeNode_NavNodeMap[parentNode].AppendChild(ulHeadingsNode);
                                    break;
                                }
                                counter++;
                            }

                            if (parentNode == null || counter > 7)
                            {
                                navNode.AppendChild(ulHeadingsNode);
                            }
                        }


                        treeNode_NavNodeMap.Add(urakawaNode, ulHeadingsNode);

                        // create navLabel
                        XmlNode liHeadingNode = navigationDocument.CreateElement(null, "li", ulHeadingsNode.NamespaceURI);
                        ulHeadingsNode.AppendChild(liHeadingNode);

                        // create text node
                        //XmlNode txtNode = navigationDocument.CreateElement(null, "text", navNode.NamespaceURI);
                        //navLabel.AppendChild(txtNode);
                        
                        //txtNode.AppendChild(
                        //navigationDocument.CreateTextNode(txtMedia));

                        // create audio node
                        //XmlNode audioNode = navigationDocument.CreateElement(null, "audio", navNode.NamespaceURI);
                        //navLabel.AppendChild(audioNode);
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNode, "clipBegin", FormatTimeString(externalAudio.ClipBegin));
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNode, "clipEnd", FormatTimeString(externalAudio.ClipEnd));
                        //XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, audioNode, "src",
                            //AddSectionNameToAudioFile ? AddSectionNameToAudioFileName(externalAudio.Src, section.Label) : Path.GetFileName(externalAudio.Src));

                        //playOrderList_Sorted.Add(navPointNode);

                        // add content node
                        if (strSectionID != null)
                        {
                            XmlNode contentAnchorNode = navigationDocument.CreateElement(null, "a", navNode.NamespaceURI);
                            liHeadingNode.AppendChild(contentAnchorNode);
                            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, contentAnchorNode, "href",  htmlFileName+ "#" + strSectionID);
                            
                            contentAnchorNode.AppendChild(
                            navigationDocument.CreateTextNode(txtMedia));


                            ////System.Windows.Forms.MessageBox.Show("Navpoint");
                        }
                        ///int navPointDepth = GetDepthOfNavPointNode(navigationDocument, navPointNode);
                        int navPointDepth = 1;
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
                    string strSmilDuration = FormatTimeString(smilElapseTime) ;
                    AddMetadata_Smil(smilDocument, strSmilDuration, null);

                    XmlReaderWriterHelper.WriteXmlDocument(smilDocument, Path.Combine(m_OutputDirectory, smilFileName), AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null);

                    smilElapseTime.Add(durationOfCurrentSmil);
                    m_FilesList_Smil.Add(smilFileName);
                    m_SmilDurationForOpfMetadata.Add(strSmilDuration);
                    smilDocument = null;

                    m_FilesList_Html.Add(htmlFileName);
                    XmlReaderWriterHelper.WriteXmlDocument(htmlDocument, Path.Combine(m_OutputDirectory, htmlFileName), AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null);
                    htmlDocument = null;

                    // add smil custon test list items to ncx custom test list
                    foreach (string customTestName in currentSmilCustomTestList)
                    {

                        if (!ncxCustomTestList.Contains(customTestName))
                            ncxCustomTestList.Add(customTestName);
                    }

                }

            }
            /*
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
             */ 
            // rewrite the smil files that has anchor references
            foreach (XmlDocument sd in m_AnchorSmilDoc_SmileFileNameMap.Keys)
            {
                XmlReaderWriterHelper.WriteXmlDocument(sd, Path.Combine(m_OutputDirectory, m_AnchorSmilDoc_SmileFileNameMap[sd]),
                    (AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null));
            }
            m_AnchorSmilDoc_SmileFileNameMap = null;
            if (RequestCancellation)
            {
                //m_DTBDocument = null;
                navigationDocument = null;
                return;
            }
            //SaveXukAction.WriteXmlDocument(m_DTBDocument, Path.Combine(m_OutputDirectory, m_Filename_Content));

            if (RequestCancellation)
            {
                //m_DTBDocument = null;
                navigationDocument = null;
                return;
            }
            // write ncs document to file
            m_TotalTime = new Time(smilElapseTime);
            AddMetadata_Ncx(navigationDocument, totalPageCount.ToString(), maxNormalPageNumber.ToString(), maxDepth.ToString(), null);
            XmlReaderWriterHelper.WriteXmlDocument(navigationDocument, Path.Combine(m_OutputDirectory, "Navigation.html"), AlwaysIgnoreIndentation ? GetXmlWriterSettings(false) : null);
        }

        protected XmlDocument CreateStub_NavigationDocument()
        {
            XmlDocument navigationDocument = new XmlDocument();
            navigationDocument.XmlResolver = null;

            navigationDocument.CreateXmlDeclaration("1.0", "utf-8", null);
            navigationDocument.AppendChild(navigationDocument.CreateDocumentType("html",
                "-//W3C//DTD XHTML 1.0 Transitional//EN",
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                null));

            XmlNode rootNode = navigationDocument.CreateElement(null,
                "html",
                "http://www.w3.org/1999/xhtml");

            navigationDocument.AppendChild(rootNode);

            XmlDocumentHelper.CreateAppendXmlAttribute(navigationDocument, rootNode, "xmlns:epub", "http://www.idpf.org/2007/ops");
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

        private  XmlDocument CreateStub_XhtmlContentDocument(string language, string strInternalDTD, List<ExternalFileData> list_ExternalStyleSheets)
        {
            XmlDocument xhtmlDocument = new XmlDocument();
            xhtmlDocument.XmlResolver = null;

            xhtmlDocument.CreateXmlDeclaration("1.0", "utf-8", null);

            

            xhtmlDocument.AppendChild(xhtmlDocument.CreateDocumentType("html",
                null,
                null,
                null));

            XmlNode htmlNode = xhtmlDocument.CreateElement(null,
                "html",
                "http://www.w3.org/1999/xhtml");

            xhtmlDocument.AppendChild(htmlNode);


            XmlDocumentHelper.CreateAppendXmlAttribute(xhtmlDocument, htmlNode, "xmlns:epub", "http://www.idpf.org/2007/ops");
            XmlDocumentHelper.CreateAppendXmlAttribute(xhtmlDocument, htmlNode, XmlReaderWriterHelper.XmlLang, (String.IsNullOrEmpty(language) ? "en-US" : language));
            XmlDocumentHelper.CreateAppendXmlAttribute(xhtmlDocument, htmlNode, "xmlns", "http://www.w3.org/1999/xhtml");

            XmlNode headNode = xhtmlDocument.CreateElement(null, "head", htmlNode.NamespaceURI);
            htmlNode.AppendChild(headNode);
            XmlNode bookNode = xhtmlDocument.CreateElement(null, "body", htmlNode.NamespaceURI);
            htmlNode.AppendChild(bookNode);

            return xhtmlDocument;
        }

        protected override XmlDocument CreateStub_OpfDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;

            xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);

            XmlNode package = xmlDoc.CreateElement(null,
                "package",
                NS_URL_EPUB);

            xmlDoc.AppendChild(package);

            XmlDocumentHelper.CreateAppendXmlAttribute(xmlDoc, package, "version", "3.0");

            //if (isXukSpine)
            //{
                //XmlAttribute xmlAttr = m_Presentation.RootNode.GetXmlProperty().GetAttribute("prefix");
                //if (xmlAttr != null)
                //{
                    ////"rendition: http://www.idpf.org/vocab/rendition/# cc: http://creativecommons.org/ns#"
                    //XmlDocumentHelper.CreateAppendXmlAttribute(xmlDoc, package, "prefix", xmlAttr.Value);
                //}
            //}

            XmlNode metadata = xmlDoc.CreateElement(null, "metadata", package.NamespaceURI);
            package.AppendChild(metadata);

            XmlNode manifest = xmlDoc.CreateElement(null, "manifest", package.NamespaceURI);
            package.AppendChild(manifest);

            XmlNode spine = xmlDoc.CreateElement(null, "spine", package.NamespaceURI);
            package.AppendChild(spine);

            return xmlDoc;

                    }


        protected override void AddMetadata_Opf(XmlDocument opfDocument)
        {
            XmlNode dc_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "metadata", null); //opfDocument.GetElementsByTagName("dc-metadata")[0];
            //XmlNode x_metadataNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "x-metadata", null); //opfDocument.GetElementsByTagName("x-metadata")[0];

            urakawa.metadata.Metadata mdId = AddMetadata_DtbUid(true, opfDocument, dc_metadataNode);

            //AddMetadata_Generator(opfDocument, x_metadataNode);

            bool textOnly = m_TotalTime == null || m_TotalTime.AsLocalUnits == 0;

            if (true || !textOnly)
            {
                //AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_TOTAL_TIME, FormatTimeString(m_TotalTime));
            }

            //type 1 "audioOnly"
            //type 2 "audioNCX"
            //type 3 "audioPartText"
            //type 4 "audioFullText"
            //type 5 "textPartAudio"
            //type 6 "textNCX"
            //http://www.daisy.org/z3986/specifications/daisy_202.html#dtbclass
            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE).Count == 0)
            {
                //AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE, m_Filename_Content != null ?
                    //(textOnly ? "textNCX" : "audioFullText") //"textPartAudio"
                    //: "audioNCX");
            }

            //audio,text,image ???
            if (true || m_Presentation.GetMetadata(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT).Count == 0)
            {
                //AddMetadataAsAttributes(opfDocument, x_metadataNode, SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT, m_Filename_Content != null ?
                    //(textOnly ? "text" : "audio,text") //"audio,text"
                    //: "audio");
            }

            AddMetadataAsInnerText(opfDocument, dc_metadataNode, SupportedMetadata_Z39862005.DC_Format.ToLower(), "ANSI/NISO Z39.86-2005");


            bool hasMathML_z39_86_extension_version = false;
            bool hasMathML_DTBook_XSLTFallback = false;

            foreach (urakawa.metadata.Metadata m in m_Presentation.Metadatas.ContentsAs_Enumerable)
            {
                string name = m.NameContentAttribute.Name;

                if (name == SupportedMetadata_Z39862005._z39_86_extension_version)
                {
                    hasMathML_z39_86_extension_version = true;
                }
                else if (name == SupportedMetadata_Z39862005.MATHML_XSLT_METADATA)
                {
                    hasMathML_DTBook_XSLTFallback = true;
                }

                //string lowerName = m.NameContentAttribute.Name.ToLower();
                if (mdId == m
                    || SupportedMetadata_Z39862005.DTB_TOTAL_TIME.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DC_Format.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE.Equals(name, StringComparison.OrdinalIgnoreCase)
                    || SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT.Equals(name, StringComparison.OrdinalIgnoreCase)
                    )
                    continue;

                XmlNode metadataNodeCreated = null;
                //if (m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.DC + ":"))

                bool contains = false;
                foreach (string str in m_AllowedInDcMetadata)
                {
                    if (str.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        contains = true;
                        break;
                    }
                }

                bool containsDtb = false;
                foreach (string str in m_DtbAllowedInXMetadata)
                {
                    if (str.Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        containsDtb = true;
                        break;
                    }
                }

                if (contains)
                {
                    metadataNodeCreated = AddMetadataAsInnerText(opfDocument, dc_metadataNode, name, m.NameContentAttribute.Value);
                    // add other metadata attributes if any
                    foreach (urakawa.metadata.MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id" || ma.Name == urakawa.metadata.Metadata.PrimaryIdentifierMark)
                        {
                            continue;
                        }
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                    }
                }
                //else
                //items in x-metadata may start with dtb: ONLY if they are in the list of allowed dtb:* items
                //OR, items in x-metadata may be anything else (non-dtb:*).
                else if (
                    (
                    containsDtb
                    && m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DTB + ":", StringComparison.OrdinalIgnoreCase)
                    )
                    || !m.NameContentAttribute.Name.StartsWith(SupportedMetadata_Z39862005.NS_PREFIX_DTB + ":", StringComparison.OrdinalIgnoreCase)
                    )
                {
                    //metadataNodeCreated = AddMetadataAsAttributes(opfDocument, x_metadataNode, name, m.NameContentAttribute.Value);

                    // add other metadata attributes if any
                    foreach (MetadataAttribute ma in m.OtherAttributes.ContentsAs_Enumerable)
                    {
                        if (ma.Name == "id" || ma.Name == urakawa.metadata.Metadata.PrimaryIdentifierMark)
                        {
                            continue;
                        }
                        XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metadataNodeCreated, ma.Name, ma.Value);
                    }
                }

            } // end of metadata for each loop

            string mathML_XSLT = null;

            foreach (urakawa.ExternalFiles.ExternalFileData efd in m_Presentation.ExternalFilesDataManager.ManagedObjects.ContentsAs_Enumerable)
            {
                if (efd is XSLTExternalFileData)
                {
                    string filename = efd.OriginalRelativePath;
                    if (filename.StartsWith(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA))
                    {
                        filename = filename.Substring(SupportedMetadata_Z39862005.MATHML_XSLT_METADATA.Length);

                        mathML_XSLT = filename;
                        break;
                    }
                }
            }

            string mathPrefix = m_Presentation.RootNode.GetXmlNamespacePrefix(DiagramContentModelHelper.NS_URL_MATHML);

            if (!string.IsNullOrEmpty(mathML_XSLT))
            {
                DebugFix.Assert(hasMathML_z39_86_extension_version);
                DebugFix.Assert(hasMathML_DTBook_XSLTFallback);
            }
            /*
            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_z39_86_extension_version)
            {
                XmlNode metaNode = opfDocument.CreateElement(null, "meta", x_metadataNode.NamespaceURI);
                x_metadataNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "name", SupportedMetadata_Z39862005._z39_86_extension_version);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "content", "1.0");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }
            */
            if (!string.IsNullOrEmpty(mathPrefix) && !hasMathML_DTBook_XSLTFallback)
            {
                //XmlNode metaNode = opfDocument.CreateElement(null, "meta", x_metadataNode.NamespaceURI);
                //x_metadataNode.AppendChild(metaNode);
                //XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "name", SupportedMetadata_Z39862005.MATHML_XSLT_METADATA);
                //XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "content", string.IsNullOrEmpty(mathML_XSLT) ? SupportedMetadata_Z39862005._builtInMathMLXSLT : mathML_XSLT);
                //XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "scheme", DiagramContentModelHelper.NS_URL_MATHML);
            }

            
            // add uid to dc:identifier
            //XmlNodeList identifierList = opfDocument.GetElementsByTagName("dc:Identifier");
            //XmlNode identifierNode = null;
            
        }

        protected override void CreateOpfDocument()
        {
            //m_ProgressPercentage = 90;
            //reportProgress(m_ProgressPercentage, UrakawaSDK_daisy_Lang.AllFilesCreated);
            if (RequestCancellation) return;
            XmlDocument opfDocument = CreateStub_OpfDocument();

            XmlNode manifestNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "manifest", null); //opfDocument.GetElementsByTagName("manifest")[0];


            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Title);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Creator);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Subject);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Description);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Identifier);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Publisher);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Contributor);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Date);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Type);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Format);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.D_Source);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Language);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Relation);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Coverage);
            m_AllowedInDcMetadata.Add(SupportedMetadata_Z39862005.DC_Rights);

            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_EDITION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_PUBLISHER);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_RIGHTS);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_SOURCE_TITLE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_TYPE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_MULTIMEDIA_CONTENT);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_NARRATOR);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_PRODUCER);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_PRODUCED_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION_DATE);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_REVISION_DESCRIPTION);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_TOTAL_TIME);
            m_DtbAllowedInXMetadata.Add(SupportedMetadata_Z39862005.DTB_AUDIO_FORMAT);

            XmlNode navNode =  AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Ncx, "ncx", DataProviderFactory.XHTML_MIME_TYPE);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, navNode, "properties", "nav");

            if (m_Filename_Content != null)
            {
                AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Content, GetNextID(ID_OpfPrefix), DataProviderFactory.DTBOOK_MIME_TYPE);
            }

            AddFilenameToManifest(opfDocument, manifestNode, m_Filename_Opf, GetNextID(ID_OpfPrefix), DataProviderFactory.XML_MIME_TYPE);

            foreach (string externalFileName in m_FilesList_ExternalFiles)
            {
                // ALREADY escaped!
                //externalFileName = FileDataProvider.EliminateForbiddenFileNameCharacters(externalFileName);

                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(externalFileName);
                if (DataProviderFactory.CSS_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.CSS_MIME_TYPE);
                }
                else if (DataProviderFactory.XSLT_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase)
                    || DataProviderFactory.XSL_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.XSLT_MIME_TYPE_);
                }
                else if (DataProviderFactory.DTD_EXTENSION.Equals(ext, StringComparison.OrdinalIgnoreCase))
                {
                    AddFilenameToManifest(opfDocument, manifestNode, externalFileName, strID, DataProviderFactory.DTD_MIME_TYPE);
                }
            }

            if (RequestCancellation) return;

            List<string> htmlIDListInPlayOrder = new List<string>();
            XmlNode metadataNode = opfDocument.GetElementsByTagName ("metadata")[0] ;

            for (int i = 0; i < m_FilesList_Smil.Count; i++ )
            {
                string smilFileName = m_FilesList_Smil[i];
                string strSmilDuration = m_SmilDurationForOpfMetadata[i];
                string htmlFileName = m_FilesList_Html[i];

                string strSmilID = GetNextID(ID_OpfPrefix);
                string strHtmlID = GetNextID(ID_OpfPrefix);

                AddFilenameToManifest(opfDocument, manifestNode, smilFileName, strSmilID, DataProviderFactory.SMIL_MIME_TYPE_);
                XmlNode htmlNode = AddFilenameToManifest(opfDocument, manifestNode, htmlFileName , strHtmlID, DataProviderFactory.XHTML_MIME_TYPE);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, htmlNode, "media-overlays", strSmilID);
                htmlIDListInPlayOrder.Add(strHtmlID);

              // add media-overlays duration metadata
                XmlNode metaNode = opfDocument.CreateElement("meta", metadataNode.NamespaceURI);
                metadataNode.AppendChild(metaNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "property", "media:duration");
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode, "refines","#" + strSmilID);
                metaNode.AppendChild(opfDocument.CreateTextNode(strSmilDuration));
            }
            XmlNode metaNode_MOActive = opfDocument.CreateElement("meta", metadataNode.NamespaceURI);
            metadataNode.AppendChild(metaNode_MOActive);
            XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, metaNode_MOActive, "property", "media:active-class");

            metaNode_MOActive.AppendChild(opfDocument.CreateTextNode("epub - media - overlay - active"));

            if (RequestCancellation) return;

            foreach (string audioFileName in m_FilesList_SmilAudio)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(audioFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mime);
            }

            if (RequestCancellation) return;

            foreach (string imageFileName in m_FilesList_Image)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(imageFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, imageFileName, strID, mime);
            }


#if SUPPORT_AUDIO_VIDEO
            if (RequestCancellation) return;

            foreach (string videoFileName in m_FilesList_Video)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(videoFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, videoFileName, strID, mime);
            }

            foreach (string audioFileName in m_FilesList_Audio)
            {
                string strID = GetNextID(ID_OpfPrefix);

                string ext = Path.GetExtension(audioFileName);
                string mime = DataProviderFactory.GetMimeTypeFromExtension(ext);
                AddFilenameToManifest(opfDocument, manifestNode, audioFileName, strID, mime);
            }
#endif

            if (RequestCancellation) return;

            bool textOnly = m_TotalTime == null || m_TotalTime.AsLocalUnits == 0;
            if (true || !textOnly)
            {
                // copy resource files and place entry in manifest
                string sourceDirectoryPath = System.AppDomain.CurrentDomain.BaseDirectory;
                string ResourceRes_Filename = "tpbnarrator.res";
                string resourceAudio_Filename = "tpbnarrator_res.mp3";

                string ResourceRes_Filename_fullPath = Path.Combine(sourceDirectoryPath, ResourceRes_Filename);
                string resourceAudio_Filename_fullPath = Path.Combine(sourceDirectoryPath, resourceAudio_Filename);
                if (File.Exists(ResourceRes_Filename_fullPath) && File.Exists(resourceAudio_Filename_fullPath))
                {
                    if (RequestCancellation) return;

                    string destRes = Path.Combine(m_OutputDirectory, ResourceRes_Filename);

                    if (!textOnly)
                    {
                        File.Copy(ResourceRes_Filename_fullPath, destRes, true);
                        try
                        {
                            File.SetAttributes(destRes, FileAttributes.Normal);
                        }
                        catch
                        {
                        }
                    }
                    else
                    {
                        string resXml = File.ReadAllText(ResourceRes_Filename_fullPath);

                        int i = -1;
                        int j = 0;
                        while ((i = resXml.IndexOf("<audio", j)) >= 0)
                        {
                            j = resXml.IndexOf("/>", i);
                            if (j > i)
                            {
                                int len = j - i + 2;
                                resXml = resXml.Remove(i, len);

                                string fill = "";
                                for (int k = 1; k <= len; k++)
                                {
                                    fill += ' '; //k.ToString();
                                }

                                resXml = resXml.Insert(i, fill);
                            }
                            else
                            {
#if DEBUG
                                Debugger.Break();
#endif
                                break;
                            }
                        }

                        StreamWriter streamWriter = File.CreateText(destRes);
                        try
                        {
                            streamWriter.Write(resXml);
                        }
                        finally
                        {
                            streamWriter.Close();
                        }
                    }

                    AddFilenameToManifest(opfDocument, manifestNode, ResourceRes_Filename, "resource", DataProviderFactory.DTB_RES_MIME_TYPE);

                    if (!textOnly)
                    {
                        string destFile = Path.Combine(m_OutputDirectory, resourceAudio_Filename);
                        File.Copy(resourceAudio_Filename_fullPath, destFile, true);
                        try
                        {
                            File.SetAttributes(destFile, FileAttributes.Normal);
                        }
                        catch
                        {
                        }

                        AddFilenameToManifest(opfDocument, manifestNode, resourceAudio_Filename, GetNextID(ID_OpfPrefix), DataProviderFactory.AUDIO_MP3_MIME_TYPE);
                    }
                }
            }

            // create spine
            XmlNode spineNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(opfDocument, true, "spine", null); //opfDocument.GetElementsByTagName("spine")[0];

            foreach (string strHtmlID in htmlIDListInPlayOrder)
            {
                XmlNode itemRefNode = opfDocument.CreateElement(null, "itemref", spineNode.NamespaceURI);
                spineNode.AppendChild(itemRefNode);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemRefNode, "idref", strHtmlID);
                XmlDocumentHelper.CreateAppendXmlAttribute(opfDocument, itemRefNode, "linear" , "yes");
            }

            if (RequestCancellation) return;

            AddMetadata_Opf(opfDocument);

            if (RequestCancellation)
            {
                opfDocument = null;
                return;
            }

            XmlReaderWriterHelper.WriteXmlDocument(opfDocument, OpfFilePath, null);
            CreateAdditionalFilesForEPUB();
            //urakawa.daisy.export.Epub3_Export.ZipEpub(Directory.GetParent(m_EpubParentDirectoryPath).FullName, m_EpubParentDirectoryPath);
        }

        private void CreateAdditionalFilesForEPUB()
        {
string meta_InfPath = Path.Combine(m_EpubParentDirectoryPath, m_Meta_infFileName);
                if (!Directory.Exists(meta_InfPath))
                {
                    Directory.CreateDirectory(meta_InfPath);
                }
            StreamWriter sw_Container = null ;
            StreamWriter sw_MimeType = null ;
            try
            {
                string containerFilePath = Path.Combine(meta_InfPath, "container.xml");
            string relativeOpfPath = m_OutputDirectoryName + Path.DirectorySeparatorChar + Path.GetFileName(OpfFilePath);
            
                string containerXML = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n<container version=\"1.0\" xmlns=\"urn:oasis:names:tc:opendocument:xmlns:container\">\n<rootfiles>\n<rootfile full-path=\"" + relativeOpfPath + "\" media-type=\"application/oebps-package+xml\" />\n</rootfiles>\n</container>";
            sw_Container = File.CreateText (containerFilePath ) ;
            sw_Container.Write (containerXML ) ;

                string mimeTypeFilePath = Path.Combine(m_EpubParentDirectoryPath, "mimetype");
                sw_MimeType = File.CreateText (mimeTypeFilePath ) ;
                sw_MimeType.Write ("application/epub+zip");
                
            }
            finally
            {
            if (sw_Container != null ) sw_Container.Close () ;
                if ( sw_MimeType != null ) sw_MimeType.Close () ;

                
            }
            
            
        }
        

            }
}
