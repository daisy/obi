using AudioTranscriber.Helpers;
using AudioTranscriber.Services;

namespace AudioTranscriber
{
    public partial class MainForm : Form
    {
        private CancellationTokenSource? _cts;

        public MainForm()
        {
            InitializeComponent();

            AllowDrop = true;

            txtAudioPath.DragEnter += TxtAudioPath_DragEnter;
            txtAudioPath.DragDrop += TxtAudioPath_DragDrop;

            btnBrowseAudio.Click += BtnBrowseAudio_Click;
            btnBrowseOutput.Click += BtnBrowseOutput_Click;
            btnStart.Click += BtnStart_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnBrowseAudio_Click(
            object? sender,
            EventArgs e)
        {
            using OpenFileDialog dialog = new();

            dialog.Filter =
                "Audio Files|*.mp3;*.wav;*.m4a;*.aac;*.flac";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtAudioPath.Text = dialog.FileName;

                txtOutputPath.Text =
                    Path.ChangeExtension(
                        dialog.FileName,
                        ".xhtml");
            }
        }

        private void BtnBrowseOutput_Click(
            object? sender,
            EventArgs e)
        {
            using SaveFileDialog dialog = new();

            dialog.Filter =
                "Text File|*.txt";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtOutputPath.Text =
                    dialog.FileName;
            }
        }

        private void TxtAudioPath_DragEnter(
            object? sender,
            DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(
                DataFormats.FileDrop) == true)
            {
                e.Effect = DragDropEffects.Copy;
            }
        }

        private void TxtAudioPath_DragDrop(
            object? sender,
            DragEventArgs e)
        {
            string[]? files =
                e.Data?.GetData(
                    DataFormats.FileDrop)
                as string[];

            if (files?.Length > 0)
            {
                txtAudioPath.Text = files[0];

                txtOutputPath.Text =
                    Path.ChangeExtension(
                        files[0],
                        ".xhtml");
            }
        }

        private async void BtnStart_Click(
    object sender,
    EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(
                    txtAudioPath.Text))
                {
                    MessageBox.Show(
                        "Please select an audio file.");

                    return;
                }

                btnStart.Enabled = false;

                progressBar.Style =
                    ProgressBarStyle.Marquee;

                lblStatus.Text =
                    "Transcribing audio...";

                _cts =
                    new CancellationTokenSource();

                WhisperXService whisper =
                    new();

                // STEP 1:
                // Transcribe audio
                var segments =
                    await whisper.TranscribeAsync(
                        txtAudioPath.Text,
                        _cts.Token);

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

                lblStatus.Text =
                    "Completed";

                MessageBox.Show(
                    $"Transcription completed successfully.\n\n" +
                    $"XHTML saved at:\n{xhtmlPath}");
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show(
                    "Operation cancelled.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString());
            }
            finally
            {
                btnStart.Enabled = true;

                progressBar.Style =
                    ProgressBarStyle.Blocks;
            }
        }

        //  private async void BtnStart_Click(
        //object? sender,
        //EventArgs e)
        //  {
        //      try
        //      {
        //          if (!File.Exists(txtAudioPath.Text))
        //          {
        //              MessageBox.Show(
        //                  "Please select audio file.");

        //              return;
        //          }

        //          btnStart.Enabled = false;
        //          btnCancel.Enabled = true;

        //          progressBar.Visible = true;

        //          dgvTranscript.Rows.Clear();

        //          lblStatus.Text =
        //              "Preparing model...";

        //          _cts =
        //              new CancellationTokenSource();

        //          await ModelDownloader
        //              .DownloadModelAsync();

        //          WhisperXService whisper = new();

        //          Progress<string> progress =
        //              new(status =>
        //              {
        //                  lblStatus.Text =
        //                      status;
        //              });

        //          var result =  await whisper.TranscribeAsync(txtAudioPath.Text,_cts.Token);

        //          lblStatus.Text =
        //              "Populating transcript...";

        //          foreach (var item in result)
        //          {
        //              dgvTranscript.Rows.Add(
        //                  item.Start.ToString(
        //                      @"hh\:mm\:ss\.fff"),

        //                  item.End.ToString(
        //                      @"hh\:mm\:ss\.fff"),

        //                  item.Text);
        //          }

        //          lblStatus.Text =
        //              "Saving TXT file...";

        //          await ExportService
        //              .SaveTxtAsync(
        //                  result,
        //                  txtOutputPath.Text);

        //          lblStatus.Text =
        //              "Completed";

        //          MessageBox.Show(
        //              "Transcription completed successfully.");
        //      }
        //      catch (OperationCanceledException)
        //      {
        //          lblStatus.Text =
        //              "Cancelled";
        //      }
        //      catch (Exception ex)
        //      {
        //          Logger.Log(ex);

        //          MessageBox.Show(
        //              ex.ToString());
        //      }
        //      finally
        //      {
        //          btnStart.Enabled = true;
        //          btnCancel.Enabled = false;

        //          progressBar.Visible = false;
        //      }
        //  }

        private void BtnCancel_Click(
            object? sender,
            EventArgs e)
        {
            _cts?.Cancel();

            lblStatus.Text =
                "Cancelled";
        }
    }
}