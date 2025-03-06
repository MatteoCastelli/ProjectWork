using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;   // Salute massima
    private int currentHealth;     // Salute attuale
    public Vector3 spawnPoint;     // Punto di spawn del giocatore
    public DeathUI deathUI;        // Riferimento allo script per la UI di morte

    private void Start()
    {
        currentHealth = maxHealth; // Imposta la salute iniziale
        spawnPoint = transform.position;  // Imposta il punto di spawn iniziale
        deathUI.gameObject.SetActive(false); // Nascondi la schermata di morte all'inizio
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Riduce la salute

        if (currentHealth <= 0)
        {
            Die(); // Chiamare la funzione di morte quando la salute arriva a 0
        }
    }

    private void Die()
    {
        deathUI.ShowDeathScreen();  // Mostra la schermata di morte
        // Puoi anche chiamare altre funzioni per fermare il movimento del giocatore o altre azioni se necessario
    }

    public void Respawn()
    {
        transform.position = spawnPoint;  // Riposiziona il giocatore al punto di spawn
        currentHealth = maxHealth;       // Ripristina la salute
        deathUI.gameObject.SetActive(false);  // Nascondi la schermata di morte
    }
}
