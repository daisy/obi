using SemanticStructureDetector.WinForms.Builders;
using SemanticStructureDetector.WinForms.Models;
using SemanticStructureDetector.WinForms.Parsers;
using SemanticStructureDetector.WinForms.Services;
using System.Threading;

namespace SemanticStructureDetector.WinForms;

public partial class MainForm : Form
{
    private readonly XhtmlPhraseParser _parser;

    private readonly ChunkingService _chunkingService;

    private readonly OpenAiStructureService _openAiService;

    private readonly SemanticXhtmlBuilder _builder;

    private readonly StructurePostProcessor _postProcessor;
    private CancellationTokenSource? _cancellationTokenSource;

    public MainForm()
    {
        InitializeComponent();

        _parser =
            new XhtmlPhraseParser();

        _chunkingService =
            new ChunkingService();

        _openAiService =
            new OpenAiStructureService(
                new HttpClient
                {
                    Timeout =
                        TimeSpan.FromMinutes(10)
                });

        _builder =
            new SemanticXhtmlBuilder();

        _postProcessor = new StructurePostProcessor();
    }

    //--------------------------------------------------
    // BROWSE XHTML
    //--------------------------------------------------

    private void btnBrowse_Click(
        object sender,
        EventArgs e)
    {
        using var dialog =
            new OpenFileDialog();

        dialog.Filter =
            "XHTML Files|*.xhtml;*.html";

        if (dialog.ShowDialog()
            == DialogResult.OK)
        {
            txtInputFile.Text =
                dialog.FileName;
        }
    }

    //--------------------------------------------------
    // PROCESS
    //--------------------------------------------------

    private async void btnProcess_Click(
        object sender,
        EventArgs e)
    {
        try
        {
            btnProcess.Enabled = false;

            _cancellationTokenSource = new CancellationTokenSource();

            btnCancel.Enabled = true;

            progressBar.Value = 0;

            txtLog.Clear();

            //--------------------------------------------------
            // VALIDATION
            //--------------------------------------------------

            if (!File.Exists(
                txtInputFile.Text))
            {
                MessageBox.Show(
                    "Please select a valid XHTML file.");

                return;
            }

            //--------------------------------------------------
            // LOAD XHTML
            //--------------------------------------------------

            txtLog.AppendText(
                "Loading XHTML..."
                + Environment.NewLine);

            var phrases =
                _parser.Parse(
                    txtInputFile.Text);


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
                        txtInputFile.Text)!,

                    Path.GetFileNameWithoutExtension(
                        txtInputFile.Text)
                    + "_semantic.xhtml");

            //--------------------------------------------------
            // SAVE
            //--------------------------------------------------

            await File.WriteAllTextAsync(
                output,
                xhtml);

            //--------------------------------------------------
            // COMPLETE
            //--------------------------------------------------

            txtLog.AppendText(
                "Completed"
                + Environment.NewLine);

            txtLog.AppendText(
                output
                + Environment.NewLine);

            MessageBox.Show(
                "Semantic XHTML generated successfully.");
        }
        catch (OperationCanceledException)
        {
            txtLog.AppendText(
                "Operation cancelled."
                + Environment.NewLine);

            MessageBox.Show(
                "GPT structuring cancelled.");
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                ex.ToString());
        }

        finally
        {
            btnProcess.Enabled = true;

            btnCancel.Enabled = false;

            _cancellationTokenSource?.Dispose();

            _cancellationTokenSource = null;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        btnCancel.Enabled = false;

        txtLog.AppendText(
            "Cancelling..."
            + Environment.NewLine);

        _cancellationTokenSource?.Cancel();
    }
}