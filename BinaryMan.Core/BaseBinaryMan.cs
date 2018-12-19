using BinaryMan.Core.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BinaryMan.Core
{
    using System.Threading;

    public abstract class BaseBinaryMan : IBinaryMan
    {
        public abstract Task<IList<BinaryInfo>> ListAllLatest();

        public abstract Task<IList<BinaryInfo>> ListByName(string binaryName);
        public abstract Task<BinaryInfo> GetBinaryInfo(string binaryName, Version version = null);
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

        public Task<BinaryInfo> UploadFromFile(string binaryFilePath, string binaryName, Version binaryVersion, CancellationToken token, string tag = null)
        {
            return UploadFromFile(new FileInfo(binaryFilePath), binaryName, binaryVersion, token, tag);
        }

        public abstract Task<BinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName, Version binaryVersion, CancellationToken token,
            string tag = null);
    }
}
