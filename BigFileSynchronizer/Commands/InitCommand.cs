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

            // 2. Создание директории конфигов
            string configDir = Path.Combine(root, ".config_bfs");
            Directory.CreateDirectory(configDir);

            // 3. Создание config.json
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

            // 4. Создание drive_links.json
            string linksPath = Path.Combine(configDir, "drive_links.json");
            Console.WriteLine("[Init] Resetting drive_links.json...");
            var empty = new DriveLinks();
            empty.Save(linksPath);

            // 5. Создание универсального pre-push hook
            string hookPath = Path.Combine(root, ".git", "hooks", "pre-push");
            File.WriteAllText(hookPath, GenerateUniversalHook(), new UTF8Encoding(false));
            MakeExecutable(hookPath);

            Console.WriteLine("[Init] Git pre-push hook installed (universal).");
            Console.WriteLine("[Init] Initialization complete.");
        }

        private static string GenerateUniversalHook()
        {
            return
                "#!/bin/sh\n" +
                "echo \"[BFS] Syncing assets before push...\"\n" +
                "if [ -f ./bfsgit ]; then\n" +
                "  ./bfsgit push\n" +
                "elif [ -f ./bfsgit.exe ]; then\n" +
                "  ./bfsgit.exe push\n" +
                "else\n" +
                "  echo \"[BFS] bfsgit(.exe) not found.\"\n" +
                "  exit 1\n" +
                "fi\n" +
                "result=$?\n" +
                "if [ $result -ne 0 ]; then\n" +
                "  echo \"[BFS] Sync failed. Push aborted.\"\n" +
                "  exit 1\n" +
                "fi\n";
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
                // Windows — безопасно игнорировать
            }
        }
    }
}
