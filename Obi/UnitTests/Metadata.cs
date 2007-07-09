using System;
using System.Collections.Generic;
using System.Text;
using Obi;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public class Metadata
    {
        [Test]
        public void XukVersion()
        {
            UserProfile profile = new UserProfile();
            profile.Culture = new System.Globalization.CultureInfo("en");
            profile.Name = "Obird";
            profile.Organization = "The Urakawa Project";
            Project project = Project.BlankProject(System.IO.Path.GetTempPath());
            project.Create(System.IO.Path.GetTempFileName(), "Test project", "test_id", profile, false);
            Assert.AreEqual(project.XukVersion, Project.CURRENT_XUK_VERSION);
        }
    }
}