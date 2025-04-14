using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BigFileSynchronizer.Core;

namespace BigFileSynchronizer.Utils
{
    public static class FileScanner
    {
        public static List<string> ScanFiles(Config config)
        {
            var result = new List<string>();
            long minBytes = config.MinFileSizeMB * 1024L * 1024L;

            Console.WriteLine("[DEBUG] config == null? " + (config == null));
            Console.WriteLine("[DEBUG] config.Paths == null? " + (config.Paths == null));
            Console.WriteLine("[DEBUG] Paths.Count: " + config.Paths?.Count);


            foreach (var basePath in config.Paths)
            {
                if (!Directory.Exists(basePath))
                    continue;

                var files = Directory.EnumerateFiles(basePath, "*.*", SearchOption.AllDirectories)
                    .Where(path =>
                    {
                        var ext = Path.GetExtension(path).ToLowerInvariant();
                        if (!config.IncludeExtensions.Contains(ext))
                            return false;

                        var info = new FileInfo(path);
                        return info.Length >= minBytes;
                    });

                result.AddRange(files);
            }

            return result;
        }
    }
}
