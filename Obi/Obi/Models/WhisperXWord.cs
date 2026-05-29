using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Obi.Models
{
    public class WhisperXWord
    {
        [JsonPropertyName("word")]
        public string Word
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

        [JsonPropertyName("confidence")]
        public double Confidence
        {
            get;
            set;
        }
    }
}
