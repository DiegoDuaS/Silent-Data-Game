using UnityEngine;
using System;

public abstract class Pickup : MonoBehaviour
{
    public static event Action<Pickup> OnPickupCollected;

    [Header("Data & Audio")]
    [SerializeField] protected PickupData data; 
    [SerializeField] protected AudioClip pickupSFX;

    public PickupData Data => data;

    public void Collect()
    {
        if (pickupSFX != null)
        {
            AudioManager.Instance.PlaySFX(pickupSFX);
        }

        Use();

        OnPickupCollected?.Invoke(this);

        Destroy(gameObject);
    }

    public abstract void Use();

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
}