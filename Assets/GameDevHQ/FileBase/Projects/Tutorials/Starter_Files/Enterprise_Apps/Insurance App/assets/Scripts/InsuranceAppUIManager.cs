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

    public List<AWSCase> StoredCases { get => _storedCases; set => _storedCases = value; }
    public List<GameObject> Panels { get => _panels; set => _panels = value; }
    public List<Panels> History { get => _history; set => _history = value; }

    private void Awake()
    {
        _instance = this;
    }
    #endregion

    public AppCase activeCase;

    [SerializeField]
    private List<AWSCase> _storedCases = new List<AWSCase>();
    [SerializeField]
    private List<Panels> _history = new List<Panels>() { global::Panels.MainMenu };
    [SerializeField]
    private List<GameObject> _panels = new List<GameObject>();

    public void NavigateTo(Panels panel, bool browsingHistory)
    {
        Select(panel).SetActive(true);

        SelectAllBut(panel).ForEach(p => p.SetActive(false));

        if (global::Panels.MainMenu.Equals(panel))
            Select(global::Panels.Frame).SetActive(false);
        else
            Select(global::Panels.Frame).SetActive(true);

        if (global::Panels.CaseOverview.Equals(panel))
            Select(global::Panels.OverviewScroll).SetActive(true);
        else if (global::Panels.SelectCase.Equals(panel))
            Select(global::Panels.SelectScroll).SetActive(true);

        if (!browsingHistory)
            History.Add(panel);

    }

    public void NavigateTo(Panels panel)
    {
        NavigateTo(panel, false);
    }

    public GameObject Select(Panels panel)
    {
        return Panels
            .Select(p => p)
            .Where(p1 => p1.name.Contains(panel.ToString()))
            .First();
    }

    private List<GameObject> SelectAllBut(Panels panel)
    {
        return Panels
            .Select(p => p)
            .Where(p1 => !p1.name.Contains(panel.ToString()))
            .ToList();
    }

}
