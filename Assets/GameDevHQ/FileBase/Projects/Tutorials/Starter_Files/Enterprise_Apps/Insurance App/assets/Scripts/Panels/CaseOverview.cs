using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CaseOverview : BasePanel, IPanel
{
    [SerializeField]
    private TextMeshProUGUI _dateText,
      _locationText,
      _locationNotesText,
      _nameText,
      _buttonText,
      _photoNotesText,
      _photoNotesTitle;

    [SerializeField]
    private RawImage _photoImage,
      _locationImage;

    [SerializeField]
    private Button _button;

    private Vector3 _photoPos,
      _photoNotesTitlePos,
      _photoNotesPos;

    public Vector3 shfit;

    private AWSCase _aws = null;
    CaseSerializer _cs = null;

    public RawImage PhotoImage { get => _photoImage; }
    public RawImage LocationImage { get => _locationImage; }

    private void Awake()
    {
        _photoPos = _photoImage.rectTransform.localPosition;
        _photoNotesTitlePos = _photoNotesTitle.rectTransform.localPosition;
        _photoNotesPos = _photoNotesText.rectTransform.localPosition;
    }

    private new void OnEnable()
    {
        base.OnEnable();
        FillPanelFromCase();

        if (InsuranceAppUIManager.Instance.activeCase == null || CaseMode.Insert != InsuranceAppUIManager.Instance.activeCase.Mode)
            SetBackButton();
        else
            SetSubmitButton();
    }

    #region Case Info Filling
    private void FillPanelFromCase()
    {
        if (InsuranceAppUIManager.Instance.activeCase != null)
        {
            _dateText.text = InsuranceAppUIManager.Instance.activeCase.Date;
            _locationText.text = InsuranceAppUIManager.Instance.activeCase.Location;
            _locationNotesText.text = InsuranceAppUIManager.Instance.activeCase.LocationNotes;
            _nameText.text = InsuranceAppUIManager.Instance.activeCase.ClientName;
            _photoNotesText.text = InsuranceAppUIManager.Instance.activeCase.PhotoNotes;
            _photoImage.rectTransform.sizeDelta = InsuranceAppUIManager.Instance.activeCase.Photo.rectTransform.sizeDelta;
            _photoImage.texture = InsuranceAppUIManager.Instance.activeCase.Photo.texture;
            _locationImage.texture = InsuranceAppUIManager.Instance.activeCase.LocationImage.texture;
            AdaptPhoto();
        }
    }

    private void AdaptPhoto()
    {
        if (_photoImage.rectTransform.sizeDelta.y > _photoImage.rectTransform.sizeDelta.x)
        {
            _photoImage.rectTransform.localPosition = _photoPos + shfit;
            _photoNotesTitle.rectTransform.localPosition = _photoNotesTitlePos + shfit * 1.8f;
            _photoNotesText.rectTransform.localPosition = _photoNotesPos + shfit * 1.8f;
        }
        else
        {
            _photoImage.rectTransform.localPosition = _photoPos;
            _photoNotesTitle.rectTransform.localPosition = _photoNotesTitlePos;
            _photoNotesText.rectTransform.localPosition = _photoNotesPos;
        }
    }
    #endregion

    #region Button
    private void SetSubmitButton()
    {
        _buttonText.text = "Submit";
        _button.onClick.AddListener(() => ProcessInfo());
        StartCoroutine(PrepareForSubmition());
    }

    private IEnumerator PrepareForSubmition()
    {
        _button.enabled = false;
        _cs = new CaseSerializer();
        yield return new WaitUntil(() => (_aws = _cs.AppToAws(InsuranceAppUIManager.Instance.activeCase)) != null);
        _button.enabled = true;
    }

    private void SetBackButton()
    {
        _buttonText.text = "back";
        _button.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.MainMenu));
    }

    public void ProcessInfo()
    {
        string path = _cs.Serialize(_aws);
        string bucketName = AWSManager.Instance.MainBucketName;
        AWSManager.Instance.PostObject(path, bucketName, BackToMainMenu);
    }

    private void BackToMainMenu()
    {
    
        InsuranceAppUIManager.Instance.NavigateTo(Panels.MainMenu);
    }
    #endregion
}
