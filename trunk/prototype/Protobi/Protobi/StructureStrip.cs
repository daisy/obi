using System;
using System.Collections.Generic;
using System.Text;

namespace Protobi
{
    public class StructureStrip : SeqStrip
    {
        public StructureHead Heading { get { return ((StructureStripModel)mModel).Heading; } }

        public StructureStrip(StripManager manager, string label, ParStrip parent)
        {
            InitBaseMembers(manager, parent);
            InitModelUserControl(label, parent);
        }

        protected void InitModelUserControl(string label, ParStrip parent)
        {
            mModel = new StructureStripModel(label, (ParStripModel)parent.Model);
            mUserControl = new StructureStripUserControl(this);
            
        }
    }

    public class StructureStripModel : SeqStripModel
    {
        private StructureHead mHeading;
        private List<PageItem> mPages;

        public StructureHead Heading { get { return mHeading; } }
     
        public StructureStripModel(string label, ParStripModel parent)
            : base(label, parent)
        {
            mHeading = new StructureHead(Localizer.GetString("default_heading"), 1);
            mPages = new List<PageItem>();
        }
    }
}
