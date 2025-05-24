using System;
using System.IO;

namespace BigFileSynchronizer.Commands
{
    public static class ResetCommand
    {
        public static void Execute()
        {
            DeleteDir(".config_bfs");
            DeleteDir("bfs_cache");

            string hookPath = Path.Combine(".git", "hooks", "pre-push");
            string hookPathCmd = Path.Combine(".git", "hooks", "pre-push.cmd");

            DeleteFile(hookPath);
            DeleteFile(hookPathCmd);

            Console.WriteLine("[Reset] Note: bfsgit.exe was not deleted (currently running). Delete manually if needed.");
            Console.WriteLine("[Reset] Done.");
        }

        private static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, recursive: true);
                    Console.WriteLine($"[Reset] Directory deleted: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Reset] Failed to delete directory {path}: {ex.Message}");
                }
            }
        }

        private static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    Console.WriteLine($"[Reset] File deleted: {path}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Reset] Failed to delete file {path}: {ex.Message}");
                }
            }
        }
    }

}
