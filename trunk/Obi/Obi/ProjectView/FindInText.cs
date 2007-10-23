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
     * Press F3 to bring up the FindInText form
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
        int mOriginalPosition;

        public FindInText()
        {
            mStripsPanel = null;
            mOriginalPosition = -1;
            InitializeComponent();
        }

        //this is the function that starts this widget
        //eventually, the parameter here should be a list of strips
        //or a list of ISearchable's.
        //we need to know the user's current selection too, and we need to be able to select a control
        public void ShowFindInText(FlowLayoutPanel stripsPanel)
        {
            //if this form is already being shown and has a search string, do a "Find Next"
            if (this.Visible && mString.Text.Length > 0) FindNextInText();
            else
            {
                System.Diagnostics.Trace.Write("Find in Text: starting search form\n");
                mStripsPanel = stripsPanel;
                this.Visible = true;
                mString.Focus();
            }
        }

        private void FindNextInText()
        {
            //there is a bug here because currentlyselectedstrip is always the one that was clicked, not the one that
            //had a search result in it.  when Strip.Select() works, it will be better.
            int currentSelection = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
            System.Diagnostics.Trace.Write("Find in Text: Find Next from index = " + currentSelection + "\n");
            search(GetIndexOfNextStrip(mStripsPanel, currentSelection), mString.Text, 1);
        }

        private void FindPreviousInText()
        {
            int currentSelection = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
            //start at the strip before our current strip
            int previousStrip = GetIndexOfPreviousStrip(mStripsPanel, currentSelection);
            System.Diagnostics.Trace.Write("Find in Text: Find Previous from index = " + currentSelection + "\n");
            search(GetIndexOfPreviousStrip(mStripsPanel, currentSelection), mString.Text, -1);
        }

        //search from starting point and continue in the specified direction (1 = forward, -1 = backwards)
        private void search(int startingPoint, String searchString, int direction)
        {
            int startIndex = startingPoint;
            if (startIndex == -1)
            {
                startIndex = GetIndexOfFirstStrip(mStripsPanel);
                if (startIndex == -1)
                {
                    System.Diagnostics.Trace.Write("Find in Text: Nothing to search\n");
                    return;
                }
            }

            System.Diagnostics.Trace.Write("Find in Text: Searching..., starting from " + startIndex + "\n");
         
            bool found = false;
            while (startIndex != -1 && found == false)
            {
                Control control = mStripsPanel.Controls[startIndex];
                //make sure that this is the type of control we want to search
                System.Diagnostics.Debug.Assert(MeetsCriteria(control));

                if (((ISearchable)control).Matches(mString.Text))
                {
                    //does this function work? 
                    ((Strip)control).Selected = true;
                    found = true;
                }
                else
                {
                    if (direction == 1) startIndex = GetIndexOfNextStrip(mStripsPanel, startIndex);
                    else if (direction == -1) startIndex = GetIndexOfPreviousStrip(mStripsPanel, startIndex);
                    //don't loop forever
                    if (startIndex == mOriginalPosition)
                    {
                        System.Diagnostics.Trace.Write("Find in Text: searched all text.  Stopping.\n");
                        break;
                    }
                    System.Diagnostics.Trace.Write("Find in Text: still looking.  Index = " + startIndex + "\n");
                }
            }

            if (found)
            {
                System.Diagnostics.Trace.Write("Find in Text: Found occurence at " + startIndex + "\n");
            }
            else
            {
                System.Diagnostics.Trace.Write("Find in Text: Not found or no more found.\n");
                //reset the original position to wherever we are now
                //so if the user presses F3, they can start searching again
                //anyway...i think this will work, but it needs to be tested
                mOriginalPosition = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
            }
        }

        private void mString_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(e);
        }
     
        //enter = search
        //f3 = search or search next
        //shift + f3 = search previous
        //escape = close
        private void ProcessKeys(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                mOriginalPosition = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
                search(mOriginalPosition, mString.Text, 1);
            }
            else if (e.KeyCode == Keys.Escape) this.Visible = false;
           
            else if (e.KeyCode == Keys.F3)
            {
                if (Control.ModifierKeys == Keys.Shift) FindPreviousInText();
                else FindNextInText();
            }
        }

        //does the widget meet our criteria for being a searchable widget?
        //this function is only temporary
        private static bool MeetsCriteria(Control control)
        {
            if (control != null && control is ISearchable && control is Strip) return true;
            else return false;
        }

        /// <summary>
        /// Try to match target string with search string.
        /// Do only case-insensitive match now, but should improve, perhaps with regex?
        /// </summary>
        // This method is used by all Searchables to implement the string matching
        //if this method grows, it should probably move out of this file.
        public static bool Match(string target, string search)
        {
            return target.ToLowerInvariant().Contains(search.ToLowerInvariant());
        }

        #region Indexing
        //get the index of the currently selected ISearchable item
        //ideally, we could count on ISearchable objects to also be index-able, so this function would return ISearchable instead
        private static int GetIndexOfCurrentlySelectedStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = 0; i < stripsPanel.Controls.Count; i++)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control) && ((Strip)control).Selected) return i;
            }
            return -1;
        }

        //get the index of the first ISearchable strip
        private static int GetIndexOfFirstStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = 0; i < stripsPanel.Controls.Count; i++)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            return -1;
        }
        //get the index of the last ISearchable strip
        private static int GetIndexOfLastStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = stripsPanel.Controls.Count - 1; i >= 0; i--)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            return -1;
        }

        //get the next ISearchable strip
        private int GetIndexOfNextStrip(FlowLayoutPanel stripsPanel, int startIndex)
        {
            for (int i = startIndex + 1; i < stripsPanel.Controls.Count; i++)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            //loop back to the beginning
            return GetIndexOfFirstStrip(stripsPanel);
        }

        //get the previous ISearchable strip
        private static int GetIndexOfPreviousStrip(FlowLayoutPanel stripsPanel, int startIndex)
        {
            for (int i = startIndex - 1; i >= 0; i--)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            //loop back to the end
            return GetIndexOfLastStrip(stripsPanel);
        }
    #endregion

    }
}
