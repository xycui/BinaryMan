namespace BinaryMan.Core
{
    using System.IO;
    using System.IO.Compression;
    using System.Threading;
    using System.Threading.Tasks;
    using Schema;

    public static class BinaryManExtensions
    {
        public static async Task<TBinaryInfo> PackageAndUpload<T, TBinaryInfo>(this T binaryMan, TBinaryInfo binaryInfo, DirectoryInfo packageDir, CancellationToken token)
            where TBinaryInfo : BinaryInfo, new()
            where T : IBinaryMan<TBinaryInfo>
        {
            var tmpPath = Path.GetTempPath();
            var pkgFileInfo = new FileInfo(Path.Combine(tmpPath, binaryInfo.Id));
            if (pkgFileInfo.Exists)
            {
                pkgFileInfo.Delete();
            }
            ZipFile.CreateFromDirectory(packageDir.FullName, pkgFileInfo.FullName);
            binaryInfo = await binaryMan.UploadFromFile(pkgFileInfo, binaryInfo, token);
            pkgFileInfo.Delete();

            return binaryInfo;
        }
    }
}
