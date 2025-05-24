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
            // Remove old archive if it already exists
            if (File.Exists(outputZipPath))
            {
                File.Delete(outputZipPath);
            }

            Directory.CreateDirectory(Path.GetDirectoryName(outputZipPath)!);

            using var archive = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

            foreach (var relativePath in filePaths)
            {
                string fullPath = Path.Combine(rootDirectory, relativePath);
                archive.CreateEntryFromFile(fullPath, relativePath, CompressionLevel.Optimal);
            }

            // Manifest is already generated outside, no need to regenerate here (optional)
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
