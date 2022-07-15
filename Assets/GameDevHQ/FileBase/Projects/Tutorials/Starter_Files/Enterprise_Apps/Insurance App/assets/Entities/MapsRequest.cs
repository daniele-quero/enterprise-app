using UnityEngine;

[System.Serializable]
public class MapsRequest
{
    [SerializeField]
    private int zoom;
    [SerializeField]
    private float latitude;
    [SerializeField]
    private float longitude;
    [SerializeField]
    private int xSize;
    [SerializeField]
    private int ySize;
    [SerializeField]
    private string url;

    public int Zoom { get => zoom; set => zoom = value; }
    public float Latitude { get => latitude; set => latitude = value; }
    public float Longitude { get => longitude; set => longitude = value; }
    public int XSize { get => xSize; set => xSize = value; }
    public int YSize { get => ySize; set => ySize = value; }
    public string Url { get => url; set => url = value; }
}
