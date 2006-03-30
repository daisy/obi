using System;
using System.Collections.Generic;

namespace Protobi
{
    /// <summary>
    /// The basic command class. All commands inherit from this one.
    /// </summary>
    public abstract class Command
    {
        public virtual string Label { get { return null; } } 
        public abstract void Do();
        public abstract void Undo();
    }

    /// <summary>
    /// A cons command looks like a regular command but in fact incorporates a series of commands.
    /// </summary>
    public class ConsCommand : Command
    {
        private string mLabel;            // the label that will be shown to the user
        private Command mCar;             // the first command
        private Command mCdr;             // the rest

        public override string Label { get { return mLabel; } }

        /// <summary>
        /// Create a new cons command from two existing commands, and specifiy the label to use for the resulting command.
        /// For the moment, the label has an ellipsis (...) appended to show that it is a ConsCommand.
        /// </summary>
        /// <param name="label">The label that will appear for this command.</param>
        /// <param name="car">The first command.</param>
        /// <param name="cdr">The second command.</param>
        public ConsCommand(string label, Command car, Command cdr)
        {
            mLabel = label + " (...)";  // for debugging purposes only
            mCar = car;
            mCdr = cdr;
        }

        /// <summary>
        /// Do the car command first, then the cdr.
        /// </summary>
        public override void Do()
        {
            mCar.Do();
            mCdr.Do();
        }

        /// <summary>
        /// Undo the cdr command first, then the car.
        /// </summary>
        public override void Undo()
        {
            mCdr.Undo();
            mCar.Undo();
        }
    }

    /// <summary>
    /// A stack to manage undo/redo.
    /// </summary>
    //  (Actually we use two stacks, but that's an implementation detail.)
    public class UndoRedoStack
    {
        Stack<Command> mUndo;  // the undo stack
        Stack<Command> mRedo;  // the redo stack

        public string UndoLabel { get { return mUndo.Count == 0 ? null : mUndo.Peek().Label; } }
        public string RedoLabel { get { return mRedo.Count == 0 ? null : mRedo.Peek().Label; } }

        /// <summary>
        /// Create the undo stack.
        /// </summary>
        public UndoRedoStack()
        {
            mUndo = new Stack<Command>();
            mRedo = new Stack<Command>();
        }

        /// <summary>
        /// Push a new command to the undo stack and clear the redo stack.
        /// It is assumed that the command just has, or will immediatly be executed.
        /// </summary>
        /// <param name="command">The latest command.</param>
        public void Push(Command command)
        {
            mUndo.Push(command);
            mRedo.Clear();
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
