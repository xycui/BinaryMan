using BinaryMan.Core.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BinaryMan.Core
{
    using System.Threading;

    public abstract class BaseBinaryMan<TBinaryInfo> : IBinaryMan<TBinaryInfo> where TBinaryInfo : BinaryInfo, new()
    {
        public abstract Task<IList<TBinaryInfo>> ListAllLatest();

        public abstract Task<IList<TBinaryInfo>> ListByName(string binaryName);
        public abstract Task<TBinaryInfo> GetBinaryInfo(string binaryName, Version version = null);
        public abstract Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, FileInfo destFile, CancellationToken token);

        public Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, string destFilePath, CancellationToken token)
        {
            return DownloadToFile(binaryName, binaryVersion, new FileInfo(destFilePath), token);
        }

        public abstract Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, DirectoryInfo destDir, CancellationToken token);

        public Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, string destDirPath, CancellationToken token)
        {
            return DownloadToDir(binaryName, binaryVersion, new DirectoryInfo(destDirPath), token);
        }

        public Task<TBinaryInfo> UploadFromFile(string binaryFilePath, string binaryName, Version binaryVersion, CancellationToken token, string tag = null)
        {
            return UploadFromFile(new FileInfo(binaryFilePath), binaryName, binaryVersion, token, tag);
        }

        public Task<TBinaryInfo> UploadFromFile(string binaryFilePath, TBinaryInfo binaryInfo, CancellationToken token)
        {
            return UploadFromFile(new FileInfo(binaryFilePath), binaryInfo, token);
        }

        public abstract Task<TBinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName, Version binaryVersion, CancellationToken token,
            string tag = null);

        public abstract Task<TBinaryInfo> UploadFromFile(FileInfo binaryFile, TBinaryInfo binaryInfo,
            CancellationToken token);
    }
}
