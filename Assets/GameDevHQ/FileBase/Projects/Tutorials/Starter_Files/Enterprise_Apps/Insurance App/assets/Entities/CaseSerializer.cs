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

    public AppCase AwsToApp(AWSCase @case)
    {
        var app = new AppCase(@case.CaseNumber);
        app.ClientName = @case.ClientName;
        app.Date = @case.Date;
        app.Location = @case.Location;
        app.LocationNotes = @case.LocationNotes;
        app.Mode = @case.Mode;
        app.PhotoNotes = @case.PhotoNotes;
        //Texture2D loadTexture = NativeCamera.LoadImageAtPath("C:/Users/danio/AppData/LocalLow/DefaultCompany/RainyDay - Insurance Appprova.png");
        return app;
    }

    public string Serialize(AWSCase @case)
    {

        string subFolderPath = Application.persistentDataPath + Path.DirectorySeparatorChar + @case.CaseNumber;
        string path = subFolderPath + Path.DirectorySeparatorChar + @case.CaseNumber + "_casefile.dat";

        if (!Directory.Exists(subFolderPath))
            Directory.CreateDirectory(subFolderPath);

        FileStream file = File.Create(path);
        _bf.Serialize(file, @case);
        file.Close();
        return path;
    }

    public static Texture2D ToTexture2D(Texture texture)
    {
        return Texture2D.CreateExternalTexture(
            texture.width,
            texture.height,
            TextureFormat.RGBA32,
            false, true,
            texture.GetNativeTexturePtr());
    }
}
