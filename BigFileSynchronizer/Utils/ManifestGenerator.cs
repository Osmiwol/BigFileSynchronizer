using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BigFileSynchronizer.Utils
{
    public static class ManifestGenerator
    {
        public static string CreateManifest(string branch, string fullCommitHash, List<string> filePaths)
        {
            string shortHash = fullCommitHash?.Length >= 7 ? fullCommitHash.Substring(0, 7) : "unknown";

            string manifestFileName = $"manifest_{branch}_{shortHash}.txt";
            var sb = new StringBuilder();

            sb.AppendLine("# BigFileSynchronizer Manifest");
            sb.AppendLine($"# Branch: {branch}");
            sb.AppendLine($"# Commit: {fullCommitHash}");
            sb.AppendLine($"# Created: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC\n");

            sb.AppendLine("# Files included:");
            foreach (var path in filePaths)
                sb.AppendLine(path);

            sb.AppendLine("\n# Restore paths are relative to project root");

            File.WriteAllText(manifestFileName, sb.ToString());
            return manifestFileName;
        }
    }
}
