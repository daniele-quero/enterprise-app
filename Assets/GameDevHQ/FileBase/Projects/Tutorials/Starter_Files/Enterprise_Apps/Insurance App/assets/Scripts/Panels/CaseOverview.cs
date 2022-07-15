using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CaseOverview : BasePanel, IPanel
{
    public TextMeshProUGUI dateText;
    public TextMeshProUGUI locationText;
    public TextMeshProUGUI locationNotesText;
    public TextMeshProUGUI nameText;
    public RawImage photoImage;
    public RawImage locationImage;
    public Button button;
    public TextMeshProUGUI buttonText;
    public TextMeshProUGUI photoNotesText;
    public TextMeshProUGUI photoNotesTitle;
    private Vector3 photoPos;
    private Vector3 photoNotesTitlePos;
    private Vector3 photoNotesPos;
    public Vector3 shfit;

    private AWSCase _aws = null;
    CaseSerializer _cs = null;

    private void Awake()
    {
        photoPos = photoImage.rectTransform.localPosition;
        photoNotesTitlePos = photoNotesTitle.rectTransform.localPosition;
        photoNotesPos = photoNotesText.rectTransform.localPosition;
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
            dateText.text = InsuranceAppUIManager.Instance.activeCase.Date;
            locationText.text = InsuranceAppUIManager.Instance.activeCase.Location;
            locationNotesText.text = InsuranceAppUIManager.Instance.activeCase.LocationNotes;
            nameText.text = InsuranceAppUIManager.Instance.activeCase.ClientName;
            photoNotesText.text = InsuranceAppUIManager.Instance.activeCase.PhotoNotes;
            photoImage.rectTransform.sizeDelta = InsuranceAppUIManager.Instance.activeCase.Photo.rectTransform.sizeDelta;
            photoImage.texture = InsuranceAppUIManager.Instance.activeCase.Photo.texture;
            locationImage.texture = InsuranceAppUIManager.Instance.activeCase.LocationImage.texture;
            AdaptPhoto();               
        }
    }

    private void AdaptPhoto()
    {
        if (photoImage.rectTransform.sizeDelta.y > photoImage.rectTransform.sizeDelta.x)
        {
            photoImage.rectTransform.localPosition = photoPos + shfit;
            photoNotesTitle.rectTransform.localPosition = photoNotesTitlePos + shfit * 1.8f;
            photoNotesText.rectTransform.localPosition = photoNotesPos + shfit * 1.8f;
        }
        else
        {
            photoImage.rectTransform.localPosition = photoPos;
            photoNotesTitle.rectTransform.localPosition = photoNotesTitlePos;
            photoNotesText.rectTransform.localPosition = photoNotesPos;
        }
    }
    #endregion

    #region Button
    private void SetSubmitButton()
    {
        buttonText.text = "Submit";
        button.onClick.AddListener(() => ProcessInfo());
        StartCoroutine(PrepareForSubmition());
    }

    private IEnumerator PrepareForSubmition()
    {
        button.enabled = false;
        _cs = new CaseSerializer();
        yield return new WaitUntil(() => (_aws = _cs.AppToAws(InsuranceAppUIManager.Instance.activeCase)) != null);
        button.enabled = true;
    }

    private void SetBackButton()
    {
        buttonText.text = "back";
        button.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.MainMenu));
    }
    #endregion

    public void ProcessInfo()
    {
        _cs.Serialize(_aws);
    }
}
