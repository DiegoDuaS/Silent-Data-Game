using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootAbility : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool hasGun = true;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (hasGun && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, shootRange, enemyLayer))
        {
                EventManager.TriggerEnemyHit(hit.collider.gameObject);   
        }
        
    }

    // --- GIZMOS ---
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
    }
}