// ScanCommand.cs
using System;
using System.IO;
using BigFileSynchronizer.Core;
using BigFileSynchronizer.Utils;

namespace BigFileSynchronizer.Commands
{
    public static class ScanCommand
    {
        public static void Execute()
        {
            string configPath = Path.Combine(".config_bfs", "config.json");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("[Scan] Config not found: .config_bfs/config.json");
                return;
            }

            var config = Config.Load(configPath);
            var files = FileScanner.ScanFiles(config);

            if (files.Count == 0)
            {
                Console.WriteLine("[Scan] No matching files found.");
                return;
            }

            Console.WriteLine("[Scan] Files to be considered for upload:");
            foreach (var file in files)
            {
                var sizeMB = new FileInfo(file).Length / (1024.0 * 1024.0);
                Console.WriteLine($"  {file} ({sizeMB:F1} MB)");
            }

            Console.WriteLine($"[Scan] Total: {files.Count} file(s).");
        }
    }

}
