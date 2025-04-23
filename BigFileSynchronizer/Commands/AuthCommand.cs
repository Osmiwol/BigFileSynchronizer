using System;
using System.IO;
using Newtonsoft.Json.Linq;
using BigFileSynchronizer.Core;

namespace BigFileSynchronizer.Commands
{
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

            string targetPath = Path.Combine(".bfs", "service_account.json");
            Directory.CreateDirectory(".bfs");
            File.Copy(inputPath, targetPath, overwrite: true);
            Console.WriteLine("[Auth] service_account.json copied to .bfs/");

            // Активация через Drive API
            try
            {
                var uploader = new GoogleDriveUploader(targetPath);
                uploader.Touch();
                Console.WriteLine("[Auth] Google service account activated.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Auth] Activation failed: {ex.Message}");
                return;
            }

            Console.WriteLine("Введите Google Drive folder ID (или нажмите Enter, чтобы использовать root):");
            string? folderId = Console.ReadLine()?.Trim();

            string configPath = Path.Combine(".bfs", "config.json");
            if (File.Exists(configPath))
            {
                var config = Config.Load(configPath);
                config.Cloud = "GoogleDrive";
                if (!string.IsNullOrEmpty(folderId))
                {
                    config.CloudFolderId = folderId;
                    Console.WriteLine($"[Auth] GoogleDriveFolderId saved: {folderId}");
                }
                config.Save(configPath);
            }
            else
            {
                Console.WriteLine("[Auth] config.json not found — run init first.");
            }

            Console.WriteLine("[Auth] Complete.");
        }
    }
}
