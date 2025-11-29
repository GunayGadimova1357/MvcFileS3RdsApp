using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon.S3.Model;

namespace MvcS3Files.Services;

public class S3FileService
{
    private readonly IAmazonS3 _s3Client;   
    private readonly string _bucketName;    

    public S3FileService(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _bucketName = config["AWS:BucketName"]!;
        //Console.WriteLine($"Using bucket: {_bucketName}");
    }
    
    public async Task<string> UploadAsync(IFormFile file)
    {
        // Console.WriteLine($"Uploading: {file.FileName}");

        if (file == null || file.Length == 0)
            throw new Exception("empty file");

        
        var key = $"uploads/{Guid.NewGuid()}_{file.FileName}";

        try
        {
            await using var stream = file.OpenReadStream();

            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = stream,
                Key = key,
                BucketName = _bucketName,
                ContentType = file.ContentType
            };

            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(uploadRequest);

            // Console.WriteLine($"Upload success Key: {key}");
            return key;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Upload ERROR: {ex.Message}");
            throw;
        }
    }
    
    public async Task DeleteAsync(string key)
    {
        //Console.WriteLine($"Deleting from bucket: {_bucketName}, key: {key}");

        try
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.DeleteObjectAsync(deleteRequest);

            // Console.WriteLine($"Successfully deleted: {key}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Delete ERROR: {ex.Message}");
            throw;
        }
    }
    
    public async Task<Stream> DownloadAsync(string key)
    {
        // Console.WriteLine($"Downloading: {key}");
        try
        {
            var response = await _s3Client.GetObjectAsync(_bucketName, key);
            return response.ResponseStream;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Download ERROR: {ex.Message}");
            throw;
        }
    }
}
