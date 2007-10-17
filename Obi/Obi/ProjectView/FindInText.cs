using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace Obi.ProjectView
{
    /// <summary>
    /// Interface for all controls that have searchable text (strips, blocks, metadata panels so far.)
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// True if there is text that matches the search string.
        /// </summary>
        bool Matches(string search);

        /// <summary>
        /// Replace the text that matched the search string with the replace string.
        /// </summary>
        /// <remarks>Throw an exception if the search doesn't match.</remarks>
        void Replace(string search, string replace);
    }

    /*
     * Disclaimer: this is new and doesn't yet work as described below.
     * 
     * Press F3 to bring up the SearchText form
     * Type and press enter to start searching
     * F3 to search next
     * Shift-F3 to search previous
     * Esc to clear and close the form
     * 
     * As the search criteria is matched, the corresponding UI item is highlighted and played
     * */
    public partial class FindInText : UserControl
    {
        FlowLayoutPanel mStripsPanel;

        public FindInText()
        {
            InitializeComponent();
        }

        //eventually this will take as input:
        //the strips (or their view/panel)
        //the toc
        //the metadata
        //or this function will be collapsed into the SearchText constructor or something that assures that it
        //is called before the search starts.
        public void InitializeComponentsToSearch(FlowLayoutPanel stripsPanel)
        {
            mStripsPanel = stripsPanel;    
        }

        //when they press enter, start searching
        //press escape to close (although the scope of this should be wider than just the text box)
        private void mSearchString_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Return) startSearch(mSearchString.Text);
            if (e.KeyChar == (char)Keys.Escape) this.Visible = false;
        }

        //search the views
        //note that this actually searches the views, not the data model
        //it feels a bit weird (perhaps because it's so lightweight) but i think it's the right approach
        private void startSearch(String searchString)
        {
            bool found = false;

            foreach (Control c in mStripsPanel.Controls)
            {
               //todo: consider starting at the user's current selection

                //this is not case-sensitive
                if (c is Strip && ((Strip)c).Label.ToLowerInvariant().Contains(searchString.ToLowerInvariant()))
                {
                    //does this function work? 
                    ((Strip)c).Select();
                    found = true;
                    break;
                }
            }

            if (found)
            {
                //todo: change the menu item text from "Find" to "Find next" menu item
                //enable "Find previous"
                MessageBox.Show("Found it! Yay!");
                
            }
            else
            {
                //alert "not found", but perhaps more elegantly
                MessageBox.Show("Didn't find it :(");
            }
        }

        //when the form loads, focus on the text box
        private void SearchText_Load(object sender, EventArgs e)
        {
            mSearchString.Focus();
        }

        public void FindNext()
        {
            
        }

        public void FindPrevious()
        {
            
        }

        /// <summary>
        /// Try to match target string with search string.
        /// Do only exact match now, but should improve for caseless match, maybe regex?
        /// </summary>
        // This method is used by all Searchables to implement the string matching
        public static bool Match(string target, string search)
        {
            return target.Contains(search);
        }
    }
}
