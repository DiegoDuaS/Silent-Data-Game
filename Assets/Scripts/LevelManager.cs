using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Progress")]
    [SerializeField] private int targetFiles = 3;
    private int collectedFiles = 0;
    private int currentSecurityLevel = 0;
    private bool hasPhone = false;

    [Header("Health System")]
    public int maxHealth = 100;
    public int currentHealth = 70;

    [Header("References & Audio")]
    [SerializeField] private AudioClip ambienceMusic;
    [SerializeField] private AudioClip victorySFX;

    [Header("Inventory")]
    [SerializeField] private List<string> playerInventory = new List<string>();

    private void Awake()
    {
  
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (ambienceMusic != null) AudioManager.Instance.PlayAmbience(ambienceMusic);
    }

    private void OnEnable() => Pickup.OnPickupCollected += ProcessPickup;
    private void OnDisable() => Pickup.OnPickupCollected -= ProcessPickup;

    private void ProcessPickup(Pickup pickup)
    {
        if (pickup is ClassifiedDocs)
        {
            collectedFiles++;
            EventLevel1Manager.TriggerFileCollected(collectedFiles);
            if (collectedFiles >= targetFiles) MissionAccomplished();
        }
        else if (pickup is Keycard)
        {
            currentSecurityLevel++;
            EventLevel1Manager.TriggerSecurityLevelChanged(currentSecurityLevel);
        }
        else if (pickup is Phone || pickup is Gun)
        {
            AddToInventory(pickup.Data.itemName);
        }

    }

    public void AddToInventory(string itemName)
    {


        if (!playerInventory.Contains(itemName))
        {
            playerInventory.Add(itemName);

    
            if (itemName == "Celphone")
            {
                hasPhone = true;
                EventLevel1Manager.TriggerPhoneCollected();
            }
        }
        
    }

    public List<string> GetInventory() => playerInventory;

    public void SetInventory(List<string> savedItems)
    {
        Debug.Log($"<color=magenta>[LevelManager]: Restoring {savedItems.Count} items from Save File...</color>");

        playerInventory.Clear();
        foreach (string id in savedItems)
        {
            AddToInventory(id);
        }
    }

    public int GetSecurityLevel() => currentSecurityLevel;
    public void SetSecurityLevel(int value) => currentSecurityLevel = value;

    public int GetFilesCollected() => collectedFiles;
    public void SetFilesCollected(int value) => collectedFiles = value;

    public bool HasPhone() => hasPhone;

    public void ModifyHealth(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        EventLevel1Manager.TriggerHealthChanged(currentHealth);
    }

    private void MissionAccomplished()
    {
        AudioManager.Instance.StopAmbience();
        if (victorySFX != null) AudioManager.Instance.PlaySFX(victorySFX);
        GameManager.Instance.ChangeScene("Level3", 2);
    }
}