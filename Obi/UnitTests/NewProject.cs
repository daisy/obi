using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Obi;
using urakawa;

namespace UnitTests
{
    [TestFixture]
    public class NewProject
    {
        [Test]
        public void Factories()
        {
            Obi.Project project = new Obi.Project(Path.GetTempFileName());
            Presentation presentation = project.getPresentation();
            Assert.AreSame(presentation, presentation.getTreeNodeFactory().getPresentation());
            Assert.AreSame(presentation, presentation.getPropertyFactory().getPresentation());
        }

        [Test]
        public void TitleSection()
        {
            UserProfile profile = new UserProfile();
            profile.Culture = new CultureInfo("en");
            profile.Name = "Obird";
            profile.Organization = "The Urakawa Project";
            string title = "Title test";
            Obi.Project project = new Obi.Project(System.IO.Path.GetTempFileName(), title, "test_id", profile, true);
            Assert.IsTrue(project.RootNode is RootNode);
        }
    }
}
