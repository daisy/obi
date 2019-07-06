using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace DTBDowngrader
    {
    public class Daisy3To202
        {
        string m_InputOpfPath;
        string m_OutputDirectory;
        DTBFilesInfo m_DAISY3Info;
        XmlDocument m_InputNcxDoc;
        XmlDocument m_NccDocument;
        Dictionary<int, XmlNode> m_PageListOpf;
        Dictionary<string, List<string>> m_SmilReferences;
        Dictionary<string, string> m_SmilNccIDMap;

        public Daisy3To202 ( string sourceOpf, string outputDirectory )
            {
            m_InputOpfPath = CopyFiles ( sourceOpf, outputDirectory );
            m_OutputDirectory = outputDirectory;
            m_DAISY3Info = new DTBFilesInfo ( m_InputOpfPath );
            m_InputNcxDoc = CommonFunctions.CreateXmlDocument ( m_DAISY3Info.NcxPath );
            m_NccDocument = CreateNCCSkeletonXmlDocument ();
            m_PageListOpf = new Dictionary<int, XmlNode> ();
            m_SmilReferences = new Dictionary<string, List<string>> ();
            m_SmilNccIDMap = new Dictionary<string, string> ();

            ExtractPageList ();
            CreateNccNavigationStructure ();
            CreateNCCMetadata ();
            UpdateAllSmils ();
            WriteNCCFile ();
            }

        private string CopyFiles ( string sourceOpf, string outputDirectory )
            {
            DirectoryInfo inputDir = Directory.GetParent ( sourceOpf );

            FileInfo[] files = inputDir.GetFiles ();

            foreach (FileInfo f in files)
                {
                f.CopyTo ( Path.Combine ( outputDirectory, f.Name ) );
                }

            return Path.Combine ( outputDirectory, new FileInfo ( sourceOpf ).Name );
            }

        private void WriteNCCFile ()
            {
            string inputDir = Directory.GetParent ( m_InputOpfPath ).FullName;
            string nccPath = inputDir + "\\ncc.html";

            if (File.Exists ( nccPath ))
                File.Delete ( nccPath );

            File.Create ( nccPath ).Close ();
            CommonFunctions.WriteXmlDocumentToFile ( m_NccDocument, nccPath );
            }
        private XmlDocument CreateNCCSkeletonXmlDocument ()
            {
            System.Reflection.Assembly currentAssembly = System.Reflection.Assembly.GetExecutingAssembly ();
            Stream nccStream = currentAssembly.GetManifestResourceStream ( "DTBDowngrader.ncc.html" );


            XmlTextReader Reader = null;
            XmlDocument NccXmlDoc = null;
            // create xml reader and load xml document
            try
                {
                Reader = new XmlTextReader ( nccStream );
                Reader.XmlResolver = null;

                NccXmlDoc = new XmlDocument ();
                NccXmlDoc.XmlResolver = null;
                NccXmlDoc.Load ( Reader );
                }
            finally
                {
                Reader.Close ();
                Reader = null;
                }

            return NccXmlDoc;
            }

        private void CreateNccNavigationStructure ()
            {

            TraverseNavMapAndCreateHeadings ( m_InputNcxDoc.GetElementsByTagName ( "navMap" )[0] );

            }

        private void TraverseNavMapAndCreateHeadings ( XmlNode node )
            {
            if (node.LocalName == "navPoint")
                {
                CreateHeadings ( node );
                }
            foreach (XmlNode n in node.ChildNodes)
                {
                TraverseNavMapAndCreateHeadings ( n );
                }
            }

        private void CreateHeadings ( XmlNode navPointNode )
            {
            XmlNode testNode = navPointNode.ParentNode;
            int i = 1;
            for (i = 1; i <= 6; i++)
                {
                if (testNode.LocalName == "navMap")
                    {
                    break;
                    }
                else
                    {
                    testNode = testNode.ParentNode;
                    }
                }

            //MessageBox.Show ( navPointNode.Attributes.GetNamedItem ( "playOrder" ).Value  + " : " + i.ToString () );

            string headingString = "h" + i.ToString ();
            string nSpace = m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI;
            XmlNode bodyNode = m_NccDocument.GetElementsByTagName ( "body" )[0];

            XmlNode headingNode = m_NccDocument.CreateElement ( null, headingString, nSpace );
            string headingClass = navPointNode == m_InputNcxDoc.GetElementsByTagName ( "navMap" )[0].FirstChild ? "title" : "section";
            CreateNccAttribute ( headingNode, "class", headingClass );
            CreateNccAttribute ( headingNode, "id",
                navPointNode.Attributes.GetNamedItem ( "id" ).Value );

            XmlNode anchorNode = m_NccDocument.CreateElement ( null, "a", nSpace );
            string smilRef = navPointNode.ChildNodes[1].Attributes.GetNamedItem ( "src" ).Value;
            CreateNccAttribute ( anchorNode, "href", smilRef );
            anchorNode.AppendChild ( m_NccDocument.CreateTextNode ( navPointNode.ChildNodes[0].FirstChild.InnerText ) );
            //MessageBox.Show ( smilRef );
            headingNode.AppendChild ( anchorNode );
            bodyNode.AppendChild ( headingNode );

            // check if next playOrder is of page, if so, add page
            int navPointPlayOrder = int.Parse ( navPointNode.Attributes.GetNamedItem ( "playOrder" ).Value );

            if (m_PageListOpf.ContainsKey ( navPointPlayOrder + 1 ))
                {
                AddPageToNCC ( m_PageListOpf[navPointPlayOrder + 1] );
                }
            AddSmilReferences ( smilRef, navPointNode.Attributes.GetNamedItem ( "id" ).Value );
            //UpdateSmilFile ( smilRef, navPointNode.Attributes.GetNamedItem ( "id" ).Value );
            }

        
        private void AddPageToNCC ( XmlNode pageNodeOpf )
            {
            string nSpace = m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI;
            XmlNode bodyNode = m_NccDocument.GetElementsByTagName ( "body" )[0];

            XmlNode pageNode = m_NccDocument.CreateElement ( null, "span", nSpace );

            string pageType = "page-" + pageNodeOpf.Attributes.GetNamedItem ( "type" ).Value;
            CreateNccAttribute ( pageNode, "class", pageType );
            CreateNccAttribute ( pageNode, "id",
        pageNodeOpf.Attributes.GetNamedItem ( "id" ).Value );

            XmlNode anchorNode = m_NccDocument.CreateElement ( null, "a", nSpace );
            string smilRef = pageNodeOpf.ChildNodes[1].Attributes.GetNamedItem ( "src" ).Value;
            CreateNccAttribute ( anchorNode, "href", smilRef );
            anchorNode.AppendChild ( m_NccDocument.CreateTextNode (
                pageNodeOpf.ChildNodes[0].FirstChild.InnerText ) );
            //MessageBox.Show ( smilRef );
            pageNode.AppendChild ( anchorNode );
            bodyNode.AppendChild ( pageNode );

            AddSmilReferences ( smilRef, pageNodeOpf.Attributes.GetNamedItem ( "id" ).Value );
            }

        private void AddSmilReferences ( string smilSRC, string nccID)
            {
            string[] smilFragments = smilSRC.Split ( '#' );
            string smilFilename = smilFragments[0];
            string smilID = smilFragments[1];

            if ( !m_SmilReferences.ContainsKey(smilFilename ))
                {
                m_SmilReferences.Add (smilFilename, new List<string> ()) ;
                }
            m_SmilReferences[smilFilename].Add( smilID) ;
            m_SmilNccIDMap.Add(smilID, nccID ) ;

            }

        private void UpdateAllSmils ()
            {
            foreach (string name  in m_SmilReferences.Keys)
                {
                UpdateSmilFile ( Path.Combine( m_DAISY3Info.BaseDirectory, name),
m_SmilReferences[name] );
                }
            }

        private void UpdateSmilFile ( string smilFullPath, List<string> smilIDs )
            {
            XmlDocument smilXmlDoc = CommonFunctions.CreateXmlDocument ( smilFullPath);

                smilXmlDoc.RemoveChild ( smilXmlDoc.DocumentType );
                
                XmlDocumentType type = smilXmlDoc.CreateDocumentType ( "smil",
                    "-//W3C//DTD SMIL 1.0//EN",
                    "http://www.w3.org/TR/REC-smil/SMIL10.dtd",
                    null );

                smilXmlDoc.InsertBefore ( type, smilXmlDoc.DocumentElement );
                
            XmlNodeList metaList = smilXmlDoc.GetElementsByTagName ( "meta" );

            foreach (XmlNode n in metaList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:uid")
                    {
                    n.Attributes.GetNamedItem ( "name" ).Value = "dc:identifier";
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value.Contains ( ":totalElapsedTime" ))
                    {
                    TimeSpan t = CommonFunctions.GetTimeSpan ( n.Attributes.GetNamedItem ( "content" ).Value );
                    n.Attributes.GetNamedItem ( "content" ).Value = t.ToString ();
                    }

                string name = n.Attributes.GetNamedItem ( "name" ).Value;
                n.Attributes.GetNamedItem ( "name" ).Value = name.Replace ( "dtb:", "ncc:" );
                }
            // add format 
            XmlNode formatNode = smilXmlDoc.CreateElement ( null, "meta", metaList[0].ParentNode.NamespaceURI );
            metaList[0].ParentNode.AppendChild ( formatNode );
            formatNode.Attributes.Append ( smilXmlDoc.CreateAttribute ( "name" ) );
            formatNode.Attributes.Append ( smilXmlDoc.CreateAttribute("content") );
            formatNode.Attributes.GetNamedItem("name").Value = "dc:format" ;
            formatNode.Attributes.GetNamedItem("content").Value = "Daisy 2.02" ;

            // add layout
            XmlNode headNode = smilXmlDoc.GetElementsByTagName ( "head" )[0];
            XmlNode layoutNode = smilXmlDoc.CreateElement ( null, "layout", headNode.NamespaceURI );
            headNode.AppendChild ( layoutNode );
            XmlNode regionNode = smilXmlDoc.CreateElement ( null, "region", headNode.NamespaceURI );
            layoutNode.AppendChild ( regionNode );
            regionNode.Attributes.Append (
                smilXmlDoc.CreateAttribute ( "id" ) );
            regionNode.Attributes.GetNamedItem ( "id" ).Value = "textView";

            // change audio attributes and find total duration
            TimeSpan duration = new TimeSpan(0) ;
            XmlNodeList audioNodeList = smilXmlDoc.GetElementsByTagName ( "audio" );
            int audioId = 0;
            foreach (XmlNode n in audioNodeList)
                {
                duration =  duration.Add(
                    CalculateTimeDifference(
                    n.Attributes.GetNamedItem("clipBegin").Value,
                n.Attributes.GetNamedItem ("clipEnd").Value )  );
                
                XmlAttribute beginAtt = smilXmlDoc.CreateAttribute ( "clip-begin" );
                beginAtt.Value = n.Attributes.GetNamedItem ( "clipBegin" ).Value;
                n.Attributes.RemoveNamedItem ( "clipBegin" );

                XmlAttribute endAtt = smilXmlDoc.CreateAttribute ( "clip-end" );
                endAtt.Value = n.Attributes.GetNamedItem ( "clipEnd" ).Value;
                n.Attributes.RemoveNamedItem ( "clipEnd" );

                n.Attributes.Append ( beginAtt );
                n.Attributes.Append ( endAtt );

                n.Attributes.Append( smilXmlDoc.CreateAttribute("id") );
                n.Attributes.GetNamedItem("id").Value ="a"+  m_DAISY3Info.Identifier + "audio" + audioId  ;
                audioId++;
                }

            // add dur to seq.
            XmlNode seqNode = smilXmlDoc.GetElementsByTagName ( "body" )[0].FirstChild;
            if ( seqNode.LocalName == "seq")
                {
                seqNode.Attributes.RemoveNamedItem ( "fill" );
                XmlAttribute att = smilXmlDoc.CreateAttribute("dur") ;
                att.Value = duration.ToString () ;
                seqNode.Attributes.Append (att ) ;
                }
            
                // add par node, text node and seq around pars
                XmlNodeList seqChildList = seqNode.ChildNodes;
                MessageBox.Show ( "SmilID :" + smilIDs.Count.ToString () );
                for ( int i = 0 ;    i < seqChildList.Count; i++ )
                    {
                    XmlNode parNode = seqChildList[i];
                    if (parNode.Attributes.GetNamedItem ( "id" ) != null &&
                        smilIDs.Contains ( parNode.Attributes.GetNamedItem ( "id" ).Value ))
                        {
                        string strID = parNode.Attributes.GetNamedItem ( "id" ).Value;
                        XmlNode seqParNode = smilXmlDoc.CreateElement ( null, "par", seqNode.NamespaceURI );
                        seqNode.AppendChild ( seqParNode );

                        XmlNode textNode = smilXmlDoc.CreateElement ( null, "text", seqNode.NamespaceURI );
                        seqParNode.AppendChild ( textNode );
                        textNode.Attributes.Append ( smilXmlDoc.CreateAttribute ( "src" ) );
                        textNode.Attributes.GetNamedItem ( "src" ).Value = "ncc.html#" + m_SmilNccIDMap[strID];

                        textNode.Attributes.Append ( smilXmlDoc.CreateAttribute ( "id" ) );
                        textNode.Attributes.GetNamedItem ( "id" ).Value = strID;
                        XmlNode seqNode_FirstParChild = smilXmlDoc.CreateElement ( null, "seq", seqNode.NamespaceURI );
                        seqParNode.AppendChild ( seqNode_FirstParChild );

                        // copy all pars from primary seq to child seq

                        for ( int j = i; j < seqChildList.Count ; j++ )
                            {
                            //MessageBox.Show ( j.ToString () );
                            XmlNode n = seqNode.ChildNodes[j];
                            if ( n!= null &&  n.LocalName == "par" &&
                                n.Attributes.GetNamedItem ( "id" ) != null && 
                                (n.Attributes.GetNamedItem("id").Value == strID 
                                || !smilIDs.Contains(n.Attributes.GetNamedItem("id").Value  )) )
                                {
                            XmlNode par_internal = n.CloneNode ( true );


                            if (par_internal.LocalName == "par"
                                && par_internal.FirstChild.LocalName == "audio")
                                {
                                seqNode_FirstParChild.AppendChild ( par_internal.FirstChild );
                                //MessageBox.Show ( n.LocalName );
                                }
                            else
                                {
                                MessageBox.Show ( n.FirstChild.Attributes.GetNamedItem ( "clip-begin" ).Value );
                                break;
                                }
                                
                                }
                            }


                        }
                }//foreach id ends
            
            for ( int i = seqNode.ChildNodes.Count - 1; i >=0;i-- )
                {
                if (seqNode.ChildNodes[i].LocalName == "par"
                    && seqNode.ChildNodes[i].FirstChild.LocalName == "audio")
                    {
                    seqNode.RemoveChild ( seqNode.ChildNodes[i] );

                    System.Media.SystemSounds.Asterisk.Play ();
                    }
                else
                    {
                    MessageBox.Show ( seqNode.ChildNodes[i].LocalName + " : " + seqNode.ChildNodes[i].FirstChild.LocalName + " : not removed" );
                    }
                }
             
            //seqNode.RemoveAll ();
            //seqNode.AppendChild ( ParqNodeClone );
            // add endsync attribute to par
            XmlNodeList parList = smilXmlDoc.GetElementsByTagName ( "par" );
            foreach (XmlNode n in parList)
                {
                if (n.Attributes.GetNamedItem ( "endsync" ) == null)
                    {
                    n.Attributes.Append ( smilXmlDoc.CreateAttribute ( "endsync" ) );
                    n.Attributes.GetNamedItem ( "endsync" ).Value = "last";
                    }
                }

            CommonFunctions.WriteXmlDocumentToFile ( smilXmlDoc, smilFullPath);
            smilXmlDoc = null;
            RemoveSmilSmlns ( smilFullPath);
            }

        private TimeSpan CalculateTimeDifference ( string begin, string end )
            {
            TimeSpan beginTime = CommonFunctions.GetTimeSpan ( begin );
            TimeSpan endTime = CommonFunctions.GetTimeSpan ( end );
            
            return endTime.Subtract ( beginTime);
            }

        private void RemoveSmilSmlns ( string smilPath )
            {
            StreamReader sr = new StreamReader ( smilPath );
            string s = sr.ReadToEnd ();
            sr.Close ();
            sr = null;
            string replaceString = "<smil xmlns=\"http://www.w3.org/2001/SMIL20/\">";

            s = s.Replace (
                replaceString,
                    "<smil>" );
            //MessageBox.Show ( s );
            StreamWriter sw = new StreamWriter ( smilPath );
            sw.Write ( s );
            sw.Close ();
            sw = null;
            }


        private void ExtractPageList ()
            {
            XmlDocument ncxDocument = CommonFunctions.CreateXmlDocument ( m_DAISY3Info.NcxPath );

            XmlNode pageListNode = ncxDocument.GetElementsByTagName ( "pageList" )[0];

            foreach (XmlNode n in pageListNode.ChildNodes)
                {
                if (n.LocalName == "pageTarget")
                    {
                    m_PageListOpf.Add (
                        int.Parse ( n.Attributes.GetNamedItem ( "playOrder" ).Value ), n );
                    }
                }
            }



        private void CreateNCCMetadata ()
            {
            XmlNode headNode = m_NccDocument.GetElementsByTagName ( "head" )[0];

            XmlElement metaElement = m_NccDocument.CreateElement ( null, "meta", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            CreateNccAttribute ( metaElement, "content", "text/html; charset=utf-8" );
            CreateNccAttribute ( metaElement, "http-equiv", "Content-type" );
            headNode.AppendChild ( metaElement );

            XmlElement titleElement = m_NccDocument.CreateElement ( null, "title", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            titleElement.AppendChild ( m_NccDocument.CreateTextNode ( m_DAISY3Info.title ) );
            headNode.AppendChild ( titleElement );

            // create dc metadata

            // collect non duplicate  Metadata from opf and ncx in a dictionary
            Dictionary<string, string> XMetaData = new Dictionary<string, string> ();

            foreach (string s in m_DAISY3Info.DCMetadata.Keys)
                {
                XMetaData.Add ( s, m_DAISY3Info.DCMetadata[s] );
                }

            foreach (string s in m_DAISY3Info.XMetaData.Keys)
                {
                XMetaData.Add ( s, m_DAISY3Info.XMetaData[s] );
                }

            if (XMetaData.ContainsKey ( "dc:Format" ))
                {
                XMetaData["dc:Format"] = "Daisy 2.02";
                }

            // remove daisy 2.02 incompatible metadata from opf metadata
            XMetaData.Remove ( "dc:Title" );
            XMetaData.Remove ( "generator" );
            XMetaData.Remove ( "dtb:multimediaContent" );
            XMetaData.Remove ( "dtb:audioFormat" );
            XMetaData.Remove ( "dtb:totalPageCount" );

            foreach (string s in m_DAISY3Info.NcxMetaData.Keys)
                {
                if (!XMetaData.ContainsKey ( s ))
                    {
                    XMetaData.Add ( s, m_DAISY3Info.NcxMetaData[s] );
                    }
                }
            XMetaData.Remove ( "dtb:uid" );
            if (XMetaData.ContainsKey ( "dtb:maxPageNumber" ))
                {
                XMetaData.Add ( "ncc:maxPageNormal", XMetaData["dtb:maxPageNumber"] );
                XMetaData.Remove ( "dtb:maxPageNumber" );
                }



            // add TOC items
            string TOCItems = m_InputNcxDoc.GetElementsByTagName ( "navPoint" ).Count.ToString ();
            XMetaData.Add ( "ncc:tocItems", TOCItems );

            // count  normal, front and special pages
            int frontPages = 0;
            int normalPages = 0;
            int specialPages = 0;
            foreach (XmlNode n in m_PageListOpf.Values)
                {
                string pageType = n.Attributes.GetNamedItem ( "type" ).Value;
                switch (pageType)
                    {
                case "front":
                frontPages++;
                break;

                case "normal":
                normalPages++;
                break;

                case "special":
                specialPages++;
                break;
                    }
                }

            XMetaData.Add ( "ncc:pageNormal", normalPages.ToString () );
            XMetaData.Add ( "ncc:pageFront", frontPages.ToString () );
            XMetaData.Add ( "ncc:pageSpecial", specialPages.ToString () );

            // add info about sidebar, prod notes and foot notes
            //XMetaData.Add ( "ncc:sidebars", "0" );
            //XMetaData.Add ( "ncc:prodnotes", "0" );
            //XMetaData.Add ( "ncc:footnotes", "0" );


            // create XMetadata
            foreach (string s in XMetaData.Keys)
                {
                CreateNccMetadataNodes ( headNode, s.Replace ( "dtb:", "ncc:" )
                    , XMetaData[s] );
                }

            //CommonFunctions.WriteXmlDocumentToFile ( m_NccDocument, 
            //System.AppDomain.CurrentDomain.BaseDirectory +  "\\1.xml" );
            System.Windows.Forms.MessageBox.Show ( "done" );
            }

        private XmlElement CreateNccMetadataNodes ( XmlNode headNode, string name, string content )
            {
            XmlElement metaElement = m_NccDocument.CreateElement ( null, "meta", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            CreateNccAttribute ( metaElement, "name", name );
            CreateNccAttribute ( metaElement, "content", content );

            headNode.AppendChild ( metaElement );
            return metaElement;
            }


        private XmlAttribute CreateNccAttribute ( XmlNode node, string attributeName, string attributeValue )
            {
            XmlAttribute att = m_NccDocument.CreateAttribute ( attributeName );
            att.Value = attributeValue;
            node.Attributes.Append ( att );
            return att;
            }




        }
    }
