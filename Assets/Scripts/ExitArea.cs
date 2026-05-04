using UnityEngine;
using UnityEngine.InputSystem;

public class ExitArea : MonoBehaviour
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
        // Si el jugador está en el área y presiona la tecla E
        if (isPlayerInArea && Keyboard.current.eKey.wasPressedThisFrame)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        Debug.Log("[ExitArea] Nivel completado. Transicionando al Nivel 2...");

        GameManager.Instance.ChangeScene("Level2", 1);
    }
}