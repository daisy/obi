using System;
using System.Collections.Generic;

using urakawa.command;
using urakawa.core;
using urakawa.exception;
using urakawa.property.channel;
using urakawa.property.xml;
using urakawa.progress;

namespace Obi
{
    /// <summary>
    /// Base class for nodes in the Obi tree; doesn't do much.
    /// All nodes in the tree will be of a derived class.
    /// </summary>
    public abstract class ObiNode : TreeNode
    {
        private bool mUsed;  // mark node as being in use or not

        /// <summary>
        /// Create a new node for a presentation.
        /// </summary>
        protected ObiNode(Presentation presentation) : base()
        {
            setPresentation(presentation);
            mUsed = true;
        }

        /// <summary>
        /// Get the nearest ancestor of the given type. This is useful to get the section ancestor of a phrase
        /// regardless of its nesting level.
        /// </summary>
        public T AncestorAs<T>() where T : ObiNode
        {
            ObiNode parent = getParent() as ObiNode;
            return parent == null || parent is T ? parent as T : parent.AncestorAs<T>();
        }

        /// <summary>
        /// Get the preceding node in the tree.
        /// </summary>
        public virtual ObiNode PrecedingNode
        {
            get { return getParent().indexOf(this) > 0 ? ((ObiNode)getPreviousSibling()).LastLeaf : ParentAs<ObiNode>(); }
        }

        /// <summary>
        /// Get the last used phrase child of the node. If the node itself is unused, just get the last phrase.
        /// If there is no such phrase, return null.
        /// </summary>
        public abstract EmptyNode LastUsedPhrase { get; }

        /// <summary>
        /// Last leaf from a given node. In this case, we don't distinguish between phrase and section nodes.
        /// </summary>
        public ObiNode LastLeaf
        {
            get
            {
                int children = getChildCount();
                return children == 0 ? this : ((ObiNode)getChild(children - 1)).LastLeaf;
            }
        }

        /// <summary>
        /// Get the following node in the tree.
        /// </summary>
        public virtual ObiNode FollowingNode
        {
            get
            { 
                return getParent().indexOf(this) < getParent().getChildCount() - 1 ?
                    ((ObiNode)getNextSibling()).FirstLeaf : 
                    ((ObiNode)getParent()).FollowingNode;
            }
        }

        /// <summary>
        /// Following node in document order: section, then its phrase children, then its section children,
        /// then its siblings, etc.
        /// </summary>
        public virtual ObiNode FollowingNodeAfter(int index_after)
        {
            ObiNode parent = ParentAs<ObiNode>();
            if (parent != null)
            {
                int index_self = parent.indexOf(this);
                return getChildCount() > index_after + 1 ? (ObiNode)getChild(index_after + 1) :
                    index_self < getParent().getChildCount() - 1 ? (ObiNode)getParent().getChild(index_self + 1) :
                    parent.FollowingNodeAfter(index_self);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Get a command to renumber this node and all following nodes from this number.
        /// </summary>
        public virtual CompositeCommand RenumberCommand(ProjectView.ProjectView view, PageNumber from)
        {
            ObiNode n = FollowingNode;
            return n == null ? null : n.RenumberCommand(view, from);
        }

        /// <summary>
        /// First leaf from a given node.
        /// </summary>
        public ObiNode FirstLeaf
        {
            get
            {
                int childrent = getChildCount();
                return childrent == 0 ? this : ((ObiNode)getChild(0)).FirstLeaf;
            }
        }

        /// <summary>
        /// Index of this node in its parent's list of children.
        /// </summary>
        public virtual int Index { get { return getParent().indexOf(this); } }

        /// <summary>
        /// Check whether the node is rooted in the tree or not.
        /// </summary>
        public bool IsRooted
        {
            get { return this is RootNode || (getParent() is ObiNode && ((ObiNode)getParent()).IsRooted); }
        }

        /// <summary>
        /// Last descendant of the same kind as this node.
        /// </summary>
        public virtual ObiNode LastDescendant { get { return this; } }

        /// <summary>
        /// Level of the node in the tree. It is assumed that the root is an ObiNode with a level of 0.
        /// </summary>
        public virtual int Level { get { return 1 + ((ObiNode)getParent()).Level; } }

        /// <summary>
        /// Get the parent node as an ObiNode or any subclass thereof. I am not sure what "thereof" means, though.
        /// </summary>
        public T ParentAs<T>() where T : ObiNode { return getParent() as T; }


        // Our own overrides

        public virtual void AppendChild(ObiNode node) { appendChild(node); }
        public virtual ObiNode Detach() { return (ObiNode)detach(); }
        public abstract void Insert(ObiNode node, int index);
        public void InsertAfter(ObiNode node, ObiNode anchor) { insertAfter(node, anchor); }
        public void InsertAfterSelf(ObiNode node) { getParent().insertAfter(node, this); }
        public void InsertBefore(ObiNode node, ObiNode anchor) { insertBefore(node, anchor); }
        public void RemoveChild(ObiNode child) { removeChild(child); }

        /// <summary>
        /// Presentation to which this node belongs.
        /// </summary>
        public Presentation Presentation { get { return (Presentation)getPresentation(); } }

        public abstract SectionNode SectionChild(int index);
        public abstract int SectionChildCount { get; }
        public abstract EmptyNode PhraseChild(int index);
        public abstract int PhraseChildCount { get; }
        public abstract double Duration { get; }

        public abstract PhraseNode FirstUsedPhrase { get; }

        /// <summary>
        /// Used flag.
        /// </summary>
        public bool Used
        {
            get { return mUsed; }
            set
            {
                if (mUsed != value)
                {
                    mUsed = value;
                    Presentation.SignalUsedStatusChanged(this);
                }
            }
        }

        // XUK stuff
        private static readonly string USED_ATTR_NAME = "used";  // name of the used attribute

        protected override void XukInChild(System.Xml.XmlReader source, IProgressHandler handler)
        {
            base.xukInChild(source, handler);
            XukInNodeProperties();
        }


        protected virtual void XukInNodeProperties()
        {
            XmlProperty xmlProp = this.getProperty<XmlProperty>();
            if (xmlProp != null)
            {
                XmlAttribute attrUsed = xmlProp.getAttribute(USED_ATTR_NAME, xmlProp.getNamespaceUri());
                string used =attrUsed != null?  attrUsed.getValue(): null;
                if (used != null && used == "False") mUsed = false;
            }
        }

        
        /// <summary>
        /// Read back the used attribute.
        /// </summary>
        protected override void XukInAttributes(System.Xml.XmlReader reader)
        {
            if (reader.AttributeCount > 1 || (reader.AttributeCount == 1 && reader.GetAttribute("xmlns") == null))
            {
                string used = reader.GetAttribute(USED_ATTR_NAME);
                if (used != null && used == "False") mUsed = false;
            }
            base.xukInAttributes(reader);
        }

        protected override void XukOutChildren(System.Xml.XmlWriter destination, Uri baseUri, IProgressHandler handler)
        {
            if (Presentation.UseXukFormat) UpdateXmlProperties();
            base.xukOutChildren(destination, baseUri, handler);
        }


        protected virtual void UpdateXmlProperties()
        {
            XmlProperty xmlProp = ObiNodeGetOrCreateXmlProperty();
            UpdateAttributesInXmlProperty(xmlProp, USED_ATTR_NAME, this.Used.ToString () );
        }

        protected XmlProperty ObiNodeGetOrCreateXmlProperty()
        {
            XmlProperty xmlProp = this.getProperty<urakawa.property.xml.XmlProperty>();
            if (xmlProp == null)
            {
                xmlProp = Presentation.getPropertyFactory().createXmlProperty();
                this.addProperty(xmlProp);
                xmlProp.setQName(this.getXukLocalName(), this.getXukNamespaceUri());
            }
            return xmlProp;
        }

        protected void UpdateAttributesInXmlProperty(XmlProperty xmlProp, string attributeLocalName, string attributeValue)
        {
            XmlAttribute attr = xmlProp.getAttribute(attributeLocalName, xmlProp.getNamespaceUri());
            if (attr == null)
            {
                xmlProp.setAttribute(attributeLocalName, xmlProp.getNamespaceUri(), attributeValue);
            }
            else
            {
                attr.setValue(attributeValue);
            }
        }

        /// <summary>
        /// Write the used attribute if its value is false (true being the default.)
        /// </summary>
        protected override void XukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
        {
            if (!Presentation.UseXukFormat)
            {
                if (!mUsed) destination.WriteAttributeString(USED_ATTR_NAME, "False");
            }
                base.xukOutAttributes(destination, baseUri);
            
        }

        /// <summary>
        /// Return the correct namespace URI for all Obi nodes.
        /// </summary>
        //public override string XukNamespaceUri { get { return DataModelFactory.NS;
        public new string XUK_NS = DataModelFactory.NS;//sdk2

        /// <summary>
        /// Copy the used flag as well as properties.
        /// </summary>
        protected virtual TreeNode copyProtected(bool deep, bool inclProperties)
        {
            ObiNode copy = (ObiNode)base.copyProtected(deep, inclProperties);
            copy.mUsed = mUsed;
            return copy;
        }

        /// <summary>
        /// Test whether this node is before some other node in the project.
        /// </summary>
        public bool IsBeforeInProject(PhraseNode other)
        {
            SectionNode parent = ParentAs<SectionNode>();
            SectionNode otherParent = other.ParentAs<SectionNode>();
            return parent.Position < otherParent.Position || (parent == otherParent && Index < other.Index);
        }
    }

    /// <summary>
    /// The root node of a presentation is an ObiNode as well.
    /// The level of the root node is 0. A root node has only section children.
    /// </summary>
    // sdk2
    //public class RootNode : ObiNode
    //{
    //    public static readonly string XUK_ELEMENT_NAME = "root";  // name of the element in the XUK file

    //    private string mPrimaryExportDirectory= "" ;

    //    public RootNode(Presentation presentation) : base(presentation) { }

    //    /// <summary>
    //    /// Allow only section nodes to be inserted.
    //    /// If the index is negative, count backward from the end (-1 is last.)
    //    /// </summary>
    //    public override void Insert(ObiNode node, int index)
    //    {
    //        if (!(node is SectionNode)) throw new Exception("Only section nodes can be added as children of a phrase node.");
    //        if (index < 0) index += getChildCount();
    //        insert(node, index);
    //    }

    //    /// <summary>
    //    /// The level of the root node is always 0; top-level sections have a level of 1.
    //    /// </summary>
    //    public override int Level { get { return 0; } }

    //    public override SectionNode SectionChild(int index)
    //    {
    //        if (index < 0) index = getChildCount() - index;
    //        return (SectionNode)getChild(index);
    //    }

    //    public override ObiNode PrecedingNode { get { return null; } }
    //    public override ObiNode FollowingNode { get { return null; } }

    //    public override PhraseNode FirstUsedPhrase { get { throw new Exception("A root node has no phrase children."); } }
    //    public override EmptyNode LastUsedPhrase { get { throw new Exception("A root node has no phrase children."); } }
    //    public override int SectionChildCount { get { return getChildCount(); } }
    //    public override EmptyNode PhraseChild(int index) { throw new Exception("A root node has no phrase children."); }
    //    public override int PhraseChildCount { get { return 0; } }

    //    public override double Duration
    //    {
    //        get
    //        {
    //            double duration = 0.0;
    //            acceptDepthFirst(delegate(urakawa.core.TreeNode n)
    //            {
    //                if (n is SectionNode) duration += ((SectionNode)n).Duration;
    //                return true;
    //            }, delegate(urakawa.core.TreeNode n) { });
    //            return duration;
    //        }
    //    }

    //    // Count all nodes that match the predicate.
    //    private int Count(Predicate<urakawa.core.TreeNode> p)
    //    {
    //        int count = 0;
    //        acceptDepthFirst(delegate(urakawa.core.TreeNode n)
    //        {
    //            if (p(n)) ++count;
    //            return true;
    //        }, delegate(urakawa.core.TreeNode n) { });
    //        return count;
    //    }

    //    public int PageCount
    //    {
    //        get
    //        {
    //            return Count(delegate(urakawa.core.TreeNode n) { return n is EmptyNode && ((EmptyNode)n).PageNumber != null; });
    //        }
    //    }

    //    public int PhraseCount
    //    {
    //        get
    //        {
    //            return Count(delegate(urakawa.core.TreeNode n) { return n is EmptyNode; });
    //        }
    //    }

    //    public int SectionCount
    //    {
    //        get
    //        {
    //            return Count(delegate(urakawa.core.TreeNode n) { return n is SectionNode; });
    //        }
    //    }

    //    /// <summary>
    //    /// returns list of all the sections descending from root. This may be time consuming for large project
    //    /// </summary>
    //    /// <returns></returns>
    //    public List<SectionNode> GetListOfAllSections ()
    //        {
    //        List<SectionNode> m_SectionsList = new List<SectionNode> ();
    //        this.acceptDepthFirst (
    //                delegate ( urakawa.core.TreeNode n )
    //                    {
    //                                            if (n is SectionNode )
    //                        {
    //                                                                            m_SectionsList.Add ( (SectionNode)n );
    //                        }
    //                    return true;
    //                    },
    //                delegate ( urakawa.core.TreeNode n ) { } );

    //        return m_SectionsList;
    //        }


    //    public override string getXukLocalName() { return XUK_ELEMENT_NAME; }

    //    /// <summary>
    //            ///  Path of directory containing exported DAISY book in raw PCM format
    //    /// </summary>
    //    public string PrimaryExportDirectory
    //    {
    //        get { return mPrimaryExportDirectory; }
    //        set
    //        {
    //            if (value != null) mPrimaryExportDirectory = value;
    //        }
    //    }
    //    /*
    //    private static readonly string PrimaryExportDirectory_ATTRName = "PrimaryExportDirectory";

    //    /// <summary>
    //    /// Read back the used attribute.
    //    /// </summary>
    //    protected override void xukInAttributes(System.Xml.XmlReader reader)
    //    {
    //        if (this.getProperty<XmlProperty>() == null)
    //        {
    //            mPrimaryExportDirectory = reader.GetAttribute(PrimaryExportDirectory_ATTRName);
    //        }
    //                    base.xukInAttributes(reader);
    //    }

    //    /// <summary>
    //    /// Write the used attribute if its value is false (true being the default.)
    //    /// </summary>
    //    protected override void xukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
    //    {
    //        if (!Presentation.UseXukFormat)
    //        {
    //            destination.WriteAttributeString(PrimaryExportDirectory_ATTRName, mPrimaryExportDirectory);
    //        }
    //        base.xukOutAttributes(destination, baseUri);
    //    }
    //    */

    //}
}