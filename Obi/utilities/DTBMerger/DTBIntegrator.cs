
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace renamer
    {
    class DTBIntegrator
        {
        private List<DTBFilesInfo> m_DTBFilesInfoList;

        public DTBIntegrator ( string [] pathsList)
            {
            m_DTBFilesInfoList = new List<DTBFilesInfo> ();

            for (int i = 0; i < pathsList.Length; i++)
                {
                m_DTBFilesInfoList.Add ( new DTBFilesInfo ( pathsList[i] ) );
                }

            }

        public void IntegrateOpf ()
            {

            List<XmlDocument> opfDocumentsList = new List<XmlDocument> ();
            XmlDocument firstOpf = CreateXmlDocument ( m_DTBFilesInfoList[0].OpfPath );

            for ( int i = 1 ; i < m_DTBFilesInfoList.Count ; i++ )
                {
                opfDocumentsList.Add ( CreateXmlDocument ( m_DTBFilesInfoList[i].OpfPath ) );
                }

            // update DTB time w.r.t. combined time of all DTBs
            double totalTime = 0;
            XmlNode timeNode_FirstDTD = null;

            // extract time from first DTD
            XmlNodeList metaNodeList = firstOpf.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                if ( n.Attributes.GetNamedItem ("name").Value == "dtb:totalTime" )
                    {
                    timeNode_FirstDTD = n;
                    string timeString = n.Attributes.GetNamedItem ("content").Value  ;
                    totalTime +=  TimeSpan.Parse ( timeString ).TotalMilliseconds;
                    }
                }

            // add time from all DTDs

            for (int i = 0; i < opfDocumentsList.Count; i++)
                {
                metaNodeList = opfDocumentsList[i].GetElementsByTagName ( "meta" );
                foreach (XmlNode n in metaNodeList)
                    {
                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalTime")
                        {
                        string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                        totalTime += TimeSpan.Parse ( timeString ).TotalMilliseconds;
                        }
                    }
                } // document iterator ends
            
            TimeSpan ts = new TimeSpan ( 0 ,0 ,0,
                ((int)totalTime / 1000), ((int)totalTime % 1000) );
            string tsString = ts.Hours.ToString () + ":" + ts.Minutes.ToString () + ":" + ts.Seconds.ToString () + "." + ts.Milliseconds.ToString ();
            //MessageBox.Show ( tsString );

            timeNode_FirstDTD.Attributes.GetNamedItem ( "content" ).Value = tsString;

            /// integrate manifest into manifest of first DTD
            XmlNode firstDTDManifestNode = firstOpf.GetElementsByTagName ( "manifest" )[0];

            // collects all manifest nodes from all dtds
            for (int i = 0; i < opfDocumentsList.Count; i++)
                { //1
                XmlNode manifestNode = opfDocumentsList[i].GetElementsByTagName ( "manifest" )[0];


                foreach (XmlNode n in manifestNode.ChildNodes)
                    { //2
                    if (n.LocalName == "item")
                        { //3
                        if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/smil"
                            || n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/mpeg"
                     || n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/x-wav")
                            { //4
                            XmlNode copyNode = firstOpf.ImportNode ( n.Clone () , false);
                            firstDTDManifestNode.AppendChild ( copyNode );
                            } //-4
                        } //-3

                    } //-2
                } //-1  DTD iterator loop

// integrate nodes in spine
            XmlNode firstSpineNode = firstOpf.GetElementsByTagName ( "spine" )[0];

            // collects all spine nodes from all dtds
            for (int i = 0; i < opfDocumentsList.Count; i++)
                { //1
                XmlNode spineNode = opfDocumentsList[i].GetElementsByTagName ( "spine" )[0];


                foreach (XmlNode n in spineNode.ChildNodes)
                    { //2
                    if (n.LocalName == "itemref")
                        { //3
                        XmlNode copyNode = firstOpf.ImportNode ( n.Clone () , false);
                            firstSpineNode.AppendChild ( copyNode );
                        } //-3

                    } //-2
                } //-1  DTD iterator loop

            WriteXmlDocumentToFile ( firstOpf, m_DTBFilesInfoList[0].OpfPath );

            }


        private XmlDocument CreateXmlDocument ( string path )
            {
            // create xml reader and load xml document
            XmlTextReader Reader = new XmlTextReader ( path );
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;
            return XmlDoc;
            }

        private void WriteXmlDocumentToFile (XmlDocument xmlDoc,  string path )
            {
            XmlTextWriter writer = new XmlTextWriter ( path, null );
            writer.Formatting = Formatting.Indented;
            xmlDoc.Save ( writer );
            writer.Close ();
            writer = null;
            }

        public void IntegrateNcx ()
            {

            List<XmlDocument> NcxDocumentsList = new List<XmlDocument> ();
            XmlDocument firstNcx = CreateXmlDocument ( m_DTBFilesInfoList[0].NcxPath);

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                NcxDocumentsList.Add ( CreateXmlDocument ( m_DTBFilesInfoList[i].NcxPath) );
                }

            // update metadata 
            int totalPages  = 0;
            int maxPages = 0;
            int maxDepth = 0 ;

            XmlNode totalPagesNode = null;
            XmlNode maxPagesNode = null;
            XmlNode maxDepthNode = null ;

            // extract relevant metadata from first DTD
            XmlNodeList metaNodeList = firstNcx.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalPageCount")
                    {
                    totalPagesNode = n;
                    string totalPageString  = n.Attributes.GetNamedItem ( "content" ).Value;
                    totalPages = int.Parse (totalPageString   ) ;
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:maxPageNumber")
                    {
                    maxPagesNode = n;
                    string maxPagesString  = n.Attributes.GetNamedItem ( "content" ).Value;
                    maxPages = int.Parse (maxPagesString  ) ;
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value ==  "dtb:depth" )
                    {
                    maxDepthNode = n ;
                    string str  = n.Attributes.GetNamedItem ( "content" ).Value;
                    maxDepth = int.Parse (str) ;
                    }

                }

            // update page meta nodes w.r.t. all dtds
            for (int i = 0; i < NcxDocumentsList.Count; i++)
                {
                metaNodeList = NcxDocumentsList[i].GetElementsByTagName ( "meta" );
                foreach (XmlNode n in metaNodeList)
                    {
                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalPageCount")
                        {
                        string totalPageString = n.Attributes.GetNamedItem ( "content" ).Value;
                        totalPages += int.Parse ( totalPageString );
                        }

                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:maxPageNumber")
                        {
                        string maxPagesString = n.Attributes.GetNamedItem ( "content" ).Value;
                         int temp  = int.Parse ( maxPagesString );
                         maxPages += temp;
                         //if (temp > maxPages) maxPages = temp;
                        }

                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:depth")
                        {
                        string str = n.Attributes.GetNamedItem ( "content" ).Value;
                         int temp  = int.Parse ( str);
                         if (temp > maxDepth ) maxDepth = temp;
                        }

                    }
                } // document iterator ends
            totalPagesNode.Attributes.GetNamedItem ( "content" ).Value = totalPages.ToString ();
            maxPagesNode.Attributes.GetNamedItem ( "content" ).Value = maxPages.ToString ();
            maxDepthNode.Attributes.GetNamedItem ( "content" ).Value = maxDepth.ToString ();


            // get navMap  and pageList of first DTD
            XmlNode firstNavMapNode = firstNcx.GetElementsByTagName ( "navMap" )[0];
            XmlNode firstPageListNode = firstNcx.GetElementsByTagName ( "pageList" )[0];

            XmlNodeList navPointsList = firstNcx.GetElementsByTagName ( "navPoint" );
            int maxPlayOrderNav = 0;
            

            foreach (XmlNode n in navPointsList)
                {
                if (n.LocalName == "navPoint")
                    {
                    string playOrderString = n.Attributes.GetNamedItem ( "playOrder" ).Value;
                    int temp = int.Parse ( playOrderString );
                    if (temp > maxPlayOrderNav) maxPlayOrderNav = temp;
                    }
                }

            // page list
            XmlNodeList pageTargetsList = firstNcx.GetElementsByTagName ( "pageTarget" );
            //int maxPlayOrderPage = 0;
            int maxPageValue = 0;

            foreach (XmlNode n in pageTargetsList)
                {
                if (n.LocalName == "pageTarget")
                    {
                    string playOrderString = n.Attributes.GetNamedItem ( "playOrder" ).Value;
                    int temp = int.Parse ( playOrderString );
                    if (temp > maxPlayOrderNav) maxPlayOrderNav= temp;

                    playOrderString = n.Attributes.GetNamedItem ( "value" ).Value;
                    temp = int.Parse ( playOrderString );
                    if (temp > maxPageValue) maxPageValue = temp;
                    }
                }

            for (int i = 0; i < NcxDocumentsList.Count; i++)
                {

                // page list
                XmlNodeList navPoints = NcxDocumentsList[i].GetElementsByTagName ( "navPoint" );
                int playOrderNavPoints = 0;


                foreach (XmlNode n in navPoints)
                    {
                    if (n.LocalName == "navPoint")
                        {
                        string playOrderString = n.Attributes.GetNamedItem ( "playOrder" ).Value;
                        int temp = int.Parse ( playOrderString );
                        if (temp > playOrderNavPoints ) playOrderNavPoints = temp;
                        //MessageBox.Show ( temp.ToString () );
                        n.Attributes.GetNamedItem ( "playOrder" ).Value = (maxPlayOrderNav + temp).ToString ();

                        firstNavMapNode.AppendChild ( firstNcx.ImportNode ( n , true) );
                        }
                    }


                XmlNodeList pagePoints = NcxDocumentsList[i].GetElementsByTagName ( "pageTarget" );
                //int playOrderPagePoints = 0;
                int pageValue = 0;
                
                foreach (XmlNode n in pagePoints)
                    {
                    if (n.LocalName == "pageTarget")
                        {
                        string playOrderString = n.Attributes.GetNamedItem ( "playOrder" ).Value;
                        int temp = int.Parse ( playOrderString );
                        if (temp > playOrderNavPoints) playOrderNavPoints = temp;
                        n.Attributes.GetNamedItem ( "playOrder" ).Value = (maxPlayOrderNav + temp).ToString ();

                        //firstPageListNode.AppendChild ( firstNcx.ImportNode ( n , true) );

                        playOrderString = n.Attributes.GetNamedItem ( "value" ).Value;
                        temp = int.Parse ( playOrderString );
                        if (temp > pageValue) pageValue = temp;
                        n.Attributes.GetNamedItem ( "value" ).Value = (maxPageValue + temp).ToString ();

                        firstPageListNode.AppendChild ( firstNcx.ImportNode ( n, true ) );
                        }
                    }



                maxPlayOrderNav += playOrderNavPoints;
                //maxPlayOrderPage += playOrderPagePoints;
                maxPageValue += pageValue;
                }




            WriteXmlDocumentToFile ( firstNcx, m_DTBFilesInfoList[0].NcxPath);
            }

        public void UpdateAllSmilFiles ()
            {
            TimeSpan initialTime = m_DTBFilesInfoList[0].TotalTime;

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                MessageBox.Show ( initialTime.ToString () );
                foreach (string s in m_DTBFilesInfoList[i].SmilFilePathsList)
                    {
                    UpdateSmilFile ( s, initialTime );
                    }

                initialTime =  initialTime.Add ( m_DTBFilesInfoList[i].TotalTime );
                }


            }

        private void UpdateSmilFile ( string smilPath,  TimeSpan baseTime)
            {
            XmlDocument smilDoc = CreateXmlDocument ( smilPath );

            XmlNodeList metaNodeList = smilDoc.GetElementsByTagName ("meta") ;

            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalElapsedTime")
                    {
                    string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                    TimeSpan smilTime = TimeSpan.Parse ( timeString );
                    n.Attributes.GetNamedItem ( "content" ).Value = baseTime.Add ( smilTime ).ToString ();
                    }
                }

            WriteXmlDocumentToFile ( smilDoc, smilPath );
            }



        public void MoveSmilAndAudioFiles ()
            {
            string baseDirectory = m_DTBFilesInfoList[0].BaseDirectory;
            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                foreach (string s in m_DTBFilesInfoList[i].SmilFilesList)
                    {
                    string sourcePath = Path.Combine ( m_DTBFilesInfoList[i].BaseDirectory, s );
                    string destinationPath = Path.Combine ( baseDirectory, s );

                    File.Move ( sourcePath, destinationPath );
                    }

                foreach (string s in m_DTBFilesInfoList[i].AudioFilesList)
                    {
                    string sourcePath = Path.Combine ( m_DTBFilesInfoList[i].BaseDirectory, s );
                    string destinationPath = Path.Combine ( baseDirectory, s );

                    File.Move ( sourcePath, destinationPath );
                    }
                }// DTB iterator ends

            }


        }
    }
