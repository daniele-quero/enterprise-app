using UnityEngine;
using TMPro;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class FindCase : MonoBehaviour, IPanel
{
    [SerializeField]
    private TMP_InputField _caseNumberInputField;
    [SerializeField]
    private Button _findButton;
    private CaseSerializer _cs;
    private List<MemoryStreamWrapper> _mswList = new List<MemoryStreamWrapper>();
    private MemoryStreamListWrapper _listWrapper = new MemoryStreamListWrapper();

    private void Awake()
    {
        _findButton.onClick.AddListener(() => ProcessInfo());
    }

    private void OnEnable()
    {
        InsuranceAppUIManager.Instance.StoredCases.Clear();
        _cs = new CaseSerializer();
    }

    private IEnumerator DeserializeOnDownloads(int results)
    {
        
        Debug.Log("Find Case Panel - Waiting for the streams...");
        yield return new WaitUntil(() => CheckStream(results));
        
        _listWrapper.MswList.ForEach(mw => InsuranceAppUIManager.Instance.StoredCases.Add(_cs.Deserialize(mw.Ms)));
        _mswList.Clear();
        _listWrapper.MswList = _mswList;

        Debug.Log($"Find Case Panel - {InsuranceAppUIManager.Instance.StoredCases.Count} cases stored");
        InsuranceAppUIManager.Instance.NavigateTo(Panels.SelectCase);
    }

    private bool CheckStream(int results)
    {
        if (_mswList.Count != results) return false;

        foreach (var m in _mswList)
            if (m.Ms == null || m.Ms.Length <= 0) return false;

        return true;
    }

    public void ProcessInfo()
    {
        var results = AWSManager.Instance.FilterObjects(_caseNumberInputField.text);
        _listWrapper.MswList = _mswList;
        AWSManager.Instance.GetObjects(results, _listWrapper);
        StartCoroutine(DeserializeOnDownloads(results.Count));
    }

}
