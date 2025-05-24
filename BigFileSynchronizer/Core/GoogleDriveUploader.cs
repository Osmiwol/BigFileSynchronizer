using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using File = Google.Apis.Drive.v3.Data.File;

namespace BigFileSynchronizer.Core
{
    public class GoogleDriveUploader
    {
        private readonly DriveService _driveService;
        private readonly string _uploadFolderId;

        public GoogleDriveUploader(string serviceAccountPath, string? uploadFolderId = null)
        {
            if (!System.IO.File.Exists(serviceAccountPath))
                throw new FileNotFoundException("Service account JSON not found", serviceAccountPath);

            var credential = GoogleCredential.FromFile(serviceAccountPath)
                .CreateScoped(DriveService.Scope.DriveFile);

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "BigFileSynchronizer"
            });

            _uploadFolderId = uploadFolderId ?? "root";
        }
        public DriveService GetDriveService()
        {
            return _driveService;
        }

        public string UploadFile(string localPath)
        {
            var fileMetadata = new File
            {
                Name = Path.GetFileName(localPath),
                Parents = new[] { _uploadFolderId }
            };

            using var stream = new FileStream(localPath, FileMode.Open, FileAccess.Read);

            var request = _driveService.Files.Create(fileMetadata, stream, "application/zip");
            request.Fields = "id";
            var result = request.Upload();

            if (result.Status != UploadStatus.Completed)
                throw new Exception($"Upload failed: {result.Exception?.Message}");

            return request.ResponseBody.Id;
        }

        public void DownloadFile(string fileId, string outputPath)
        {
            using var stream = new MemoryStream();
            var request = _driveService.Files.Get(fileId);
            request.MediaDownloader.ProgressChanged += p =>
            {
                if (p.Status == Google.Apis.Download.DownloadStatus.Failed)
                    throw new Exception("Download failed.");
            };

            request.Download(stream);
            stream.Position = 0;

            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            using var fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(fileStream);
        }

        public void Touch()
        {
            var listRequest = _driveService.Files.List();
            listRequest.PageSize = 1;
            listRequest.Fields = "files(id, name)";
            listRequest.Q = "trashed = false";
            listRequest.Execute();
        }

        

    }
}
