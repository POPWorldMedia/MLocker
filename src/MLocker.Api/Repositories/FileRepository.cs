﻿using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace MLocker.Api.Repositories
{
    public interface IFileRepository
    {
        Task SaveFile(string fileName, byte[] bytes, string contentType);
        Task<Stream> GetFile(string fileName);
        Task<Tuple<Stream, string>> GetFileWithContentType(string fileName);
    }

    public class FileRepository : IFileRepository
    {
        private readonly IConfig _config;
        private const string ContainerName = "music";

        public FileRepository(IConfig config)
        {
            _config = config;
        }

        public async Task SaveFile(string fileName, byte[] bytes, string contentType)
        {
            await using var memoryStream = new MemoryStream(bytes);
            var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            await blobClient.DeleteIfExistsAsync();
            await blobClient.UploadAsync(memoryStream, new BlobHttpHeaders {ContentType = contentType});
        }

        public async Task<Stream> GetFile(string fileName)
        {
            var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var stream = await blobClient.OpenReadAsync();
            return stream;
        }

        public async Task<Tuple<Stream, string>> GetFileWithContentType(string fileName)
        {
            var serviceClient = new BlobServiceClient(_config.StorageConnectionString);
            var containerClient = serviceClient.GetBlobContainerClient(ContainerName);
            var blobClient = containerClient.GetBlobClient(fileName);
            var exists = await blobClient.ExistsAsync();
            if (!exists)
                return Tuple.Create<Stream, string>(null, null);
            var stream = await blobClient.OpenReadAsync();
            var contentType = (await blobClient.GetPropertiesAsync()).Value.ContentType;
            return Tuple.Create(stream, contentType);
        }
    }
}