using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Obi
{
    class IOUtils
    {

        /// <summary>
        /// Check if a string representation of a directory 
        /// exists as a directory on the filesystem,
        /// if not, try to create it, asking the user first.
        /// </summary>
        /// <param name="dirPath">String representation of the directory to be checked/created</param>
        /// <returns>True if the directory exists or was successfully created, false otherwise.</returns>        
        
        public static bool ValidateAndCreateDir(string dirPath) {
            if (File.Exists(dirPath))
            {                
                return false;
            }
            
            if (!Directory.Exists(dirPath))
            {
                DialogResult result = MessageBox.Show(
                        String.Format(Localizer.Message("create_directory_query"), dirPath),
                        String.Format(Localizer.Message("create_directory_caption")),
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(dirPath);                        
                        return true;
                    }
                    catch (Exception x)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("create_directory_failure"), dirPath, x.Message),
                          String.Format(Localizer.Message("error")),
                          MessageBoxButtons.OK, MessageBoxIcon.Error);                            
                        return false;
                    }
                } //if (result == DialogResult.Yes)
                return false;
            } //if (!Directory.Exists(dirPath))
            return true;
        } //private void CheckAndCreateDir
    }
}
