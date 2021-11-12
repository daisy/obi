using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using urakawa.daisy;
using urakawa.command;

namespace Obi.ImportExport
{
    public class ImportMetadata
    {
        private Obi.ProjectView.ProjectView m_ProjectView;
        private string m_metadataFieldsNotImported = string.Empty;
        
        public bool ImportFromCSVFile(string CSVFullPath, ObiPresentation presentation, Obi.ProjectView.ProjectView projectView = null)
        {
            List<string> nameList = new List<string>();
            List<string> content = new List<string>();
            bool result = ReadListsFromCSVFile(nameList, content, CSVFullPath, presentation, projectView);
            return result;
        }

        private bool ReadListsFromCSVFile(List<string> nameList, List<string> content, string CSVFullPath, ObiPresentation presentation, Obi.ProjectView.ProjectView projectView)
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
            CompositeCommand command =
                        presentation.CreateCompositeCommand(Localizer.Message("modify_metadata_entry"));

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

                if (cellsInLineArray.Length >= 2 && !string.IsNullOrWhiteSpace(cellsInLineArray[0]) && !string.IsNullOrWhiteSpace(cellsInLineArray[1]))
                {
                    if (projectView == null)
                        presentation.SetSingleMetadataItem(cellsInLineArray[0], cellsInLineArray[1]);
                    else if (projectView != null)
                    {
                        if (projectView.CanAddMetadataEntry())
                        {
                            bool entryModified = false;
                            foreach (urakawa.metadata.Metadata entry in presentation.GetMetadata(cellsInLineArray[0]))
                            {
                                if (cellsInLineArray[0].ToLower() == "dtb:produceddate" || cellsInLineArray[0].ToLower() == "dtb:revisiondate" || cellsInLineArray[0].ToLower() == "dc:date" || cellsInLineArray[0].ToLower() == "dtb:sourcedate")
                                {
                                    string dateFormat = GetDate(cellsInLineArray[0], cellsInLineArray[1]);
                                    cellsInLineArray[1] = dateFormat;
                                }
                                if (cellsInLineArray[0].ToLower() == "custom")
                                {
                                    m_metadataFieldsNotImported += "\n\n" + "-> " + cellsInLineArray[0] + " " + Localizer.Message("Metadata_NameCannotBeCustom");
                                }
                                else
                                {
                                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Metadata.ModifyContent(projectView, entry, cellsInLineArray[1]));
                                }
                                entryModified = true;
                            }
                            if (!entryModified)
                            {
                                if(Metadata.DAISY3MetadataNames.Contains(cellsInLineArray[0],StringComparer.OrdinalIgnoreCase))
                                {
                                    foreach (string metadataName in Metadata.DAISY3MetadataNames)
                                    {
                                        if (metadataName.IndexOf(cellsInLineArray[0], StringComparison.OrdinalIgnoreCase) >= 0)
                                        {
                                            cellsInLineArray[0] = metadataName;
                                        }
                                    }
                                    cellsInLineArray[0]  = Metadata.DAISY3MetadataNames.ElementAt(Metadata.DAISY3MetadataNames.IndexOf(cellsInLineArray[0]));
                                }
                                if (cellsInLineArray[0].ToLower() == "custom")
                                {
                                    m_metadataFieldsNotImported += "\n\n" + "-> " + cellsInLineArray[0] + " " + Localizer.Message("Metadata_NameCannotBeCustom");
                                }
                                else
                                {
                                    Commands.Metadata.AddEntry cmd = new Commands.Metadata.AddEntry(projectView, cellsInLineArray[0]);
                                    command.ChildCommands.Insert(command.ChildCommands.Count, cmd);
                                    if (cellsInLineArray[0] == "dtb:producedDate" || cellsInLineArray[0] == "dtb:revisionDate" || cellsInLineArray[0] == "dc:Date" || cellsInLineArray[0] == "dtb:sourceDate")
                                    {
                                        cellsInLineArray[1] = GetDate(cellsInLineArray[0], cellsInLineArray[1]);
                                    }
                                    command.ChildCommands.Insert(command.ChildCommands.Count, new Commands.Metadata.ModifyContent(projectView, cmd.Entry, cellsInLineArray[1]));
                                }
                            }
                        }
                    }
                }

            }
            if (command.ChildCommands.Count > 0) presentation.UndoRedoManager.Execute(command);
            if (m_metadataFieldsNotImported != string.Empty)
            {
                MessageBox.Show(String.Format(Localizer.Message("Metadata_NotImported"), m_metadataFieldsNotImported), Localizer.Message("Metadata_Imported"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(String.Format(Localizer.Message("Metadata_ImportedSuccessfully"), m_metadataFieldsNotImported), Localizer.Message("Metadata_Imported"),
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }


            return true;
        }
        private string GetDate(string entryName, string entryValue)
        {
            string strYearVal = entryValue;
            int yearOnlyDate = 0;
            int.TryParse(strYearVal, out yearOnlyDate);
            if (yearOnlyDate > 0 && strYearVal.Length == 4)
            {
                return strYearVal;
            }
            try
            {
                DateTime dateEntry = DateTime.Parse(entryValue);
                string strDate = dateEntry.Year.ToString() + "-" + AdjustSingleDigitDate(dateEntry.Month.ToString()) + "-" + AdjustSingleDigitDate(dateEntry.Day.ToString());
                return strDate;
            }
            catch (System.FormatException ex)
            {

                m_metadataFieldsNotImported += "\n\n" + "-> " + entryName + " " + Localizer.Message("Metadata_InvalidDateFormat");
                return "";
            }
            catch (System.Exception ex)
            {
                m_metadataFieldsNotImported += "\n\n" + "-> " + entryName + " " + Localizer.Message("MetadataView_InvalidMetadata")  +"\n\n" + ex.ToString();
                return "";
            }

        }
        private string AdjustSingleDigitDate(string dateFragment)
        {
            if (dateFragment.Length == 1)
                return string.Concat("0", dateFragment);
            else
                return dateFragment;
        }


    }
}
