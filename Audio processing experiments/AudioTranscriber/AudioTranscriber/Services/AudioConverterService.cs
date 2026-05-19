using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFMpegCore;

namespace AudioTranscriber.Services
{
    public static class AudioConverterService
    {
        public static async Task<string>
            ConvertToWhisperFormatAsync(
                string inputFile)
        {
            string tempFolder =
                Path.Combine(
                    Path.GetTempPath(),
                    "AudioTranscriber");

            Directory.CreateDirectory(
                tempFolder);

            string outputFile =
                Path.Combine(
                    tempFolder,
                    $"{Guid.NewGuid()}.wav");

            await FFMpegArguments
                .FromFileInput(inputFile)
                .OutputToFile(
                    outputFile,
                    true,
                    options => options
                        .WithAudioCodec("pcm_s16le")
                        .WithCustomArgument("-ar 16000")
                        .WithCustomArgument("-ac 1"))
                .ProcessAsynchronously();

            return outputFile;
        }
    }
}
