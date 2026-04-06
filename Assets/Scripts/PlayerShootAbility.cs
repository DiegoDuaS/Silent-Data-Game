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
        // 1. Rayo desde el centro de la cámara
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        // Usamos el origen y dirección por separado para evitar errores de sobrecarga
        if (Physics.Raycast(ray.origin, ray.direction, out hit, shootRange, enemyLayer))
        {
            // Si el Gizmo se pone verde, esto DEBE ejecutarse ahora
           
                EventManager.TriggerEnemyHit(hit.collider.gameObject);
                Debug.Log("<color=red>Target Hit: </color>" + hit.collider.name);
           
        }
        else
        {
            Debug.Log("<color=orange>No enemy in sight.</color>");
        }

        Debug.Log("Bang! Shot fired.");
    }

    // --- GIZMOS ---
    private void OnDrawGizmos()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (mainCamera == null) return;

        Vector3 centerPoint = new Vector3(0.5f, 0.5f, 0);
        Ray ray = mainCamera.ViewportPointToRay(centerPoint);
        RaycastHit hit;

        // 1. SHOOT LINE GIZMO
        // Changes color to green if an enemy is within range/aim
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

        // 2. RANGE AREA GIZMO
        // Draws a sphere around the player to show the total 360° reach
        Gizmos.color = new Color(1, 0, 0, 0.2f); // Transparent red
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }

    public void EquipGun()
    {
        hasGun = true;
        Debug.Log("<color=yellow>[SYSTEM]: Pistol equipped.</color>");
    }
}