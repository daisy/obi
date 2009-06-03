using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO ;

namespace DTBDowngrader
    {
    public class Daisy3To202
        {
        string m_InputOpf;
        string m_OutputDirectory;
        DTBFilesInfo m_DAISY3Info;
        XmlDocument m_NccDocument;

        public Daisy3To202 ( string sourceOpf, string outputDirectory )
            {
            m_InputOpf = sourceOpf;
            m_OutputDirectory = outputDirectory;
            m_DAISY3Info = new DTBFilesInfo ( sourceOpf );
            m_NccDocument = CreateNCCSkeletonXmlDocument ();
            CreateNccNavigationStructure ();
            CreateNCCMetadata ();

            }

        private XmlDocument CreateNCCSkeletonXmlDocument ()
            {
            XmlDocument NccXmlDoc = null;
            try
                {
                NccXmlDoc = CommonFunctions.CreateXmlDocument ( 
                    Directory.GetParent (System.Reflection.Assembly.GetExecutingAssembly ().Location).FullName + "\\ncc.html" );
                }
            catch (System.Exception ex)
                {
                System.Windows.Forms.MessageBox.Show ( ex.ToString () );
                }
            return NccXmlDoc;
            }

        private void CreateNccNavigationStructure ()
            {
            XmlDocument ncxDocument= CommonFunctions.CreateXmlDocument(m_DAISY3Info.NcxPath ) ;
TraverseNavMapAndCreateHeadings ( ncxDocument.GetElementsByTagName("navMap")[0]);
            
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
            int i = 1 ;
            for (i = 1; i <=  6; i++)
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

            System.Windows.Forms.MessageBox.Show ( navPointNode.Attributes.GetNamedItem ( "playOrder" ).Value  + " : " + i.ToString () );

            string headingString = "h" + i.ToString ();
            string nSpace = m_NccDocument.GetElementsByTagName ( "heml" )[0].NamespaceURI;
            XmlNode bodyNode = m_NccDocument.GetElementsByTagName ( "body" )[0];

            XmlNode headingNode = m_NccDocument.CreateElement(null, headingString, nSpace ) ;
            CreateNccAttribute ( headingNode, "id",
                navPointNode.Attributes.GetNamedItem ( "id" ).Value );

            CreateNccAttribute ( headingNode, "class", "section" );

            bodyNode.AppendChild ( headingNode );
            }

        

        private void CreateNCCMetadata ()
            {
            XmlNode headNode =  m_NccDocument.GetElementsByTagName ( "head" )[0];

            XmlElement metaElement =  m_NccDocument.CreateElement (     null, "meta",m_NccDocument.GetElementsByTagName("html")[0].NamespaceURI) ;
            CreateNccAttribute ( metaElement, "content", "text/html; charset=utf-8" );
            CreateNccAttribute ( metaElement, "http-equiv", "Content-type");
            headNode.AppendChild ( metaElement );

            XmlElement titleElement = m_NccDocument.CreateElement ( null, "title", m_NccDocument.GetElementsByTagName ( "html" )[0].NamespaceURI );
            titleElement.AppendChild ( m_NccDocument.CreateTextNode( m_DAISY3Info.title )) ;
            headNode.AppendChild(titleElement) ;

            // create dc metadata
            foreach (string s in m_DAISY3Info.DCMetadata.Keys)
                {
                CreateNccMetadataNodes ( headNode, s, m_DAISY3Info.DCMetadata[s] );
                }

            // collect non duplicate  XMetadata from opf and ncx in a dictionary
            Dictionary<string, string> XMetaData = new Dictionary<string, string> ();

            foreach (string s in m_DAISY3Info.XMetaData.Keys)
                {
                XMetaData.Add ( s, m_DAISY3Info.XMetaData[s] );
                }

            foreach (string s in m_DAISY3Info.NcxMetaData.Keys)
                {
                if (!XMetaData.ContainsKey ( s ))
                    {
                    XMetaData.Add ( s, m_DAISY3Info.NcxMetaData[s] );
                    }
                }
            // remove uid
            XMetaData.Remove ( "dtb:uid" );


            // create XMetadata
            foreach (string s in XMetaData.Keys)
                {
                CreateNccMetadataNodes ( headNode, s.Replace ("dtb:" , "ncc:")
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
