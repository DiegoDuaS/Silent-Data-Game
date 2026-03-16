using UnityEngine;

public class DoorSecurity : MonoBehaviour
{
    [Header("Configuración de Seguridad")]
    [SerializeField] private int levelKeycardNecessary = 1;
    [SerializeField] private int currentLevelKeycard = 0;

    [Header("Movimiento")]
    [SerializeField] private float distanciaDescenso = 3.5f;
    [SerializeField] private float velocidad = 5f;
    private Vector3 posicionCerrada;
    private Vector3 posicionAbierta;

    [Header("Estado")]
    [SerializeField] private bool estaAbierta = false;
    [SerializeField] private bool jugadorCerca = false;

    private void Start()
    {
        posicionCerrada = transform.position;
        // Ahora restamos en Y para que baje
        posicionAbierta = posicionCerrada + (Vector3.down * distanciaDescenso);
    }

    private void OnEnable() => EventManager.OnSecurityLevelChanged += ActualizarNivelSeguridad;
    private void OnDisable() => EventManager.OnSecurityLevelChanged -= ActualizarNivelSeguridad;

    private void ActualizarNivelSeguridad(int nuevoNivel)
    {
        currentLevelKeycard = nuevoNivel;
        VerificarAcceso();
    }

    void Update()
    {
        Vector3 objetivo = estaAbierta ? posicionAbierta : posicionCerrada;

        // Solo movemos si la distancia es considerable para evitar el "jitter" o bug
        if (Vector3.Distance(transform.position, objetivo) > 0.001f)
        {
            transform.position = Vector3.Lerp(transform.position, objetivo, Time.deltaTime * velocidad);
        }
        else
        {
            // Forzamos la posición final para que se quede quieta exactamente en el punto
            transform.position = objetivo;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
            VerificarAcceso();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
            estaAbierta = false;
        }
    }

    private void VerificarAcceso()
    {
        if (jugadorCerca && currentLevelKeycard >= levelKeycardNecessary)
        {
            estaAbierta = true;
        }
    }
}