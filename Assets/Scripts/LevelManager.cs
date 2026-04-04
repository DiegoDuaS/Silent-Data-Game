using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Progreso")]
    [SerializeField] private int documentosObjetivo = 3;
    private int documentosRecogidos = 0;
    private int nivelSeguridadActual = 0;
    private bool tieneTelefono = false;

    [Header("Sistema de Salud")]
    public int saludMaxima = 100;
    public int saludActual = 70;

    [Header("Referencias y Audio")]
    [SerializeField] private GameObject puertaSalida;
    [SerializeField] private AudioClip musicaAmbiente;
    [SerializeField] private AudioClip sfxVictoria;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (musicaAmbiente != null) AudioManager.Instance.PlayAmbience(musicaAmbiente);

        // Inicializar UI
        EventManager.TriggerHealthChanged(saludActual);
        EventManager.TriggerFileCollected(0);
        EventManager.TriggerSecurityLevelChanged(nivelSeguridadActual);
    }

    private void OnEnable() => Pickup.OnPickupCollected += ProcesarEntrada;
    private void OnDisable() => Pickup.OnPickupCollected -= ProcesarEntrada;

    private void ProcesarEntrada(Pickup pickup)
    {
        // Si es un documento
        if (pickup is ClassifiedDocs)
        {
            documentosRecogidos++;
            EventManager.TriggerFileCollected(documentosRecogidos);

            if (documentosRecogidos >= documentosObjetivo) MisionCumplida();
        }
        // Si es una Keycard
        else if (pickup is Keycard)
        {
            nivelSeguridadActual++;
            EventManager.TriggerSecurityLevelChanged(nivelSeguridadActual);
        }
    }

    public void ModificarSalud(int cantidad)
    {
        saludActual = Mathf.Clamp(saludActual + cantidad, 0, saludMaxima);
        EventManager.TriggerHealthChanged(saludActual);
    }

    private void MisionCumplida()
    {
        AudioManager.Instance.StopAmbience();
        if (sfxVictoria != null) AudioManager.Instance.PlaySFX(sfxVictoria);
        if (puertaSalida != null) puertaSalida.SetActive(false);
    }

    public int GetSecurityLevel() => nivelSeguridadActual;
    public int GetFilesCollected() => documentosRecogidos;
    public bool HasPhone() => tieneTelefono;
}