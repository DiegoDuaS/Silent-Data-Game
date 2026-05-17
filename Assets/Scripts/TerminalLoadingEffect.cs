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

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference jumpAction;

    public static void ResetState()
    {
        IsReadyToTransition = false;
    }

    private void OnEnable()
    {
        if (jumpAction != null)
            jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        if (jumpAction != null)
            jumpAction.action.Disable();
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

        while (jumpAction == null || !jumpAction.action.triggered)
        {
            yield return null;
        }

        IsReadyToTransition = true;
    }

    private IEnumerator BlinkRoutine()
    {
        while (!IsReadyToTransition)
        {
            pressSpaceText.enabled = !pressSpaceText.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        pressSpaceText.enabled = true;
    }
}