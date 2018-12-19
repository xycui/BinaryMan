using System;
using System.Collections.Generic;

namespace BinaryMan.Azure
{
    using Core;
    using Core.Schema;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    //todo: Finish the implementation and change to public
    internal class BlobOnlyBinaryMan : BaseBinaryMan<BinaryInfo>
    {
        private readonly string _binaryContainerName = "binary-man-container";
        private readonly CloudBlobContainer _binaryContainer;

        public BlobOnlyBinaryMan(string accountConStr) : this(accountConStr, string.Empty, string.Empty)
        {
        }

        public BlobOnlyBinaryMan(string accountConStr, string containerName, string tableName)
        {
            var account = CloudStorageAccount.Parse(accountConStr);
            _binaryContainerName = !string.IsNullOrEmpty(containerName) ? containerName : _binaryContainerName;
            _binaryContainer = account.CreateCloudBlobClient().GetContainerReference(_binaryContainerName);
        }

        public override Task<IList<BinaryInfo>> ListAllLatest()
        {
            throw new NotImplementedException();
        }

        public override Task<IList<BinaryInfo>> ListByName(string binaryName)
        {
            throw new NotImplementedException();
        }

        public override Task<BinaryInfo> GetBinaryInfo(string binaryName, Version version = null)
        {
            throw new NotImplementedException();
        }

        public override async Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, FileInfo destFile, CancellationToken token)
        {
            await Task.Yield();
            _ = destFile ?? throw new ArgumentNullException(nameof(destFile));
            if (destFile.Directory != null && !destFile.Directory.Exists)
            {
                destFile.Directory.Create();
            }

            var tmpFileInfo = new FileInfo($"{destFile.FullName}.tmp");


            tmpFileInfo.CopyTo(destFile.FullName, true);
            tmpFileInfo.Delete();
            return destFile;
        }

        public override Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, DirectoryInfo destDir, CancellationToken token)
        {
            return DownloadToFile(binaryName, binaryVersion,
                new FileInfo(Path.Combine(destDir.FullName, $"{binaryName}_{binaryVersion}")), token);
        }

        public override Task<BinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName, Version binaryVersion, CancellationToken token, string tag = null)
        {
            throw new NotImplementedException();
        }

        public override Task<BinaryInfo> UploadFromFile(FileInfo binaryFile, BinaryInfo binaryInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
