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


        private CancellationTokenSource? _cancellationTokenSource;

        private readonly XhtmlPhraseParser _parser;

        private readonly ChunkingService _chunkingService;

        private readonly OpenAiStructureService _openAiService;

        private readonly SemanticXhtmlBuilder _builder;

        private readonly StructurePostProcessor _postProcessor;

        public CreateProjectFromAudio()
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
        }

        public string AudioPath { get => txtAudioPath.Text; }

        public string SemanticXhtmlPath { get => m_SemanticXhtmlPath; }
        private void m_btnBrowseAudio_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();

            dialog.Filter =
                "Audio Files|*.mp3;*.wav;*.m4a;*.aac;*.flac";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtAudioPath.Text = dialog.FileName;
            }
        }

        private async void m_btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                m_btnCancel.Enabled = true;

                txtLog.Clear();

                if (string.IsNullOrWhiteSpace(
                    txtAudioPath.Text))
                {
                    MessageBox.Show(
                        "Please select an audio file.");

                    return;
                }

                m_btnStart.Enabled = false;

                progressBar.Style =
                    ProgressBarStyle.Marquee;

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

                await WhisperXInstallerService
                    .EnsureModelsAsync(
                        whisperProgress);

                WhisperXService whisper =
                    new();


                // STEP 1:
                // Transcribe audio
                var segments =
                    await whisper.TranscribeAsync(
                        txtAudioPath.Text,
                        _cts.Token,
                        whisperProgress);

                // STEP 2:
                // Generate XHTML path
                string xhtmlPath =
                    Path.Combine(
                        Path.GetDirectoryName(
                            txtAudioPath.Text)!,

                        Path.GetFileNameWithoutExtension(
                            txtAudioPath.Text) +
                        ".xhtml");

                // STEP 3:
                // Export XHTML
                await XhtmlExportService.SaveAsync(
                    segments,
                    xhtmlPath);

                progressBar.Style =
                    ProgressBarStyle.Blocks;

                //lblStatus.Text =
                //    "Completed";
                txtLog.AppendText("Transcription Completed successfully" + Environment.NewLine);
                txtLog.AppendText("Now starting semantic analysis..." + Environment.NewLine);


                //MessageBox.Show(
                //    $"Transcription completed successfully.\n\n" +
                //    $"XHTML saved at:\n{xhtmlPath}");


                _cancellationTokenSource = new CancellationTokenSource();

                m_btnCancel.Enabled = true;


                m_SemanticXhtmlPath = string.Empty;

                progressBar.Value = 0;


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

                progressBar.Maximum =
                    chunks.Count;


                //--------------------------------------------------
                // PROCESS CHUNKS
                //--------------------------------------------------

                int chunkIndex = 1;

                foreach (var chunk in chunks)
                {
                    _cancellationTokenSource
                        .Token
                        .ThrowIfCancellationRequested();

                    txtLog.AppendText(
                        $"Processing chunk " +
                        $"{chunkIndex}/{chunks.Count}"
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

                    progressBar.Value += 1;

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
                //    "Semantic XHTML generated successfully.");
                txtLog.AppendText("Semantic XHTML generated successfully" + Environment.NewLine);

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
                    ProgressBarStyle.Blocks;
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            m_btnCancel.Enabled = false;

            txtLog.AppendText("Cancelling..." + Environment.NewLine);
            _cts?.Cancel();
            _cancellationTokenSource?.Cancel();

        }

        private void m_btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
