using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum AIState { Idle, Patrolling, Chasing }

    [Header("Patrol")]
    [SerializeField] private Transform wayPoints;
    [SerializeField] private float waitAtPoint = 2f;
    private int currentWaypoint = 0;
    private float waitCounter;

    [Header("Components")]
    private NavMeshAgent agent;

    [Header("AI States")]
    [SerializeField] private AIState currentState = AIState.Idle;

    [Header("Chasing")]
    [SerializeField] private float chaseRange = 5f;

    [Header("Suspicious")]
    [SerializeField] private float suspiciousTime = 3f;
    private float timeSinceLastSawPlayer;

    private GameObject player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (wayPoints == null || wayPoints.childCount == 0)
        {
            Debug.LogError("Waypoints non assegnati o vuoti! Assegna un oggetto con waypoint.");
            return;
        }

        waitCounter = waitAtPoint;
        timeSinceLastSawPlayer = suspiciousTime;
    }

    private void Update()
    {
        if (wayPoints == null || wayPoints.childCount == 0)
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case AIState.Idle:
                if (waitCounter > 0)
                {
                    waitCounter -= Time.deltaTime;
                }
                else
                {
                    currentState = AIState.Patrolling;
                    agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);
                }

                if (distanceToPlayer <= chaseRange)
                {
                    currentState = AIState.Chasing;
                }
                break;

            case AIState.Patrolling:
                if (wayPoints == null) return; // Ulteriore sicurezza
                if (agent.remainingDistance <= 0.2f && !agent.pathPending)
                {
                    currentWaypoint = (currentWaypoint + 1) % wayPoints.childCount;
                    currentState = AIState.Idle;
                    waitCounter = waitAtPoint;
                }

                if (distanceToPlayer <= chaseRange)
                {
                    currentState = AIState.Chasing;
                }
                break;

            case AIState.Chasing:
                agent.SetDestination(player.transform.position);

                if (distanceToPlayer > chaseRange)
                {
                    timeSinceLastSawPlayer -= Time.deltaTime;
                    if (timeSinceLastSawPlayer <= 0)
                    {
                        currentState = AIState.Patrolling;
                        agent.SetDestination(wayPoints.GetChild(currentWaypoint).position);
                        timeSinceLastSawPlayer = suspiciousTime;
                    }
                }
                else
                {
                    timeSinceLastSawPlayer = suspiciousTime;
                }
                break;
        }
    }
}
