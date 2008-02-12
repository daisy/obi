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

        /// <summary>
        /// Path to the export directory.
        /// </summary>
        public string ExportPath;

        /// <summary>
        /// Path to the original project.
        /// </summary>
        public string ProjectPath;

        private static readonly string BASE_NAME = "obi_dtb";                 // base name of the exported files (e.g. "obi_dtb.ncx")
        private static readonly string XSLT_FILE = "Export/Z.xslt";           // name of the main stylesheet (relative to the assembly)
        private static readonly string PACKAGE_XSLT = "Export/package.xslt";  // name of the package file XSLT

        public Z()
        {
            mTransformer = new System.Xml.Xsl.XslCompiledTransform(true);
            mTransformationArguments = new System.Xml.Xsl.XsltArgumentList();
            mTransformationArguments.AddParam("uid", "", "UID");
            Type t = GetType();
            mResolver = new XslResolver(t);
            mAssemblyLocation = System.IO.Path.GetDirectoryName(t.Assembly.Location);
            mTransformer.Load(System.IO.Path.Combine(mAssemblyLocation, XSLT_FILE), null, mResolver);
        }


        /// <summary>
        /// Set the total time parameter for the output.
        /// </summary>
        public double TotalTime { set { mTransformationArguments.AddParam("total-time", "", value); } }

        /// <summary>
        /// Allow read-only access to the transformation arguments list.
        /// </summary>
        public System.Xml.Xsl.XsltArgumentList TransformationArguments { get { return mTransformationArguments; } }

        /// <summary>
        /// Write the full fileset for the XUK input.
        /// </summary>
        public void WriteFileset(System.Xml.XmlReader input)
        {
            System.IO.StringWriter writer = new System.IO.StringWriter();
            System.Xml.XmlWriter output = System.Xml.XmlWriter.Create(writer);
            mTransformer.Transform(input, mTransformationArguments, output);
            System.Xml.XPath.XPathDocument z = new System.Xml.XPath.XPathDocument(new System.IO.StringReader(writer.ToString()));

            System.Xml.Xsl.XslCompiledTransform packageXslt = new System.Xml.Xsl.XslCompiledTransform(true);
            packageXslt.Load(System.IO.Path.Combine(mAssemblyLocation, PACKAGE_XSLT), null, mResolver);
            System.Xml.XmlWriter packageFile = System.Xml.XmlWriter.Create(System.IO.Path.Combine(ExportPath, BASE_NAME + ".opf"),
                packageXslt.OutputSettings);
            packageXslt.Transform(z, packageFile);
            
            /*
            System.IO.StringWriter writer = new System.IO.StringWriter();
            System.Xml.XmlWriter results = System.Xml.XmlWriter.Create((System.IO.TextWriter)writer);
            try
            {
                mTransformer.Transform(input, TransformationArguments, results);
            }
            catch (Exception)
            { 
                results = null;
            }
            input.Close();

            // Delete the SMIL files (?)
            string[] smilFiles = System.IO.Directory.GetFiles(OutputDir, "*.smil");
            foreach(string smilFile in smilFiles) System.IO.File.Delete(smilFile);

            XmlDocument resDoc = new XmlDocument();
            resDoc.LoadXml(writer.ToString());
            XmlWriterSettings fileSettings = new XmlWriterSettings();
            fileSettings.Indent = true;
            //TODO:Remove following line
            resDoc.Save(OutputDir + "/raw.xml");

            XmlNamespaceManager xPathNSManager = new XmlNamespaceManager((XmlNameTable)new NameTable());
            xPathNSManager.AddNamespace("smil", "http://www.w3.org/2001/SMIL20/Language");
            xPathNSManager.AddNamespace("opf", "http://openebook.org/namespaces/oeb-package/1.0/");
            xPathNSManager.AddNamespace("ncx", "http://www.daisy.org/z3986/2005/ncx/");

            XmlNode ncxTree = resDoc.DocumentElement.SelectSingleNode("//ncx:ncx", xPathNSManager);
            string ncxFilename = (string)TransformationArguments.GetParam("ncxFilename", "");
            if (ncxFilename == "") ncxFilename = "navigation.ncx";
            XmlWriter ncxFile = XmlWriter.Create(OutputDir + "/" + ncxFilename, fileSettings);
            ncxFile.WriteNode(ncxTree.CreateNavigator(), false);
            ncxFile.Close();
            ncxTree.ParentNode.RemoveChild(ncxTree); //remove the written bit

            #region Calculating running time, setting on smil file nodes as required

            TimeSpan prevDuration = new TimeSpan();
            try
            {
                string tmpXpathStatement = "//*[self::smil:smil or self::smil:audio]";
                XmlNodeList lstAudAndSmil = resDoc.DocumentElement.SelectNodes(tmpXpathStatement, xPathNSManager);
                
                for (int i = 0; i < lstAudAndSmil.Count;i++)
                {
                    XmlElement curElement = (XmlElement)lstAudAndSmil[i];
                    switch (curElement.LocalName)
                    {
                        case "smil":
                            XmlElement ndElapsed = (XmlElement)curElement.SelectSingleNode(".//smil:meta[@name='dtb:totalElapsedTime']", xPathNSManager);
                            ndElapsed.SetAttribute("content",prevDuration.ToString().TrimEnd(".0".ToCharArray()));
                            break;
                        case "audio":
                            try
                            {
                                prevDuration = prevDuration.Subtract(TimeSpan.Parse(curElement.GetAttribute("clipBegin")));
                            }
                            catch { } 
                            try
                            {
                                prevDuration = prevDuration.Add(TimeSpan.Parse((curElement.GetAttribute("clipEnd"))));
                            }
                            catch { }
                            break;
                        default:

                            break;

                    }
                }
            }
            catch(Exception eAnything)
            {
                //TODO: Error forwarding
                System.Diagnostics.Debug.WriteLine(eAnything.ToString());
            }

            //TODO:Remove following line
            //resDoc.Save(strOutputDir + "/raw.xml");
            #endregion 

            XmlElement metaDtbTotalDuration = (XmlElement)resDoc.SelectSingleNode("//opf:meta[@name='dtb:totalTime']",xPathNSManager);
            metaDtbTotalDuration.SetAttribute("content", prevDuration.ToString().TrimEnd(".0".ToCharArray()));

            XmlNode opfTree = resDoc.DocumentElement.SelectSingleNode("//opf:package", xPathNSManager);
            string opfFilename = (string)TransformationArguments.GetParam("packageFilename","");
            if (opfFilename == "")
                opfFilename = "package.opf";
            XmlWriter opfFile = XmlWriter.Create(OutputDir + "/" + opfFilename, fileSettings);
            opfFile.WriteNode(opfTree.CreateNavigator(), false);
            opfFile.Close();
            opfTree.ParentNode.RemoveChild(opfTree); //remove the written bit

            XmlNodeList smilTrees = resDoc.DocumentElement.SelectNodes("//smil:smil", xPathNSManager);
            for (int i = smilTrees.Count - 1; i > -1; i--)
            {
                XmlElement newRoot = (XmlElement)smilTrees[i];
                XmlWriter smilFile = XmlWriter.Create(OutputDir + "/" + newRoot.GetAttribute("filename") + ".smil",fileSettings);
                newRoot.RemoveAttribute("filename");
                smilFile.WriteNode(newRoot.CreateNavigator(),false);
                smilFile.Close();
                newRoot.ParentNode.RemoveChild(newRoot);
            }

            XmlNodeList filesToCopy = resDoc.DocumentElement.SelectNodes("filenames/file",xPathNSManager);
            foreach(XmlNode fileNode in filesToCopy)
            {
                string strSourceFileName = ContextFolder + "\\" + fileNode.InnerText;
                strSourceFileName = strSourceFileName.Replace("/","\\");

                string strDestFileName = fileNode.InnerText.Substring((fileNode.InnerText.LastIndexOf("/") > 0) ? fileNode.InnerText.LastIndexOf("/")+1 : 0);
                strDestFileName = OutputDir + "\\" + strDestFileName;
                strDestFileName = strDestFileName.Replace("/","\\");
                try
                {
                    System.IO.File.Copy(strSourceFileName,strDestFileName, true);
                }
                catch (Exception eAnything)
                {
                    System.Diagnostics.Debug.WriteLine(eAnything.ToString());
                }
            }
             * */
        }
        
        // Write the package file
    }
}
