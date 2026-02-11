using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace RresourcepackStudio.Classes.Configs
{
    public enum FileType
    {
        [JsonStringEnumMemberName("dir")]
        Directory,

        [JsonStringEnumMemberName("file")]
        File,
    }

    public class ProjectFileConfig
    {
        public class Index
        {
            [JsonProperty("name")]
            public string Name { get; set; } = "Resourcepack Studio Project";

            [JsonProperty("pack-info")]
            public PackInfo PackInfo { get; set; } = new PackInfo();

            [JsonProperty("files")]
            public FileInfo[]? Files { get; set; } = null;
        }

        public class PackInfo
        {
            [JsonProperty("build-name")]
            public string BuildName { get; set; } = "ResourcePack.zip";

            [JsonProperty("description")]
            public string Description { get; set; } = "The resource pack built using the §n§lResourcepack Studio§r§r.";

            [JsonProperty("version")]
            public VersionInfo Version { get; set; } = new VersionInfo();

            [JsonProperty("language")]
            public LanguageInfo[]? Language = null;
        }

        public class VersionInfo
        {
            [JsonProperty("min")]
            public int Min { get; set; } = 34;
            
            [JsonProperty("max")]
            public int Max { get; set; } = 46;
        }

        public class LanguageInfo
        {
            [JsonProperty("code")]
            public string Code { get; set; } = "zh_cn";

            [JsonProperty("name")]
            public string Name { get; set; } = "简体中文";

            [JsonProperty("region")]
            public string Region { get; set; } = "中国大陆";

        }

        public class FileInfo
        {
            [JsonProperty("name")]
            public string Name { get; set; } = "New File";

            [JsonProperty("type")]
            public FileType Type { get; set; } = FileType.File;

            [JsonProperty("children")]
            public FileInfo[]? Children = null;
        }
    }
}
