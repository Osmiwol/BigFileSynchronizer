using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BigFileSynchronizer.Core;
using BigFileSynchronizer.Utils;

namespace BigFileSynchronizer.Commands
{
    public static class PushCommand
    {
        public static void Execute()
        {
            string configPath = Path.Combine("BigFileSynchronizer.config.json");
            string driveLinksPath = Path.Combine(".state", "drive_links.json");

            if (!File.Exists(configPath))
            {
                Console.WriteLine("[Push] config.json not found.");
                return;
            }

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);

            var allFiles = FileScanner.ScanFiles(config);
            Console.WriteLine($"[Push] Found {allFiles.Count} matching files.");

            var filesToUpload = new List<string>();
            var now = DateTime.UtcNow;

            foreach (var file in allFiles)
            {
                var hash = Hasher.ComputeSHA256(file);
                var size = new FileInfo(file).Length;

                if (driveLinks.Files.TryGetValue(file, out var entry))
                {
                    if (entry.Size == size && entry.Sha256 == hash)
                        continue; // файл не изменился
                }

                filesToUpload.Add(file);
            }

            if (filesToUpload.Count == 0)
            {
                Console.WriteLine("[Push] Nothing new to upload.");
                return;
            }

            Console.WriteLine($"[Push] Archiving {filesToUpload.Count} new/changed files...");

            Directory.CreateDirectory("build");
            string archiveName = $"archive_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            string archivePath = Path.Combine("build", archiveName);

            Archiver.CreateZip(filesToUpload, Directory.GetCurrentDirectory(), archivePath);

            string driveId = CloudUploader.Upload(archivePath);

            foreach (var file in filesToUpload)
            {
                var size = new FileInfo(file).Length;
                var hash = Hasher.ComputeSHA256(file);

                driveLinks.Files[file] = new DriveLinks.DriveLinkEntry
                {
                    DriveId = driveId,
                    Size = size,
                    Sha256 = hash,
                    UploadedAt = now
                };
            }

            Directory.CreateDirectory(".state");
            driveLinks.Save(driveLinksPath);

            Console.WriteLine("[Push] Upload and state update complete.");
        }
    }
}
