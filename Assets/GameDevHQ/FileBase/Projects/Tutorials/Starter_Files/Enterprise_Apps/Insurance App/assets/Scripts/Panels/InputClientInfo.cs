using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class InputClientInfo : BasePanel, IPanel
{
    [SerializeField]
    private TMP_InputField _firstNameInput;
    [SerializeField]
    private TMP_InputField _lastNameInput;
    

    private void Awake()
    {
        ContinueButton.onClick.AddListener(() => ProcessInfo());
        ContinueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputLocation));
    }

    private new void OnEnable()
    {
        base.OnEnable();
#if !UNITY_EDITOR
        StartCoroutine(EnableButton());
#endif
    }

    private new IEnumerator EnableButton()
    {
        base.OnEnable();
        ContinueButton.interactable = false;
        yield return new WaitUntil(() => CheckFields());
        ContinueButton.interactable = true;
    }

    private bool CheckFields()
    {
        return !String.IsNullOrWhiteSpace(_firstNameInput.text) && !String.IsNullOrWhiteSpace(_lastNameInput.text);
    }

    public void ProcessInfo()
    {
        InsuranceAppUIManager.Instance.activeCase.ClientName = _firstNameInput.text + " " + _lastNameInput.text;
        InsuranceAppUIManager.Instance.activeCase.Date = DateTime.Now.ToString();
        InsuranceAppUIManager.Instance.activeCase.Mode = CaseMode.Insert;
    }

}
