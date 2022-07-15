using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class InsuranceAppUIManager : MonoBehaviour
{
    #region Singleton
    private static InsuranceAppUIManager _instance;

    public static InsuranceAppUIManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogError("Missing UIManager");

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public AppCase activeCase;

    public List<AppCase> storedCases = new List<AppCase>();

    public List<GameObject> panels = new List<GameObject>();

    public void NavigateTo(Panels panel)
    {
        Select(panel).SetActive(true);

        SelectAllBut(panel).ForEach(p => p.SetActive(false));

        if(Panels.MainMenu.Equals(panel))
            Select(Panels.Frame).SetActive(false);
        else
            Select(Panels.Frame).SetActive(true);

        if (Panels.CaseOverview.Equals(panel))
            Select(Panels.Scroll).SetActive(true);
    }

    private GameObject Select(Panels panel)
    {
        return panels
            .Select(p => p)
            .Where(p1 => p1.name.Contains(panel.ToString()))
            .First();
    }

    private List<GameObject> SelectAllBut(Panels panel)
    {
        return panels
            .Select(p => p)
            .Where(p1 => !p1.name.Contains(panel.ToString()))
            .ToList();
    }

}
