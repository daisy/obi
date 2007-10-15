using System;
using urakawa.core;

namespace Obi.Commands.TOC
{
    /// <summary>
    /// Add a new section or subsection, then let the user rename it.
    /// </summary>
    public class AddNewSection: Commands.Node.AddNewSection
    {
        public AddNewSection(ProjectView.ProjectView view, NodeSelection selection) : base(view, selection) { }
        public override string getShortDescription() { return Localizer.Message("add_new_section_command"); }
    }
}