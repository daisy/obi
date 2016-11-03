using System.Xml;
using System.Collections.Generic ;
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
    [XukNameUglyPrettyAttribute("section", "section")]
    public class SectionNode : ObiNode
    {
        //public new static string XukString = "section";
        //public override string GetTypeNameFormatted()
        //{
        //    return XukString;
        //}

        public override string XmlPropertyString { get { return "level"; } } 
        private EmptyNode mHeading = null;  // section heading


        /// <summary>
        /// Create a new section node with the default label.
        /// </summary>
        /// //sdk2
        //public SectionNode(Presentation presentation): base(presentation)
        //{
        //    mHeading = null;
        //}

        public SectionNode() : base()
        {
            ;            
        }

        /// <summary>
        /// Append a child node (of any kind) at the right position.
        /// </summary>
        public override void AppendChild(ObiNode node)
        {
            int index = node is EmptyNode ? FirstSectionIndex : Children.Count;
            ((TreeNode)this).Insert(node, index);
        }

        /// <summary>
        /// Test whether this section's level can be increased (i.e. not the first child of its parent.)
        /// </summary>
        public bool CanIncreaseLevel { get { return Index > 0; } }

        /// <summary>
        /// Copy the children of a section node.
        /// </summary>
        protected void CopyChildren(SectionNode destinationNode)
        {
            for (int i = 0; i < PhraseChildCount; ++i)
            {
                destinationNode.Insert((EmptyNode)PhraseChild(i).Copy(true), i);
            }
            for (int i = 0; i < SectionChildCount; ++i)
            {
                destinationNode.Insert((SectionNode)SectionChild(i).Copy(true, true), i);
            }
        }

        // Copy the section and its contents (shallow)
        protected override TreeNode CopyProtected(bool deep, bool inclProperties)
        {
            SectionNode copy = (SectionNode)base.CopyProtected(deep, inclProperties);
            // Even when doing a shallow copy, we still should copy children (!)
            if (!deep)
            {
                for (int i = 0; i < PhraseChildCount; ++i) copy.AppendChild((EmptyNode)PhraseChild(i).Copy(true, true));
            }
            return copy;
        }

        /// <summary>
        /// Set the heading to be the given node, if it has audio and there is no heading already.
        /// Return whether the node was actually suitable as heading.
        /// </summary>
        public bool DidSetHeading(EmptyNode node)
        {
            if (node is PhraseNode && mHeading == null)
            {
                mHeading = node;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Total duration of the section.
        /// </summary>
        public override double Duration
        {
            get
            {
                double duration = 0;
                for (int i = 0; i < PhraseChildCount; ++i) duration += PhraseChild(i).Duration;
                return duration;
            }
        }

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
        /// Get or set the heading phrase for this section.
        /// </summary>
        public EmptyNode Heading
        {
            get { return mHeading; }
        }

        /// <summary>
        /// Index of this section relative to the other sections.
        /// </summary>
        public override int Index
        {
            get
            {
                TreeNode parent = Parent;
                int index = parent.Children.IndexOf(this);
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
                index < 0 ? Children.Count + index : index + FirstSectionIndex ;
            ((TreeNode)this).Insert(node, index);
            if (node is PhraseNode && ((PhraseNode)node).Role_ == EmptyNode.Role.Heading) DidSetHeading((PhraseNode)node);
        }

        /// <summary>
        /// The label of the node is its title.
        /// </summary>
        public string Label
        {
            get { return LabelTextMedia.Text; }//sdk2
            set { LabelTextMedia.Text= value; }
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
        /// Last used phrase of the section.
        /// </summary>
        public override EmptyNode LastUsedPhrase
        {
            get
            {
                int i;
                for (i = PhraseChildCount - 1; i >= 0 && !Used && !PhraseChild(i).Used; --i) { }
                return i >= 0 ? PhraseChild(i) : null;
            }
        }

        /// <summary>
        /// Following section in flat order: first child if there is a child, next sibling, or parent's sibling, etc.
        /// </summary>
        public SectionNode FollowingSection
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
        public new SectionNode NextSibling
        {
            get
            {
                TreeNode parent = Parent;
                int index = parent.Children.IndexOf(this);
                return index == parent.Children.Count - 1 ? null : (SectionNode)parent.Children.Get(index + 1);
            }
        }

        /// <summary>
        /// Get the child phrase at an index relative to phrases only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public override EmptyNode PhraseChild(int index)
        {
            if (index < 0) index = PhraseChildCount + index;
            return (EmptyNode)Children.Get(index);
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
                TreeNode parent = Parent;
                int index = parent.Children.IndexOf (this);
                return index > 0 && parent.Children.Get(index - 1) is SectionNode ? (SectionNode)parent.Children.Get(index - 1) : null;
            }
        }

        /// <summary>
        /// The preceding section in "flat" order: the parent for the first child,
        /// or the last descendant of the previous sibling.
        /// </summary>
        public SectionNode PrecedingSection
        {
            get
            {
                SectionNode previous = PrecedingSibling;
                return previous == null ? ParentAs<SectionNode>() : (SectionNode)previous.LastDescendant;
            }
        }

        /// <summary>
        /// Get the child section at an index relative to sections only.
        /// If the index is negative, start from the end of the list.
        /// </summary>
        public override SectionNode SectionChild(int index)
        {
            if (index < 0) index = Children.Count + index;//sdk2
            return (SectionNode)Children.Get(index + FirstSectionIndex);
        }

        /// <summary>
        /// Number of section children.
        /// </summary>
        public override int SectionChildCount { get { return Children.Count - FirstSectionIndex; } }

        /// <summary>
        /// String output to help with debugging.
        /// </summary>
        public override string ToString()
        {
            string tempLabel = Label.Replace("\n", string.Empty);
            return string.Format(Localizer.Message("section_to_string"),
                Used ? "" : Localizer.Message("unused"),
                tempLabel,
                Duration == 0.0 ? Localizer.Message("empty") : string.Format(Localizer.Message("duration_s_ms"), Duration / 1000.0),
                string.Format(Localizer.Message("section_level_to_string"), IsRooted ? Level : 0),
                PhraseChildCount == 0 ? "" :
                    PhraseChildCount == 1 ? Localizer.Message("section_one_phrase_to_string") :
                        string.Format(Localizer.Message("section_phrases_to_string"), PhraseChildCount));
        }

        /// <summary>
        /// Unset the heading node, if it was indeed this one.
        /// </summary>
        public void UnsetHeading(PhraseNode node)
        {
            if (node == mHeading) mHeading = null;
        }


        // The index of the first section in the list of children. 
        // If there is no section this is simply the number of children (i.e. where the first phrase would be.)
        private int FirstSectionIndex
        {
            get
            {
                for (int i = 0; i < Children.Count; ++i) if (Children.Get(i) is SectionNode) return i;
                return Children.Count;
            }
        }

        // The text media object for the label.
        private TextMedia LabelTextMedia
        {
            //get { return (TextMedia)getProperty<ChannelsProperty>().getMedia(Presentation.TextChannel); }
            get { return (TextMedia)this.GetOrCreateProperty <ChannelsProperty>().GetMedia(Presentation.ChannelsManager.GetOrCreateTextChannel () ); }//sdk2
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


        public List<SectionNode> GetAllChildSections()
        {
            List<SectionNode> sectionsList = new List<SectionNode>();
            TraverseAndCollectAllSubSections(this, sectionsList);
            System.Console.WriteLine("Count of child sections: " + sectionsList.Count);
            return sectionsList;
        }

        private void TraverseAndCollectAllSubSections(SectionNode node, List<SectionNode> sectionsList)
        {
            if (node.SectionChildCount == 0)
            {
                return;
            }
            else
            {
                for (int i = 0; i < node.SectionChildCount; i++)
                {
                    SectionNode n = node.SectionChild(i);
                    sectionsList.Add(n);
                    TraverseAndCollectAllSubSections(n, sectionsList);
                }
            }
        }


    }
}