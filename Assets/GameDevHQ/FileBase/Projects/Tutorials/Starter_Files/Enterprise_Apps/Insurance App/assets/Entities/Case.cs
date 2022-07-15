using UnityEngine;
using System;

[System.Serializable]
public class Case
{
    [SerializeField]
    protected string _caseNumber;
    [SerializeField]
    protected string _date;
    [SerializeField]
    protected string _clientName;
    [SerializeField]
    protected string _location;
    [SerializeField]
    protected string _locationNotes;
    [SerializeField]
    protected string _photoNotes;

    [SerializeField]
    protected CaseMode _mode;

    public string ClientName { get => _clientName; set => _clientName = value; }
    public string Location { get => _location; set => _location = value; }
    public string LocationNotes { get => _locationNotes; set => _locationNotes = value; }
    public string PhotoNotes { get => _photoNotes; set => _photoNotes = value; }
    public string Date { get => _date; set => _date = value; }
    public string CaseNumber { get => _caseNumber; set => _caseNumber = value; }

    public CaseMode Mode { get => _mode; set => _mode = value; }

    public Case(string caseNumber)
    {
        _caseNumber = caseNumber ?? throw new ArgumentNullException(nameof(caseNumber));
    }
}

public enum CaseMode
{
    Insert,
    Retrieve
}
