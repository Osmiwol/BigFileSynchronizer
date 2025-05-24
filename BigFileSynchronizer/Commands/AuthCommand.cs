using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BigFileSynchronizer.Core;
using Google.Apis.Drive.v3;

public static class AuthCommand
{
    public static void Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("[Auth] Usage: auth <path-to-service_account.json>");
            return;
        }

        string inputPath = args[1];
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"[Auth] File not found: {inputPath}");
            return;
        }

        string json = File.ReadAllText(inputPath);
        var obj = JObject.Parse(json);
        string? clientEmail = obj["client_email"]?.ToString();
        string? privateKey = obj["private_key"]?.ToString();

        if (string.IsNullOrEmpty(clientEmail) || string.IsNullOrEmpty(privateKey))
        {
            Console.WriteLine("[Auth] Invalid service_account.json (missing client_email or private_key)");
            return;
        }

        string configDir = ".config_bfs";
        string targetPath = Path.Combine(configDir, "service_account.json");
        Directory.CreateDirectory(configDir);
        File.Copy(inputPath, targetPath, overwrite: true);
        Console.WriteLine("[Auth] service_account.json copied to .config_bfs/");

        // Activation Drive API
        GoogleDriveUploader uploader;
        try
        {
            uploader = new GoogleDriveUploader(targetPath);
            uploader.Touch();
            Console.WriteLine("[Auth] Google service account activated.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] Activation failed: {ex.Message}");
            return;
        }

        string configPath = Path.Combine(configDir, "config.json");
        if (!File.Exists(configPath))
        {
            Console.WriteLine("[Auth] config.json not found — run init first.");
            return;
        }

        var config = Config.Load(configPath);
        config.Cloud = "GoogleDrive";

        // Требуем ввести валидный ID папки
        while (true)
        {
            Console.WriteLine("Enter Google Drive folder ID (required, must exist and be shared to the service account)\nhttps://drive.google.com/drive/.../folders/[id]:");
            
            string? folderId = Console.ReadLine()?.Trim();

            if (!string.IsNullOrEmpty(folderId))
            {
                if (IsFolderIdValid(uploader, folderId))
                {
                    config.CloudFolderId = folderId;
                    Console.WriteLine($"[Auth] GoogleDriveFolderId saved: {folderId}");
                    break;
                }
                else
                {
                    Console.WriteLine("[Auth] Folder ID is invalid or not accessible by your service account. Please try again.");
                }
            }
            else
            {
                Console.WriteLine("[Auth] Folder ID is required! See README for setup instructions.");
            }
        }

        config.Save(configPath);
        Console.WriteLine("[Auth] Complete.");
    }

    // Валидация ID папки через Drive API
    private static bool IsFolderIdValid(GoogleDriveUploader uploader, string folderId)
    {
        try
        {
            // Попробуем создать временный файл в папке
            var service = uploader.GetDriveService();
            var tempFile = new Google.Apis.Drive.v3.Data.File
            {
                Name = $"bfs_test_{Guid.NewGuid().ToString().Substring(0, 8)}.tmp",
                Parents = new[] { folderId }
            };
            using (var stream = new MemoryStream(new byte[1])) // Пустой файл
            {
                var request = service.Files.Create(tempFile, stream, "application/octet-stream");
                request.Fields = "id";
                var result = request.Upload();

                if (result.Status == Google.Apis.Upload.UploadStatus.Completed)
                {
                    // Удаляем тестовый файл
                    var createdFileId = request.ResponseBody.Id;
                    service.Files.Delete(createdFileId).Execute();
                    return true;
                }
            }
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Auth] Exception during folder check: {ex.Message}");
            return false;
        }
    }
}
