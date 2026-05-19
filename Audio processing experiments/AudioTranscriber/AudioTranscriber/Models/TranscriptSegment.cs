using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioTranscriber.Models
{
    public class TranscriptSegment
    {
        public string Text { get; set; } =
            string.Empty;

        public TimeSpan Start { get; set; }

        public TimeSpan End { get; set; }
    }
}