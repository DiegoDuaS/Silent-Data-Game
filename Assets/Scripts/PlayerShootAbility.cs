using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShootAbility : MonoBehaviour
{
    [Header("Shooting Settings")]
    [SerializeField] private float shootRange = 50f;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool hasGun = true;

    [Header("Input Configuration")]
    [SerializeField] private InputActionReference shootAction;

    private Camera mainCamera;

    private void OnEnable()
    {
        if (shootAction != null)
            shootAction.action.Enable();
    }

    private void OnDisable()
    {
        if (shootAction != null)
            shootAction.action.Disable();
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
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Gamepad.current != null && GameManager.Instance != null)
        {
            Gamepad.current.SetMotorSpeeds(0.2f, 0.5f);
        }
        
        RaycastHit hit;

        if (Physics.Raycast(ray.origin, ray.direction, out hit, shootRange, enemyLayer))
        {
            EventManager.TriggerEnemyHit(hit.collider.gameObject);
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
    }
}