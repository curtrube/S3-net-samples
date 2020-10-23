using System;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace S3Demo
{
    class Program
    {
        private const string bucketName = "imagegallerydemo";

        // Bucket region
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.USEast2;

        private static IAmazonS3 client;

        public static void Main()
        {
            client = new AmazonS3Client(bucketRegion);
            WritingAnObjectAsync().Wait();

            ListingObjectsAsync().Wait();
        }

        static async Task ListingObjectsAsync()
        {
            try
            {
                ListObjectsV2Request request = new ListObjectsV2Request
                {
                    BucketName = bucketName,
                    MaxKeys = 10
                };
                ListObjectsV2Response response;
                do
                {
                    response = await client.ListObjectsV2Async(request);

                    // Process the response.
                    foreach (S3Object entry in response.S3Objects)
                    {
                        Console.WriteLine($"key = {entry.Key} size = {entry.Size} lastModified={entry.LastModified}");
                    }

                    Console.WriteLine("Next Continuation Token: {0}", response.NextContinuationToken);
                    request.ContinuationToken = response.NextContinuationToken;
                } while (response.IsTruncated);
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                Console.WriteLine("S3 error occurred. Exception: " + amazonS3Exception.ToString());
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.ToString());
                Console.ReadKey();
            }
        }

        static async Task WritingAnObjectAsync()
        {
            try
            {
                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = "keys/TestKey3",
                    ContentBody = "sample text"
                };

                PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);

                string putObjectResponse;
                putObjectResponse = response1.ResponseMetadata.ToString();
                Console.WriteLine(putObjectResponse);

                // 2. Put the object-set ContentType and add metadata.
                // var putRequest2 = new PutObjectRequest
                // {
                //     BucketName = bucketName,
                //     Key = "keys/TestKey2",
                //     // FilePath = filePath,
                //     ContentType = "text/plain"
                // };

                // putRequest2.Metadata.Add("x-amz-meta-title", "someTitle");
                // PutObjectResponse response2 = await client.PutObjectAsync(putRequest2);
            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                    "Error encountered ***. Message:'{0}' when writing an object"
                    , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
        }
    }
}