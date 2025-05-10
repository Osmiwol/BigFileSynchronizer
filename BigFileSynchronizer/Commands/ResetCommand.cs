// ResetCommand.cs
using System;
using System.IO;

namespace BigFileSynchronizer.Commands
{
    public static class ResetCommand
    {
        public static void Execute()
        {
            DeleteDir(".bfs");
            DeleteDir("build");

            DeleteFile(".git/hooks/pre-push");
            DeleteFile(".git/hooks/pre-push.cmd");

            Console.WriteLine("[Reset] Note: BigFileSynchronizer.exe was not deleted (currently running). Delete manually if needed.");
            Console.WriteLine("[Reset] Done.");
        }

        // Deletes a directory and all its contents if it exists
        private static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
                Console.WriteLine($"[Reset] Removed directory: {path}");
            }
        }

        // Deletes a file if it exists and is accessible
        private static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Console.WriteLine($"[Reset] Removed file: {path}");
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"[Reset] Skipped deleting {path} (access denied).");
                }
            }
        }
    }
}
