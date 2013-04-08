using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace DTBMerger
    {
    public enum PageMergeOptions { KeepExisting, Renumber } ;

    public partial class Integrator
        {
        private List<DTBFilesInfo> m_DTBFilesInfoList;
        private PageMergeOptions m_PageMergeOptions ;
        private bool m_CanRemoveDuplicatePagesInDAISY3;

        public Integrator ( List<string> pathsList , PageMergeOptions pageOption)
            {
            m_DTBFilesInfoList = new List<DTBFilesInfo> ();

            for (int i = 0; i < pathsList.Count; i++)
                {
                m_DTBFilesInfoList.Add ( new DTBFilesInfo ( pathsList[i] ) );
                }
            m_PageMergeOptions = pageOption;
            }

        /// <summary>
        /// A flag to disable removal of duplicate pages in daisy 3, a precaution due to last minutes changes
        /// </summary>
        public bool CanRemoveDuplicatePagesInDAISY3
            {
            get { return m_CanRemoveDuplicatePagesInDAISY3; }
            set { m_CanRemoveDuplicatePagesInDAISY3 = value; }
            }


        public void IntegrateDAISY3DTBs ()
            {
            IntegrateOpf ();
            IntegrateNcx ();
            UpdateAllSmilFiles ();
            MoveSmilAndAudioFiles ();
            }


        protected void IntegrateOpf ()
            {

            List<XmlDocument> opfDocumentsList = new List<XmlDocument> ();
            XmlDocument firstOpf = CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[0].OpfPath );

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                opfDocumentsList.Add ( CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[i].OpfPath ) );
                }

            // update DTB time w.r.t. combined time of all DTBs
            TimeSpan totalTime = new TimeSpan ( 0 );

            XmlNode timeNode_FirstDTD = null;

            // extract time from first DTD
            XmlNodeList metaNodeList = firstOpf.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalTime")
                    {
                    timeNode_FirstDTD = n;
                    string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                    totalTime = CommonFunctions.GetTimeSpan ( timeString );
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
                        totalTime = totalTime.Add ( CommonFunctions.GetTimeSpan ( timeString ) );
                        }
                    }
                } // document iterator ends

            string tsString = GetTimeString ( totalTime );
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
                            XmlNode copyNode = firstOpf.ImportNode ( n.Clone (), false );
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
                        XmlNode copyNode = firstOpf.ImportNode ( n.Clone (), false );
                        firstSpineNode.AppendChild ( copyNode );
                        } //-3

                    } //-2
                } //-1  DTD iterator loop

            CommonFunctions.WriteXmlDocumentToFile ( firstOpf, m_DTBFilesInfoList[0].OpfPath );

            }


        protected void IntegrateNcx ()
            {

            List<XmlDocument> NcxDocumentsList = new List<XmlDocument> ();
            XmlDocument firstNcx = CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[0].NcxPath );

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                NcxDocumentsList.Add ( CommonFunctions.CreateXmlDocument ( m_DTBFilesInfoList[i].NcxPath ) );
                }

            // update metadata 
            int totalPages = 0;
            int maxPages = 0;
            int maxDepth = 0;

            XmlNode totalPagesNode = null;
            XmlNode maxPagesNode = null;
            XmlNode maxDepthNode = null;

            // extract relevant metadata from first DTD
            XmlNodeList metaNodeList = firstNcx.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalPageCount")
                    {
                    totalPagesNode = n;
                    string totalPageString = n.Attributes.GetNamedItem ( "content" ).Value;
                    totalPages = int.Parse ( totalPageString );
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:maxPageNumber")
                    {
                    maxPagesNode = n;
                    string maxPagesString = n.Attributes.GetNamedItem ( "content" ).Value;
                    maxPages = int.Parse ( maxPagesString );
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:depth")
                    {
                    maxDepthNode = n;
                    string str = n.Attributes.GetNamedItem ( "content" ).Value;
                    maxDepth = int.Parse ( str );
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
                        int temp = int.Parse ( maxPagesString );
                        maxPages += temp;
                        //if (temp > maxPages) maxPages = temp;
                        }

                    if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:depth")
                        {
                        string str = n.Attributes.GetNamedItem ( "content" ).Value;
                        int temp = int.Parse ( str );
                        if (temp > maxDepth) maxDepth = temp;
                        }

                    }
                } // document iterator ends
            totalPagesNode.Attributes.GetNamedItem ( "content" ).Value = totalPages.ToString ();
            maxPagesNode.Attributes.GetNamedItem ( "content" ).Value = maxPages.ToString ();
            maxDepthNode.Attributes.GetNamedItem ( "content" ).Value = maxDepth.ToString ();


            // get navMap  and pageList of first DTD
            XmlNode firstNavMapNode = firstNcx.GetElementsByTagName ( "navMap" )[0];
            XmlNodeList ncxPageList = firstNcx.GetElementsByTagName ( "pageList" ) ;
            XmlNode firstPageListNode = null;

            if ( ncxPageList != null && ncxPageList.Count > 0 )
                {
            firstPageListNode = ncxPageList[0];
                }
            else
                {
                // create pagelist node in ncx
                firstPageListNode = firstNcx.CreateElement ( null, "pageList", firstNavMapNode.NamespaceURI );
                firstNavMapNode.ParentNode.AppendChild ( firstPageListNode ) ;
                }
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
                    if (temp > maxPlayOrderNav) maxPlayOrderNav = temp;

                    string typeString = n.Attributes.GetNamedItem ( "type" ).Value;
                    if (typeString == "normal")
                        {
                        string valueString = n.Attributes.GetNamedItem ( "value" ).Value;
                        temp = int.Parse ( valueString );
                        if (temp > maxPageValue) maxPageValue = temp;
                        }

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
                        if (temp > playOrderNavPoints) playOrderNavPoints = temp;
                        //MessageBox.Show ( temp.ToString () );
                        n.Attributes.GetNamedItem ( "playOrder" ).Value = (maxPlayOrderNav + temp).ToString ();
                        //MessageBox.Show ( maxPlayOrderNav.ToString ()  + ":" + temp.ToString ());

                        }
                    }

                foreach (XmlNode n in NcxDocumentsList[i].GetElementsByTagName ( "navMap" )[0].ChildNodes)
                    {
                    if (n.LocalName == "navPoint")
                        {
                        firstNavMapNode.AppendChild ( firstNcx.ImportNode ( n, true ) );
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

                        string typeString = n.Attributes.GetNamedItem ( "type" ).Value;
                        if (typeString == "normal")
                            {
                            if (m_PageMergeOptions == PageMergeOptions.Renumber)
                                {
                                pageValue++;
                                n.Attributes.GetNamedItem ( "value" ).Value = (maxPageValue + pageValue).ToString ();

                                XmlNode textNode = n.FirstChild.FirstChild;
                                if (textNode.LocalName == "text")
                                    textNode.InnerText = (maxPageValue  + pageValue).ToString ();

                                /*
                                string valueString = n.Attributes.GetNamedItem ( "value" ).Value;
                                temp = int.Parse ( valueString );
                                if (temp > pageValue) pageValue = temp;
                                n.Attributes.GetNamedItem ( "value" ).Value = (maxPageValue + temp).ToString ();

                                XmlNode textNode = n.FirstChild.FirstChild;
                                if (textNode.LocalName == "text")
                                    textNode.InnerText = (maxPageValue + temp).ToString ();
                                 */ 
                                }
                            }
                                 
                        firstPageListNode.AppendChild ( firstNcx.ImportNode ( n, true ) );
                        }
                    }



                maxPlayOrderNav += playOrderNavPoints;
                //maxPlayOrderPage += playOrderPagePoints;
                maxPageValue += pageValue;
                }

            // create namespage manager for first ncc document
            XmlNamespaceManager firstDocNSManager = new XmlNamespaceManager ( firstNcx.NameTable );
            firstDocNSManager.AddNamespace ( "firstNS",
                firstNcx.DocumentElement.NamespaceURI );

            if (m_PageMergeOptions == PageMergeOptions.KeepExisting && firstPageListNode.ChildNodes.Count > 0
                && m_CanRemoveDuplicatePagesInDAISY3)
                { // 1
                XmlNodeList duplicateRemovePageList = firstPageListNode.SelectNodes ( ".//firstNS:pageTarget",
                    firstDocNSManager );
                int previousPage = -1;
                bool isPageDeleted = false; // flag to indicate if any page is deleted.

                for (int i = 0; i < duplicateRemovePageList.Count; i++)
                    { //2
                    string pageType = duplicateRemovePageList[i].Attributes.GetNamedItem ( "type" ).Value;
                    if (pageType == "normal")
                        { //3
                        int pageNo = int.Parse ( duplicateRemovePageList[i].Attributes.GetNamedItem ( "value" ).Value );
                        if (previousPage == pageNo)
                            { //4
                            XmlNode parent = duplicateRemovePageList[i].ParentNode;
                            parent.RemoveChild ( duplicateRemovePageList[i] );
                            parent = null;
                            isPageDeleted = true;
                            } //-4
                        previousPage = pageNo;

                        } //-3

                    } // end of for loop -2

                // fix the play order
                if (isPageDeleted)
                    { //2
                    XmlNodeList playOrderAttrList = firstNcx.SelectNodes ( "/firstNS:ncx//@playOrder",
                        firstDocNSManager );

                    Dictionary<int, List<XmlAttribute>> attrDictionary = new Dictionary<int, List<XmlAttribute>> ();
                    List<int> sortList = new List<int> ();

                    foreach (XmlNode n in playOrderAttrList)
                        { //3
                        int playOrder = int.Parse ( ((XmlAttribute)n).Value );

                        if (!attrDictionary.ContainsKey ( playOrder ))
                            { //4
                            List<XmlAttribute> attrList = new List<XmlAttribute> ();
                            attrList.Add ( (XmlAttribute)n );
                            attrDictionary.Add ( playOrder, attrList );
                            sortList.Add ( playOrder );
                            } //-4
                        else
                            { //4
                            attrDictionary[playOrder].Add ( (XmlAttribute)n );
                            } //-4
                        } //-3
                    sortList.Sort ();

                    for (int i = 0; i < sortList.Count; i++)
                        { //3
                        List<XmlAttribute> attrList = attrDictionary[sortList[i]];

                        foreach (XmlAttribute attr in attrList)
                            { //4
                            attr.Value = (i + 1).ToString ();
                            } //-4
                        } //-3
                    } //-2

                // if page node deleted, update total page metadata
                if (isPageDeleted)
                    { //2
                    XmlNodeList totalPageList = firstPageListNode.SelectNodes ( ".//firstNS:pageTarget",
                        firstDocNSManager );
                    if (totalPagesNode.Attributes.GetNamedItem ( "content" ) != null)
                        { //3
                        totalPagesNode.Attributes.GetNamedItem ( "content" ).Value = totalPageList.Count.ToString ();
                        } //-3
                    } //-2

                } // end of page renumbering option check -1
            
            // Pages are updated a lot so it is important to set max normal pages metadata accordingly
                XmlNodeList finalPageList= firstPageListNode.SelectNodes ( ".//firstNS:pageTarget",
                        firstDocNSManager );
                if (finalPageList != null && finalPageList.Count > 0)
                    {
                    int maxNormalPageValue = 0;
                    for (int i = 0; i < finalPageList.Count; i++)
                        {
                        string pageType = finalPageList[i].Attributes.GetNamedItem ( "type" ).Value;
                        if (pageType == "normal")
                            {
                            int pageNo = int.Parse ( finalPageList[i].Attributes.GetNamedItem ( "value" ).Value );

                            if (pageNo > maxNormalPageValue)
                                {
                                maxNormalPageValue = pageNo;
                                }
                            }
                        }
                    // update in metadata
                    if (maxPagesNode.Attributes.GetNamedItem ( "content" ) != null)
                        {
                        maxPagesNode.Attributes.GetNamedItem ( "content" ).Value = maxNormalPageValue.ToString ();
                        }

                    }//end of update page metadata

                 


            // if page list do not have children, remove it
            if (firstPageListNode.ChildNodes.Count == 0)
                {
                XmlNode parentNode = firstPageListNode.ParentNode;
                parentNode.RemoveChild ( firstPageListNode );
                }


            CommonFunctions.WriteXmlDocumentToFile ( firstNcx, m_DTBFilesInfoList[0].NcxPath );
            }

        protected void UpdateAllSmilFiles ()
            {
            TimeSpan initialTime = m_DTBFilesInfoList[0].TotalTime;

            for (int i = 1; i < m_DTBFilesInfoList.Count; i++)
                {
                foreach (string s in m_DTBFilesInfoList[i].SmilFilePathsList)
                    {
                    UpdateSmilFile ( s, initialTime );
                    }

                initialTime = initialTime.Add ( m_DTBFilesInfoList[i].TotalTime );
                }


            }

        private void UpdateSmilFile ( string smilPath, TimeSpan baseTime )
            {
            XmlDocument smilDoc = CommonFunctions.CreateXmlDocument ( smilPath );

            XmlNodeList metaNodeList = smilDoc.GetElementsByTagName ( "meta" );

            foreach (XmlNode n in metaNodeList)
                {
                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:totalElapsedTime")
                    {
                    string timeString = n.Attributes.GetNamedItem ( "content" ).Value;
                    TimeSpan smilTime = CommonFunctions.GetTimeSpan ( timeString );
                    smilTime = baseTime.Add ( smilTime );
                    n.Attributes.GetNamedItem ( "content" ).Value = GetTimeString ( smilTime );
                    }

                if (n.Attributes.GetNamedItem ( "name" ).Value == "dtb:uid")
                    {
                    n.Attributes.GetNamedItem ( "content" ).Value = m_DTBFilesInfoList[0].Identifier;
                    }
                }

            CommonFunctions.WriteXmlDocumentToFile ( smilDoc, smilPath );
            }



        protected void MoveSmilAndAudioFiles ()
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


        // Convert a number to roman numerals (lowercase)
        private string ToRoman ( int n )
            {
            if (n <= 0) throw new Exception ( "Number must be greater than 0." );
            string roman = "";
            // Thousands
            while (n >= 1000)
                {
                roman += "m";
                n -= 1000;
                }
            // Hundreds
            if (n >= 900)
                {
                roman += "cm";
                n -= 900;
                }
            else if (n >= 500)
                {
                roman += "d";
                n -= 500;
                }
            else if (n >= 400)
                {
                roman += "cd";
                n -= 400;
                }
            while (n >= 100)
                {
                roman += "c";
                n -= 100;
                }
            // Dozens
            if (n >= 90)
                {
                roman += "xc";
                n -= 90;
                }
            else if (n >= 50)
                {
                roman += "l";
                n -= 50;
                }
            else if (n >= 40)
                {
                roman += "xl";
                n -= 40;
                }
            while (n >= 10)
                {
                roman += "x";
                n -= 10;
                }
            // Units
            if (n >= 9)
                {
                roman += "ix";
                n -= 9;
                }
            else if (n >= 5)
                {
                roman += "v";
                n -= 5;
                }
            else if (n >= 4)
                {
                roman += "iv";
                n -= 4;
                }
            while (n >= 1)
                {
                roman += "i";
                --n;
                }
            return roman;
            }

        private string GetTimeString ( TimeSpan time )
            {
                if (time.TotalDays < 1)
                {
                    return time.ToString().Split('.')[0];
                }
                else
                {
                    string strHrs = (time.Hours + (time.Days * 24)).ToString();
                    if (strHrs.Length < 2) strHrs = "0" + strHrs;
                    string strMins = time.Minutes.ToString();
                    if (strMins.Length < 2) strMins = "0" + strMins;
                    string strSeconds = time.Seconds.ToString() +"."+ time.Milliseconds.ToString ();
                    if (strSeconds.Length < 2) strSeconds = "0" + strSeconds;
                    string strTime = strHrs + ":" + strMins + ":" + strSeconds;
                    return strTime;
                }
            /*
            return time.ToString ();
            string strHours = time.Hours.ToString ();
            if (strHours.Length < 2)
                strHours = "0" + strHours;

            string strMinutes = time.Minutes.ToString ();
            if (strMinutes.Length < 2)
                strMinutes = "0" + strMinutes;

            string strSeconds = time.Seconds.ToString ();
            if (strSeconds.Length < 2)
                strSeconds = "0" + strSeconds;

            string strMilliSeconds = time.Milliseconds.ToString ();
            //if (strMilliSeconds.Length > 3)
            //strMilliSeconds = strMilliSeconds.Substring ( 0, 3 );

            return strHours + ":" + strMinutes + ":" + strSeconds + "." + strMilliSeconds;
             */ 
            }


        }
    }
