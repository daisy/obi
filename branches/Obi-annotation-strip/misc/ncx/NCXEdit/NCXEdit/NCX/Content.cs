using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    public class Content
    {
        private string mId;  // the id normally comes from an XML file
        private Uri mSrc;    // the source URI

        public Uri Src { get { return mSrc; } }

        public Content(Uri src)
        {
            mId = String.Format("content_{0}", GetHashCode());
            mSrc = src;
        }

        public string ToXML()
        {
            return String.Format("<content id=\"{0}\" src=\"{1}\"/>", mId, mSrc);
        }
    }
}
