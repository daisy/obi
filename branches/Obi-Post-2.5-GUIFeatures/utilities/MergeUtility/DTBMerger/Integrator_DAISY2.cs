using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;


namespace DTBMerger
    {
    public partial class Integrator
        {

        public void IntegrateDAISY2_02DTBs ()
            {
            // first get total time from smil files as total time in ncc file has rounding off errors
            TimeSpan totalTime =  UpdateAllSmilFilesForDAISY2 ();
            IntegrateNCCForDAISY2 ( totalTime);
            MoveSmilAndAudioFiles ();
            UpdateMasterSmilFile ();
            }

        private void IntegrateNCCForDAISY2 ( TimeSpan totalTime)
            {

            List<XmlDocument> nccDocumentsList = new List<XmlDocument> ();

            XmlDocument firstNccDocument = CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[0].OpfPath );

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                nccDocumentsList.Add ( CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[i].OpfPath ) );
                }

            // get max page normal
            // create namespage manager for first ncc document
            XmlNamespaceManager firstDocNSManager = new XmlNamespaceManager ( firstNccDocument.NameTable );
            firstDocNSManager.AddNamespace ( "firstNS",
                firstNccDocument.DocumentElement.NamespaceURI );

            XmlNodeList spanNodesList = firstNccDocument.SelectNodes ( "/firstNS:html/firstNS:body//firstNS:span",
                firstDocNSManager );

            int maxNormalPageNo = 0;
            //XmlNodeList spanNodesList = firstNccDocument.GetElementsByTagName ( "span" );
            foreach (XmlNode n in spanNodesList)
                {
                if (n.Attributes.GetNamedItem ( "class" ).Value == "page-normal")
                    {
                    XmlNode anchorNode = n.SelectSingleNode ( ".//firstNS:a",
                    firstDocNSManager );
                    if (anchorNode == null)
                        {
                        MessageBox.Show ( n.Attributes.GetNamedItem ( "id" ).Value );
                        continue;
                        }
                    string strPageNo = anchorNode.InnerText;
                    int pageNo = int.Parse ( strPageNo );
                    if (pageNo > maxNormalPageNo) maxNormalPageNo = pageNo;
                    }
                }



            // add all nodes in body
            XmlNode firstNCCBodyNode = firstNccDocument.GetElementsByTagName ( "body" )[0];

            for (int i = 0; i < nccDocumentsList.Count; i++)
                {
                XmlNode bodyNode = nccDocumentsList[i].GetElementsByTagName ( "body" )[0];
                foreach (XmlNode n in bodyNode.ChildNodes)
                    {
                    XmlNode newNode = firstNccDocument.ImportNode ( n.CloneNode ( true ), true );
                    firstNCCBodyNode.AppendChild ( newNode );
                    }
                } // nccDocuments for loop ends
            if (m_PageMergeOptions == PageMergeOptions.Renumber)
                {
                maxNormalPageNo = 0;
                }



            XmlNodeList pageNodesList = firstNccDocument.SelectNodes ( "/firstNS:html/firstNS:body//firstNS:span",
                firstDocNSManager );
            int previousPageNo = -1;
            List<XmlNode> duplicatePageNodeList = new List<XmlNode> ();
            
            foreach (XmlNode n in pageNodesList)
                {
                if (n.Attributes.GetNamedItem ( "class" ).Value == "page-normal")
                    {
                    XmlNode pageAnchorNode = n.SelectSingleNode ( ".//firstNS:a",
                            firstDocNSManager );
                    if (pageAnchorNode == null)
                        {
                        System.Diagnostics.Debug.Fail ( "span node is not complete, ID:", n.Attributes.GetNamedItem ( "id" ).Value );
                        }

                    if (m_PageMergeOptions == PageMergeOptions.Renumber)
                        {
                        maxNormalPageNo++;

                        XmlText txtNode = firstNccDocument.CreateTextNode ( maxNormalPageNo.ToString () );
                        pageAnchorNode.RemoveChild ( pageAnchorNode.FirstChild );
                        pageAnchorNode.AppendChild ( txtNode );
                        }
                    else
                        {
                        // do not renumber             
                        // only remove consective duplicate page numbers
                        int pageNo = int.Parse ( pageAnchorNode.InnerText );

                        if (previousPageNo == pageNo)
                            {
                            // duplicate page so add to duplicate page list
                            duplicatePageNodeList.Add ( n );

                            // add div so as to  remove duplicate span.
                            XmlAttribute AttrID = (XmlAttribute)n.Attributes.GetNamedItem ( "id" ).Clone ();
                            XmlNode newAnchor = pageAnchorNode.CloneNode ( true );
                            XmlNode divNode = firstNccDocument.CreateElement ( null, "div", firstNCCBodyNode.NamespaceURI );
                            divNode.Attributes.Append ( AttrID );
                            divNode.Attributes.Append (
                                firstNccDocument.CreateAttribute ( "class" ) );
                            divNode.Attributes.GetNamedItem ( "class" ).Value = "group";
                            divNode.AppendChild ( newAnchor );

                            n.ParentNode.InsertBefore ( divNode, n );

                            }
                        if (pageNo > maxNormalPageNo) maxNormalPageNo = pageNo;
                        previousPageNo = pageNo;
                        }
                    }
                }


            // remove duplicate page span nodes
            for (int i = 0; i < duplicatePageNodeList.Count; i++)
                {
                XmlNode parent = duplicatePageNodeList[i].ParentNode;
                parent.RemoveChild ( duplicatePageNodeList[i] );
                parent = null;
                }

            // update metadata

            // add totalTimes for all DTBs
            /*
            TimeSpan totalTime = new TimeSpan ();

            for (int i = 0; i < m_DTBFilesInfoList.Count; i++)
                {
                totalTime = totalTime.Add ( m_DTBFilesInfoList[i].time );
                }
             */
            string strTotalTime = CommonFunctions.GetStringTotalTimeRoundedOff ( totalTime);

            int pageFront = 0;
            int pageNormal = 0;
            int pageSpecial = 0;
            int tocItems = 0;

            foreach (XmlDocument xmlDoc in nccDocumentsList)
                {
                XmlNodeList metaNodesList = xmlDoc.GetElementsByTagName ( "meta" );
                foreach (XmlNode n in metaNodesList)
                    {
                    if (n.Attributes.GetNamedItem ( "name" ) != null)
                        {
                        string metaDataName = n.Attributes.GetNamedItem ( "name" ).Value;
                        string metadataContent = n.Attributes.GetNamedItem ( "content" ).Value;

                        if (metaDataName == "ncc:pageFront")
                            {
                            pageFront += int.Parse ( metadataContent );
                            }
                        if (metaDataName == "ncc:pageNormal")
                            {
                            pageNormal += int.Parse ( metadataContent );
                            }
                        if (metaDataName == "ncc:pageSpecial")
                            {
                            pageSpecial += int.Parse ( metadataContent );
                            }
                        if (metaDataName == "ncc:tocItems")
                            {
                            tocItems += int.Parse ( metadataContent );
                            }
                        }

                    }// meta node list foreach ends
                } // ncc doc foreach loop ends

            // add up variables in first ncc metadata
            XmlNodeList firstMetaNodesList = firstNccDocument.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in firstMetaNodesList)
                {
                if (n.Attributes.GetNamedItem ( "name" ) != null)
                    {
                    string metaDataName = n.Attributes.GetNamedItem ( "name" ).Value;
                    string metadataContent = n.Attributes.GetNamedItem ( "content" ).Value;

                    if (metaDataName == "ncc:pageFront")
                        {
                        pageFront += int.Parse ( metadataContent );
                        n.Attributes.GetNamedItem ( "content" ).Value = pageFront.ToString ();
                        }
                    if (metaDataName == "ncc:pageNormal")
                        {
                        pageNormal += int.Parse ( metadataContent );
                        n.Attributes.GetNamedItem ( "content" ).Value = pageNormal.ToString ();
                        }
                    if (metaDataName == "ncc:pageSpecial")
                        {
                        pageSpecial += int.Parse ( metadataContent );
                        n.Attributes.GetNamedItem ( "content" ).Value = pageSpecial.ToString ();
                        }
                    if (metaDataName == "ncc:tocItems")
                        {
                        tocItems += int.Parse ( metadataContent );
                        n.Attributes.GetNamedItem ( "content" ).Value = tocItems.ToString ();
                        }
                    if (metaDataName == "ncc:maxPageNormal")
                        {
                        n.Attributes.GetNamedItem ( "content" ).Value = maxNormalPageNo.ToString ();
                        }

                    if (metaDataName == "ncc:totalTime")
                        {
                        n.Attributes.GetNamedItem ( "content" ).Value = strTotalTime;
                        }

                    }
                }// metanode foreach loop ends


            CommonFunctions.WriteXmlDocumentToFile ( firstNccDocument, m_DTBFilesInfoList[0].OpfPath );
            }


        private TimeSpan UpdateAllSmilFilesForDAISY2 ()
            {
            TimeSpan initialTime = new TimeSpan ();
            double bookDuration= 0 ;
            foreach (string s in m_DTBFilesInfoList[0].SmilFilePathsList)
                {
                initialTime= initialTime.Add( UpdateSmilFileForDAISY2 ( s, initialTime, false) );
                }

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                foreach (string s in m_DTBFilesInfoList[i].SmilFilePathsList)
                    {
                    initialTime = initialTime.Add ( UpdateSmilFileForDAISY2 ( s, initialTime , true) );
                    }

                //initialTime = initialTime.Add ( m_DTBFilesInfoList[i].TotalTime );
                }
            return initialTime;
            }

        private TimeSpan UpdateSmilFileForDAISY2 ( string smilPath, TimeSpan baseTime, bool canWrite )
            {
            XmlDocument smilDoc = CommonFunctions.CreateXmlDocument ( smilPath );

            XmlNodeList metaNodeList = smilDoc.GetElementsByTagName ( "meta" );

            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ) != null)
                    {
                    if (n.Attributes.GetNamedItem ( "name" ).Value == "ncc:totalElapsedTime")
                        {
                        //string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                        //TimeSpan smilTime = CommonFunctions.GetTimeSpan ( timeString );
                        //smilTime = baseTime.Add ( smilTime );
                        n.Attributes.GetNamedItem ( "content" ).Value = GetTimeString ( baseTime);
                        }

                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dc:identifier")
                        {
                        n.Attributes.GetNamedItem ( "content" ).Value = m_DTBFilesInfoList[0].Identifier;
                        }
                    } //null check ends
                } // foreach ends
            if (canWrite)
                {
                CommonFunctions.WriteXmlDocumentToFile ( smilDoc, smilPath );
                }

            // get duration of current smil
            XmlNode firstSeqNode = smilDoc.GetElementsByTagName ( "body" )[0].FirstChild ;
            string strDur = firstSeqNode.Attributes.GetNamedItem ( "dur" ).Value;
            strDur = strDur.Replace ( "s", "" );
            double duration =  double.Parse ( strDur );
            return new TimeSpan ( Convert.ToInt64 ( 10000000 * duration ) );
            }

        private void UpdateMasterSmilFile ()
            {
            string masterSmilPath = Path.Combine ( m_DTBFilesInfoList[0].BaseDirectory, "master.smil" );
            if (File.Exists ( masterSmilPath ))
                {
                XmlDocument masterSmilDocument = CommonFunctions.CreateXmlDocument ( masterSmilPath );
                XmlNode bodyNode = masterSmilDocument.GetElementsByTagName ( "body" )[0];
                // clear all children of body
                bodyNode.RemoveAll ();

                // load the ncc document
                XmlDocument firstNccDocument = CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[0].OpfPath );
                XmlNamespaceManager firstDocNSManager = new XmlNamespaceManager ( firstNccDocument.NameTable );
                firstDocNSManager.AddNamespace ( "firstNS",
                    firstNccDocument.DocumentElement.NamespaceURI );

                XmlNodeList smilRefNodesList = firstNccDocument.SelectNodes ( "//firstNS:body/firstNS:h1 | //firstNS:body/firstNS:h2 | //firstNS:body/firstNS:h3 | //firstNS:body/firstNS:h4 | //firstNS:body/firstNS:h5 | //firstNS:body/firstNS:h6",
                    firstDocNSManager );

                List<string> smilFilesList = new List<string> ();
                foreach (XmlNode n in smilRefNodesList)
                    {
                    XmlNode anchorNode = n.SelectSingleNode ( ".//firstNS:a",
                        firstDocNSManager );
                    string smilFilename = anchorNode.Attributes.GetNamedItem ( "href" ).Value;
                    smilFilename = smilFilename.Split ( '#' )[0];
                    if (!smilFilesList.Contains ( smilFilename ))
                        {

                        smilFilesList.Add ( smilFilename );

                        // addsmil entry to master.smil
                        XmlNode refNode = masterSmilDocument.CreateElement ( null, "ref", bodyNode.NamespaceURI );
                        bodyNode.AppendChild ( refNode );

                        refNode.Attributes.Append (
                            masterSmilDocument.CreateAttribute ( "title" ) );
                        refNode.Attributes.GetNamedItem ( "title" ).Value = anchorNode.InnerText;

                        refNode.Attributes.Append (
                            masterSmilDocument.CreateAttribute ( "src" ) );
                        refNode.Attributes.GetNamedItem ( "src" ).Value = smilFilename;

                        refNode.Attributes.Append (
                            masterSmilDocument.CreateAttribute ( "id" ) );
                        refNode.Attributes.GetNamedItem ( "id" ).Value = "ms_" + Path.GetFileNameWithoutExtension ( smilFilename );
                        }
                    }
                // calculate total time in master smil
                TimeSpan totalTime = new TimeSpan ();

                for (int i = 0; i < m_DTBFilesInfoList.Count; i++)
                    {
                    totalTime = totalTime.Add ( m_DTBFilesInfoList[i].time );
                    }

                // update time in smil metadata
                XmlNodeList metaDataList = masterSmilDocument.GetElementsByTagName ( "meta" );

                foreach (XmlNode n in metaDataList)
                    {
                    if (n.ParentNode.LocalName == "head" &&
                        n.Attributes.GetNamedItem ( "name" ) != null
                    && n.Attributes.GetNamedItem ( "name" ).Value == "ncc:timeInThisSmil")
                        {
                        n.Attributes.GetNamedItem ( "content" ).Value = totalTime.ToString ();
                        }
                    }

                firstNccDocument = null;
                CommonFunctions.WriteXmlDocumentToFile ( masterSmilDocument, masterSmilPath );
                }
            else
                MessageBox.Show ( "master smil do not exist at: " + masterSmilPath );


            }

        }
    }
