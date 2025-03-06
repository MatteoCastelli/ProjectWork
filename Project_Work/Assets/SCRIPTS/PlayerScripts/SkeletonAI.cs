using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public enum AIState { Idle, Chasing }

    [Header("Components")]
    private NavMeshAgent agent;

    [Header("AI States")]
    [SerializeField] private AIState currentState = AIState.Idle;

    [Header("Chasing")]
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float approachRange = 10f;
    [SerializeField] private float stoppingDistance = 2f; // Distanza minima dal giocatore
    [SerializeField] private float approachSpeed = 1.5f;
    [SerializeField] private float normalSpeed = 3.5f;


    private GameObject player;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Giocatore non trovato! Assicurati che il giocatore abbia il tag 'Player'.");
            return;
        }
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case AIState.Idle:
                if (distanceToPlayer <= chaseRange)
                {
                    currentState = AIState.Chasing;
                }
                break;

            case AIState.Chasing:
                if (distanceToPlayer > stoppingDistance)
                {
                    agent.SetDestination(player.transform.position);
                }
                else
                {
                    agent.ResetPath(); // Ferma il movimento senza cambiare stato
                }
                break;

        }
    }

    private void OnDrawGizmosSelected()
    {
        // Disegna il raggio di inseguimento nel Scene View per visualizzare l'area di attivazione
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
    }
}
