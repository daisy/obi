using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Models
{
    public class TranscriptionOptions
    {
        public WhisperModel WhisperModel
        {
            get;
            set;
        }

        public string Language
        {
            get;
            set;
        } = "auto";
    }
}
