using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace DTBDowngrader
    {
    public class DTBFilesInfo
        {
        private string m_OpfPath;
        private string m_NcxPath;
        private string m_BaseDirectory;
        private TimeSpan m_TotalTime;
        private string m_Title;
        private string m_Identifier;
        private List<string> m_SmilFilesList;
        private List<string> m_SmilPathList;
        private List<string> m_AudioFilesList;
        private List<string> m_AudioFilePathsList;
        private Dictionary<string, string> m_DCMetaData;
        private Dictionary<string, string> m_XMetaData;
        private Dictionary<string, string> m_NcxMetaData;

        public DTBFilesInfo ( string path )
            {
            m_AudioFilesList = new List<string> ();
            m_AudioFilePathsList = new List<string> ();
            m_SmilFilesList = new List<string> ();
            m_SmilPathList = new List<string> ();
            m_DCMetaData = new Dictionary<string, string> ();
            m_XMetaData = new Dictionary<string, string> ();
            m_NcxMetaData = new Dictionary<string, string> ();

            ExtractInfoFromOpf ( path );
            ExtractMetaDataFromNcx ();            
            }

        public string OpfPath { get { return m_OpfPath; } }

        public string NcxPath { get { return m_NcxPath; } }

        public string BaseDirectory { get { return m_BaseDirectory; } }

        public TimeSpan TotalTime { get { return m_TotalTime; } }

        public string Title { get { return m_Title; } }

        public string Identifier { get { return m_Identifier; } }

        public List<string> SmilFilesList { get { return m_SmilFilesList; } }

        public List<string> SmilFilePathsList { get { return m_SmilPathList; } }

        public List<string> AudioFilesList { get { return m_AudioFilesList; } }

        public List<string> AudioFilePathsList { get { return m_AudioFilePathsList; } }

        public string title
            {
            get { return m_Title; }
            }
        public TimeSpan time
            {
            get { return m_TotalTime; }
            }
        public string ID
            {
            get { return m_Identifier; }
            }

        public Dictionary<string, string> DCMetadata { get { return m_DCMetaData; } }

        public Dictionary<string, string> XMetaData { get { return m_XMetaData; } }

        public Dictionary<string, string> NcxMetaData { get { return m_NcxMetaData; } }

        public void ExtractInfoFromOpf ( string path )
            {
            m_OpfPath = path;
            m_BaseDirectory = Directory.GetParent ( m_OpfPath ).FullName;

            XmlDocument XmlDoc = CommonFunctions.CreateXmlDocument ( m_OpfPath );

            string baseDirectoryPath = Directory.GetParent ( m_OpfPath ).FullName;


            XmlNodeList dcMetaDataList = XmlDoc.GetElementsByTagName ( "dc-metadata" )[0].ChildNodes;

            // add DC metadata to dictionary 
            foreach (XmlNode n in dcMetaDataList)
                {
                m_DCMetaData.Add ( n.Name,
                    n.InnerText );
                }
            m_Identifier = m_DCMetaData["dc:Identifier"];
            m_Title =  m_DCMetaData["dc:Title"];
            

            //  extract XMetadata and also  total time of DTD
            XmlNodeList metaNodeList = XmlDoc.GetElementsByTagName ( "meta" );
            foreach (XmlNode n in metaNodeList)
                {
                // collect XMetadata
                m_XMetaData.Add ( n.Attributes.GetNamedItem ( "name" ).Value,
                     n.Attributes.GetNamedItem ( "content" ).Value );

                }
            string timeString = m_XMetaData["dtb:totalTime"] ;
            m_TotalTime = CommonFunctions.GetTimeSpan ( timeString) ;
            if (timeString.EndsWith ( "ms" ))
                {
                m_XMetaData["dtb:totalTime"] = m_TotalTime.ToString ();
                }
            

            //foreach (string s in m_DCMetaData.Keys)
                //MessageBox.Show ( s +" = " +
                    //m_DCMetaData[s] );

            //foreach (string s in m_XMetaData.Keys)
                //MessageBox.Show ( s + " :XMetadata" );

            XmlNode manifestNode = XmlDoc.GetElementsByTagName ( "manifest" )[0];

            Dictionary<string, string> smilIdDictionary = new Dictionary<string, string> ();

            XmlNodeList manifestItemList = XmlDoc.GetElementsByTagName ( "item" );

            foreach (XmlNode n in manifestItemList)
                {
                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/x-dtbncx+xml")
                    {
                    m_NcxPath = Path.Combine ( baseDirectoryPath,
                        n.Attributes.GetNamedItem ( "href" ).Value );
                    }

                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/smil")
                    {

                    smilIdDictionary.Add ( n.Attributes.GetNamedItem ( "id" ).Value,
                        n.Attributes.GetNamedItem ( "href" ).Value );

                    }

                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/mpeg"
                     || n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/x-wav")
                    {
                    m_AudioFilesList.Add ( n.Attributes.GetNamedItem ( "href" ).Value );
                    m_AudioFilePathsList.Add ( Path.Combine ( baseDirectoryPath,
                        n.Attributes.GetNamedItem ( "href" ).Value ) );
                    }

                }

            XmlNode spineNode = XmlDoc.GetElementsByTagName ( "spine" )[0];

            XmlNodeList spineItemRefList = spineNode.ChildNodes;
            foreach (XmlNode n in spineItemRefList)
                {
                if (n.LocalName == "itemref")
                    {
                    string idString = n.Attributes.GetNamedItem ( "idref" ).Value;
                    if (smilIdDictionary.ContainsKey ( idString ))
                        {
                        m_SmilFilesList.Add ( smilIdDictionary[idString] );
                        m_SmilPathList.Add ( Path.Combine ( baseDirectoryPath,
                            smilIdDictionary[idString] ) );
                        }

                    }
                }


            //foreach (String s in AudioFilePathsList)
            //MessageBox.Show ( s );
            XmlDoc = null;

            }

        private void ExtractMetaDataFromNcx ()
            {
            XmlDocument nccDocument = CommonFunctions.CreateXmlDocument ( m_NcxPath );

            XmlNodeList metaDataList = nccDocument.GetElementsByTagName ( "meta" );

            foreach (XmlNode n in metaDataList)
                {
                m_NcxMetaData.Add ( n.Attributes.GetNamedItem ( "name" ).Value,
                    n.Attributes.GetNamedItem ( "content" ).Value );
                }
            }



        }
    }
