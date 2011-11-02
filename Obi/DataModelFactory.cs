using System;

namespace Obi
{
    /// <summary>
    /// The custom Data Model Factory for Obi.
    /// </summary>
    //public class DataModelFactory : urakawa.DataModelFactory
    public class DataModelFactory 
    {
        public const string NS = "http://www.daisy.org/urakawa/obi";  // Obi-specific namespace
        public static readonly string XUK_VERSION = "xuk/obi;2.0";             // versioning for Obi XUK files

        /// <summary>
        /// The generator string for XUK export identifies the version of Obi and of the toolkit used.
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


        // Custom factories for: asset manager, presentations, and tree nodes.
        // For consistency, override both methods without parameters and with
        // localname/nsuri parameters.

        //sdk2
        //public override urakawa.media.data.MediaDataManager createMediaDataManager()
        //public urakawa.media.data.MediaDataManager createMediaDataManager()
        //{
        //    return createMediaDataManager(typeof(Audio.DataManager).Name, NS);
        //}

        //sdk2
        //public override urakawa.media.data.MediaDataManager createMediaDataManager(string localName, string namespaceUri)
        //public urakawa.media.data.MediaDataManager createMediaDataManager(string localName, string namespaceUri)//sdk2
        //{
        //    return namespaceUri == NS && localName == typeof(Audio.DataManager).Name ?
        //        new Audio.DataManager() : base.createMediaDataManager(localName, namespaceUri);
        //}

        //sdk2
        //public override urakawa.Presentation createPresentation()
        //public urakawa.Presentation createPresentation()
        //{
        //    return createPresentation(typeof(Obi.Presentation).Name, NS);
        //}

        //sdk2
        //public override urakawa.Presentation createPresentation(string localName, string namespaceUri)
        //public urakawa.Presentation createPresentation(string localName, string namespaceUri)/
        //{
        //    return namespaceUri == NS && localName == typeof(Obi.Presentation).Name ?
        //        new Obi.Presentation() : base.createPresentation(localName, namespaceUri);
        //}

        //
        //public urakawa.core.TreeNodeFactory createTreeNodeFactory()//sdk2
        //{
        //    return createTreeNodeFactory(typeof(ObiNodeFactory).Name, NS);
        //}

        //public override urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        //public urakawa.core.TreeNodeFactory createTreeNodeFactory(string localName, string namespaceUri)
        //{
        //    return namespaceUri == NS && localName == typeof(ObiNodeFactory).Name ?
        //        new ObiNodeFactory() : base.createTreeNodeFactory(localName, namespaceUri);
        //}
    }
}
