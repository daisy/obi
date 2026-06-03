using Obi.Builders;
using Obi.Models;
using Obi.Parsers;
using Obi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class CreateProjectFromAudio : Form
    {
        private string m_SemanticXhtmlPath;
        private CancellationTokenSource? _cts;
        private string? m_MergedAudioPath;
        private List<string> m_filePaths;


        private CancellationTokenSource? _cancellationTokenSource;

        private readonly XhtmlPhraseParser _parser;

        private readonly ChunkingService _chunkingService;

        private readonly OpenAiStructureService _openAiService;

        private readonly SemanticXhtmlBuilder _builder;

        private readonly StructurePostProcessor _postProcessor;

        public CreateProjectFromAudio(string[] filesPathArray)
        {
            InitializeComponent();

            _parser = new XhtmlPhraseParser();

            _chunkingService = new ChunkingService();

            _openAiService = new OpenAiStructureService(
                    new HttpClient
                    {
                        Timeout =
                            TimeSpan.FromMinutes(10)
                    });

            _builder = new SemanticXhtmlBuilder();

            _postProcessor = new StructurePostProcessor();
            if (filesPathArray != null)
            {
                m_filePaths = new List<string>(filesPathArray);
            }
            else
            {
                m_filePaths = new List<string>();
            }
            m_filePaths.Sort();
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;

            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstAudioFiles.Items.Add(System.IO.Path.GetFileName(str));
                }
            }

        }

        public string AudioPath { get => m_MergedAudioPath; }

        public string SemanticXhtmlPath { get => m_SemanticXhtmlPath; }


        private void m_btnAddAudio_Click(object sender, EventArgs e)
        {

            OpenFileDialog select_File = new OpenFileDialog();
            select_File.Filter = Localizer.Message("audio_file_filter");
            int index = m_filePaths.Count;
            select_File.RestoreDirectory = true;
            select_File.Multiselect = true;
            if (select_File.ShowDialog() == DialogResult.OK)
            {
                string[] fileNames = select_File.FileNames;
                foreach (string fileName in fileNames)
                {
                    string nameOfFile = System.IO.Path.GetFileName(fileName);
                    if (nameOfFile != null) lstAudioFiles.Items.Add(nameOfFile);
                    m_filePaths.Add(fileName);
                }

                lstAudioFiles.SelectedIndex = -1;

            }

        }

        private async void m_btnStart_Click(object sender, EventArgs e)
        {
            try
            {

                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
                m_btnRemove.Enabled = false;
                m_btnAdd.Enabled = false;

                m_btnCancel.Enabled = true;

                txtLog.Clear();

                if (lstAudioFiles.Items.Count == 0)
                {
                    MessageBox.Show(
                        "Please select one or more audio files.");

                    return;
                }
                m_btnStart.Enabled = false;

                progressBar.Style =  ProgressBarStyle.Continuous;

                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;

                //lblStatus.Text =
                //    "Transcribing audio...";

                txtLog.AppendText("Transcribing audio......" + Environment.NewLine);

                _cts =
                    new CancellationTokenSource();


                IProgress<string> whisperProgress =
                    new Progress<string>(
                        message =>
                        {
                            txtLog.AppendText(
                                message +
                                Environment.NewLine);

                            if (message.Contains(
                                "Loading WhisperX model"))
                            {
                                progressBar.Style =
                                    ProgressBarStyle.Continuous;

                                progressBar.Value = 10;
                            }
                            else if (message.Contains(
                                "Whisper model loaded"))
                            {
                                progressBar.Value = 20;
                            }
                            else if (message.Contains(
                                "Loading audio"))
                            {
                                progressBar.Value = 30;
                            }
                            else if (message.Contains(
                                "Audio loaded"))
                            {
                                progressBar.Value = 40;
                            }
                            else if (message.Contains(
                                "Transcribing audio"))
                            {
                                progressBar.Value = 50;
                            }
                            else if (message.Contains(
                                "Transcription completed"))
                            {
                                progressBar.Value = 55;
                            }
                            else if (message.Contains(
                                "Loading alignment model"))
                            {
                                progressBar.Value = 60;
                            }
                            else if (message.Contains(
                                "Alignment completed"))
                            {
                                progressBar.Value = 65;
                            }
                            else if (message.Contains(
                                "Saving JSON"))
                            {
                                progressBar.Value = 68;
                            }
                            else if (message.Contains(
                                "Completed"))
                            {
                                progressBar.Value = 70;
                            }
                        });

                if (!await WhisperXInstallerService
                    .IsPythonEnvironmentInstalledAsync())
                {
                    txtLog.AppendText(
                        "Installing WhisperX..." +
                        Environment.NewLine);

                    await WhisperXInstallerService
                        .InstallAsync(
                            whisperProgress);
                }

                progressBar.Value = 0;

                WhisperXService whisper =
                    new();


                // STEP 1:
                // Transcribe audio


                string mergedAudio =
                    AudioMergeService.Merge(m_filePaths);

                m_MergedAudioPath = mergedAudio;
                var segments =
                    await whisper.TranscribeAsync(
                        mergedAudio,
                        _cts.Token,
                        whisperProgress);

                // STEP 2:
                // Generate XHTML path
                string xhtmlPath =
                    Path.Combine(
                        Path.GetDirectoryName(
                            m_filePaths[0])!,
                        "CombinedTranscription.xhtml");

                // STEP 3:
                // Export XHTML
                await XhtmlExportService.SaveAsync(
                    segments,
                    xhtmlPath);

                progressBar.Style =
                    ProgressBarStyle.Continuous;

                //lblStatus.Text =
                //    "Completed";
                txtLog.AppendText("Transcription Completed successfully" + Environment.NewLine);
                txtLog.AppendText("Now starting semantic analysis..." + Environment.NewLine);

                progressBar.Value = 70;

                //MessageBox.Show(
                //    $"Transcription completed successfully.\n\n" +
                //    $"XHTML saved at:\n{xhtmlPath}");


                _cancellationTokenSource = new CancellationTokenSource();

                m_btnCancel.Enabled = true;


                m_SemanticXhtmlPath = string.Empty;


                //--------------------------------------------------
                // VALIDATION
                //--------------------------------------------------

                if (!File.Exists(xhtmlPath))
                {
                    MessageBox.Show("Please select a valid audio file.");

                    return;
                }

                //--------------------------------------------------
                // LOAD XHTML
                //--------------------------------------------------

                txtLog.AppendText("Loading XHTML..." + Environment.NewLine);

                var phrases =
                    _parser.Parse(
                        xhtmlPath);


                var semanticBuilder = new SemanticPhraseBuilder();

                phrases =
                    semanticBuilder.Build(
                        phrases);

                var refinementService = new PhraseRefinementService();

                phrases = refinementService.Refine(phrases);

                txtLog.AppendText(
                    $"Loaded {phrases.Count} phrases"
                    + Environment.NewLine);



                //--------------------------------------------------
                // CHUNKING
                //--------------------------------------------------

                var chunks =
                    _chunkingService.Chunk(
                        phrases);

                txtLog.AppendText(
                    $"Created {chunks.Count} chunks"
                    + Environment.NewLine);



                //--------------------------------------------------
                // STRUCTURE MAP
                //--------------------------------------------------

                var structureMap =
                    new Dictionary<
                        string,
                        StructureItem>();



                //--------------------------------------------------
                // PROCESS CHUNKS
                //--------------------------------------------------

                int chunkIndex = 1;

                foreach (var chunk in chunks)
                {
                    _cancellationTokenSource
                        .Token
                        .ThrowIfCancellationRequested();

                    //txtLog.AppendText(
                    //    $"Processing chunk " +
                    //    $"{chunkIndex}/{chunks.Count}"
                    //    + Environment.NewLine);

                    int structuringPercent =
                        (int)Math.Round(
                            (chunkIndex * 100.0)
                            / chunks.Count);

                    txtLog.AppendText(
                        $"Structuring: " +
                        $"{structuringPercent}% " +
                        $"({chunkIndex}/{chunks.Count} chunks)"
                        + Environment.NewLine);

                    var result =
                            await _openAiService
                                .DetectStructureAsync(
                                    chunk,
                                    _cancellationTokenSource.Token);

                    //--------------------------------------------------
                    // DEDUPLICATE
                    //--------------------------------------------------

                    foreach (var item
                        in result.Structure)
                    {
                        structureMap[
                            item.PhraseId] = item;
                    }

                    structuringPercent =
                        (int)Math.Round(
                            (chunkIndex * 100.0)
                            / chunks.Count);

                    progressBar.Value =
                        70
                        + (int)Math.Round(
                            structuringPercent * 0.30);

                    chunkIndex++;
                }


                //--------------------------------------------------
                // FINAL STRUCTURE
                //--------------------------------------------------

                var allStructure =
                    structureMap.Values.ToList();

                allStructure = _postProcessor.Process(phrases, allStructure);

                txtLog.AppendText(
                    $"Final structure items: " +
                    $"{allStructure.Count}"
                    + Environment.NewLine);


                //--------------------------------------------------
                // BUILD SEMANTIC XHTML
                //--------------------------------------------------

                txtLog.AppendText(
                    "Building semantic XHTML..."
                    + Environment.NewLine);

                string xhtml =
                    _builder.Build(
                        phrases,
                        allStructure);


                //--------------------------------------------------
                // OUTPUT FILE
                //--------------------------------------------------

                string output =
                    Path.Combine(
                        Path.GetDirectoryName(
                            xhtmlPath)!,

                        Path.GetFileNameWithoutExtension(
                            xhtmlPath)
                        + "_semantic.xhtml");


                //--------------------------------------------------
                // SAVE
                //--------------------------------------------------

                await File.WriteAllTextAsync(output, xhtml);


                //--------------------------------------------------
                // COMPLETE
                //--------------------------------------------------

                txtLog.AppendText(
                    "Completed"
                    + Environment.NewLine);

                txtLog.AppendText(
                    output
                    + Environment.NewLine);

                m_SemanticXhtmlPath = output;


                //MessageBox.Show(
                //    $"Semantic XHTML generated successfully.\n\n" +
                //$"XHTML saved at:\n{m_SemanticXhtmlPath}");

                txtLog.AppendText("Semantic XHTML generated successfully" + Environment.NewLine);

                progressBar.Value = 100;

                txtLog.AppendText("Now Project import will start..." + Environment.NewLine);
                this.Close();

            }
            catch (OperationCanceledException)
            {
                txtLog.AppendText("Operation cancelled." + Environment.NewLine);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString());
            }
            finally
            {
                m_btnStart.Enabled = true;


                m_btnCancel.Enabled = false;

                _cancellationTokenSource?.Dispose();

                _cancellationTokenSource = null;

                _cts?.Dispose();

                _cts = null;

                progressBar.Style =
                    ProgressBarStyle.Continuous;
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            m_btnAdd.Enabled = true;
            m_btnCancel.Enabled = false;
            progressBar.Value = 0;

            txtLog.AppendText("Cancelling..." + Environment.NewLine);
            _cts?.Cancel();
            _cancellationTokenSource?.Cancel();

        }

        private void m_btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void m_btnMoveUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAudioFiles.Items.Count != 0)
                {

                    if (lstAudioFiles.SelectedIndex != 0 && lstAudioFiles.SelectedIndex != -1)
                    {
                        int tempIndexStore = lstAudioFiles.SelectedIndex;
                        object item = lstAudioFiles.SelectedItem;

                        int index = lstAudioFiles.SelectedIndex;
                        // List<string> filePaths = new List<string>(mfilePaths);
                        //object itemInList = filesPath[index];

                        object itemInList = m_filePaths[index];

                        lstAudioFiles.Items.RemoveAt(index);
                        m_filePaths.RemoveAt(index);


                        lstAudioFiles.Items.Insert(index - 1, item);
                        m_filePaths.Insert(index - 1, itemInList.ToString());
                        if ((tempIndexStore - 1) != -1)
                            lstAudioFiles.SelectedIndex = tempIndexStore - 1;

                    }

                }

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }

        }

        private void m_btnMoveDown_Click(object sender, EventArgs e)
        {
            try
            {
                int index = lstAudioFiles.SelectedIndex;

                if (lstAudioFiles.Items.Count != 0)
                {
                    if (lstAudioFiles.SelectedIndex != lstAudioFiles.Items.Count - 1 && lstAudioFiles.SelectedIndex != -1)
                    {
                        int tempIndexStore = lstAudioFiles.SelectedIndex;
                        object item = lstAudioFiles.SelectedItem;

                        object itemInList = m_filePaths[index];
                        lstAudioFiles.Items.RemoveAt(index);

                        m_filePaths.RemoveAt(index);

                        lstAudioFiles.Items.Insert(index + 1, item);

                        m_filePaths.Insert(index + 1, itemInList.ToString());
                        //   if ((tempIndexStore+1) != lstManualArrange.Items.Count - 1)
                        lstAudioFiles.SelectedIndex = tempIndexStore + 1;
                    }
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }



        }

        private void m_btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstAudioFiles.Items.Count != 0)
                {
                    if (lstAudioFiles.SelectedIndex >= 0)
                    {
                        object item = lstAudioFiles.SelectedItem;
                        int tempIndex = lstAudioFiles.SelectedIndex;
                        lstAudioFiles.Items.Remove(item);
                        for (int i = 0; i < m_filePaths.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(m_filePaths[i]) == (string)item)
                            {
                                m_filePaths.RemoveAt(i);
                                break;
                            }
                        }
                        if (lstAudioFiles.Items.Count != 0)
                        {
                            if (tempIndex > lstAudioFiles.Items.Count - 1)
                            {
                                lstAudioFiles.SelectedIndex = lstAudioFiles.Items.Count - 1;
                            }
                            else
                            {
                                lstAudioFiles.SelectedIndex = tempIndex;
                            }
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            if (lstAudioFiles.Items.Count < 1)
            {
                m_btnRemove.Enabled = false;
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
            }



            //if (lstAudioFiles.SelectedIndex >= 0)
            //{
            //    lstAudioFiles.Items.RemoveAt(lstAudioFiles.SelectedIndex);
            //}
        }

        private void lstAudioFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstAudioFiles.SelectedIndex == 0)
            {
                m_btnRemove.Enabled = true;
                if (lstAudioFiles.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = true;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstAudioFiles.SelectedIndex == lstAudioFiles.Items.Count - 1)
            {
                m_btnRemove.Enabled = true;
                if (lstAudioFiles.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = false;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstAudioFiles.SelectedIndex > 0)
            {
                m_btnRemove.Enabled = true;
                if (lstAudioFiles.Items.Count != 1)
                {
                    m_btnMoveUp.Enabled = true;
                    m_btnMoveDown.Enabled = true;

                }
                else
                {
                    m_btnMoveUp.Enabled = false;
                    m_btnMoveDown.Enabled = false;

                }
            }
            else if (lstAudioFiles.SelectedIndex < 0)
            {
                m_btnMoveUp.Enabled = false;
                m_btnMoveDown.Enabled = false;
                m_btnRemove.Enabled = false;
            }
        }

        private void m_btnAscendingOrder_Click(object sender, EventArgs e)
        {
            List<string> filenames = new List<string>(); // Contains file names
            Dictionary<String, String> fileNamesDictionary = new Dictionary<string, string>(); //used for storing filename as key and path as value
            List<string> tempDuplicateFileName = new List<string>(); //contains duplicate file names with path
            m_filePaths.Sort();
            foreach (string str in m_filePaths)
            {
                filenames.Add(System.IO.Path.GetFileName(str));
                if (!fileNamesDictionary.ContainsKey(System.IO.Path.GetFileName(str)))
                {
                    fileNamesDictionary.Add(System.IO.Path.GetFileName(str), str);
                }
                else
                {
                    if (!tempDuplicateFileName.Contains(fileNamesDictionary[System.IO.Path.GetFileName(str)]))
                    {
                        tempDuplicateFileName.Add(fileNamesDictionary[System.IO.Path.GetFileName(str)]);
                    }
                    tempDuplicateFileName.Add(str);
                }
            }
            filenames.Sort();
            tempDuplicateFileName.Sort();
            int tempLength = m_filePaths.Count;
            List<string> tempList = new List<string>();
            foreach (string str in filenames)
            {
                if (fileNamesDictionary.ContainsKey(str))
                {
                    tempList.Add(fileNamesDictionary[str]);
                    if (tempDuplicateFileName.Contains(fileNamesDictionary[str]))
                    {
                        int tempIndex = tempDuplicateFileName.IndexOf(fileNamesDictionary[str]);
                        tempDuplicateFileName.RemoveAt(tempIndex);
                        for (int i = tempIndex; i < tempDuplicateFileName.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(tempDuplicateFileName[i]) == str)
                            {
                                fileNamesDictionary[str] = tempDuplicateFileName[i];
                                break;
                            }
                        }

                    }
                }
            }
            lstAudioFiles.Items.Clear();

            if (tempList.Count != 0)
            {
                m_filePaths.Clear();
                m_filePaths = tempList;
            }
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstAudioFiles.Items.Add(System.IO.Path.GetFileName(str));
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
        }

        private void m_btnDesendingOrder_Click(object sender, EventArgs e)
        {
            List<string> filenames = new List<string>(); // Contains file names
            Dictionary<String, String> fileNamesDictionary = new Dictionary<string, string>(); //used for storing filename as key and path as value
            List<string> tempDuplicateFileName = new List<string>(); //contains duplicate file names with path
            m_filePaths.Sort();
            foreach (string str in m_filePaths)
            {
                filenames.Add(System.IO.Path.GetFileName(str));
                if (!fileNamesDictionary.ContainsKey(System.IO.Path.GetFileName(str)))
                {
                    fileNamesDictionary.Add(System.IO.Path.GetFileName(str), str);
                }
                else
                {
                    if (!tempDuplicateFileName.Contains(fileNamesDictionary[System.IO.Path.GetFileName(str)]))
                    {
                        tempDuplicateFileName.Add(fileNamesDictionary[System.IO.Path.GetFileName(str)]);
                    }
                    tempDuplicateFileName.Add(str);
                }
            }
            filenames.Sort();
            tempDuplicateFileName.Sort();

            List<string> tempList = new List<string>();
            foreach (string str in filenames)
            {
                if (fileNamesDictionary.ContainsKey(str))
                {
                    tempList.Add(fileNamesDictionary[str]);
                    if (tempDuplicateFileName.Contains(fileNamesDictionary[str]))
                    {
                        int tempIndex = tempDuplicateFileName.IndexOf(fileNamesDictionary[str]);
                        tempDuplicateFileName.RemoveAt(tempIndex);
                        for (int i = tempIndex; i < tempDuplicateFileName.Count; i++)
                        {
                            if (System.IO.Path.GetFileName(tempDuplicateFileName[i]) == str)
                            {
                                fileNamesDictionary[str] = tempDuplicateFileName[i];
                                break;
                            }
                        }

                    }
                }
            }
            if (tempList.Count != 0)
            {
                m_filePaths.Clear();
                m_filePaths = tempList;
            }
            int totLength = m_filePaths.Count;

            List<string> tempDescending = new List<string>();
            for (int i = totLength - 1; i >= 0; i--)
            {
                tempDescending.Add(m_filePaths[i]);
            }

            m_filePaths = tempDescending;

            lstAudioFiles.Items.Clear();
            foreach (string str in m_filePaths)
            {
                if (str != null)
                {
                    lstAudioFiles.Items.Add(System.IO.Path.GetFileName(str));
                }
            }
            m_btnMoveUp.Enabled = false;
            m_btnMoveDown.Enabled = false;
            m_btnRemove.Enabled = false;
        }
    }
}
