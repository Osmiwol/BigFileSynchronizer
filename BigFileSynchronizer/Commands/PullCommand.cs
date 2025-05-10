// PullCommand.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using BigFileSynchronizer.Core;
using BigFileSynchronizer.Utils;

namespace BigFileSynchronizer.Commands
{
    public static class PullCommand
    {
        public static void Execute()
        {
            string configPath = Path.Combine(".bfs", "config.json");
            string driveLinksPath = Path.Combine(".bfs", "drive_links.json");
            string serviceAccountPath = Path.Combine(".bfs", "service_account.json");
            string buildDir = "build";

            if (!File.Exists(configPath) || !File.Exists(serviceAccountPath))
            {
                Console.WriteLine("[Pull] Missing config or service account.");
                return;
            }

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);
            var uploader = new GoogleDriveUploader(serviceAccountPath, config.CloudFolderId);

            int restored = 0;

            foreach (var kvp in driveLinks.Files)
            {
                string relativePath = kvp.Key;
                var entry = kvp.Value;

                bool needRestore = !File.Exists(relativePath) || Hasher.ComputeSHA256(relativePath) != entry.Sha256;
                if (!needRestore)
                    continue;

                Console.WriteLine($"[Pull] Restoring: {relativePath}");

                string tempArchive = Path.Combine(buildDir, $"restore_{entry.DriveId}.zip");
                Directory.CreateDirectory(buildDir);

                uploader.DownloadFile(entry.DriveId, tempArchive);

                using var archive = ZipFile.OpenRead(tempArchive);
                var zipEntry = archive.Entries.FirstOrDefault(e =>
                    e.FullName.Replace('\\', '/').Equals(relativePath.Replace('\\', '/'), StringComparison.OrdinalIgnoreCase));

                if (zipEntry != null)
                {
                    string fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);
                    Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
                    zipEntry.ExtractToFile(fullPath, overwrite: true);
                    Console.WriteLine($"[Pull] Extracted: {relativePath}");
                    restored++;
                }
                else
                {
                    Console.WriteLine($"[Pull] File not found in archive: {relativePath}");
                }
            }

            Console.WriteLine($"[Pull] Done. Restored {restored} file(s).");
        }
    }
}
