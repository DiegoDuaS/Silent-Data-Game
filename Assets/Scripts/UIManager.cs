using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("HUD")]
    public Slider barraVidaSlider;
    public TextMeshProUGUI counterFilesTexto;
    public TextMeshProUGUI keyCardLevelTexto;
    public TextMeshProUGUI hackingStatusTexto; 

    [Header("Inventario")]
    public Transform panelInventario;
    public GameObject itemPrefab;

    private void OnEnable()
    {
        EventManager.OnHealthChanged += ActualizarBarraVida;
        EventManager.OnFileCollected += ActualizarContadorFiles;
        EventManager.OnSecurityLevelChanged += ActualizarTextoKeycard;
        EventManager.OnPhoneCollected += ActivarHUDHackeo;
        Pickup.OnPickupCollected += IntentarAñadirAInventario;
    }

    private void OnDisable()
    {
        EventManager.OnHealthChanged -= ActualizarBarraVida;
        EventManager.OnFileCollected -= ActualizarContadorFiles;
        EventManager.OnSecurityLevelChanged -= ActualizarTextoKeycard;
        EventManager.OnPhoneCollected -= ActivarHUDHackeo;
        Pickup.OnPickupCollected -= IntentarAñadirAInventario;
    }

    private void ActualizarBarraVida(int salud) => barraVidaSlider.value = salud;

    private void ActualizarContadorFiles(int total) => counterFilesTexto.text = "Files Found: " + total;

    private void ActualizarTextoKeycard(int nivel) => keyCardLevelTexto.text = "Key Card Level: " + nivel;

    private void ActivarHUDHackeo()
    {
        if (hackingStatusTexto != null)
        {
            hackingStatusTexto.gameObject.SetActive(true);
        }
    }

    private void IntentarAñadirAInventario(Pickup pickup)
    {
  
        if (!(pickup is ClassifiedDocs) && !(pickup is Keycard))
        {
            GameObject nuevoItem = Instantiate(itemPrefab, panelInventario);
            Image imagen = nuevoItem.GetComponentInChildren<Image>();

            if (imagen != null)
                imagen.sprite = pickup.Data.icon;
        }
    }
}