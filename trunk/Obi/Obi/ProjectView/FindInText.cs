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

  
    /// <summary>
    /// Find text in searchable controls (right now this means that we search strip titles, block annotations)
    /// </summary>
    /// <remarks>
    /// Press Control + F to bring up the FindInText form (F3 and Shift + F3 should also work)
    /// Type and press enter to start searching
    /// F3 to search next
    /// Shift-F3 to search previous
    /// Esc to clear and close the form; or wait for the form timeout.
    /// As the search criteria is matched, the corresponding UI item is selected and played
    /// </remarks>
    public partial class FindInText : UserControl
    {
        FlowLayoutPanel mStripsPanel;
        int mOriginalPosition;
        bool mFoundFirst;
        private ProjectView mView;  // the parent project view
     
        public FindInText()
        {
            mStripsPanel = null;
            mOriginalPosition = -1;
            mView = null;
            mFoundFirst = false;
            InitializeComponent();
        }

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mView != null) throw new Exception("Cannot set the project view again!");
                mView = value;
            }
        }

        /// <summary>
        /// Say whether next and previous text searching is allowed
        /// </summary>
        public bool CanFindNextPreviousText
        {
            get { return mFoundFirst; }
        }

        /// <summary>
        /// This function displays the find in text form.
        /// </summary>
        public void StartNewSearch(FlowLayoutPanel strips)
        {
            mStripsPanel = strips;
            mView.ObiForm.Status(Localizer.Message("find_in_text_init"));
            mView.FindInTextVisible = true;
            mFoundFirst = false;
            mView.ObiForm.UpdateFindInTextMenuItems();
            mString.SelectAll();
            mString.Focus();
        }

        /// <summary>
        /// Search for the next occurrence of the text.  
        /// </summary>
        public void FindNext()
        {
            if (!Visible || !mFoundFirst)
            {
                //debugging exception only!
                throw new Exception("Find next not available; form is not being shown or text was never found in the first place.");
            }
            if (mString.Text.Length == 0)
            {
                mString.Focus();
                mView.ObiForm.Status(Localizer.Message("please_enter_some_text"));
                return;
            }
            int currentSelection = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
            mView.ObiForm.Status(Localizer.Message("find_next_in_text"));
            search(GetIndexOfNextStrip(mStripsPanel, currentSelection), mString.Text, 1);
        }

        /// <summary>
        /// Search for the previous occurrence of the text
        /// </summary>
        public void FindPrevious()
        {
            if (!Visible || !mFoundFirst)
            {
                //debugging exception only!
                throw new Exception("Find previous not available; form is not being shown or text was never found in the first place.");
            }
            if (mString.Text.Length == 0)
            {
                mString.Focus();
                mView.ObiForm.Status(Localizer.Message("please_enter_some_text"));
                return;
            }

            int currentSelection = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
            //start at the strip before our current strip
            int previousStrip = GetIndexOfPreviousStrip(mStripsPanel, currentSelection);
            mView.ObiForm.Status(Localizer.Message("find_prev_in_text"));
            search(GetIndexOfPreviousStrip(mStripsPanel, currentSelection), mString.Text, -1);
        }

        /// <summary>
        /// Search from the starting point and continue in the specified direction
        /// </summary>
        /// <param name="startingPoint">index of current position</param>
        /// <param name="searchString">what to search for</param>
        /// <param name="direction">1 = forward, -1 = backwards</param>
        private void search(int startingPoint, String searchString, int direction)
        {
            int startIndex = startingPoint;
            if (startIndex == -1)
            {
                startIndex = GetIndexOfFirstStrip(mStripsPanel);
                if (startIndex == -1)
                {
                    mView.ObiForm.Status(Localizer.Message("nothing_to_search"));
                    return;
                }
            }

            mView.ObiForm.Status(Localizer.Message("searching"));
         
            bool found = false;
            while (startIndex != -1 && found == false)
            {
                Control control = mStripsPanel.Controls[startIndex];
                //make sure that this is the type of control we want to search
                System.Diagnostics.Debug.Assert(MeetsCriteria(control));

                if (((ISearchable)control).Matches(mString.Text))
                {
                    mView.SelectedStripNode = ((Strip)control).Node;
                    found = true;
                }
                else
                {
                    if (direction == 1) startIndex = GetIndexOfNextStrip(mStripsPanel, startIndex);
                    else if (direction == -1) startIndex = GetIndexOfPreviousStrip(mStripsPanel, startIndex);
                }

                //don't loop forever
                if (startIndex == mOriginalPosition) break;
            }

            if (found)
            {
                if (!mFoundFirst) mFoundFirst = true;
                mView.ObiForm.Status(String.Format(Localizer.Message("found_in_text"), mString.Text));
            }
            else
            {
                if (startIndex == mOriginalPosition) mView.ObiForm.Status(Localizer.Message("finished_searching_all"));
                else mView.ObiForm.Status(Localizer.Message("not_found_in_text"));
                //reset the original position to wherever we are now
                mOriginalPosition = startIndex;
                // also deselect
                mView.Selection = null;
            }
            mView.ObiForm.UpdateFindInTextMenuItems();
        }

        /// <summary>
        /// Catch keypresses in the text field
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mString_KeyDown(object sender, KeyEventArgs e)
        {
            ProcessKeys(e);
        }
     
        /// <summary>
        /// process key presses
        /// </summary>
        /// <param name="e"></param>
        private void ProcessKeys(KeyEventArgs e)
        {
            //start the search
            if (e.KeyCode == Keys.Return)
            {
                mOriginalPosition = GetIndexOfCurrentlySelectedStrip(mStripsPanel);
                //if nothing is selected, start at the top
                if (mOriginalPosition == -1) mOriginalPosition = GetIndexOfFirstStrip(mStripsPanel);
                search(mOriginalPosition, mString.Text, 1);
            }
            //close the form
            else if (e.KeyCode == Keys.Escape)
            {
                mView.FindInTextVisible = false;
            }
            //find previous or next
            else if (e.KeyCode == Keys.F3)
            {
                if (Control.ModifierKeys == Keys.Shift) FindPrevious();
                else FindNext();
            }
            //maybe the user wants to start a new search
            else if (e.KeyCode == Keys.F && Control.ModifierKeys == Keys.Control)
            {
                StartNewSearch(mStripsPanel);
            }
        }

        /// <summary>
        /// Test if the widget meets our criteria for being a searchable widget
        /// </summary>
        /// <param name="control"></param>
        /// <returns></returns>
        private static bool MeetsCriteria(Control control)
        {
            if (control != null && control is ISearchable && control is Strip) return true;
            else return false;
        }

        /// <summary>
        /// Try to match target string with search string.
        /// Do only case-insensitive match now, but should improve, perhaps with regex?
        /// </summary>
        /// <remarks>
        /// This method is used by all ISearchables to implement the string matching
        /// </remarks>
        public static bool Match(string target, string search)
        {
            return target.ToLowerInvariant().Contains(search.ToLowerInvariant());
        }


        /// <summary>
        /// event raised when the text on the form changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mString_TextChanged(object sender, EventArgs e)
        {
            //if the text has been cleared, pretend it's a new search
            if (mString.Text == "") StartNewSearch(mStripsPanel);
        }

        #region Indexing

        /// <summary>
        /// get the index of the currently selected ISearchable item
        /// </summary>
        /// <param name="stripsPanel"></param>
        /// <returns></returns>
        ///ideally, we could count on ISearchable objects to also be index-able, so this function would return ISearchable instead
        private static int GetIndexOfCurrentlySelectedStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = 0; i < stripsPanel.Controls.Count; i++)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control) && ((Strip)control).Selected) return i;
            }
            return -1;
        }

        /// <summary>
        /// get the index of the first ISearchable strip
        /// </summary>
        /// <param name="stripsPanel"></param>
        /// <returns></returns>
        private static int GetIndexOfFirstStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = 0; i < stripsPanel.Controls.Count; i++)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            return -1;
        }
        
        /// <summary>
        /// get the index of the last ISearchable strip
        /// </summary>
        /// <param name="stripsPanel"></param>
        /// <returns></returns>
        private static int GetIndexOfLastStrip(FlowLayoutPanel stripsPanel)
        {
            for (int i = stripsPanel.Controls.Count - 1; i >= 0; i--)
            {
                Control control = stripsPanel.Controls[i];
                if (MeetsCriteria(control)) return i;
            }
            return -1;
        }
        
        /// <summary>
        /// get the next ISearchable strip
        /// </summary>
        /// <param name="stripsPanel"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
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

        /// <summary>
        /// get the previous ISearchable strip
        /// </summary>
        /// <param name="stripsPanel"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
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
