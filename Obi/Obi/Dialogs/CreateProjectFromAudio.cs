using Obi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Obi.Dialogs
{
    public partial class CreateProjectFromAudio : Form
    {
        private string m_txtOutputPath;
        private CancellationTokenSource? _cts;
        public CreateProjectFromAudio()
        {
            InitializeComponent();
        }

        private void m_btnBrowseAudio_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dialog = new();

            dialog.Filter =
                "Audio Files|*.mp3;*.wav;*.m4a;*.aac;*.flac";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txtAudioPath.Text = dialog.FileName;

                m_txtOutputPath = Path.ChangeExtension(dialog.FileName, ".xhtml");
            }
        }

        private async void m_btnStart_Click(object sender, EventArgs e)
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

                m_btnStart.Enabled = false;

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
                m_btnStart.Enabled = true;

                progressBar.Style =
                    ProgressBarStyle.Blocks;
            }
        }

        private void m_btnCancel_Click(object sender, EventArgs e)
        {
            _cts?.Cancel();

            lblStatus.Text =
                "Cancelled";
        }
    }
}
