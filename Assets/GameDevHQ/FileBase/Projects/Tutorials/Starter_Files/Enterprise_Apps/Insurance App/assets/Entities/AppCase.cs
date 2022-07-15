using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AppCase : Case
{
    [SerializeField]
    private RawImage _photo;
    private string _photoPath;
    [SerializeField]
    private RawImage _locationImage;
    private string _locationImagePath;

    public AppCase(string caseNumber) : base(caseNumber)
    {
    }

    public RawImage Photo { get => _photo; set => _photo = value; }
    public string PhotoPath { get => _photoPath; set => _photoPath = value; }
    public RawImage LocationImage { get => _locationImage; set => _locationImage = value; }
    public string LocationImagePath { get => _locationImagePath; set => _locationImagePath = value; }
}

