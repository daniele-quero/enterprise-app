using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class SelectedCase : MonoBehaviour, ISelectHandler
{
    [SerializeField]
    private Button _button;
    [SerializeField]
    private AWSCase _case;
    [SerializeField]
    private TextMeshProUGUI _date,
      _location,
      _clientName;
    private SelectCase _selectCasePanel;

    public AWSCase Case { get => _case; set => _case = value; }

    public void OnSelect(BaseEventData eventData)
    {
        Debug.Log($"Case # {_case.CaseNumber} selected");
        _selectCasePanel.Selected = _case;
    }

    private void Awake()
    {
        _button.onClick.AddListener(() => _button.Select());
        _selectCasePanel = InsuranceAppUIManager.Instance.Select(Panels.SelectCase).GetComponent<SelectCase>();
    }

    private void Start()
    {
        StartCoroutine(OnGetCase());
    }

    private void FillCaseInfo()
    {
        _date.text = _case.Date;
        _location.text = _case.Location;
        _clientName.text = _case.ClientName;
    }

    private IEnumerator OnGetCase()
    {
        yield return new WaitUntil(() => _case?.ClientName != null);
        FillCaseInfo();
    }


}
