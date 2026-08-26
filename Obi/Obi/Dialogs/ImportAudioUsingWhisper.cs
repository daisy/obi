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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Obi.Dialogs
{
    public partial class ImportAudioUsingWhisper : Form
    {
        private string m_SemanticXhtmlPath;
        private CancellationTokenSource? _cts;
        private string? m_MergedAudioPath;
        private List<string> m_FilePaths;
        private List<string> m_XhtmlPath;
        private Dictionary<string, string> m_XhtmlFilePathsDictionary;
        private bool m_ImportAudioFilesInEachSection;
        private bool m_CreateSectionForEachPhrase;
        private string? m_LogFilePath;
        private bool m_IsTranscribing = false;


        private CancellationTokenSource? _cancellationTokenSource;

        private readonly XhtmlPhraseParser _parser;

        private readonly ChunkingService _chunkingService;

        private readonly OpenAiStructureService _openAiService;

        private readonly SemanticXhtmlBuilder _builder;

        private readonly StructurePostProcessor _postProcessor;

        private readonly TranscriptionCoordinator _transcriptionCoordinator;

        private WhisperModel m_Model;
        private string m_BookLanguage = "auto";
        private TranscriptionEngine m_TranscriptionEngine = TranscriptionEngine.Auto;
        private bool m_UpdatingLanguageEngineLists;
        public ImportAudioUsingWhisper(List<string> filePaths, bool importAudioFilesInEachSection, bool createSectionForEachPhrase)
        {
            InitializeComponent();

            _transcriptionCoordinator = new TranscriptionCoordinator(new WhisperXService(), new ParakeetService());

            m_TranscriptionEngineCb.DisplayMember =
                nameof(TranscriptionEngineItem.DisplayName);

            m_TranscriptionEngineCb.ValueMember =
                nameof(TranscriptionEngineItem.Engine);

            m_TranscriptionEngineCb.SelectedIndexChanged +=
                m_TranscriptionEngineCb_SelectedIndexChanged;


            m_ModelCb.DataSource = new List<WhisperModelItem>
                {
                    new()
                    {
                        Model = WhisperModel.Large,
                        DisplayName = "Large (Best Accuracy)"
                    },
                    new()
                    {
                        Model = WhisperModel.Medium,
                        DisplayName = "Medium (Balanced)"
                    },
                    new()
                    {
                        Model = WhisperModel.Small,
                        DisplayName = "Small (Fastest)"
                    }
                };

            m_ModelCb.DisplayMember = "DisplayName";

            m_ModelCb.SelectedIndex = 1;

            m_BookLanguageCb.DisplayMember =
                nameof(WhisperLanguageItem.DisplayName);

            m_BookLanguageCb.ValueMember =
                nameof(WhisperLanguageItem.LanguageCode);

            m_BookLanguageCb.SelectedIndexChanged +=
                m_BookLanguageCb_SelectedIndexChanged;

            m_ImportAudioFilesInEachSection = importAudioFilesInEachSection;
            m_CreateSectionForEachPhrase = createSectionForEachPhrase;
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

            InitializeLanguageAndEngineSelections();

            UpdateWhisperModelAvailability();

            if (filePaths != null)
            {
                m_FilePaths = filePaths;
            }
        }

        public Dictionary<string, string> XhtmlFilePathsDictionary
        {
            get { return m_XhtmlFilePathsDictionary; }
        }
        private void Log(string message)
        {
            m_LogTxt.AppendText(message + Environment.NewLine);

            if (!string.IsNullOrEmpty(m_LogFilePath))
            {
                try
                {
                    File.AppendAllText(m_LogFilePath, message + Environment.NewLine);
                }
                catch
                {
                    // Never let logging break transcription.
                }
            }
        }

        private async void StartImportProcess()
        {
            try
            {
                //m_LogFilePath = Path.Combine(Path.GetDirectoryName(m_FilePaths[0])!, "WhisperX Log.txt");
                Directory.CreateDirectory(ObiPaths.LogsFolder);
                m_LogFilePath = Path.Combine(ObiPaths.LogsFolder, "WhisperX Log.txt");
                if (!string.IsNullOrEmpty(m_LogFilePath))
                {
                    File.WriteAllText(m_LogFilePath, string.Empty);
                }

                m_btnCancel.Enabled = true;
                m_ModelCb.Enabled = false;
                m_BookLanguageCb.Enabled = false;
                m_TranscriptionEngineCb.Enabled = false;

                m_LogTxt.Clear();

                m_ProgressBar.Style = ProgressBarStyle.Continuous;

                m_ProgressBar.Minimum = 0;
                m_ProgressBar.Maximum = 100;
                m_ProgressBar.Value = 0;



                m_Model = ((WhisperModelItem)m_ModelCb.SelectedItem).Model;

                m_BookLanguage = ((WhisperLanguageItem)m_BookLanguageCb.SelectedItem).LanguageCode;

                m_TranscriptionEngine = ((TranscriptionEngineItem)m_TranscriptionEngineCb.SelectedItem).Engine;
                TranscriptionOptions transcriptionOptions = new()
                                                            {
                                                                WhisperModel = m_Model,
                                                                Language = m_BookLanguage
                                                            };


                Log("Transcribing audio......");

                Log($"Whisper model: {m_Model}");
                Log($"Transcription engine: " + $"{m_TranscriptionEngine}");

                _cts =
                    new CancellationTokenSource();

                int parakeetChunkCount = 0;

                IProgress<string> whisperProgress =
                    new Progress<string>(
                        message =>
                        {
                            Log(message);


                            // ==========================================================
                            // PARAKEET PROGRESS
                            //
                            // Parakeet reports:
                            //
                            // Number of chunks: 12
                            // Parakeet chunk 1/12
                            // Parakeet chunk 2/12
                            // ...
                            // Parakeet chunk 12/12
                            //
                            // We map the Parakeet phases to approximately:
                            //
                            //  0 - 15%   Model loading
                            // 15 - 20%   Audio preparation
                            // 20 - 85%   Chunk transcription
                            // 85 - 92%   Chunk merging
                            // 92 - 98%   Phrase building / cleanup
                            // 98 - 100%  Completion
                            // ==========================================================


                            // ----------------------------------------------------------
                            // Model / processor loading
                            // ----------------------------------------------------------

                            if (message.Contains(
                                "Loading Parakeet processor"))
                            {
                                m_ProgressBar.Style =
                                    ProgressBarStyle.Continuous;

                                m_ProgressBar.Value = 5;

                                return;
                            }


                            if (message.Contains(
                                "Parakeet processor loaded"))
                            {
                                m_ProgressBar.Value = 10;

                                return;
                            }


                            if (message.Contains(
                                "Loading Parakeet model"))
                            {
                                m_ProgressBar.Value = 12;

                                return;
                            }


                            if (message.Contains(
                                "Parakeet model loaded"))
                            {
                                m_ProgressBar.Value = 15;

                                return;
                            }


                            // ----------------------------------------------------------
                            // Audio preparation
                            // ----------------------------------------------------------

                            if (message.Contains(
                                "Preparing long-audio Parakeet transcription"))
                            {
                                m_ProgressBar.Value = 16;

                                return;
                            }


                            if (message.Contains(
                                "Converting audio to 16 kHz mono PCM"))
                            {
                                m_ProgressBar.Value = 18;
                                    
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


                                m_ProgressBar.Value = 20;

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


                                        m_ProgressBar.Value =
                                            value;
                                    }


                                    return;
                                }
                            }


                            // ----------------------------------------------------------
                            // Chunk transcription activity
                            //
                            // Keep the current chunk progress while these messages
                            // arrive. Do not reset the progress.
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
                                m_ProgressBar.Value = 88;

                                return;
                            }


                            if (message.Contains(
                                "Checking for residual duplicate words"))
                            {
                                m_ProgressBar.Value = 90;

                                return;
                            }


                            // ----------------------------------------------------------
                            // Phrase construction
                            // ----------------------------------------------------------

                            if (message.Contains(
                                "Building phrase segments"))
                            {
                                m_ProgressBar.Value = 92;

                                return;
                            }


                            if (message.Contains(
                                "Reconstructed words"))
                            {
                                m_ProgressBar.Value = 94;

                                return;
                            }


                            if (message.Contains(
                                "Remaining phrases after cleanup"))
                            {
                                m_ProgressBar.Value = 98;

                                return;
                            }


                            // ==========================================================
                            // WHISPERX PROGRESS
                            //
                            // Keep the existing WhisperX behavior unchanged.
                            // ==========================================================

                            if (message.Contains(
                                "Loading WhisperX model"))
                            {
                                m_ProgressBar.Style =
                                    ProgressBarStyle.Continuous;

                                m_ProgressBar.Value = 10;

                                return;
                            }


                            if (message.Contains(
                                "Whisper model loaded"))
                            {
                                m_ProgressBar.Value = 20;

                                return;
                            }


                            if (message.Contains(
                                "Loading audio"))
                            {
                                m_ProgressBar.Value = 30;

                                return;
                            }


                            if (message.Contains(
                                "Audio loaded"))
                            {
                                m_ProgressBar.Value = 40;

                                return;
                            }


                            if (message.Contains(
                                "Transcribing audio"))
                            {
                                m_ProgressBar.Value = 50;

                                return;
                            }


                            if (message.Contains(
                                "Transcription completed"))
                            {
                                m_ProgressBar.Value = 70;

                                return;
                            }


                            if (message.Contains(
                                "Loading alignment model"))
                            {
                                m_ProgressBar.Value = 80;

                                return;
                            }


                            if (message.Contains(
                                "Alignment completed"))
                            {
                                m_ProgressBar.Value = 85;

                                return;
                            }


                            if (message.Contains(
                                "Saving JSON"))
                            {
                                m_ProgressBar.Value = 90;

                                return;
                            }


                            if (message.Contains(
                                "Completed"))
                            {
                                m_ProgressBar.Value = 100;

                                return;
                            }
                        });

                // ==========================================================
                // RESOLVE TRANSCRIPTION ENGINE
                // ==========================================================

                TranscriptionEngine effectiveEngine =
                    m_TranscriptionEngine;


                // ----------------------------------------------------------
                // Explicit engine
                // ----------------------------------------------------------

                if (effectiveEngine != TranscriptionEngine.Auto)
                {
                    Log(
                        $"Selected engine: {effectiveEngine}");
                }


                // ----------------------------------------------------------
                // Auto engine
                // ----------------------------------------------------------

                else
                {
                    // ------------------------------------------------------
                    // If language itself is Auto, WhisperX is temporarily
                    // required only to determine the language.
                    // ------------------------------------------------------

                    bool languageIsAuto =
                        string.IsNullOrWhiteSpace(
                            m_BookLanguage)
                        ||
                        m_BookLanguage
                            .Trim()
                            .Equals(
                                "auto",
                                StringComparison.OrdinalIgnoreCase);


                    if (languageIsAuto)
                    {
                        if (!await WhisperXInstallerService
                            .IsPythonEnvironmentInstalledAsync())
                        {
                            Log(
                                "Installing WhisperX for " +
                                "automatic language detection...");

                            await WhisperXInstallerService
                                .InstallAsync(
                                    whisperProgress);
                        }
                    }


                    effectiveEngine =
                        await ResolveAutomaticEngineAsync(
                            transcriptionOptions,
                            whisperProgress);


                    Log(
                        $"Auto selected engine: " +
                        $"{effectiveEngine}");
                }


                // ==========================================================
                // PREPARE SELECTED ENGINE
                // ==========================================================

                if (effectiveEngine ==
                    TranscriptionEngine.Whisper)
                {
                    if (!await WhisperXInstallerService
                        .IsPythonEnvironmentInstalledAsync())
                    {
                        Log("Installing WhisperX...");

                        await WhisperXInstallerService
                            .InstallAsync(
                                whisperProgress);
                    }
                }


                if (effectiveEngine ==
                    TranscriptionEngine.Parakeet)
                {
                    if (!await ParakeetInstallerService
                        .IsPythonEnvironmentInstalledAsync())
                    {
                        Log(
                            "Parakeet environment is not installed.");

                        Log(
                            "Installing Parakeet...");

                        await ParakeetInstallerService
                            .InstallAsync(
                                whisperProgress);
                    }
                }

                m_ProgressBar.Value = 0;

                //   WhisperXService whisper = new();


                m_XhtmlFilePathsDictionary = new Dictionary<string, string>();

                // STEP 1:
                // Transcribe audio

                //if (!m_ImportAudioFilesInEachSection && !m_CreateSectionForEachPhrase)
                //{
                //    string mergedAudio =
                //        AudioMergeService.Merge(m_FilePaths);
                //    if (mergedAudio != null)
                //    {
                //        m_FilePaths.Clear();
                //        m_FilePaths.Add(mergedAudio);
                //    }
                //}

                if (m_ImportAudioFilesInEachSection || m_CreateSectionForEachPhrase)
                {
                    var batchResults = await _transcriptionCoordinator.TranscribeBatchAsync(m_FilePaths, effectiveEngine, transcriptionOptions,_cts.Token,whisperProgress);

                    foreach (string filePath in m_FilePaths)
                    {
                        var segments =
                            batchResults[filePath];

                        string xhtmlPath =
                            Path.Combine(
                                Path.GetDirectoryName(filePath)!,
                                Path.GetFileNameWithoutExtension(filePath) +
                                ".xhtml");

                        await XhtmlExportService.SaveAsync(
                            segments,
                            xhtmlPath);

                        m_XhtmlFilePathsDictionary.Add(
                            filePath,
                            xhtmlPath);
                    }
                }

                else
                {
                    string mergedAudio = AudioMergeService.Merge(m_FilePaths, whisperProgress);
                    if (mergedAudio != null)
                    {
                        m_FilePaths.Clear();
                        m_FilePaths.Add(mergedAudio);
                    }
                    //m_MergedAudioPath = mergedAudio;

                    {
                        var segments = await _transcriptionCoordinator.TranscribeAsync(mergedAudio, effectiveEngine, transcriptionOptions,_cts.Token,whisperProgress);

                        // STEP 2:
                        // Generate XHTML path
                        string xhtmlPath =
                            Path.Combine(
                                Path.GetDirectoryName(
                                    mergedAudio)!,
                                  Path.GetFileNameWithoutExtension(mergedAudio) + ".xhtml");

                        // STEP 3:
                        // Export XHTML
                        await XhtmlExportService.SaveAsync(
                            segments,
                            xhtmlPath);

                        m_XhtmlFilePathsDictionary.Add(mergedAudio, xhtmlPath);
                    }
                }

                m_ProgressBar.Style =
                    ProgressBarStyle.Continuous;

                //lblStatus.Text =
                //    "Completed";  
                Log("Transcription Completed successfully");

                m_ProgressBar.Value = 100;
                Close();



            }
            catch (OperationCanceledException)
            {
                Log("Operation cancelled.");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                //MessageBox.Show(ex.ToString());

                MessageBox.Show(
                    ex.Message,
                    Localizer.Message("import_phrase_error_caption"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                Close();
            }
            finally
            {


                m_btnCancel.Enabled = false;

                _cancellationTokenSource?.Dispose();

                _cancellationTokenSource = null;

                _cts?.Dispose();

                _cts = null;

                m_ProgressBar.Style =
                    ProgressBarStyle.Continuous;
            }
        }

        private void CancelTranscribing()
        {
            m_btnCancel.Enabled = false;

            m_ProgressBar.Value = 0;

            Log("Cancelling...");
            _cts?.Cancel();
            _cancellationTokenSource?.Cancel();
            m_btnCancel.Enabled = true;

        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            if (m_IsTranscribing)
            {
                CancelTranscribing();
            }
            Close();
            m_IsTranscribing = false;
        }



        private void m_btnStart_Click(object sender, EventArgs e)
        {
            m_btnStart.Enabled = false;
            m_IsTranscribing = true;
            StartImportProcess();
        }


        // ==========================================================
        // LANGUAGE / ENGINE SELECTION
        // ==========================================================

        private void InitializeLanguageAndEngineSelections()
        {
            m_UpdatingLanguageEngineLists = true;

            try
            {
                // --------------------------------------------------
                // Initial engine list
                // --------------------------------------------------

                m_TranscriptionEngineCb.DataSource =
                    CreateEngineItems(
                        includeParakeet: true);

                m_TranscriptionEngineCb.SelectedValue =
                    TranscriptionEngine.Auto;


                // --------------------------------------------------
                // Initial language list
                // --------------------------------------------------

                m_BookLanguageCb.DataSource =
                    CreateLanguageItems(
                        includeAllLanguages: true);

                m_BookLanguageCb.SelectedValue =
                    "auto";


                // --------------------------------------------------
                // Initial internal values
                // --------------------------------------------------

                m_TranscriptionEngine =
                    TranscriptionEngine.Auto;

                m_BookLanguage =
                    "auto";
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // CREATE ENGINE LIST
        // ==========================================================

        private static List<TranscriptionEngineItem>
            CreateEngineItems(
                bool includeParakeet)
        {
            var items =
                new List<TranscriptionEngineItem>
                {
            new()
            {
                Engine =
                    TranscriptionEngine.Auto,

                DisplayName =
                    "Auto"
            }
                };


            if (includeParakeet)
            {
                items.Add(
                    new TranscriptionEngineItem
                    {
                        Engine =
                            TranscriptionEngine.Parakeet,

                        DisplayName =
                            "Parakeet"
                    });
            }


            items.Add(
                new TranscriptionEngineItem
                {
                    Engine =
                        TranscriptionEngine.Whisper,

                    DisplayName =
                        "Whisper"
                });


            return items;
        }


        // ==========================================================
        // CREATE LANGUAGE LIST
        // ==========================================================

        private static List<WhisperLanguageItem>
            CreateLanguageItems(
                bool includeAllLanguages)
        {
            if (includeAllLanguages)
            {
                return WhisperLanguages.Languages
                    .ToList();
            }


            return WhisperLanguages.Languages
                .Where(
                    language =>
                        language.LanguageCode
                            .Equals(
                                "auto",
                                StringComparison.OrdinalIgnoreCase)
                        ||
                        ParakeetLanguages.SupportedCodes.Contains(
                            language.LanguageCode
                                .Trim()
                                .ToLowerInvariant()))
                .ToList();
        }


        // ==========================================================
        // BOOK LANGUAGE CHANGED
        // ==========================================================

        private void m_BookLanguageCb_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (m_UpdatingLanguageEngineLists)
                return;


            if (m_BookLanguageCb.SelectedItem
                is not WhisperLanguageItem selectedLanguage)
            {
                return;
            }


            string language =
                string.IsNullOrWhiteSpace(
                    selectedLanguage.LanguageCode)
                    ? "auto"
                    : selectedLanguage.LanguageCode
                        .Trim()
                        .ToLowerInvariant();


            m_BookLanguage =
                language;


            // ------------------------------------------------------
            // If a specific language is selected, determine whether
            // Parakeet supports it.
            // ------------------------------------------------------

            bool parakeetSupported =
                language == "auto"
                ||
                ParakeetLanguages.SupportedCodes.Contains(
                    language);


            // ------------------------------------------------------
            // Hindi / unsupported language:
            //
            // Parakeet must disappear.
            //
            // If Parakeet was selected, switch to Whisper.
            // ------------------------------------------------------

            if (!parakeetSupported)
            {
                if (m_TranscriptionEngine ==
                    TranscriptionEngine.Parakeet)
                {
                    SetEngineSelection(
                        TranscriptionEngine.Whisper);
                }


                RefreshEngineList(
                    includeParakeet: false);

                return;
            }


            // ------------------------------------------------------
            // Language is supported by Parakeet or Auto Detect.
            //
            // Restore all engines.
            // ------------------------------------------------------

            RefreshEngineList(
                includeParakeet: true);
        }


        // ==========================================================
        // TRANSCRIPTION ENGINE CHANGED
        // ==========================================================

        private void m_TranscriptionEngineCb_SelectedIndexChanged(
            object? sender,
            EventArgs e)
        {
            if (m_UpdatingLanguageEngineLists)
                return;


            if (m_TranscriptionEngineCb.SelectedItem
                is not TranscriptionEngineItem selectedEngine)
            {
                return;
            }


            TranscriptionEngine engine =
                selectedEngine.Engine;


            m_TranscriptionEngine =
                engine;

            UpdateWhisperModelAvailability();

            // ------------------------------------------------------
            // Parakeet selected.
            //
            // Only Parakeet-supported languages + Auto Detect
            // should be available.
            // ------------------------------------------------------

            if (engine ==
                TranscriptionEngine.Parakeet)
            {
                string language =
                    string.IsNullOrWhiteSpace(
                        m_BookLanguage)
                        ? "auto"
                        : m_BookLanguage
                            .Trim()
                            .ToLowerInvariant();


                bool supported =
                    language == "auto"
                    ||
                    ParakeetLanguages.SupportedCodes.Contains(
                        language);


                // --------------------------------------------------
                // If the current language is not supported,
                // switch language to Auto Detect.
                // --------------------------------------------------

                if (!supported)
                {
                    SetLanguageSelection(
                        "auto");
                }


                RefreshLanguageList(
                    parakeetOnly: true);

                return;
            }


            // ------------------------------------------------------
            // Auto or Whisper:
            //
            // All Whisper-supported languages are available.
            // ------------------------------------------------------

            RefreshLanguageList(
                parakeetOnly: false);
        }

        // ==========================================================
        // UPDATE WHISPER MODEL AVAILABILITY
        // ==========================================================

        private void UpdateWhisperModelAvailability()
        {
            if (m_TranscriptionEngineCb.SelectedItem
                is not TranscriptionEngineItem selectedEngine)
            {
                return;
            }


            m_ModelCb.Enabled =
                selectedEngine.Engine !=
                TranscriptionEngine.Parakeet;
        }


        // ==========================================================
        // REFRESH ENGINE LIST
        // ==========================================================

        private void RefreshEngineList(
            bool includeParakeet)
        {
            TranscriptionEngine selectedEngine =
                m_TranscriptionEngine;


            m_UpdatingLanguageEngineLists = true;

            try
            {
                List<TranscriptionEngineItem> items =
                    CreateEngineItems(
                        includeParakeet);


                m_TranscriptionEngineCb.DataSource =
                    items;


                // --------------------------------------------------
                // Keep current selection if still available.
                // Otherwise select Whisper.
                // --------------------------------------------------

                bool selectionExists =
                    items.Any(
                        item =>
                            item.Engine ==
                            selectedEngine);


                if (selectionExists)
                {
                    m_TranscriptionEngineCb.SelectedValue =
                        selectedEngine;
                }
                else
                {
                    m_TranscriptionEngine =
                        TranscriptionEngine.Whisper;

                    m_TranscriptionEngineCb.SelectedValue =
                        TranscriptionEngine.Whisper;
                }
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // REFRESH LANGUAGE LIST
        // ==========================================================

        private void RefreshLanguageList(
            bool parakeetOnly)
        {
            string selectedLanguage =
                string.IsNullOrWhiteSpace(
                    m_BookLanguage)
                    ? "auto"
                    : m_BookLanguage
                        .Trim()
                        .ToLowerInvariant();


            m_UpdatingLanguageEngineLists = true;

            try
            {
                List<WhisperLanguageItem> languages =
                    CreateLanguageItems(
                        includeAllLanguages:
                            !parakeetOnly);


                m_BookLanguageCb.DataSource =
                    languages;


                bool selectionExists =
                    languages.Any(
                        language =>
                            language.LanguageCode
                                .Equals(
                                    selectedLanguage,
                                    StringComparison.OrdinalIgnoreCase));


                if (selectionExists)
                {
                    m_BookLanguageCb.SelectedValue =
                        selectedLanguage;
                }
                else
                {
                    m_BookLanguage =
                        "auto";

                    m_BookLanguageCb.SelectedValue =
                        "auto";
                }
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // SET ENGINE SELECTION
        // ==========================================================

        private void SetEngineSelection(
            TranscriptionEngine engine)
        {
            m_UpdatingLanguageEngineLists = true;

            try
            {
                m_TranscriptionEngine =
                    engine;

                m_TranscriptionEngineCb.SelectedValue =
                    engine;
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }


        // ==========================================================
        // SET LANGUAGE SELECTION
        // ==========================================================

        private void SetLanguageSelection(
            string language)
        {
            language =
                string.IsNullOrWhiteSpace(language)
                    ? "auto"
                    : language
                        .Trim()
                        .ToLowerInvariant();


            m_UpdatingLanguageEngineLists = true;

            try
            {
                m_BookLanguage =
                    language;

                m_BookLanguageCb.SelectedValue =
                    language;
            }
            finally
            {
                m_UpdatingLanguageEngineLists = false;
            }
        }

        private async Task<TranscriptionEngine>
    ResolveAutomaticEngineAsync(
        TranscriptionOptions transcriptionOptions,
        IProgress<string> progress)
        {
            string language =
                string.IsNullOrWhiteSpace(
                    m_BookLanguage)
                    ? "auto"
                    : m_BookLanguage
                        .Trim()
                        .ToLowerInvariant();


            // ----------------------------------------------------------
            // Explicit language
            // ----------------------------------------------------------

            if (language != "auto")
            {
                if (ParakeetLanguages.SupportedCodes.Contains(
                    language))
                {
                    progress.Report(
                        $"Book language: {language}");

                    progress.Report(
                        "Auto selected Parakeet.");

                    return TranscriptionEngine.Parakeet;
                }


                progress.Report(
                    $"Book language '{language}' " +
                    "is not supported by Parakeet.");

                progress.Report(
                    "Auto selected WhisperX.");

                return TranscriptionEngine.Whisper;
            }


            // ----------------------------------------------------------
            // Auto language + Auto engine
            //
            // We need to know the language before deciding whether
            // Parakeet is appropriate.
            // ----------------------------------------------------------

            progress.Report(
                "Book language: Auto Detect");

            progress.Report(
                "Detecting book language with WhisperX...");


            WhisperXService whisperXService =
                new WhisperXService();


            string detectedLanguage =
                await whisperXService.DetectLanguageAsync(
                    m_FilePaths[0],
                    m_Model,
                    _cts!.Token,
                    progress);


            detectedLanguage =
                detectedLanguage
                    .Trim()
                    .ToLowerInvariant();


            // Store the detected language so that the actual
            // transcription receives the correct language.
            m_BookLanguage =
                detectedLanguage;

            transcriptionOptions.Language =
                detectedLanguage;


            progress.Report(
                $"Book language detected: " +
                $"{detectedLanguage}");


            if (ParakeetLanguages.SupportedCodes.Contains(
                detectedLanguage))
            {
                progress.Report(
                    "Detected language is supported by Parakeet.");

                progress.Report(
                    "Auto selected Parakeet.");

                return TranscriptionEngine.Parakeet;
            }


            progress.Report(
                "Detected language is not supported by Parakeet.");

            progress.Report(
                "Auto selected WhisperX.");

            return TranscriptionEngine.Whisper;
        }
    }
}
