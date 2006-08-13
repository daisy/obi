using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Commands
{
    class CommandManager
    {
        Stack<Command> mUndo;  // the undo stack
        Stack<Command> mRedo;  // the redo stack

        /// <summary>
        /// The label of the command on top of the undo stack, or null if the stack is empty.
        /// </summary>
        public string UndoLabel
        {
            get
            {
                return mUndo.Count == 0 ? null : mUndo.Peek().Label;
            }
        }

        /// <summary>
        /// The label of the command on top of the redo stack, or null if the stack is empty.
        /// </summary>
        public string RedoLabel
        {
            get
            {
                return mRedo.Count == 0 ? null : mRedo.Peek().Label;
            }
        }

        /// <summary>
        /// Test whether there currently is a command to undo.
        /// </summary>
        public bool HasUndo
        {
            get
            {
                return mUndo.Count > 0;
            }
        }

        /// <summary>
        /// Test whether there currently is a command to undo.
        /// </summary>
        public bool HasRedo
        {
            get
            {
                return mRedo.Count > 0;
            }
        }

        /// <summary>
        /// Create an empty undo stack.
        /// </summary>
        public CommandManager()
        {
            mUndo = new Stack<Command>();
            mRedo = new Stack<Command>();
        }

        /// <summary>
        /// Clear the undo/redo stack, used when reading in a new project.
        /// </summary>
        public void Clear()
        {
            mUndo.Clear();
            mRedo.Clear();
        }

        /// <summary>
        /// Push a new command to the undo stack and clear the redo stack.
        /// It is assumed that the command just has, or will immediatly be executed.
        /// </summary>
        /// <param name="command">The latest command.</param>
        public void Add(Command command)
        {
            mUndo.Push(command);
            mRedo.Clear();
            System.Windows.Forms.MessageBox.Show(string.Format("added {0}", command.ToString()));
        }

        /// <summary>
        /// Undo the last command and push it on the redo stack. There is no effect if the undo stack is empty.
        /// </summary>
        public void Undo()
        {
            if (mUndo.Count > 0)
            {
                Command command = mUndo.Pop();
                command.Undo();
                mRedo.Push(command);
            }
        }

        /// <summary>
        /// Redo the last command that was undone and put it back on the undo stack.
        /// There is no effect if the redo stack is empty.
        /// </summary>
        public void Redo()
        {
            if (mRedo.Count > 0)
            {
                Command command = mRedo.Pop();
                command.Do();
                mUndo.Push(command);
            }
        }
    }
}
