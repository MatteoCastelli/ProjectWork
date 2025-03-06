using UnityEngine;

public class DeathUI : MonoBehaviour
{
    public GameObject deathScreen;  // Il pannello della schermata di morte
    public GameObject respawnButton;  // Il bottone di respawn

    private void Start()
    {
        deathScreen.SetActive(false);  // La schermata di morte è nascosta all'inizio
        respawnButton.SetActive(false);  // Il bottone di respawn è nascosto all'inizio
    }

    public void ShowDeathScreen()
    {
        deathScreen.SetActive(true);  // Mostra la schermata di morte
        respawnButton.SetActive(true);  // Mostra il bottone di respawn
    }

    public void HideDeathScreen()
    {
        deathScreen.SetActive(false);  // Nascondi la schermata di morte
        respawnButton.SetActive(false);  // Nascondi il bottone di respawn
    }

    public void OnRespawnButtonClicked()
    {
        // Respawn logica (potresti fare chiamare respawn al giocatore)
        GameObject.Find("Player").GetComponent<PlayerHealth>().Respawn();
    }
}
