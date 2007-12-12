using System;
using System.Collections.Generic;
using System.Windows.Forms;
using urakawa.core;

namespace Obi.Dialogs
{
    /// <summary>
    /// Dialog to select a page number in the project.
    /// </summary>
    public partial class GoToPage : Form
    {
        private List<PhraseNode> mPages;  // the list of pages in the project
        public PhraseNode SelectedPage;   // the one that was selected

        /// <summary>
        /// Simple constructor used by the designer. Use the one with a project argument.
        /// </summary>
        public GoToPage()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Create a dialog for the pages of a project.
        /// </summary>
        /// <param name="project">The project to select a page in.</param>
        public GoToPage(Presentation presentation)
        {
            InitializeComponent();
            SelectedPage = null;
        }

        /// <summary>
        /// Commit the selection.
        /// </summary>
        private void mOKButton_Click(object sender, EventArgs e)
        {
            if (mPagesComboBox.SelectedIndex >= 0 && mPagesComboBox.SelectedIndex < mPages.Count)
                SelectedPage = mPages[mPagesComboBox.SelectedIndex];
        }
    }
}