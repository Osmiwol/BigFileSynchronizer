using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigFileSynchronizer.Core
{
    public class Config
    {
        public string Project { get; set; }
        public string Cloud { get; set; }
        public List<string> Paths { get; set; }
        public string ArchiveFormat { get; set; }
        public int MaxArchiveSizeMB { get; set; }
        public int MinFileSizeMB { get; set; }
        public List<string> IncludeExtensions { get; set; }

        public static Config Load(string path) { return null; }
        public void Save(string path) { }
    }
}
