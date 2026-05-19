using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioTranscriber.Services
{
    public static class ModelDownloader
    {
        private const string ModelUrl =
            "https://huggingface.co/ggerganov/whisper.cpp/resolve/main/ggml-large-v3.bin";

        public static string GetModelPath()
        {
            string modelFolder =
                Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Assets",
                    "Models");

            Directory.CreateDirectory(modelFolder);

            return Path.Combine(
                modelFolder,
                "ggml-large-v3.bin");
        }

        public static async Task DownloadModelAsync(
            IProgress<int>? progress = null,
            CancellationToken cancellationToken = default)
        {
            string modelPath = GetModelPath();

            if (File.Exists(modelPath))
                return;

            using HttpClient client = new();

            client.Timeout =
                TimeSpan.FromHours(2);

            using HttpResponseMessage response =
                await client.GetAsync(
                    ModelUrl,
                    HttpCompletionOption.ResponseHeadersRead,
                    cancellationToken);

            response.EnsureSuccessStatusCode();

            long? totalBytes =
                response.Content.Headers.ContentLength;

            await using Stream contentStream =
                await response.Content.ReadAsStreamAsync(
                    cancellationToken);

            await using FileStream fileStream =
                new FileStream(
                    modelPath,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    81920,
                    true);

            byte[] buffer = new byte[81920];

            long totalRead = 0;

            int bytesRead;

            while ((bytesRead =
                await contentStream.ReadAsync(
                    buffer,
                    cancellationToken)) > 0)
            {
                await fileStream.WriteAsync(
                    buffer.AsMemory(
                        0,
                        bytesRead),
                    cancellationToken);

                totalRead += bytesRead;

                if (totalBytes.HasValue &&
                    progress != null)
                {
                    int percent =
                        (int)((double)totalRead /
                        totalBytes.Value * 100);

                    progress.Report(percent);
                }
            }
        }
    }
}