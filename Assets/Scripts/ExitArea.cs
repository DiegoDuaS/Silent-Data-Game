using UnityEngine;
using UnityEngine.InputSystem;

public class ExitArea : MonoBehaviour
{
    private bool isPlayerInArea = false;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference interactAction;

    private void OnEnable()
    {
        if (interactAction != null)
            interactAction.action.Enable();
    }

    private void OnDisable()
    {
        if (interactAction != null)
            interactAction.action.Disable();
    }

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
        if (isPlayerInArea && interactAction != null && interactAction.action.triggered)
        {
            HandleLevelWin();
        }
    }

    private void HandleLevelWin()
    {
        if (LevelManager.Instance != null && LevelManager.Instance.GetSecurityLevel() > 0)
        {
            LevelManager.Instance.MissionAccomplished();
        }
        
    }
}