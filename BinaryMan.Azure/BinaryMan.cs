﻿namespace BinaryMan.Azure
{
    using Core;
    using Core.Schema;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using StorageMate.Azure.Table;
    using StorageMate.Core.ObjectStore;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class BinaryMan<TBinaryInfo> : BaseBinaryMan<TBinaryInfo> where TBinaryInfo : BinaryInfo, new()
    {
        private readonly string _binaryContainerName = "binary-man-container";
        private readonly string _cloudTableName = "BinaryManMetaData";

        private readonly CloudBlobContainer _binaryContainer;
        private readonly IDataAccessor<string, TBinaryInfo> _binaryInfoStore;
        private readonly IDataInsighter<TBinaryInfo> _binaryInfoInsighter;


        public BinaryMan(string accountConStr) : this(accountConStr, string.Empty, string.Empty)
        {
        }

        public BinaryMan(string accountConStr, string containerName, string tableName)
        {
            var account = CloudStorageAccount.Parse(accountConStr);
            _binaryContainerName = !string.IsNullOrEmpty(containerName)
                ? NamingUtil.CamelCase2Dash(containerName)
                : _binaryContainerName;
            _cloudTableName = !string.IsNullOrEmpty(tableName) ? tableName : _cloudTableName;
            _binaryContainer = account.CreateCloudBlobClient().GetContainerReference(_binaryContainerName);
            _binaryContainer.CreateIfNotExistsAsync();
            _binaryInfoStore = new TableBasedDataAccessor<TBinaryInfo>(accountConStr, _cloudTableName);
            _binaryInfoInsighter = new TableDataInsighter<TBinaryInfo>(accountConStr, $"{_cloudTableName}Stats");
        }

        public override async Task<IList<TBinaryInfo>> ListAllLatest()
        {
            var nameList = await _binaryInfoInsighter.ListAllAsync(info => info.Name);
            var list = new List<TBinaryInfo>();
            foreach (var name in nameList)
            {
                list.Add(await GetBinaryInfo(name));
            }

            return list;
        }

        public override async Task<IList<TBinaryInfo>> ListByName(string binaryName)
        {
            var idList =
                await _binaryInfoInsighter.ListWithConditionAsync(info => info.Name, binaryName, info => info.Id);

            var list = new List<TBinaryInfo>();
            foreach (var id in idList)
            {
                list.Add(await _binaryInfoStore.ReadAsync(id));
            }

            return list;
        }

        public override async Task<TBinaryInfo> GetBinaryInfo(string binaryName, Version version = null)
        {
            binaryName = string.IsNullOrEmpty(binaryName)
                ? throw new ArgumentNullException(nameof(binaryName))
                : binaryName.Trim();

            var key = version == null
                ? $"{typeof(BinaryInfo)}_{nameof(BinaryInfo.Name)}_{binaryName}"
                : $"{typeof(BinaryInfo)}_{nameof(BinaryInfo.Id)}_{new BinaryInfo(binaryName, version).Id}";

            return await _binaryInfoStore.ReadAsync(key);
        }

        public override async Task<FileInfo> DownloadToFile(string binaryName, Version binaryVersion, FileInfo destFile,
            CancellationToken token)
        {
            _ = destFile ?? throw new ArgumentNullException(nameof(destFile));
            if (destFile.Directory != null && !destFile.Directory.Exists)
            {
                destFile.Directory.Create();
            }

            var remoteName = GetBinaryRemoteName(binaryName, binaryVersion);
            var binaryInfo =
                await _binaryInfoStore.ReadAsync(
                    $"{nameof(BinaryInfo.Id)}_{new BinaryInfo(binaryName, binaryVersion).Id}");
            if (binaryInfo == null || !await _binaryContainer.GetBlobReference(remoteName).ExistsAsync())
            {
                throw new Exception("Remote package could not be found");
            }

            var tmpFileInfo = new FileInfo($"{destFile.FullName}.tmp");
            await _binaryContainer.GetBlobReference(remoteName)
                .DownloadToFileAsync(tmpFileInfo.FullName, FileMode.Create);

            tmpFileInfo.CopyTo(destFile.FullName, true);
            tmpFileInfo.Delete();
            return destFile;
        }

        public override Task<FileInfo> DownloadToDir(string binaryName, Version binaryVersion, DirectoryInfo destDir,
            CancellationToken token)
        {
            return DownloadToFile(binaryName, binaryVersion,
                new FileInfo(Path.Combine(destDir.FullName, $"{binaryName}_{binaryVersion}")), token);
        }

        public override async Task<TBinaryInfo> UploadFromFile(FileInfo binaryFile, string binaryName,
            Version binaryVersion, CancellationToken token, string tag = null)
        {
            var binaryInfo = await GetBinaryInfo(binaryName);
            if (binaryInfo != null)
            {
                _ = binaryInfo.Version > binaryVersion
                    ? throw new Exception(
                        $"Repo latest version {binaryInfo.Version} larger than upload one {binaryVersion}")
                    : binaryInfo.Version == binaryVersion
                        ? throw new Exception(
                            $"Version {binaryVersion} is already existed in the repo.")
                        : binaryVersion;
            }

            //todo add upload binary to blob logic
            var remoteName = GetBinaryRemoteName(binaryName, binaryVersion);
            var remoteBlob = _binaryContainer.GetBlockBlobReference(remoteName);
            await remoteBlob.UploadFromFileAsync(binaryFile.FullName);

            binaryInfo = new TBinaryInfo { Name = binaryName, Version = binaryVersion, Tag = tag };
            var writeDataTask =
                _binaryInfoStore.WriteAsync($"{typeof(TBinaryInfo)}_{nameof(binaryInfo.Id)}_{binaryInfo.Id}",
                    binaryInfo);
            var writeLatestTask =
                _binaryInfoStore.WriteAsync($"{typeof(TBinaryInfo)}_{nameof(binaryInfo.Name)}_{binaryName}",
                    binaryInfo);
            await _binaryInfoInsighter.AddForStatsAsync(binaryInfo);
            return (await Task.WhenAll(writeDataTask, writeLatestTask)).FirstOrDefault();
        }

        private static string GetBinaryRemoteName(string binaryName, Version binaryVersion)
        {
            return $"{binaryName.Trim('/')}/{binaryVersion}";
        }
    }

    public class BinaryMan : BinaryMan<BinaryInfo>
    {
        public BinaryMan(string accountConStr) : base(accountConStr)
        {
        }

        public BinaryMan(string accountConStr, string containerName, string tableName) : base(accountConStr,
            containerName, tableName)
        {
        }
    }
}
