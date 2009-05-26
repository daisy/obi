using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace DTBMerger
    {
    public class DTBRenamer
        {
        string m_OpfPath;
        string m_NcxPath;
        List<string> m_OriginalSmilList;
        Dictionary<string, string> m_SmilMap;
        List<string>  m_OriginalAudioFileList;
        Dictionary<string, string> m_AudioFileMap;
        string m_InitialString;


        public DTBRenamer ( String path , string initialString)
            {
            m_OpfPath = path;
            m_InitialString = initialString;
            m_SmilMap = new Dictionary<string, string> ();
            m_AudioFileMap = new Dictionary<string, string> ();
            m_OriginalAudioFileList = new List<string> ();
            ExtractFileNamesFromOpf ();
            }

        public void RenameDTBFilesSet ()
            {
            GenerateSmilMapAndAudioMapDictionary();
            UpdateReferencesInOpf ();
            UpdateReferencesInNcx ();
            UpdateAudioReferencesInAllSmils ();
            RenameSmilAndAudioFiles ();
            }

        private void GenerateSmilMapAndAudioMapDictionary ()
            {
            List<string> smilFileList = m_OriginalSmilList ;

            
            for ( int i = 0 ; i < smilFileList.Count ; i++ )
                {
                m_SmilMap.Add ( smilFileList[i],
                    m_InitialString + i.ToString () + ".smil" );

                }

            foreach (string s in m_OriginalAudioFileList)
                {
                m_AudioFileMap.Add ( s,
                    m_InitialString + s );
                }
            //foreach (string s in m_SmilMap.Values)
                //MessageBox.Show ( s );
            }

        private void ExtractFileNamesFromOpf ()
            {
            List<string> smilList = new List<string> ();
            // create xml reader and load xml document
            XmlTextReader Reader = new XmlTextReader ( m_OpfPath );
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;

            XmlNode manifestNode = XmlDoc.GetElementsByTagName ( "manifest" )[0];

            Dictionary<string, string> smilIdDictionary = new Dictionary<string, string> ();

            XmlNodeList manifestItemList =  XmlDoc.GetElementsByTagName ( "item" );

            foreach (XmlNode n in manifestItemList)
                {
                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/x-dtbncx+xml")
                    {
                    m_NcxPath = Path.Combine ( Directory.GetParent (m_OpfPath).FullName,
                        n.Attributes.GetNamedItem ( "href" ).Value );
                    }

                if (n.Attributes.GetNamedItem ( "media-type" ).Value == "application/smil")
                    {

                    smilIdDictionary.Add ( n.Attributes.GetNamedItem ("id").Value,
                        n.Attributes.GetNamedItem ( "href" ).Value );
                        
                                        }

                                    if (n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/mpeg"
                                        ||   n.Attributes.GetNamedItem ( "media-type" ).Value == "audio/x-wav")
                                        {
                                        m_OriginalAudioFileList.Add ( n.Attributes.GetNamedItem ( "href" ).Value );
                                        }

                }

            XmlNode spineNode = XmlDoc.GetElementsByTagName ( "spine" )[0];

            XmlNodeList spineItemRefList = spineNode.ChildNodes;
            foreach (XmlNode n in spineItemRefList)
                {
                if (n.LocalName == "itemref")
                    {
                    string idString = n.Attributes.GetNamedItem ( "idref" ).Value;
                    if ( smilIdDictionary.ContainsKey( idString))
                        {
                        smilList.Add ( smilIdDictionary[idString] );
                            
                        }

                    }
                }

            //foreach (String s in smilList)
                //MessageBox.Show ( s );
            XmlDoc = null;
             m_OriginalSmilList =  smilList;
            }

        private void UpdateReferencesInOpf ()
            {
            XmlTextReader Reader = new XmlTextReader ( m_OpfPath );
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;

            // dictionary for updating item ref   IDs
            Dictionary<string, string> iDDictionary = new Dictionary<string, string> ();

            XmlNode manifestNode = XmlDoc.GetElementsByTagName ( "manifest" )[0];

            XmlNodeList itemList = manifestNode.ChildNodes;

            for (int i = 0; i < itemList.Count; i++)
                { //1
                if (itemList[i].LocalName == "item")
                    { //2
                    string fileName = itemList[i].Attributes.GetNamedItem ( "href" ).Value;

                    if (m_SmilMap.ContainsKey ( fileName ))
                        { //3
                        itemList[i].Attributes.GetNamedItem ( "href" ).Value = m_SmilMap[fileName];

                        string id = itemList[i].Attributes.GetNamedItem ( "id" ).Value;
                        string newID = m_InitialString + id;
                        //string newID = id;
                        itemList[i].Attributes.GetNamedItem ( "id" ).Value = newID;
                        iDDictionary.Add ( id, newID);

                        } //-3

                    if (m_AudioFileMap.ContainsKey ( fileName ))
                        { //3
                        itemList[i].Attributes.GetNamedItem ( "href" ).Value = m_AudioFileMap[fileName];

                        string id = itemList[i].Attributes.GetNamedItem ( "id" ).Value;
                        string newID = m_InitialString + id;
                        //string newID = id;
                        itemList[i].Attributes.GetNamedItem ( "id" ).Value = newID;
                        
                        iDDictionary.Add ( id, newID );
                        } //-3
} //-2
                    } //-1

                

                XmlNode spineNode = XmlDoc.GetElementsByTagName ( "spine" )[0];

                foreach (XmlNode n in spineNode.ChildNodes)
                    { //2
                    if (n.LocalName == "itemref")
                        { //3
                        string iDRefString = n.Attributes.GetNamedItem ( "idref" ).Value;
                        n.Attributes.GetNamedItem ( "idref" ).Value = iDDictionary[iDRefString];
                        } //-3

                    } //-2

            try
                {
                XmlTextWriter writer = new XmlTextWriter ( m_OpfPath, null );
                writer.Formatting = Formatting.Indented;
                XmlDoc.Save ( writer );
                writer.Close ();
                XmlDoc = null;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void UpdateReferencesInNcx()
            {
            XmlTextReader Reader = new XmlTextReader ( m_NcxPath);
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;
            
            XmlNodeList navPointsList = XmlDoc.GetElementsByTagName ( "navPoint" );

            foreach (XmlNode n in navPointsList)
                {
                string idString = n.Attributes.GetNamedItem ( "id" ).Value;
                string newId = m_InitialString + idString;
                n.Attributes.GetNamedItem ( "id" ).Value = newId;
                }


            XmlNodeList pageTargetsList  = XmlDoc.GetElementsByTagName ( "pageTarget" );

            foreach (XmlNode n in pageTargetsList)
                {
                string idString = n.Attributes.GetNamedItem ( "id" ).Value;
                string newId = m_InitialString + idString;
                n.Attributes.GetNamedItem ( "id" ).Value = newId;
                }
            

            XmlNodeList contentNodeList  = XmlDoc.GetElementsByTagName ( "content");

            for (int i = 0; i < contentNodeList.Count; i++)
                {
                    string smilFileName = contentNodeList[i].Attributes.GetNamedItem ( "src" ).Value;
                int indexOfChar  = smilFileName.IndexOf ("#") ;
                smilFileName = smilFileName.Substring (0 , indexOfChar ) ;
                
                    if (m_SmilMap.ContainsKey ( smilFileName ))
                        {
                        string attributeValue = contentNodeList[i].Attributes.GetNamedItem ( "src" ).Value ;
                        attributeValue =  attributeValue.Replace ( smilFileName, m_SmilMap[smilFileName] );

                        contentNodeList[i].Attributes.GetNamedItem ( "src" ).Value = attributeValue;

                        }
                    
                }

XmlNodeList audioNodeList  = XmlDoc.GetElementsByTagName ( "audio");

for (int i = 0; i < audioNodeList.Count; i++)
    {
    string srcValue = audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value;
    audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value = m_AudioFileMap[srcValue];
    }


            try
                {
                XmlTextWriter writer = new XmlTextWriter ( m_NcxPath, null );
                writer.Formatting = Formatting.Indented;
                XmlDoc.Save ( writer );
                writer.Close ();
                XmlDoc = null;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }
            }

        private void UpdateAudioReferencesInAllSmils ()
            {
            string baseDirectoryString = Directory.GetParent ( m_OpfPath).FullName ;
            for ( int i = 0 ; i < m_OriginalSmilList.Count ; i++ )
                {
                UpdateAudioReferencesInSmil ( Path.Combine (baseDirectoryString, m_OriginalSmilList[i]));
                }

            }



        private void UpdateAudioReferencesInSmil ( string smilFilePath )
            {
            XmlTextReader Reader = new XmlTextReader ( smilFilePath);
            Reader.XmlResolver = null;

            XmlDocument XmlDoc = new XmlDocument ();
            XmlDoc.XmlResolver = null;
            XmlDoc.Load ( Reader );
            Reader.Close ();
            Reader = null;

XmlNodeList audioNodeList = XmlDoc.GetElementsByTagName ( "audio" );

            for (int i = 0; i < audioNodeList.Count; i++)
                {
                string srcValue = audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value;
                audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value = m_AudioFileMap[srcValue];
                }


            try
                {
                XmlTextWriter writer = new XmlTextWriter ( smilFilePath, null );
                writer.Formatting = Formatting.Indented;
                XmlDoc.Save ( writer );
                writer.Close ();
                XmlDoc = null;
                }
            catch (System.Exception ex)
                {
                MessageBox.Show ( ex.ToString () );
                }

            }


        private void RenameSmilAndAudioFiles  ()
            {
            string baseDirectoryPath = Directory.GetParent ( m_OpfPath ).FullName;

            foreach (string strKey in m_SmilMap.Keys)
                {
                string sourcePath = Path.Combine ( baseDirectoryPath, strKey );
                string destinationPath = Path.Combine ( baseDirectoryPath, m_SmilMap[strKey] );

                File.Move ( sourcePath, destinationPath );
                //MessageBox.Show ( destinationPath );
                }


            foreach (string strKey in m_AudioFileMap.Keys)
                {
                string sourcePath = Path.Combine ( baseDirectoryPath, strKey );
                string destinationPath = Path.Combine ( baseDirectoryPath, m_AudioFileMap[strKey] );

                File.Move ( sourcePath, destinationPath );
                //MessageBox.Show ( destinationPath );
                }

            }


        }
    }
