using System;
using System.Collections.Generic;
using System.IO;
using urakawa.command;
using System.Windows.Forms;
using System.Reflection;

namespace Obi.ImportExport
{
    /// <summary>
    /// imports the CSV file
    /// </summary>
    public class ImportStructureFromCSV 
    {
        private ObiPresentation m_Presentation ;
        private List<string> m_audioFilePath = new List<string>();
        List<string> m_AudioFilePath1 = new List<string>();
        List<string> m_AudioFilePath2 = new List<string>();
        List<string> m_AudioFilePath3 = new List<string>();
        List<string> m_AudioFilePath4 = new List<string>();
        List<string> m_AudioFilePath5 = new List<string>();
        private ProjectView.ProjectView m_ProjectView;
        private bool m_IsPhraseDetectionSettingsShown = false;
        private long m_Threshold;
        private double m_Gap;
        private double m_LeadingSilence;
        private string m_audioFilesNotImported = string.Empty;

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

        public string AudioFilesNotImported
        {
            get
            {
                return m_audioFilesNotImported;
            }
        }
        private void ReadListsFromCSVFile(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, string CSVFullPath)
        {
            string[] linesInFiles = File.ReadAllLines(CSVFullPath);


                    

            foreach (string line in linesInFiles)
            {
                bool isValid = true;
                Console.WriteLine();
                Console.WriteLine(line);
                string[] cellsInLineArray = null;
                if (Path.GetExtension(CSVFullPath).ToLower() == ".csv")
                {
                    if (m_ProjectView.ObiForm.Settings.Project_CSVImportHavingSemicolon)                    
                        cellsInLineArray = line.Split(';');                    
                    else
                        cellsInLineArray = line.Split(',');

                    if(cellsInLineArray.Length>2)
                    {
                        if (cellsInLineArray[1].Contains("\"\"\""))
                        {
                            int indexOfEndOfDoubleQuotes = 0;
                            for(int index = 2; index < cellsInLineArray.Length; index++)
                            {
                                if (cellsInLineArray[index].Contains("\"\"\""))
                                {
                                    indexOfEndOfDoubleQuotes = index;
                                    break;
                                }
                            }
                            string sectionName = string.Empty;
                            if (indexOfEndOfDoubleQuotes > 0)
                            {
                                for (int i = 1; i <= indexOfEndOfDoubleQuotes; i++)
                                {
                                    if (i == indexOfEndOfDoubleQuotes)
                                    {
                                        sectionName += cellsInLineArray[i].Replace("\"\"\"", string.Empty);
                                        cellsInLineArray[i] = string.Empty;
                                        break;
                                    }
                                    else
                                    {
                                        sectionName += cellsInLineArray[i].Replace("\"\"\"", string.Empty) + ",";
                                    }
                                    cellsInLineArray[i] = string.Empty;
                                }
                            }


                            cellsInLineArray[1] = sectionName;

                            int j = 2;
                            for (int i = 2; i < cellsInLineArray.Length; i++)
                            {
                                if (cellsInLineArray[i] != string.Empty)
                                {

                                    if (cellsInLineArray[j] == string.Empty)
                                    {
                                        cellsInLineArray[j] = cellsInLineArray[i];
                                        cellsInLineArray[i] = string.Empty;
                                        j++;
                                    }
                                
                                }

                            }

                        }

                    }
                    if(cellsInLineArray.Length > 1) 
                    {
                        if (cellsInLineArray[1].Contains("\""))
                        {
                            cellsInLineArray[1] = cellsInLineArray[1].Replace("\"", string.Empty);
                        }
                    }

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
                        if (i == 3 || i == 4 || i == 5 || i == 6 || i == 7 )
                        {
                            if (cellsInLineArray[i] == string.Empty || string.IsNullOrWhiteSpace(cellsInLineArray[i]))
                            {
                                cellsInLineArray[i] = "Untitled";
                                if (i == 3)
                                    m_AudioFilePath1.Add(string.Empty);
                                else if (i == 4)
                                    m_AudioFilePath2.Add(string.Empty);
                                else if (i == 5)
                                    m_AudioFilePath3.Add(string.Empty);
                                else if (i == 6)
                                    m_AudioFilePath4.Add(string.Empty);
                                else if (i == 7)
                                    m_AudioFilePath5.Add(string.Empty);
                            }
                            else
                            {
                                try
                                {
                                    cellsInLineArray[i] = cellsInLineArray[i].Trim();
                                    if (Path.GetPathRoot(cellsInLineArray[i]) == string.Empty)
                                    {
                                        cellsInLineArray[i] = Path.GetDirectoryName(CSVFullPath) + "\\" + cellsInLineArray[i];
                                    }
                                    string filePath = cellsInLineArray[i];
                                    if (i == 3)
                                        m_AudioFilePath1.Add(filePath);
                                    else if (i == 4)
                                        m_AudioFilePath2.Add(filePath);
                                    else if (i == 5)
                                        m_AudioFilePath3.Add(filePath);
                                    else if (i == 6)
                                        m_AudioFilePath4.Add(filePath);
                                    else if (i == 7)
                                        m_AudioFilePath5.Add(filePath);
                                }
                                catch (ArgumentException ex)
                                {
                                    string audioFile = string.Empty;
                                    for (int j = i; j < cellsInLineArray.Length; j++)
                                    {
                                        //audioFile.Trim(new Char[] { '"' });
                                        audioFile += cellsInLineArray[j];
                                    }

                                    audioFile = audioFile.Replace("\"", string.Empty).Trim();
                                    m_audioFilesNotImported += ", " + audioFile;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.ToString());
                                }
                            }
                        }
                        
                    }

                }

            }
            Console.WriteLine("lists loaded");
            m_audioFilePath = m_AudioFilePath1;
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

                if (m_AudioFilePath1.Count > i && m_AudioFilePath1[i] != null)
                {
                    ImportAudio(m_AudioFilePath1[i], section);
                }
                if (m_AudioFilePath2.Count > i && m_AudioFilePath2[i] != null)
                {
                    ImportAudio(m_AudioFilePath2[i], section);
                }
                if (m_AudioFilePath3.Count > i && m_AudioFilePath3[i] != null)
                {
                    ImportAudio(m_AudioFilePath3[i], section);
                } if (m_AudioFilePath4.Count > i && m_AudioFilePath4[i] != null)
                {
                    ImportAudio(m_AudioFilePath4[i], section);
                }
                if (m_AudioFilePath5.Count > i && m_AudioFilePath5[i] != null)
                {
                    ImportAudio(m_AudioFilePath5[i], section);
                }
                if (pagesPerSection.Count > i && pagesPerSection[i] > 0)
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
             {
                 tempAudioFilePathsArray[0] = string.Empty;
                 if (path != string.Empty)
                     m_audioFilesNotImported +=  ", "+ Path.GetFileName(path);
             }
            if(tempAudioFilePathsArray.Length != 0)
              path = tempAudioFilePathsArray[0];

            if (m_ProjectView.ObiForm.Settings.Project_CSVImportPhraseDetection && !m_IsPhraseDetectionSettingsShown)
            {
                m_Threshold = (long)m_ProjectView.ObiForm.Settings.Audio_DefaultThreshold;
                m_Gap = (double)m_ProjectView.ObiForm.Settings.Audio_DefaultGap;
                m_LeadingSilence = (double)m_ProjectView.ObiForm.Settings.Audio_DefaultLeadingSilence;
                Dialogs.SentenceDetection sentenceDetection = new Obi.Dialogs.SentenceDetection(m_Threshold, m_Gap, m_LeadingSilence, m_ProjectView.ObiForm.Settings); //@fontconfig
                m_IsPhraseDetectionSettingsShown = true;
                if (sentenceDetection.ShowDialog() == DialogResult.OK)
                {
                    m_Threshold = sentenceDetection.Threshold;
                    m_Gap = sentenceDetection.Gap;
                    m_LeadingSilence = sentenceDetection.LeadingSilence;
                }
            }
            try
            {
                if (path != string.Empty)
                {
                    PhraseNode phraseNode = m_Presentation.CreatePhraseNode(path);

                    if (phraseNode != null)
                        m_Presentation.Do(this.GetCommandForImportAudioFileInEachSection(phraseNode, sectionNode));
                    if (m_ProjectView.ObiForm.Settings.Project_CSVImportPhraseDetection)
                    {
                        ApplyPhraseDetectionOnPhrase(phraseNode, m_Threshold, m_Gap, m_LeadingSilence);
                    }
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

        private void ApplyPhraseDetectionOnPhrase(PhraseNode phraseNode, long threshold, double gap, double before)
        {
            urakawa.command.CompositeCommand phraseDetectionCommand = null;
            phraseDetectionCommand = Commands.Node.SplitAudio.GetPhraseDetectionCommand(m_ProjectView, phraseNode, threshold, gap, before, m_ProjectView.ObiForm.Settings.Audio_MergeFirstTwoPhrasesAfterPhraseDetection,m_Presentation);
            m_Presentation.Do(phraseDetectionCommand);
        }
    }
}
