using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    class NavPoint
    {
        private NavPoint mParent;          // parent navpoint or navmap
        private uint mLevel;               // level
        private NavPoint mPrev;            // previous navpoint
        private NavPoint mNext;            // next navpoint
        private string mIdAttr;            // XML id
        private string mClassAttr;         // XML class attribute
        private List<NavLabel> mLabels;    // navlabels
        private Content mContent;          // content
        private List<NavPoint> mChildren;  // children navpoints

        /// <summary>
        /// Create a new navpoint with the given labels and content.
        /// </summary>
        /// <param name="labels">List of labels.</param>
        /// <param name="content">SMIL content.</param>
        NavPoint(List<NavLabel> labels, Content content)
        {
            mParent = null;
            mLevel = 1;
            mPrev = null;
            mNext = null;
            mIdAttr = String.Format("navpoint_{0}", GetHashCode());
            mClassAttr = null;
            mLabels = labels;
            mChildren = new List<NavPoint>();
        }

        public NavPoint Parent { get { return mParent; } set { mParent = value; } }
        public uint Level { get { return mLevel; } set { mLevel = value; } }
        public NavPoint Prev { get { return mPrev; } set { mPrev = value; } }
        public NavPoint Next { get { return mNext; } set { mNext = value; } }

        /// <summary>
        /// Append a child to the list of children. All links (parent, prev and next) and level are maintained.
        /// </summary>
        /// <param name="child">The navpoint to append to the list of children.</param>
        public void AppendChild(NavPoint child)
        {
            AppendChildRaw(child);
            child.Level = mLevel + 1;
            NavPoint last = LastDescendantOrSelf();
            child.Prev = last;
            NavPoint last_next = last.Next;
            last.Next = child;
            child.Next = last_next;
            if (child.Next != null)
            {
                child.Next.Prev = child;
            }
        }

        /// <summary>
        /// Attempt to move the navigation point up in the navigation map.
        /// </summary>
        /// <returns>True if the move could be made, false otherwise. In case of failure, nothing was changed.</returns>
        public bool MoveUp()
        {
            // These three conditions must be fulfilled to be able to move the navpoint up
            if (mPrev.Prev != null && mPrev.Prev.Level >= mLevel - 1 && mPrev.Level <= mLevel + 1)
            {
                // Find the new parent to add the navpoint to, and the sibling after which to add it.
                // We just need to find the closest navpoint with a level just below ours to attach to,
                // and the sibling will be the first navpoint of the same level that we encounter,
                // starting for the previous navpoint's previous navpoint.
                NavPoint parent = mPrev.Prev;
                NavPoint sibling = null;
                while (parent.Level != mLevel - 1)
                {
                    sibling = parent;
                    parent = parent.Parent;
                }
                mParent.RemoveChild(this);
                mParent.AddChildAfter(this, sibling);
                // Now we update the links, this is a bit tricky
                NavPoint next = mNext;
                NavPoint prev = mPrev;
                mPrev.Prev.Next = this;
                mNext = mPrev;
                mPrev.Next = next;
                if (next != null)
                {
                    next.Prev = mPrev;
                }
                mPrev = mPrev.Prev;
                mPrev.Prev = this;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempt to move the navigation point down in the navigation map. This is equivalent to moving the following
        /// navpoint up (if there is a following navpoint...)
        /// </summary>
        /// <returns>True if the move could be made, false otherwise. In case of failure, nothing was changed.</returns>
        public bool MoveDown()
        {
            if (mNext != null)
            {
                return mNext.MoveUp();
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Add a new child after a (possibly) existing child. If the existing child is undefined, then prepend the new child.
        /// </summary>
        /// <param name="newChild">The new child to add.</param>
        /// <param name="existingChild">The (possibly) existing child to add after.</param>
        private void AddChildAfter(NavPoint newChild, NavPoint existingChild)
        {
            if (existingChild != null)
            {
                mChildren.Insert(mChildren.IndexOf(existingChild) + 1, newChild);
            }
            else
            {
                mChildren.Insert(0, newChild);
            }
        }

        /// <summary>
        /// Append a child but do not update links, except for the parent link.
        /// </summary>
        /// <param name="child">The navpoint to append to the list of children.</param>
        private void AppendChildRaw(NavPoint child)
        {
            child.Parent = this;
            mChildren.Add(child);
        }

        /// <summary>
        /// Find the last descendant of a navpoint and return it, or the navpoint itself if it has no descendant.
        /// </summary>
        /// <returns>Either this navpoint or its last descendant.</returns>
        private NavPoint LastDescendantOrSelf()
        {
            return mChildren.Count == 0 ? this : mChildren[mChildren.Count - 1].LastDescendantOrSelf();
        }

        /// <summary>
        /// Remove a child from the list of children. No other modification is made (no update of links, etc.)
        /// </summary>
        /// <param name="child">The child navpoint to remove.</param>
        private void RemoveChild(NavPoint child)
        {
            mChildren.Remove(child);
        }
    }
}
