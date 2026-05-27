using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace AudioTranscriber.Models
{
    public class WhisperXResult
    {
        [JsonPropertyName("phrases")]
        public List<WhisperXPhrase>
            Phrases
        {
            get;
            set;
        } = new();


    }

    public class WhisperXPhrase
    {
        [JsonPropertyName("phraseId")]
        public string PhraseId
        {
            get;
            set;
        } = string.Empty;

        [JsonPropertyName("start")]
        public double Start
        {
            get;
            set;
        }

        [JsonPropertyName("end")]
        public double End
        {
            get;
            set;
        }

        [JsonPropertyName("text")]
        public string Text
        {
            get;
            set;
        } = string.Empty;

        [JsonPropertyName("words")]
        public List<WhisperXWord>
            Words
        {
            get;
            set;
        } = new();

        [JsonPropertyName("confidence")]
        public double Confidence
        {
            get;
            set;
        }
    }


}