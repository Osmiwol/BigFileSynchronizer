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
            string configPath = Path.Combine(".bfs", "config.json");
            string driveLinksPath = Path.Combine(".bfs", "drive_links.json");
            string serviceAccountPath = Path.Combine(".bfs", "service_account.json");

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);

            // 1. Найти файлы
            var allFiles = FileScanner.ScanFiles(config);
            var filesToUpload = new List<string>();

            foreach (var file in allFiles)
            {
                var hash = Hasher.ComputeSHA256(file);
                var size = new FileInfo(file).Length;

                if (driveLinks.Files.TryGetValue(file, out var entry))
                {
                    if (entry.Size == size && entry.Sha256 == hash)
                        continue;
                }

                filesToUpload.Add(file);
            }

            if (filesToUpload.Count == 0)
            {
                Console.WriteLine("[Push] Nothing new to upload.");
                return;
            }

            // 2. Архивация
            string driveId = Hasher.ComputeSHA256(filesToUpload[0]);
            string archiveName = $"archive_{driveId}.zip";
            string archivePath = Path.Combine("build", archiveName);
            Directory.CreateDirectory("build");

            Archiver.CreateZip(filesToUpload, Directory.GetCurrentDirectory(), archivePath);

            // 3. Загрузка в Google Drive
            var uploader = new GoogleDriveUploader(serviceAccountPath, config.CloudFolderId);
            string uploadedId = uploader.UploadFile(archivePath);
            Console.WriteLine($"[Push] Uploaded to Google Drive: ID={uploadedId}");

            // 4. Обновление кэша
            var now = DateTime.UtcNow;
            foreach (var file in filesToUpload)
            {
                var size = new FileInfo(file).Length;
                var hash = Hasher.ComputeSHA256(file);

                driveLinks.Files[file] = new DriveLinks.DriveLinkEntry
                {
                    DriveId = uploadedId,
                    Size = size,
                    Sha256 = hash,
                    UploadedAt = now
                };
            }

            Directory.CreateDirectory(".bfs");
            driveLinks.Save(driveLinksPath);

            Console.WriteLine("[Push] Upload and state update complete.");

        }
    }
}
