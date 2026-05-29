using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Models
{
    public class TranscriptSegment
    {
        public string PhraseId
        {
            get;
            set;
        } = string.Empty;

        public string Text
        {
            get;
            set;
        } = string.Empty;

        public TimeSpan Start
        {
            get;
            set;
        }

        public TimeSpan End
        {
            get;
            set;
        }

        public List<WordTimestamp> Words
        {
            get;
            set;
        } = new();

        public double Confidence
        {
            get;
            set;
        }
    }
}
