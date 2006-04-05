using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    public class ParStrip : Strip
    {
        private StructureStrip mStructureStrip;
        private AudioStrip mAudioStrip;

        public StructureStrip StructureStrip { get { return mStructureStrip; } }
        public AudioStrip AudioStrip { get { return mAudioStrip; } }
        public override bool CanAddAudioStrip { get { return mAudioStrip == null; } }

        public ParStrip(StripManager manager, string label)
        {
            InitBaseMembers(manager);
            InitModelUserControl(label);
        }

        protected override void InitModelUserControl(string label)
        {
            mStructureStrip = new StructureStrip(Manager, Localizer.GetString("structure_strip_label"), this);
            mModel = new ParStripModel(label, (StructureStripModel)mStructureStrip.Model);
            mUserControl = new ParStripUserControl(this);
            ((ParStripUserControl)mUserControl).AddStripUserControl(mStructureStrip.UserControl);
            ((StructureStripUserControl)(mStructureStrip.UserControl)).ParentStrip = (ParStripUserControl)mUserControl;
            mAudioStrip = null;
            Console.WriteLine("Bing!");
        }

        public void AddAudioStrip()
        {
            mAudioStrip = new AudioStrip(Manager, Localizer.GetString("audio_strip"), this);
            ((ParStripUserControl)mUserControl).AddStripUserControl(mAudioStrip.UserControl);
            ((AudioStripUserControl)(mAudioStrip.UserControl)).ParentStrip = (ParStripUserControl)mUserControl;
        }
    }

    public class ParStripModel : StripModel
    {
        private StructureStripModel mStructureStrip;
        private AudioStripModel mAudioStrip;

        public StructureStripModel StructureStrip { get { return mStructureStrip; } }
        public AudioStripModel AudioStrip
        {
            get { return mAudioStrip; }
            set { if (mAudioStrip == null) mAudioStrip = value; }
        }

        public ParStripModel(string label, StructureStripModel structureStrip)
            : base(label)
        {
            mStructureStrip = structureStrip;
            mAudioStrip = null;
        }
    }
}
