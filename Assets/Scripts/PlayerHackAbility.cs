using UnityEngine;
using UnityEngine.InputSystem; 

public class PlayerHackAbility : MonoBehaviour
{
    [SerializeField] private bool puedeHackear = false;

    private void OnEnable()
    {
        EventManager.OnPhoneCollected += ActivarHackeo;
    }

    private void OnDisable()
    {
        EventManager.OnPhoneCollected -= ActivarHackeo;
    }

    private void ActivarHackeo()
    {
        puedeHackear = true;
    }

    void Update()
    {
        if (puedeHackear && Keyboard.current.hKey.wasPressedThisFrame)
        {
            IntentarHackear();
        }
    }

    private void IntentarHackear()
    {
        Debug.Log("Hackeo");
    }
}