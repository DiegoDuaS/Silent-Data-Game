using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ArmedGuardBehaviour : MonoBehaviour
{
    public enum GuardState { Guarding, Suspicious, Found }

    [Header("General Settings")]
    [SerializeField] GuardState currentState = GuardState.Guarding;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float waitTimeAtWaypoint = 5.0f;
    [SerializeField] Animator anim;

    [Header("Detection (Sound & Vision)")]
    [SerializeField] Transform player;
    [SerializeField] float hearingRadius = 8f;
    [SerializeField] float viewDistance = 15f;
    [SerializeField] float viewAngle = 90f;
    [SerializeField] LayerMask obstructionMask;

    [Header("Timers")]
    [SerializeField] float reactionIdleTime = 3.0f;
    [SerializeField] float searchTimeAtLocation = 6.0f;
    [SerializeField] float timeToDetect = 3.0f;

    [Header("Combat & Speed Settings")]
    [SerializeField] float shootingDistance = 5.0f;
    [SerializeField] float chaseSpeed = 6.0f;
    [SerializeField] float suspiciousSpeed = 1.5f;
    private float patrolSpeed;

    [Header("UI Elements")]
    [SerializeField] GameObject questionMarkUI;
    [SerializeField] GameObject exclamationMarkUI;

    private int wpIndex = 0;
    private bool isWaiting = false;
    private bool isInvestigating = false;
    private float detectionTimer = 0f;
    private Vector3 lastKnownPosition;
    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    void Start()
    {
        patrolSpeed = agent.speed;
        SetDestinationToWaypoint();
        if (exclamationMarkUI) exclamationMarkUI.SetActive(false);
    }

    void Update()
    {
        anim.SetFloat("Speed", agent.velocity.magnitude);

        HandleUIBillboard();
        CheckFieldOfView();

        switch (currentState)
        {
            case GuardState.Guarding:
                CheckForNoise();
                HandleGuarding();
                break;
            case GuardState.Suspicious:
                CheckForNoise();
                break;
            case GuardState.Found:
                HandleFoundState();
                break;
        }
    }

    void CheckFieldOfView()
    {
        if (currentState == GuardState.Found) return;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= viewDistance && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstructionMask))
            {
                detectionTimer += Time.deltaTime;
                if (detectionTimer >= timeToDetect)
                {
                    StartFoundState();
                }
                return;
            }
        }

        if (detectionTimer > 0)
        {
            detectionTimer = Mathf.Max(0, detectionTimer - Time.deltaTime);
        }
    }

    void StartFoundState()
    {
        StopAllCoroutines();
        currentState = GuardState.Found;
        agent.speed = chaseSpeed;

        anim.SetBool("isFound", true);
        anim.SetBool("isSuspicious", false);

        if (questionMarkUI) questionMarkUI.SetActive(false);
        if (exclamationMarkUI) exclamationMarkUI.SetActive(true);

        isInvestigating = false;
    }

    void HandleFoundState()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        bool canSeePlayer = false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        if (distanceToPlayer <= viewDistance && Vector3.Angle(transform.forward, directionToPlayer) < viewAngle / 2)
        {
            if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstructionMask))
            {
                canSeePlayer = true;
                lastKnownPosition = player.position;
            }
        }

        if (!canSeePlayer)
        {
            StopFoundState();
            return;
        }

        if (distanceToPlayer > shootingDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            Vector3 lookDir = (player.position - transform.position).normalized;
            lookDir.y = 0;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 10f);
        }
    }

    void StopFoundState()
    {
        currentState = GuardState.Suspicious;
        anim.SetBool("isFound", false);
        anim.SetBool("isSuspicious", true);

        if (exclamationMarkUI) exclamationMarkUI.SetActive(false);
        if (questionMarkUI) questionMarkUI.SetActive(true);

        detectionTimer = 0f;
        isInvestigating = false;

        StopAllCoroutines();
        StartCoroutine(SuspiciousSequence());
    }

    void CheckForNoise()
    {
        bool isPlayerSneaking = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isPlayerSneaking && distanceToPlayer <= hearingRadius)
        {
            lastKnownPosition = player.position;
            if (!isInvestigating)
            {
                StopAllCoroutines();
                isWaiting = false;
                StartCoroutine(SuspiciousSequence());
            }
        }
    }

    System.Collections.IEnumerator SuspiciousSequence()
    {
        isInvestigating = true;
        currentState = GuardState.Suspicious;

        agent.speed = suspiciousSpeed;
        anim.SetBool("isSuspicious", true);
        if (questionMarkUI) questionMarkUI.SetActive(true);
        agent.isStopped = true;
        agent.velocity = Vector3.zero;

        yield return new WaitForSeconds(reactionIdleTime);

        agent.isStopped = false;
        agent.SetDestination(lastKnownPosition);

        while (agent.pathPending || agent.remainingDistance > 0.5f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(searchTimeAtLocation);

        agent.speed = patrolSpeed;

        if (questionMarkUI) questionMarkUI.SetActive(false);
        anim.SetBool("isSuspicious", false);
        currentState = GuardState.Guarding;
        isInvestigating = false;
        SetDestinationToWaypoint();
    }

    void HandleGuarding()
    {
        if (!isWaiting && !agent.pathPending && agent.remainingDistance < 0.5f)
        {
            StartCoroutine(WaitAtWaypoint());
        }
    }

    System.Collections.IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTimeAtWaypoint);
        if (currentState == GuardState.Guarding && waypoints.Count > 0)
        {
            wpIndex = (wpIndex + 1) % waypoints.Count;
            SetDestinationToWaypoint();
        }
        isWaiting = false;
    }

    void SetDestinationToWaypoint()
    {
        if (waypoints.Count > 0 && agent.isOnNavMesh)
        {
            agent.speed = patrolSpeed;
            agent.isStopped = false;
            agent.SetDestination(waypoints[wpIndex].position);
        }
    }

    void HandleUIBillboard()
    {
        GameObject activeUI = exclamationMarkUI.activeSelf ? exclamationMarkUI : (questionMarkUI.activeSelf ? questionMarkUI : null);

        if (activeUI != null)
        {
            Vector3 dir = Camera.main.transform.position - activeUI.transform.position;
            activeUI.transform.rotation = Quaternion.LookRotation(-dir);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);

        Gizmos.color = Color.red;
        Vector3 forward = transform.forward;
        Vector3 leftBoundary = Quaternion.Euler(0, -viewAngle / 2, 0) * forward;
        Vector3 rightBoundary = Quaternion.Euler(0, viewAngle / 2, 0) * forward;

        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + leftBoundary * viewDistance);
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + rightBoundary * viewDistance);
        Gizmos.DrawWireSphere(transform.position, viewDistance);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootingDistance);
    }

    public void AlertFromDrone(Vector3 position)
    {
        lastKnownPosition = position;

        if (currentState != GuardState.Found)
        {
            StopAllCoroutines();
            isWaiting = false;
            isInvestigating = false;
            StartCoroutine(SuspiciousSequence());
        }
    }
}