using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace NCXEdit.NCX
{
    // public enum TextDir { LTR, RTL };

    public class NavLabel
    {
        private Text mText;
        private Audio mAudio;
        // private CultureInfo mLang;
        // private TextDir mDir;

        public NavLabel(Text text, Audio audio)
        {
            mText = text;
            mAudio = audio;
        }

        public NavLabel(Text text)
        {
            mText = text;
            mAudio = null;
        }

        public NavLabel(Audio audio)
        {
            mText = null;
            mAudio = audio;
        }

        public override string ToString()
        {
            if (mText == null)
            {
                return mAudio.ToString();
            }
            else
            {
                if (mAudio == null)
                {
                    return mText.ToString();
                }
                else
                {
                    return String.Format("<{0}, {1}>", mText, mAudio);
                }
            }
        }

        public string ToXML()
        {
            return "<navLabel>" + XMLText() + XMLAudio() + "</navLabel>";
        }

        private string XMLText()
        {
            return mText == null ? "" : mText.ToXML();
        }

        private string XMLAudio()
        {
            return mAudio == null ? "" : mAudio.ToXML();
        }
    }
}
