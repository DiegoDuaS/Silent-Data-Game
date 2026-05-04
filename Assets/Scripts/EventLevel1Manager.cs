using System;
using UnityEngine;

public class EventLevel1Manager : MonoBehaviour
{
    public static event Action<bool> OnPlayerAtExitRange;

    public static void PlayerAtExitRange(bool isAtRange)
    {
        OnPlayerAtExitRange?.Invoke(isAtRange);
    }
}