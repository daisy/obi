using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands.Strips
{
    public class AddNewStrip: Commands.Node.AddNewSection
    {
        public AddNewStrip(ProjectView.ProjectView view) : base(view) { }
        public override string getShortDescription() { return Localizer.Message("add_strip_command"); }
    }
}
