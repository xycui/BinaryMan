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
            Version = version;
            Tag = tag;
        }

        [StatsTarget]
        public string Id => $"{Name}_{Version}";
        [StatsTarget, StatsCondition]
        public string Name;
        public DateTimeOffset UploadTime = DateTimeOffset.UtcNow;
        [StatsTarget]
        public Version Version;
        public string Tag;
        public int DownloadCount;
    }
}
