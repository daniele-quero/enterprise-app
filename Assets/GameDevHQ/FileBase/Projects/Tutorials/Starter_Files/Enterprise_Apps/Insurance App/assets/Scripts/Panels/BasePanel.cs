using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BasePanel : MonoBehaviour
{
    public TextMeshProUGUI caseNumber;
    public Button continueButton;

    protected void OnEnable()
    {
        if (InsuranceAppUIManager.Instance.activeCase != null)
            caseNumber.text = InsuranceAppUIManager.Instance.activeCase.CaseNumber;
    }

    protected IEnumerator EnableButton()
    {
        yield return null;
    }
}
