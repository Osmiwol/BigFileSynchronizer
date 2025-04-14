using System;
using System.IO;

namespace BigFileSynchronizer.Commands
{
    public static class ResetCommand
    {
        public static void Execute()
        {
            DeleteDir(".bfs");
            DeleteDir("upload_mock");
            DeleteDir("build");

            DeleteFile("BigFileSynchronizer.exe");
            DeleteFile("BigFileSynchronizer.pdb");

            string hookPath = Path.Combine(".git", "hooks", "pre-push");
            DeleteFile(hookPath);

            Console.WriteLine("[Reset] Done.");
        }

        private static void DeleteDir(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
                Console.WriteLine($"[Reset] Removed directory: {path}");
            }
        }

        private static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                Console.WriteLine($"[Reset] Removed file: {path}");
            }
        }
    }
}
