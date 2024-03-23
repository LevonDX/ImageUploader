using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using ImageUploader.Services.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageUploader.Services.Concrete
{
    public class AzureBlobImageSaver : IImageSaver
    {
        private const string ConnectionString = "BlobEndpoint=https://march23test.blob.core.windows.net/;QueueEndpoint=https://march23test.queue.core.windows.net/;FileEndpoint=https://march23test.file.core.windows.net/;TableEndpoint=https://march23test.table.core.windows.net/;SharedAccessSignature=sv=2022-11-02&ss=bfqt&srt=sco&sp=rwdlacupiytfx&se=2025-03-26T16:14:41Z&st=2024-03-23T08:14:41Z&spr=https&sig=JgtpGlusjnZt2txVbhVhsbOLnpzT4gsIg3u2Ge1aApk%3D";

        public async Task SaveImageAsync(string fileName, string container)
        {
            // save 'fileName' to Azure Blob Storage
            if (fileName == null)
            {
                throw new ArgumentNullException();
            }
            if (container == null)
            {
                throw new ArgumentException();
            }

            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);
            
            using (var fileStream = System.IO.File.OpenRead(fileName))
            {
               await blobClient.UploadAsync(fileStream, true);
            }
        }

        public async Task<string> SaveImageAsync(System.IO.Stream fileStream, string fileName, string container)
        {
            // save 'fileStream' to Azure Blob Storage
            if (fileStream == null)
            {
                throw new ArgumentNullException();
            }
            if (container == null)
            {
                throw new ArgumentException();
            }

            BlobServiceClient blobServiceClient = new BlobServiceClient(ConnectionString);

            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(container);

            await containerClient.CreateIfNotExistsAsync();

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            fileStream.Position = 0;
            await blobClient.UploadAsync(fileStream, true);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = container,
                BlobName = fileName,
                Resource = "b", // For blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Optional: to avoid clock skew issues
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);
            Thread.Sleep(1000);
            string sas = blobClient.GenerateSasUri(sasBuilder).ToString();

            return sas;
        }
    }
}