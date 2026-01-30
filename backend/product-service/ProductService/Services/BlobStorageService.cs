using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace ProductService.Services
{
    public class BlobStorageService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        private readonly BlobServiceClient _serviceClient;

        public BlobStorageService(IOptions<BlobOptions> options)
        {
            _connectionString = options.Value.ConnectionString ?? throw new ArgumentNullException("AzureBlob:ConnectionString");
            _containerName = options.Value.ContainerName;
            _serviceClient = new BlobServiceClient(_connectionString);
        }

        public async Task<(string blobName, string url)> UploadAsync(string fileName, Stream content, string contentType, CancellationToken ct = default)
        {
            var container = _serviceClient.GetBlobContainerClient(_containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob, cancellationToken: ct);
            var ext = Path.GetExtension(fileName);
            var blobName = $"{Guid.NewGuid():N}{ext}";
            var blob = container.GetBlobClient(blobName);
            await blob.UploadAsync(content, new BlobHttpHeaders{ ContentType = contentType }, cancellationToken: ct);
            return (blobName, blob.Uri.ToString());
        }
    }
}
