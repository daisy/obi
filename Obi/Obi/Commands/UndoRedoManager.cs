using System;

namespace Obi.Commands
{
    public class UndoRedoManager: urakawa.undo.UndoRedoManager
    {
        public event EventHandler CommandExecuted;
        public event EventHandler CommandUnexecuted;

        public UndoRedoManager()
            : base()
        {
        }

        public override void execute(urakawa.undo.ICommand command)
        {
            base.execute(command);
            if (CommandExecuted != null) CommandExecuted(this, new EventArgs());
        }

        public override void undo()
        {
            base.undo();
            if (CommandUnexecuted != null) CommandUnexecuted(this, new EventArgs());
        }

        public override void redo()
        {
            base.redo();
            if (CommandExecuted != null) CommandExecuted(this, new EventArgs());
        }
    }
}
