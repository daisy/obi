using System.Xml;
using urakawa.core;
using urakawa.property.channel;
using urakawa.exception;
using urakawa.media;

namespace Obi
{
    /// <summary>
    /// Section nodes are Obi nodes with either section nodes or phrase nodes as their children.
    /// All phrase children come first, then the section children.
    /// </summary>
    public class SectionNode : ObiNode
    {
        private EmptyNode mHeading;  // section heading

        public static readonly string XUK_ELEMENT_NAME = "section";  // element name in XUK output


        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        public SectionNode(Presentation presentation): base(presentation)
        {
            mHeading = null;
        }


        /// <summary>
        /// Append a child node (of any kind) at the right position.
        /// </summary>
        public override void AppendChild(ObiNode node)
        {
            int index = node is EmptyNode ? FirstSectionIndex : getChildCount();
            insert(node, index);
        }

        protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            SectionNode copy = (SectionNode)base.copyProtected(deep, inclProperties);
            // Even when doing a shallow copy, we still should copy children (!)
            if (!deep)
            {
                for (int i = 0; i < PhraseChildCount; ++i) copy.AppendChild((EmptyNode)PhraseChild(i).copy(true, true));
            }
            return copy;
        }

        /// <summary>
        /// Copy a section node (and its contents if the deep flag is set.)
        /// </summary>
        /*protected override TreeNode copyProtected(bool deep, bool inclProperties)
        {
            SectionNode copy = Presentation.CreateSectionNode();
            copy.Label = Label;
            copy.Used = Used;
            if (deep)
            {
                CopyChildren(copy);
                if (mHeading != null) copy.Heading = (PhraseNode)copy.getChild(indexOf(mHeading));
            }
            else
            {
                copy.mHeading = mHeading;
            }
            return copy;
        }*/

        /// <summary>
        /// Find the first used phrase in the section, if any.
        /// Return null if no such phrase exists.
        /// </summary>
        public override PhraseNode FirstUsedPhrase
        {
            get
            {
                PhraseNode first = null;
                for (int i = 0; i < PhraseChildCount && first == null; ++i) first = PhraseChild(i).FirstUsedPhrase;
                return first;
            }
        }

        /// <summary>
        /// Name of the element in the XUK file for this node.
        /// </summary>
        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        /// <summary>
        /// Get or set the heading phrase for this section.
        /// </summary>
        public EmptyNode Heading
        {
            get { return mHeading; }
            set { mHeading = value; }
        }

        /// <summary>
        /// Index of this section relative to the other sections.
        /// </summary>
        public override int Index
        {
            get
            {
                TreeNode parent = getParent();
                int index = parent.indexOf(this);
                return index - (parent is SectionNode ? ParentAs<SectionNode>().FirstSectionIndex : 0);
            }
        }

        /// <summary>
        /// Insert a node at the given index.
        /// The index is interpreted relatively to the position of the other phrases or sections.
        /// If the index is negative, count backward from the end (i.e. -1 inserts in the last position)
        /// </summary>
        public override void Insert(ObiNode node, int index)
        {
            index = node is EmptyNode ? index < 0 ? FirstSectionIndex + index : index :
                                        index < 0 ? getChildCount() + index : index + FirstSectionIndex;
            insert(node, index);
        }

        /// <summary>
        /// The label of the node is its title.
        /// </summary>
        public string Label
        {
            get { return LabelTextMedia.getText(); }
            set { LabelTextMedia.setText(value); }
        }

        /// <summary>
        /// Get the last subsection from this section.
        /// </summary>
        public override ObiNode LastDescendant
        {
            get
            {
                int n = SectionChildCount;
                return n == 0 ? this : SectionChild(n - 1).LastDescendant;
            }
        }

        /// <summary>
        /// Next section in flat order: first child if there is a child, next sibling, or parent's sibling, etc.
        /// </summary>
        public SectionNode NextSection
        {
            get
            {
                if (SectionChildCount > 0)
                {
                    return SectionChild(0);
                }
                else
                {
                    SectionNode sibling = NextSibling;
                    if (sibling != null)
                    {
                        return sibling;
                    }
                    else
                    {
                        SectionNode parent = ParentAs<SectionNode>();
                        while (parent != null && sibling == null)
                        {
                            sibling = parent.NextSibling;
                            parent = parent.ParentAs<SectionNode>();
                        }
                        return sibling;
                    }
                }
            }
        }

        /// <summary>
        /// Return the next section sibling node, or null if this is the last child.
        /// </summary>
        public SectionNode NextSibling
        {
            get
            {
                TreeNode parent = getParent();
                int index = parent.indexOf(this);
                return index == parent.getChildCount() - 1 ? null : (SectionNode)parent.getChild(index + 1);
            }
        }

        /// <summary>
        /// Get the child phrase at an index relative to phrases only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public override EmptyNode PhraseChild(int index)
        {
            if (index < 0) index = PhraseChildCount + index;
            return (EmptyNode)getChild(index);
        }

        /// <summary>
        /// Number of phrase children.
        /// </summary>
        public override int PhraseChildCount { get { return FirstSectionIndex; } }

        /// <summary>
        /// Position when view as a flat list.
        /// </summary>
        public int Position
        {
            get
            {
                return Index > 0 ? PrecedingSibling.Position + PrecedingSibling.Span :
                    ParentAs<SectionNode>() == null ? 0 : 1 + ParentAs<SectionNode>().Position;
            }
        }

        /// <summary>
        /// The preceding section sibling node, or null if this is the first child.
        /// </summary>
        public SectionNode PrecedingSibling
        {
            get
            {
                TreeNode parent = getParent();
                int index = parent.indexOf(this);
                return index > 0 && parent.getChild(index - 1) is SectionNode ? (SectionNode)parent.getChild(index - 1) : null;
            }
        }

        /// <summary>
        /// The preceding section in "flat" order. If the section is the first child, then the preceding one is the parent.
        /// </summary>
        public SectionNode PrecedingSection
        {
            get
            {
                SectionNode previous = PrecedingSibling;
                if (previous == null)
                {
                    previous = ParentAs<SectionNode>();
                }
                else
                {
                    while (previous.SectionChildCount > 0) previous = previous.SectionChild(-1);
                }
                return previous;
            }
        }

        /// <summary>
        /// Get the child section at an index relative to sections only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public override SectionNode SectionChild(int index)
        {
            if (index < 0) index = getChildCount() + index;
            return (SectionNode)getChild(index + FirstSectionIndex);
        }

        /// <summary>
        /// Number of section children.
        /// </summary>
        public override int SectionChildCount { get { return getChildCount() - FirstSectionIndex; } }

        /// <summary>
        /// String output to help with debugging.
        /// </summary>
        public override string ToString()
        {
            return string.Format("SectionNode<{0}>\"{1}\"", IsRooted ? Level.ToString() : "unrooted", Label);
        }


        /// <summary>
        /// Copy the children of a section node.
        /// </summary>
        protected void CopyChildren(SectionNode destinationNode)
        {
            for (int i = 0; i < PhraseChildCount; ++i)
            {
                destinationNode.Insert((EmptyNode)PhraseChild(i).copy(true), i);
            }
            for (int i = 0; i < SectionChildCount; ++i)
            {
                destinationNode.Insert((SectionNode)SectionChild(i).copy(true, true), i);
            }
        }

        // The index of the first section in the list of children. 
        // If there is no section this is simply the number of children (i.e. where the first phrase would be.)
        private int FirstSectionIndex
        {
            get
            {
                for (int i = 0; i < getChildCount(); ++i) if (getChild(i) is SectionNode) return i;
                return getChildCount();
            }
        }

        // The text media object for the label.
        private TextMedia LabelTextMedia
        {
            get { return (TextMedia)getProperty<ChannelsProperty>().getMedia(Presentation.TextChannel); }
        }

        // Span of the section in number of nodes. If there are no subsections, the span is 1;
        // otherwise it is 1 + the sum of the spans of all subsections.
        private int Span
        {
            get
            {
                int span = 1;
                for (int i = 0; i < SectionChildCount; ++i) span += SectionChild(i).Span;
                return span;
            }
        }
    }
}