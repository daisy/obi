using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Obi.NCX
{
    public enum TextDirection { Default, LTR, RTL };

    public class Navlabel
    {
        private string mText;        // optional text element
        private Audio mAudio;        // optional audio element
        private CultureInfo mLang;   // language of this label
        private TextDirection mDir;  // text direction

        public Navlabel(string text, Audio audio, CultureInfo lang, TextDirection dir)
        {
            mText = text;
            mAudio = audio;
            mLang = lang;
            mDir = dir;
        }
    }
}
