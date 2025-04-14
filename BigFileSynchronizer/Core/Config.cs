using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace BigFileSynchronizer.Core
{
    public class Config
    {
        [JsonProperty("Project")]
        public string Project { get; set; }

        [JsonProperty("Cloud")]
        public string Cloud { get; set; }

        [JsonProperty("Paths")]
        public List<string> Paths { get; set; } = new();

        [JsonProperty("ArchiveFormat")]
        public string ArchiveFormat { get; set; }

        [JsonProperty("MaxArchiveSizeMB")]
        public int MaxArchiveSizeMB { get; set; }

        [JsonProperty("MinFileSizeMB")]
        public int MinFileSizeMB { get; set; }

        [JsonProperty("IncludeExtensions")]
        public List<string> IncludeExtensions { get; set; } = new();

        public static Config Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("Config file not found", path);

            var json = File.ReadAllText(path);
            var config = JsonConvert.DeserializeObject<Config>(json) ?? new Config();

            config.Paths ??= new List<string>();
            config.IncludeExtensions ??= new List<string>();

            return config;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
