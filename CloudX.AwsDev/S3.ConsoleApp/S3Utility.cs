using Amazon.S3;
using Amazon.S3.Model;

namespace S3.ConsoleApp
{
    internal class S3Utility
    {
        private readonly AmazonS3Client _client;
        private readonly string _bucketName;

        public S3Utility(AmazonS3Client client, string bucketName)
        {
            _client = client;
            _bucketName = bucketName;
        }

        public async Task GetLatestObjectVersionByDateAsync(string fileName, DateTime? date = null)
        {
            var listVersions = await _client.ListVersionsAsync(new ListVersionsRequest { BucketName = _bucketName, Prefix = fileName });

            if (!listVersions.Versions.Any())
            {
                throw new Exception($"No versions of the file '{fileName}' are found");
            }

            S3ObjectVersion? latestVersion = date is null
                ? listVersions.Versions.SingleOrDefault(x => x.IsLatest)
                : listVersions.Versions.Where(x => x.LastModified < date).OrderByDescending(x => x.LastModified).FirstOrDefault();

            if (latestVersion is null)
            {
                throw new Exception($"Latest version of the file '{fileName}' is null");
            }

            GetObjectRequest request = new GetObjectRequest { BucketName = _bucketName, Key = fileName, VersionId = latestVersion.VersionId };

            using GetObjectResponse getResponse = await _client.GetObjectAsync(request);
            using StreamReader reader = new StreamReader(getResponse.ResponseStream);
            Console.WriteLine(latestVersion.VersionId);
            Console.WriteLine(reader.ReadToEnd());
        }
    }
}
