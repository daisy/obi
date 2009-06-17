using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;


namespace DTBMerger
    {
    public partial class DTBIntegrator
        {

        public void IntegrateDAISY2DTBs ()
            {
            IntegrateNCCForDAISY2 ();
            UpdateAllSmilFilesForDAISY2 ();
            MoveSmilAndAudioFiles ();
            }

        private void IntegrateNCCForDAISY2 ()
            {

            List<XmlDocument> nccDocumentsList = new List<XmlDocument> ();

            XmlDocument firstNccDocument = CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[0].OpfPath );

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                nccDocumentsList.Add ( CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[i].OpfPath ) );
                }

            // get max page normal
            int maxNormalPageNo = 0;
            XmlNodeList spanNodesList = firstNccDocument.GetElementsByTagName ( "span" );
            foreach (XmlNode n in spanNodesList)
                {
                if (n.Attributes.GetNamedItem ( "class" ).Value == "page-normal")
                    {
                    string strPageNo = n.FirstChild.InnerText;
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

            XmlNodeList pageNodesList = firstNccDocument.GetElementsByTagName ( "span" );
            foreach (XmlNode n in pageNodesList)
                {
                if (n.Attributes.GetNamedItem ( "class" ).Value == "page-normal")
                    {
                    if (m_PageMergeOptions == PageMergeOptions.Renumber)
                        {
                        maxNormalPageNo++;

                        XmlText txtNode = firstNccDocument.CreateTextNode ( maxNormalPageNo.ToString () );
                        n.FirstChild.RemoveChild ( n.FirstChild.FirstChild );
                        n.FirstChild.AppendChild ( txtNode );
                        }
                    else
                        {
                        int pageNo = int.Parse ( n.FirstChild.InnerText );
                        if (pageNo > maxNormalPageNo) maxNormalPageNo = pageNo;
                        }
                    }
                }

            // update metadata

            // add totalTimes for all DTBs
            TimeSpan totalTime = new TimeSpan ();

            for (int i = 0; i < m_DTBFilesInfoList.Count; i++)
                {
                totalTime = totalTime.Add ( m_DTBFilesInfoList[i].time );
                }
            string strTotalTime = totalTime.ToString ();

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


        protected void UpdateAllSmilFilesForDAISY2 ()
            {
            TimeSpan initialTime = m_DTBFilesInfoList[0].TotalTime;

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                foreach (string s in m_DTBFilesInfoList[i].SmilFilePathsList)
                    {
                    UpdateSmilFileForDAISY2 ( s, initialTime );
                    }

                initialTime = initialTime.Add ( m_DTBFilesInfoList[i].TotalTime );
                }


            }

        private void UpdateSmilFileForDAISY2 ( string smilPath, TimeSpan baseTime )
            {
            XmlDocument smilDoc = CommonFunctions.CreateXmlDocument ( smilPath );

            XmlNodeList metaNodeList = smilDoc.GetElementsByTagName ( "meta" );

            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ) != null)
                    {
                    if (n.Attributes.GetNamedItem ( "name" ).Value == "ncc:totalElapsedTime")
                        {
                        string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                        TimeSpan smilTime = CommonFunctions.GetTimeSpan ( timeString );
                        smilTime = baseTime.Add ( smilTime );
                        n.Attributes.GetNamedItem ( "content" ).Value = GetTimeString ( smilTime );
                        }

                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dc:identifier")
                        {
                        n.Attributes.GetNamedItem ( "content" ).Value = m_DTBFilesInfoList[0].Identifier;
                        }
                    } //null check ends
                } // foreach ends

            CommonFunctions.WriteXmlDocumentToFile ( smilDoc, smilPath );
            }


        }
    }
