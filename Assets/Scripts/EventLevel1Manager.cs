using System;
using UnityEngine;

public class EventLevel1Manager : MonoBehaviour
{
    public static event Action<bool> OnPlayerAtExitRange;
    public static event Action<int> OnHealthChanged;
    public static event Action<int> OnFileCollected;
    public static event Action<int> OnSecurityLevelChanged;
    public static event Action OnPhoneCollected;
    public static event Action OnGunCollected;
    public static event Action OnCheckpointReached;
    public static event Action<GameObject> OnEnemyHit;
    public static event Action<string> OnItemRestored;
    public static event Action<string> OnObjectDiscovered;
    public static event Action OnGameOver;
    public static event Action OnVictory;
    public static event Action<bool> OnGamePaused;

    public static void TriggerGameOver() => OnGameOver?.Invoke();
    public static void TriggerVictory() => OnVictory?.Invoke();
    public static void TriggerGamePaused(bool isPaused) => OnGamePaused?.Invoke(isPaused);
    public static void TriggerHealthChanged(int currentHealth) => OnHealthChanged?.Invoke(currentHealth);
    public static void TriggerFileCollected(int currentTotal) => OnFileCollected?.Invoke(currentTotal);
    public static void TriggerSecurityLevelChanged(int newLevel) => OnSecurityLevelChanged?.Invoke(newLevel);
    public static void TriggerPhoneCollected() => OnPhoneCollected?.Invoke();

    public static void TriggerGunCollected() => OnGunCollected?.Invoke();
    public static void TriggerCheckpointReached() => OnCheckpointReached?.Invoke();

    public static void TriggerItemRestored(string name) => OnItemRestored?.Invoke(name);

    public static void TriggerEnemyHit(GameObject enemy)
    {
        OnEnemyHit?.Invoke(enemy);
    }

    public static void PlayerAtExitRange(bool isAtRange)
    {
        OnPlayerAtExitRange?.Invoke(isAtRange);
    }
    public static void TriggerObjectDiscovered(string objectName)
    {
        OnObjectDiscovered?.Invoke(objectName);
    }
}