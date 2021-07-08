using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using urakawa.daisy;

namespace Obi.ImportExport
{
    public class ImportMetadata
    {
        
        public bool ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation)
        {
            List<string> nameList = new List<string>();
            List<string> content = new List<string>();
            bool result = ReadListsFromCSVFile(nameList, content, CSVFullPath, presentation);
            return result;
        }

        private bool ReadListsFromCSVFile(List<string> nameList, List<string> content, string CSVFullPath,ObiPresentation presentation)
        {
            string[] linesInFiles;
            try
            {

                linesInFiles = File.ReadAllLines(CSVFullPath, Encoding.Default);
            }
            catch (IOException e)
            {
                MessageBox.Show(Localizer.Message("FileInUse"), Localizer.Message("Caption_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            catch (Exception e)
            {
                return false;
            }

            foreach (string line in linesInFiles)
            {
                string[] cellsInLineArray = null;
                if (Path.GetExtension(CSVFullPath).ToLower() == ".csv")
                {
                    cellsInLineArray = line.Split(',');
                }
                else
                {
                    cellsInLineArray = line.Split('\t');
                }

                if(cellsInLineArray.Length >= 2)
                presentation.SetSingleMetadataItem(cellsInLineArray[0], cellsInLineArray[1]);

            }

            return true;
        }


    }
}
