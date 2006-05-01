using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    public class Audio
    {
        private string mId;           // the id normally comes from an XML file
        private string mClass;        // the class attribute
        private Uri mSrc;             // the source URI
        private DateTime mClipBegin;  // clip begin time
        private DateTime mClipEnd;    // clip end time

        public string ClassAttr { get { return mClass; } set { mClass = value; } }
        public Uri Src { get { return mSrc; } }
        public DateTime ClipBegin { get { return mClipBegin; } }
        public DateTime ClipEnd { get { return mClipEnd; } }

        public Audio(Uri src, DateTime clipBegin, DateTime clipEnd)
        {
            mId = null; //  String.Format("audio_{0}", GetHashCode());
            mClass = null;
            mSrc = src;
            mClipBegin = clipBegin;
            mClipEnd = clipEnd;
        }

        public override string ToString()
        {
            return String.Format("@audio({0}:{1}:{2})", mSrc, mClipBegin, mClipEnd);
        }

        public string ToXML()
        {
            string _class = mClass == null ? "" : String.Format(" class=\"{0}\"", mClass);
            return String.Format("<audio id=\"{0}\"{1} src=\"{2}\" clipBegin=\"{3}\" ClipEnd=\"{4}\"/>",
                mId, _class, mSrc, mClipBegin, mClipEnd);
        }
    }
}
