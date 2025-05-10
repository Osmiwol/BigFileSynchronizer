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
            var extensions = new HashSet<string>(config.IncludeExtensions.Select(e => e.ToLowerInvariant()));

            Console.WriteLine("[Scan] Scanning all files in current directory...");

            var allFiles = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*.*", SearchOption.AllDirectories);

            foreach (var file in allFiles)
            {
                string ext = Path.GetExtension(file).ToLowerInvariant();
                if (!extensions.Contains(ext)) continue;

                long size = new FileInfo(file).Length;
                if (size < minBytes) continue;

                string relativePath = Path.GetRelativePath(Directory.GetCurrentDirectory(), file);
                result.Add(relativePath);
            }

            Console.WriteLine($"[Scan] Found {result.Count} matching file(s).");
            return result;
        }
    }
}
