using UnityEngine;
using System;

public abstract class Pickup : MonoBehaviour
{
    public static event Action<Pickup> OnPickupCollected;

    [Header("Data & Audio")]
    [SerializeField] protected PickupData data;
    [SerializeField] protected AudioClip pickupSFX;

    [Header("Visibility System")]
    [SerializeField] protected bool ocultarAlInicio = false;

    private MeshRenderer[] mallasDelObjeto;

    public bool YaFueDescubierto { get; private set; } = false;
    public PickupData Data => data;

    protected virtual void Start()
    {

        mallasDelObjeto = GetComponentsInChildren<MeshRenderer>();

        if (ocultarAlInicio)
        {
            CambiarVisibilidad(false);
            YaFueDescubierto = false;
        }
        else
        {

            YaFueDescubierto = true;
        }
    }

    public void DescubrirObjeto()
    {
        if (YaFueDescubierto) return;

        YaFueDescubierto = true;
        CambiarVisibilidad(true);

        EventLevel1Manager.TriggerObjectDiscovered(gameObject.name);
    }

    private void CambiarVisibilidad(bool visible)
    {
        if (mallasDelObjeto != null)
        {
            foreach (var malla in mallasDelObjeto)
            {
                if (malla != null) malla.enabled = visible;
            }
        }
    }

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
        if (YaFueDescubierto && other.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
}