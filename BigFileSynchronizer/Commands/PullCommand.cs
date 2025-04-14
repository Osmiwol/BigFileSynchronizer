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
            Console.WriteLine("[Pull] Checking state...");

            string configPath = "BigFileSynchronizer.config.json";
            string driveLinksPath = Path.Combine(".state", "drive_links.json");
            string archiveFolder = "upload_mock";

            if (!File.Exists(configPath) || !File.Exists(driveLinksPath))
            {
                Console.WriteLine("[Pull] Config or state not found.");
                return;
            }

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);

            int restored = 0;

            foreach (var kvp in driveLinks.Files)
            {
                string filePath = kvp.Key;
                var entry = kvp.Value;

                bool needRestore = !File.Exists(filePath) || Hasher.ComputeSHA256(filePath) != entry.Sha256;
                if (!needRestore)
                    continue;

                Console.WriteLine($"[Pull] Restoring: {filePath}");

                string archiveName = $"archive_{entry.DriveId}.zip";
                string archivePath = Path.Combine(archiveFolder, archiveName);

                if (!File.Exists(archivePath))
                {
                    Console.WriteLine($"[Pull] Archive not found: {archiveName}");
                    continue;
                }

                try
                {
                    using var archive = ZipFile.OpenRead(archivePath);
                    var entryInZip = archive.Entries.FirstOrDefault(e => e.FullName.Replace('\\', '/') == filePath.Replace('\\', '/'));
                    if (entryInZip == null)
                    {
                        Console.WriteLine($"[Pull] File not found in archive: {filePath}");
                        continue;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                    entryInZip.ExtractToFile(filePath, overwrite: true);

                    Console.WriteLine($"[Pull] Extracted: {filePath}");
                    restored++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Pull] Error restoring {filePath}: {ex.Message}");
                }
            }

            Console.WriteLine($"[Pull] Done. Restored {restored} file(s).");
        }
    }
}
