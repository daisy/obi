using System;
using System.Xml;

using urakawa.daisy;

namespace Obi.ImportExport
{
    
    public class ExportStructure 
    {
        public ExportStructure()
        {
    }

    private void CreateElementsForSection ( XmlDocument nccDocument, SectionNode section, int sectionIndex )
            {
                        string nccFileName = "ncc.html";
            XmlNode bodyNode = nccDocument.GetElementsByTagName ( "body" )[0];
            XmlNode headingNode = nccDocument.CreateElement ( null, "h" + section.Level.ToString (), bodyNode.NamespaceURI );
            if (sectionIndex == 0)
                {
                XmlDocumentHelper.CreateAppendXmlAttribute ( nccDocument, headingNode, "class", "title" );
                }
            else
                {
                    XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, headingNode, "class", "section");
                }
                string headingID = "h"+ IncrementID;
            XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, headingNode, "id", headingID);
            bodyNode.AppendChild ( headingNode );

                        bool isFirstPhrase = true;
            //EmptyNode adjustedPageNode = m_NextSectionPageAdjustmentDictionary[section];
            bool isPreviousNodeEmptyPage = false;
            
            for (int i = 0; i < section.PhraseChildCount ; i++)
                {//1
                    EmptyNode phrase = null;
                //first handle the first phrase of the project if it is also the page
                // in such a case i=0 will be skipped being page, second phrase is exported and then first page is inserted to i=2 
                    if (i == 2 )
                    {//2
                        //phrase = m_FirstPageNumberedPhraseOfFirstSection;
                        //m_FirstPageNumberedPhraseOfFirstSection = null;
                        --i;
                    }//-2
                    else if (i < section.PhraseChildCount  ) 
                    {//2
                        phrase = section.PhraseChild(i);
                    }//-2
                    else
                    {   //2
                        //phrase = adjustedPageNode;
                        //adjustedPageNode = null;
                    }//-2
                    if (phrase.Role_ == EmptyNode.Role.Page && isFirstPhrase && i < section.PhraseChildCount) 
                    {   //2
                        continue;
            }//-2

                if ((phrase is PhraseNode && phrase.Used)
                    || ( phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page  &&  phrase.Used))
                    {//2
                    
                    string pageID = null;
                    XmlNode pageNode = null;
                    if (!isFirstPhrase && phrase.Role_ == EmptyNode.Role.Page)
                        {//3
                        string strClassVal = null;
                        // increment page counts and get page kind
                        switch (phrase.PageNumber.Kind)
                            {//4
                        case PageKind.Front:
                        //m_PageFrontCount++;
                        strClassVal = "page-front";
                        break;

                        case PageKind.Normal:
                        //m_PageNormalCount++;
                        //if (phrase.PageNumber.Number > m_MaxPageNormal) m_MaxPageNormal = phrase.PageNumber.Number;
                        strClassVal = "page-normal";
                        break;

                        case PageKind.Special:
                        //m_PageSpecialCount++;
                        strClassVal = "page-special";
                        break;

                            }//-4

                        pageNode = nccDocument.CreateElement ( null, "span", bodyNode.NamespaceURI );
                        XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, pageNode, "class", strClassVal);
                        pageID = "p" + IncrementID;
                        XmlDocumentHelper.CreateAppendXmlAttribute(nccDocument, pageNode, "id", pageID);
                        bodyNode.AppendChild ( pageNode );

                        }//-3

                        // add anchor and href to ncc elements
                        //XmlNode anchorNode = nccDocument.CreateElement ( null, "a", bodyNode.NamespaceURI );

                        //if (isFirstPhrase)
                            //{
                            //headingNode.AppendChild ( anchorNode );
                            //CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );
                            //anchorNode.AppendChild (
                                //nccDocument.CreateTextNode ( section.Label ) );
                            //}
                        //else if (pageNode != null)
                            //{

                            //pageNode.AppendChild ( anchorNode );
                            //CreateAppendXmlAttribute ( nccDocument, anchorNode, "href", smilFileName + "#" + txtID );

                            //anchorNode.AppendChild (
                                //nccDocument.CreateTextNode ( phrase.PageNumber.ToString () ) );

                            //}
                        //}

                    
                    isFirstPhrase = false;

                    if (phrase is EmptyNode && phrase.Role_ == EmptyNode.Role.Page)
                    {
                        isPreviousNodeEmptyPage = true;
                    }
                    else
                    {
                        isPreviousNodeEmptyPage = false;
                    }
                    
                } // for loop ends

            
                }
            }

        private int m_IdCounter;
        private string IncrementID { get { return (++m_IdCounter).ToString(); } }
            }
}
