using System;
using System.Collections.Generic;
using System.Text;

using urakawa.core;
using VirtualAudioBackend;

namespace Obi
{
    class AssetProperty: IProperty
    {
        private AudioMediaAsset mAsset;

        public AudioMediaAsset Asset
        {
            get
            {
                return mAsset;
            }
            set
            {
                mAsset = value;
            }
        }

        internal AssetProperty()
        {
            mAsset = null;
        }

        #region IProperty Members

        public IProperty copy()
        {
            return null;
        }

        public ICoreNode getOwner()
        {
            return null;
        }

        public void setOwner(ICoreNode newOwner)
        {
        }

        #endregion

        #region IXUKable Members

        // We don't need to save that, nor reading it back again

        public bool XUKin(System.Xml.XmlReader source)
        {
            return true;
        }

        public bool XUKout(System.Xml.XmlWriter destination)
        {
            return true;
        }

        #endregion
    }

    public class AssetPropertyFactory : NodeTypePropertyFactory
    {
        /// <summary>
        /// Constructor setting the <see cref="Presentation"/> to which the instance belongs
        /// </summary>
        /// <param name="p">The presentation to which the instance belongs.</param>
        public AssetPropertyFactory(Presentation p)
            : base(p)
        {
        }

        /// <summary>
        /// Creates a <see cref="IProperty"/> of <see cref="Type"/> matching a given type string
        /// </summary>
        /// <param name="typeString">The given type string</param>
        /// <returns></returns>
        public override IProperty createProperty(string typeString)
        {
            if (typeString == "AssetProperty")
            {
                return new AssetProperty();
            }
            else
            {
                return base.createProperty(typeString);
            }
        }
    }
}
