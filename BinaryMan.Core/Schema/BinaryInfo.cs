namespace BinaryMan.Core.Schema
{
    using StorageMate.Core.Stats;
    using System;

    public class BinaryInfo
    {
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
        public string Id => $"{Name}_{Version}";
        [StatsTarget, StatsCondition]
        public string Name;
        public DateTimeOffset UploadTime = DateTimeOffset.UtcNow;
        [StatsTarget]
        public string VersionString;
        public Version Version => string.IsNullOrEmpty(VersionString) ? null : Version.Parse(VersionString);
        public string Tag;
        public int DownloadCount;
    }
}
