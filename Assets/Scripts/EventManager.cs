using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<int> OnHealthChanged;
    public static event Action<int> OnFileCollected;
    public static event Action<int> OnSecurityLevelChanged;
    public static event Action OnPhoneCollected;
    public static event Action OnCheckpointReached;
    public static event Action<GameObject> OnEnemyHit;
    public static event Action<string> OnItemRestored;


    public static void TriggerHealthChanged(int currentHealth) => OnHealthChanged?.Invoke(currentHealth);
    public static void TriggerFileCollected(int currentTotal) => OnFileCollected?.Invoke(currentTotal);
    public static void TriggerSecurityLevelChanged(int newLevel) => OnSecurityLevelChanged?.Invoke(newLevel);
    public static void TriggerPhoneCollected() => OnPhoneCollected?.Invoke();
    public static void TriggerCheckpointReached() => OnCheckpointReached?.Invoke();

    public static void TriggerItemRestored(string name) => OnItemRestored?.Invoke(name);

    public static void TriggerEnemyHit(GameObject enemy)
    {
        OnEnemyHit?.Invoke(enemy);
    }
}