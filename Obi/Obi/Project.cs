using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class Project
    {
        private bool mUnsaved;    // true if the project was modified and not saved
        private string mXUKPath;  // path to the XUK file

        public bool Unsaved
        {
            get
            {
                return mUnsaved;
            }
        }

        public string XUKPath
        {
            get
            {
                return mXUKPath;
            }
        }

        /// <summary>
        /// Create a blank project.
        /// </summary>
        public Project()
        {
            mUnsaved = false;
        }

        /// <summary>
        /// Create a project from a XUK file.
        /// </summary>
        /// <param name="xukPath"></param>
        public Project(string xukPath)
        {
            mUnsaved = false;
            mXUKPath = xukPath;
        }

        public void Save()
        {
            mUnsaved = false;
        }

        public void Touch()
        {
            mUnsaved = true;
        }
    }
}
