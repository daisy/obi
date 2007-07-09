using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using urakawa.property;

// This file contains the custom properties sutff

namespace Obi
{
    public abstract class ObiProperty: Property
    {
        /*protected TreeNode mOwner;  // owner of the property

        public override TreeNode getOwner()
        {
            return mOwner;
        }

        public override void setOwner(TreeNode newOwner)
        {
            mOwner = (TreeNode)newOwner;
        }*/

        public virtual bool XUKIn(System.Xml.XmlReader source)
        {
            return false;
        }

        public virtual bool XUKOut(System.Xml.XmlWriter destination)
        {
            return true;
        }
    }

    public class ObiPropertyFactory : urakawa.PropertyFactory
    {
        public ObiPropertyFactory() : base()
		{
		}

        public override Property createProperty(string localName, string namespaceUri)
        {
            if (namespaceUri == Program.OBI_NS)
            {
                switch (localName)
                {
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
    /// Page property: assign a page number to a phrase node.
    /// </summary>
    public class PageProperty : ObiProperty
    {
        private int mPageNumber;                          // page number (non-negative integer)
        public static readonly string NodeName = "page";  // the XML element name for XUK in/out
        public static readonly string AttrName = "num";   // attribute name for the page number

        /// <summary>
        /// Get or set the page number.
        /// </summary>
        public int PageNumber
        {
            get { return mPageNumber; }
            set { if (value >= 0) mPageNumber = value; }
        }

        /// <summary>
        /// Create a new page property.
        /// </summary>
        internal PageProperty()
            : base()
        {
            mPageNumber = 0;
        }

        /// <summary>
        /// Copy of the page property (with the same page number and owner.)
        /// </summary>
        /// <returns>A copy of the property.</returns>
        protected override Property copyProtected()
        {
            PageProperty copy = new PageProperty();
            copy.setOwner(getOwner());
            copy.mPageNumber = mPageNumber;
            return copy;
        }

        /// <summary>
        /// Read in a page property from a XUK file.
        /// </summary>
        public override bool XUKIn(System.Xml.XmlReader source)
        {
            if (source == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Reader is null");
            if (source.LocalName == NodeName &&
                source.NamespaceURI == Program.OBI_NS &&
                source.NodeType == System.Xml.XmlNodeType.Element)
            {
                string page = source.GetAttribute(AttrName);
                mPageNumber = Int32.Parse(page);
                if (source.IsEmptyElement) return true;
            }
            return false;
        }

        /// <summary>
        /// Write out the page property to a XUK file.
        /// </summary>
        public override bool XUKOut(System.Xml.XmlWriter destination)
        {
            if (destination == null) throw new urakawa.exception.MethodParameterIsNullException("Xml Writer is null");
            destination.WriteStartElement(NodeName, Program.OBI_NS);
            destination.WriteAttributeString(AttrName, mPageNumber.ToString());
            destination.WriteEndElement();
            return true;
        }
    }
}