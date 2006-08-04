using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    public abstract class TitleAuthor
    {
        private string mIdAttr;  // id attribute, from an existing NCX file -- not used right now
        private Text mText;      // text element, mandatory
        private Audio mAudio;    // audio element, mandatory

        public Text Text { get { return mText; } set { mText = value; } }
        public Audio Audio { get { return mAudio; } set { mAudio = value; } }

        public TitleAuthor(Text text, Audio audio)
        {
            mIdAttr = null;
            mText = text;
            mAudio = audio;
        }
    }

    public class Title : TitleAuthor
    {
        public Title(Text text, Audio audio) : base(text, audio) { }
    }

    public class Author : TitleAuthor
    {
        public Author(Text text, Audio audio) : base(text, audio) { }
    }
}
