using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.media;

namespace Obi.Strips
{
    /// <summary>
    /// ParStrips are similar to par elements in SMIL as they contain parallel contents.
    /// In the current version a par strip contains:
    ///   1. a structure strip
    ///   2. an audio strip
    /// </summary>
    public class ParStrip: Strip
    {
        private string mLabel;                   // the strip label
        private CoreNode mNode;                  // the core node corresponding to this strip
        private StructureStrip mStructureStrip;  // the structure strip
        private AudioStrip mAudioStrip;          // the audio strip

        public delegate void LabelChangedHandler(object sender, LabelChangedEventArgs e);
        public event LabelChangedHandler LabelChanged;

        /// <summary>
        /// The label of the strip is intended to give information to the user.
        /// It should be searchable.
        /// </summary>
        public string Label
        {
            get
            {
                return mLabel;
            }
            set
            {
                string old_label = mLabel;
                mLabel = value;
                LabelChanged(this, new LabelChangedEventArgs(old_label, mLabel, false));
            }
        }

        /// <summary>
        /// Get the presentation that this strip belongs to (from its node)
        /// </summary>
        public Presentation Presentation
        {
            get
            {
                return (Presentation)mNode.getPresentation();
            }
        }

        public ParStrip()
        {
            mLabel = null;
            mStructureStrip = new StructureStrip(this);
            mAudioStrip = new AudioStrip(this);
            mNode = null;
        }

        /// <summary>
        /// Create a new par strip.
        /// </summary>
        /// <param name="label">The strip's label.</param>
        /// <param name="node">The strip's node (i.e. the heading node.)</param>
        public ParStrip(string label, CoreNode node)
        {
            mLabel = label;
            mStructureStrip = new StructureStrip(this);
            mAudioStrip = new AudioStrip(this);
            mNode = node;
        }

        public override string ToString()
        {
            return String.Format("<{0}> \"{1}\"", base.ToString(), mLabel);
        }
    }
}
