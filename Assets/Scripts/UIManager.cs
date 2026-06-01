using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Modes (Canvas Panels)")]
    public GameObject gameUIPanel;
    public GameObject cameraUIPanel;

    [Header("HUD Elements")]
    public Slider healthBarSlider;

    [Header("Inventory Settings")]
    public Transform inventoryPanel;
    public GameObject itemPrefab;

    [Header("Notifications")]
    public GameObject notificationPanel;
    public TextMeshProUGUI notificationText;
    public float notificationDuration = 3f;

    [Header("Interaction Prompts")]
    public GameObject interactionPanel;
    public TextMeshProUGUI interactionText;
    private bool isCurrentlyAtExit = false;

    [Header("Menu Panels")]
    public GameObject pausePanel;
    public GameObject gameOverPanel;
    public GameObject winPanel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (interactionPanel != null) interactionPanel.SetActive(false);
        if (interactionText != null) interactionText.gameObject.SetActive(false);

        UpdateHealthBar(100);

        ToggleHackingUI(false);
        LockCursor(true);
    }

    private void OnEnable()
    {
        EventLevel1Manager.OnHealthChanged += UpdateHealthBar;
        Pickup.OnPickupCollected += HandlePhysicalPickup;
        EventLevel1Manager.OnGamePaused += HandlePauseMenu;
        EventLevel1Manager.OnGameOver += ShowGameOver;
        EventLevel1Manager.OnVictory += ShowWinScreen;
        EventLevel1Manager.OnObjectDiscovered += ShowNotification;
        EventLevel1Manager.OnPlayerAtExitRange += HandleExitPrompt;
        EventLevel1Manager.OnSecurityLevelChanged += UpdatePromptIfStandingAtExit;
    }

    private void OnDisable()
    {
        EventLevel1Manager.OnHealthChanged -= UpdateHealthBar;
        Pickup.OnPickupCollected -= HandlePhysicalPickup;
        EventLevel1Manager.OnGamePaused -= HandlePauseMenu;
        EventLevel1Manager.OnGameOver -= ShowGameOver;
        EventLevel1Manager.OnVictory -= ShowWinScreen;
        EventLevel1Manager.OnObjectDiscovered -= ShowNotification;

        EventLevel1Manager.OnPlayerAtExitRange -= HandleExitPrompt;
        EventLevel1Manager.OnSecurityLevelChanged -= UpdatePromptIfStandingAtExit;
    }

    private void HandleExitPrompt(bool inRange)
    {
        isCurrentlyAtExit = inRange;

        if (inRange)
        {
            UpdateExitText();
            if (interactionPanel != null) interactionPanel.SetActive(true);
            if (interactionText != null) interactionText.gameObject.SetActive(true);
        }
        else
        {
            if (interactionPanel != null) interactionPanel.SetActive(false);
            if (interactionText != null) interactionText.gameObject.SetActive(false);
        }
    }

    private void UpdateExitText()
    {
        if (interactionText == null) return;

        if (interactionPanel != null) interactionPanel.SetActive(true);
        if (interactionText != null) interactionText.gameObject.SetActive(true);

        if (LevelManager.Instance != null && LevelManager.Instance.GetSecurityLevel() > 0)
        {
            interactionText.text = "Press 'E' to enter";
        }
        else
        {
            interactionText.text = "You need the keycard to open the door";
        }
    }

    private void UpdatePromptIfStandingAtExit(int level)
    {
        if (isCurrentlyAtExit) UpdateExitText();
    }

    public void ToggleHackingUI(bool isHacking)
    {
        if (gameUIPanel != null) gameUIPanel.SetActive(!isHacking);
        if (cameraUIPanel != null) cameraUIPanel.SetActive(isHacking);
    }

    private void ShowNotification(string objectName)
    {
        if (notificationText != null)
        {
            StopAllCoroutines();
            StartCoroutine(DisplayNotificationRoutine(objectName));
        }
    }

    private IEnumerator DisplayNotificationRoutine(string objectName)
    {
        if (notificationPanel != null) notificationPanel.SetActive(true);
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(true);
            notificationText.text = $"[ System ] Object found: {objectName}";
        }

        yield return new WaitForSeconds(notificationDuration);

        if (notificationPanel != null) notificationPanel.SetActive(false);
        if (notificationText != null)
        {
            notificationText.text = "";
            notificationText.gameObject.SetActive(false);
        }
    }

    private void UpdateHealthBar(int health) { if (healthBarSlider != null) healthBarSlider.value = health; }

    private void HandlePhysicalPickup(Pickup pickup) { CreateInventoryIcon(pickup.Data); }

    private void CreateInventoryIcon(PickupData data)
    {
        if (data == null || data.icon == null) return;
        string uiName = data.itemName + "_UI_Icon";
        if (inventoryPanel.Find(uiName) != null) return;

        GameObject newItem = Instantiate(itemPrefab, inventoryPanel);
        newItem.name = uiName;
        Image img = newItem.GetComponentInChildren<Image>();
        if (img != null) img.sprite = data.icon;
    }

    private void HandlePauseMenu(bool isPaused)
    {
        if (pausePanel != null) pausePanel.SetActive(isPaused);
        LockCursor(!isPaused);
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        LockCursor(false);
    }

    private void ShowWinScreen()
    {
        if (interactionText != null) interactionText.gameObject.SetActive(false);
        if (winPanel != null) winPanel.SetActive(true);
        LockCursor(false);
    }

    private void LockCursor(bool lockState)
    {
        Cursor.lockState = lockState ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !lockState;
    }

   
}