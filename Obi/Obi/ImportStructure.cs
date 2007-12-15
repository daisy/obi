using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Obi
{
    /// <summary>
    /// Import an external file (XHTML) and use it to fill in the sections in a newly-created project
    /// </summary>
    class ImportStructure
    {
        private Obi.Presentation mPresentation;
        private SectionNode mLastSection;
        private Obi.ProjectView.ProjectView mView;

        //these are section nodes that could accept children
        private System.Collections.Generic.Stack <Obi.SectionNode> mOpenSectionNodes;
        
        public ImportStructure(){}

        //this first pass will translate h1-h6 elements to SectionNodes
        public void ImportFromXHTML(String xhtmlDocument, Obi.Presentation presentation, Obi.ProjectView.ProjectView view)
        {            
            mOpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            mPresentation = presentation;
            mView = view;
            LoadFromXHTML(new Uri(xhtmlDocument));
        }

        /// <summary>
        /// Utility function to grab the text of the <title> element
        /// </summary>
        /// <param name="fileUri"></param>
        /// <returns></returns>
        public static String grabTitle(Uri fileUri)
        {
            XmlTextReader source = new XmlTextReader(fileUri.ToString());
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            
            String title = "";
            if (!source.ReadToFollowing("title")) title = "";
            else title = source.ReadString();

            source.Close();
            return title;
        }

        //starts the process of creating SectionNode's from h1...h6 elements
        private void LoadFromXHTML(Uri fileUri)
        {
            XmlTextReader source = new XmlTextReader(fileUri.ToString());
            source.WhitespaceHandling = WhitespaceHandling.Significant;
            bool foundHeadings = false;
            mLastSection = null;

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
                            Obi.SectionNode node = createSectionNode(source);
                            if (node != null && node.Level < 6) mOpenSectionNodes.Push(node);
                            mLastSection = node;
                        }
                        else if ((source.LocalName == "p" || source.LocalName == "span") && 
                            source.GetAttribute("class") == "page")
                        {
                            addPage(source);
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
            if (foundHeadings == false) throw new Exception("No headings found.");
        }

        private void addPage(XmlTextReader source)
        {
            if (mLastSection == null) throw new Exception("Error adding page number: no parent section found");
            
            int pageNumber;
            string pageNumberString = getElementText(source);
            if (!int.TryParse(pageNumberString, out pageNumber)) return;
            EmptyNode node = mPresentation.CreatePhraseNode();
            mLastSection.AppendChild(node);
            node.PageNumber = pageNumber;
        }

        //source points to a heading element (h1 through h6)
        private SectionNode createSectionNode(XmlTextReader source)
        {
            //get the level of this node.  
            int level = int.Parse(source.LocalName.Substring(1));
            SectionNode parent = getAvailableParent(level);
            SectionNode section = mPresentation.CreateSectionNode();
            //if no parent was found, then we must be an h1 sibling or first node
            if (parent == null)
            {
                if (source.LocalName != "h1") throw new Exception("Heading element ordering is wrong.");
                mPresentation.RootNode.AppendChild(section);
            }
            else parent.AppendChild(section);

       //     section.Label = getElementText(source);
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

        private string getElementText(XmlTextReader source)
        {
            string elementName = source.Name;
            string allText = "";
            allText += source.ReadString();
            //wait for the end tag; acculumate the text inside
            while (!(source.NodeType == XmlNodeType.EndElement && source.Name == elementName))
            {
                allText += source.ReadString();
                source.Read();
            }
            return allText;
        }
    }
}
