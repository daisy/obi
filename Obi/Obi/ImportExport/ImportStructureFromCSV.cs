using Obi.Dialogs;
using Obi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using urakawa.command;
using Obi.Services;
using System.Threading;

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
        private PhraseDetectionMethod m_PhraseDetectionMethod = PhraseDetectionMethod.None;
        // AI transcription settings
        private TranscriptionSettings? m_TranscriptionSettings;
        private TranscriptionCoordinator? m_TranscriptionCoordinator;
        private CancellationTokenSource? m_CancellationTokenSource;

        public ImportStructureFromCSV()
        {
        }

        public void ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation, ProjectView.ProjectView projectView, ProgressDialog progressDialog)
        {
            m_Presentation = presentation;
            m_ProjectView = projectView;

            // Reset settings for a new CSV import.
            m_TranscriptionSettings = null;
            m_IsPhraseDetectionSettingsShown = false;

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
            if (m_ProjectView.ObiForm.Settings.Project_CSVImportPhraseDetection)
            {
                using (PhraseDetectionMethodDialog dialog = new PhraseDetectionMethodDialog())
                {
                    if (dialog.ShowDialog() != DialogResult.OK)
                    {
                        return;
                    }

                    m_PhraseDetectionMethod = dialog.SelectedMethod;
                    if (m_PhraseDetectionMethod == PhraseDetectionMethod.AI)
                    {
                        using (TranscriptionSettingsDialog settingsDialog =
                            new TranscriptionSettingsDialog())
                        {
                            if (settingsDialog.ShowDialog() != DialogResult.OK)
                            {
                                return;
                            }

                            m_TranscriptionSettings = settingsDialog.SelectedTranscriptionSettings;

                            progressDialog.EnableLog(Path.Combine(ObiPaths.LogsFolder,"AI Transcription Log.txt"));

                            progressDialog.Log("AI Phrase Detection selected.");

                            progressDialog.Log("Transcription Engine: " + m_TranscriptionSettings.Engine.ToString());

                            progressDialog.Log("Language: " + m_TranscriptionSettings.Language.ToString());

                            if (m_TranscriptionSettings.Engine == TranscriptionEngine.Whisper)
                            {
                                progressDialog.Log("Whisper Model: " + m_TranscriptionSettings.WhisperModel.ToString());
                            }
                        }
                        m_TranscriptionCoordinator = new TranscriptionCoordinator(new WhisperXService(), new ParakeetService());
                    }
                }
            }
            else
            {
                m_PhraseDetectionMethod = PhraseDetectionMethod.None;
            }

            m_CancellationTokenSource = new CancellationTokenSource();

            progressDialog.OperationCancelled +=  ProgressDialog_OperationCancelled;

            try
            {
                CreateStructureAsync(levelsList, sectionNames, pagesPerSection, m_audioFilePath, progressDialog).GetAwaiter().GetResult();
            }
            finally
            {
                progressDialog.OperationCancelled -= ProgressDialog_OperationCancelled;

                m_CancellationTokenSource?.Dispose();
                m_CancellationTokenSource = null;
            }
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

        public PhraseDetectionMethod PhraseDetectionMethod
        {
            get
            {
                return m_PhraseDetectionMethod;
            }
            set
            {
                m_PhraseDetectionMethod = value;
            }
        }

        private void ProgressDialog_OperationCancelled(object sender, EventArgs e)
        {
            m_CancellationTokenSource?.Cancel();
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


        private async Task CreateStructureAsync(List<int> levelsList, List<string> sectionNamesList, List<int> pagesPerSection, List<string> audioFilePath, ProgressDialog progressDialog)
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
                    await ImportAudioAsync(m_AudioFilePath1[i], section, progressDialog);
                }
                if (m_AudioFilePath2.Count > i && m_AudioFilePath2[i] != null)
                {
                    await ImportAudioAsync(m_AudioFilePath2[i], section, progressDialog);
                }
                if (m_AudioFilePath3.Count > i && m_AudioFilePath3[i] != null)
                {
                    await ImportAudioAsync(m_AudioFilePath3[i], section, progressDialog);
                } if (m_AudioFilePath4.Count > i && m_AudioFilePath4[i] != null)
                {
                    await ImportAudioAsync(m_AudioFilePath4[i], section, progressDialog);
                }
                if (m_AudioFilePath5.Count > i && m_AudioFilePath5[i] != null)
                {
                    await ImportAudioAsync(m_AudioFilePath5[i], section, progressDialog);
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

        public async Task ImportAudioAsync(string path, SectionNode sectionNode, ProgressDialog progressDialog)
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

            if (m_PhraseDetectionMethod == PhraseDetectionMethod.Traditional && !m_IsPhraseDetectionSettingsShown)
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
                    if (m_PhraseDetectionMethod == PhraseDetectionMethod.AI)
                    {
                        progressDialog.Log("----------------------------------------");

                        progressDialog.Log("Processing audio: " + Path.GetFileName(path));
                        await ImportAIPhrasesAsync(path,sectionNode, progressDialog);
                    }
                    else
                    {
                        PhraseNode phraseNode = m_Presentation.CreatePhraseNode(path);

                        if (phraseNode != null)
                            m_Presentation.Do(this.GetCommandForImportAudioFileInEachSection(phraseNode, sectionNode));
                        if (m_PhraseDetectionMethod == PhraseDetectionMethod.Traditional)
                        {
                            ApplyPhraseDetectionOnPhrase(phraseNode, m_Threshold, m_Gap, m_LeadingSilence);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                throw;
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

        private async Task ImportAIPhrasesAsync(string audioFilePath, SectionNode sectionNode, ProgressDialog progressDialog)
        {
            if (m_TranscriptionCoordinator == null || m_TranscriptionSettings == null)
            {
                throw new InvalidOperationException("AI transcription settings are not initialized.");
            }

            progressDialog.UpdateProgressBar(null,new System.ComponentModel.ProgressChangedEventArgs(0,null));

            progressDialog.Log("Starting AI transcription...");

            progressDialog.Log("Audio file: " + Path.GetFileName(audioFilePath));

            if (m_TranscriptionSettings.Engine == TranscriptionEngine.Whisper)
            {
                if (!await WhisperXInstallerService.IsPythonEnvironmentInstalledAsync())
                {
                    progressDialog.Log("WhisperX environment not found.");

                    progressDialog.Log("Installing WhisperX environment...");

                    await WhisperXInstallerService.InstallAsync();

                    progressDialog.Log("WhisperX environment installation completed.");
                }
            }

            if (m_TranscriptionSettings.Engine == TranscriptionEngine.Parakeet)
            {
                if (!await ParakeetInstallerService.IsPythonEnvironmentInstalledAsync())
                {
                    progressDialog.Log("Parakeet environment not found.");

                    progressDialog.Log("Installing Parakeet environment...");

                    await ParakeetInstallerService.InstallAsync();

                    progressDialog.Log("Parakeet environment installation completed.");
                }
            }

            TranscriptionOptions transcriptionOptions = new TranscriptionOptions
                {
                    WhisperModel = m_TranscriptionSettings.WhisperModel,

                    Language = m_TranscriptionSettings.Language
                };
            int parakeetChunkCount = 0;

            IProgress<string> progress =
               new SynchronousProgress<string>(
                    message =>
                    {
                        Console.WriteLine("AI transcription: " + message);

                        progressDialog.Log(message);

                        // ==========================================================
                        // PARAKEET PROGRESS
                        //
                        // 0 - 15%   Model / processor loading
                        // 15 - 20%  Audio preparation
                        // 20 - 85%  Chunk transcription
                        // 85 - 92%  Chunk merging
                        // 92 - 98%  Phrase building / cleanup
                        // 98 - 100% Completion
                        // ==========================================================

                        if (message.Contains(
                            "Loading Parakeet processor"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    5,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Parakeet processor loaded"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    10,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Loading Parakeet model"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    12,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Parakeet model loaded"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    15,
                                    null));

                            return;
                        }


                        // ----------------------------------------------------------
                        // Audio preparation
                        // ----------------------------------------------------------

                        if (message.Contains(
                            "Preparing long-audio Parakeet transcription"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    16,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Converting audio to 16 kHz mono PCM"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    18,
                                    null));

                            return;
                        }


                        // ----------------------------------------------------------
                        // Number of chunks
                        // ----------------------------------------------------------

                        if (message.StartsWith(
                            "Number of chunks:",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            string countText =
                                message.Substring(
                                    "Number of chunks:".Length)
                                .Trim();

                            if (int.TryParse(
                                countText,
                                out int count) &&
                                count > 0)
                            {
                                parakeetChunkCount =
                                    count;
                            }

                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    20,
                                    null));

                            return;
                        }


                        // ----------------------------------------------------------
                        // Individual chunk
                        //
                        // Example:
                        //
                        // Parakeet chunk 5/12
                        // ----------------------------------------------------------

                        if (message.StartsWith(
                            "Parakeet chunk ",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            int slashIndex =
                                message.IndexOf('/');

                            if (slashIndex > 0)
                            {
                                string chunkNumberText =
                                    message.Substring(
                                        "Parakeet chunk ".Length,
                                        slashIndex -
                                        "Parakeet chunk ".Length)
                                    .Trim();

                                string totalChunksText =
                                    message.Substring(
                                        slashIndex + 1)
                                    .Trim();

                                if (int.TryParse(
                                        chunkNumberText,
                                        out int chunkNumber) &&
                                    int.TryParse(
                                        totalChunksText,
                                        out int totalChunks) &&
                                    chunkNumber >= 1 &&
                                    totalChunks > 0)
                                {
                                    parakeetChunkCount =
                                        totalChunks;

                                    double fraction =
                                        (double)chunkNumber /
                                        totalChunks;

                                    int value =
                                        20 +
                                        (int)Math.Round(
                                            fraction * 65.0);

                                    value =
                                        Math.Max(
                                            20,
                                            Math.Min(
                                                85,
                                                value));

                                    progressDialog.UpdateProgressBar(
                                        null,
                                        new System.ComponentModel.ProgressChangedEventArgs(
                                            value,
                                            null));
                                }

                                return;
                            }
                        }


                        // ----------------------------------------------------------
                        // Chunk activity
                        //
                        // Keep the current chunk progress while these messages
                        // arrive.
                        // ----------------------------------------------------------

                        if (message.Contains(
                            "Preparing Parakeet input"))
                        {
                            return;
                        }


                        if (message.Contains(
                            "Transcribing chunk"))
                        {
                            return;
                        }


                        if (message.Contains(
                            "Chunk transcription completed"))
                        {
                            return;
                        }


                        if (message.Contains(
                            "Decoding chunk"))
                        {
                            return;
                        }


                        // ----------------------------------------------------------
                        // Merging
                        // ----------------------------------------------------------

                        if (message.Contains(
                            "Merging chunk transcripts"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    88,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Checking for residual duplicate words"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    90,
                                    null));

                            return;
                        }


                        // ----------------------------------------------------------
                        // Phrase construction
                        // ----------------------------------------------------------

                        if (message.Contains(
                            "Building phrase segments"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    92,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Reconstructed words"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    94,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Remaining phrases after cleanup"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    98,
                                    null));

                            return;
                        }


                        // ==========================================================
                        // WHISPERX PROGRESS
                        // ==========================================================

                        if (message.Contains(
                            "Loading WhisperX model"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    10,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Whisper model loaded"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    20,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Loading audio"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    30,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Audio loaded"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    40,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Transcribing audio"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    50,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Transcription completed"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    70,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Loading alignment model"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    80,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Alignment completed"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    85,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Saving JSON"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    90,
                                    null));

                            return;
                        }


                        if (message.Contains(
                            "Completed"))
                        {
                            progressDialog.UpdateProgressBar(
                                null,
                                new System.ComponentModel.ProgressChangedEventArgs(
                                    100,
                                    null));

                            return;
                        }
                    });

            List<TranscriptSegment> segments = await m_TranscriptionCoordinator.TranscribeAsync(audioFilePath,m_TranscriptionSettings.Engine,transcriptionOptions, m_CancellationTokenSource?.Token ?? CancellationToken.None,progress);

            if (m_CancellationTokenSource?.IsCancellationRequested == true)
            {
                throw new OperationCanceledException(m_CancellationTokenSource.Token);
            }

            progressDialog.Log("Transcription returned " + (segments == null ? 0 : segments.Count) + " segments.");

            if (segments == null || segments.Count == 0)
            {
                progressDialog.Log("No transcription segments were returned.");
                return;
            }

            string xhtmlPath = Path.Combine( Path.GetDirectoryName(audioFilePath)!, Path.GetFileNameWithoutExtension(audioFilePath) + ".xhtml");

            progressDialog.Log("Saving transcription to XHTML...");

            progressDialog.Log("XHTML file: " + Path.GetFileName(xhtmlPath));

            if (m_CancellationTokenSource?.IsCancellationRequested == true)
            {
                throw new OperationCanceledException(m_CancellationTokenSource.Token);
            }

            await XhtmlExportService.SaveAsync(segments, xhtmlPath);

            progressDialog.Log("XHTML transcription saved.");

            ImportExport.ImportTranscript import = new ImportExport.ImportTranscript(xhtmlPath, m_Presentation, m_ProjectView.ObiForm.Settings, audioFilePath);

            progressDialog.Log("Converting transcription into Obi phrases...");

            if (m_CancellationTokenSource?.IsCancellationRequested == true)
            {
                throw new OperationCanceledException(m_CancellationTokenSource.Token);
            }

            import.DoWork();

            progressDialog.Log("Obi phrase conversion completed.");

            List<PhraseNode> phrases = import.Phrases;

            progressDialog.Log("Phrases created: " + phrases.Count);

            if (phrases.Count == 0)
            {
                progressDialog.Log("No Obi phrases were created.");
                return;
            }

            CompositeCommand command = m_Presentation.CreateCompositeCommand(Localizer.Message("import_phrases"));

            int insertIndex = sectionNode.PhraseChildCount;

            for (int i = 0; i < phrases.Count; i++)
            {
                Commands.Node.AddNode addCmd = new Commands.Node.AddNode(m_ProjectView, phrases[i], sectionNode, insertIndex + i, false);

                command.ChildCommands.Insert(command.ChildCommands.Count, addCmd);
            }

            progressDialog.Log("Adding " +  phrases.Count + " phrases to the section...");

            m_Presentation.Do(command);

            progressDialog.Log("Audio phrase import completed.");
        }

        private class SynchronousProgress<T> : IProgress<T>
        {
            private readonly Action<T> m_Action;

            public SynchronousProgress(Action<T> action)
            {
                m_Action = action;
            }

            public void Report(T value)
            {
                m_Action(value);
            }
        }
    }
}
