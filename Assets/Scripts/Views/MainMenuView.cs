using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    public Button pvpButton;
    public Button pveButton;

    public void Initialize(UnityAction onPvPButtonClicked, UnityAction onPvEButtonClicked)
    {
        pvpButton.onClick.AddListener(onPvPButtonClicked);
        pveButton.onClick.AddListener(onPvEButtonClicked);
    }
}