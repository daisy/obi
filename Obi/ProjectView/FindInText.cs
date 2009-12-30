using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Obi.ProjectView
{
    /// <summary>
    /// Interface for all controls that have searchable text (strips, blocks, metadata panels so far.)
    /// </summary>
    public interface ISearchable
    {
        /// <summary>
        /// The string to match for this item.
        /// </summary>
        string ToMatch();
    }

    /// <summary>
    /// Find text in searchable controls (right now this means that we search strip titles, block annotations)
    /// </summary>
    /// <remarks>
    /// Press Control + F to bring up the FindInText form (F3 and Shift + F3 should also work)
    /// Type and press enter to start searching
    /// F3 to search next
    /// Shift-F3 to search previous
    /// Esc to clear and close the form; or wait for the form timeout (4 seconds).
    /// As the search criteria is matched, the corresponding UI item is selected and played
    /// </remarks>
    public partial class FindInText : UserControl
    {
        private ContentView mStripsView;
        private int mOriginalPosition;
        private int mNumberSearched;
        private bool mFoundFirst;
        private ProjectView mProjectView;
        private enum SearchDirection { NEXT, PREVIOUS };
        private SearchDirection mLastDirection;
        private Timer mTimer;
        private float mBaseFontSize;

        public FindInText()
        {
            mStripsView = null;
            mOriginalPosition = 0;
            mNumberSearched = 0;
            mProjectView = null;
            mFoundFirst = false;
            InitializeComponent();
            mBaseFontSize = Font.SizeInPoints;
            mTimer = new Timer();
            mTimer.Interval = 7000;
            mTimer.Tick += new EventHandler(mTimer_Tick);
        }

        /// <summary>
        /// Set the zoom factor for the search bar.
        /// </summary>
        public float ZoomFactor
        {
            set
            {
                /*if (value != 0.0)
                {
                    Font = new Font(Font.FontFamily, mBaseFontSize * value);
                }*/
            }
        }
      

        /// <summary>
        /// The parent project view. Should be set ASAP, and only once.
        /// </summary>
        public ProjectView ProjectView
        {
            set
            {
                if (mProjectView != null) throw new Exception("Cannot set the project view again!");
                mProjectView = value;
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
        public void StartNewSearch(ContentView strips)
        {
            mStripsView = strips;
            mProjectView.FindInTextVisible = true;
            mFoundFirst = false;
            mNumberSearched = 0;
            mProjectView.ObiForm.UpdateFindInTextMenuItems();
            mString.SelectAll();
            mString.Focus();
            mProjectView.ObiForm.Status(Localizer.Message("find_in_text_init"));
        }

        /// <summary>
        /// See if it's time to close the form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mTimer_Tick(object sender, EventArgs e)
        {
                   mTimer.Stop();
           mProjectView.FindInTextVisible = false;
           PlayFindBarCollapse ();
        }

        public void FindNext()
        {
            FindAnother(SearchDirection.NEXT);
        }
        public void FindPrevious()
        {
            FindAnother(SearchDirection.PREVIOUS);
        }

        /// <summary>
        /// Search for the first occurrence, starting at the current selection
        /// </summary>
        private void InitialSearch()
        {
            mOriginalPosition = GetCurrentIndex();
            mLastDirection = SearchDirection.NEXT;
            //if nothing is selected, start at the top
            if (mOriginalPosition == -1) mOriginalPosition = 0;
            Search(mOriginalPosition, mString.Text, SearchDirection.NEXT, true);
        }

        /// <summary>
        /// Search for the next or previous occurrence of the text.  
        /// </summary>
        private void FindAnother(SearchDirection dir)
        {
            if (!mFoundFirst)
            {
                InitialSearch();
            }
            else
            {
                if (!Visible)
                {
                    //debugging exception only!
                    //throw new Exception("Find next/previous not available: form is not being shown.");
                }
                if (mString.Text.Length == 0)
                {
                    mString.Focus();
                    mProjectView.ObiForm.Status(Localizer.Message("please_enter_some_text"));
                    return;
                }

                //this returns an index into the StripsView.Searchables collection
                int currentSelection = GetCurrentIndex();
                if (dir == SearchDirection.NEXT)
                {
                    currentSelection = GetNextIndex(currentSelection);
                    mProjectView.ObiForm.Status(Localizer.Message("find_next_in_text"));
                    mTimer.Stop ();
                    mTimer.Start ();
                }
                else
                {
                    currentSelection = GetPreviousIndex(currentSelection);
                    mProjectView.ObiForm.Status(Localizer.Message("find_prev_in_text"));
                    mTimer.Stop ();
                    mTimer.Start ();
                                    }
                Search(currentSelection, mString.Text, dir, false);
            }
        }

        /// <summary>
        /// Search from the starting point and continue in the specified direction
        /// </summary>
        /// <param name="startingPoint">index in Searchables of current position.  this must be valid.</param>
        /// <param name="searchString">what to search for</param>
        /// <param name="direction">NEXT or PREVIOUS</param>
        private void Search(int startingPoint, String searchString, SearchDirection direction, bool isInitialSearch)
        {
            if (startingPoint < 0 || startingPoint >= mStripsView.Searchables.Count)
                throw new Exception("Search index " + startingPoint + "out of bounds.  Min = 0, Max = " + mStripsView.Searchables.Count);
            if (mStripsView.Searchables.Count == 0)
            {
                mProjectView.ObiForm.Status(Localizer.Message("nothing_to_search"));
                return;
            }

            mProjectView.ObiForm.Status(Localizer.Message("searching"));

            //if we're switching direction, update the last position
            if (direction != mLastDirection)
            {
                mLastDirection = direction;
                mOriginalPosition = startingPoint;
                mNumberSearched = 0;
            }
            int startIndex = startingPoint;
            bool found = false;
            int SafetyCounter = 0;
            //there might be a way to wrangle Searchables.Find(...) to do the work for us with a Predicate, but
            //as there is no FindNext or FindPrevious (especially the latter), it seems like more work than it's worth
            while (found == false)
            {
                // added to prevent going into infinate loop
            SafetyCounter++;
            if (SafetyCounter > 2 * mStripsView.Searchables.Count)
                {
                mProjectView.ObiForm.Status ( Localizer.Message ( "finished_searching_all" ) );
                return;
                }
                        
                if (startIndex == mOriginalPosition && mNumberSearched == mStripsView.Searchables.Count)
                {
                    mProjectView.ObiForm.Status(Localizer.Message("finished_searching_all"));
                    mNumberSearched = 0;
                    return;
                }

                mNumberSearched++;
                string to_match = mString.Text.ToLowerInvariant();
                if (mStripsView.Searchables[startIndex].ToMatch().Contains(to_match))
                {
                    SetSelection(mStripsView.Searchables[startIndex]);
                    found = true;
                }
                else
                {
                    if (direction == SearchDirection.NEXT) startIndex = GetNextIndex(startIndex);
                    else if (direction == SearchDirection.PREVIOUS) startIndex = GetPreviousIndex(startIndex);
                }
            }

            if (found)
            {
                if (!mFoundFirst) mFoundFirst = true;
                mProjectView.ObiForm.Status(String.Format(Localizer.Message("found_in_text"), mString.Text));
            }
            else
            {
                mProjectView.ObiForm.Status(Localizer.Message("not_found_in_text"));
                //deselect
                mProjectView.Selection = null;
                mFoundFirst = false;
            }
            mProjectView.ObiForm.UpdateFindInTextMenuItems();
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
                InitialSearch();
            }
            //close the form
            else if (e.KeyCode == Keys.Escape)
            {
                mProjectView.FindInTextVisible = false;
                if (mProjectView.CanFocusOnTOCView) mProjectView.FocusOnTOCView ();
                else if (mProjectView.CanFocusOnContentView) mProjectView.FocusOnContentView ();
            }
            //find previous or next
            else if (e.KeyCode == Keys.F3)
            {
                if (CanFindNextPreviousText)
                {
                    if (Control.ModifierKeys == Keys.Shift) FindPrevious();
                    else FindNext();
                }
            }
            //maybe the user wants to start a new search
            else if (e.KeyCode == Keys.F && Control.ModifierKeys == Keys.Control)
            {
                StartNewSearch(mStripsView);
            }
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
            if (mString.Text == "") StartNewSearch(mStripsView);
        }

        /// <summary>
        /// Get the current selection and return its index in the Searchables collection
        /// </summary>
        /// <returns></returns>
        private int GetCurrentIndex()
        {
            if (mProjectView.Selection == null) return -1;
            //need an easy way to convert between NodeSelection and ISearchable
            //otherwise we break the genericity of ISearchable and write ugly code (see below)
            //for now, the easiest thing is look for something that is selected in the Searchables collection
            else
            {
                foreach (ISearchable c in mStripsView.Searchables)
                {
                    if (c is Strip && ((Strip)c).Highlighted) return mStripsView.Searchables.IndexOf((Strip)c);
                    else if (c is Block && ((Block)c).Highlighted) return mStripsView.Searchables.IndexOf((Block)c);
                }
            }
            return 0;

        }

        /// <summary>
        /// Get the previous index in Searchables (loop to the end)
        /// </summary>
        /// <param name="currentSelection"></param>
        /// <returns></returns>
        private int GetPreviousIndex(int currentSelection)
        {
            if (currentSelection <= 0) return mStripsView.Searchables.Count - 1;
            else return currentSelection-1;
        }
        /// <summary>
        /// Get the next index in Searchables (loop to the beginning)
        /// </summary>
        /// <param name="currentSelection"></param>
        /// <returns></returns>
        private int GetNextIndex(int currentSelection)
        {
            if (currentSelection >= mStripsView.Searchables.Count-1) return 0;
            else return currentSelection+1;
        }
        /// <summary>
        /// Set the selection in the ProjectView
        /// </summary>
        /// <param name="selection"></param>
        private void SetSelection(ISearchable selection)
        {
            //this breaks the genericity of ISearchable, but I don't know about (or I don't understand) generic selection in Obi
            if (selection is Block) mProjectView.SelectedBlockNode = ((Block)selection).Node;
            else if (selection is Strip) mProjectView.SelectedStripNode = ((Strip)selection).Node;
        }

        private void FindInText_Enter(object sender, EventArgs e)
        {
            mTimer.Stop();
        }

        private void FindInText_Leave(object sender, EventArgs e)
        {
            mTimer.Start();
        }

        private void mString_Enter(object sender, EventArgs e)
        {
            mTimer.Stop();
        }

        private void mPreviousButton_Click(object sender, EventArgs e)
        {
            mProjectView.FindPreviousInText();
        }

        private void mNextButton_Click(object sender, EventArgs e)
        {
            mProjectView.FindNextInText ();
        }

        private void mCloseButton_Click(object sender, EventArgs e)
        {
            mProjectView.FindInTextVisible = false;
        }

        private void FindInText_FontChanged(object sender, EventArgs e)
        {

        }

        private void FindInText_SizeChanged(object sender, EventArgs e)
        {

        }

        private void PlayFindBarCollapse ()
            {
            string FilePath = Path.Combine ( System.AppDomain.CurrentDomain.BaseDirectory, "FindBarCollapse.wav" );
            if ( mProjectView.ObiForm.Settings.AudioClues  &&   File.Exists ( FilePath ) )
                {
                System.Media.SoundPlayer voicePlayer = new System.Media.SoundPlayer ( FilePath );
                voicePlayer.Play ();
                }
            }
    }
}
