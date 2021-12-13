// See https://aka.ms/new-console-template for more information
using Amazon;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;
using S3.ConsoleApp;

Console.WriteLine("GetLatestObjectVersionByDate");
var bucketName = "siarhei-hanko-task2";
var fileName = "file1.txt";
var profileName = "readAccessS3";

try
{
    var sharedCredFile = new SharedCredentialsFile();
    if (!sharedCredFile.TryGetProfile(profileName, out var profile))
    {
        throw new Exception($"Error: couldn't get aws profile: '{profileName}'");
    }

    AWSCredentialsFactory.TryGetAWSCredentials(profile, sharedCredFile, out var credentials);
    var s3Client = new AmazonS3Client(credentials, RegionEndpoint.EUWest1);

    var utility = new S3Utility(s3Client, bucketName);

    // get latest file without specifying any date.
    // Expected result: aLq.VLAEFJmRteXhJQW24R1YTYQrb9Ia
    //                  file1
    //                  v3
    await utility.GetLatestObjectVersionByDateAsync(fileName);

    // get latest file not newer than date = 2021-12-13T20:00:00.
    // Expected result: DqCNBtiSZjBElnriTelWo8f0Iyg5l1J8,
    //                  file1,
    //                  v2
    var dateString = "2021-12-13T20:00:00";

    if (DateTime.TryParse(dateString, out var date))
    {
        await utility.GetLatestObjectVersionByDateAsync(fileName, date);
    }
    else
    {
        throw new Exception($"Error: couldn't parse string: {dateString} to DateTime");
    }
}
catch (Exception e)
{
    Console.WriteLine(e);
}