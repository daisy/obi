using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public enum PageKind { Front, Normal, Special };

    public class PageNumber
    {
        private PageKind mKind;
        private int mNumber;
        private string mLabel;

        public PageNumber(int number, PageKind kind)
        {
            if (mKind == PageKind.Special) throw new Exception("Special pages have a label, not a number.");
            mNumber = number;
            mKind = kind;
            mLabel = null;            
        }

        public PageNumber(int number) : this(number, PageKind.Normal) { }

        public PageNumber(string label)
        {
            mKind = PageKind.Special;
            mLabel = label;
            mNumber = 0;
        }


        /// <summary>
        /// Get the number as an arabic number or an unquoted label.
        /// </summary>
        public string ArabicNumberOrLabel { get { return mKind == PageKind.Special ? mLabel : mNumber.ToString(); } }

        /// <summary>
        /// Clone the page number.
        /// </summary>
        public PageNumber Clone() { return mKind == PageKind.Special ? new PageNumber(mLabel) : new PageNumber(mNumber, mKind); }

        /// <summary>
        /// Get the kind of the page.
        /// </summary>
        public PageKind Kind { get { return mKind; } }

        /// <summary>
        /// Make a new page number following this one.
        /// </summary>
        public PageNumber NextPageNumber() 
        {
            return mKind == PageKind.Special ? new PageNumber(mLabel + "*") : new PageNumber(mNumber + 1, mKind); 
        }

        /// <summary>
        /// Get the actual page number.
        /// </summary>
        public int Number { get { return mNumber; } }

        /// <summary>
        /// Get the display string for the number (i.e. arabic, roman or label.)
        /// Label is quoted.
        /// </summary>
        public override string ToString()
        {
            return mKind == PageKind.Front ? ToRoman(mNumber) :
                mKind == PageKind.Normal ? mNumber.ToString() :
                string.Format("\"{0}\"", mLabel);
        }

        /// <summary>
        /// Unquoted text version of the page number (for DAISY export.)
        /// </summary>
        public string Unquoted
        {
            get
            {
                return mKind == PageKind.Front ? ToRoman(mNumber) :
                    mKind == PageKind.Normal ? mNumber.ToString() :
                    mLabel;
            }
        }

        // Convert a number to roman numerals (lowercase)
        private static string ToRoman(int n)
        {
            if (n <= 0) throw new Exception("Number must be greater than 0.");
            string roman = "";
            // Thousands
            while (n >= 1000)
            {
                roman += "m";
                n -= 1000;
            }
            // Hundreds
            if (n >= 900)
            {
                roman += "cm";
                n -= 900;
            }
            else if (n >= 500)
            {
                roman += "d";
                n -= 500;
            }
            else if (n >= 400)
            {
                roman += "cd";
                n -= 400;
            }
            while (n >= 100)
            {
                roman += "c";
                n -= 100;
            }
            // Dozens
            if (n >= 90)
            {
                roman += "xc";
                n -= 90;
            }
            else if (n >= 50)
            {
                roman += "l";
                n -= 50;
            }
            else if (n >= 40)
            {
                roman += "xl";
                n -= 40;
            }
            while (n >= 10)
            {
                roman += "x";
                n -= 10;
            }
            // Units
            if (n >= 9)
            {
                roman += "ix";
                n -= 9;
            }
            else if (n >= 5)
            {
                roman += "v";
                n -= 5;
            }
            else if (n >= 4)
            {
                roman += "iv";
                n -= 4;
            }
            while (n >= 1)
            {
                roman += "i";
                --n;
            }
            return roman;
        }
    }
}
