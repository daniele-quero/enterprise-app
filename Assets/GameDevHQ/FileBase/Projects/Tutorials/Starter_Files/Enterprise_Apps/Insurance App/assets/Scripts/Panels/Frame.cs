using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Frame : MonoBehaviour
{
    public Button home;
    public Button back;
    public Button forth;
    public Button options;

    private void Awake()
    {
        home.onClick.AddListener(() => InsuranceAppUIManager.Instance.NavigateTo(Panels.MainMenu));
    }
}
