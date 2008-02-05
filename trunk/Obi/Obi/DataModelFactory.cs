using System;

namespace Obi
{
    /// <summary>
    /// The custom Data Model Factory for Obi.
    /// </summary>
    public class DataModelFactory : urakawa.DataModelFactory
    {
        public static readonly string NS = "http://www.daisy.org/urakawa/obi";
        public static readonly string XUK_VERSION = "xuk/obi;pre-2";

        /// <summary>
        /// Generator string for XUK export
        /// </summary>
        public static string Generator
        {
            get
            {
                return String.Format("{0} v{1} with toolkit: {2} v{3} (http://urakawa.sf.net/obi)",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Name,
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Name,
                    System.Reflection.Assembly.GetAssembly(typeof(urakawa.Project)).GetName().Version);
            }
        }

        public override urakawa.media.data.MediaDataManager createMediaDataManager()
        {
            return createMediaDataManager(typeof(Audio.DataManager).Name, NS);
        }

        public override urakawa.media.data.MediaDataManager createMediaDataManager(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(Audio.DataManager).Name ?
                new Audio.DataManager() : base.createMediaDataManager(localName, namespaceUri);
        }

        public override urakawa.Presentation createPresentation()
        {
            return createPresentation(typeof(Obi.Presentation).Name, NS);
        }

        public override urakawa.Presentation createPresentation(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(Obi.Presentation).Name ?
                new Obi.Presentation() : base.createPresentation(localName, namespaceUri);
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory()
        {
            return createTreeNodeFactory(typeof(ObiNodeFactory).Name, NS);
        }

        public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        {
            return namespaceUri == NS && localName == typeof(ObiNodeFactory).Name ?
                new ObiNodeFactory() : base.createTreeNodeFactory(localName, namespaceUri);
        }
    }
}
