namespace BinaryMan.Azure.Tests
{
    using System;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using NUnit.Framework;

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
        public void Test()
        {
            var binaryMan = new BinaryMan(ConnStr, BlobContainerName, TableName);

            CloudStorageAccount account = CloudStorageAccount.Parse("");

            Console.WriteLine(account.CreateCloudBlobClient().GetContainerReference("test").GetBlobReference("reply.prod.txt").IsSnapshot);
            Console.WriteLine(account.CreateCloudBlobClient().GetContainerReference("test").GetBlobReference("reply.prod.txt").SnapshotQualifiedUri);
            foreach (var listBlobItem in account.CreateCloudBlobClient().GetContainerReference("test").ListBlobsSegmentedAsync("",true,BlobListingDetails.All,500,new BlobContinuationToken(),new BlobRequestOptions(), new OperationContext()).Result.Results)
            {
                var file = listBlobItem as CloudBlockBlob;
                Console.WriteLine(file.SnapshotTime +"  " +file.SnapshotQualifiedStorageUri);
            }

            Assert.Pass();
        }
    }
}