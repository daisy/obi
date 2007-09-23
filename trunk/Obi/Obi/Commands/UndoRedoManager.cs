using System;

namespace Obi.Commands
{
    public class UndoRedoEventArgs: EventArgs
    {
        public urakawa.undo.ICommand Command;
        public UndoRedoManager Manager;

        public UndoRedoEventArgs(urakawa.undo.ICommand command, UndoRedoManager manager)
        {
            Command = command;
            Manager = manager;
        }
    }

    public delegate void UndoRedoEventHandler(object sender, UndoRedoEventArgs e);

    public class UndoRedoManager: urakawa.undo.UndoRedoManager
    {
        public event UndoRedoEventHandler CommandExecuted;
        public event UndoRedoEventHandler CommandUnexecuted;

        public UndoRedoManager()
            : base()
        {
        }

        public override void execute(urakawa.undo.ICommand command)
        {
            base.execute(command);
            if (CommandExecuted != null) CommandExecuted(this, new UndoRedoEventArgs(command, this));
        }

        // TODO get the command from the undo stack
        public override void undo()
        {
            base.undo();
            if (CommandUnexecuted != null) CommandUnexecuted(this, new UndoRedoEventArgs(null, this));
        }

        // TODO get the command from the redo stack
        public override void redo()
        {
            base.redo();
            if (CommandExecuted != null) CommandExecuted(this, new UndoRedoEventArgs(null, this));
        }
    }
}
