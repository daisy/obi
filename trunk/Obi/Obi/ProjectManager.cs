using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class ProjectManager
    {
        private static readonly ProjectManager mInstance = new ProjectManager();
        private Project mProject;

        public static ProjectManager Instance
        {
            get
            {
                return mInstance;
            }
        }

        public Project Project
        {
            get
            {
                return mProject;
            }
        }

        /// <summary>
        /// True if there are unsaved changes in the project.
        /// </summary>
        public bool Unsaved
        {
            get
            {
                return mProject != null && mProject.Unsaved;
            }
        }

        private ProjectManager()
        {
            mProject = null;
        }

        /// <summary>
        /// Create a new project and replace the old one with the new one.
        /// </summary>
        public void CreateProject()
        {
            mProject = new Project();
        }

        /// <summary>
        /// Open a XUK file and create a new project object from it.
        /// Throw an exception if there is an error reading the file.
        /// </summary>
        /// <param name="path">Complete path of the XUK file.</param>
        public void OpenXUK(string path)
        {
            mProject = new Project(path);
            Console.WriteLine("Open XUK file {0}.", path);
        }
    }
}