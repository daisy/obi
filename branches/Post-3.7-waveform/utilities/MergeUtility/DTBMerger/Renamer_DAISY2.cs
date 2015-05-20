using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Windows.Forms;

namespace DTBMerger
    {
    public partial class Renamer
        {
        private Dictionary<string, string> m_NccIDMap = new Dictionary<string, string> ();


        public void Rename2_02DTBFilesSet ()
            {
            DTBFilesInfo dtbInfo = new DTBFilesInfo ( m_OpfPath );
            m_OriginalSmilList = dtbInfo.SmilFilesList;
            m_OriginalAudioFileList = dtbInfo.AudioFilesList;
            GenerateSmilMapAndAudioMapDictionary ();
            UpdateNccReferencesInDAISY2 ();
            UpdateAllSmilForDAISY2 ();
            RenameSmilAndAudioFiles ();
            }


        private void UpdateNccReferencesInDAISY2 ()
            {

            XmlDocument nccDocument = CommonFunctions.CreateXmlDocument ( m_OpfPath );

            // change smil references in anchor nodes
            XmlNodeList anchorNodesList = nccDocument.GetElementsByTagName ( "a" );
            foreach (XmlNode n in anchorNodesList)
                {

                if (n.Attributes.GetNamedItem ( "href" ) != null)
                    {
                    string strSmilRef = n.Attributes.GetNamedItem ( "href" ).Value;
                    string smilFileName = strSmilRef.Split ( '#' )[0];
                    if (m_SmilMap.ContainsKey ( smilFileName ))
                        {
                        n.Attributes.GetNamedItem ( "href" ).Value =
                            strSmilRef.Replace ( smilFileName, m_SmilMap[smilFileName] );
                        }
                    } // attribute null check
                }// foreach for anchor nodes ends

            // update ids
            XmlNode bodyNode = nccDocument.GetElementsByTagName ( "body" )[0];

            TraverseNccAndUpdateIDs ( bodyNode );

            CommonFunctions.WriteXmlDocumentToFile ( nccDocument, m_OpfPath );
            }


        private void TraverseNccAndUpdateIDs ( XmlNode node )
            {

            if (node.NodeType == XmlNodeType.Element && node.Attributes.GetNamedItem ( "id" ) != null)
                {
                string oldID = node.Attributes.GetNamedItem ( "id" ).Value;
                string newID = m_InitialString + oldID;
                node.Attributes.GetNamedItem ( "id" ).Value = newID;
                m_NccIDMap.Add ( oldID, newID );
                }

            foreach (XmlNode n in node.ChildNodes)
                {
                TraverseNccAndUpdateIDs ( n );
                }
            }

        private void UpdateAllSmilForDAISY2 ()
            {
            string baseDirectoryString = Directory.GetParent ( m_OpfPath ).FullName;
            for (int i = 0; i < m_OriginalSmilList.Count; i++)
                {
                UpdateReferencesInSmilFileForDAISY2 ( Path.Combine ( baseDirectoryString, m_OriginalSmilList[i] ) );
                }
            }



        private void UpdateReferencesInSmilFileForDAISY2 ( string smilFilePath )
            {

            XmlDocument XmlDoc = CommonFunctions.CreateXmlDocument ( smilFilePath );

            XmlNodeList audioNodeList = XmlDoc.GetElementsByTagName ( "audio" );

            for (int i = 0; i < audioNodeList.Count; i++)
                {
                string srcValue = audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value;
                audioNodeList[i].Attributes.GetNamedItem ( "src" ).Value = m_AudioFileMap[srcValue];
                }

            // update src for text nodes for headings and pages in ncc file
            XmlNodeList txtNodesList = XmlDoc.GetElementsByTagName ( "text" );

            foreach (XmlNode n in txtNodesList)
                {
                if (n.Attributes.GetNamedItem ( "src" ) != null)
                    {
                    string originalReference = n.Attributes.GetNamedItem ( "src" ).Value;
                    string refFragment = originalReference.Split ( '#' )[1];

                    if (refFragment != null && m_NccIDMap.ContainsKey ( refFragment ))
                        {
                        string newReference = originalReference.Replace ( refFragment, m_NccIDMap[refFragment] );
                        n.Attributes.GetNamedItem ( "src" ).Value = newReference;
                        }
                    }
                }



            CommonFunctions.WriteXmlDocumentToFile ( XmlDoc, smilFilePath );
            }






        }
    }
