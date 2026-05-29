using UnityEngine;
using UnityEngine.InputSystem;
using StarterAssets;

public class PlayerHackAbility : MonoBehaviour
{
    [Header("Hacking Settings")]
    [SerializeField] private bool puedeHackear = false;
    [SerializeField] private float hackRange = 30f;
    [SerializeField] private LayerMask capasHackeables;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference hackAction;

    private Camera mainCamera;
    private Camera currentHackedCamera;
    private bool isHackingView = false;
    private ThirdPersonController playerController;

    private void OnEnable()
    {
        EventLevel1Manager.OnPhoneCollected += ActivarHackeo;

        if (hackAction != null)
            hackAction.action.Enable();
    }

    private void OnDisable()
    {
        EventLevel1Manager.OnPhoneCollected -= ActivarHackeo;

        if (hackAction != null)
            hackAction.action.Disable();
    }

    private void Start()
    {
        mainCamera = Camera.main;
        playerController = GetComponent<ThirdPersonController>();
    }

    private void ActivarHackeo()
    {
        puedeHackear = true;
    }

    void Update()
    {
        if (puedeHackear && hackAction != null && hackAction.action.triggered)
        {
            if (isHackingView)
            {
                ExitHackedView();
            }
            else
            {
                IntentarHackear();
            }
        }
    }

    private void IntentarHackear()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Debug.Log("<color=orange>[Hack] 1. Botón presionado. Lanzando Raycast...</color>");

        if (Physics.Raycast(ray, out hit, hackRange, capasHackeables))
        {
            Debug.Log($"<color=yellow>[Hack] 2. Impacto: </color><b>{hit.collider.gameObject.name}</b> | <color=yellow>Tag inicial: </color><b>{hit.collider.tag}</b>");

            Transform currentTransform = hit.collider.transform;
            bool esCamara = false;
            Transform objetoConTag = null;

            while (currentTransform != null)
            {
                if (currentTransform.CompareTag("Camera"))
                {
                    esCamara = true;
                    objetoConTag = currentTransform;
                    break;
                }
                currentTransform = currentTransform.parent;
            }

            if (esCamara)
            {
                // --- FIX 1: Agregar 'true' para buscar también en objetos apagados ---
                Camera hackedCam = objetoConTag.GetComponentInChildren<Camera>(true);

                if (hackedCam != null)
                {
                    Debug.Log("<color=green>[Hack] 3. ¡Cámara hackeada con éxito!</color>");
                    EnterHackedView(hackedCam);
                }
                else
                {
                    Debug.LogWarning("[Hack] ERROR: El objeto tiene el Tag 'Camera', pero no se encontró el componente Camera de Unity.");
                }
            }
            else
            {
                Debug.Log($"[Hack] 3. Objeto inválido. Se chocó contra '{hit.collider.gameObject.name}'.");
            }
        }
        else
        {
            Debug.Log("<color=gray>[Hack] Raycast en el vacío absoluto.</color>");
        }
    }

    private void EnterHackedView(Camera hackedCam)
    {
        currentHackedCamera = hackedCam;

        mainCamera.gameObject.SetActive(false);
        currentHackedCamera.gameObject.SetActive(true);

        if (playerController != null)
            playerController.enabled = false;

        HackedCameraControl controlCamara = hackedCam.GetComponent<HackedCameraControl>();
        if (controlCamara != null)
        {
            controlCamara.ActivarControlManual(true);
        }

        isHackingView = true;
        Debug.Log("<color=cyan>[Sistema] Conexión remota establecida.</color>");
    }

    private void ExitHackedView()
    {
        if (currentHackedCamera != null)
        {
            HackedCameraControl controlCamara = currentHackedCamera.GetComponent<HackedCameraControl>();
            if (controlCamara != null)
            {
                controlCamara.ActivarControlManual(false);
            }

            currentHackedCamera.gameObject.SetActive(false);
            currentHackedCamera = null;
        }

        if (mainCamera != null)
            mainCamera.gameObject.SetActive(true);

        if (playerController != null)
            playerController.enabled = true;

        isHackingView = false;
        Debug.Log("<color=yellow>[Sistema] Desconectado de la red de cámaras.</color>");
    }



    private void OnDrawGizmos()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 centerPoint = new Vector3(0.5f, 0.5f, 0);
        Ray ray = mainCamera.ViewportPointToRay(centerPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, hackRange, capasHackeables))
        {
            bool esCamara = hit.collider.CompareTag("Camera");
            if (!esCamara && hit.collider.transform.root != null)
            {
                esCamara = hit.collider.transform.root.CompareTag("Camera");
            }

            if (esCamara)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(ray.origin, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.3f);
            }
            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(ray.origin, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.1f);
            }
        }
        else
        {
            Gizmos.color = Color.gray;
            Gizmos.DrawRay(ray.origin, ray.direction * hackRange);
        }
    }
}