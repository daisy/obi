using System;
using System.Collections.Generic;

namespace Obi.ImportExport
{
    /// <summary>
    /// imports the CSV file
    /// </summary>
    public class ImportStructureFromCSV 
    {
        private ObiPresentation m_Presentation ;

        public ImportStructureFromCSV()
        {
        }

            public void ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation)
        {   
            m_Presentation = presentation;
        List<int> levelsList = new List<int> ();
                List<string> sectionNames = new List<string>() ;
                List <int> pagesPerSection = new List<int>() ;
                levelsList.Add(1);
                levelsList.Add(2);
                sectionNames.Add("first");
                sectionNames.Add("second");
                pagesPerSection.Add(0);
                pagesPerSection.Add(2);
                CreateStructure(levelsList, sectionNames, pagesPerSection);
        }

        private void ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, string CSVFullPath)
        {

        }

        private void CreateStructure(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection)
        {
            List<SectionNode> listOfSectionNodes = new List<SectionNode>();
            int pageNumber = 0;
            SectionNode currentSection = null;
            Console.WriteLine("level list " + levelsList.Count);
            for (int i = 0; i < levelsList.Count; i++)
            {
                SectionNode section = m_Presentation.CreateSectionNode();
                section.Label = sectionNamesList[i];
                Console.WriteLine("section " + section.Label);
                if (currentSection == null)
                {   
                    m_Presentation.RootNode.AppendChild(section);
                }
                else
                {
                // iterate back in list of sections to find the parent
                    for (int j = listOfSectionNodes.Count - 1; j >= 0; j--)
                    {
                        SectionNode iterationSection = listOfSectionNodes[j];
                        if (iterationSection.Level < levelsList[i])
                        {
                            iterationSection.AppendChild(section);
                        }
                    }
                }
                currentSection = section;
                listOfSectionNodes.Add(section);
                if (pagesPerSection[i] > 0)
                {
                    for(int j = 0; j < pagesPerSection[i]; j++)
                    {
                        EmptyNode pageNode = m_Presentation.TreeNodeFactory.Create<EmptyNode>();
                        ++pageNumber;
                        pageNode.PageNumber = new PageNumber(pageNumber, PageKind.Normal);
                section.AppendChild(pageNode);
                Console.WriteLine("page : " + pageNode.PageNumber.ToString());
                    }
                }
            }
        }

    }
}
