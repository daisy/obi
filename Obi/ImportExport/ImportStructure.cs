using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Obi.ImportExport
{
    /// <summary>
    /// Import an external file (XHTML) and use it to fill in the sections in a newly-created project.
    /// </summary>
    public class ImportStructure
    {
        private ObiPresentation mPresentation;            // target presentation
        private SectionNode mCurrentSection;               // section currently populated
        private Stack<Obi.SectionNode> mOpenSectionNodes;  // these are section nodes that could accept children

        
        /// <summary>
        /// Simple constructor.
        /// </summary>
        public ImportStructure() {}


        /// <summary>
        /// Grab the title element to seed the title info.
        /// If no title element is found, use a default title.
        /// </summary>
        public static String GrabTitle(Uri fileUri)
        {
            XmlTextReader source = GetXmlReader(fileUri);
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            String title = null;
            try
            {
                title = source.ReadToFollowing("title") ? source.ReadString() : Localizer.Message("default_project_title");
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                source.Close();
            }
            return title;
        }

        /// <summary>
        /// Populate the presentation from an XHTML file.
        /// </summary>
        public void ImportFromXHTML(string xhtml_path, ObiPresentation presentation)
        {
            mOpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            mPresentation = presentation;
            LoadFromXHTML(new Uri(xhtml_path));
        }


        // Get an XML reader without a resolver so that the DTD is skipped
        private static XmlTextReader GetXmlReader(Uri uri)
        {
            XmlTextReader reader = new XmlTextReader(uri.ToString());
            reader.XmlResolver = null;
            return reader;
        }

        // Starts the process of creating SectionNode's from h1...h6 elements
        private void LoadFromXHTML(Uri fileUri)
        {
            XmlTextReader source = GetXmlReader(fileUri);
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            bool foundHeadings = false;
            mCurrentSection = null;
            try
            {
                while (source.Read())
                {
                    if (source.NodeType == XmlNodeType.Element)
                    {
                        if (source.LocalName == "h1" || source.LocalName == "h2" || source.LocalName == "h3" ||
                            source.LocalName == "h4" || source.LocalName == "h5" || source.LocalName == "h6")
                        {
                            foundHeadings = true;
                            Obi.SectionNode node = CreateSectionNode(source);
                            if (node != null && node.Level < 6) mOpenSectionNodes.Push(node);
                            mCurrentSection = node;
                        }
                        else if ((source.LocalName == "p" || source.LocalName == "span"))
                        {
                            string classAttr = source.GetAttribute("class");
                            if (classAttr == "page" || classAttr == "page-normal")
                            {
                                addPage(source, PageKind.Normal);
                            }
                            else if (classAttr == "page-front")
                            {
                                addPage(source, PageKind.Front);
                            }
                            else if (classAttr == "page-special")
                            {
                                addPage(source, PageKind.Special);
                            }
                        }
                    }
                    if (source.EOF) break;
                }
            }
            catch (Exception e)
            {
                source.Close();
                throw e;
            }
            if (!foundHeadings) throw new Exception(Localizer.Message("no_headings_found"));
        }

        // Add a page of the given kind; parse the content to get the page number.
        private void addPage(XmlTextReader source, PageKind kind)
        {
            if (mCurrentSection == null) throw new Exception(Localizer.Message("error_adding_page_number"));
            string pageNumberString = GetTextContent(source);
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
                EmptyNode node = mPresentation.TreeNodeFactory.Create<EmptyNode>();
                node.PageNumber = number;
                mCurrentSection.AppendChild(node);
            }
        }

        // Create a section node for a heading element (where the XML reader currently is)
        private SectionNode CreateSectionNode(XmlTextReader source)
        {
            int level = int.Parse(source.LocalName.Substring(1));
            SectionNode parent = getAvailableParent(level);
            SectionNode section = mPresentation.CreateSectionNode();
            section.Label = GetTextContent(source);
            //if no parent was found, then we must be an h1 sibling or first node
            if (parent == null)
            {
                if (source.LocalName != "h1") throw new Exception(Localizer.Message("wrong_heading_order"));
                mPresentation.RootNode.AppendChild(section);
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
            int sz = mOpenSectionNodes.Count;
            for (int i = 0; i < sz; i++)
            {
                if (mOpenSectionNodes.Peek().Level == level - 1) return mOpenSectionNodes.Peek();
                //remove the ones we aren't going to use (the stack will have h1 at the bottom, h5 at the top)
                else mOpenSectionNodes.Pop();
            }
            return null;
        }

        // Get the text content of an element
        private string GetTextContent(XmlTextReader source)
        {
            string elementName = source.Name;
            string allText = "";
            allText += source.ReadString();
            // wait for the end tag; acculumate the text inside
            while (!(source.NodeType == XmlNodeType.EndElement && source.Name == elementName))
            {
                allText += source.ReadString();
                source.Read();
            }
            return allText;
        }
    }
}
