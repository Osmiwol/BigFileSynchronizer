using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace BigFileSynchronizer.Core
{
    public class DriveLinks
    {
        public Dictionary<string, DriveLinkEntry> Files { get; set; } = new();

        public class DriveLinkEntry
        {
            public string DriveId { get; set; }
            public long Size { get; set; }
            public string Sha256 { get; set; }
            public DateTime UploadedAt { get; set; }
        }

        public static DriveLinks Load(string path)
        {
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<DriveLinks>(json) ?? new DriveLinks();
        }

        public static DriveLinks LoadOrEmpty(string path)
        {
            return File.Exists(path) ? Load(path) : new DriveLinks();
        }

        public void Save(string path)
        {
            string json = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}
