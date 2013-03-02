using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using urakawa.core;
using urakawa.daisy;
using urakawa.xuk;

namespace Obi.ImportExport
{
    public class DAISY202Import
    {
        private string m_NccPath;
        private ObiPresentation m_Presentation;            // target presentation
        private SectionNode m_CurrentSection;               // section currently populated
        private Stack<Obi.SectionNode> m_OpenSectionNodes;  // these are section nodes that could accept children


        public DAISY202Import(string nccPath, ObiPresentation presentation)
        {
            m_NccPath = nccPath;
            m_OpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            m_Presentation = presentation;
        }
        public static String GrabTitle(string filePath)
        {
            Uri fileUri = new Uri(filePath);
            return GrabTitle(fileUri);
        }

        public static String GrabTitle(Uri fileUri)
        {
            XmlTextReader source = GetXmlReader(fileUri);
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            String title = null;
            try
            {
                title = source.ReadToFollowing("title") ? source.ReadString() : Localizer.Message("default_project_title");
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                source.Close();
            }
            return title;
        }

        public void ImportFromXHTML()
        {
            XmlDocument nccDocument = XmlReaderWriterHelper.ParseXmlDocument(m_NccPath, false, false);
            
            XmlNode bodyNode = nccDocument.GetElementsByTagName("body")[0];
            ParseNccDocument(bodyNode);
        }

        private void ParseNccDocument(XmlNode node)
        {
            if ( node.ChildNodes.Count == 0 ) return ;
            foreach (XmlNode n in node.ChildNodes)
            {
                                    if (n.NodeType == XmlNodeType.Element)
                    {
                        if (n.LocalName == "h1" || n.LocalName == "h2" || n.LocalName == "h3" ||
                            n.LocalName == "h4" || n.LocalName == "h5" || n.LocalName == "h6")
                        {
                            //foundHeadings = true;
                            Obi.SectionNode section = CreateSectionNode(n);
                            if (section  != null && section.Level < 6) m_OpenSectionNodes.Push(section );
                            m_CurrentSection = section ;
                        }
                        else if ((n.LocalName == "p" || n.LocalName == "span"))
                        {
                            string classAttr = n.Attributes.GetNamedItem("class").Value;
                            if (classAttr == "page" || classAttr == "page-normal")
                            {
                                addPage(n, PageKind.Normal);
                            }
                            else if (classAttr == "page-front")
                            {
                                addPage(n, PageKind.Front);
                            }
                            else if (classAttr == "page-special")
                            {
                                addPage(n, PageKind.Special);
                            }
                        }
                                    }          
                ParseNccDocument(n);
            }
        }

        // Add a page of the given kind; parse the content to get the page number.
        private void addPage(XmlNode node, PageKind kind)
        {
            if (m_CurrentSection == null) throw new Exception(Localizer.Message("error_adding_page_number"));
            string pageNumberString = GetTextContent(node);
            PageNumber number = null;
            if (kind == PageKind.Special && pageNumberString != null && pageNumberString != "")
            {
                number = new PageNumber(pageNumberString);
            }
            else if (kind == PageKind.Front || kind == PageKind.Normal)
            {
                int pageNumber = EmptyNode.SafeParsePageNumber(pageNumberString);
                if (pageNumber > 0) number = new PageNumber(pageNumber, kind);
            }
            if (number != null)
            {
                EmptyNode n = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                n.PageNumber = number;
                m_CurrentSection.AppendChild(n);
            }
        }


                    private SectionNode CreateSectionNode(XmlNode node)
        {
            int level = int.Parse(node.LocalName.Substring(1));
            SectionNode parent = getAvailableParent(level);
            SectionNode section = m_Presentation.CreateSectionNode();
            section.Label = GetTextContent(node);
            //if no parent was found, then we must be an h1 sibling or first node
            if (parent == null)
            {
                if (node.LocalName != "h1") throw new Exception(Localizer.Message("wrong_heading_order"));
                m_Presentation.RootNode.AppendChild(section);
            }   
            else parent.AppendChild(section); 
            return section;
        }
        
        /// <summary>
        /// Go through the available section nodes and find one that is allowed to have children with the given level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private SectionNode getAvailableParent(int level)
        {
            int sz = m_OpenSectionNodes.Count;
            for (int i = 0; i < sz; i++)
            {
                if (m_OpenSectionNodes.Peek().Level == level - 1) return m_OpenSectionNodes.Peek();
                //remove the ones we aren't going to use (the stack will have h1 at the bottom, h5 at the top)
                else m_OpenSectionNodes.Pop();
            }
            return null;
        }

        private string GetTextContent(XmlNode node)
        {
            //string elementName = source.Name;
            string allText = node.InnerText ;
            
            return allText;
        
}



        private static XmlTextReader GetXmlReader(Uri uri)
        {
            XmlTextReader reader = new XmlTextReader(uri.ToString());
            reader.XmlResolver = null;
            return reader;
        }
    }
}
