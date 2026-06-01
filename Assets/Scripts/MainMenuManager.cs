using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioClip mainMenuMusic;

    void Start()
    {
        Time.timeScale = 1f;

        if (mainMenuMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayAmbience(mainMenuMusic);
        }
    }
}