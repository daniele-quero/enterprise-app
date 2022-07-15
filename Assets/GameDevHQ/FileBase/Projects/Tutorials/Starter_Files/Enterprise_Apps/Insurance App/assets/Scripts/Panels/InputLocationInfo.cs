using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputLocationInfo : BasePanel, IPanel
{
    public RawImage locationImage;
    public TMP_InputField locationNotesInput;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI errorText;
    public MapsAPIHandler _mapsAPIHandler = new MapsAPIHandler();

    private void Awake()
    {
        continueButton.onClick.AddListener(() => ProcessInfo());
        continueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputPhoto));
    }

    private new void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(_mapsAPIHandler.GetGPSCoordinates(isErrorCallback));
        StartCoroutine(_mapsAPIHandler.DownloadMap(locationImage, (int)locationImage.rectTransform.sizeDelta.x, (int)locationImage.rectTransform.sizeDelta.y, isErrorCallback));
        StartCoroutine(_mapsAPIHandler.GetLocation(locationText, isErrorCallback));
        StartCoroutine(EnableButton());
    }
    
    public void isErrorCallback(bool isError)
    {
        Debug.Log("Is error? -> " + isError);
        errorText.enabled = isError;
    }

    private new IEnumerator EnableButton()
    {
        base.OnEnable();
        continueButton.interactable = false;
        string ignored;
        yield return new WaitUntil(() => HasResponse(out ignored));
        continueButton.interactable = !errorText.enabled;
    }

    private bool HasResponse(out string json)
    {
        return _mapsAPIHandler.jsonResponses.TryGetValue(API.geocode, out json);
    }

    public void ProcessInfo()
    {
        string json;
        string location = HasResponse(out json) ? _mapsAPIHandler.GetAddress(json) : "No Location Retrieved";
        InsuranceAppUIManager.Instance.activeCase.Location = location;
        InsuranceAppUIManager.Instance.activeCase.LocationNotes = locationNotesInput.text;
        InsuranceAppUIManager.Instance.activeCase.LocationImage = locationImage;
        InsuranceAppUIManager.Instance.activeCase.LocationImagePath = _mapsAPIHandler.mapPath;
    }
}

