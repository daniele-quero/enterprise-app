using Amazon.S3.Model;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

class CaseSerializer
{
    private BinaryFormatter _bf;

    public CaseSerializer()
    {
        _bf = new BinaryFormatter();
    }

    public AWSCase AppToAws(AppCase @case)
    {
        var aws = new AWSCase(@case.CaseNumber);
        aws.ClientName = @case.ClientName;
        aws.Date = @case.Date;
        aws.Location = @case.Location;
        aws.LocationNotes = @case.LocationNotes;
        aws.Mode = @case.Mode;
        aws.PhotoNotes = @case.PhotoNotes;
        aws.LocationImage = NativeCamera.LoadImageAtPath(@case.LocationImagePath, -1, false).EncodeToPNG();
#if !UNITY_EDITOR
        aws.Photo = NativeCamera.LoadImageAtPath(@case.PhotoPath).EncodeToPNG();
#endif
        return aws;
    }

    public AppCase AwsToApp(AWSCase @case, AppCase app)
    {
        app.CaseNumber = @case.CaseNumber;
        app.ClientName = @case.ClientName;
        app.Date = @case.Date;
        app.Location = @case.Location;
        app.LocationNotes = @case.LocationNotes;
        app.Mode = @case.Mode;
        app.PhotoNotes = @case.PhotoNotes;
        app.LocationImage = InsuranceAppUIManager.Instance.Select(Panels.CaseOverview).GetComponent<CaseOverview>().LocationImage;
        app.LocationImage.texture = DecodeFromBytes(@case.LocationImage);
        app.Photo = InsuranceAppUIManager.Instance.Select(Panels.CaseOverview).GetComponent<CaseOverview>().PhotoImage;
        app.Photo.texture = DecodeFromBytes(@case.Photo);
        app.Mode = @case.Mode;
        return app;
    }

    public string Serialize(AWSCase @case)
    {
        string path = GetPath(@case);

        if (!Directory.Exists(GetSubfolder(@case)))
            Directory.CreateDirectory(GetSubfolder(@case));

        FileStream file = File.Create(path);
        _bf.Serialize(file, @case);
        file.Seek(0, SeekOrigin.Begin);
        file.Close();
        return path;
    }

    public AWSCase Deserialize(MemoryStream ms)
    {
        Debug.Log("Case Serializer - Deserializing");

        AWSCase c = (AWSCase)_bf.Deserialize(ms);
        c.Mode = CaseMode.Retrieve;

        Debug.Log($"Case Serializer - Done. Test Name is: { c.ClientName}");

        Debug.Log($"Case Serializer - Disposing of the stream ({ms.Length} bytes)");
        ms.Dispose();
        return c;
    }

    public string GetSubfolder(AWSCase @case)
    {
        return Application.persistentDataPath + Path.DirectorySeparatorChar + @case.CaseNumber;
    }

    public string GetPath(AWSCase @case)
    {
        return GetSubfolder(@case) + Path.DirectorySeparatorChar + GetFileName(@case.CaseNumber);
    }

    public string GetFileName(string caseNum)
    {
        return caseNum + "_casefile.dat";
    }

    public static Texture2D DecodeFromBytes(byte[] bytes)
    {
        Texture2D t = new Texture2D(2, 2);
        t.LoadImage(bytes);
        return t;
    }
}
