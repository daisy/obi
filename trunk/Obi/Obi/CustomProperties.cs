using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using VirtualAudioBackend;

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

        public ObiPropertyFactory(Presentation presentation) : base(presentation)
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
                    case "NodeTypeProperty":
                        return new NodeTypeProperty();
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
    ///   * Page is a page node in the tree;
    ///   * Vanilla is a vanilla core node (should not occur in the tree.)
    /// </summary>
    public enum NodeType { Root, Section, Phrase, Page, Vanilla };

    public class NodeTypeProperty : ObiProperty
    {
        private NodeType mNodeType;

        public NodeType NodeType
        {
            get { return mNodeType; }
            set { mNodeType = value; }
        }

        internal NodeTypeProperty()
            : base()
        {
            mNodeType = NodeType.Vanilla;
        }

        public override IProperty copy()
        {
            NodeTypeProperty copy = new NodeTypeProperty();
            copy.setOwner(mOwner);
            copy.NodeType = mNodeType;
            return copy;
        }

        public override bool XUKIn(System.Xml.XmlReader source)
        {
            if (source == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Reader is null");
            if (source.LocalName == "NodeTypeProperty" &&
                source.NamespaceURI == ObiPropertyFactory.ObiNS &&
                source.NodeType == System.Xml.XmlNodeType.Element)
            {
                string type = source.GetAttribute("type");
                mNodeType = type == "Root" ? NodeType.Root :
                    type == "Section" ? NodeType.Section :
                    type == "Phrase" ? NodeType.Phrase :
                    type == "Page" ? NodeType.Page : NodeType.Vanilla;
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
            destination.WriteStartElement("NodeTypeProperty", ObiPropertyFactory.ObiNS);
            destination.WriteAttributeString("type", mNodeType.ToString());
            destination.WriteEndElement();
            return true;
        }
    }

    public class AssetProperty: ObiProperty
    {
        private AudioMediaAsset mAsset;  // the asset for this node

        public AudioMediaAsset Asset
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
}