using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Strips
{
    public class AddNewStrip: Commands.Node.AddNewSection
    {
        // This is a bit ugly, but we don't have a dummy node yet. The point is that
        // the new strip is always right after the selected one: if the selected strip
        // has children, then the new strip is added as a first child; otherwise as the
        // next sibling.
        public AddNewStrip(ProjectView.ProjectView view)
            : base(view, new NodeSelection(view.Selection.Node, view.Selection.Control,         
                    view.Selection.Node is SectionNode && ((SectionNode)view.Selection.Node).SectionChildCount > 0)) {}
        public override string getShortDescription() { return Localizer.Message("add_strip_command"); }
    }
}
