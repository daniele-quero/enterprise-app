using UnityEngine;
using Amazon;
using Amazon.S3;
using Amazon.CognitoIdentity;
using Amazon.S3.Model;

public class AWSManager : MonoBehaviour
{



    private void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "eu-west-3:e515abf0-e040-4028-b9b2-b81dc4cf7aec", // Identity pool ID
            RegionEndpoint.EUWest3 // Region
            );

        AmazonS3Client S3Client = new AmazonS3Client(credentials, RegionEndpoint.EUWest3);
        ListBuckets(S3Client);

    }

    private void ListBuckets(AmazonS3Client S3Client)
    {
        S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log("Bucket: " + s3b.BucketName);
                });
            }
            else
            {
                Debug.Log("AWS Error: " + responseObject.Exception);
            }
        });
    }
}
