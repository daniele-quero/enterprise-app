using UnityEngine;
using System.Numerics;
using UnityEngine.UI;
using System.Collections;
using System;

public class MainMenu : BasePanel
{
    [SerializeField]
    private Button _createCaseButton;
    [SerializeField]
    private Button _findCaseButton;

    private string GetNewCaseNumber()
    {
        string casenumber = (AWSManager.Instance.LastCaseNumber +1).ToString("R").PadLeft(10, '0');
        return casenumber;
    }

    private void Awake()
    {
        _createCaseButton.onClick.AddListener(() => CreateCase());
        _createCaseButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputClientInfo));

        _findCaseButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.FindCase));
    }

    private void Start()
    {
        StartCoroutine(EnableButton(_createCaseButton, ()=>AWSManager.Instance.LastCaseNumber > BigInteger.MinusOne));
        StartCoroutine(EnableButton(_findCaseButton, ()=> AWSManager.Instance.S3Objects !=null && AWSManager.Instance.S3Objects.Count>0));
    }

    private void CreateCase()
    {
        InsuranceAppUIManager.Instance.activeCase = new AppCase(GetNewCaseNumber());
        InsuranceAppUIManager.Instance.activeCase.Mode = CaseMode.Insert;
    }

    private IEnumerator EnableButton(Button b, Func<bool> condition)
    {
        b.interactable = false;
        yield return new WaitUntil(condition);
        b.interactable = true;
    }

    private new void OnEnable()
    {
        StartCoroutine(AWSManager.Instance.GetObjectList());
    }
}
