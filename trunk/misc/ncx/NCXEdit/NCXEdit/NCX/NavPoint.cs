using System;
using System.Collections.Generic;
using System.Text;

namespace NCXEdit.NCX
{
    public class NavMapOrNavPoint
    {
        protected uint mLevel;               // level (1 or more for navpoint, 0 for navmap)
        protected NavMapOrNavPoint mParent;  // parent navpoint or navmap
        protected NavMapOrNavPoint mPrev;    // previous navpoint, or start of navmap
        protected NavPoint mNext;            // next navpoint
        protected List<NavPoint> mChildren;  // children navpoints

        protected string mIdAttr;            // id attribute (from an existing XML file) -- unused right now

        public NavMapOrNavPoint Parent { get { return mParent; } set { mParent = value; } }
        public NavMapOrNavPoint Prev { get { return mPrev; } set { mPrev = value; } }

        public NavMapOrNavPoint(uint level)
        {
            mLevel = level;
            mParent = null;
            mPrev = null;
            mNext = null;
            mChildren = new List<NavPoint>();
            mIdAttr = null;
        }
    }

    public class NavMap: NavMapOrNavPoint
    {
        /// <summary>
        /// Create a new navigation map. As it cannot be empty, a first navigation point is created as well.
        /// </summary>
        public NavMap(): base(0)
        {
            List<NavLabel> labels = new List<NavLabel>();
            labels.Add(new NavLabel(new Text("*** Edit me! ***")));
            NavPoint navpoint = new NavPoint(labels, new Content(new Uri("")));
            mNext = navpoint;
            navpoint.Parent = this;
            navpoint.Prev = this;
        }
    }

    public class NavPoint: NavMapOrNavPoint
    {
        private List<NavLabel> mLabels;  // navlabels
        private Content mContent;        // content

        private string mClassAttr;       // class attribute from existing XML file -- unused right now

        public NavPoint(List<NavLabel> labels, Content content)
            : base(1)
        {
            mLabels = labels;
            mContent = content;
        }

        /*
        
        
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
            mIdAttr = String.Format("navpoint_{0}", GetHashCode());
            mClassAttr = null;
            mLabels = labels;
            mContent = content;
        }

        public uint Level { get { return mLevel; } set { mLevel = value; } }

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
        /// Decrease the level of a navpoint. This only works if it has no children and the level is higher than 1
        /// (i.e. there is a grand parent)
        /// </summary>
        /// <returns>True if the change could be made, false otherwise. In case of failure, nothing was changed.</returns>
        public bool DecreaseLevel()
        {
            if (mParent.Parent != null && mChildren.Count == 0)
            {
                List<NavPoint> siblings = mParent.RemoveChildAndSiblings(this);
                NavPoint parent = mParent;
                mParent = mParent.Parent;
                mParent.AddChildAfter(this, parent);
                foreach (NavPoint sibling in siblings)
                {
                    AppendChildRaw(sibling);
                }
                --mLevel;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Increase the level: only possible if the node has a previous sibling.
        /// Then that sibling becomes the parent of the node and of its children.
        /// </summary>
        /// <returns>True if the change could be made, false otherwise. In case of failure, nothing was changed.</returns>
        public bool IncreaseLevel()
        {
            NavPoint sibling = mParent.PreviousSibling(this);
            if (sibling != null)
            {
                mParent.RemoveChild(this);
                mParent = sibling;
                mParent.AppendChildRaw(this);
                foreach (NavPoint child in mChildren)
                {
                    mParent.AppendChildRaw(child);
                }
                mChildren.Clear();
                ++mLevel;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Delete a navpoint, only if it has no children.
        /// </summary>
        /// <returns>True if the change could be made, false otherwise. In case of failure, nothing was changed.</returns>
        public bool Remove()
        {
            if (mChildren.Count == 0)
            {
                mParent.RemoveChild(this);
                if (mPrev != null)
                {
                    mPrev.Next = mNext;
                }
                if (mNext != null)
                {
                    mNext.Prev = mPrev;
                }
                return true;
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
        /// Find the previous sibling of a child.
        /// </summary>
        /// <param name="child">The child whose sibling we are looking for.</param>
        /// <returns>The previous sibling or null if it was the first child.</returns>
        private NavPoint PreviousSibling(NavPoint child)
        {
            int i = mChildren.IndexOf(child);
            if (i > 0)
            {
                return mChildren[i - 1];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Remove a child from the list of children. No other modification is made (no update of links, etc.)
        /// </summary>
        /// <param name="child">The child navpoint to remove.</param>
        private void RemoveChild(NavPoint child)
        {
            mChildren.Remove(child);
        }

        /// <summary>
        /// Remove a child node and all its following siblings and return the list of siblings
        /// </summary>
        /// <param name="child">The child navpoint to remove.</param>
        /// <returns>The siblings of the remove navpoint.</returns>
        private List<NavPoint> RemoveChildAndSiblings(NavPoint child)
        {
            int i = mChildren.IndexOf(child);
            List<NavPoint> siblings = mChildren.GetRange(i + 1, mChildren.Count - i - 1);
            mChildren.RemoveRange(i, mChildren.Count - i);
            return siblings;
        }
         */
    }
}
