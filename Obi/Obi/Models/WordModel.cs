using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Models
{
    public class WordModel
    {
        public string Text { get; set; } = "";

        public double Start { get; set; }

        public double End { get; set; }

        public double Confidence { get; set; }
    }
}
