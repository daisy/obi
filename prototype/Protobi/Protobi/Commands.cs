// All the actual commands used in the prototype.
// See Command.cs for the command/undo framework.

using System;
using System.Drawing;

namespace Protobi
{
    /// <summary>
    /// Append a strip to the strip manager.
    /// </summary>
    public class AppendStripCommand : Command
    {
        protected StripManagerController mManager;  // the strip manager
        protected StripController mStrip;           // the strip to append

        public override string Label { get { return Localizer.GetString("append_strip"); } }

        /// <summary>
        /// Create an append strip command. The strip will be created automatically.
        /// </summary>
        /// <param name="manager">The strip manager to append to.</param>
        public AppendStripCommand(StripManagerController manager)
        {
            mManager = manager;
            mStrip = null;
        }

        /// <summary>
        /// Append the strip. The first time it will be created; when redoing, the same strip will be appended again.
        /// </summary>
        public override void Do()
        {
            if (mStrip == null)
            {
                mStrip = mManager.AppendStrip();
            }
            else
            {
                mManager.AppendStrip(mStrip);
            }
        }

        /// <summary>
        /// Remove the strip from the strip manager.
        /// </summary>
        public override void Undo()
        {
            mManager.RemoveLastStrip();
        }
    }

    public class AppendContainerStripCommand : AppendStripCommand
    {
        public override string Label { get { return Localizer.GetString("append_container_strip"); } }

        public AppendContainerStripCommand(StripManagerController manager): base(manager)
        {
        }

        public override void Do()
        {
            if (mStrip == null)
            {
                mStrip = mManager.AppendContainerStrip();
            }
            else
            {
                mManager.AppendStrip(mStrip);
            }
        }
    }

    /// <summary>
    /// Rename a strip. Because renaming may change the size of the stip, we may actually need a ConsCommand.
    /// </summary>
    public class RenameStripCommand : Command
    {
        private StripController mStrip;  // the renamed strip
        private string mLabelBefore;     // the original name of the strip
        private string mLabelAfter;      // the new name (once the command is done)

        public override string Label { get { return Localizer.GetString("rename_strip"); } }

        /// <summary>
        /// Create a rename strip command.
        /// </summary>
        /// <param name="strip">The strip to rename.</param>
        /// <param name="after">The new name of the strip.</param>
        public RenameStripCommand(StripController strip, string after)
        {
            mStrip = strip;
            mLabelBefore = mStrip.Label;
            mLabelAfter = after;
        }

        /// <summary>
        /// Change the name of the strip to its new name.
        /// </summary>
        public override void Do()
        {
            mStrip.Label = mLabelAfter;
        }

        /// <summary>
        /// Change the name of the strip back to its old name.
        /// </summary>
        public override void Undo()
        {
            mStrip.Label = mLabelBefore;
        }
    }

    /// <summary>
    /// Change the size of a strip.
    /// </summary>
    public class ResizeStripCommand : Command
    {
        private StripController mStrip;  // the strip to resize
        private Size mSizeBefore;        // the original size of the strip
        private Size mSizeAfter;         // the new size of the strip

        public override string Label { get { return Localizer.GetString("resize_strip"); } }

        /// <summary>
        /// Create a resize strip command *after* the change has occurred.
        /// </summary>
        /// <param name="strip">The strip to resize.</param>
        /// <param name="before">The original size of the strip, before the change.</param>
        /// <param name="after">The new size of the strip, after the change.</param>
        public ResizeStripCommand(StripController strip, Size before, Size after)
        {
            mStrip = strip;
            mSizeBefore = before;
            mSizeAfter = after;
        }

        /// <summary>
        /// Create a resize strip command *before* the change occurs.
        /// The "before" size is actually the current size of the strip. 
        /// </summary>
        /// <param name="strip">The strip to resize</param>
        /// <param name="after">The new size of the strip after the resizing takes effect.</param>
        public ResizeStripCommand(StripController strip, Size after)
        {
            mStrip = strip;
            mSizeBefore = strip.UserControl.Size;
            mSizeAfter = after;
        }

        /// <summary>
        /// Change the size of the strip to its new size.
        /// </summary>
        public override void Do()
        {
            mStrip.UserControl.Size = mSizeAfter;
            mStrip.UserControl.Parent.Refresh();
        }

        /// <summary>
        /// Change the size of the strip back to its old size.
        /// </summary>
        public override void Undo()
        {
            mStrip.UserControl.Size = mSizeBefore;
            mStrip.UserControl.Parent.Refresh();
        }
    }
}
