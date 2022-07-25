using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class BasePanel : MonoBehaviour
{
    [SerializeField]
    protected TextMeshProUGUI _caseNumber;
    [SerializeField]
    protected Button _continueButton;

    public TextMeshProUGUI CaseNumber { get => _caseNumber; set => _caseNumber = value; }
    public Button ContinueButton { get => _continueButton; set => _continueButton = value; }

    protected void OnEnable()
    {
        if (CaseNumber !=null && InsuranceAppUIManager.Instance.activeCase != null)
            CaseNumber.text = "Case Number: " + InsuranceAppUIManager.Instance.activeCase.CaseNumber;
    }

    protected IEnumerator EnableButton()
    {
        yield return null;
    }
}
