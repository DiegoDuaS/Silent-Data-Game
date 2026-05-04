using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class TerminalLoadingEffect : MonoBehaviour
{
    public static bool IsReadyToTransition { get; private set; }

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI terminalText;
    [SerializeField] private TextMeshProUGUI pressSpaceText;

    [Header("Settings")]
    [SerializeField] private float typingSpeed = 0.05f;
    [SerializeField] private float blinkInterval = 0.5f;

    public static void ResetState()
    {
        IsReadyToTransition = false;
    }

    private void Start()
    {
        IsReadyToTransition = false;
        terminalText.text = "";

        if (pressSpaceText != null)
            pressSpaceText.gameObject.SetActive(false);

        StartCoroutine(TypeRoutine());
    }

    private IEnumerator TypeRoutine()
    {
        string[] messagesToDisplay = GameManager.Instance.CurrentLoadingText;

        if (messagesToDisplay == null || messagesToDisplay.Length == 0)
        {
            messagesToDisplay = new string[] { "> LOADING...", "> SYSTEM READY." }; 
        }

        foreach (string line in messagesToDisplay)
        {
            terminalText.text += "\n";
            foreach (char c in line)
            {
                terminalText.text += c;
                yield return new WaitForSeconds(typingSpeed);
            }
            yield return new WaitForSeconds(0.3f);
        }

        if (pressSpaceText != null)
        {
            pressSpaceText.gameObject.SetActive(true);
            StartCoroutine(BlinkRoutine());
        }

        while (Keyboard.current == null || !Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            yield return null;
        }

        IsReadyToTransition = true;
    }

    // Corrutina para el efecto de parpadeo (tintileo)
    private IEnumerator BlinkRoutine()
    {
        while (!IsReadyToTransition)
        {
            // Alternamos la opacidad o simplemente activamos/desactivamos
            pressSpaceText.enabled = !pressSpaceText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        // Al terminar, nos aseguramos de que el texto sea visible
        pressSpaceText.enabled = true;
    }
}