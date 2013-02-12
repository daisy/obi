using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

using urakawa.core;
using urakawa.media;
using urakawa.media.timing;
using urakawa.property.channel;
using urakawa.metadata;
using urakawa.daisy.export.visitor;

namespace Obi.ImportExport
    {
    public enum ExportFormat { DAISY3_0, DAISY2_02, Both_DAISY3_DAISY202 } ;

    public class DAISY202Export:urakawa.daisy.export.Daisy3_Export
        {

        //private ObiPresentation m_Presentation;
        private string m_ExportDirectory;
        private Dictionary<string, string> m_MetadataMap;
        private Dictionary<string, string> m_SmilMetadata;
        private int m_PageFrontCount;
        private int m_PageNormalCount;
        private int m_PageSpecialCount;
        private int m_MaxPageNormal;
        private int m_IdCounter;
        private int m_ExportedSectionCount;
        private Time m_SmilElapseTime;
        private Dictionary<string, string> m_SmilFile_TitleMap;
        private Dictionary<SectionNode, EmptyNode> m_NextSectionPageAdjustmentDictionary;
        private int m_AudioFileSectionLevel;
        private EmptyNode m_FirstPageNumberedPhraseOfFirstSection;
        private int m_MaxDepth = 0 ;
        private List<string> m_FilesList = null;
        //private bool m_EncodeToMP3;
        //private int m_BitRate_Mp3;

        public DAISY202Export(ObiPresentation presentation, string exportDirectory, bool encodeToMp3, ushort mp3BitRate, 
            AudioLib.SampleRate sampleRate, bool stereo,
            int audioFileSectionLevel)
            :
            base(presentation, exportDirectory, null, encodeToMp3, mp3BitRate,
            sampleRate, stereo,
            true,false)
            {
            m_Presentation = presentation;
            m_ExportDirectory = exportDirectory;
            m_MetadataMap = CreateDAISY3To2MetadataMap ();
            m_SmilFile_TitleMap = new Dictionary<string, string> ();
            m_NextSectionPageAdjustmentDictionary = new Dictionary<SectionNode, EmptyNode>();
            m_AudioFileSectionLevel = audioFileSectionLevel;
            m_FilesList = new List<string>();
            //m_EncodeToMP3 = encodeToMP3;
            }

        public override void ConfigureAudioFileDelegates()
        {
            m_TriggerDelegate = delegate(urakawa.core.TreeNode node) { return node is SectionNode && ((SectionNode)node).Level <= m_AudioFileSectionLevel; };
            m_SkipDelegate = delegate(urakawa.core.TreeNode node) { return !((ObiNode)node).Used; };
        }


        private List<SectionNode> GetSectionsList(urakawa.core.TreeNode rNode) //sdk2 :used treenode instead of rootnode
            {
            List<SectionNode> sectionsList = new List<SectionNode> ();
            m_FirstPageNumberedPhraseOfFirstSection = null;
            rNode.AcceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {

                        if (n is SectionNode )
                            {
                                if (!((SectionNode)n).Used) return false;
                            sectionsList.Add ( (SectionNode)n );
                            m_NextSectionPageAdjustmentDictionary.Add((SectionNode)n , null );
                            }
                            else if ( n is EmptyNode && ((EmptyNode)n).Used 
                            && ((EmptyNode)n).Index == 0 && ((EmptyNode)n).Role_ == EmptyNode.Role.Page )
                        {   
                                if ( sectionsList.Count >= 2) m_NextSectionPageAdjustmentDictionary[sectionsList[sectionsList.Count-2]] = (EmptyNode)n ;
                                if (sectionsList.Count == 1) m_FirstPageNumberedPhraseOfFirstSection = (EmptyNode)n;
                                
                            }
                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );

            return sectionsList;
            }
            /*
            public int BitRate_Mp3
            {
                get { return  m_BitRate_Mp3 ; }
                    set{ m_BitRate_Mp3 = value ; }
            }
        
            private Channel CreateAudioFiles()
            {
                TreeNodeTestDelegate nodeIsSection = delegate(urakawa.core.TreeNode node) { return node is SectionNode && ((SectionNode)node).Level <= m_AudioFileSectionLevel; };
                TreeNodeTestDelegate nodeIsUnused = delegate(urakawa.core.TreeNode node) { return !((ObiNode)node).Used; };

                m_Presentation.RemoveAllPublishChannels(); // remove any publish channel, in case they exist

                PublishFlattenedManagedAudioVisitor visitor = new PublishFlattenedManagedAudioVisitor(nodeIsSection, nodeIsUnused);

                //urakawa.property.channel.Channel publishChannel = mPresentation.AddChannel(ObiPresentation.PUBLISH_AUDIO_CHANNEL_NAME);

                Channel publishChannel = m_Presentation.ChannelFactory.CreateAudioChannel();
                publishChannel.Name = ObiPresentation.PUBLISH_AUDIO_CHANNEL_NAME;

                visitor.DestinationChannel = publishChannel;
                visitor.SourceChannel = m_Presentation.ChannelsManager.GetOrCreateAudioChannel();
                visitor.DestinationDirectory = new Uri(m_ExportDirectory );

                visitor.EncodePublishedAudioFilesToMp3 = m_EncodeToMP3;
                if(m_EncodeToMP3 && m_BitRate_Mp3 >= 32)  visitor.BitRate_Mp3 = (ushort)m_BitRate_Mp3;
                uint sampleRate = m_Presentation.MediaDataManager.DefaultPCMFormat.Data.SampleRate;
                if (sampleRate == 44100) visitor.EncodePublishedAudioFilesSampleRate = AudioLib.SampleRate.Hz44100;
                else if (sampleRate == 22050) visitor.EncodePublishedAudioFilesSampleRate = AudioLib.SampleRate.Hz22050;
                else if (sampleRate == 11025) visitor.EncodePublishedAudioFilesSampleRate = AudioLib.SampleRate.Hz11025;
                visitor.DisableAcmCodecs = true;
                visitor.EncodePublishedAudioFilesToMp3 = m_EncodeToMP3;

                m_Presentation.RootNode.AcceptDepthFirst(visitor);
                return publishChannel;
            }
            */

        public override void DoWork()
            {
                m_FilesList.Clear();
                Channel publishChannel = PublishAudioFiles ();

                if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            List<SectionNode> sectionsList = GetSectionsList ( m_Presentation.RootNode);

            if (RequestCancellation_RemovePublishChannel(publishChannel)) return;
            CreateFileSet ( sectionsList );

            //m_Presentation.ChannelsManager.RemoveManagedObject(publishChannel);
            RemovePublishChannel(publishChannel);
            }


        private void CreateFileSet ( List<SectionNode> sectionsList )
            {
            XmlDocument nccDocument = CreateNCCStubDocument ();

            m_IdCounter = 0;
            // initialize page counts to 0
            m_PageFrontCount = 0;
            m_PageNormalCount = 0;
            m_PageSpecialCount = 0;
            m_MaxPageNormal = 0;
            m_ExportedSectionCount = 0;

            // generate smil metadata dictionary.
            m_SmilMetadata = PopulateSmilMetadataDictionary ();
            m_SmilFile_TitleMap.Clear ();
            m_SmilElapseTime = new Time ();


            for (int i = 0; i < sectionsList.Count; i++)
                {
                try
                    {
                        if (m_MaxDepth < sectionsList[i].Level) m_MaxDepth = sectionsList[i].Level;
                    CreateElementsForSection ( nccDocument, sectionsList[i], i );
                    }
                catch (System.Exception ex)
                    {
                    MessageBox.Show ( ex.ToString () );
                    }
                }


            int tocItemsCount = m_ExportedSectionCount + m_PageFrontCount + m_PageNormalCount + m_PageSpecialCount;
            CreateNCCMetadata ( nccDocument, tocItemsCount.ToString () );

            // write ncc file
            WriteXmlDocumentToFile ( nccDocument,
                Path.Combine ( m_ExportDirectory, "ncc.html" ) );

            // create master.smil file
            XmlDocument smilDocument = CreateSmilStubDocument ();
            XmlNode bodyNode = smilDocument.GetElementsByTagName ( "body" )[0];

            foreach (string smilFileName in m_SmilFile_TitleMap.Keys)
                {
                XmlNode refNode = smilDocument.CreateElement ( null, "ref", bodyNode.NamespaceURI );
                bodyNode.AppendChild ( refNode );
                CreateAppendXmlAttribute ( smilDocument, refNode, "title", m_SmilFile_TitleMap[smilFileName] );
                CreateAppendXmlAttribute ( smilDocument, refNode, "src", smilFileName );
                CreateAppendXmlAttribute ( smilDocument, refNode, "id", "ms_" + Path.GetFileNameWithoutExtension ( smilFileName ) );
                }

            // add dc:title, this is mandatory for master.smil
            urakawa.metadata.Metadata titleMetadata = ((ObiPresentation) m_Presentation).GetFirstMetadataItem ( "dc:Title" );
            if (titleMetadata != null)
                {
                XmlNode masterSmilHeadNode = smilDocument.GetElementsByTagName ( "head" )[0];
                XmlNode smilTitleMetadataNode = smilDocument.CreateElement ( null, "meta", masterSmilHeadNode.NamespaceURI );
                masterSmilHeadNode.AppendChild ( smilTitleMetadataNode );
                CreateAppendXmlAttribute ( smilDocument, smilTitleMetadataNode, "name", "dc:title" );
                CreateAppendXmlAttribute ( smilDocument, smilTitleMetadataNode, "content", titleMetadata.NameContentAttribute.Value);
                }
            //AddSmilHeadElements ( smilDocument, null, m_SmilElapseTime.ToString ().Split ( '.' )[0] );
            AddSmilHeadElements ( smilDocument, null,GetStringTotalTimeRoundedOff( m_SmilElapseTime.AsTimeSpan)  );

            WriteXmlDocumentToFile ( smilDocument,
                Path.Combine ( m_ExportDirectory, "master.smil" ) );
            }

        private void CreateElementsForSection ( XmlDocument nccDocument, SectionNode section, int sectionIndex )
            {
            Time sectionDuration = new Time ();
            string nccFileName = "ncc.html";
            XmlNode bodyNode = nccDocument.GetElementsByTagName ( "body" )[0];
            XmlNode headingNode = nccDocument.CreateElement ( null, "h" + section.Level.ToString (), bodyNode.NamespaceURI );
            if (sectionIndex == 0)
                {
                CreateAppendXmlAttribute ( nccDocument, headingNode, "class", "title" );
                }
            else
                {
                CreateAppendXmlAttribute ( nccDocument, headingNode, "class", "section" );
                }
            string headingID = "h" + IncrementID;
            CreateAppendXmlAttribute ( nccDocument, headingNode, "id", headingID );
            bodyNode.AppendChild ( headingNode );

            // create smil document
            string smilNumericFrag = (sectionIndex + 1).ToString ().PadLeft ( 3, '0' );
            string smilFileName =smilNumericFrag + ".smil";
            XmlDocument smilDocument = CreateSmilStubDocument ();
            XmlNode smilBodyNode = smilDocument.GetElementsByTagName ( "body" )[0];

            // create main seq
            XmlNode mainSeq = smilDocument.CreateElement ( null, "seq", smilBodyNode.NamespaceURI );
            CreateAppendXmlAttribute ( smilDocument, mainSeq, "id", "sq" + IncrementID );

            // declare aseq node which is parent of audio elements
            XmlNode seqNode_AudioParent = null;
            // declare seq for heading phrase
            XmlNode seqNode_HeadingAudioParent = null;

            smilBodyNode.AppendChild ( mainSeq );
            bool isFirstPhrase = true;
            EmptyNode adjustedPageNode = m_NextSectionPageAdjustmentDictionary[section];
            bool isPreviousNodeEmptyPage = false;
            
            for (int i = 0; i < section.PhraseChildCount || adjustedPageNode != null; i++)
                {
                    EmptyNode phrase = null;
                //first handle the first phrase of the project if it is also the page
                // in such a case i=0 will be skipped being page, second phrase is exported and then first page is inserted to i=2 
                    if (i == 2 && m_FirstPageNumberedPhraseOfFirstSection != null)
                    {
                        phrase = m_FirstPageNumberedPhraseOfFirstSection;
                        m_FirstPageNumberedPhraseOfFirstSection = null;
                        --i;
                    }
                    else if (i < section.PhraseChildCount  ) 
                    {
                        phrase = section.PhraseChild(i);
                    }
                    else
                    {   
                        phrase = adjustedPageNode;
                        adjustedPageNode = null;
                    }
                    if (phrase.Role_ == EmptyNode.Role.Page && isFirstPhrase && i < section.PhraseChildCount) 
                    {   
                        continue;
            }

                if ((phrase is PhraseNode && phrase.Used)
                    || ( phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page  &&  phrase.Used))
                    {
                    
                    string pageID = null;
                    XmlNode pageNode = null;
                    if (!isFirstPhrase && phrase.Role_ == EmptyNode.Role.Page)
                        {
                        string strClassVal = null;
                        // increment page counts and get page kind
                        switch (phrase.PageNumber.Kind)
                            {
                        case PageKind.Front:
                        m_PageFrontCount++;
                        strClassVal = "page-front";
                        break;

                        case PageKind.Normal:
                        m_PageNormalCount++;
                        if (phrase.PageNumber.Number > m_MaxPageNormal) m_MaxPageNormal = phrase.PageNumber.Number;
                        strClassVal = "page-normal";
                        break;

                        case PageKind.Special:
                        m_PageSpecialCount++;
                        strClassVal = "page-special";
                        break;

                            }

                        pageNode = nccDocument.CreateElement ( null, "span", bodyNode.NamespaceURI );
                        CreateAppendXmlAttribute ( nccDocument, pageNode, "class", strClassVal );
                        pageID = "p" + IncrementID;
                        CreateAppendXmlAttribute ( nccDocument, pageNode, "id", pageID );
                        bodyNode.AppendChild ( pageNode );

                        }

                    // create smil nodes

                    // if phrase node is first phrase of section or is page node then create par and text
                    if (isFirstPhrase
                        || phrase.Role_ == EmptyNode.Role.Page
                        || isPreviousNodeEmptyPage)
                        {
                        // create par 
                        XmlNode parNode = smilDocument.CreateElement ( null, "par", smilBodyNode.NamespaceURI );
                        mainSeq.AppendChild ( parNode );
                        CreateAppendXmlAttribute ( smilDocument, parNode, "endsync", "last" );
                        CreateAppendXmlAttribute ( smilDocument, parNode, "id", "pr" + IncrementID );

                        XmlNode txtNode = smilDocument.CreateElement ( null, "text", smilBodyNode.NamespaceURI );
                        parNode.AppendChild ( txtNode );
                        string txtID = "txt" + IncrementID;
                        CreateAppendXmlAttribute ( smilDocument, txtNode, "id", txtID );

                        if (isFirstPhrase || isPreviousNodeEmptyPage)
                            {
                            CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + "#" + headingID );
                            }
                        else if (pageNode != null)
                            {
                            CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + "#" + pageID );
                            }

                        // create seq which will hol audio children 
                            if (phrase is PhraseNode)
                            {
                                seqNode_AudioParent = smilDocument.CreateElement(null, "seq", smilBodyNode.NamespaceURI);
                                parNode.AppendChild(seqNode_AudioParent);
                                // hold seq for heading phrase in a variable.
                                if (isFirstPhrase)
                                {
                                    seqNode_HeadingAudioParent = seqNode_AudioParent;
                                }
                                CreateAppendXmlAttribute(smilDocument, seqNode_AudioParent, "id", "sq" + IncrementID);
                            }

                        // add anchor and href to ncc elements
                        XmlNode anchorNode = nccDocument.CreateElement ( null, "a", bodyNode.NamespaceURI );

                        if (isFirstPhrase)
                            {
                            headingNode.AppendChild ( anchorNode );
                            CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );
                            anchorNode.AppendChild (
                                nccDocument.CreateTextNode ( section.Label ) );
                            }
                        else if (pageNode != null)
                            {

                            pageNode.AppendChild ( anchorNode );
                            CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );

                            anchorNode.AppendChild (
                                nccDocument.CreateTextNode ( phrase.PageNumber.ToString () ) );

                            }
                        }

                    // create audio elements for external audio medias
                        if (phrase is PhraseNode)
                        {
                            Channel publishChannel = m_Presentation.ChannelsManager.GetChannelsByName(urakawa.daisy.export.Daisy3_Export.PUBLISH_AUDIO_CHANNEL_NAME)[0];
                            ExternalAudioMedia externalMedia = (ExternalAudioMedia)phrase.GetProperty<ChannelsProperty>().GetMedia(publishChannel);

                            XmlNode audioNode = smilDocument.CreateElement(null, "audio", smilBodyNode.NamespaceURI);
                            seqNode_AudioParent.AppendChild(audioNode);
                            string relativeSRC = AddSectionNameToAudioFile? 
                                AddSectionNameToAudioFileName(externalMedia.Src, phrase.ParentAs<SectionNode>().Label): 
                                Path.GetFileName(externalMedia.Src);
                            CreateAppendXmlAttribute(smilDocument, audioNode, "src", relativeSRC);
                            CreateAppendXmlAttribute(smilDocument, audioNode, "clip-begin",
                                GetNPTSmiltime(externalMedia.ClipBegin.AsTimeSpan));
                            CreateAppendXmlAttribute(smilDocument, audioNode, "clip-end",
                                GetNPTSmiltime(externalMedia.ClipEnd.AsTimeSpan));
                            CreateAppendXmlAttribute(smilDocument, audioNode, "id", "aud" + IncrementID);
                            sectionDuration.Add(externalMedia.Duration);
                            if (!m_FilesList.Contains(relativeSRC)) m_FilesList.Add(relativeSRC);

                    // copy audio element if phrase has  heading role and is not first phrase
                    if (phrase.Role_ == EmptyNode.Role.Heading && !isFirstPhrase)
                        {
                        XmlNode audioNodeCopy = audioNode.Clone ();
                        audioNodeCopy.Attributes.GetNamedItem ( "id" ).Value = "aud" + IncrementID;
                        seqNode_HeadingAudioParent.PrependChild ( audioNodeCopy );
                        sectionDuration.Add ( externalMedia.Duration );
                        }
                }//Check for audio containing phrase ends
                    isFirstPhrase = false;

                    if (phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page)
                    {
                        isPreviousNodeEmptyPage = true;
                    }
                    else
                    {
                        isPreviousNodeEmptyPage = false;
                    }
                    }// if for phrasenode ends
                } // for loop ends

            // check if heading node have some children else remove it.
            // it is possible that all phrases are empty phrases or unused phrases, so there are no anchor children of heading node.
            if (headingNode.ChildNodes.Count == 0)
                {
                bodyNode.RemoveChild ( headingNode );
                }
            else
                {
                // first increment exported section count
                m_ExportedSectionCount++;

                string strDurTime = TruncateTimeToDecimalPlaces ( sectionDuration.AsTimeSpan.TotalSeconds.ToString (), 3 );
                //string strDurTime = Math.Round ( sectionDuration.getTime ().TotalSeconds, 3, MidpointRounding.ToEven).ToString ();
                strDurTime = strDurTime + "s";
                CreateAppendXmlAttribute ( smilDocument, mainSeq, "dur", strDurTime );

                //AddSmilHeadElements ( smilDocument, m_SmilElapseTime.ToString (), sectionDuration.ToString () );
                AddSmilHeadElements(smilDocument, AdjustTimeStringForDay( m_SmilElapseTime.AsTimeSpan) , 
                    AdjustTimeStringForDay( sectionDuration.AsTimeSpan ));
                m_SmilElapseTime.Add( sectionDuration );
                m_SmilFile_TitleMap.Add ( smilFileName, section.Label );

                WriteXmlDocumentToFile ( smilDocument,
                    Path.Combine ( m_ExportDirectory, smilFileName ) );
                if (!m_FilesList.Contains(smilFileName)) m_FilesList.Add(smilFileName);
                }
            }

        private string IncrementID { get { return (++m_IdCounter).ToString (); } }

        private string GetNPTSmiltime ( TimeSpan time )
            {
            double dTime = Math.Round ( time.TotalSeconds, 3, MidpointRounding.ToEven );
            string strTime = "npt=" + dTime.ToString () + "s";
            return strTime;
            }


        private XmlAttribute CreateAppendXmlAttribute ( XmlDocument xmlDoc, XmlNode node, string name, string val )
            {
            XmlAttribute attr = xmlDoc.CreateAttribute ( name );
            attr.Value = val;
            node.Attributes.Append ( attr );
            return attr;
            }

        private void AddSmilHeadElements ( XmlDocument smilDocument, string elapseTime , string timeInSmil )
            {
            XmlNode headNode = smilDocument.GetElementsByTagName ( "head" )[0];

            // add metadata from dictionary
            foreach (string s in m_SmilMetadata.Keys)
                {
                XmlNode metaNode = smilDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                headNode.AppendChild ( metaNode );

                CreateAppendXmlAttribute ( smilDocument, metaNode, "name", s );
                CreateAppendXmlAttribute ( smilDocument, metaNode, "content", m_SmilMetadata[s] );
                }
            // add dc:format
            XmlNode formatNode = smilDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( formatNode );
            CreateAppendXmlAttribute ( smilDocument, formatNode, "name", "dc:format" );
            CreateAppendXmlAttribute ( smilDocument, formatNode, "content", "Daisy 2.02" );

            if (elapseTime != null)
                {
                XmlNode metaNode1 = smilDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                headNode.AppendChild ( metaNode1 );
                CreateAppendXmlAttribute ( smilDocument, metaNode1, "name", "ncc:totalElapsedTime" );
                CreateAppendXmlAttribute ( smilDocument, metaNode1, "content", elapseTime );
                }

            if (timeInSmil != null)
                {
                XmlNode metaNode1 = smilDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                headNode.AppendChild ( metaNode1 );
                CreateAppendXmlAttribute ( smilDocument, metaNode1, "name", "ncc:timeInThisSmil" );
                CreateAppendXmlAttribute ( smilDocument, metaNode1, "content", timeInSmil );
                }

            XmlNode layoutNode = smilDocument.CreateElement ( null, "layout", headNode.NamespaceURI );
            headNode.AppendChild ( layoutNode );
            XmlNode regionNode = smilDocument.CreateElement ( null, "region", headNode.NamespaceURI );
            layoutNode.AppendChild ( regionNode );
            regionNode.Attributes.Append (
                smilDocument.CreateAttribute ( "id" ) );
            regionNode.Attributes.GetNamedItem ( "id" ).Value = "textView";

            }


        private XmlDocument CreateNCCStubDocument ()
            {
            XmlDocument nccDocument = new XmlDocument ();
            nccDocument.XmlResolver = null;

            nccDocument.CreateXmlDeclaration ( "1.0", "utf-8", null );
            nccDocument.AppendChild ( nccDocument.CreateDocumentType ( "html",
                "-//W3C//DTD XHTML 1.0 Transitional//EN",
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                null ) );

            XmlNode htmlNode = nccDocument.CreateElement ( null,
                "html",
                "http://www.w3.org/1999/xhtml" );

            nccDocument.AppendChild ( htmlNode );


            CreateAppendXmlAttribute ( nccDocument, htmlNode, "lang", "en" );
            CreateAppendXmlAttribute ( nccDocument, htmlNode, "xml:lang", "en" );


            XmlNode headNode = nccDocument.CreateElement ( null, "head", htmlNode.NamespaceURI );
            htmlNode.AppendChild ( headNode );
            XmlNode bodyNode = nccDocument.CreateElement ( null, "body", htmlNode.NamespaceURI );
            htmlNode.AppendChild ( bodyNode );

            return nccDocument;
            }

        private XmlDocument CreateSmilStubDocument ()
            {
            XmlDocument smilDocument = new XmlDocument ();
            smilDocument.XmlResolver = null;

            smilDocument.AppendChild ( smilDocument.CreateXmlDeclaration ( "1.0", "utf-8", null ) );
            smilDocument.AppendChild ( smilDocument.CreateDocumentType ( "smil",
                "-//W3C//DTD SMIL 1.0//EN",
                    "http://www.w3.org/TR/REC-smil/SMIL10.dtd",
                null ) );
            XmlNode smilRootNode = smilDocument.CreateElement ( null,
                "smil", null );

            //"http://www.w3.org/1999/xhtml" );
            smilDocument.AppendChild ( smilRootNode );

            XmlNode headNode = smilDocument.CreateElement ( null, "head", smilRootNode.NamespaceURI );
            smilRootNode.AppendChild ( headNode );
            XmlNode bodyNode = smilDocument.CreateElement ( null, "body", smilRootNode.NamespaceURI );
            smilRootNode.AppendChild ( bodyNode );

            return smilDocument;
            }


        /// <summary>
        /// write xml document in file passed as parameter
        /// </summary>
        /// <param name="xmlDoc"></param>
        /// <param name="path"></param>
        public void WriteXmlDocumentToFile ( XmlDocument xmlDoc, string path )
            {
            XmlTextWriter writer = null;
            try
                {
                if (!File.Exists ( path ))
                    {
                    File.Create ( path ).Close ();
                    }

                writer = new XmlTextWriter ( path, null );
                writer.Formatting = Formatting.Indented;
                xmlDoc.Save ( writer );
                }
            finally
                {
                writer.Close ();
                writer = null;
                }
            }


        private void CreateNCCMetadata ( XmlDocument nccDocument, string tocItems )
            {

            List<urakawa.metadata.Metadata> items = m_Presentation.Metadatas.ContentsAs_ListCopy;
            items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
            {
                int names = a.NameContentAttribute.Name.CompareTo(b.NameContentAttribute.Name);
                return names == 0 ? a.NameContentAttribute.Value.CompareTo(b.NameContentAttribute.Value) : names;
            } );


            XmlNode headNode = nccDocument.GetElementsByTagName ( "head" )[0];

            // create first meta node
            XmlNode firstMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( firstMetaNode );
            CreateAppendXmlAttribute ( nccDocument, firstMetaNode, "content", "text/html; charset=utf-8" );
            CreateAppendXmlAttribute ( nccDocument, firstMetaNode, "http-equiv", "Content-type" );
            //<meta content="text/html; charset=utf-8" http-equiv="Content-type"/>

            ObiPresentation presentation = (ObiPresentation)m_Presentation;
            urakawa.metadata.Metadata titleMetadata = presentation.GetFirstMetadataItem ( "dc:Title" );
            if (titleMetadata != null)
                {
                XmlNode titleMetadataNode = nccDocument.CreateElement ( null, "title", headNode.NamespaceURI );
                headNode.AppendChild ( titleMetadataNode );
                titleMetadataNode.AppendChild (
                    nccDocument.CreateTextNode(titleMetadata.NameContentAttribute.Value));
                }

            // add dc:date from produced date in case dc:date do not exists
                urakawa.metadata.Metadata producedDateMetadata = presentation.GetFirstMetadataItem(Metadata.DTB_PRODUCED_DATE);
                if (presentation.GetFirstMetadataItem(Metadata.DC_DATE) == null
                &&    producedDateMetadata != null)
                {
                XmlNode dateMetadataNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                headNode.AppendChild ( dateMetadataNode );
                CreateAppendXmlAttribute ( nccDocument, dateMetadataNode, "name",  "dc:date");
                CreateAppendXmlAttribute(nccDocument, dateMetadataNode, "content", producedDateMetadata.NameContentAttribute.Value);
                }


            // add existing metadata items to nccn
            foreach (urakawa.metadata.Metadata m in items)
                {
                    if (m_MetadataMap.ContainsKey(m.NameContentAttribute.Name))
                    {
                    XmlNode metaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                    headNode.AppendChild ( metaNode );
                    CreateAppendXmlAttribute(nccDocument, metaNode, "name", m_MetadataMap[m.NameContentAttribute.Name]);
                    CreateAppendXmlAttribute(nccDocument, metaNode, "content", m.NameContentAttribute.Value);
                    }
                }

            // to do add more metadata items.
            // add dc:format
            XmlNode formatNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( formatNode );
            CreateAppendXmlAttribute ( nccDocument, formatNode, "name", "dc:format" );
            CreateAppendXmlAttribute ( nccDocument, formatNode, "content", "Daisy 2.02" );

            // ncc:charset
            XmlNode charsetNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( charsetNode );
            CreateAppendXmlAttribute ( nccDocument, charsetNode, "name", "ncc:charset" );
            CreateAppendXmlAttribute ( nccDocument, charsetNode, "content", "utf-8" );


            XmlNode pageMetaNode = null;
            // ncc:pageFront
            pageMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( pageMetaNode );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "name", "ncc:pageFront" );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "content", m_PageFrontCount.ToString () );

            //  ncc:maxPageNormal and pageNormal
            pageMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( pageMetaNode );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "name", "ncc:maxPageNormal" );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "content", m_MaxPageNormal.ToString () );

            // ncc:pageNormal
            pageMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( pageMetaNode );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "name", "ncc:pageNormal" );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "content", m_PageNormalCount.ToString () );


            // ncc:pageSpecial
            pageMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( pageMetaNode );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "name", "ncc:pageSpecial" );
            CreateAppendXmlAttribute ( nccDocument, pageMetaNode, "content", m_PageSpecialCount.ToString () );


            // ncc:prodNotes
            // ncc:tocItems
            XmlNode tocItemNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( tocItemNode );
            CreateAppendXmlAttribute ( nccDocument, tocItemNode, "name", "ncc:tocItems" );
            CreateAppendXmlAttribute ( nccDocument, tocItemNode, "content", tocItems );

            // ncc:totalTime
            XmlNode totalTimeNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( totalTimeNode );
            CreateAppendXmlAttribute ( nccDocument, totalTimeNode, "name", "ncc:totalTime" );
            //CreateAppendXmlAttribute ( nccDocument, totalTimeNode, "content", m_SmilElapseTime.ToString ().Split ( '.' )[0] );
            CreateAppendXmlAttribute ( nccDocument, totalTimeNode, "content", GetStringTotalTimeRoundedOff ( m_SmilElapseTime.AsTimeSpan ) );

            XmlNode multimediaType  = nccDocument.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(multimediaType);
            CreateAppendXmlAttribute(nccDocument, multimediaType, "name", "ncc:multimediaType");
            CreateAppendXmlAttribute(nccDocument, multimediaType, "content", "audioNcc");

            // ncc:depth
            XmlNode depthNode = nccDocument.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(depthNode);
            CreateAppendXmlAttribute(nccDocument, depthNode, "name", "ncc:depth");
            CreateAppendXmlAttribute(nccDocument, depthNode, "content", m_MaxDepth.ToString());

            // ncc:files
            int filesCount = m_FilesList.Count + 2;
            XmlNode filesCountNode = nccDocument.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(filesCountNode);
            CreateAppendXmlAttribute(nccDocument, filesCountNode, "name", "ncc:files");
            CreateAppendXmlAttribute(nccDocument, filesCountNode, "content", filesCount.ToString());
        
        }

        private Dictionary<string, string> PopulateSmilMetadataDictionary ()
            {
            Dictionary<string, string> metadataItems = new Dictionary<string, string> ();
            /*
            List<urakawa.metadata.Metadata> items = m_Presentation.getListOfMetadata ();
            items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
            {
                int names = a.getName ().CompareTo ( b.getName () );
                return names == 0 ? a.getContent ().CompareTo ( b.getContent () ) : names;
            } );


            XmlNode headNode = nccDocument.GetElementsByTagName ( "head" )[0];
            // add existing metadata items to ncc
            foreach (urakawa.metadata.Metadata m in items)
                {
                if ( metadataMap.ContainsKey(m.getName () ))
                    {
                    */
            ObiPresentation presentation = (ObiPresentation)m_Presentation;
            urakawa.metadata.Metadata identifier = presentation.GetFirstMetadataItem("dc:Identifier");
            if (identifier != null)
                {
                    metadataItems.Add(m_MetadataMap[identifier.NameContentAttribute.Name],
               identifier.NameContentAttribute.Value);
                }

                urakawa.metadata.Metadata format = presentation.GetFirstMetadataItem("dc:Format");
            if (format != null)
                {
                    metadataItems.Add(m_MetadataMap[format.NameContentAttribute.Name],
               format.NameContentAttribute.Value);
                }


                urakawa.metadata.Metadata generator = presentation.GetFirstMetadataItem("generator");
            if (generator != null)
                {
                    metadataItems.Add(m_MetadataMap[generator.NameContentAttribute.Name],
               generator.NameContentAttribute.Value);
                }


            return metadataItems;
            }


        private Dictionary<string, string> CreateDAISY3To2MetadataMap ()
            {
            Dictionary<string, string> MetadataMap = new Dictionary<string, string> ();


            MetadataMap.Add ( "dc:Contributor", "dc:contributor" );
            MetadataMap.Add ( "dc:Coverage", "dc:coverage" );
            MetadataMap.Add ( "dc:Creator", "dc:creator" );
            MetadataMap.Add ( "dc:Date", "dc:date" );
            MetadataMap.Add ( "dc:Description", "dc:description" );
            MetadataMap.Add ( "dc:Format", "dc:format" );
            MetadataMap.Add ( "dc:Identifier", "dc:identifier" );
            MetadataMap.Add ( "dc:Language", "dc:language" );
            MetadataMap.Add ( "dc:Publisher", "dc:publisher" );
            MetadataMap.Add ( "dc:Source", "dc:source" );
            MetadataMap.Add ( "dc:Subject", "dc:subject" );
            MetadataMap.Add ( "dc:Title", "dc:title" );
            MetadataMap.Add ( "dc:Type", "dc:type" );
            MetadataMap.Add ( "dc:Relation", "dc:relation" );
            MetadataMap.Add ( "dc:Rights", "dc:rights" );
            // X-Metadata
            MetadataMap.Add ( "dtb:sourceDate", "ncc:sourceDate" );
            MetadataMap.Add ( "dtb:sourceEdition", "ncc:sourceEdition" );
            MetadataMap.Add ( "dtb:sourcePublisher", "ncc:sourcePublisher" );
            MetadataMap.Add ( "dtb:sourceRights", "ncc:sourceRights" );
            MetadataMap.Add ( "dtb:sourceTitle", "ncc:sourceTitle" );
            MetadataMap.Add ( "dtb:multimediaType", "ncc:multimediaType" );
            //MetadataMap.Add ("dtb:multimediaContent";
            MetadataMap.Add ( "dtb:narrator", "ncc:narrator" );
            MetadataMap.Add ( "dtb:producer", "ncc:producer" );
            MetadataMap.Add ( "dtb:producedDate", "ncc:producedDate" );
            MetadataMap.Add ( "dtb:revision", "ncc:revision" );
            MetadataMap.Add ( "dtb:revisionDate", "ncc:revisionDate" );
            //MetadataMap.Add ("dtb:revisionDescription";
            //MetadataMap.Add ( "dtb:totalTime", "ncc:totalTime" ); // its commented because total time should be generated while export
            //MetadataMap.Add ("dtb:audioFormat";

            MetadataMap.Add ( "generator", "ncc:generator" );
            //MetadataMap.Add ( "obi:xukversion", "obi:xukversion" );

            return MetadataMap;
            }

        private string TruncateTimeToDecimalPlaces ( string strTime, int decimalPlaces )
            {
            int decimalIndex = strTime.IndexOf ( "." );
            if (strTime.Length > decimalIndex + decimalPlaces + 1)
                {
                if (decimalIndex == 0)
                    return strTime.Split ( '.' )[0];

                strTime = strTime.Substring ( 0, decimalIndex + decimalPlaces + 1 );
                }
            return strTime;
            }

        private string GetStringTotalTimeRoundedOff ( TimeSpan time )
            {
            string strMS = time.Milliseconds.ToString ();
            int compareDigit  = 0 ;
            int.TryParse( strMS.Substring ( 0, 1 ) , out compareDigit);
            if (compareDigit >= 5)
                {
                TimeSpan additiveSpan = new TimeSpan ( Convert.ToInt64 ( .5 * 10000000 ) );
                time = time.Add ( additiveSpan );
                }
                if (time.Days >= 1)
                {
                    return AdjustTimeStringForDay(time).Split('.')[0];
                }
                else
                {
                    return time.ToString().Split('.')[0];
                }
            }

        private string AdjustTimeStringForDay(TimeSpan time)
        {
            if (time.Days >= 1)
            {
                string [] timeStringArray = time    .ToString().Split ( ':' ) ;
                string strTIME = time.TotalHours.ToString().Split('.')[0];
                for ( int i  = 1 ; i < timeStringArray.Length ; i++ )
                {
                    strTIME += ":" + timeStringArray[i];
                }
                return TruncateTimeToDecimalPlaces (strTIME,3);
            }
            else
            {
                return time.ToString () ;
            }
        }

    }
    }
