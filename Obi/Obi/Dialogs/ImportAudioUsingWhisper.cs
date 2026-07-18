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

        private WhisperModel m_Model;
        public ImportAudioUsingWhisper(List<string> filePaths, bool importAudioFilesInEachSection, bool createSectionForEachPhrase)
        {
            InitializeComponent();


            cmbModel.DataSource = new List<WhisperModelItem>
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

            cmbModel.DisplayMember = "DisplayName";

            cmbModel.SelectedIndex = 0;
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
            txtLog.AppendText(message + Environment.NewLine);

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

                txtLog.Clear();

                progressBar.Style = ProgressBarStyle.Continuous;

                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = 0;



                m_Model = ((WhisperModelItem)cmbModel.SelectedItem).Model;


                Log("Transcribing audio......");

                Log($"Whisper model: {m_Model}");

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
                                progressBar.Value = 70;
                            }
                            else if (message.Contains(
                                "Loading alignment model"))
                            {
                                progressBar.Value = 80;
                            }
                            else if (message.Contains(
                                "Alignment completed"))
                            {
                                progressBar.Value = 85;
                            }
                            else if (message.Contains(
                                "Saving JSON"))
                            {
                                progressBar.Value = 90;
                            }
                            else if (message.Contains(
                                "Completed"))
                            {
                                progressBar.Value = 100;
                            }
                        });

                if (!await WhisperXInstallerService
                    .IsPythonEnvironmentInstalledAsync())
                {
                    Log("Installing WhisperX...");

                    await WhisperXInstallerService.InstallAsync(whisperProgress);
                }

                progressBar.Value = 0;

                WhisperXService whisper = new();
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
                    var batchResults =
                        await whisper.TranscribeBatchAsync(m_FilePaths, m_Model, _cts.Token, whisperProgress);

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
                    string mergedAudio = AudioMergeService.Merge(m_FilePaths);
                    if (mergedAudio != null)
                    {
                        m_FilePaths.Clear();
                        m_FilePaths.Add(mergedAudio);
                    }
                    //m_MergedAudioPath = mergedAudio;

                    {
                        var segments =
                            await whisper.TranscribeAsync(mergedAudio, m_Model, _cts.Token, whisperProgress);

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

                progressBar.Style =
                    ProgressBarStyle.Continuous;

                //lblStatus.Text =
                //    "Completed";  
                Log("Transcription Completed successfully");

                progressBar.Value = 100;
                Close();



            }
            catch (OperationCanceledException)
            {
                Log("Operation cancelled.");
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                MessageBox.Show(ex.ToString());
            }
            finally
            {


                m_btnCancel.Enabled = false;

                _cancellationTokenSource?.Dispose();

                _cancellationTokenSource = null;

                _cts?.Dispose();

                _cts = null;

                progressBar.Style =
                    ProgressBarStyle.Continuous;
            }
        }

        private void CancelTranscribing()
        {
            m_btnCancel.Enabled = false;

            progressBar.Value = 0;

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
    }
}
