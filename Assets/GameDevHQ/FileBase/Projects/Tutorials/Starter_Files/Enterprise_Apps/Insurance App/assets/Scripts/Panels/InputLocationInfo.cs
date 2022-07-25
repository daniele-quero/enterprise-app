using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class InputLocationInfo : BasePanel, IPanel
{
    [SerializeField]
    private RawImage _locationImage;
    [SerializeField]
    private TMP_InputField _locationNotesInput;
    [SerializeField]
    private TextMeshProUGUI _locationText;
    [SerializeField]
    private TextMeshProUGUI _errorText;
    [SerializeField]
    private MapsAPIHandler _mapsAPIHandler = new MapsAPIHandler();

    private void Awake()
    {
        ContinueButton.onClick.AddListener(() => ProcessInfo());
        ContinueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputPhoto));
    }

    private new void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(_mapsAPIHandler.GetGPSCoordinates(isErrorCallback));
        StartCoroutine(_mapsAPIHandler.DownloadMap(_locationImage, (int)_locationImage.rectTransform.sizeDelta.x, (int)_locationImage.rectTransform.sizeDelta.y, isErrorCallback));
        StartCoroutine(_mapsAPIHandler.GetLocation(_locationText, isErrorCallback));
        StartCoroutine(EnableButton());
    }
    
    public void isErrorCallback(bool isError)
    {
        if(isError)
            Debug.Log("Is error? -> " + isError);

        _errorText.enabled = isError;
    }

    private new IEnumerator EnableButton()
    {
        base.OnEnable();
        ContinueButton.interactable = false;
        string ignored;
        yield return new WaitUntil(() => HasResponse(out ignored));
        ContinueButton.interactable = !_errorText.enabled;
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
        InsuranceAppUIManager.Instance.activeCase.LocationNotes = _locationNotesInput.text;
        InsuranceAppUIManager.Instance.activeCase.LocationImage = _locationImage;
        InsuranceAppUIManager.Instance.activeCase.LocationImagePath = _mapsAPIHandler.mapPath;
    }
}

