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
        private Obi.Project mProject;
        private SectionNode mLastSection;

        //these are section nodes that could accept children
        private System.Collections.Generic.Stack <Obi.SectionNode> mOpenSectionNodes;
        
        public ImportStructure(){}

        //this first pass will translate h1-h6 elements to SectionNodes
        public void ImportFromXHTML(String xhtmlDocument, Obi.Project project)
        {
            
            mOpenSectionNodes = new System.Collections.Generic.Stack<Obi.SectionNode>();
            mProject = project;
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
                        else if (source.LocalName == "p" || source.LocalName == "span")
                        {
                            if (source.GetAttribute("class") == "page")
                            {
                                addPage(source);
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
            if (foundHeadings == false) throw new Exception("No headings found.");
        }

        private void addPage(XmlTextReader source)
        {
            if (mLastSection == null) throw new Exception("Error adding page number: no parent section found");

            // TODO mProject.AddEmptyPhraseNode(mLastSection, mLastSection.PhraseChildCount);
            //the phrase we just added should be at the end of the phrase child list
            PhraseNode pagePhrase = mLastSection.PhraseChild(mLastSection.PhraseChildCount - 1);
            pagePhrase.Annotation = getElementText(source);
            //is this the right function to call?
            mProject.DidSetPageNumberOnPhrase(pagePhrase);
           
        }

        //source points to a heading element (h1 through h6)
        //weird behavior:  the first section will always be last, until Project.CreateSiblingSectionNode(..)
        //inserts sections in-order.
        private SectionNode createSectionNode(XmlTextReader source)
        {
            //get the level of this node.  
            int level = int.Parse(source.LocalName.Substring(1));
            SectionNode newNode = null;

            //find the appropriate parent node
            SectionNode parentNode = null;
            int count = mOpenSectionNodes.Count;
            for (int i = 0; i<count; i++)
            {
                if (mOpenSectionNodes.Peek().Level == level - 1)
                {
                    parentNode = mOpenSectionNodes.Peek();
                    break;
                }
                else
                {
                    //pop off the ones we don't need
                    mOpenSectionNodes.Pop();
                }
            }

            //if no parent was found, then we must be an h1 sibling or first node
            if (parentNode == null)
            {
                if (source.LocalName != "h1") throw new Exception("Heading element ordering is wrong.");

                if (mProject.FirstSection == null)
                {
                    newNode = mProject.CreateSiblingSectionNode(null);
                }
                else
                {
                    //walk up the tree to find the last top-level section
                    //is there an easier way to do this?
                    SectionNode lastSection = mProject.LastSection;
                    while (lastSection.ParentSection != null) lastSection = lastSection.ParentSection;
                    newNode = mProject.CreateSiblingSectionNode(lastSection);
                }
            }
            else
            {
                newNode = mProject.CreateChildSectionNode(parentNode);
            }

            if (newNode != null) mProject.RenameSectionNode(newNode, getElementText(source));
            return newNode;
        }

        private string getElementText(XmlTextReader source)
        {
            string elementName = source.Name;
            string allText = "";
            allText += source.ReadString();
           // source.Read();
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
