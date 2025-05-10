using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using BigFileSynchronizer.Utils;

namespace BigFileSynchronizer.Utils
{
    public static class Archiver
    {
        public static void CreateZip(List<string> filePaths, string rootDirectory, string outputZipPath, string branchName, string fullCommitHash)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(outputZipPath)!);

            using var archive = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

            foreach (var relativePath in filePaths)
            {
                string fullPath = Path.Combine(rootDirectory, relativePath);
                archive.CreateEntryFromFile(fullPath, relativePath, CompressionLevel.Optimal);
            }

            // creating manifest-file
            string shortHash = fullCommitHash.Substring(0, 7);
            string manifestName = $"manifest_{branchName}_{shortHash}.txt";
            string manifestContent = GenerateManifestContent(filePaths, branchName, fullCommitHash);
            var manifestEntry = archive.CreateEntry(manifestName);

            using var writer = new StreamWriter(manifestEntry.Open(), Encoding.UTF8);
            writer.Write(manifestContent);
        }

        private static string GenerateManifestContent(List<string> paths, string branch, string fullHash)
        {
            var sb = new StringBuilder();
            sb.AppendLine("# BigFileSynchronizer manifest");
            sb.AppendLine("# Branch: " + branch);
            sb.AppendLine("# Commit: " + fullHash);
            sb.AppendLine("# Created: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("# Place the resources from the archive in the paths indicated below.");
            sb.AppendLine();

            foreach (var path in paths)
                sb.AppendLine(path);

            return sb.ToString();
        }
    }
}
