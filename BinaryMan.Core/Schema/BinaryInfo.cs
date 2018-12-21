using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("BinaryMan.Azure")]
namespace BinaryMan.Core.Schema
{
    using StorageMate.Core.Stats;
    using System;

    public class BinaryInfo
    {
        private string _id;
        public BinaryInfo()
        {
        }

        public BinaryInfo(string name, Version version, string tag = null)
        {
            Name = name;
            VersionString = version?.ToString();
            Tag = tag ?? string.Empty;
        }

        [StatsTarget]
        public string Id
        {
            get => string.IsNullOrEmpty(_id) ? $"{Name}_{Version}" : _id;
            internal set => _id = value;
        }
        [StatsTarget, StatsCondition]
        public string Name;
        public DateTimeOffset UploadTime = DateTimeOffset.UtcNow;
        [StatsTarget]
        public string VersionString { get; internal set; }
        public Version Version
        {
            get => string.IsNullOrEmpty(VersionString) ? null : Version.Parse(VersionString);
            set => VersionString = value == null ? string.Empty : value.ToString();
        }
        public string Tag;
        public int DownloadCount;
    }
}
