namespace BinaryMan.Core
{
    using Schema;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IBinaryMan : IBinaryMan<BinaryInfo>
    {
    }

    public interface IBinaryMan<TBinaryInfo> where TBinaryInfo : BinaryInfo, new()
    {
        Task<IList<TBinaryInfo>> ListAllLatest();
        Task<IList<TBinaryInfo>> ListByName(string binaryName);
        Task<TBinaryInfo> GetBinaryInfo(string binaryName, Version version = null);
        Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, FileInfo destFile, CancellationToken token);
        Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, string destFilePath, CancellationToken token);
        Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, DirectoryInfo destDir, CancellationToken token);
        Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, string destDirPath, CancellationToken token);

        Task<TBinaryInfo> UploadFromFile(string binaryFilePath, string binaryName, Version binaryVersion,
            CancellationToken token, string tag = null);
        Task<TBinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName, Version binaryVersion, CancellationToken token,
            string tag = null);
    }
}
