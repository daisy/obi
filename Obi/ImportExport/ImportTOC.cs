using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using urakawa.daisy;

namespace Obi.ImportExport
{
    /// <summary>
    /// imports the CSV,xml files
    /// </summary>
    public class ImportTOC
    {
        private List<int> m_LevelsList  = new List<int>();
        private List<string> m_SectionsNames = new List<string>();

        public ImportTOC()
        {
        }

        public bool ImportFromCSVFile(string CSVFullPath)
        {
            List<int> levelsList = new List<int>();
            List<string> sectionNames = new List<string>();
            bool result = ReadListsFromCSVFile(levelsList, sectionNames, CSVFullPath);
            m_LevelsList = levelsList;
            m_SectionsNames = sectionNames;
            return result;
        }

        public void ImportFromXHTML(string filePath)
        {
            try
            {
                XmlDocument xhtmlDocument = null;

                xhtmlDocument = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(filePath, false, false);

                if (xhtmlDocument != null && xhtmlDocument.DocumentType != null &&
                    xhtmlDocument.DocumentType.ParentNode != null)
                {
                    //XmlNode headNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(
                    //    xhtmlDocument.DocumentElement, true, "head", xhtmlDocument.DocumentElement.NamespaceURI);
                    //PopulateMetadataFromNcc(headNode);
                    XmlNode bodyNode = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(
                        xhtmlDocument.DocumentElement, true, "body", xhtmlDocument.DocumentElement.NamespaceURI);
                    if(bodyNode != null)
                    ParseXHTMLDocument(bodyNode);
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void ParseXHTMLDocument(XmlNode node)
        {
            if (node.ChildNodes.Count == 0) return;
            foreach (XmlNode n in node.ChildNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    if (n.LocalName == "h1" || n.LocalName == "h2" || n.LocalName == "h3" ||
                        n.LocalName == "h4" || n.LocalName == "h5" || n.LocalName == "h6")
                    {
                        int level = int.Parse(n.LocalName.Substring(1));
                        m_LevelsList.Add(level);
                        string tempSectionName =
                            n.InnerText.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                        m_SectionsNames.Add(tempSectionName);
                    }
                }

                ParseXHTMLDocument(n);
            }

        }



        public List<int> LevelsListOfImportedTocList
        {
            get
            {
                return m_LevelsList;
            }
        }
        public List<string> SectionNamesOfImportedTocList
        {
            get
            {
                return m_SectionsNames;
            }
        }

        private bool ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, string CSVFullPath)
        {
            string[] linesInFiles;
            try
            {

                linesInFiles = File.ReadAllLines(CSVFullPath);
            }
            catch (IOException e)
            {
                MessageBox.Show(Localizer.Message("FileInUse"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

            string tempString = "";

            foreach (string line in linesInFiles)
            {
                bool isValid = true;
                Console.WriteLine();
                Console.WriteLine(line);
                string[] cellsInLineArray = null;
                if (Path.GetExtension(CSVFullPath).ToLower() == ".csv")
                {
                    cellsInLineArray = line.Split(',');
                }
                else
                {
                    cellsInLineArray = line.Split('\t');
                }
                for (int i = 0; i < cellsInLineArray.Length; i++)
                {
                    if (i == 0)
                    {
                        int Level;
                        bool CorrectFormat = int.TryParse(cellsInLineArray[i], out Level);
                        if (CorrectFormat && Level > 0)
                        {
                            levelsList.Add(Level);
                        }
                        else if (CorrectFormat && Level < 0)
                        {
                            return false;
                        }
                        else
                        {
                            isValid = false;
                            continue;
                        }
                    }

                    if (isValid)
                    {

                        if (i == 1)
                        {
                            if (cellsInLineArray[i] == "")
                            {
                                cellsInLineArray[i] = "Untitled";
                            }
                            sectionNamesList.Add(cellsInLineArray[i]);
                        }
                    }

                }

            }

            return true;
        }

        //public void ImportFromXMLFile(string filePath)
        //{
        //    try
        //    {
        //        XmlDocument dtbookFileDoc = urakawa.xuk.XmlReaderWriterHelper.ParseXmlDocument(filePath, false, false);

        //        if (dtbookFileDoc != null && dtbookFileDoc.DocumentType != null &&
        //            dtbookFileDoc.DocumentType.ParentNode != null)
        //        {
        //            XmlNode xmlNode = dtbookFileDoc.DocumentType.ParentNode;
        //            parseContentDocument(filePath, xmlNode);
        //            if (m_SectionsNames.Count != m_LevelsList.Count)
        //            {
        //                m_SectionsNames.Add(Localizer.Message("default_section_label"));
        //            }
        //        }

        //    }
        //    catch (IOException e)
        //    {
        //        MessageBox.Show(Localizer.Message("FileInUse"), Localizer.Message("Caption_Error"),
        //            MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        return;
        //    }
        //    catch (Exception e)
        //    {
        //        return;
        //    }

        //}
        //private void parseContentDocument(string filePath, XmlNode xmlNode)
        //{

        //    XmlNodeType xmlType = xmlNode.NodeType;
        //    switch (xmlType)
        //    {
        //        case XmlNodeType.Attribute:
        //            {
        //                System.Diagnostics.Debug.Fail("Calling this method with an XmlAttribute should never happen !!");
        //                break;
        //            }
        //        case XmlNodeType.Document:
        //            {

        //                XmlNode bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "body", null);

        //                if (bodyElement == null)
        //                {
        //                    bodyElement = XmlDocumentHelper.GetFirstChildElementOrSelfWithName(xmlNode, true, "book", null);
        //                }

        //                if (bodyElement != null)
        //                {

        //                    parseContentDocument(filePath, bodyElement);
        //                }
        //                break;
        //            }
        //        case XmlNodeType.Element:
        //            {
        //                if (xmlNode.LocalName == "doctitle")
        //                {
        //                    m_LevelsList.Add(1);
        //                }
        //                if (xmlNode.LocalName.StartsWith("level"))
        //                {
        //                    string tempLevel = xmlNode.LocalName.Replace("level", string.Empty);
        //                    int n;
        //                    bool isNumeric = int.TryParse(tempLevel, out n);

        //                    if (isNumeric)
        //                    {
        //                        if (m_LevelsList.Count != m_SectionsNames.Count)
        //                        {
        //                            m_SectionsNames.Add(Localizer.Message("default_section_label"));
        //                        }
        //                        m_LevelsList.Add(n);
        //                    }
        //                }

        //                if (xmlNode.LocalName == "h1" || xmlNode.LocalName == "h2" || xmlNode.LocalName == "h3"
        //                    || xmlNode.LocalName == "h4" || xmlNode.LocalName == "h5" || xmlNode.LocalName == "h6" ||
        //                    xmlNode.LocalName == "HD" || xmlNode.LocalName == "doctitle")
        //                {
        //                    string tempSectionName =
        //                        xmlNode.InnerText.Replace("\n", "").Replace("\r", "").Replace("\t", "");
        //                    m_SectionsNames.Add(tempSectionName);


        //                }


        //                foreach (XmlNode childXmlNode in xmlNode.ChildNodes)
        //                {
        //                    parseContentDocument(filePath, childXmlNode);
        //                }
        //                break;
        //            }
        //        case XmlNodeType.Whitespace:
        //        case XmlNodeType.CDATA:
        //        case XmlNodeType.SignificantWhitespace:
        //        case XmlNodeType.Text:
        //            {

        //                break;

        //            }
        //        default:
        //            {
        //                return;
        //            }
        //    }
        //}


    }
}
