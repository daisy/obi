using System;
using System.IO;

namespace Obi
{
    public partial class Project
    {
        static string mXsltFile = "XukToZed.xslt";
        static string mDaisyOutputDirSuffix = "_daisy";
        public void ExportToZed()
        {
            
            //first, sort out the audio clips

            //then save the xuk file
            if (this.Unsaved) this.Save();

            //then invoke the XukToZed transformation
            string outputFolder = System.IO.Path.GetDirectoryName(XUKPath);
            string xukFileName = System.IO.Path.GetFileNameWithoutExtension(XUKPath);
            outputFolder += "\\" + xukFileName + mDaisyOutputDirSuffix;
            //create the output folder if it doesn't exist
            //todo: consider asking the user if they want to choose a folder
            if (!System.IO.Directory.Exists(outputFolder)) System.IO.Directory.CreateDirectory(outputFolder);
            
            ConvertXukToZed(xukFileName, outputFolder);

        }

        private void ConvertXukToZed(string safeProjectName, string outputFolder)
        {
            //assuming that the stylesheet is given as relative to the contextFolderName (set below)
            string xsltPath = Path.GetDirectoryName(GetType().Assembly.Location) + @"\" + mXsltFile;
            XukToZed.XukToZed x2z = new XukToZed.XukToZed(xsltPath);

            x2z.OuputDir = outputFolder;
            x2z.contextFolderName = Environment.CurrentDirectory;

            string tmpPackageName = safeProjectName + ".opf";
            x2z.TransformationArguments.AddParam("packageFilename", "", tmpPackageName);

            string tmpNcxName = safeProjectName + ".ncx";
            x2z.TransformationArguments.AddParam("ncxFilename", "", tmpNcxName);

            System.Xml.XmlReaderSettings readSettings = new System.Xml.XmlReaderSettings();
            readSettings.XmlResolver = null;

            try
            {
                System.Xml.XmlReader xukDatasource = System.Xml.XmlReader.Create(XUKPath);

                x2z.WriteZed(xukDatasource);

                if (!System.IO.File.Exists(x2z.OuputDir + "/" + tmpNcxName) ||
                    !System.IO.File.Exists(x2z.OuputDir + "/" + tmpPackageName))
                {
                    throw new Exception("Error saving project as DAISY");
                }
                else
                {
                    string success = String.Format(Localizer.Message("saved_as_daisy"), outputFolder);

                    System.Windows.Forms.MessageBox.Show(success);

                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.Print("EXPORT FAILED: {0}", e.Message);
            }
        }
    }
}
