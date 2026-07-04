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
        private Dictionary <string, string> m_XhtmlFilePathsDictionary;
        private bool m_ImportAudioFilesInEachSection;
        private bool m_CreateSectionForEachPhrase;


        private CancellationTokenSource? _cancellationTokenSource;

        private readonly XhtmlPhraseParser _parser;

        private readonly ChunkingService _chunkingService;

        private readonly OpenAiStructureService _openAiService;

        private readonly SemanticXhtmlBuilder _builder;

        private readonly StructurePostProcessor _postProcessor;
        public ImportAudioUsingWhisper(List<string> filePaths, bool importAudioFilesInEachSection, bool createSectionForEachPhrase)
        {
            InitializeComponent();
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
            StartImportProcess();
        }

        public Dictionary<string, string> XhtmlFilePathsDictionary
        {
            get { return m_XhtmlFilePathsDictionary; }
        }
        private async void StartImportProcess()
        {
            try
            {


                m_btnCancel.Enabled = true;

                txtLog.Clear();

                progressBar.Style = ProgressBarStyle.Continuous;

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
                        await whisper.TranscribeBatchAsync(
                            m_FilePaths,
                            _cts.Token,
                            whisperProgress);

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
                            await whisper.TranscribeAsync(
                                mergedAudio,
                                _cts.Token,
                                whisperProgress);

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
                txtLog.AppendText("Transcription Completed successfully" + Environment.NewLine);

                progressBar.Value = 100;
              //  Close();



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
            m_btnCancel.Enabled = false;
            progressBar.Value = 0;

            txtLog.AppendText("Cancelling..." + Environment.NewLine);
            _cts?.Cancel();
            _cancellationTokenSource?.Cancel();
            Close();
        }

    }
}
