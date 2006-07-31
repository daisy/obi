using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using urakawa.core;

// a lot copied from ExampleCustomProperty in the toolkit.

namespace Obi
{
    /// <summary>
    /// Types of nodes in the tree:
    ///   * RootNode is the root of the tree;
    ///   * SectionNode is a section node in the tree;
    ///   * PhraseNode is a phrase in the tree;
    ///   * PageNode is a page node in the tree;
    ///   * Vanilla is a vanilla core node (should not occur in the tree.)
    /// </summary>
    public enum NodeType { RootNode, SectionNode, PhraseNode, PageNode, Vanilla };

    public class NodeTypeProperty: IProperty
    {
        private CoreNode mOwner;  // the node that owns this property
        private NodeType mType;   // the type of the node

        public NodeType Type
        {
            get
            {
                return mType;
            }
            set
            {
                mType = value;
            }
        }

        internal NodeTypeProperty()
        {
            mOwner = null;
            mType = NodeType.Vanilla;
        }

        #region IProperty Members

        public IProperty copy()
        {
            return null;
        }

        public ICoreNode getOwner()
        {
            return mOwner;
        }

        public void setOwner(ICoreNode newOwner)
        {
            if (!mOwner.GetType().IsAssignableFrom(newOwner.GetType()))
            {
                throw new urakawa.exception.MethodParameterIsWrongTypeException(
                    "The owner must be a CoreNode or a subclass of CoreNode");
            }
            IPropertyFactory propFact = this.getOwner().getPresentation().getPropertyFactory();
            if (!propFact.GetType().IsSubclassOf(typeof(NodeTypePropertyFactory)))
            {
                throw new urakawa.exception.OperationNotValidException(
                    "The property factory of the presentation of the owner must subclass NodeTypePropertyFactory");
            }
            mOwner = (CoreNode)newOwner;
        }

        #endregion

        #region IXUKable Members

        // The propery is saved as:
        // <NodeTypeProperty xmlns="http://www.daisy.org/ns/obi" type="RootNode"/>

        private const string NS = "http://www.daisy.org/ns/obi";
        private const string ElementName = "NodeTypeProperty";
        private const string AttributeName = "type";

        public bool XUKin(XmlReader source)
        {
            if (source == null)
            {
                throw new urakawa.exception.MethodParameterIsNullException("Xml Reader is null");
            }
            if (!(source.LocalName == ElementName
                && source.NamespaceURI == NS
                && source.NodeType == System.Xml.XmlNodeType.Element))
            {
                return false;
            }
            string type = source.GetAttribute(AttributeName);
            System.Diagnostics.Debug.Print("Got node type element, type = <{0}>", type);
            mType = type == "RootNode" ? NodeType.RootNode :
                type == "SectionNode" ? NodeType.SectionNode :
                type == "PhraseNode" ? NodeType.PhraseNode :
                type == "PageNode" ? NodeType.PageNode : NodeType.Vanilla;
            if (source.IsEmptyElement) return true;
            while (source.Read())
            {
                if (source.NodeType == System.Xml.XmlNodeType.Element)
                {
                    break;
                }
                if (source.NodeType == System.Xml.XmlNodeType.EndElement) return true;
                if (source.EOF) break;
            }
            return false;
        }

        public bool XUKout(XmlWriter destination)
        {
            if (destination == null)
            {
                throw new urakawa.exception.MethodParameterIsNullException("Xml Writer is null");
            }
            destination.WriteStartElement(ElementName, NS);
            destination.WriteAttributeString(AttributeName, mType.ToString());
            destination.WriteEndElement();
            return true;
        }

        #endregion
    }

    public class NodeTypePropertyFactory : PropertyFactory
    {
        /// <summary>
		/// Constructor setting the <see cref="Presentation"/> to which the instance belongs
		/// </summary>
		/// <param name="p">The presentation to which the instance belongs.</param>
		public NodeTypePropertyFactory(Presentation p) : base(p)
		{
		}

		/// <summary>
		/// Creates a <see cref="IProperty"/> of <see cref="Type"/> matching a given type string
		/// </summary>
		/// <param name="typeString">The given type string</param>
		/// <returns></returns>
        public override IProperty createProperty(string typeString)
        {
            if (typeString == "NodeTypeProperty")
            {
                return new NodeTypeProperty();
            }
            else
            {
                return base.createProperty(typeString);
            }
        }
    }
}
