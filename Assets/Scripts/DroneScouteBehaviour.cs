using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;

public class DroneScoutBehaviour : MonoBehaviour
{
    public enum DroneState { Patrolling, Tracking }

    [Header("General Settings")]
    [SerializeField] DroneState currentState = DroneState.Patrolling;
    [SerializeField] List<Transform> waypoints;
    [SerializeField] float flyHeight = 3.5f;
    [SerializeField] float waitTimeAtWaypoint = 3.0f;

    [Header("Detection (Spherical)")]
    [SerializeField] Transform player;
    [SerializeField] float detectionRadius = 10f;
    [SerializeField] LayerMask obstructionMask;
    [SerializeField] float timeToAlert = 1.5f;

    [Header("Drone Visuals")]
    [SerializeField] Light scoutLight;
    [SerializeField] Color patrolColor = Color.white;
    [SerializeField] Color alertColor = Color.red;

    private int wpIndex = 0;
    private float detectionTimer = 0f;
    private bool isWaiting = false;
    private NavMeshAgent agent => GetComponent<NavMeshAgent>();

    void Start()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }

        if (scoutLight) scoutLight.color = patrolColor;
        SetDestinationToWaypoint();
    }

    void Update()
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        CheckProximity();

        if (currentState == DroneState.Patrolling)
        {
            HandlePatrol();
        }
        else if (currentState == DroneState.Tracking)
        {
            FollowPlayerDistanced();
        }
    }

    void CheckProximity()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);

        if (distToPlayer <= detectionRadius)
        {
            if (!Physics.Raycast(transform.position, (player.position - transform.position).normalized, distToPlayer, obstructionMask))
            {
                detectionTimer += Time.deltaTime;
                if (scoutLight) scoutLight.color = Color.Lerp(patrolColor, alertColor, detectionTimer / timeToAlert);

                if (detectionTimer >= timeToAlert && currentState != DroneState.Tracking)
                {
                    StartTracking();
                }
                return;
            }
        }

        if (currentState != DroneState.Tracking)
        {
            detectionTimer = Mathf.Max(0, detectionTimer - Time.deltaTime);
            if (scoutLight) scoutLight.color = patrolColor;
        }
    }

    void HandlePatrol()
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

        if (currentState == DroneState.Patrolling && waypoints.Count > 0)
        {
            wpIndex = (wpIndex + 1) % waypoints.Count;
            SetDestinationToWaypoint();
        }
        isWaiting = false;
    }

    void SetDestinationToWaypoint()
    {
        if (waypoints.Count == 0) return;
        agent.isStopped = false;
        agent.SetDestination(waypoints[wpIndex].position);
    }

    void StartTracking()
    {
        StopAllCoroutines();
        isWaiting = false;
        currentState = DroneState.Tracking;
        AlertNearbyGuards();
    }

    void FollowPlayerDistanced()
    {
        agent.isStopped = false;
        agent.SetDestination(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > detectionRadius + 5f)
        {
            currentState = DroneState.Patrolling;
            detectionTimer = 0;
            SetDestinationToWaypoint();
        }
    }

    void AlertNearbyGuards()
    {
        Collider[] nearbyGuards = Physics.OverlapSphere(transform.position, 30f);

        foreach (var g in nearbyGuards)
        {
            var armed = g.GetComponent<ArmedGuardBehaviour>();
            var melee = g.GetComponent<GuardBehaviour>();

            if (armed != null)
            {
                armed.AlertFromDrone(player.position);
            }
            else if (melee != null)
            {
                melee.AlertFromDrone(player.position);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = (detectionTimer > 0) ? Color.red : Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.DrawSphere(transform.position, 0.5f);

        if (agent != null && agent.isOnNavMesh && agent.hasPath)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, agent.destination);
        }

        if (currentState == DroneState.Tracking)
        {
            Gizmos.color = new Color(1, 0, 0, 0.1f);
            Gizmos.DrawSphere(transform.position, 30f);
        }
    }
}