using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BigFileSynchronizer.Core;
using BigFileSynchronizer.Utils;
using Newtonsoft.Json;

namespace BigFileSynchronizer.Commands
{
    public static class ScanCommand
    {
        public static void Execute()
        {
            string configPath = "BigFileSynchronizer.config.json";
            if (!File.Exists(configPath))
            {
                Console.WriteLine("[Scan] config.json не найден. Сначала вызовите init.");
                return;
            }

            var config = Config.Load(configPath);
            var allFiles = FileScanner.ScanFiles(config);

            if (allFiles.Count == 0)
            {
                Console.WriteLine("[Scan] Крупные ассеты не найдены.");
                return;
            }

            // Определим директории, которых нет в config.Paths
            var detectedDirs = allFiles
                .Select(path => Path.GetDirectoryName(path))
                .Where(d => !string.IsNullOrEmpty(d))
                .Select(PathNormalizer.ToUnixPath)
                .Distinct()
                .ToList();

            var newDirs = detectedDirs
                .Where(d => !config.Paths.Any(p => PathNormalizer.ToUnixPath(p).Equals(d, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (newDirs.Count == 0)
            {
                Console.WriteLine("[Scan] Новых директорий не обнаружено.");
                return;
            }

            Console.WriteLine("[Scan] Найдены потенциальные новые директории с крупными файлами:");
            foreach (var dir in newDirs)
                Console.WriteLine("  " + dir);

            Console.WriteLine("Добавить их в config.json? [Y/n]");
            var answer = Console.ReadLine()?.Trim().ToLower();

            if (answer is "n" or "no")
            {
                Console.WriteLine("[Scan] Конфиг не изменён.");
                return;
            }

            config.Paths.AddRange(newDirs);
            config.Paths = config.Paths.Distinct().OrderBy(p => p).ToList();

            string updated = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(configPath, updated);

            Console.WriteLine("[Scan] Обновлён config.json.");
        }
    }

    internal static class PathNormalizer
    {
        public static string ToUnixPath(string path) =>
            path.Replace('\\', '/').TrimEnd('/');
    }
}
