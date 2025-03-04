using System;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInput;
    public static Action reloadInput;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    [SerializeField] private GameObject crosshair; // Aggiunto il riferimento al mirino

    private void Update()
    {
        // Input per sparare
        if (Input.GetMouseButton(0)) // Tasto sinistro del mouse
            shootInput?.Invoke();

        // Input per ricaricare
        if (Input.GetKeyDown(reloadKey)) // Tasto "R" per ricaricare
            reloadInput?.Invoke();

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
