using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.Export
{
    class Z
    {
        private System.Xml.Xsl.XslCompiledTransform mTransformer;
        private XslResolver mResolver;
        private string mAssemblyLocation;
        private System.Xml.Xsl.XsltArgumentList mTransformationArguments;

        // Path (and corresponding uri) to the export directory
        private string mExportPath;
        private Uri mExportUri;

        private List<double> mElapsedTimes;

        /// <summary>
        /// Path to the original project.
        /// </summary>
        public string ProjectPath;

        private static readonly string BASE_NAME = "obi_dtb";                 // base name of the exported files (e.g. "obi_dtb.ncx")
        private static readonly string XSLT_FILE = "Export/Z.xslt";           // name of the main stylesheet (relative to the assembly)
        private static readonly string PACKAGE_XSLT = "Export/package.xslt";  // name of the package file XSLT
        private static readonly string SMIL_XSLT = "Export/smil.xslt";        // name of the SMIL XSLT

        public Z()
        {
            mTransformer = new System.Xml.Xsl.XslCompiledTransform(true);
            mTransformationArguments = new System.Xml.Xsl.XsltArgumentList();
            mTransformationArguments.AddExtensionObject("http://www.daisy.org/urakawa/obi/xslt-extensions", this);
            Type t = GetType();
            mResolver = new XslResolver(t);
            mAssemblyLocation = System.IO.Path.GetDirectoryName(t.Assembly.Location);
            mTransformer.Load(System.IO.Path.Combine(mAssemblyLocation, XSLT_FILE), null, mResolver);            
        }


        /// <summary>
        /// Set the total time parameter for the output.
        /// </summary>
        public List<double> ElapsedTimes { set { mElapsedTimes = value; } }

        public string ExportPath
        {
            set
            {
                mExportPath = value;
                mExportUri = new Uri(value);
            }
        }

        /// <summary>
        /// Allow read-only access to the transformation arguments list.
        /// </summary>
        public System.Xml.Xsl.XsltArgumentList TransformationArguments { get { return mTransformationArguments; } }

        /// <summary>
        /// Get the relative path of a file for a given extension. 
        /// </summary>
        public string RelativePath(string ext) { return BASE_NAME + ext; }

        /// <summary>
        /// Get the relative path for a file for a given extension, adding the id to distinguish
        /// it from other files of the same kind.
        /// </summary>
        public string RelativePath(string ext, string id) { return BASE_NAME + id + ext; }

        /// <summary>
        /// Get the relative path for an URI (passed as a string from XSLT.)
        /// </summary>
        public string RelativePathForUri(string uri)
        {
            return mExportUri.MakeRelativeUri(new Uri(uri)).ToString();
        }

        /// <summary>
        /// Get the total elapsed time for a given section.
        /// </summary>
        public double TotalElapsedTime(int position) { return mElapsedTimes[position]; }

        // Get the full path of a file for a given extension.
        private string FullPath(string ext) { return System.IO.Path.Combine(mExportPath, RelativePath(ext)); }
        private string FullPath(string ext, string id) { return System.IO.Path.Combine(mExportPath, RelativePath(ext, id)); }

        /// <summary>
        /// Write the full fileset for the XUK input.
        /// </summary>
        public void WriteFileset(System.Xml.XmlReader input)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            System.Xml.XmlWriter output = System.Xml.XmlWriter.Create(writer);
            mTransformer.Transform(input, mTransformationArguments, output);
            System.Xml.XPath.XPathDocument z = new System.Xml.XPath.XPathDocument(new System.IO.StringReader(writer.ToString()));
            WritePackageFile(z);
            WriteSMILFiles(z);
            WriteNCX(z);
        }

        // Write the package file from the Z composite document
        private void WritePackageFile(System.Xml.XPath.XPathDocument z)
        {
            System.Xml.Xsl.XslCompiledTransform packageXslt = new System.Xml.Xsl.XslCompiledTransform(true);
            packageXslt.Load(System.IO.Path.Combine(mAssemblyLocation, PACKAGE_XSLT), null, mResolver);
            System.Xml.XmlWriter packageFile = System.Xml.XmlWriter.Create(FullPath(".opf"), packageXslt.OutputSettings);
            packageXslt.Transform(z, packageFile);
        }

        // Write the SMIL files from the Z composite document
        private void WriteSMILFiles(System.Xml.XPath.XPathDocument z)
        {
            System.Xml.Xsl.XslCompiledTransform smilXslt = new System.Xml.Xsl.XslCompiledTransform(true);
            smilXslt.Load(System.IO.Path.Combine(mAssemblyLocation, SMIL_XSLT), null, mResolver);
            System.Xml.XPath.XPathNavigator navigator = z.CreateNavigator();
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(navigator.NameTable);
            nsmgr.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/");
            System.Xml.XPath.XPathNodeIterator it = navigator.Select("/z/smil:smil", nsmgr);
            while (it.MoveNext())
            {
                System.Xml.Xsl.XsltArgumentList args = new System.Xml.Xsl.XsltArgumentList();
                string id = it.Current.GetAttribute("id", "");
                args.AddParam("id", "", id);
                System.Xml.XmlWriter smilFile = System.Xml.XmlWriter.Create(FullPath(".smil", id),
                    smilXslt.OutputSettings);
                smilXslt.Transform(z, args, smilFile);
            }
        }

        // Write the NCX file from the Z composite document
        private void WriteNCX(System.Xml.XPath.XPathDocument z)
        {
        }
    }
}
