using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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
    public int currentHealth = 100;

    [Header("References & Audio")]
    [SerializeField] private AudioClip ambienceMusic;
    [SerializeField] private AudioClip victoryMusic; 
    [SerializeField] private AudioClip gameOverMusic;

    [Header("Inventory")]
    [SerializeField] private List<string> playerInventory = new List<string>();



    private bool isPaused = false;
    private bool isGameOver = false;
    private bool isVictory = false;

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
        Time.timeScale = 1; 
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }

    private void OnEnable() => Pickup.OnPickupCollected += ProcessPickup;
    private void OnDisable() => Pickup.OnPickupCollected -= ProcessPickup;

    private void ProcessPickup(Pickup pickup)
    {
        if (pickup is ClassifiedDocs)
        {
            collectedFiles++;
            EventLevel1Manager.TriggerFileCollected(collectedFiles);
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
        if (isGameOver || isVictory) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        EventLevel1Manager.TriggerHealthChanged(currentHealth);

        if (currentHealth <= 0)
        {
            isGameOver = true;
            Time.timeScale = 0;

            AudioManager.Instance.StopAmbience();
            if (gameOverMusic != null) AudioManager.Instance.PlayAmbience(gameOverMusic);

            EventLevel1Manager.TriggerGameOver();
        }
    }

    public void MissionAccomplished()
    {
        if (isGameOver || isVictory) return;

        isVictory = true;
        Time.timeScale = 0;
        AudioManager.Instance.StopAmbience();
        if (victoryMusic != null) AudioManager.Instance.PlayAmbience(victoryMusic);

        EventLevel1Manager.TriggerVictory();
    }

    public void TogglePause()
    {
        if (isGameOver || isVictory) return;

        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        EventLevel1Manager.TriggerGamePaused(isPaused);
    }

    public void Button_QuitToMainMenu()
    {
        Time.timeScale = 1;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.ChangeScene("MainMenu", 0);
        }
      
    }

    public void Button_RestartLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}