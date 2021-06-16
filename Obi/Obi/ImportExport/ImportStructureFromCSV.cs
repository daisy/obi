using System;
using System.Collections.Generic;
using System.IO;
using urakawa.command;

namespace Obi.ImportExport
{
    /// <summary>
    /// imports the CSV file
    /// </summary>
    public class ImportStructureFromCSV 
    {
        private ObiPresentation m_Presentation ;
        private List<string> m_audioFilePath = new List<string>();
        private ProjectView.ProjectView m_ProjectView;

        public ImportStructureFromCSV()
        {
        }

        public void ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation, ProjectView.ProjectView projectView)
        {
            m_Presentation = presentation;
            m_ProjectView = projectView;
            List<int> levelsList = new List<int>();
            List<string> sectionNames = new List<string>();
            List<int> pagesPerSection = new List<int>();
            //levelsList.Add(1);
            //levelsList.Add(2);
            //sectionNames.Add("first");
            //sectionNames.Add("second");
            //pagesPerSection.Add(0);
            //pagesPerSection.Add(2);
            ReadListsFromCSVFile(levelsList, sectionNames, pagesPerSection, CSVFullPath);
            CreateStructure(levelsList, sectionNames, pagesPerSection, m_audioFilePath);
        }

        public List<string> AudioFilePaths
        {
            get
            {
                return m_audioFilePath;
            }
        }
        private void ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, string CSVFullPath)
        {
            string[] linesInFiles = File.ReadAllLines(CSVFullPath);


            List<string> audioFilePath = new List<string>();
                    

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
                        if (i == 3)
                        {
                            if (cellsInLineArray[i] == string.Empty || string.IsNullOrWhiteSpace(cellsInLineArray[i]))
                            {
                                cellsInLineArray[i] = "Untitled";
                                audioFilePath.Add(string.Empty);

                            }
                            else
                            {
                                if (Path.GetPathRoot(cellsInLineArray[i]) == string.Empty)
                                {
                                    cellsInLineArray[i] = Path.GetDirectoryName(CSVFullPath) + "\\" + cellsInLineArray[i];
                                }
                                string filePath = cellsInLineArray[i]; 
                                audioFilePath.Add(filePath);
                            }
                        }
                        
                    }

                }

            }
            Console.WriteLine("lists loaded");
            m_audioFilePath = audioFilePath;
        }


        private void CreateStructure(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, List<string> audioFilePath)
        {
            List<ObiNode> listOfSectionNodes = new List<ObiNode>();
            listOfSectionNodes.Add((ObiNode)m_Presentation.RootNode);
            int pageNumber = 0;
            SectionNode currentSection = null;
            Console.WriteLine("level list  count" + levelsList.Count);
            for (int i = 0; i < levelsList.Count; i++)
            {
                SectionNode section = m_Presentation.CreateSectionNode();
                section.Label = sectionNamesList[i].Trim();
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

                if (audioFilePath.Count > i && audioFilePath[i] != null)
                {
                    ImportAudio(audioFilePath[i], section);
                }
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
                //if (audioFilePath.Count > i && audioFilePath[i] != null)
                //{
                //    ImportAudio(audioFilePath[i], section);
                //}
            }
        }

        public void ImportAudio(string path,SectionNode sectionNode)
        {
             List<string> tempAudioFilePaths = new List<string>();
             string[] tempAudioFilePathsArray = new string[1];

             if (path != string.Empty && System.IO.File.Exists(path) && (System.IO.Path.GetExtension(path).ToLower() == ".wav" || System.IO.Path.GetExtension(path).ToLower() == ".mp3" 
                 || System.IO.Path.GetExtension(path).ToLower() == ".mp4" || System.IO.Path.GetExtension(path).ToLower() == ".m4a"))
            {
                tempAudioFilePathsArray[0] = path;
                tempAudioFilePathsArray = Audio.AudioFormatConverter.ConvertFiles(tempAudioFilePathsArray, m_Presentation);
            }
            else
                tempAudioFilePathsArray[0] = string.Empty;

            path = tempAudioFilePathsArray[0];

            try
            {
                if (path != string.Empty)
                {
                    PhraseNode phraseNode = m_Presentation.CreatePhraseNode(path);

                    if (phraseNode != null)
                        m_Presentation.Do(this.GetCommandForImportAudioFileInEachSection(phraseNode, sectionNode));
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(path + ": " + e.Message, Localizer.Message("Caption_Error"));
            }
        }


        private CompositeCommand GetCommandForImportAudioFileInEachSection(PhraseNode phraseNode, SectionNode section)
        {
            CompositeCommand command = m_Presentation.CreateCompositeCommand(Localizer.Message("import_phrases"));
            Commands.Node.AddNode addCmd = new Commands.Node.AddNode(m_ProjectView, phraseNode, section, section.PhraseChildCount, false);
            command.ChildCommands.Insert(command.ChildCommands.Count, addCmd);
            return command;
        }

    }
}
