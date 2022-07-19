[System.Serializable]
public class AWSCase : Case
{
    private byte[] photo;
    private byte[] locationImage;

    public byte[] Photo { get => photo; set => photo = value; }
    public byte[] LocationImage { get => locationImage; set => locationImage = value; }

    public AWSCase(string caseNumber) : base(caseNumber)
    {
    }
}

