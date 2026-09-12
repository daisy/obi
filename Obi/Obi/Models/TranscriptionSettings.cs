namespace Obi.Models
{
    public class TranscriptionSettings
    {
        public TranscriptionEngine Engine { get; set; }

        public string Language { get; set; } = "en";

        public WhisperModel WhisperModel { get; set; }
    }
}