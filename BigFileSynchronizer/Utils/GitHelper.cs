// GitHelper.cs
using System.Diagnostics;

namespace BigFileSynchronizer.Utils
{
    public static class GitHelper
    {
        // Returns the current git branch name
        public static string GetBranchName()
        {
            return RunGitCommand("rev-parse --abbrev-ref HEAD").Trim();
        }

        // Returns the full commit hash of HEAD
        public static string GetFullCommitHash()
        {
            return RunGitCommand("rev-parse HEAD").Trim();
        }

        // Runs a git command and returns the output
        private static string RunGitCommand(string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "git",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result;
        }
    }
}
