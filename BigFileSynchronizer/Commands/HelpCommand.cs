// HelpCommand.cs
using System;

namespace BigFileSynchronizer.Commands
{
    public static class HelpCommand
    {
        public static void Execute()
        {
            Console.WriteLine("BigFileSynchronizer — available commands:");
            Console.WriteLine();
            Console.WriteLine("  init      — initialize project (creates .bfs/, hook, config)");
            Console.WriteLine("  auth      — authorize Google Drive via service account");
            Console.WriteLine("  push      — upload new or changed assets to Google Drive");
            Console.WriteLine("  pull      — restore missing files from cloud archive");
            Console.WriteLine("  scan      — show list of assets that would be uploaded");
            Console.WriteLine("  reset     — remove generated files and state");
            Console.WriteLine("  help      — show this help message");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  BigFileSynchronizer.exe init");
            Console.WriteLine("  BigFileSynchronizer.exe auth service_account.json");
            Console.WriteLine("  git push  → will trigger auto push to cloud");
        }
    }
}
