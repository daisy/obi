namespace Obi.Commands
{
    /// <summary>
    /// Touch the project, making it seem modified.
    /// (Also used in the GUI to refresh the display.)
    /// </summary>
    public class Touch : Command__OLD__
    {
        private Project mProject;

        public override string Label
        {
            get { return Localizer.Message("touch_command_label"); }
        }

        /// <summary>
        /// Create a new command.
        /// </summary>
        /// <param name="project">The project touched.</param>
        public Touch(Project project)
            : base()
        {
            mProject = project;
        }

        /// <summary>
        /// Do: touch the project.
        /// </summary>
        public override void Do()
        {
            mProject.Touch();
        }

        /// <summary>
        /// Undo: nothing to do.
        /// </summary>
        public override void Undo()
        {
        }
    }
}
