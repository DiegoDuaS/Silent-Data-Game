using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    [Header("HUD Elements")]
    public Slider healthBarSlider;
    public TextMeshProUGUI filesCounterText;
    public TextMeshProUGUI keyCardLevelText;
    public TextMeshProUGUI hackingStatusText;

    [Header("Inventory Settings")]
    public Transform inventoryPanel;
    public GameObject itemPrefab;

    [Header("Database")]
    [SerializeField] private List<PickupData> itemCatalog;

    private void OnEnable()
    {
        EventManager.OnHealthChanged += UpdateHealthBar;
        EventManager.OnFileCollected += UpdateFilesCounter;
        EventManager.OnSecurityLevelChanged += UpdateKeyCardText;
        EventManager.OnPhoneCollected += HandlePhoneInventoryUI;
        Pickup.OnPickupCollected += HandlePhysicalPickup;
    }

    private void OnDisable()
    {
        EventManager.OnHealthChanged -= UpdateHealthBar;
        EventManager.OnFileCollected -= UpdateFilesCounter;
        EventManager.OnSecurityLevelChanged -= UpdateKeyCardText;
        EventManager.OnPhoneCollected -= HandlePhoneInventoryUI;
        Pickup.OnPickupCollected -= HandlePhysicalPickup;
    }

    private void UpdateHealthBar(int health) { if (healthBarSlider != null) healthBarSlider.value = health; }
    private void UpdateFilesCounter(int total) { if (filesCounterText != null) filesCounterText.text = "Files Found: " + total; }
    private void UpdateKeyCardText(int level) { if (keyCardLevelText != null) keyCardLevelText.text = "Key Card Level: " + level; }

    private void HandlePhysicalPickup(Pickup pickup)
    {
        if (!(pickup is Phone) && !(pickup is ClassifiedDocs) && !(pickup is Keycard) && !(pickup is Medkit) && !(pickup is USB))
        {
            CreateInventoryIcon(pickup.Data);
        }
    }

    private void HandlePhoneInventoryUI()
    {
        if (hackingStatusText != null) hackingStatusText.gameObject.SetActive(true);
        PickupData phoneData = itemCatalog.Find(d => d.itemName == "Celphone");

        if (phoneData != null)
        {
            CreateInventoryIcon(phoneData);
        }
        else
        {
            Debug.LogWarning("[UIManager]: Could not find 'Celphone' in the Item Catalog!");
        }
    }

    private void CreateInventoryIcon(PickupData data)
    {
        string uiName = data.itemName + "_UI_Icon";
        if (inventoryPanel.Find(uiName) != null) return;

        GameObject newItem = Instantiate(itemPrefab, inventoryPanel);
        newItem.name = uiName;

        Image img = newItem.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = data.icon;
        }
    }
}