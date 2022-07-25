using UnityEngine;
using Amazon;
using Amazon.S3;
using Amazon.CognitoIdentity;
using Amazon.S3.Model;
using System.IO;
using System.Numerics;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Text.RegularExpressions;
using Amazon.Runtime;

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

    [SerializeField]
    private string _mainBucketName;

    public BigInteger LastCaseNumber { get; private set; } = BigInteger.MinusOne;
    public List<S3Object> S3Objects { get; private set; }
    public List<Dictionary<string, string>> MetadataList { get; private set; }
    public string MainBucketName { get => _mainBucketName; set => _mainBucketName = value; }

    #region Init
    private void Awake()
    {
        _instance = this;
        AWSInit();
        ListBuckets(_S3Client);
        StartCoroutine(GetObjectList());
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
    }
    #endregion

    #region List Buckets
    public void ListBuckets(AmazonS3Client S3Client)
    {
        Debug.Log($"AWS Manager - Retrieving buckets");
        S3Client.ListBucketsAsync(new ListBucketsRequest(), (Amazon.Runtime.AmazonServiceResult<ListBucketsRequest, ListBucketsResponse> responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                responseObject.Response.Buckets.ForEach(s3b => Debug.Log($"AWS Manager - Bucket: {s3b.BucketName}"));

                if (responseObject.Response.Buckets.Count > 0)
                    MainBucketName = responseObject.Response.Buckets[0].BucketName;
            }
            else
                Debug.Log($"AWS Manager - AWS Error: {responseObject.Exception}");
        });
    }
    #endregion

    #region Get Objects
    public void GetObjects(List<S3Object> list, MemoryStreamListWrapper streams)
    {
        if (list == null)
        {
            Debug.LogWarning($"AWS Manager - List of S3Object to download is null: can't be.");
            return;
        }

        Debug.Log($"AWS Manager - About to download {list.Count} objects from {MainBucketName}");

        foreach (var o in list)
        {
            var msw = new MemoryStreamWrapper();
            GetObject(o.Key, msw);
            Debug.Log($"AWS Manager - Adding {o.Key} to the stream list");
            streams.MswList.Add(msw);
        }
    }

    public void GetObject(string key, MemoryStreamWrapper msw)
    {
        Debug.Log($"AWS Manager - Downloading {key}");
        _S3Client.GetObjectAsync(MainBucketName, key, (responseObj) =>
        {
            var response = responseObj.Response;
            if (response.ResponseStream != null)
            {
                Debug.Log($"AWS Manager - Download completed for {key}");
                byte[] data;
                using (StreamReader r = new StreamReader(response.ResponseStream))
                {

                    using (MemoryStream m = new MemoryStream())
                    {
                        var buffer = new byte[512];
                        var read = default(int);
                        while ((read = r.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                            m.Write(buffer, 0, read);

                        data = m.ToArray();
                        Debug.Log($"AWS Manager - {key} size is: {m.Length}");
                    }
                }

                msw.Ms = new MemoryStream(data);
                return;
            }
            else
                Debug.LogWarning($"AWS Manager - No content for key: {key}");
        });
    }

    #endregion

    #region List Objects
    public IEnumerator GetObjectList()
    {
        Debug.Log($"AWS Manager - Retreiving object list");
        float t0 = Time.time;
        yield return new WaitUntil(() => MainBucketName != null || Time.time == t0 + 20f);

        _S3Client.ListObjectsAsync(MainBucketName, (responseObject) =>
        {
            if (responseObject.Exception == null)
            {
                S3Objects = responseObject.Response.S3Objects.Select(o => o).OrderBy(o => o.Key).ToList();
                Debug.Log($"AWS Manager - {S3Objects.Count} objects in the bucket");

                if (S3Objects.Count > 0)
                    LastCaseNumber = GetCaseNumber(S3Objects[S3Objects.Count - 1].Key);
                else
                    LastCaseNumber = BigInteger.Zero;

                Debug.Log($"Last case is {LastCaseNumber}");
            }
            else
                Debug.LogWarning($"AWS Manager - Exception while listing the objects: {responseObject.Exception}");
        });
    }
    #endregion

    #region Post Object

    public void PostObject(string path, string bucketName, Action postCallback = null)
    {
        var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
        string fileName = GetKey(path);

        if (fileName == null)
        {
            Debug.LogWarning("AWS Manager - No File to post");
            return;
        }

        var request = new PostObjectRequest()
        {
            Bucket = bucketName,
            Key = fileName,
            InputStream = stream,
            CannedACL = S3CannedACL.Private,
            Region = RegionEndpoint.EUWest1,
        };

        Debug.Log("AWS Manager - Making HTTP post call with S3");

        _S3Client.PostObjectAsync(request, (responseObj) =>
        {
            if (responseObj.Exception == null)
            {
                Debug.Log($"AWS Manager - {responseObj.Request.Key} posted to bucket {responseObj.Request.Bucket}");
                postCallback?.Invoke();
            }
            else
                Debug.LogWarning($"AWS Manager - Exception while posting the result object: {responseObj.Exception}");
        });

    }
    #endregion

    #region Utilities
    private BigInteger GetCaseNumber(string key)
    {
        var m = Regex.Match(key, @"([0-9]{10})_casefile.dat");
        BigInteger num = BigInteger.Zero;
        BigInteger.TryParse(m.Groups[1].Value, out num);
        return num;
    }
    public string GetKey(string path)
    {
        var pathElements = path.Split(Path.DirectorySeparatorChar);
        string fileName = null;

        if (pathElements.Length > 0)
            fileName = pathElements[pathElements.Length - 1];

        return fileName;
    }

    public List<S3Object> FilterObjects(string query)
    {
        return S3Objects.Select(o => o).Where(o => o.Key.Contains(query)).ToList();
    }
    #endregion
}
