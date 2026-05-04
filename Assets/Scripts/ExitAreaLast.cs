using UnityEngine;
using UnityEngine.InputSystem;

public class ExitAreaLast : MonoBehaviour
{
    private bool isPlayerInArea = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = true;
            EventLevel1Manager.PlayerAtExitRange(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInArea = false;
            EventLevel1Manager.PlayerAtExitRange(false);
        }
    }

    private void Update()
    {
        if (isPlayerInArea && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        GameManager.Instance.ChangeScene("MainMenu", 1);
    }
}