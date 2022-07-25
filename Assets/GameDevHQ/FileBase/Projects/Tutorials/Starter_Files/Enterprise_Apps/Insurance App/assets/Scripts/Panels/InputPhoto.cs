using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections;

public class InputPhoto : BasePanel, IPanel
{
    [SerializeField]
    private RawImage _photoImage;
    [SerializeField]
    private TMP_InputField _photoNotesInput;
    private string _photoPath;
    private bool _photoTaken = false;

    private void Awake()
    {
        ContinueButton.onClick.AddListener(() => ProcessInfo());
        ContinueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.CaseOverview));
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
        PhotoHandler.TakePicture(-1, _photoImage);
        _photoPath = PhotoHandler.Path;
        _photoTaken = _photoPath != null;
    }

    private new IEnumerator EnableButton()
    {
        base.OnEnable();
        ContinueButton.interactable = false;
        yield return new WaitUntil(() => _photoTaken);
        ContinueButton.interactable = true;
    }

    public void ProcessInfo()
    {
        InsuranceAppUIManager.Instance.activeCase.PhotoNotes = _photoNotesInput.text;
        InsuranceAppUIManager.Instance.activeCase.Photo = _photoImage;
        InsuranceAppUIManager.Instance.activeCase.PhotoPath = _photoPath;
    }

}
