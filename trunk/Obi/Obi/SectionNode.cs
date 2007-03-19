using System.Xml;

using urakawa.core;
using urakawa.exception;
using urakawa.media;

namespace Obi
{
    /// <summary>
    /// Children of a SectionNode are PhraseNodes or other SectionNodes. PhraseNodes appear first in the list.
    /// </summary>
    public class SectionNode : ObiNode
    {
        private TextMedia mMedia;    // text media for the title of the section
        private int mSectionOffset;  // index of the first section child
        private int mSpan;           // span of this section: 1 + sum of the span of each child section.

        public static readonly string Name = "section";

        /// <summary>
        /// Show info about this node as "{info string} #index@position `label'"
        /// </summary>
        public override string InfoString
        {
            get { return string.Format("{0} #{1}@{2} `{3}'", base.InfoString, Index, Position, Label); }
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
        /// The label of the node is its title.
        /// </summary>
        public string Label
        {
            get { return mMedia.getText(); }
            set
            {
                mMedia.setText(value);
                if (mProject.TextChannel != null)
                    ChannelsProperty.setMedia(mProject.TextChannel, mMedia);
            }
        }

        /// <summary>
        /// Index of this section relative to the other sections.
        /// </summary>
        public override int Index
        {
            get
            {
                CoreNode parent = (CoreNode)getParent();
                int index = parent.indexOf(this);
                return index - (parent is SectionNode ? ((SectionNode)parent).mSectionOffset : 0);
            }
        }

        /// <summary>
        /// Number of phrase children.
        /// </summary>
        public int PhraseChildCount
        {
            get { return mSectionOffset; }
        }

        /// <summary>
        /// Number of section children.
        /// </summary>
        public int SectionChildCount
        {
            get { return getChildCount() - mSectionOffset; }
        }

        /// <summary>
        /// Return the parent as a section node. If the parent is really the root of the tree, return null.
        /// </summary>
        public SectionNode ParentSection
        {
            get
            {
                IBasicTreeNode parent = getParent();
                return parent is SectionNode ? (SectionNode)parent : null;
            }
        }

        /// <summary>
        /// Previous section in "flat" order. If the section is the first child, then the previous is the parent.
        /// </summary>
        public SectionNode PreviousSection
        {
            get
            {
                SectionNode previous = PreviousSibling;
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
        /// Return the previous section sibling node, or null if this is the first child.
        /// </summary>
        public SectionNode PreviousSibling
        {
            get
            {
                CoreNode parent = (CoreNode)getParent();
                int index = parent.indexOf(this);
                int offset = parent is SectionNode ? ((SectionNode)parent).mSectionOffset : 0;
                return index == offset ? null : (SectionNode)parent.getChild(index - 1);
            }
        }

        /// <summary>
        /// Return the next section sibling node, or null if this is the last child.
        /// </summary>
        public SectionNode NextSibling
        {
            get
            {
                CoreNode parent = (CoreNode)getParent();
                int index = parent.indexOf(this);
                return index == parent.getChildCount() - 1 ? null : (SectionNode)parent.getChild(index + 1);
            }
        }

        /// <summary>
        /// Position of this section in the "flat list" of sections. If the section is the first child,
        /// then it is simply the position of its parent + 1. Otherwise, it is the position of its previous
        /// sibling + the span of the previous sibling.
        /// </summary>
        public int Position
        {
            get
            {
                SectionNode sibling = PreviousSibling;
                if (sibling == null)
                {
                    SectionNode parent = ParentSection;
                    return parent == null ? 0 : 1 + parent.Position;
                }
                else
                {
                    return sibling.mSpan + sibling.Position;
                }
            }
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
        /// Get the child phrase at an index relative to phrases only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public PhraseNode PhraseChild(int index)
        {
            if (index < 0) index = PhraseChildCount + index;
            return (PhraseNode)getChild(index);
        }

        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        internal SectionNode(Project project, int id)
            : base(project, id)
        {
            mMedia = (TextMedia)getPresentation().getMediaFactory().createMedia(urakawa.media.MediaType.TEXT);
            Label = Localizer.Message("default_section_label");
            mSectionOffset = 0;
            mSpan = 1;
        }

        /// <summary>
        /// Name of the element in the XUK file for this node.
        /// </summary>
        protected override string getLocalName()
        {
            return Name;
        }

        /// <summary>
        /// When the section is read, update its label with the actual text media data.
        /// </summary>
        public override bool XUKIn(XmlReader source)
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

        /// <summary>
        /// Add a child section at the given index. The span of this section, plus that of parent sections, is increased accordingly.
        /// </summary>
        public void AddChildSection(SectionNode node, int index)
        {
            insert(node, index + mSectionOffset);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Add a new child section right before the context seciont section.
        /// </summary>
        public void AddChildSectionBefore(SectionNode node, SectionNode anchorNode)
        {
            insertBefore(node, anchorNode);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Append a child section.
        /// </summary>
        public void AppendChildSection(SectionNode node)
        {
            base.appendChild(node);
            UpdateSpan(node.mSpan);
        }

        public override void  appendChild(ITreeNode node)
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

        /// <summary>
        /// Update span of current and ancestor sections.
        /// </summary>
        /// <param name="span">The span change (can be negative if nodes are removed.)</param>
        private void UpdateSpan(int span)
        {
            for (IBasicTreeNode parent = this; parent is SectionNode; parent = parent.getParent())
            {
                ((SectionNode)parent).mSpan += span;
            }
        }

        /// <summary>
        /// Add a child phrase at the given index.
        /// </summary>
        public void AddChildPhrase(PhraseNode node, int index)
        {
            insert(node, index);
            ++mSectionOffset;
        }

        public void AppendChildPhrase(PhraseNode node)
        {
            base.appendChild(node);
            ++mSectionOffset;
        }

        /// <summary>
        /// Add a new child phrase after an existing child phrase.
        /// </summary>
        /// <param name="node">The existing child phrase after which to add.</param>
        /// <param name="newNode">The new child phrase to add.</param>
        internal void AddChildPhraseAfter(PhraseNode node, PhraseNode newNode)
        {
            insertAfter(node, newNode);
            ++mSectionOffset;
        }

        /// <summary>
        /// Remove a child phrase.
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChildPhrase(PhraseNode node)
        {
            node.detach();
            --mSectionOffset;
        }

        /// <summary>
        /// Detach this node section from its parent and update the span.
        /// </summary>
        internal SectionNode DetachFromParent()
        {
            if (ParentSection != null) ParentSection.UpdateSpan(-mSpan);
            return (SectionNode)this.detach();
        }

        /// <summary>
        /// Copy a section node.
        /// </summary>
        /// <param name="deep">Flag telling whether to copy descendants as well.</param>
        /// <returns>The copied section node.</returns>
        public new SectionNode copy(bool deep)
        {
            SectionNode copy = (SectionNode)
                getPresentation().getCoreNodeFactory().createNode(Name, ObiPropertyFactory.ObiNS);
            copy.Label = Label;
            copy.Used = Used;
            copyProperties(copy);
            if (deep) CopyChildren(copy);
            return copy;
        }

        /// <summary>
        /// Copy the children of a section node.
        /// </summary>
        /// <param name="destinationNode"></param>
        protected void CopyChildren(SectionNode destinationNode)
        {
            for (int i = 0; i < PhraseChildCount; ++i)
            {
                destinationNode.AddChildPhrase(PhraseChild(i).copy(true), i);
            }
            for (int i = 0; i < SectionChildCount; ++i)
            {
                destinationNode.AddChildSection(SectionChild(i).copy(true), i);
            }
        }
    }
}