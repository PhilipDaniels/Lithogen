using System;
using System.IO;
using Lithogen.Core;
using Lithogen.Engine.Implementations;
using NUnit.Framework;

namespace Lithogen.Engine.Tests.ImplementationTests
{
    [TestFixture]
    public class RebaserTests
    {
        const string PROJDIR = @"C:\mysite";
        const string PROJFILE = @"C:\mysite\mysite.csproj";
        Settings TheSettings;
        Rebaser Rebaser;

        [TestFixtureSetUp]
        public void MakeRebaser()
        {
            // Create a rebaser for
            TheSettings = new Settings();
            TheSettings.ProjectFile = PROJFILE;
            TheSettings.LithogenWebsiteDirectory = Path.Combine(PROJDIR, @"\bin\Debug\Lithogen.Website");
            TheSettings.ViewsDirectory = Path.Combine(PROJDIR, @"views");
            TheSettings.ContentDirectory = Path.Combine(PROJDIR, @"content");
            TheSettings.ScriptsDirectory = Path.Combine(PROJDIR, @"scripts");
            TheSettings.ImagesDirectory = Path.Combine(PROJDIR, @"images");

            Rebaser = new Rebaser(TheSettings);
        }

        [Test]
        public void Ctor_WhenPassedNullSettings_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new Rebaser(null));
        }

        [Test]
        public void GetPathToRoot_WhenGivenNullContainingFilename_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Rebaser.GetPathToRoot(null));
        }

        [Test]
        public void GetPathToRoot_WhenGivenWhitespaceFilename_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Rebaser.GetPathToRoot(" "));
        }

        [Test]
        public void GetPathToRoot_WhenGivenFileOutsideKnownDirectories_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Rebaser.GetPathToRoot(@"c:\blah.txt"));
            Assert.Throws<ArgumentException>(() => Rebaser.GetPathToRoot(Path.Combine(PROJDIR, @"somewhere\foo.txt")));
        }

        [Test]
        public void GetPathToRoot_WhenGivenFileInViewRoot_ReturnsSingleDot()
        {
            string filename = Path.Combine(TheSettings.ViewsDirectory, @"foo.txt");
            string result = Rebaser.GetPathToRoot(filename);
            Assert.AreEqual("./", result);
        }

        [Test]
        public void GetPathToRoot_WhenGivenFileInViewSubFolder_ReturnsOneParentDirectory()
        {
            string filename = Path.Combine(TheSettings.ViewsDirectory, @"sub\foo.txt");
            string result = Rebaser.GetPathToRoot(filename);
            Assert.AreEqual("../", result);
        }

        [Test]
        public void GetPathToRoot_WhenGivenFileInViewSubSubFolder_ReturnsTwoParentDirectories()
        {
            string filename = Path.Combine(TheSettings.ViewsDirectory, @"sub1\sub2\foo.txt");
            string result = Rebaser.GetPathToRoot(filename);
            Assert.AreEqual("../../", result);
        }


        [Test]
        public void GetPathToRoot_WhenGivenFileInContentRoot_ReturnsOneParentDirectory()
        {
            string filename = Path.Combine(TheSettings.ContentDirectory, @"foo.css");
            string result = Rebaser.GetPathToRoot(filename);
            Assert.AreEqual("../", result);
        }

        [Test]
        public void GetPathToRoot_WhenGivenFileInContentSubFolder_ReturnsTwoParentDirectories()
        {
            string filename = Path.Combine(TheSettings.ContentDirectory, @"sub\foo.css");
            string result = Rebaser.GetPathToRoot(filename);
            Assert.AreEqual("../../", result);
        }
   
        [Test]
        public void ReplaceRootsInFile_WhenGivenNullContainingFilename_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Rebaser.ReplaceRootsInFile(null, "a"));
        }

        [Test]
        public void ReplaceRootsInFile_WhenGivenWhitespaceFilename_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Rebaser.ReplaceRootsInFile(" ", "a"));
        }

        [Test]
        public void ReplaceRootsInFile_WhenGivenNullContents_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Rebaser.ReplaceRootsInFile("file.txt", null));
        }

        [Test]
        public void ReplaceRootsInFile_WhenGivenEmptyContents_ReturnsContentsUnchanged()
        {
            string filename = Path.Combine(TheSettings.ViewsDirectory, "foo.html");
            string contents = "";
            string result = Rebaser.ReplaceRootsInFile(filename, contents);
            Assert.AreEqual(contents, result);
        }

        [Test]
        public void ReplaceRootsInFile_WhenGivenContentsThatDoesNotContainMarker_ReturnsContentsUnchanged()
        {
            string filename = Path.Combine(TheSettings.ViewsDirectory, "foo.html");
            string contents = "<html><body>My body.</body></html>";
            string result = Rebaser.ReplaceRootsInFile(filename, contents);
            Assert.AreEqual(contents, result);
        }

        [Test]
        public void ReplaceRootsInFile_WhenGivenContentsThatContainsMarker_ReplacesMarkerWithPathToRoot()
        {
            string filename = Path.Combine(TheSettings.ScriptsDirectory, "foo.html");
            string pathToRoot = Rebaser.GetPathToRoot(filename);
            string template = "<html><head><link href='{0}content/something.css'/></head><body>My body.</body></html>";
            string contents = String.Format(template, "PATHTOROOT(~)");
            string expectedResult = String.Format(template, pathToRoot);
            string result = Rebaser.ReplaceRootsInFile(filename, contents);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
