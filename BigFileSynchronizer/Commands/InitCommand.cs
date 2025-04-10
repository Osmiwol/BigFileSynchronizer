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

            // 1. Проверка на git-репозиторий
            if (!Directory.Exists(Path.Combine(root, ".git")))
            {
                Console.WriteLine("[Init] Not a git repository.");
                return;
            }

            // 2. Создание конфига, если его нет
            string configPath = Path.Combine(root, "BigFileSynchronizer.config.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("[Init] Creating default config...");

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
            }
            else
            {
                Console.WriteLine("[Init] Config already exists.");
            }

            // 3. Создание .state и кэша
            string stateDir = Path.Combine(root, ".state");
            string linksPath = Path.Combine(stateDir, "drive_links.json");
            Directory.CreateDirectory(stateDir);

            if (!File.Exists(linksPath))
            {
                var empty = new DriveLinks();
                empty.Save(linksPath);
                Console.WriteLine("[Init] Empty drive_links.json created.");
            }
            else
            {
                Console.WriteLine("[Init] drive_links.json already exists.");
            }

            // 4. Создание git hook
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
                // Windows CMD-скрипт
                return "@echo off\r\n" +
                       "echo [BigFileSynchronizer] Syncing assets before push...\r\n" +
                       "BigFileSynchronizer\\BigFileSynchronizer.exe push\r\n" +
                       "if %ERRORLEVEL% NEQ 0 (\r\n" +
                       "    echo [BigFileSynchronizer] Sync failed. Push aborted.\r\n" +
                       "    exit /B 1\r\n" +
                       ")\r\n";
            }
            else
            {
                // Bash для Unix
                var sb = new StringBuilder();
                sb.AppendLine("#!/bin/bash");
                sb.AppendLine("echo \"[BigFileSynchronizer] Syncing assets before push...\"");
                sb.AppendLine("./BigFileSynchronizer/BigFileSynchronizer.exe push");
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