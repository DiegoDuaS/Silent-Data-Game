using UnityEngine;
using TMPro;

public class UILevel1Manager : MonoBehaviour
{
    [SerializeField] private GameObject pressText;

    private void OnEnable()
    {
        EventLevel1Manager.OnPlayerAtExitRange += ToggleExitUI;
    }

    private void OnDisable()
    {
        EventLevel1Manager.OnPlayerAtExitRange -= ToggleExitUI;
    }

    private void ToggleExitUI(bool show)
    {
        if (pressText != null) pressText.SetActive(show);
    }
}