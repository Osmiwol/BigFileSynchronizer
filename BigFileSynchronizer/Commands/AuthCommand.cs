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

            // Try to activate Google Drive API
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

            string configPath = Path.Combine(".bfs", "config.json");
            if (!File.Exists(configPath))
            {
                Console.WriteLine("[Auth] config.json not found — run init first.");
                return;
            }

            var config = Config.Load(configPath);
            config.Cloud = "GoogleDrive";

            bool needInput = string.IsNullOrEmpty(config.CloudFolderId) || config.CloudFolderId == "null";

            if (needInput)
            {
                Console.WriteLine("Enter Google Drive folder ID (or press Enter to use root):");
                string? folderId = Console.ReadLine()?.Trim();

                config.CloudFolderId = string.IsNullOrEmpty(folderId) ? "root" : folderId;
                Console.WriteLine($"[Auth] GoogleDriveFolderId set to: {config.CloudFolderId}");
            }
            else
            {
                Console.WriteLine($"[Auth] Existing folder ID: {config.CloudFolderId}");
            }

            config.Save(configPath);
            Console.WriteLine("[Auth] Complete.");
        }
    }
}
