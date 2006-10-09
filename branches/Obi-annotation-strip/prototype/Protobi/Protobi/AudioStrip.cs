using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    public class AudioStrip : SeqStrip
    {
        public override bool CanAddAudioFile { get { return ((AudioStripModel)mModel).WaveFile == null; } }

        public AudioStrip(StripManager manager, string label, ParStrip parent)
        {
            InitBaseMembers(manager, parent);
            InitModelUserControl(label);
        }

        protected override void InitModelUserControl(string label)
        {
            mModel = new AudioStripModel(label, (ParStripModel)mParent.Model);
            mUserControl = new AudioStripUserControl(this);
        }

        public void LoadFile(WaveFile file)
        {
            ((AudioStripModel)mModel).WaveFile = file;
            ((AudioStripUserControl)mUserControl).WaveFile = file; 
        }
    }

    public class AudioStripModel : SeqStripModel
    {
        private WaveFile mFile;

        public WaveFile WaveFile
        {
            get { return mFile; }
            set { if (mFile == null) mFile = value; }
        }

        public AudioStripModel(string label, ParStripModel parent)
            : base(label, parent)
        {
            mFile = null;
        }
    }
}
