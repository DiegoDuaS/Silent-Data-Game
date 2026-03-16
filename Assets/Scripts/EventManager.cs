using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<int> OnHealthChanged;
    public static event Action<int> OnFileCollected;
    public static event Action<int> OnSecurityLevelChanged;
    public static event Action OnPhoneCollected;

    public static void TriggerHealthChanged(int currentHealth) => OnHealthChanged?.Invoke(currentHealth);

    public static void TriggerFileCollected(int currentTotal) => OnFileCollected?.Invoke(currentTotal);

    public static void TriggerSecurityLevelChanged(int newLevel) => OnSecurityLevelChanged?.Invoke(newLevel);

    public static void TriggerPhoneCollected() => OnPhoneCollected?.Invoke();
}