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

    private float inputX = 0f;
    private float inputY = 0f;
    private Vector3 rotacionInicial;

    private void Awake()
    {
        rotacionInicial = transform.localEulerAngles;
    }

    public void ActivarControlManual(bool activar)
    {
        estaSiendoControlada = activar;

        if (activar)
        {
            inputX = 0f;
            inputY = 0f;

            if (lookAction != null)
                lookAction.action.Enable();
        }
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

        Vector2 inputMirar = lookAction.action.ReadValue<Vector2>();

        inputY += inputMirar.x * sensibilidadCama;
        inputX += inputMirar.y * sensibilidadCama;

        inputX = Mathf.Clamp(inputX, -maxAnguloVertical, maxAnguloVertical);
        inputY = Mathf.Clamp(inputY, -maxAnguloHorizontal, maxAnguloHorizontal);

        transform.localRotation = Quaternion.Euler(rotacionInicial.x + inputX, rotacionInicial.y + inputY, rotacionInicial.z);
    }

    private void EscanearEntorno()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, rangoEscaneo, capaObjetosOcultos))
        {
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