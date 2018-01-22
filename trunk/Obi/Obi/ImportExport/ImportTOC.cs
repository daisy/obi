using System;
using System.Collections.Generic;
using System.IO;

namespace Obi.ImportExport
{
    /// <summary>
    /// imports the CSV file
    /// </summary>
    public class ImportTOC
    {
        private ObiPresentation m_Presentation;
        private List<int> m_LevelsList;
        private List<string> m_SectionsNames;

        public ImportTOC()
        {
        }

        public void ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation)
        {
            m_Presentation = presentation;
            List<int> levelsList = new List<int>();
            List<string> sectionNames = new List<string>();
            ReadListsFromCSVFile(levelsList, sectionNames, CSVFullPath);
            m_LevelsList = levelsList;
            m_SectionsNames = sectionNames;

        }


        public List<int> LevelsListOfImportedCSV
        {
            get
            {
                return m_LevelsList;
            }
        }
        public List<string> SectionNamesOfImportedCSV
        {
            get
            {
                return m_SectionsNames;
            }
        }

        private void ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, string CSVFullPath)
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
                        }
                    }

                }

            }
            Console.WriteLine("lists loaded");
        }

    }
}
