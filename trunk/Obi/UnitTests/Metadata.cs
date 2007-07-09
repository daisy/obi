using System;
using System.IO;
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
            string title = "Test project";
            string id = "test_id";
            Project project = new Project(System.IO.Path.GetTempFileName(), title, id, profile, false);
            Assert.AreEqual(project.XukVersion, Project.CURRENT_XUK_VERSION);
            Assert.AreEqual(profile.Name, project.GetSingleMetadataItem(Obi.Metadata.DTB_NARRATOR).getContent());
            Assert.AreEqual(profile.Culture.ToString(),
                project.GetSingleMetadataItem(Obi.Metadata.DC_LANGUAGE).getContent());
            Assert.AreEqual(profile.Organization, project.GetSingleMetadataItem(Obi.Metadata.DC_PUBLISHER).getContent());
            Assert.AreEqual(id, project.GetSingleMetadataItem(Obi.Metadata.DC_IDENTIFIER).getContent());
            Assert.AreEqual(title, project.Title);
        }
    }
}