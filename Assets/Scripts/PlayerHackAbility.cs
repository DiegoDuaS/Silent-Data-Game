using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerHackAbility : MonoBehaviour
{
    [SerializeField] private bool puedeHackear = false;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference hackAction;

    private void OnEnable()
    {
        EventManager.OnPhoneCollected += ActivarHackeo;
        if (hackAction != null)
            hackAction.action.Enable();
    }

    private void OnDisable()
    {
        EventManager.OnPhoneCollected -= ActivarHackeo;

        if (hackAction != null)
            hackAction.action.Disable();
    }

    private void ActivarHackeo()
    {
        puedeHackear = true;
    }

    void Update()
    {
        if (puedeHackear && hackAction != null && hackAction.action.triggered)
        {
            IntentarHackear();
        }
    }

    private void IntentarHackear()
    {
        Debug.Log("Hackeo");
    }
}