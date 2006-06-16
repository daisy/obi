using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using Obi.Strips;

namespace Obi.NCX
{
    /// <summary>
    /// NCX navigation point.
    /// </summary>
    public class Navpoint
    {
        private Navpoint mParent;          // parent navpoint or navmap
        protected uint mLevel;             // level (greater than 0)
        private Navpoint mPrev;            // previous navpoint in navmap
        private Navpoint mNext;            // next navpoint in navmap
        private List<Navpoint> mChildren;  // children navpoints
        private ParStrip mContent;         // content strip
        private List<Navlabel> mLabels;    // list of navlabels

        /// <summary>
        /// Create a new Navpoint that is not yet part of any navmap. 
        /// </summary>
        /// <param name="content">Content strip for this navpoint.</param>
        /// <param name="labels">List of all labels.</param>
        public Navpoint(ParStrip content, List<Navlabel> labels)
        {
            mParent = null;
            mLevel = 0;
            mPrev = null;
            mNext = null;
            mChildren = new List<Navpoint>();
            mContent = content;
            mLabels = labels;
        }
    }
}
