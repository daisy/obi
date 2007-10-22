using System;
using System.Collections.Generic;
using System.Text;
using urakawa.core;
using urakawa.property;

namespace Obi
{
    /// <summary>
    /// Property factory for Obi properties.
    /// </summary>
    public class ObiPropertyFactory : urakawa.property.PropertyFactory
    {
        /// <summary>
        /// Create a new Obi or default property.
        /// </summary>
        public override Property createProperty(string localName, string namespaceUri)
        {
            if (namespaceUri == DataModelFactory.NS)
            {
                if (localName == PageProperty.XUK_ELEMENT_NAME)
                {
                    return new PageProperty();
                }
                else
                {
                    throw new Exception(String.Format("Cannot create property named `{0}'", localName));
                }
            }
            return base.createProperty(localName, namespaceUri);
        }

        public override string getXukNamespaceUri() { return DataModelFactory.NS; }
    }

    /// <summary>
    /// Page property: assign a page number to a phrase node.
    /// </summary>
    public class PageProperty : Property
    {
        private int mPageNumber;  // page number (non-negative integer)

        public static readonly string XUK_ELEMENT_NAME = "page";    // the XML element name for XUK in/out
        public static readonly string XUK_ATTRIBUTE_NAME = "num";   // attribute name for the page number

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
        public PageProperty(): base()
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
            //copy.setOwner(getOwner());
            copy.mPageNumber = mPageNumber;
            return copy;
        }

        /// <summary>
        /// Read in a page property from a XUK file.
        /// </summary>
        protected override void XukInAttributes(System.Xml.XmlReader source)
        {
            base.XukInAttributes(source);
            string page = source.GetAttribute(XUK_ATTRIBUTE_NAME);
            mPageNumber = Int32.Parse(page);
        }

        /// <summary>
        /// Write out the page property to a XUK file.
        /// </summary>
        protected override void XukOutAttributes(System.Xml.XmlWriter destination, Uri baseUri)
        {
            destination.WriteAttributeString(XUK_ATTRIBUTE_NAME, mPageNumber.ToString());
            base.XukOutAttributes(destination, baseUri);
        }
    }
}