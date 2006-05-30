using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class Project
    {
        private bool mUnsaved;    // true if the project was modified and not saved
        private string mXUKPath;  // path to the XUK file
        private string mTitle;    // temporary title (from file name)

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

        public string Title
        {
            get
            {
                return mTitle;
            }
        }

        /// <summary>
        /// Create a project from a XUK file.
        /// </summary>
        /// <param name="xukPath"></param>
        public Project(string xukPath)
        {
            mUnsaved = false;
            mXUKPath = xukPath;
            mTitle = xukPath == null ? "(Untitled project)" : System.IO.Path.GetFileNameWithoutExtension(xukPath);
        }

        /// <summary>
        /// Save the project.
        /// </summary>
        public void Save()
        {
            mUnsaved = false;
        }

        /// <summary>
        /// Make a copy of this project and save it under a new name.
        /// </summary>
        /// <param name="path">The path of the new project.</param>
        public void SaveAs(string path)
        {
        }

        /// <summary>
        /// Pretend that the project was modified.
        /// </summary>
        public void Touch()
        {
            mUnsaved = true;
        }

        /// <summary>
        /// Set the XUK path if and only if it was not set before.
        /// </summary>
        /// <param name="path">The new XUK path.</param>
        /// <returns>True if the path was actually set.</returns>
        public bool SetXukPath(string path)
        {
            if (mXUKPath == null)
            {
                mXUKPath = path;
                mTitle = System.IO.Path.GetFileNameWithoutExtension(path);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
