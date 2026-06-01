using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootAbility : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private bool hasGun = false;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference shootAction;

    [Header("Audio")]
    [SerializeField] private AudioClip shootSFX;

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
        if (shootSFX != null)
        {
            AudioManager.Instance.PlaySFX(shootSFX);
        }

        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            EventLevel1Manager.TriggerEnemyHit(hit.collider.gameObject);
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