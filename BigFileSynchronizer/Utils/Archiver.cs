using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace BigFileSynchronizer.Utils
{
    public static class Archiver
    {
        /// <summary>
        /// Архивирует список файлов, сохраняя относительные пути относительно projectRoot.
        /// </summary>
        /// <param name="filePaths">Список полных путей к файлам</param>
        /// <param name="projectRoot">Корень проекта (относительно которого сохраняется структура)</param>
        /// <param name="outputZipPath">Путь к архиву, который будет создан</param>
        public static void CreateZip(List<string> filePaths, string projectRoot, string outputZipPath)
        {
            if (File.Exists(outputZipPath))
                File.Delete(outputZipPath);

            Directory.CreateDirectory(Path.GetDirectoryName(outputZipPath)!);

            using var zip = ZipFile.Open(outputZipPath, ZipArchiveMode.Create);

            foreach (var fullPath in filePaths)
            {
                if (!File.Exists(fullPath))
                    continue;

                string relativePath = Path.GetRelativePath(projectRoot, fullPath).Replace('\\', '/');

                zip.CreateEntryFromFile(fullPath, relativePath, CompressionLevel.Optimal);
            }
        }
    }
}
