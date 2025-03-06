using UnityEngine;
using UnityEngine.AI;

public class EnemyAnimationController : MonoBehaviour
{
    private Animator animator;
    private NavMeshAgent agent;

    [Header("Movement Parameters")]
    [SerializeField] private float chaseRange = 10f;
    private GameObject player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        // Calcola la velocità del nemico
        float speed = agent.velocity.magnitude;

        // Imposta il parametro Speed nell'Animator per il Blend Tree
        animator.SetFloat("Speed", speed);

        // Controlla la distanza dal giocatore e fai inseguire il nemico
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= chaseRange)
        {
            // Il nemico insegue il giocatore
            agent.SetDestination(player.transform.position);
        }
        else
        {
            // Il nemico sta fermo o pattuglia
            agent.SetDestination(transform.position);
        }
    }
}
