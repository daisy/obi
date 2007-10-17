using System;
using System.Windows.Forms;

namespace Obi.Commands.Strips
{
    class ImportPhrases: Command
    {
        public ImportPhrases(ProjectView.ProjectView view, string[] filenames)
            : base(view)
        {
        }

        public static string[] SelectPhrases()
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = Localizer.Message("audio_file_filter");
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileNames : null;
        }

        /*
                foreach (string path in dialog.FileNames)
                {
                    try
                    {
                        mProjectPanel.Project.DidAddPhraseFromFile(path, insert.node, insert.index);
                        ++insert.index;
                    }
                    catch (Exception)
                    {
                        MessageBox.Show(String.Format(Localizer.Message("import_phrase_error_text"), path),
                            Localizer.Message("import_phrase_error_caption"),
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
                // select the first added phrase
                //mProjectPanel.CurrentSelection = new NodeSelection(insert.node, this);
            }
        }
         */
    }
}
