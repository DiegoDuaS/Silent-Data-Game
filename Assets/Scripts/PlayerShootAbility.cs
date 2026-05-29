using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootAbility : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask enemyLayer;

    // 1. Lo ponemos en 'false' para que el jugador empiece desarmado
    [SerializeField] private bool hasGun = false;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference shootAction;

    private Camera mainCamera;

    private void OnEnable()
    {
        if (shootAction != null)
            shootAction.action.Enable();
        EventLevel1Manager.OnGunCollected += EquipGun;
    }

    private void OnDisable()
    {
        if (shootAction != null)
            shootAction.action.Disable();
        EventLevel1Manager.OnGunCollected -= EquipGun;
    }

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (hasGun && shootAction != null && shootAction.action.triggered)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Debug.Log("<color=orange>[Shoot] 1. Gatillo presionado. Disparando...</color>");

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            Debug.Log($"<color=green>[Shoot] 2. ¡Le diste a un enemigo! Objeto: {hit.collider.gameObject.name}</color>");
            EventLevel1Manager.TriggerEnemyHit(hit.collider.gameObject);
        }
        else
        {
            Debug.Log("<color=gray>[Shoot] 2. Fallaste. La bala no tocó nada en la EnemyLayer.</color>");
        }
    }


    private void OnDrawGizmos()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 centerPoint = new Vector3(0.5f, 0.5f, 0);
        Ray ray = mainCamera.ViewportPointToRay(centerPoint);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(ray.origin, hit.point);
            Gizmos.DrawWireSphere(hit.point, 0.3f);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * shootRange);
        }

        Gizmos.color = new Color(1, 0, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    public void EquipGun()
    {
        hasGun = true;
        Debug.Log("<color=cyan>[Player] ¡Pistola equipada mediante evento!</color>");
    }
}