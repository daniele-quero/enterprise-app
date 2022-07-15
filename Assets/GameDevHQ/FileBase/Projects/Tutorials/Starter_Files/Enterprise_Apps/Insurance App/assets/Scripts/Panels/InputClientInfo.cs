using System;
using System.Collections;
using System.IO;
using System.Text;
using TMPro;
using UnityEngine;

public class InputClientInfo : BasePanel, IPanel
{
    public TMP_InputField firstNameInput;
    public TMP_InputField lastNameInput;
    

    private void Awake()
    {
        continueButton.onClick.AddListener(() => ProcessInfo());
        continueButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputLocation));
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
        continueButton.interactable = false;
        yield return new WaitUntil(() => CheckFields());
        continueButton.interactable = true;
    }

    private bool CheckFields()
    {
        return !String.IsNullOrWhiteSpace(firstNameInput.text) && !String.IsNullOrWhiteSpace(lastNameInput.text);
    }

    public void ProcessInfo()
    {
        InsuranceAppUIManager.Instance.activeCase.ClientName = firstNameInput.text + " " + lastNameInput.text;
        InsuranceAppUIManager.Instance.activeCase.Date = DateTime.Now.ToString();
        InsuranceAppUIManager.Instance.activeCase.Mode = CaseMode.Insert;
    }

}
