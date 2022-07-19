using UnityEngine;
using Amazon;
using Amazon.S3;
using Amazon.CognitoIdentity;
using Amazon.S3.Model;
using System.IO;
using System.Numerics;

public class AWSManager : MonoBehaviour
{

    #region Singleton
    private static AWSManager _instance;

    public static AWSManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Missing AWSManager");

            return _instance;
        }
    }
    #endregion


    private AmazonS3Client _S3Client;
    private string _mainBucketName;
    private BigInteger _lastCaseNumber = 0l;

    public string MainBucketName { get => _mainBucketName; }
    public BigInteger LastCaseNumber { get => _lastCaseNumber; }

    private void Awake()
    {
        _instance = this;
        AWSInit();

        ListBuckets(_S3Client);
    }

    private void AWSInit()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            "eu-west-1:c895b3bb-3e74-45d8-913b-ac354985936d", // Identity pool ID
            RegionEndpoint.EUWest1 // Region
            );
        _S3Client = new AmazonS3Client(credentials, RegionEndpoint.EUWest1);
        _lastCaseNumber = 0l;
    }

    public void ListBuckets(AmazonS3Client S3Client)
    {
        S3Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach((s3b) =>
                {
                    Debug.Log("Bucket: " + s3b.BucketName);
                });

                if (responseObject.Response.Buckets.Count > 0)
                    _mainBucketName = responseObject.Response.Buckets[0].BucketName;
            }
            else
            {
                Debug.Log("AWS Error: " + responseObject.Exception);
            }
        });
    }

    //private void GetObject()
    //{
    //    ResultText.text = string.Format("fetching {0} from bucket {1}", SampleFileName, S3BucketName);
    //    Client.GetObjectAsync(S3BucketName, SampleFileName, (responseObj) =>
    //    {
    //        string data = null;
    //        var response = responseObj.Response;
    //        if (response.ResponseStream != null)
    //        {
    //            using (StreamReader reader = new StreamReader(response.ResponseStream))
    //            {
    //                data = reader.ReadToEnd();
    //            }

    //            ResultText.text += "\n";
    //            ResultText.text += data;
    //        }
    //    });
    //}

    public void PostObject(string path, string bucketName)
    {
        var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        Debug.Log(path);
        var pathElements = path.Split(Path.DirectorySeparatorChar);
        string fileName = null;

        if (pathElements.Length > 0)
            fileName = pathElements[pathElements.Length - 1];
        else
        {
            Debug.LogWarning("No File to post");
            return;
        }

        var request = new PostObjectRequest()
        {
            Bucket = bucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = RegionEndpoint.EUWest1
        };

        Debug.Log("Making HTTP post call with S3");

        _S3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log( string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket)) ;
            }
            else
            {
                Debug.LogWarning(string.Format("Exception while posting the result object: {0}", responseObj.Response.HttpStatusCode.ToString()));
            }
        });


    }

}
