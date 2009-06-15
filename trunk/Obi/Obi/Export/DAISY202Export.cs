using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

using urakawa.core;
using urakawa.media;
using urakawa.property.channel;
using urakawa.metadata;

namespace Obi.Export
    {
    public class DAISY202Export
        {
        private Presentation m_Presentation;
        private string m_ExportDirectory;
        private int m_PageFrontCount;
        private int m_PageNormalCount;
        private int m_PageSpecialCount;
        int m_IdCounter;

        public DAISY202Export ( Presentation presentation, string exportDirectory)
            {
            m_Presentation = presentation;
            m_ExportDirectory = exportDirectory;

            }


        List<SectionNode> GetSectionsList ( RootNode rNode )
            {
            List<SectionNode> sectionsList = new List<SectionNode> () ;
            rNode.acceptDepthFirst(
                    delegate(urakawa.core.TreeNode n)
                    {
                    
                        if (n is SectionNode && ((SectionNode)n).Used )
                            {
                            
                            sectionsList.Add ( (SectionNode) n ) ;
                            }
                        return true;
                    },
                    delegate(urakawa.core.TreeNode n) { });
                
                return sectionsList ;
            }

        public void CreateDAISY202Files ()
            {
            List<SectionNode> sectionsList = GetSectionsList ( m_Presentation.RootNode );
            MessageBox.Show ( "starting ncc main function" );
            CreateFileSet ( sectionsList );
            }


        private void CreateFileSet ( List<SectionNode> sectionsList )
            {
            XmlDocument nccDocument = CreateNCCStubDocument ();
            
            // initialize page counts to 0
            m_PageFrontCount = 0;
            m_PageNormalCount = 0;
            m_PageSpecialCount = 0;

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

            // to do: change class of first heading to title
            CreateNCCMetadata ( nccDocument );
            // to do: write ncc file
            WriteXmlDocumentToFile (nccDocument,
                Path.Combine ( m_ExportDirectory,"ncc.html")  );
            }
        
        private void CreateElementsForSection ( XmlDocument nccDocument, SectionNode section, int sectionIndex)
            {
            
            string nccFileName = "ncc.html";
            XmlNode bodyNode = nccDocument.GetElementsByTagName ( "body" )[0];
            XmlNode headingNode = nccDocument.CreateElement (null, "h" + section.Level.ToString () , bodyNode.NamespaceURI);
            CreateAppendXmlAttribute ( nccDocument, headingNode, "class", "section" );
            string headingID = "h" + IncrementID;
            CreateAppendXmlAttribute ( nccDocument, headingNode, "id", headingID);
            bodyNode.AppendChild ( headingNode );

            // create smil document
            string smilFileName = sectionIndex.ToString ()+ ".smil" ;
            XmlDocument smilDocument = CreateSmilStubDocument ();
            XmlNode smilBodyNode = smilDocument.GetElementsByTagName ( "body" )[0];

            // create main seq
            XmlNode mainSeq = smilDocument.CreateElement ( null, "seq", smilBodyNode.NamespaceURI );
            // declare aseq node which is parent of audio elements
            XmlNode seqNode_AudioParent= null;
            smilBodyNode.AppendChild ( mainSeq );

            for (int i = 0; i < section.PhraseChildCount; i++)
                {
                if (section.PhraseChild ( i ) is PhraseNode 
                    && section.PhraseChild ( i ).Used)
                    {
                    PhraseNode phrase = (PhraseNode)section.PhraseChild ( i );

                    string pageID= null;
                    XmlNode pageNode = null ;
                    if (phrase.Role_ == EmptyNode.Role.Page)
                        {
                        pageNode = nccDocument.CreateElement ( null, "span", bodyNode.NamespaceURI );
                        CreateAppendXmlAttribute ( nccDocument, pageNode, "type", phrase.PageNumber.Kind + "-page" );
                        pageID = "p" + IncrementID;
                        CreateAppendXmlAttribute ( nccDocument, pageNode, "id", pageID );
                        bodyNode.AppendChild ( pageNode );
                        }

                    // create smil nodes
                    
                    // if phrase node is first phrase of section or is page node then create par and text
                    if (phrase.Index == 0
                        || phrase.Role_ == EmptyNode.Role.Page)
                        {
                        // create par 
                        XmlNode parNode = smilDocument.CreateElement ( null, "par", smilBodyNode.NamespaceURI );
                        mainSeq.AppendChild ( parNode );

                        XmlNode txtNode = smilDocument.CreateElement ( null, "text", smilBodyNode.NamespaceURI );
                        parNode.AppendChild ( txtNode );
                        string txtID = "txt" + IncrementID ;
                        CreateAppendXmlAttribute ( smilDocument, txtNode, "id",txtID );
                        if ( pageID != null)
                            {
                            CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + pageID);
                            }
                        else
                            {
                        CreateAppendXmlAttribute ( smilDocument, txtNode, "src", nccFileName + headingID );
                            }

                        // create seq which will hol audio children 
                        seqNode_AudioParent = smilDocument.CreateElement ( null, "seq", smilBodyNode.NamespaceURI );
                        parNode.AppendChild ( seqNode_AudioParent );

                        // add anchor and href to ncc elements
                        XmlNode anchorNode = nccDocument.CreateElement ( null, "a", bodyNode.NamespaceURI );
                        
                        if ( phrase.Index == 0 )
                            {
                        headingNode.AppendChild ( anchorNode );
                        CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );
                        anchorNode.AppendChild (
                            nccDocument.CreateTextNode ( section.Label ) );
                            }
                        else if ( pageNode != null )
                            {
                            pageNode.AppendChild ( anchorNode );
                        CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );
                            
                            anchorNode.AppendChild ( 
                                nccDocument.CreateTextNode(phrase.PageNumber.ToString () )) ;
                            
                            }
                        }

                    // create audio elements for external audio medias
                    //(ManagedAudioMedia)getProperty<ChannelsProperty>().getMedia(Presentation.AudioChannel)
                    //getProperty<ChannelsProperty>().getMedia(Presentation.PUBLISH_AUDIO_CHANNEL_NAME)
                    Channel publishChannel = m_Presentation.GetSingleChannelByName ( Presentation.PUBLISH_AUDIO_CHANNEL_NAME ) ;
                    ExternalAudioMedia externalMedia = (ExternalAudioMedia)phrase.getProperty<ChannelsProperty> ().getMedia ( publishChannel );

                    XmlNode audioNode = smilDocument.CreateElement ( null, "audio", smilBodyNode.NamespaceURI );
                    seqNode_AudioParent.AppendChild ( audioNode );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "src", externalMedia.getSrc() );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "clip-begin", externalMedia.getClipBegin().ToString () );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "clip-end", externalMedia.getClipEnd().ToString() );
                    CreateAppendXmlAttribute ( smilDocument, audioNode, "id", "aud"+IncrementID);
                    }// if for phrasenode ends
                } // for loop ends

            WriteXmlDocumentToFile ( smilDocument,
                Path.Combine ( m_ExportDirectory,smilFileName ) );
            }

        private string IncrementID { get { return (++m_IdCounter).ToString (); } }

        private XmlAttribute CreateAppendXmlAttribute ( XmlDocument xmlDoc, XmlNode node , string name, string val )
            {
            XmlAttribute attr = xmlDoc.CreateAttribute ( name );
            attr.Value = val;
            node.Attributes.Append ( attr );
            return attr;
            }

        private XmlDocument CreateNCCStubDocument ()
            {
            XmlDocument nccDocument = new XmlDocument ();
            nccDocument.XmlResolver = null;
            
            nccDocument.CreateXmlDeclaration ( "1.0", "utf-8", null );
            nccDocument.AppendChild( nccDocument.CreateDocumentType ( "html", 
                "-//W3C//DTD XHTML 1.0 Transitional//EN",
                "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd",
                null) );
            
            XmlNode htmlNode = nccDocument.CreateElement ( null,
                "html", 
                "http://www.w3.org/1999/xhtml" );

            nccDocument.AppendChild ( htmlNode );
            
            
                
            try
                {
                CreateAppendXmlAttribute ( nccDocument, htmlNode, "lang", "en" );
                CreateAppendXmlAttribute ( nccDocument, htmlNode, "xml:lang", "en" );
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            XmlNode headNode = nccDocument.CreateElement ( null, "head", htmlNode.NamespaceURI );
            htmlNode.AppendChild ( headNode );
            XmlNode bodyNode =  nccDocument.CreateElement ( null, "body", htmlNode.NamespaceURI );
            htmlNode.AppendChild ( bodyNode );
            
            return nccDocument;
            }

        private XmlDocument CreateSmilStubDocument ()
            {
            XmlDocument smilDocument = new XmlDocument ();
            smilDocument.XmlResolver = null;

            smilDocument.CreateXmlDeclaration ( "1.0", "utf-8", null );
            smilDocument.AppendChild( smilDocument.CreateDocumentType ( "smil",
                "-//W3C//DTD SMIL 1.0//EN",
                    "http://www.w3.org/TR/REC-smil/SMIL10.dtd",
                null ) );
            XmlNode smilRootNode = smilDocument.CreateElement( null,
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


        private void CreateNCCMetadata ( XmlDocument nccDocument )
            {
            Dictionary<string, string> metadataMap = CreateDAISY3To2MetadataMap ();

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
                    XmlNode metaNode = nccDocument.CreateElement ( null, "meta", headNode.NamespaceURI );
                    headNode.AppendChild ( metaNode );
                    CreateAppendXmlAttribute ( nccDocument, metaNode, "name", metadataMap[m.getName ()] );
                    CreateAppendXmlAttribute(nccDocument,metaNode,"content", m.getContent() ) ;
                    }

                // to do add more metadata items.
                //  ncc:maxPageNormal
                // ncc:pageFront
                // ncc:pageNormal
                // ncc:pageSpecial
                // ncc:prodNotes
                // ncc:tocItems


                }


            }



        Dictionary<string, string> CreateDAISY3To2MetadataMap ()
            {
            Dictionary<string, string> MetadataMap = new Dictionary<string, string> ();

             
             MetadataMap.Add ( "dc:Contributor", "dc:contributor" );
             MetadataMap.Add ( "dc:Coverage", "dc:coverage" );
        MetadataMap.Add( "dc:Creator","dc:creator");
        MetadataMap.Add ( "dc:Date", "dc:date" );
        MetadataMap.Add( "dc:Description","dc:description");
        MetadataMap.Add ( "dc:Format", "dc:format" );
        MetadataMap.Add ( "dc:Identifier", "dc:identifier" );
        MetadataMap.Add ( "dc:Language", "dc:language" );
        MetadataMap.Add( "dc:Publisher","dc:publisher" );
        MetadataMap.Add ( "dc:Source", "dc:source" );
        MetadataMap.Add ( "dc:Subject", "dc:subject" );
        MetadataMap.Add ( "dc:Title", "dc:title" );
         MetadataMap.Add("dc:Type","dc:type");
        MetadataMap.Add( "dc:Relation","dc:relation");
        MetadataMap.Add ("dc:Rights","dc:rights");
        // X-Metadata
        MetadataMap.Add ( "dtb:sourceDate","ncc:sourceDate");
        MetadataMap.Add ("dtb:sourceEdition","ncc:sourceEdition");
        MetadataMap.Add ("dtb:sourcePublisher","ncc:sourcePublisher");
        MetadataMap.Add ("dtb:sourceRights","ncc:sourceRights");
        MetadataMap.Add ("dtb:sourceTitle","ncc:sourceTitle");
        MetadataMap.Add ("dtb:multimediaType","ncc:multimediaType");
        //MetadataMap.Add ("dtb:multimediaContent";
        MetadataMap.Add ("dtb:narrator","ncc:narrator");
        MetadataMap.Add ("dtb:producer", "ncc:producer");
        MetadataMap.Add ("dtb:producedDate","ncc:producedDate");
        MetadataMap.Add ("dtb:revision","ncc:revision") ;
        MetadataMap.Add ("dtb:revisionDate", "ncc:revisionDate");
        //MetadataMap.Add ("dtb:revisionDescription";
        MetadataMap.Add ("dtb:totalTime","ncc:totalTime");
        //MetadataMap.Add ("dtb:audioFormat";

        MetadataMap.Add ("generator","ncc:generator");
            MetadataMap.Add ("obi:xukversion","obi:xukversion");
             
        return MetadataMap;
            }

        }
    }
