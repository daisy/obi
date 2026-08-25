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
        private TranscriptionEngine m_TranscriptionEngine =  TranscriptionEngine.Auto;
        public ImportAudioUsingWhisper(List<string> filePaths, bool importAudioFilesInEachSection, bool createSectionForEachPhrase)
        {
            InitializeComponent();

            _transcriptionCoordinator = new TranscriptionCoordinator(new WhisperXService(), new ParakeetService());

            m_TranscriptionEngineCb.DataSource =
    new List<TranscriptionEngineItem>
    {
        new()
        {
            Engine =
                TranscriptionEngine.Auto,

            DisplayName =
                "Auto"
        },

        new()
        {
            Engine =
                TranscriptionEngine.Parakeet,

            DisplayName =
                "Parakeet"
        },

        new()
        {
            Engine =
                TranscriptionEngine.Whisper,

            DisplayName =
                "Whisper"
        }
    };

            m_TranscriptionEngineCb.DisplayMember =
                "DisplayName";

            m_TranscriptionEngineCb.SelectedIndex = 0;


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

            m_BookLanguageCb.DataSource = WhisperLanguages.Languages;

            m_BookLanguageCb.DisplayMember = nameof(WhisperLanguageItem.DisplayName);

            m_BookLanguageCb.ValueMember = nameof(WhisperLanguageItem.LanguageCode);

            m_BookLanguageCb.SelectedIndex = 0;

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


                IProgress<string> whisperProgress =
                    new Progress<string>(
                        message =>
                        {
                            Log(message);

                            if (message.Contains(
                                "Loading WhisperX model"))
                            {
                                m_ProgressBar.Style =
                                    ProgressBarStyle.Continuous;

                                m_ProgressBar.Value = 10;
                            }
                            else if (message.Contains(
                                "Whisper model loaded"))
                            {
                                m_ProgressBar.Value = 20;
                            }
                            else if (message.Contains(
                                "Loading audio"))
                            {
                                m_ProgressBar.Value = 30;
                            }
                            else if (message.Contains(
                                "Audio loaded"))
                            {
                                m_ProgressBar.Value = 40;
                            }
                            else if (message.Contains(
                                "Transcribing audio"))
                            {
                                m_ProgressBar.Value = 50;
                            }
                            else if (message.Contains(
                                "Transcription completed"))
                            {
                                m_ProgressBar.Value = 70;
                            }
                            else if (message.Contains(
                                "Loading alignment model"))
                            {
                                m_ProgressBar.Value = 80;
                            }
                            else if (message.Contains(
                                "Alignment completed"))
                            {
                                m_ProgressBar.Value = 85;
                            }
                            else if (message.Contains(
                                "Saving JSON"))
                            {
                                m_ProgressBar.Value = 90;
                            }
                            else if (message.Contains(
                                "Completed"))
                            {
                                m_ProgressBar.Value = 100;
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
