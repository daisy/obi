using System.Collections.Generic;

namespace Zaboom.Commands
{
    public class CommandManager: urakawa.undo.CommandManager
    {
        private UserControls.ProjectPanel projectPanel;

        public CommandManager(UserControls.ProjectPanel projectPanel)
            : base()
        {
            this.projectPanel = projectPanel;
        }

        public void AddAndExecute(Command command)
        {
            command.CommandManager = this;
            execute(command);
        }

        public UserControls.ProjectPanel ProjectPanel { get { return projectPanel; } }
    }

    public abstract class Command: urakawa.undo.ICommand
    {
        protected CommandManager manager;
        protected List<UserControls.Selectable> previousSelection;
        
        public CommandManager CommandManager
        {
            set
            {
                manager = value;
                previousSelection = new List<UserControls.Selectable>(manager.ProjectPanel.Selected);
            }
        }

        #region ICommand Members

        public bool canUnExecute() { return true; }
        public abstract void execute();
        public abstract string getExecuteShortDescription();
        public abstract string getUnExecuteShortDescription();
        public abstract void unExecute();

        #endregion
    }
}
