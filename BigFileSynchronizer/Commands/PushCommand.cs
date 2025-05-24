// PushCommand.cs
using System;
using System.Collections.Generic;
using System.IO;
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

            if (!File.Exists(configPath) || !File.Exists(serviceAccountPath))
            {
                Console.WriteLine("[Push] Missing config or service account.");
                return;
            }

            var config = Config.Load(configPath);
            var driveLinks = DriveLinks.LoadOrEmpty(driveLinksPath);

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

            // Get git branch and commit info
            string branch = GitHelper.GetBranchName() ?? "nogit";
            string commit = GitHelper.GetFullCommitHash();
            string driveId = Hasher.ComputeSHA256(filesToUpload[0]);            

            if (string.IsNullOrWhiteSpace(commit) || commit.Length < 7)
            {
                Console.WriteLine("[Push] Git not available — using fallback commit ID.");
                commit = Guid.NewGuid().ToString("N"); // random unique ID
            }


            // Generate human-readable manifest file and include it in the archive
            string manifestPath = ManifestGenerator.CreateManifest(branch, commit, filesToUpload);
            filesToUpload.Add(manifestPath); // Include manifest in the archive

            // Create archive with files + manifest
            string archiveName = $"archive_{driveId}.zip";
            string archivePath = Path.Combine("build", archiveName);
            Archiver.CreateZip(filesToUpload, Directory.GetCurrentDirectory(), archivePath, branch, commit);

            // Remove temporary manifest after packaging
            if (File.Exists(manifestPath))
                File.Delete(manifestPath);

            // Upload archive to Google Drive
            var uploader = new GoogleDriveUploader(serviceAccountPath, config.CloudFolderId);
            string uploadedId = uploader.UploadFile(archivePath);
            Console.WriteLine($"[Push] Uploaded to Google Drive: ID={uploadedId}");

            // Update drive_links with new/updated files
            var now = DateTime.UtcNow;
            foreach (var file in filesToUpload)
            {
                if (!File.Exists(file) || file.EndsWith(".txt")) continue; // Skip manifest entry

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

            driveLinks.Save(driveLinksPath);
            Console.WriteLine("[Push] Upload and state update complete.");
        }
    }
}
