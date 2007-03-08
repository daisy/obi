using System;
using System.IO;

namespace Obi
{
    public partial class Project
    {
        public static string DaisyOutputDirSuffix = "_daisy";
        
        private static string mXsltFile = "XukToZed.xslt";
        
        /// <summary>
        /// 
        /// </summary>
        public void ExportToZed(string outputPath)
        {
            UpdatePublicationMetadata();
          
            string originalAssetDirectory = this.AssetManager.AssetsDirectory;

            //these assets go in a new directory (old_dir_name + "_temp")
            //the project's asset directory isn't changed because we're going to switch
            //everything back to the original name when we're done
            string tempAssetDirectory = Path.GetDirectoryName(this.AssetManager.AssetsDirectory);
            tempAssetDirectory += "_temp\\";
            Directory.CreateDirectory(tempAssetDirectory);

            // sort out the audio clips: create one file per section, then one clip per phrase in this section.
            Visitors.CleanupAssets cleanAssVisitor = new Visitors.CleanupAssets(tempAssetDirectory, false);
   
            //clean up the assets
            this.RootNode.acceptDepthFirst(cleanAssVisitor);
            cleanAssVisitor = null;

            //remove the original asset directory, since all the cleaned assets are in the temp folder
            System.IO.Directory.Delete(originalAssetDirectory, true);
      
            //rename the temp folder to the original directory's name
            System.IO.Directory.Move(tempAssetDirectory, originalAssetDirectory);

            //tell the assets about their new directory
            cleanAssVisitor = new Visitors.CleanupAssets(originalAssetDirectory, true);
            this.RootNode.acceptDepthFirst(cleanAssVisitor);

            // then save the xuk file to update the date metadata.
            Save();

            // then invoke the XukToZed transformation
            string xukFileName = System.IO.Path.GetFileNameWithoutExtension(XUKPath);
            // create the output folder if it doesn't exist
            // if (!System.IO.Directory.Exists(outputPath)) System.IO.Directory.CreateDirectory(outputPath);            
            ConvertXukToZed(xukFileName, outputPath);
        }

        /// <summary>
        /// Update the publication metadata (produced date, revision, revision date) before exporting.
        /// </summary>
        private void UpdatePublicationMetadata()
        {
            urakawa.project.Metadata producedDate = null;
            urakawa.project.Metadata revision = null;
            urakawa.project.Metadata revisionDate = null;

            foreach (object o in getMetadataList())
            {
                urakawa.project.Metadata meta = (urakawa.project.Metadata)o;
                if (meta.getName() == SimpleMetadata.MetaProducedDate)
                {
                    producedDate = meta;
                }
                else if (meta.getName() == SimpleMetadata.MetaRevision)
                {
                    revision = meta;
                }
                else if (meta.getName() == SimpleMetadata.MetaRevisionDate)
                {
                    revisionDate = meta;
                }
            }

            string date = DateTime.Today.ToString("yyyy-MM-dd");
            if (producedDate == null)
            {
                System.Diagnostics.Debug.Assert(revisionDate == null && revision == null);
                producedDate = AddMetadata(SimpleMetadata.MetaProducedDate, date);
            }
            else
            {
                if (revision != null)
                {
                    System.Diagnostics.Debug.Assert(revisionDate != null);
                    int rev = Int32.Parse(revision.getContent()) + 1;
                    revision.setContent(rev.ToString());
                    revisionDate.setContent(date);
                }
                else
                {
                    System.Diagnostics.Debug.Assert(revisionDate == null);
                    AddMetadata(SimpleMetadata.MetaRevision, "1");
                    AddMetadata(SimpleMetadata.MetaRevisionDate, date);
                }
            }
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
