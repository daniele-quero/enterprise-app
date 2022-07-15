using UnityEngine;
using System.Numerics;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button createCaseButton;

    private string GetNewCaseNumber()
    {
        //use AWS
        BigInteger number = new BigInteger(Random.Range(0, 10000000000));
        string casenumber = "CASE NUMBER: " + number.ToString("R").PadLeft(10, '0');
        return casenumber;
    }

    private void Awake()
    {
        createCaseButton.onClick.AddListener(() => CreateCase());
        createCaseButton.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.InputClientInfo));
    }

    public void CreateCase()
    {
        InsuranceAppUIManager.Instance.activeCase = new AppCase(GetNewCaseNumber());
        InsuranceAppUIManager.Instance.activeCase.Mode = CaseMode.Insert;
    }


}
