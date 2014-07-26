using System;
using System.Collections.Generic;
using System.IO;

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
                //levelsList.Add(1);
                //levelsList.Add(2);
                //sectionNames.Add("first");
                //sectionNames.Add("second");
                //pagesPerSection.Add(0);
                //pagesPerSection.Add(2);
                ReadListsFromCSVFile(levelsList, sectionNames, pagesPerSection, CSVFullPath);
                CreateStructure(levelsList, sectionNames, pagesPerSection);
        }

        private void ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, string CSVFullPath)
        {
            string[] linesInFiles = File.ReadAllLines(CSVFullPath);
            
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
                            Console.WriteLine("section parsing : " + cellsInLineArray[i]);
                        }
                        if (i == 2)
                        {

                            int Pages;
                            bool CorrectFormat = int.TryParse(cellsInLineArray[i], out Pages);
                            if (CorrectFormat)
                            {
                                pagesPerSection.Add(Pages);
                            }
                            else
                            {
                                pagesPerSection.Add(0);
                            }

                        }
                    }

                }

            }
            Console.WriteLine("lists loaded");
        }

        private void CreateStructure(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection)
        {
            List<ObiNode> listOfSectionNodes = new List<ObiNode>();
            listOfSectionNodes.Add((ObiNode)m_Presentation.RootNode);
            int pageNumber = 0;
            SectionNode currentSection = null;
            Console.WriteLine("level list  count" + levelsList.Count);
            for (int i = 0; i < levelsList.Count; i++)
            {
                SectionNode section = m_Presentation.CreateSectionNode();
                section.Label = sectionNamesList[i];
                Console.WriteLine("section " + section.Label + ", level: " + levelsList[i]);
                if (currentSection == null)
                {   
                    m_Presentation.RootNode.AppendChild(section);
                }
                else
                {
                // iterate back in list of sections to find the parent
                    for (int j = listOfSectionNodes.Count - 1; j >= 0; j--)
                    {
                        ObiNode iterationSection = listOfSectionNodes[j];
                        if (iterationSection.Level < levelsList[i])
                        {
                            iterationSection.AppendChild(section);
                            break;
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
