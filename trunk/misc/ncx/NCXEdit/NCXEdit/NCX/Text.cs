using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    public class Text
    {
        private string mId;     // the id normally comes from an XML file
        private string mClass;  // the class attribute
        private string mText;   // the text itself

        public string ClassAttr { get { return mClass; } set { mClass = value; } }

        public Text(string text)
        {
            mId = String.Format("text_{0}", GetHashCode());
            mClass = null;
            mText = text;
        }

        public override string ToString()
        {
            return mText;
        }

        private static string Quote(string text)
        {
            return text;
        }

        public string ToXML()
        {
            string _class = mClass == null ? "" : String.Format(" class=\"{0}\"", mClass);
            return String.Format("<text id=\"{0}\"{1}>{2}</text>", mId, _class, Quote(mText));
        }
    }
}
