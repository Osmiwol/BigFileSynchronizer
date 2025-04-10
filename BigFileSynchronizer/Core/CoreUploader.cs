using System;
using System.IO;

namespace BigFileSynchronizer.Core
{
    public static class CloudUploader
    {
        private static readonly string MockUploadDir = Path.Combine("upload_mock");

        public static string Upload(string archivePath)
        {
            if (!File.Exists(archivePath))
                throw new FileNotFoundException("Archive not found", archivePath);

            Directory.CreateDirectory(MockUploadDir);

            string fileName = Path.GetFileName(archivePath);
            string destination = Path.Combine(MockUploadDir, fileName);

            File.Copy(archivePath, destination, overwrite: true);

            // Эмуляция driveId (можно заменить GUID или hash)
            string fakeDriveId = Guid.NewGuid().ToString("N");

            Console.WriteLine($"[CloudUploader] Uploaded {fileName} as ID: {fakeDriveId}");

            return fakeDriveId;
        }
    }
}
