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

namespace Obi.Export
    {
    public enum ExportFormat { DAISY3_0, DAISY2_02 } ;

    public class DAISY202Export
        {

        private Presentation m_Presentation;
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


        public DAISY202Export ( Presentation presentation, string exportDirectory )
            {
            m_Presentation = presentation;
            m_ExportDirectory = exportDirectory;
            m_MetadataMap = CreateDAISY3To2MetadataMap ();
            m_SmilFile_TitleMap = new Dictionary<string, string> ();
            }


        List<SectionNode> GetSectionsList ( RootNode rNode )
            {
            List<SectionNode> sectionsList = new List<SectionNode> ();
            rNode.acceptDepthFirst (
                    delegate ( urakawa.core.TreeNode n )
                        {

                        if (n is SectionNode && ((SectionNode)n).Used)
                            {

                            sectionsList.Add ( (SectionNode)n );
                            }
                        return true;
                        },
                    delegate ( urakawa.core.TreeNode n ) { } );

            return sectionsList;
            }

        public void CreateDAISY202Files ()
            {
            List<SectionNode> sectionsList = GetSectionsList ( m_Presentation.RootNode );

            CreateFileSet ( sectionsList );

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
            urakawa.metadata.Metadata titleMetadata = m_Presentation.GetFirstMetadataItem ( "dc:Title" );
            if (titleMetadata != null)
                {
                XmlNode masterSmilHeadNode = smilDocument.GetElementsByTagName ( "head" )[0];
                XmlNode smilTitleMetadataNode = smilDocument.CreateElement ( null, "meta", masterSmilHeadNode.NamespaceURI );
                masterSmilHeadNode.AppendChild ( smilTitleMetadataNode );
                CreateAppendXmlAttribute ( smilDocument, smilTitleMetadataNode, "name", "dc:title" );
                CreateAppendXmlAttribute ( smilDocument, smilTitleMetadataNode, "content", titleMetadata.getContent () );
                }
            //AddSmilHeadElements ( smilDocument, null, m_SmilElapseTime.ToString ().Split ( '.' )[0] );
            AddSmilHeadElements ( smilDocument, null,GetStringTotalTimeRoundedOff( m_SmilElapseTime.copy().getTime ())  );

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

            for (int i = 0; i < section.PhraseChildCount; i++)
                {
                if (section.PhraseChild ( i ) is PhraseNode
                    && section.PhraseChild ( i ).Used)
                    {
                    PhraseNode phrase = (PhraseNode)section.PhraseChild ( i );

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
                        || phrase.Role_ == EmptyNode.Role.Page)
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

                        if (isFirstPhrase)
                            {
                            CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + "#" + headingID );
                            }
                        else if (pageNode != null)
                            {
                            CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + "#" + pageID );
                            }

                        // create seq which will hol audio children 
                        seqNode_AudioParent = smilDocument.CreateElement ( null, "seq", smilBodyNode.NamespaceURI );
                        parNode.AppendChild ( seqNode_AudioParent );
                        // hold seq for heading phrase in a variable.
                        if (isFirstPhrase)
                            {
                            seqNode_HeadingAudioParent = seqNode_AudioParent;
                            }
                        CreateAppendXmlAttribute ( smilDocument, seqNode_AudioParent, "id", "sq" + IncrementID );


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
                    //(ManagedAudioMedia)getProperty<ChannelsProperty>().getMedia(Presentation.AudioChannel)
                    //getProperty<ChannelsProperty>().getMedia(Presentation.PUBLISH_AUDIO_CHANNEL_NAME)
                    Channel publishChannel = m_Presentation.GetSingleChannelByName ( Presentation.PUBLISH_AUDIO_CHANNEL_NAME );
                    ExternalAudioMedia externalMedia = (ExternalAudioMedia)phrase.getProperty<ChannelsProperty> ().getMedia ( publishChannel );

                    XmlNode audioNode = smilDocument.CreateElement ( null, "audio", smilBodyNode.NamespaceURI );
                    seqNode_AudioParent.AppendChild ( audioNode );
                    string relativeSRC = Path.GetFileName ( externalMedia.getSrc () );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "src", relativeSRC );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "clip-begin",
                        GetNPTSmiltime ( externalMedia.getClipBegin ().getTime () ) );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "clip-end",
                        GetNPTSmiltime ( externalMedia.getClipEnd ().getTime () ) );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "id", "aud" + IncrementID );
                    sectionDuration = sectionDuration.addTimeDelta ( externalMedia.getDuration () );

                    // copy audio element if phrase has  heading role and is not first phrase
                    if (phrase.Role_ == EmptyNode.Role.Heading && !isFirstPhrase)
                        {
                        XmlNode audioNodeCopy = audioNode.Clone ();
                        audioNodeCopy.Attributes.GetNamedItem ( "id" ).Value = "aud" + IncrementID;
                        seqNode_HeadingAudioParent.PrependChild ( audioNodeCopy );
                        sectionDuration = sectionDuration.addTimeDelta ( externalMedia.getDuration () );
                        }
                    isFirstPhrase = false;
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

                string strDurTime = TruncateTimeToDecimalPlaces ( sectionDuration.getTime ().TotalSeconds.ToString (), 3 );
                //string strDurTime = Math.Round ( sectionDuration.getTime ().TotalSeconds, 3, MidpointRounding.ToEven).ToString ();
                strDurTime = strDurTime + "s";
                CreateAppendXmlAttribute ( smilDocument, mainSeq, "dur", strDurTime );

                AddSmilHeadElements ( smilDocument, m_SmilElapseTime.ToString (), sectionDuration.ToString () );
                m_SmilElapseTime = m_SmilElapseTime.addTime ( sectionDuration );
                m_SmilFile_TitleMap.Add ( smilFileName, section.Label );

                WriteXmlDocumentToFile ( smilDocument,
                    Path.Combine ( m_ExportDirectory, smilFileName ) );
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

            List<urakawa.metadata.Metadata> items = m_Presentation.getListOfMetadata ();
            items.Sort ( delegate ( urakawa.metadata.Metadata a, urakawa.metadata.Metadata b )
            {
                int names = a.getName ().CompareTo ( b.getName () );
                return names == 0 ? a.getContent ().CompareTo ( b.getContent () ) : names;
            } );


            XmlNode headNode = nccDocument.GetElementsByTagName ( "head" )[0];

            // create first meta node
            XmlNode firstMetaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
            headNode.AppendChild ( firstMetaNode );
            CreateAppendXmlAttribute ( nccDocument, firstMetaNode, "content", "text/html; charset=utf-8" );
            CreateAppendXmlAttribute ( nccDocument, firstMetaNode, "http-equiv", "Content-type" );
            //<meta content="text/html; charset=utf-8" http-equiv="Content-type"/>

            urakawa.metadata.Metadata titleMetadata = m_Presentation.GetFirstMetadataItem ( "dc:Title" );
            if (titleMetadata != null)
                {
                XmlNode titleMetadataNode = nccDocument.CreateElement ( null, "title", headNode.NamespaceURI );
                headNode.AppendChild ( titleMetadataNode );
                titleMetadataNode.AppendChild (
                    nccDocument.CreateTextNode ( titleMetadata.getContent () ) );
                }

            // add dc:date from produced date in case dc:date do not exists
            urakawa.metadata.Metadata producedDateMetadata = m_Presentation.GetFirstMetadataItem (Metadata.DTB_PRODUCED_DATE  );
            if ( m_Presentation.GetFirstMetadataItem ( Metadata.DC_DATE ) == null
                &&    producedDateMetadata != null)
                {
                XmlNode dateMetadataNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                headNode.AppendChild ( dateMetadataNode );
                CreateAppendXmlAttribute ( nccDocument, dateMetadataNode, "name",  "dc:date");
                CreateAppendXmlAttribute ( nccDocument, dateMetadataNode,"content",  producedDateMetadata.getContent ()  );
                }


            // add existing metadata items to nccn
            foreach (urakawa.metadata.Metadata m in items)
                {
                if (m_MetadataMap.ContainsKey ( m.getName () ))
                    {
                    XmlNode metaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                    headNode.AppendChild ( metaNode );
                    CreateAppendXmlAttribute ( nccDocument, metaNode, "name", m_MetadataMap[m.getName ()] );
                    CreateAppendXmlAttribute ( nccDocument, metaNode, "content", m.getContent () );
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
            CreateAppendXmlAttribute ( nccDocument, totalTimeNode, "content", GetStringTotalTimeRoundedOff ( m_SmilElapseTime.copy().getTime () ) );

            XmlNode multimediaType  = nccDocument.CreateElement(null, "meta", headNode.NamespaceURI);
            headNode.AppendChild(multimediaType);
            CreateAppendXmlAttribute(nccDocument, multimediaType, "name", "ncc:multimediaType");
            CreateAppendXmlAttribute(nccDocument, multimediaType, "content", "audioNcc");

        
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
            urakawa.metadata.Metadata identifier = m_Presentation.GetFirstMetadataItem ( "dc:Identifier" );
            if (identifier != null)
                {
                metadataItems.Add ( m_MetadataMap[identifier.getName ()],
               identifier.getContent () );
                }

            urakawa.metadata.Metadata format = m_Presentation.GetFirstMetadataItem ( "dc:Format" );
            if (format != null)
                {
                metadataItems.Add ( m_MetadataMap[format.getName ()],
               format.getContent () );
                }


            urakawa.metadata.Metadata generator = m_Presentation.GetFirstMetadataItem ( "generator" );
            if (generator != null)
                {
                metadataItems.Add ( m_MetadataMap[generator.getName ()],
               generator.getContent () );
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
            MetadataMap.Add ( "dtb:totalTime", "ncc:totalTime" );
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
            return time.ToString ().Split ( '.' )[0];
            }

        }
    }
