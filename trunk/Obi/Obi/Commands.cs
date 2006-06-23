// All the actual commands used in the prototype.
// See Command.cs for the command/undo framework.

using System;
using System.Drawing;

namespace Obi
{
    public class AddStripCommand : Command
    {
        private UserControls.NRParStrip mStrip;  // the strip that was added

        public override string Label { get { return Localizer.Message("add strip"); } }

        public AddStripCommand(UserControls.NRParStrip strip)
        {
            mStrip = strip;
        }

        public override void Do()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void Undo()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }

    /// <summary>
    /// Change the size of a strip.
    /// </summary>
    public class ResizeStripCommand : Command
    {
        private UserControls.EmptyStrip mStrip;  // the strip to resize
        private Size mSizeBefore;        // the original size of the strip
        private Size mSizeAfter;         // the new size of the strip

        public override string Label { get { return Localizer.Message("resize_strip"); } }

        /// <summary>
        /// Create a resize strip command *after* the change has occurred.
        /// </summary>
        /// <param name="strip">The strip to resize.</param>
        /// <param name="before">The original size of the strip, before the change.</param>
        /// <param name="after">The new size of the strip, after the change.</param>
        public ResizeStripCommand(UserControls.EmptyStrip strip, Size before, Size after)
        {
            mStrip = strip;
            mSizeBefore = before;
            mSizeAfter = after;
        }

        /// <summary>
        /// Change the size of the strip to its new size.
        /// </summary>
        public override void Do()
        {
            mStrip.Size = mSizeAfter;
            //mStrip.Parent.Refresh();
        }

        /// <summary>
        /// Change the size of the strip back to its old size.
        /// </summary>
        public override void Undo()
        {
            mStrip.Size = mSizeBefore;
            //mStrip.UserControl.Parent.Refresh();
        }
    }
}
