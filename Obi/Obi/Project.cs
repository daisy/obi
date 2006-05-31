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
        /// <param name="path">Path of the directory in which to create the project.</param>
        /// <param name="title">Title of the project.</param>
        public Project(string title, string path)
        {
            mUnsaved = false;
            mTitle = title;
            mXUKPath = path;
        }

        /// <summary>
        /// Open a project from a XUK file.
        /// </summary>
        /// <param name="path">XUK file path.</param>
        /// <returns>The project object created from this file.</returns>
        public static Project Open(string path)
        {
            return new Project(System.IO.Path.GetFileNameWithoutExtension(path), path);
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

        /// <summary>
        /// Get a short name from a given title. Usable for XUK file and project directory filename.
        /// </summary>
        /// <param name="title">Complete title.</param>
        /// <returns>The short version.</returns>
        public static string ShortName(string title)
        {
            return System.Text.RegularExpressions.Regex.Replace(title, @"[^a-zA-Z0-9_]", "_");
        }
    }
}
