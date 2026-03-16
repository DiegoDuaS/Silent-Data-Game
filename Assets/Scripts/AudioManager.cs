using UnityEngine;

public class AudioManager : MonoBehaviour
{

    private AudioSource sfxAudio => GetComponents<AudioSource>()[0];

    private AudioSource ambienceAudio => GetComponents<AudioSource>()[1];
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxAudio.PlayOneShot(clip);
    }

    public void PlayAmbience(AudioClip clip)
    {
        ambienceAudio.Stop();
        ambienceAudio.clip = clip;
        ambienceAudio.Play();
    }

    public void StopAmbience()
    {
        ambienceAudio.Stop();
    }


}
