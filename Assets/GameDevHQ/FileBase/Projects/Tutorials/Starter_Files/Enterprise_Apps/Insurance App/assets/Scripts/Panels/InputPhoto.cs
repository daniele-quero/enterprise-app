using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

public class InputPhoto : BasePanel, IPanel
{
    public RawImage photoImage;
    public TMP_InputField photoNotesInput;
    private string _photoPath;
    private bool _photoTaken = false;

    private void Awake()
    {
        continueButton.onClick.AddListener(() => ProcessInfo());
        continueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.CaseOverview));
    }

    private new void OnEnable()
    {
        base.OnEnable();
#if !UNITY_EDITOR
        StartCoroutine(EnableButton());
#endif
    }

    private void TakePhoto()
    {
        PhotoHandler.TakePicture(-1, photoImage);
        _photoPath = PhotoHandler.Path;
        _photoTaken = _photoPath != null;
    }

    private new IEnumerator EnableButton()
    {
        base.OnEnable();
        continueButton.interactable = false;
        yield return new WaitUntil(() => _photoTaken);
        continueButton.interactable = true;
    }

    public void ProcessInfo()
    {
        InsuranceAppUIManager.Instance.activeCase.PhotoNotes = photoNotesInput.text;
        InsuranceAppUIManager.Instance.activeCase.Photo = photoImage;
        InsuranceAppUIManager.Instance.activeCase.PhotoPath = _photoPath;
    }

}
