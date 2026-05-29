using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Obi.Models
{
    public class WordTimestamp
    {
        public string Word
        {
            get;
            set;
        } = string.Empty;

        public double Start
        {
            get;
            set;
        }

        public double End
        {
            get;
            set;
        }

        public double Confidence
        {
            get;
            set;
        }
    }
}
