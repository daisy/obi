using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;

// This file contains the custom properties sutff

namespace Obi
{
    public abstract class ObiProperty: IProperty
    {
        protected CoreNode mOwner;  // owner of the property

        public abstract IProperty copy();

        public ICoreNode getOwner()
        {
            return mOwner;
        }

        public void setOwner(ICoreNode newOwner)
        {
            mOwner = (CoreNode)newOwner;
        }

        public virtual bool XUKIn(System.Xml.XmlReader source)
        {
            return false;
        }

        public virtual bool XUKOut(System.Xml.XmlWriter destination)
        {
            return true;
        }
    }

    public class ObiPropertyFactory : PropertyFactory
    {
        public static readonly string ObiNS = "http://www.daisy.org/urakawa/obi/0.5";  // NS for Obi XUK

        public ObiPropertyFactory() : base()
		{
		}

        public override IProperty createProperty(string localName, string namespaceUri)
        {
            if (namespaceUri == ObiNS)
            {
                switch (localName)
                {
                    case "AssetProperty":
                        return new AssetProperty();
                    case "info":
                    case "NodeInformationProperty":
                        return new NodeInformationProperty();
                    case "page":
                    case "PageProperty":
                        return new PageProperty();
                    default:
                        throw new Exception(String.Format("Cannot create property named `{0}'", localName));
                }
            }
            return base.createProperty(localName, namespaceUri);
        }
    }

    /// <summary>
    /// Types of nodes in the tree:
    ///   * Root is the root of the tree;
    ///   * Section is a section node in the tree;
    ///   * Phrase is a phrase in the tree;
    ///   * Vanilla is a vanilla core node (should not occur in the tree.)
    /// </summary>
    public enum NodeType { Root, Section, Phrase, Vanilla };

    /// <summary>
    /// Possible status of a phrase node in the tree:
    ///   * Used: a regular node, it is currently used.
    ///   * Unused: kept in the project but let's pretend it's not here.
    ///   * Invalid: the node has invalid asset (for instance.)
    /// For other nodes, we just use:
    ///   * NA: not applicable.
    /// </summary>
    public enum NodeStatus { Used, Unused, Invalid, NA };

    /// <summary>
    /// Information about a node: id, type, status.
    /// </summary>
    public class NodeInformationProperty : ObiProperty
    {
        private int mId;                   // node id for easier reference
        private NodeType mNodeType;        // type of the node
        private NodeStatus mNodeStatus;    // status of the node

        private static int IdCounter = 0;                 // counts created objects 
        public static readonly string NodeName = "info";  // the XML element name for XUK in/out

        public int Id
        {
            get { return mId; }
        }

        public NodeType NodeType
        {
            get { return mNodeType; }
            set { mNodeType = value; }
        }

        public NodeStatus NodeStatus
        {
            get { return mNodeStatus; }
            set { mNodeStatus = value; }
        }

        internal NodeInformationProperty()
            : base()
        {
            mId = IdCounter++;
            mNodeType = NodeType.Vanilla;
            mNodeStatus = NodeStatus.NA;
        }

        public override IProperty copy()
        {
            NodeInformationProperty copy = new NodeInformationProperty();
            copy.setOwner(mOwner);
            copy.NodeType = mNodeType;
            copy.NodeStatus = mNodeStatus;
            return copy;
        }

        public override bool XUKIn(System.Xml.XmlReader source)
        {
            if (source == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Reader is null");
            if (source.LocalName == NodeName &&
                source.NamespaceURI == ObiPropertyFactory.ObiNS &&
                source.NodeType == System.Xml.XmlNodeType.Element)
            {
                string id = source.GetAttribute("id");
                mId = Int32.Parse(id);
                if (IdCounter < mId) IdCounter = mId;
                string type = source.GetAttribute("type");
                mNodeType = type == "Root" ? NodeType.Root :
                    type == "Section" ? NodeType.Section :
                    type == "Phrase" ? NodeType.Phrase : NodeType.Vanilla;
                string status = source.GetAttribute("status");
                mNodeStatus = status == "Used" ? NodeStatus.Used :
                    status == "Unused" ? NodeStatus.Unused :
                    status == "Invalid" ? NodeStatus.Invalid : NodeStatus.NA;
                if (source.IsEmptyElement) return true;
                while (source.Read())
                {
                    if (source.NodeType == System.Xml.XmlNodeType.Element)
                    {
                        break;
                    }
                    if (source.NodeType == System.Xml.XmlNodeType.EndElement) break;
                    if (source.EOF) break;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool XUKOut(System.Xml.XmlWriter destination)
        {
            if (destination == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Writer is null");
            destination.WriteStartElement(NodeName, ObiPropertyFactory.ObiNS);
            destination.WriteAttributeString("id", mId.ToString());
            destination.WriteAttributeString("type", mNodeType.ToString());
            destination.WriteAttributeString("status", mNodeStatus.ToString());
            destination.WriteEndElement();
            return true;
        }
    }

    public class AssetProperty: ObiProperty
    {
        private Assets.AudioMediaAsset mAsset;  // the asset for this node

        public Assets.AudioMediaAsset Asset
        {
            get { return mAsset; }
            set { mAsset = value; }
        }

        internal AssetProperty(): base()
        {
            mAsset = null;
        }

        public override IProperty copy()
        {
            AssetProperty copy = new AssetProperty();
            copy.setOwner(mOwner);
            copy.Asset = mAsset;
            return copy;
        }
    }

    public class PageProperty : ObiProperty
    {
        private int mPageNumber;                          // page number (non-negative integer)
        public static readonly string NodeName = "page";  // the XML element name for XUK in/out

        public int PageNumber
        {
            get { return mPageNumber; }
            set { if (value >= 0) mPageNumber = value; }
        }

        internal PageProperty()
            : base()
        {
            mPageNumber = 0;
        }

        public override IProperty copy()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override bool XUKIn(System.Xml.XmlReader source)
        {
            if (source == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Reader is null");
            if (source.LocalName == NodeName &&
                source.NamespaceURI == ObiPropertyFactory.ObiNS &&
                source.NodeType == System.Xml.XmlNodeType.Element)
            {
                string page = source.GetAttribute("num");
                mPageNumber = Int32.Parse(page);
                if (source.IsEmptyElement) return true;
            }
            return false;
        }

        public override bool XUKOut(System.Xml.XmlWriter destination)
        {
            if (destination == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Writer is null");
            if (getOwner() != null)
            {
                destination.WriteStartElement(NodeName, ObiPropertyFactory.ObiNS);
                destination.WriteAttributeString("num", mPageNumber.ToString());
                destination.WriteEndElement();
            }
            return true;
        }
    }
}