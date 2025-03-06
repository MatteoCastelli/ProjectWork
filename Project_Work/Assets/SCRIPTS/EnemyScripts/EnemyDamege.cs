using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public float detectionRadius = 2.0f;  // Raggio di rilevamento
    public int damageAmount = 10;  // Quantità di danno inflitto
    private GameObject player;  // Riferimento al giocatore

    public float damageInterval = 1.0f;  // Intervallo tra un danno e l'altro
    private float lastDamageTime = 0.0f;  // Ultimo momento in cui è stato inflitto danno

    // Inizializzazione
    private void Start()
    {
        // Troviamo il giocatore usando il tag "Player"
        player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
        {
            Debug.LogError("Player non trovato. Assicurati di aver impostato correttamente il tag 'Player'.");
        }
    }

    // Aggiornamento per rilevare il giocatore e infliggere danno
    private void Update()
    {
        if (player == null) return; // Evitiamo errori se il giocatore non è stato trovato

        // Calcoliamo la distanza tra il nemico e il giocatore
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // Se il giocatore è abbastanza vicino e non è stato inflitto danno troppo recentemente
        if (distanceToPlayer < detectionRadius)
        {
            if (Time.time - lastDamageTime >= damageInterval)
            {
                // Infliggi danno al giocatore
                player.GetComponent<Anim_Move>().TakeDamage(damageAmount);
                lastDamageTime = Time.time;  // Aggiorna il tempo dell'ultimo danno
            }
        }
    }
}
