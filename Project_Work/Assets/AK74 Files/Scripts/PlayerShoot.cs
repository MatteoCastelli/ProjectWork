using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action reloadInput;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private GameObject crosshair;

    private void Update()
    {
        // Debug per il tasto sinistro del mouse
        if (Input.GetMouseButton(0)) // Tasto sinistro del mouse
        {
            Debug.Log("Tasto sinistro premuto!"); // Aggiungi il debug qui per tracciare l'input
            shootInput?.Invoke();
        }

        if (Input.GetKeyDown(reloadKey)) // Tasto "R" per ricaricare
        {
            Debug.Log("Tasto ricarica premuto!");
            reloadInput?.Invoke();
        }

        // Gestisci visibilità del mirino (crosshair)
        if (Input.GetMouseButton(1)) // Tasto destro del mouse per mirare
        {
            if (crosshair != null)
                crosshair.SetActive(false); // Nasconde il mirino
        }
        else
        {
            if (crosshair != null)
                crosshair.SetActive(true); // Mostra il mirino
        }
    }
}
