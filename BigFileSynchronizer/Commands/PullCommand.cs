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

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);
            var uploader = new GoogleDriveUploader(serviceAccountPath);

            int restored = 0;

            foreach (var kvp in driveLinks.Files)
            {
                string filePath = kvp.Key;
                var entry = kvp.Value;

                bool needRestore = !File.Exists(filePath) || Hasher.ComputeSHA256(filePath) != entry.Sha256;
                if (!needRestore)
                    continue;

                Console.WriteLine($"[Pull] Restoring: {filePath}");

                string tempPath = Path.Combine("build", $"restore_{entry.DriveId}.zip");
                Directory.CreateDirectory("build");

                uploader.DownloadFile(entry.DriveId, tempPath);

                using var archive = ZipFile.OpenRead(tempPath);
                var found = false;

                foreach (var zipEntry in archive.Entries)
                {
                    string zipPath = zipEntry.FullName.Replace('\\', '/');
                    string normalizedTarget = filePath.Replace('\\', '/');

                    if (string.Equals(zipPath, normalizedTarget, StringComparison.OrdinalIgnoreCase))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                        zipEntry.ExtractToFile(filePath, overwrite: true);
                        Console.WriteLine($"[Pull] Extracted: {filePath}");
                        restored++;
                        found = true;
                        break;
                    }
                }

                if (!found)
                    Console.WriteLine($"[Pull] File not found in archive: {filePath}");
            }

            Console.WriteLine($"[Pull] Done. Restored {restored} file(s).");

        }
    }
}
