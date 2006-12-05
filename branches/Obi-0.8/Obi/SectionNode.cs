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
        public static readonly string Name = "section";

        private ChannelsProperty mChannel;  // quick reference to the channel property
        private TextMedia mMedia;           // quick reference to the text media object
        private string mLabel;              // string version of the text
        private int mSectionOffset;         // index of the first section child
        private int mSpan;                  // span of this section: 1 + sum of the span of each child section.

        public override string InfoString
        {
            get { return string.Format("{0} #{1}@{2} `{3}'", base.InfoString, Index, Position, mLabel); }
        }

        /// <summary>
        /// Get/set the label of the node as a simple string.
        /// </summary>
        public string Label
        {
            get { return mLabel; }
            set
            {
                mLabel = value;
                mMedia.setText(value);
                mChannel.setMedia(Project.mTextChannel, mMedia);
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
        /// Return the previous section sibling node, or null if this is the first. 
        /// </summary>
        public SectionNode PreviousSibling
        {
            get
            {
                CoreNode parent = (CoreNode)getParent();
                int index = parent.indexOf(this);
                if (parent is SectionNode)
                {
                    return index == ((SectionNode)parent).mSectionOffset ? null : (SectionNode)parent.getChild(index - 1);
                }
                else
                {
                    return index == 0 ? null : (SectionNode)parent.getChild(index - 1);
                }
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
        /// </summary>
        public SectionNode SectionChild(int index)
        {
            CoreNode test = getChild(index + mSectionOffset);
            return (SectionNode)test;
           // return (SectionNode)getChild(index + mSectionOffset);
        }

        /// <summary>
        /// Get the child phrase at an index relative to phrases only.
        /// </summary>
        public PhraseNode PhraseChild(int index)
        {
            return (PhraseNode)getChild(index);
        }

        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        internal SectionNode(Project project, int id)
            : base(project, id)
        {
            mChannel = getPresentation().getPropertyFactory().createChannelsProperty();
            this.setProperty(mChannel);
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
                ChannelsProperty prop = (ChannelsProperty)getProperty(typeof(ChannelsProperty));
                Label = ((TextMedia)prop.getMedia(Project.mTextChannel)).getText();
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
        /// Add a new child section right after the context seciont section.
        /// </summary>
        public void AddChildSectionAfter(SectionNode node, SectionNode anchorNode)
        {
            insertAfter(node, anchorNode);
            UpdateSpan(node.mSpan);
        }

        /// <summary>
        /// Append a child section.
        /// </summary>
        public void AppendChildSection(SectionNode node)
        {
            appendChild(node);
            UpdateSpan(node.mSpan);
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
            node.DetachFromParent();
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
            if (deep) copyChildren(copy);
            return copy;
        }
        protected void copyChildren(SectionNode destinationNode)
        {
            for (int i = 0; i < this.getChildCount(); i++)
            {
                if (getChild(i).GetType() == System.Type.GetType("Obi.SectionNode"))
                {
                    SectionNode copy = ((SectionNode)getChild(i)).copy(true);
                    destinationNode.AppendChildSection(copy);
                }
                else if (getChild(i).GetType() == System.Type.GetType("Obi.PhraseNode"))
                {
                    PhraseNode copy = ((PhraseNode)getChild(i)).copy(true);
                    destinationNode.AddChildPhrase(copy, i);
                }
            }
        }

    }
}