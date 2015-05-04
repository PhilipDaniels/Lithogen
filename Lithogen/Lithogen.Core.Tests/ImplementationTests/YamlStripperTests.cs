using System;
using Lithogen.Engine;
using Lithogen.Engine.Implementations;
using NUnit.Framework;

namespace Lithogen.Core.Tests.ImplementationTests
{
    /*
     * This is really hard to test now that it hits the file system...
    [TestFixture]
    public class YamlinjectorTests
    {
        YamlModelInjector MakeInjector()
        {
            return new YamlModelInjector();
        }

        [Test]
        public void Strip_WhenGivenNullPayload_ThrowsArgumentNullException()
        {
            var injector = MakeInjector();
            Assert.Throws<ArgumentNullException>(() => injector.Strip(null));
        }

        [Test]
        public void Strip_WhenGivenNullPayloadContents_ThrowsArgumentNullException()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile();
            Assert.Throws<ArgumentNullException>(() => injector.Strip(inputPayload));
        }

        [Test]
        public void Strip_WhenGivenEmptyPayloadContents_IsNoOp()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile() { Contents = "" };
            var resultPayload = injector.Strip(inputPayload);

            AssertEqual(inputPayload, resultPayload);
        }

        [Test]
        public void Strip_WhenGivenUnclosedYaml_IsNoOp()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile() { Contents = "---\n this yaml is not really yaml, cos it's never closed." };
            var resultPayload = injector.Strip(inputPayload);

            AssertEqual(inputPayload, resultPayload);
        }

        [Test]
        public void Strip_WhenGivenYamlWhereStartIsNotOnOwnLine_IsNoOp()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile() { Contents = "--- message: this looks like Yaml but isn't because the markers are not on their own lines ---" };
            var resultPayload = injector.Strip(inputPayload);

            AssertEqual(inputPayload, resultPayload);
        }

        [Test]
        public void Strip_WhenGivenYamlWhichIsNotRightAtTheStart_IsNoOp()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile() { Contents = "ignore me\n---\n message: this is yaml\n---\n" };
            var resultPayload = injector.Strip(inputPayload);

            AssertEqual(inputPayload, resultPayload);
        }

        [Test]
        public void Strip_WhenGivenValidYaml_StripsYamlFromContentsAndReturnsYamlObject()
        {
            var injector = MakeInjector();
            var inputPayload = new PipelineFile() { Contents = "---\n message: this is yaml\n---\n and this is contents" };
            injector.Strip(inputPayload);

            Assert.AreEqual(" and this is contents", resultPayload.Contents);
            Assert.AreEqual(inputPayload.OriginalFilename, resultPayload.OriginalFilename);
            Assert.AreEqual(inputPayload.Filename, resultPayload.Filename);
            Assert.AreEqual("this is yaml", (string)resultPayload.Yaml.message);
        }

        static void AssertEqual(IPipelineFile inputFile, IPipelineFile outputFile)
        {
            Assert.AreEqual(inputFile.Filename, outputFile.Filename);
            Assert.AreEqual(inputFile.Contents, outputFile.Contents);
            Assert.AreEqual(inputFile.WorkingFilename, outputFile.WorkingFilename);
            Assert.AreEqual(inputFile.Yaml, outputFile.Yaml);
        }
    }
    */
}
