using System;
using System.IO;
using System.Text;
using BigFileSynchronizer.Core;
using Newtonsoft.Json;

namespace BigFileSynchronizer.Commands
{
    public static class InitCommand
    {
        public static void Execute()
        {
            string root = Directory.GetCurrentDirectory();

            // 1. Проверка, git-репозиторий ли это
            if (!Directory.Exists(Path.Combine(root, ".git")))
            {
                Console.WriteLine("[Init] Not a git repository.");
                return;
            }

            // 2. Создание директории конфигов
            string configDir = Path.Combine(root, ".bfs");
            Directory.CreateDirectory(configDir);

            // 3. Перегенерация config.json
            string configPath = Path.Combine(configDir, "config.json");
            Console.WriteLine("[Init] (Re)creating config.json...");
            var config = new Config
            {
                Project = Path.GetFileName(root),
                Cloud = "GoogleDrive",
                Paths = new() { "Assets/", "StreamingAssets/" },
                ArchiveFormat = "zip",
                MaxArchiveSizeMB = 750,
                MinFileSizeMB = 5,
                IncludeExtensions = new()
                {
                    ".fbx", ".obj", ".blend", ".dae", ".3ds", ".gltf", ".glb",
                    ".png", ".jpg", ".jpeg", ".tga", ".bmp", ".psd", ".tiff", ".exr",
                    ".mp3", ".wav", ".ogg", ".flac", ".aiff", ".m4a",
                    ".txt", ".csv", ".json", ".xml", ".ini", ".shader", ".cginc"
                }
            };
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, json);

            // 4. Перегенерация drive_links.json
            string linksPath = Path.Combine(configDir, "drive_links.json");
            Console.WriteLine("[Init] Resetting drive_links.json...");
            var empty = new DriveLinks();
            empty.Save(linksPath);

            // 5. Hook
            string hookPath = Path.Combine(root, ".git", "hooks", "pre-push");
            if (!File.Exists(hookPath))
            {
                Console.WriteLine("[Init] Creating git pre-push hook...");
                string hookScript = GenerateHookScript();
                File.WriteAllText(hookPath, hookScript);
                MakeExecutable(hookPath);
                Console.WriteLine("[Init] Hook installed.");
            }
            else
            {
                Console.WriteLine("[Init] pre-push hook already exists.");
            }

            Console.WriteLine("[Init] Initialization complete.");
        }

        private static string GenerateHookScript()
        {
            if (OperatingSystem.IsWindows())
            {
                return "@echo off\r\n" +
                       "echo [BigFileSynchronizer] Syncing assets before push...\r\n" +
                       "BigFileSynchronizer.exe push\r\n" +
                       "if %ERRORLEVEL% NEQ 0 (\r\n" +
                       "    echo [BigFileSynchronizer] Sync failed. Push aborted.\r\n" +
                       "    exit /B 1\r\n" +
                       ")\r\n";
            }
            else
            {
                var sb = new StringBuilder();
                sb.AppendLine("#!/bin/bash");
                sb.AppendLine("echo \"[BigFileSynchronizer] Syncing assets before push...\"");
                sb.AppendLine("./BigFileSynchronizer.exe push");
                sb.AppendLine("if [ $? -ne 0 ]; then");
                sb.AppendLine("    echo \"[BigFileSynchronizer] Sync failed. Push aborted.\"");
                sb.AppendLine("    exit 1");
                sb.AppendLine("fi");
                return sb.ToString();
            }
        }

        private static void MakeExecutable(string path)
        {
            try
            {
                if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                    System.Diagnostics.Process.Start("chmod", $"+x {path}");
            }
            catch
            {
                // Windows — игнорируем
            }
        }
    }
}
