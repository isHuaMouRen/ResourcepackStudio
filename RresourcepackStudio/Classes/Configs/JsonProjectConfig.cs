using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace RresourcepackStudio.Classes.Configs
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FileType
    {
        [EnumMember(Value = "dir")]
        Directory,

        [EnumMember(Value = "file")]
        File,
    }

    public class JsonProjectConfig
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
            public LanguageInfo[]? Language { get; set; } = null;
        }

        public class VersionInfo
        {
            [JsonProperty("min-main")]
            public int MinMain { get; set; } = 34;

            [JsonProperty("min-sub")]
            public int MinSub { get; set; } = 0;

            [JsonProperty("neutral")]
            public int Neutral { get; set; } = 34;

            [JsonProperty("max-main")]
            public int MaxMain { get; set; } = 46;

            [JsonProperty("max-sub")]
            public int MaxSub { get; set; } = 0;
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
