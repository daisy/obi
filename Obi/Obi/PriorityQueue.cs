using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class PriorityQueue<T> where T: IComparable
    {
        private List<T> mItems;  // items in the queue, organized in a binary heap.


        /// <summary>
        /// Create an empty max priority queue (highest priority item comes first.)
        /// </summary>
        public PriorityQueue()
        {
            mItems = new List<T>();
        }


        /// <summary>
        /// Clear the queue completely.
        /// </summary>
        public void Clear() { mItems.Clear(); }

        /// <summary>
        /// Get the number of items in the queue.
        /// </summary>
        public int Count { get { return mItems.Count; } }

        /// <summary>
        /// Get the item with the highest priority from the queue.
        /// </summary>
        public T Dequeue()
        {
            if (mItems.Count == 0) throw new Exception("Cannot dequeue from an empty queue!");
            T top = mItems[0];
            if (mItems.Count == 1)
            {
                // The heap becomes empty.
                mItems.RemoveAt(0);
            }
            else
            {
                // Get the first (i.e. max) element out of the heap
                // and replace it with the bottom element of the heap,
                // which is then bubbled down to its correct location.
                mItems[0] = mItems[mItems.Count - 1];
                mItems.RemoveAt(mItems.Count - 1);
                BubbleDown(0);
            }
            return top;
        }

        /// <summary>
        /// Add a new item at the right place in the queue depending on its priority.
        /// TODO: If the item was already present, its priority may be adjusted.
        /// Return true when the item was actually added. (Right now, always true.)
        /// </summary>
        public bool Enqueued(T item)
        {
            mItems.Add(item);
            BubbleUp(mItems.Count - 1);
            return true;
        }

        /// <summary>
        /// Peek at the front of the queue.
        /// </summary>
        public T Peek()
        {
            if (mItems.Count == 0) throw new Exception("Cannot peek into an empty queue!");
            return mItems[0];
        }

        public override string ToString()
        {
            string str = "{";
            if (mItems.Count > 0)
            {
                str += mItems[0].ToString();
                for (int i = 1; i < mItems.Count; ++i) str += ", " + mItems[i];
            }
            return str + "}";
        }


        // Element at index `from' will bubble down until it has no child or it a higher
        // priority than both children.
        private void BubbleDown(int from)
        {
            for (int i = from; i < mItems.Count;)
            {
                // Children of element at index i are at indices 2 * i + 1 and 2 * i + 2
                int c = 2 * i + 1;
                // Note that the comparer should return a value higher than 0 when comparing
                // with a null object.
                T child1 = c < mItems.Count ? mItems[c] : default(T);
                T child2 = c + 1 < mItems.Count ? mItems[c + 1] : default(T);
                if (mItems[i].CompareTo(child1) >= 0)
                {
                    if (mItems[i].CompareTo(child2) >= 0)
                    {
                        // Current element has a higher priority than both children
                        // so it's in its correct location.
                        break;
                    }
                    else
                    {
                        // Child #2 has a higher priority than both the current element
                        // and child #1.
                        SwapItems(i, c + 1);
                        i = c + 1;
                    }
                }
                else
                {
                    if (mItems[i].CompareTo(child2) >= 0 || child1.CompareTo(child2) >= 0)
                    {
                        // Child #1 has a higher priority than the current element
                        // and child #2.
                        SwapItems(i, c);
                        i = c;
                    }
                    else
                    {
                        // Child #2 has a higher priority than both the current element
                        // and child #1.
                        SwapItems(i, c + 1);
                        i = c + 1;
                    }
                }
            }
        }

        // Element at index `from' will bubble up until it reaches a parent node with a higher
        // priority or the top of the heap. Parent of element at index i is at index
        // (i - 1) / 2.
        private void BubbleUp(int from)
        {
            for (int i = from, p = (i - 1) / 2; p > 0 && mItems[i].CompareTo(mItems[p]) > 0; i = p, p = (p - 1) / 2)
            {
                SwapItems(i, p);
            }
        }

        // Swap the two items at indices i and j.
        private void SwapItems(int i, int j)
        {
            T t = mItems[i];
            mItems[i] = mItems[j];
            mItems[j] = t;
        }
    }
}
