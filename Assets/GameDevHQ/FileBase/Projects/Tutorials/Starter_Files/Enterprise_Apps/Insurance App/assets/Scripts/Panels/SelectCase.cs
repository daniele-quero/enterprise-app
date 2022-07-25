using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System;

public class SelectCase : MonoBehaviour, IPanel
{
    [SerializeField]
    private Button _accept;
    [SerializeField]
    private VerticalLayoutGroup _caseGroup;
    [SerializeField]
    private GameObject _selectedCaseObject;
    private int[] spacings = new int[] { 0, 0, -1020, -540, -170 };
    [SerializeField]
    private AWSCase _selected = null;

    public AWSCase Selected { get => _selected; set => _selected = value; }

    private void Awake()
    {
        _accept.onClick.AddListener(() => ProcessInfo());
        _accept.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.CaseOverview));
    }

    private void OnEnable()
    {
        CaseListSpacingAdjustemnt();
        InsuranceAppUIManager.Instance.StoredCases.ForEach(c => GameObject.Instantiate(_selectedCaseObject, _caseGroup.transform).GetComponent<SelectedCase>().Case = c);
        StartCoroutine(EnableButton(_accept, () => Selected != null));
    }

    private void CaseListSpacingAdjustemnt()
    {
        if (InsuranceAppUIManager.Instance.StoredCases.Count > 3)
            _caseGroup.spacing = spacings[4];
        else
            _caseGroup.spacing = spacings[InsuranceAppUIManager.Instance.StoredCases.Count];

    }

    private void OnDisable()
    {
        Selected = null;
        InsuranceAppUIManager.Instance.StoredCases.Clear();
    }

    private IEnumerator EnableButton(Button b, Func<bool> condition)
    {
        b.interactable = false;
        yield return new WaitUntil(condition);
        b.interactable = true;
    }

    public void ProcessInfo()
    {
        new CaseSerializer().AwsToApp(Selected, InsuranceAppUIManager.Instance.activeCase);
    }
}
