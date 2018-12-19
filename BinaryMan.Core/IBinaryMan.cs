namespace BinaryMan.Core
{
    using Schema;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBinaryMan
    {
        Task<IList<BinaryInfo>> ListAllLatest();
        Task<IList<BinaryInfo>> ListByName(string binaryName);
        Task<BinaryInfo> GetBinaryInfo(string binaryName, Version version = null);
        Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, FileInfo destFile, CancellationToken token);
        Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, string destFilePath, CancellationToken token);
        Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, DirectoryInfo destDir, CancellationToken token);
        Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, string destDirPath, CancellationToken token);

        Task<BinaryInfo> UploadFromFile(string binaryFilePath, string binaryName, Version binaryVersion,
             CancellationToken token, string tag = null);
        Task<BinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName, Version binaryVersion, CancellationToken token,
            string tag = null);
    }
}
