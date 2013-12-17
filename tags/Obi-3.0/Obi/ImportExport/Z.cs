using System;
using System.Collections.Generic;
using System.Text;

namespace Obi.ImportExport
{
    /// <summary>
    /// The Z class encapsulates the stylesheets and utility functions
    /// necessary to convert an obi/xuk project to a Z fileset.
    /// </summary>
    class Z
    {
        /// <summary>
        /// Path to the original project.
        /// </summary>
        public string ProjectPath;

        private System.Xml.Xsl.XslCompiledTransform mFilter;               // filter to the main stylesheet
        private System.Xml.Xsl.XslCompiledTransform mTransformer;          // the xuk/obi to z transformer (intermediary form)
        private System.Xml.Xsl.XsltArgumentList mTransformationArguments;  // arguments to the main stylesheet
        private XslResolver mResolver;                                     // the resolver that finds stylesheets in the assembly
        private string mAssemblyLocation;                                  // current location of the assembly

        private string mExportPath;          // path to the export directory
        private Uri mExportUri;              // corresponding URI to the export directory
        private List<double> mElapsedTimes;  // elapsed time at the beginning of each section (and at the end of the book) in ms.

        private static readonly string BASE_NAME = "obi_dtb";                 // base name of the exported files (e.g. "obi_dtb.ncx")
        private static readonly string FILTER_FILE = "Export/filter.xslt";    // name of the filter stylesheet
        private static readonly string XSLT_FILE = "Export/Z.xslt";           // name of the main stylesheet
        private static readonly string PACKAGE_XSLT = "Export/package.xslt";  // name of the package file XSLT
        private static readonly string SMIL_XSLT = "Export/smil.xslt";        // name of the SMIL XSLT
        private static readonly string NCX_XSLT = "Export/ncx.xslt";          // name of the NCX XSLT


        /// <summary>
        /// Create the Z export instance. Don't forget to set the parameters afterwards.
        /// </summary>
        public Z()
        {
            mFilter = new System.Xml.Xsl.XslCompiledTransform(true);
            mTransformer = new System.Xml.Xsl.XslCompiledTransform(true);
            mTransformationArguments = new System.Xml.Xsl.XsltArgumentList();
            mTransformationArguments.AddExtensionObject("http://www.daisy.org/urakawa/obi/xslt-extensions", this);
            Type t = GetType();
            mResolver = new XslResolver(t);
            mAssemblyLocation = System.IO.Path.GetDirectoryName(t.Assembly.Location);
            mFilter.Load(System.IO.Path.Combine(mAssemblyLocation, FILTER_FILE), null, mResolver);
            mTransformer.Load(System.IO.Path.Combine(mAssemblyLocation, XSLT_FILE), null, mResolver);
        }


        /// <summary>
        /// Set the total time parameter for the output.
        /// </summary>
        public List<double> ElapsedTimes { set { mElapsedTimes = value; } }

        /// <summary>
        /// Set the export path.
        /// </summary>
        public string ExportPath
        {
            set
            {
                mExportPath = value;
                mExportUri = new Uri(value);
            }
        }

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
            // We get a) a URI relative to the project path, and b) a stupidly escaped one that crashes the URI constructor.
            // So it's a bit convoluted, but here it is.
            //http://blogs.msdn.com/yangxind/archive/2006/11/09/don-t-use-net-system-uri-unescapedatastring-in-url-decoding.aspx
            Uri absolute = new Uri(new Uri(ProjectPath + System.IO.Path.DirectorySeparatorChar), Uri.UnescapeDataString(Uri.UnescapeDataString(uri)));
            string str = mExportUri.MakeRelativeUri(absolute).ToString();
            return str;
        }

        /// <summary>
        /// Get the total elapsed time for a given section.
        /// </summary>
        public double TotalElapsedTime(int position) { return mElapsedTimes[position]; }

        /// <summary>
        /// Get the total elapsed time for a given section, in SMIL clock value format
        /// </summary>
        public string TotalElapsedTimeFormatted(int position)
        {
            return TimeSpan.FromTicks((long)(TotalElapsedTime(position) * TimeSpan.TicksPerMillisecond)).ToString();
        }

        // Get the full path of a file for a given extension.
        private string FullPath(string ext) { return System.IO.Path.Combine(mExportPath, RelativePath(ext)); }
        private string FullPath(string ext, string id) { return System.IO.Path.Combine(mExportPath, RelativePath(ext, id)); }

        public void WriteFiltered(System.Xml.XmlReader input)
        {
            System.Xml.XmlWriter filtered = System.Xml.XmlWriter.Create(FullPath(".filtered.xuk"), mFilter.OutputSettings);
            mFilter.Transform(input, filtered);
            filtered.Close ();
            filtered = null;
        }

        /// <summary>
        /// Write the full fileset for the XUK input.
        /// </summary>
        public void WriteFileset(System.Xml.XmlReader input)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            System.Xml.XmlWriter output = System.Xml.XmlWriter.Create(writer);
            mFilter.Transform(input, output);
            System.Xml.XPath.XPathDocument filtered = new System.Xml.XPath.XPathDocument(new System.IO.StringReader(writer.ToString()));
            output.Close();
            writer.Close();
            writer = new System.IO.StringWriter();
            output = System.Xml.XmlWriter.Create(writer);
            mTransformer.Transform(filtered, mTransformationArguments, output);
            System.Xml.XPath.XPathDocument z = new System.Xml.XPath.XPathDocument(new System.IO.StringReader(writer.ToString()));
            output.Close();
            writer.Close();
            WriteXSLT(z, PACKAGE_XSLT, ".opf");
            WriteXSLT(z, NCX_XSLT, ".ncx");
            WriteSMILFiles(z);
        }

        private void WriteXSLT(System.Xml.XPath.XPathDocument z, string stylesheet, string suffix)
        {
            System.Xml.Xsl.XslCompiledTransform transform = new System.Xml.Xsl.XslCompiledTransform(true);
            transform.Load(System.IO.Path.Combine(mAssemblyLocation, stylesheet), null, mResolver);
            System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(FullPath(suffix), transform.OutputSettings);
            transform.Transform(z, writer);
        }

        // Write the SMIL files from the Z composite document
        private void WriteSMILFiles(System.Xml.XPath.XPathDocument z)
        {
            System.Xml.Xsl.XslCompiledTransform smilXslt = new System.Xml.Xsl.XslCompiledTransform(true);
            smilXslt.Load(System.IO.Path.Combine(mAssemblyLocation, SMIL_XSLT), null, mResolver);
            System.Xml.XPath.XPathNavigator navigator = z.CreateNavigator();
            System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(navigator.NameTable);
            nsmgr.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/");
            System.Xml.XPath.XPathNodeIterator it = navigator.Select("/z/smil:smil/smil:body/smil:seq", nsmgr);
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
    }
}
