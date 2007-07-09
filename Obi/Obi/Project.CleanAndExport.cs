using System;
using System.IO;

namespace Obi
{
    public partial class Project
    {
        private static string XSLT_FILE = "XukToZed.xslt";

        /// <summary>
        /// This function cleans up the assets in a project
        /// </summary>
        public void CleanProjectAssets()
        {
            string originalAssetDirectory = this.AssetManager.AssetsDirectory;

            //these assets go in a new directory (old_dir_name + "_temp")
            //the project's asset directory isn't changed because we're going to switch
            //everything back to the original name when we're done
            string tempAssetDirectory = Path.GetDirectoryName(this.AssetManager.AssetsDirectory);
            tempAssetDirectory += "_temp" + Path.DirectorySeparatorChar;
            Directory.CreateDirectory(tempAssetDirectory);

            // sort out the audio clips: create one file per section, then one clip per phrase in this section.
            Visitors.CleanupAssets cleanAssVisitor = new Visitors.CleanupAssets(tempAssetDirectory, false);

            try
            {
                //clean up the assets
                this.RootNode.acceptDepthFirst(cleanAssVisitor);
            }
            catch (Exception e)
            {
                cleanAssVisitor = null;
                //delete the temporary asset directory
                //this actually produces its own exception that some files are in use by another process. 
                //we need a fix for this in AudioMediaAsset
                System.IO.Directory.Delete(tempAssetDirectory, true);
                //pass along the exception, since the caller still needs to know about it
                throw e;
            }
            cleanAssVisitor = null;

            //remove the original asset directory, since all the cleaned assets are in the temp folder
            System.IO.Directory.Delete(originalAssetDirectory, true);

            //rename the temp folder to the original directory's name
            System.IO.Directory.Move(tempAssetDirectory, originalAssetDirectory);

            //tell the assets about their new directory
            cleanAssVisitor = new Visitors.CleanupAssets(originalAssetDirectory, true);
            this.RootNode.acceptDepthFirst(cleanAssVisitor);
            
            //is this necessary?  is it expected behavior?
            Save();
        }

        /// <summary>
        /// Export to DAISY/NISO (aka Zed)
        /// </summary>
        public void ExportToZed(string outputPath)
        {
            UpdatePublicationMetadata();

            //smarter approach: only call this if things have changed since the last time it was called
            //however, this would mean storing another status flag - is it really worth it?
            CleanProjectAssets();

            // then save the xuk file to update the date metadata.
            Save();

            // then invoke the XukToZed transformation
            string xukFileName = System.IO.Path.GetFileNameWithoutExtension(XUKPath);
            // create the output folder if it doesn't exist
            if (!System.IO.Directory.Exists(outputPath)) System.IO.Directory.CreateDirectory(outputPath);
            ConvertXukToZed(xukFileName, outputPath);
        }

        /// <summary>
        /// Update the publication metadata (produced date, revision, revision date) before exporting.
        /// </summary>
        private void UpdatePublicationMetadata()
        {
            string date = DateTime.Today.ToString("yyyy-MM-dd");
            urakawa.metadata.Metadata producedDate = GetSingleMetadataItem(Metadata.DTB_PRODUCED_DATE);
            urakawa.metadata.Metadata revision = GetSingleMetadataItem(Metadata.DTB_REVISION);
            urakawa.metadata.Metadata revisionDate = GetSingleMetadataItem(Metadata.DTB_REVISION_DATE);
            if (producedDate == null)
            {
                System.Diagnostics.Debug.Assert(revisionDate == null && revision == null);
                SetSingleMetadataItem(Metadata.DTB_PRODUCED_DATE, date);
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
                    SetSingleMetadataItem(Metadata.DTB_REVISION, "1");
                    SetSingleMetadataItem(Metadata.DTB_REVISION_DATE, date);
                }
            }
        }

        private void ConvertXukToZed(string safeProjectName, string outputFolder)
        {
            //assuming that the stylesheet is given as relative to the contextFolderName (set below)
            string xsltPath = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), XSLT_FILE);
            XukToZed.XukToZed x2z = new XukToZed.XukToZed(xsltPath);

            x2z.OuputDir = outputFolder;
            x2z.contextFolderName = Path.GetDirectoryName(mXUKPath);

            string tmpPackageName = safeProjectName + ".opf";
            x2z.TransformationArguments.AddParam("packageFilename", "", tmpPackageName);

            string tmpNcxName = safeProjectName + ".ncx";
            x2z.TransformationArguments.AddParam("ncxFilename", "", tmpNcxName);

            System.Xml.XmlReaderSettings readSettings = new System.Xml.XmlReaderSettings();
            readSettings.XmlResolver = null;

            System.Xml.XmlReader xukDatasource = System.Xml.XmlReader.Create(XUKPath);

            x2z.WriteZed(xukDatasource);
            xukDatasource.Close();

            if (!System.IO.File.Exists(x2z.OuputDir + "/" + tmpNcxName) ||
                !System.IO.File.Exists(x2z.OuputDir + "/" + tmpPackageName))
            {
                throw new Exception(Localizer.Message("error_exporting_daisy"));
            }
        }
    }
}