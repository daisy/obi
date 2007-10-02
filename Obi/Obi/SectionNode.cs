using System.Xml;
using urakawa.core;
using urakawa.property.channel;
using urakawa.exception;
using urakawa.media;

namespace Obi
{
    /// <summary>
    /// Section nodes are Obi nodes with either section nodes or phrase nodes as their children.
    /// </summary>
    public class SectionNode : ObiNode
    {
        private PhraseNode mHeading;  // section heading
        private int mSectionOffset;   // index of the first section child
        private int mSpan;            // span of this section: 1 + sum of the span of each child section.

        public static readonly string XUK_ELEMENT_NAME = "section";

        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        public SectionNode(Project project): base(project)
        {
            mHeading = null;
            mSectionOffset = 0;
            mSpan = 1;
            // Create the text media object for the label with a default label
            ITextMedia labelMedia = getPresentation().getMediaFactory().createTextMedia();
            labelMedia.setText(Localizer.Message("default_section_label"));
            ChannelsProperty.setMedia(Project.TextChannel, labelMedia);
        }


        /// <summary>
        /// Add a child phrase at the given index.
        /// </summary>
        public void AddChildPhrase(PhraseNode node, int index)
        {
            insert(node, index);
        }

        /// <summary>
        /// Add a child section at the given index.
        /// The span of this section, plus that of parent sections,
        /// is increased accordingly.
        /// </summary>
        public void AddChildSection(SectionNode node, int index)
        {
            insert(node, index + mSectionOffset);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Add a new child section right before the context seciont section.
        /// </summary>
        /// TODO: replace
        public void AddChildSectionBefore(SectionNode node, SectionNode anchorNode)
        {
            insertBefore(node, anchorNode);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Called when a new child phrase was added to maintain bookkeeping information.
        /// </summary>
        /// <param name="phrase">The new child phrase.</param>
        public void AddedPhraseNode(PhraseNode phrase)
        {
            if (phrase.Used) phrase.Used = Used;
            if (phrase.HasXukInHeadingFlag) Heading = phrase;
            ++mSectionOffset;
        }

        /// <summary>
        /// Append a child section.
        /// </summary>
        /// TODO: replace
        public void AppendChildSection(SectionNode node)
        {
            base.appendChild(node);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Copy a section node.
        /// </summary>
        /// <param name="deep">Flag telling whether to copy descendants as well.</param>
        /// <returns>The copy of the section node.</returns>
        public new SectionNode copy(bool deep)
        {
            SectionNode copy = (SectionNode)
                getPresentation().getTreeNodeFactory().createNode(XUK_ELEMENT_NAME, Program.OBI_NS);
            copy.Label = Label;
            copy.Used = Used;
            copyProperties(copy);
            if (deep)
            {
                CopyChildren(copy);
                if (mHeading != null) copy.Heading = (PhraseNode)copy.getChild(indexOf(mHeading));
            }
            else
            {
                copy.Heading = mHeading;
            }
            return copy;
        }

        /// <summary>
        /// Detach this node section from its parent and update the span.
        /// </summary>
        // TODO: replace
        public SectionNode DetachFromParent()
        {
            if (ParentSection != null) ParentSection.UpdateSpan(-mSpan);
            return (SectionNode)this.detach();
        }

        /// <summary>
        /// Find the first used phrase in the section, if any.
        /// Return null if no such phrase exists.
        /// </summary>
        public PhraseNode FirstUsedPhrase
        {
            get
            {
                int i;
                for (i = 0; i < PhraseChildCount && !PhraseChild(i).Used; ++i) { }
                return i < PhraseChildCount && PhraseChild(i).Used ? PhraseChild(i) : null;
            }
        }

        /// <summary>
        /// Name of the element in the XUK file for this node.
        /// </summary>
        public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

        /// <summary>
        /// Get or set the heading phrase for this section.
        /// </summary>
        public PhraseNode Heading
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
                return index - (parent is SectionNode ? ((SectionNode)parent).mSectionOffset : 0);
            }
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
                        SectionNode parent = ParentSection;
                        while (parent != null && sibling == null)
                        {
                            sibling = parent.NextSibling;
                            parent = parent.ParentSection;
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
        /// Return the parent as a section node.
        /// If the parent is really the root of the tree, return null.
        /// </summary>
        public SectionNode ParentSection { get { return getParent() as SectionNode; } }

        /// <summary>
        /// Number of phrase children.
        /// </summary>
        public int PhraseChildCount { get { return mSectionOffset; } }

        /// <summary>
        /// Position of this section in the "flat list" of sections. If the section is the first child,
        /// then it is simply the position of its parent + 1. Otherwise, it is the position of its previous
        /// sibling + the span of the previous sibling.
        /// </summary>
        public int Position
        {
            get
            {
                SectionNode sibling = PrecedingSibling;
                if (sibling != null)
                {
                    return sibling.Position + sibling.mSpan;
                }
                else if (this.getParent() is SectionNode)
                {
                    //this is the first section child of a section, so its position is 1 greater that the parent's
                    return 1 + ParentSection.Position;
                }
                else
                {
                    // this is the first node (no previous sibling, parent is not a section node)
                    return 0;
                }
            }
        }

        /// <summary>
        /// Get the child phrase at an index relative to phrases only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public PhraseNode PhraseChild(int index)
        {
            if (index < 0) index = PhraseChildCount + index;
            return (PhraseNode)getChild(index);
        }

        /// <summary>
        /// Previous section in "flat" order. If the section is the first child, then the previous is the parent.
        /// </summary>
        public SectionNode PreviousSection
        {
            get
            {
                SectionNode previous = PrecedingSibling;
                if (previous == null)
                {
                    previous = ParentSection;
                }
                else
                {
                    while (previous.SectionChildCount > 0) previous = previous.SectionChild(-1);
                }
                return previous;
            }
        }

        /// <summary>
        /// Return the previous section sibling node, or null if this is the first child.
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
        /// Remove a child phrase.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChildPhrase(PhraseNode node)
        {
            if (node == mHeading) mHeading = null;
            node.detach();
            --mSectionOffset;
        }

        /// <summary>
        /// Get the child section at an index relative to sections only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public SectionNode SectionChild(int index)
        {
            if (index < 0) index = SectionChildCount + index;
            return (SectionNode)getChild(index + mSectionOffset);
        }

        /// <summary>
        /// Number of section children.
        /// </summary>
        public int SectionChildCount { get { return getChildCount() - mSectionOffset; } }



        private void AfterAddingPhrase(PhraseNode node)
        {
            throw new System.Exception("Don't!!!");
        }

        /// <summary>
        /// Copy the children of a section node.
        /// </summary>
        protected void CopyChildren(SectionNode destinationNode)
        {
            for (int i = 0; i < PhraseChildCount; ++i)
            {
                destinationNode.insert(PhraseChild(i).copy(true), i);
            }
            for (int i = 0; i < SectionChildCount; ++i)
            {
                destinationNode.AddChildSection(SectionChild(i).copy(true), i);
            }
        }

        /// <summary>
        /// Get the text media object for the label.
        /// </summary>
        private TextMedia LabelTextMedia
        {
            get { return (TextMedia)ChannelsProperty.getMedia(Project.TextChannel); }
        }

        /// <summary>
        /// Update span of current and ancestor sections.
        /// </summary>
        /// <param name="span">The span change (can be negative if nodes are removed.)</param>
        private void UpdateSpan(int span)
        {
            for (TreeNode parent = this; parent is SectionNode; parent = parent.getParent())
            {
                ((SectionNode)parent).mSpan += span;
            }
        }


        /// <summary>
        /// When the section is read, update its label with the actual text media data.
        /// </summary>
        // TODO: replace this by???
        /*
        public override void XUKIn(XmlReader source)
        {
            if (base.XUKIn(source))
            {
                mMedia = (TextMedia)ChannelsProperty.getMedia(mProject.TextChannel);
                return true;
            }
            else
            {
                return false;
            }
        }
        */

        /*
        public override void  appendChild(TreeNode node)
        {
            if (node is SectionNode)
            {
                AppendChildSection((SectionNode)node);
            }
            else if (node is PhraseNode)
            {
                AppendChildPhrase((PhraseNode)node);
            }
        }
        */

    }
}