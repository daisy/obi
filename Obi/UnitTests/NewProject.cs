using System;
using System.Globalization;
using System.IO;
using NUnit.Framework;
using Obi;
using urakawa;
using urakawa.property.channel;

namespace UnitTests
{
    [TestFixture]
    public class NewProject
    {
        [Test]
        public void Factories()
        {
            Obi.Project project = CreateProject(false);
            /*Presentation presentation = project.getPresentation(0);
            Assert.AreSame(presentation, presentation.getTreeNodeFactory().getPresentation());
            Assert.AreSame(presentation, presentation.getPropertyFactory().getPresentation());*/
        }

        [Test]
        public void TitleSection()
        {
            Obi.Project project = CreateProject(true);
            Assert.IsTrue(project.RootNode is RootNode);
            Assert.IsTrue(project.RootNode.getChildCount() == 1);
            Assert.IsTrue(project.RootNode.getChild(0) is SectionNode);
            Assert.IsTrue(project.RootNode.SectionChild(0).Label == TITLE);
            Assert.IsTrue(project.RootNode.SectionChild(0).getChildCount() == 0);
        }

        [Test]
        public void Channels()
        {
            Obi.Project project = CreateProject(false);
            Assert.IsInstanceOfType(typeof(Channel), project.AnnotationChannel);
            Assert.IsInstanceOfType(typeof(Channel), project.AudioChannel);
            Assert.IsInstanceOfType(typeof(Channel), project.TextChannel);
        }

        private static readonly string TITLE = "Title test";

        /// <summary>
        /// Set up a project for further tests.
        /// </summary>
        /// <param name="createTitleSection">Create a title section or not.</param>
        /// <returns>The created project.</returns>
        private static Obi.Project CreateProject(bool createTitleSection)
        {
            /*UserProfile profile = new UserProfile();
            profile.Culture = new CultureInfo("en");
            profile.Name = "Obird";
            profile.Organization = "The Urakawa Project";
            Obi.Project project = new Obi.Project(Path.GetTempFileName());
            project.Initialize(TITLE, "test_id", profile, createTitleSection);
            return project;*/
            return null;
        }
    }
}
