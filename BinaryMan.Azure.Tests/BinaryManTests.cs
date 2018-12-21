namespace BinaryMan.Azure.Tests
{
    using Core.Schema;
    using NUnit.Framework;
    using System;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public class BinaryManTests
    {
        private const string ConnStr = "UseDevelopmentStorage=true";
        private const string BlobContainerName = "binary-man-test";
        private const string TableName = "BinaryManTest";

        [SetUp]
        public void Setup()
        {
        }


        [Test]
        public void TestUploadAndCheckResult()
        {
            _ = string.IsNullOrEmpty(ConnStr) ? throw new IgnoreException($"{ConnStr} is empty") : ConnStr;

            var binaryMan = new BinaryMan(ConnStr, BlobContainerName, TableName);
            var binaryName = Guid.NewGuid().ToString("N");
            var binaryVersion = new Version("1.0.4");
            const string binaryTag = "1000";

            var binaryInfo = new BinaryInfo(binaryName, binaryVersion, binaryTag);
            Assert.IsEmpty(Task.Run(async () => await binaryMan.ListByName(binaryName)).Result);
            Assert.IsNull(Task.Run(async () => await binaryMan.GetBinaryInfo(binaryName, binaryVersion)).Result);

            var info = Task.Run(async () => await binaryMan.UploadFromFile(new FileInfo("mock.txt"), binaryName, binaryVersion, CancellationToken.None,
                binaryTag)).Result;

            Assert.IsNotEmpty(Task.Run(async () => await binaryMan.ListByName(binaryName)).Result);
            Assert.NotNull(Task.Run(async () => await binaryMan.GetBinaryInfo(binaryName, binaryVersion)).Result);

            Assert.Pass();
        }
    }
}