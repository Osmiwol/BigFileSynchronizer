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

            // Проверка на git-репозиторий
            if (!Directory.Exists(Path.Combine(root, ".git")))
            {
                Console.WriteLine("[Init] Not a git repository.");
                return;
            }

            // Создание директории конфигов
            string configDir = Path.Combine(root, ".bfs");
            Directory.CreateDirectory(configDir);

            // config.json
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

            // drive_links.json
            string linksPath = Path.Combine(configDir, "drive_links.json");
            Console.WriteLine("[Init] Resetting drive_links.json...");
            var empty = new DriveLinks();
            empty.Save(linksPath);

            // Git hooks
            string hooksDir = Path.Combine(root, ".git", "hooks");
            Directory.CreateDirectory(hooksDir);

            // 1. Bash-совместимый (Linux/Mac/Git Bash)
            string bashHookPath = Path.Combine(hooksDir, "pre-push");
            File.WriteAllText(bashHookPath, GenerateBashHook(), new UTF8Encoding(false));
            MakeExecutable(bashHookPath);

            // 2. Batch-совместимый (CMD/Visual Studio)
            string cmdHookPath = Path.Combine(hooksDir, "pre-push.cmd");
            File.WriteAllText(cmdHookPath, GenerateCmdHook(), new UTF8Encoding(false));

            Console.WriteLine("[Init] Git hooks installed (bash + cmd).");
            Console.WriteLine("[Init] Initialization complete.");
        }

        private static string GenerateBashHook()
        {
            return """
            #!/bin/bash
            echo "[BigFileSynchronizer] Syncing assets before push..."
            ./BigFileSynchronizer.exe push
            if [ $? -ne 0 ]; then
                echo "[BigFileSynchronizer] Sync failed. Push aborted."
                exit 1
            fi
            """;
        }

        private static string GenerateCmdHook()
        {
            return """
            @echo off
            echo [BigFileSynchronizer] Syncing assets before push...
            BigFileSynchronizer.exe push
            if %ERRORLEVEL% NEQ 0 (
                echo [BigFileSynchronizer] Sync failed. Push aborted.
                exit /B 1
            )
            """;
        }

        private static void MakeExecutable(string path)
        {
            try
            {
                if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
                {
                    var chmod = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "chmod",
                        Arguments = $"+x \"{path}\"",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };
                    System.Diagnostics.Process.Start(chmod)?.WaitForExit();
                }
            }
            catch
            {
                // игнорируем — безопасно
            }
        }
    }
}
