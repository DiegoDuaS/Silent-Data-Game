using UnityEngine;
using UnityEngine.InputSystem;

public class HackedCameraControl : MonoBehaviour
{
    [Header("Input Control")]
    [SerializeField] private InputActionReference lookAction;
    [SerializeField] private float sensibilidadCama = 0.5f;

    [Header("Rotation Limits (Ángulos Locales)")]
    [SerializeField] private float maxAnguloHorizontal = 45f;
    [SerializeField] private float maxAnguloVertical = 30f;

    [Header("Escáner de Objetos")]
    [SerializeField] private float rangoEscaneo = 40f;
    [SerializeField] private LayerMask capaObjetosOcultos;

    private bool estaSiendoControlada = false;
    private float rotacionX = 0f;
    private float rotacionY = 0f;

    private void Start()
    {
        rotacionX = transform.localEulerAngles.x;
        rotacionY = transform.localEulerAngles.y;
    }

    public void ActivarControlManual(bool activar)
    {
        estaSiendoControlada = activar;

        if (activar && lookAction != null)
            lookAction.action.Enable();
    }

    private void Update()
    {
        if (!estaSiendoControlada) return;

        ControlarRotacion();
        EscanearEntorno();
    }

    private void ControlarRotacion()
    {
        if (lookAction == null) return;

        // Leemos el delta del mouse o stick de forma cruda
        Vector2 inputMirar = lookAction.action.ReadValue<Vector2>();

        // Calculamos la nueva rotación deseada
        rotacionY += inputMirar.x * sensibilidadCama;
        rotacionX += inputMirar.y * sensibilidadCama; 

        // Ponemos límites estrictos (Clamps) para evitar rotaciones irreales
        rotacionX = Mathf.Clamp(rotacionX, -maxAnguloVertical, maxAnguloVertical);
        rotacionY = Mathf.Clamp(rotacionY, -maxAnguloHorizontal, maxAnguloHorizontal);

        // Aplicamos la rotación local con respecto a su base fija
        transform.localRotation = Quaternion.Euler(rotacionX, rotacionY, 0f);
    }

    private void EscanearEntorno()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rangoEscaneo, capaObjetosOcultos))
        {
            // Ahora buscamos la clase base Pickup
            Pickup objetoOculto = hit.collider.GetComponentInParent<Pickup>();
            
            if (objetoOculto != null && !objetoOculto.YaFueDescubierto)
            {
                objetoOculto.DescubrirObjeto();
                
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = estaSiendoControlada ? Color.green : Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * rangoEscaneo);
    }
}