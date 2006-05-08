using System;
using System.Collections.Generic;
using System.Text;

namespace Obi
{
    public class ProjectManager
    {
        private static readonly ProjectManager mInstance = new ProjectManager();

        public static Localizer Instance
        {
            get
            {
                return mInstance;
            }
        }

        private ProjectManager()
        {
        }
    }
}